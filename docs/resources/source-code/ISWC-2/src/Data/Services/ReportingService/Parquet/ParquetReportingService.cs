using Azure.Storage.Blobs;
using Parquet;
using Parquet.Data;
using Parquet.Schema;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Data.Services.Audit.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.ReportingService.Parquet.Serializers;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Repositories;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.ReportingService.Parquet
{
    internal class ParquetReportingService : IReportingService
    {
        private readonly ICosmosDbRepository<AuditRequestModel> auditRequestContainer;
        private readonly IHighWatermarkRepository highWatermarkRepository;

        public ParquetReportingService(
            ICosmosDbRepository<AuditRequestModel> auditRequestContainer,
            IHighWatermarkRepository highWatermarkRepository)
        {
            this.auditRequestContainer = auditRequestContainer;
            this.highWatermarkRepository = highWatermarkRepository;
        }

        public async Task ProcessAuditChanges(BlobContainerClient container, HighWatermark highWatermark)
        {
            var feedIterator = auditRequestContainer.GetChangeFeedIterator(highWatermark.Value.AddMinutes(-15));
            var latestChanges = new List<AuditRequestModel>();
            var newHighWatermark = new DateTime();

            while (feedIterator.HasMoreResults)
            {
                foreach (var auditRequest in await feedIterator.ReadNextAsync())
                {
                    latestChanges.Add(auditRequest);
                }

                if (latestChanges.Count > 50_000 || !feedIterator.HasMoreResults)
                {
                    if (!latestChanges.Any())
                        continue;

                    var changes = latestChanges.GroupBy(x => x.CreatedDate.Date).ToDictionary(x => x.Key, x => x.ToList());

                    foreach (var dayWithNoAudit in changes.Keys.Min().Range(changes.Keys.Max()).Except(changes.Keys))
                        changes.Add(dayWithNoAudit, new List<AuditRequestModel>());

                    foreach (var date in changes)
                    {
                        var parquetFile = container.GetBlobClient(GetParquetFileName(date.Key));

                        var data = date.Value.Where(x => (x.IsProcessingError && x.IsProcessingFinished && x.TransactionError != null)
                        || (!x.IsProcessingError && x.IsProcessingFinished && x.TransactionError == null)).DistinctBy(x => x.AuditRequestId);

                        if (data.Count() == 0)
                            continue;

                        if (await parquetFile.ExistsAsync())
                            await WriteParquetFile(parquetFile, data, append: true);
                        else
                            await WriteParquetFile(parquetFile, data, append: false);
                    }

                    var latestDateFromChanges = latestChanges.OrderBy(x => x.CreatedDate).Last().CreatedDate;

                    newHighWatermark = latestDateFromChanges > newHighWatermark ? latestDateFromChanges : newHighWatermark;

                    await UpdateHighWatermark(highWatermark, newHighWatermark);

                    latestChanges.Clear();
                }
            }

            async Task UpdateHighWatermark(HighWatermark highWatermark, DateTime dateTime)
            {
                highWatermark.Value = dateTime;
                await highWatermarkRepository.UpdateAsync(highWatermark);
                await highWatermarkRepository.UnitOfWork.Save();
            }
        }

        private static string GetParquetFileName(DateTime dateTime) => $"{dateTime:yyyy/MM/dd}.parquet";

        private async Task WriteParquetFile(BlobClient parquetFile, IEnumerable<AuditRequestModel> records, bool append)
        {
            using var fileStream = new MemoryStream();

            if (append)
            {
                if (await parquetFile.ExistsAsync())
                {
                    var download = await parquetFile.DownloadAsync();
                    await download.Value.Content.CopyToAsync(fileStream);
                    fileStream.Position = 0;

                    var existingAuditRequests = new List<AuditRequestModel>();

                    using (var reader = await ParquetReader.CreateAsync(fileStream))
                    {
                        var idField = (DataField)reader.Schema[0];

                        for (int i = 0; i < reader.RowGroupCount; i++)
                        {
                            using var rg = reader.OpenRowGroupReader(i);
                            DataColumn idColumn = await rg.ReadColumnAsync(idField);
                            if (idColumn.Data is string[] ids)
                            {
                                existingAuditRequests.AddRange(
                                    ids.Where(s => !string.IsNullOrWhiteSpace(s))
                                       .Select(s => new AuditRequestModel { AuditRequestId = Guid.Parse(s!) })
                                );
                            }
                        }
                    }

                    records = records.Exclude(existingAuditRequests, x => x.AuditRequestId);
                }
            }

            fileStream.SetLength(0);
            if (records.Any())
            {
                var serializer = new AuditRequestModelSerializer(records);

                using var writer = await ParquetWriter.CreateAsync(serializer.Schema, fileStream);
                using var groupWriter = writer.CreateRowGroup();

                foreach (var column in serializer.Columns)
                {
                    await groupWriter.WriteColumnAsync(column);
                }
            }

            fileStream.Position = 0;
            await parquetFile.UploadAsync(fileStream, overwrite: true);
        }
    }
}
