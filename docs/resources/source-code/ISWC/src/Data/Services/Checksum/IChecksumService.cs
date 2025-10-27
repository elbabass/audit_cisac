using SpanishPoint.Azure.Iswc.Data.Services.Checksum.CosmosDb.Models;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.Checksum
{
    public interface IChecksumService
    {
        Task AddChecksum(SubmissionChecksums checksum);
        Task<SubmissionChecksums> GetChecksum(string agency, string agencyWorkCode);
        Task DeleteItemAsync(string id, string partitionKey);
    }
}
