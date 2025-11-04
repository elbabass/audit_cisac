using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Linq;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Services.Notifications.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.Notifications.ComosDb;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Repositories;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using Microsoft.Extensions.Configuration;
using SpanishPoint.Azure.Iswc.Framework.Extensions;

namespace SpanishPoint.Azure.Iswc.AdHoc.ReinstateTitlesConsoleApp
{
    class Program
    {

        static void Main()
        {
            var config = new ConfigurationBuilder()
              .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
              .AddUserSecrets<Program>()
              .Build();

            Console.WriteLine("START: " + DateTime.UtcNow);

            SqlConnection connection = new SqlConnection(config["ConnectionString-ISWCAzureSqlDatabase"]);

            List<string> iswcs = new List<string>() { };
            List<string> csnAgencies = new List<string>();

            using (StreamReader sr = new StreamReader("Iswcs.csv"))
            {
                string headerLine = sr.ReadLine();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var values = line.Split(',');
                    iswcs.Add($"'{values[0]}'");
                }
            }

            try
            {
                Console.WriteLine("Openning Connection...");

                connection.Open();

                Console.WriteLine("Connection successful");

                WriteLineToResultsCsv("Iswc", "AgencyWorkCode", "Title", "Agency", "CsnAgencies");

                SelectAndInsert();

                Console.WriteLine("END: " + DateTime.UtcNow);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            void SelectAndInsert()
            {
                Console.WriteLine("Select and Insert queries starting: " + DateTime.UtcNow);

                foreach (var iswcBatch in iswcs.Batch(1000))
                {
                    List<WorkInfo> workInfoList = new List<WorkInfo>();

                    StringBuilder selectQueryStringBuilder = new StringBuilder();

                    selectQueryStringBuilder.Append("SELECT i.IswcID, i.Iswc, w.WorkInfoID, t.Title, t.StandardizedTitle, t.TitleTypeID, w.IswcEligible, w.AgencyID, w.AgencyWorkCode, w.Status FROM Iswc.Iswc i ");
                    selectQueryStringBuilder.Append("JOIN Iswc.WorkInfo w ON w.IswcID = i.IswcID ");
                    selectQueryStringBuilder.Append("JOIN Iswc.Title t ON t.WorkInfoID = w.WorkInfoID ");
                    selectQueryStringBuilder.Append($"WHERE i.iswc IN ({String.Join(",", iswcBatch)})");

                    string selectSqlQuery = selectQueryStringBuilder.ToString();
                    using SqlCommand selectCommand = new SqlCommand(selectSqlQuery, connection);
                    selectCommand.CommandTimeout = 0;
                    SqlDataReader reader = selectCommand.ExecuteReader();

                    try
                    {
                        while (reader.Read())
                            workInfoList.Add(new WorkInfo()
                            {
                                Iswc = new Data.DataModels.Iswc() { Iswc1 = reader["Iswc"].ToString() },
                                IswcId = reader.GetInt64(reader.GetOrdinal("IswcID")),
                                WorkInfoId = reader.GetInt64(reader.GetOrdinal("WorkInfoID")),
                                AgencyId = reader["AgencyID"].ToString(),
                                AgencyWorkCode = reader["AgencyWorkCode"].ToString(),
                                Status = reader.GetBoolean(reader.GetOrdinal("Status")),
                                IswcEligible = reader.GetBoolean(reader.GetOrdinal("IswcEligible")),
                                Title = new List<Title>()
                                {
                                    new Title()
                                    {
                                        Title1 = reader["Title"].ToString(),
                                        StandardizedTitle = reader["StandardizedTitle"].ToString(),
                                        TitleTypeId = reader.GetInt32(reader.GetOrdinal("TitleTypeID")),
                                    }
                                }
                            });

                        reader.Close();

                        Console.WriteLine("Select batch complete: " + DateTime.UtcNow);

                        BuildAndInsertQuery(workInfoList);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                        Environment.Exit(0);
                    }

                }
            }

            void BuildAndInsertQuery(IEnumerable<WorkInfo> workInfoList)
            {
                var groupedByIswc = workInfoList.ToList().GroupBy(x => x.IswcId);
                List<string> valuesStrings = new List<string>();
                List<CsnNotifications> csnNotifications = new List<CsnNotifications>();
                List<long> workinfosToBeUpdated = new List<long>();
                SqlTransaction transaction;

                foreach (var iswc in groupedByIswc)
                {
                    var title = iswc.OrderByDescending(x => x.IswcEligible ? 1 : 0).FirstOrDefault(i => i.Title.FirstOrDefault().TitleTypeId == 2).Title.FirstOrDefault();
                    var groupByWorkInfo = iswc.GroupBy(x => x.WorkInfoId);

                    foreach (var workinfoGroup in groupByWorkInfo)
                    {
                        if (!workinfoGroup.Any(x => x.Title.FirstOrDefault().TitleTypeId == 2 && x.Status))
                        {
                            var workinfo = workinfoGroup.First();

                            if (workinfo.Status)
                            {
                                valuesStrings.Add(String.Format("(@now,@now,{0},{1},{2},'{3}','{4}',{5},{6})",
                                7, workinfo.IswcId, workinfo.WorkInfoId, title.StandardizedTitle.Replace("'", "''"), title.Title1.Replace("'", "''"), 2, 1));

                                workinfosToBeUpdated.Add(workinfo.WorkInfoId);

                                csnAgencies = groupedByIswc.FirstOrDefault(x => x.Any(i => iswc.Select(x => x.WorkInfoId).Contains(i.WorkInfoId))).Where(w => w.IswcEligible && w.Status)
                                    .Select(x => x.AgencyId).Distinct().ToList();

                                foreach (var agency in csnAgencies)
                                {
                                    csnNotifications.Add(
                                        new CsnNotifications(workinfo, agency, DateTime.UtcNow, TransactionType.CUR, "", workinfo.Iswc.Iswc1));
                                }

                                WriteLineToResultsCsv(workinfo.Iswc.Iswc1, workinfo.AgencyWorkCode, title.Title1, workinfo.AgencyId, String.Join(",", csnAgencies));
                            }
                        }
                    }
                }

                transaction = connection.BeginTransaction();

                foreach (var valuesBatch in valuesStrings.Batch(1000))
                {
                    try
                    {
                        var valueString = String.Join(",", valuesBatch);
                        StringBuilder insertQueryStringBuilder = new StringBuilder();

                        insertQueryStringBuilder.Append("INSERT INTO Iswc.Title(CreatedDate, LastModifiedDate, LastModifiedUserID, IswcID, WorkInfoID, StandardizedTitle, Title, TitleTypeID, Status) VALUES ");

                        insertQueryStringBuilder.Append(valueString);

                        var insertQueryString = insertQueryStringBuilder.ToString();

                        using SqlCommand insertTitlesCommand = new SqlCommand(insertQueryString, connection, transaction);
                        insertTitlesCommand.Parameters.AddWithValue("@now", DateTime.UtcNow);
                        insertTitlesCommand.CommandTimeout = 0;

                        insertTitlesCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                        transaction.Rollback();
                        Environment.Exit(0);
                    }
                }

                foreach (var workinfoBatch in workinfosToBeUpdated.Batch(1000))
                {
                    try
                    {
                        StringBuilder updateQueryStringBuilder = new StringBuilder();

                        updateQueryStringBuilder.Append($"UPDATE Iswc.WorkInfo SET LastModifiedDate = @now ");
                        updateQueryStringBuilder.Append($"WHERE WorkInfoID IN({String.Join(",", workinfoBatch)})");

                        var insertQueryString = updateQueryStringBuilder.ToString();

                        using SqlCommand updateWorkinfoCommand = new SqlCommand(insertQueryString, connection, transaction);
                        updateWorkinfoCommand.Parameters.AddWithValue("@now", DateTime.UtcNow);
                        updateWorkinfoCommand.CommandTimeout = 0;

                        updateWorkinfoCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error: " + e.Message);
                        Environment.Exit(0);
                    }
                }

                Console.WriteLine("Insert titles batch complete:" + DateTime.UtcNow);

                Console.WriteLine("Creating CSN messages: " + DateTime.UtcNow);

                try
                {
                    CreateCsnMessages(csnNotifications, config).Wait();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error: " + e.Message);
                    Environment.Exit(0);
                }

                Console.WriteLine("CSN messages created: " + DateTime.UtcNow);

                transaction.Commit();
            }
        }

        static void WriteLineToResultsCsv(string iswc, string agencyWorkCode, string title, string agency, string agencies)
        {
            using (StreamWriter file = new StreamWriter("Results.csv", true))
            {
                file.WriteLine(string.Format("{0},={1},{2},={3},\"=\"{4}\"\"", "\"" + iswc + "\"", "\"" + agencyWorkCode + "\"", "\"" + title + "\"", "\"" + agency + "\"", "\"" + agencies + "\""));
            };
        }

        static async Task CreateCsnMessages(IEnumerable<CsnNotifications> csnNotifications, IConfigurationRoot config)
        {
            IOptions<CosmosDbOptions> options = Options.Create(new CosmosDbOptions() { DatabaseId = "ISWC" });
            CosmosClient client = new CosmosClient(config["ConnectionString-ISWCCosmosDb"]);
            CosmosDbRepository<CsnNotifications> repository = new CosmosDbRepository<CsnNotifications>(options, client);
            CosmosDbRepository<CsnNotificationsHighWatermark> watermarkRepository = new CosmosDbRepository<CsnNotificationsHighWatermark>(options, client);
            CosmosDbNotificationService notificationService = new CosmosDbNotificationService(repository, watermarkRepository);

            foreach (var batch in csnNotifications.Batch(200))
                await notificationService.AddCsnNotifications(batch);
        }
    }
}
