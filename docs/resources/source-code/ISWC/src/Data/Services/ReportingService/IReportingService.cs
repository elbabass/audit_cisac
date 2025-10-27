using Microsoft.Azure.Storage.Blob;
using SpanishPoint.Azure.Iswc.Bdo.Reports;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.ReportingService
{
    public interface IReportingService
    {
        Task ProcessAuditChanges(CloudBlobContainer container, HighWatermark highWatermark);
    }
}
