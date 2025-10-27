using Microsoft.Azure.Cosmos;
using SpanishPoint.Azure.Iswc.Data.Services.IswcService.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Repositories;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.IswcService.CosmosDb
{
    internal class CosmosDbIswcService : IIswcService
    {
        private readonly ICosmosDbRepository<IswcModel> iswcContainer;
        private const string IswcId = "46a7e2c1-9cda-419e-b8c7-b6f755790f49";

        public CosmosDbIswcService(ICosmosDbRepository<IswcModel> iswcContainer)
        {
            this.iswcContainer = iswcContainer;
        }

        public async Task<string> GetNextIswc()
        {
            var (success, iswc) = await UpsertIswc();

            while (!success)
            {
                (success, iswc) = await UpsertIswc();
            }

            return iswc;

            async Task<(bool success, string iswc)> UpsertIswc()
            {
                var currentIswc = await iswcContainer.GetItemAsync(IswcId, IswcId);
                var nextIswc = currentIswc.Iswc.IncrementIswc();
                try
                {
                    await iswcContainer.UpdateItemAsync(IswcId, new IswcModel
                    {
                        IswcId = new Guid(IswcId),
                        Iswc = nextIswc,
                        ETag = currentIswc.ETag
                    });
                    return (true, nextIswc);
                }
                catch (CosmosException ex)
                {
                    if (ex.StatusCode == HttpStatusCode.PreconditionFailed)
                        return (false, string.Empty);
                    else
                        throw;
                }
            }
        }
    }
}
