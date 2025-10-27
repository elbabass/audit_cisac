using CsvHelper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AddMissingIPIsConsoleApp
{
    internal class Program
    {
        private readonly HttpClient httpClient;
        private readonly string databricksClusterId, databricksJobId, databricksToken, databricksBaseUrl, sftpFolderName, missingIpiFilePath;
        private readonly int apiBatchSize;
        private readonly bool refreshMissingIpiData;

        public Program()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            httpClient = new HttpClient
            {
                BaseAddress = new Uri(config["IswcApi-BaseAddress"]),
                Timeout = TimeSpan.FromMinutes(10)
            };

            databricksClusterId = config["DatabricksClusterId"];
            databricksJobId = config["DatabricksJobId"];
            databricksToken = config["DatabricksToken"];
            databricksBaseUrl = config["DatabricksBaseUrl"];
            apiBatchSize = int.Parse(config["ApiBatchSize"]);
            refreshMissingIpiData = bool.Parse(config["RefreshMissingIpiData"]);
            sftpFolderName = config["SftpFolderName"];
            missingIpiFilePath = config["MissingIpiFilePath"];
        }

        static async Task Main(string[] args)
        {
            Program program = new Program();

            if (program.refreshMissingIpiData)
            {
                Console.WriteLine("Refreshing missing IPI data...");
                string runId = await program.SubmitDatabricksJobAsync();
                await program.WaitForJobCompletionAsync(runId);
            }
            else
            {
                Console.WriteLine("Skipping refresh of missing IPI data. Using existing data in ipi.missinginterestedparty delta table.");
            }

            var baseNumbers = await program.ReadCsvFileFromDBFSAsync();
            if (baseNumbers.Count > 0)
            {
                await program.AddSpecifiedIpsAsync(baseNumbers);
            }
            else
            {
                Console.WriteLine("No IPs to add.");
            }
        }

        public async Task<string> SubmitDatabricksJobAsync()
        {
            try
            {
                var jobRunConfig = new
                {
                    job_id = databricksJobId,
                    parameters = new
                    {
                        folder_name = sftpFolderName
                    }
                };

                string jsonBody = JsonConvert.SerializeObject(jobRunConfig);

                using (var client = new HttpClient())
                {
                    string endpoint = $"{databricksBaseUrl}api/2.0/jobs/run-now";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {databricksToken}");

                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(endpoint, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        dynamic responseJson = JsonConvert.DeserializeObject(responseBody);
                        string runId = responseJson.run_id;
                        Console.WriteLine($"Job run triggered successfully. Run ID: {runId}");
                        string jobRunUrl = $"{databricksBaseUrl}jobs/{databricksJobId}/runs/{runId}";
                        Console.WriteLine($"You can view the job run at: {jobRunUrl}");
                        return runId;
                    }
                    else
                    {
                        var errorBody = await response.Content.ReadAsStringAsync();
                        throw new InvalidOperationException($"Failed to trigger job. Response: {response.StatusCode}, Details: {errorBody}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task WaitForJobCompletionAsync(string runId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {databricksToken}");

                    bool isJobCompleted = false;
                    while (!isJobCompleted)
                    {
                        string endpoint = $"{databricksBaseUrl}api/2.0/jobs/runs/get?run_id={runId}";
                        var response = await client.GetAsync(endpoint);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseBody = await response.Content.ReadAsStringAsync();
                            dynamic jobStatus = JsonConvert.DeserializeObject(responseBody);

                            string lifeCycleState = jobStatus.state.life_cycle_state;
                            string resultState = jobStatus.state.result_state;

                            if (lifeCycleState == "TERMINATED")
                            {
                                isJobCompleted = true;

                                if (resultState == "SUCCESS")
                                {
                                    Console.WriteLine("Job completed successfully!");
                                }
                                else if (resultState == "FAILED")
                                {
                                    throw new InvalidOperationException("Job failed!");
                                }
                                else
                                {
                                    throw new InvalidOperationException("Job terminated with unknown result state.");
                                }
                            }
                            else
                            {
                                await Task.Delay(5000);
                            }
                        }
                        else
                        {
                            var errorBody = await response.Content.ReadAsStringAsync();
                            throw new InvalidOperationException($"Failed to get job status. Response: {response.StatusCode}, Details: {errorBody}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while waiting for the job to complete: {ex.Message}");
            }
        }

        public async Task<List<string>> ReadCsvFileFromDBFSAsync()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {databricksToken}");
                string endpoint = $"{databricksBaseUrl}/api/2.0/dbfs/read?path={missingIpiFilePath}";

                try
                {
                    var response = await client.GetAsync(endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        dynamic responseJson = JsonConvert.DeserializeObject(responseBody);

                        string base64Content = responseJson.data.ToString();
                        byte[] fileBytes = Convert.FromBase64String(base64Content);

                        string fileContent = Encoding.UTF8.GetString(fileBytes);

                        return ParseCsv(fileContent);
                    }
                    else
                    {
                        var errorBody = await response.Content.ReadAsStringAsync();
                        throw new InvalidOperationException($"Failed to read file. Response: {response.StatusCode}, Details: {errorBody}");
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error reading file: {missingIpiFilePath}. Exception: {ex.Message}");
                }
            }
        }

        public static List<string> ParseCsv(string fileContent)
        {
            var valuesList = new List<string>();

            using (var reader = new StringReader(fileContent))
            using (var csv = new CsvReader(reader))
            {
                var records = csv.GetRecords<dynamic>();

                foreach (var record in records)
                {
                    var ipBaseNumber = record.IpBaseNumber.ToString();
                    valuesList.Add(ipBaseNumber);
                }
            }

            return valuesList;
        }

        public async Task AddSpecifiedIpsAsync(List<string> baseNumbers)
        {
            int totalBatches = (int)Math.Ceiling((double)baseNumbers.Count / apiBatchSize);

            try
            {
                for (int i = 0; i < totalBatches; i++)
                {
                    var batch = baseNumbers.Skip(i * apiBatchSize).Take(apiBatchSize).ToList();

                    try
                    {
                        bool success = await SendBatchAsync(batch);
                        if (success)
                        {
                            Console.WriteLine($"Batch {i + 1}/{totalBatches} added successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Batch {i + 1}/{totalBatches} failed.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing batch {i + 1}/{totalBatches}: {ex.Message}");
                    }

                    await Task.Delay(10000); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing batches: {ex.Message}");
            }
        }

        private async Task<bool> SendBatchAsync(List<string> batch)
        {
            try
            {
                string jsonBody = JsonConvert.SerializeObject(batch);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("Api/IpiSynchronisation/AddSpecifiedIps", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Successfully added {batch.Count} IPs.");
                    return true;
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to add IPs. Status Code: {response.StatusCode}, Details: {errorBody}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while sending batch: {ex.Message}");
                return false;
            }
        }
    }
}
