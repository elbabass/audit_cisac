using SpanishPoint.Azure.Iswc.Data.Services.Checksum.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Repositories;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.Checksum.CosmosDb
{
    public class CosmosDbChecksumService : IChecksumService
    {
        private readonly ICosmosDbRepository<SubmissionChecksums> repository;

        public CosmosDbChecksumService(ICosmosDbRepository<SubmissionChecksums> repository)
        {
            this.repository = repository;
        }

        public async Task AddChecksum(SubmissionChecksums checksum)
        {
            await repository.UpsertItemAsync(checksum);
        }

        public async Task DeleteItemAsync(string id, string partitionKey)
        {
            await repository.DeleteItemAsync(id, partitionKey);
        }

        public async Task<SubmissionChecksums> GetChecksum(string agency, string agencyWorkCode)
        {
            var id = string.Concat(agency, agencyWorkCode);
            return await repository.GetItemAsync(id, id);
        }
    }
}
