using Microsoft.Extensions.Configuration;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.AdHoc.DemergeConsoleApp
{
    class Program
    {
        static void Main()
        {
            List<SubmissionModel> submissions = new List<SubmissionModel>() { };

            using (StreamReader sr = new StreamReader("Merges.csv"))
            {
                string headerLine = sr.ReadLine();
                string line;
                while ((line = sr.ReadLine()) != null)
                {     
                    var values = line.Split(',');
                    submissions.Add(CreateSubmissionModel(values[0], values[1], values[2]));
                }
            }

            RunTransactions(submissions).Wait();
        }

        static async Task RunTransactions(IEnumerable<SubmissionModel> submissions)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddUserSecrets<Program>()
                .Build();

            foreach (var submission in submissions)
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(config["BaseAddress"])
                };
                string urlParameters = "?preferredIswc=" + submission.PreferredIswc + "&agency=" + submission.Agency + "&workcode=" + submission.WorkNumber.Number;

                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", config["Subscription-Key"]);

                HttpResponseMessage response = await client.DeleteAsync(urlParameters);
                if (response.IsSuccessStatusCode)
                    Console.WriteLine("Demerged {0}", submission.PreferredIswc);
                else
                {
                    var stream = response.Content.ReadAsStreamAsync().Result;
                    StreamReader reader = new StreamReader(stream);
                    string error = reader.ReadToEnd();
                    Console.WriteLine("Failed {0} Parent:{1} Agency:{2} Child:{3}", error, submission.PreferredIswc, submission.Agency, submission.WorkNumber.Number);
                }

                client.Dispose();
            }

            Console.WriteLine("Done");
        }

        static SubmissionModel CreateSubmissionModel(string preferredIswc, string agency, string workcode)
        {
            return new SubmissionModel()
            {
                PreferredIswc = preferredIswc,
                Agency = agency,
                WorkNumber = new WorkNumber()
                {
                    Number = workcode
                }
            };
        }
    }
}
