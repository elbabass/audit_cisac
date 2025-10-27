using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Data.Services.UpdateWorkflowHistory.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Repositories;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.UpdateWorkflowHistory.CosmosDb
{
    class CosmosDbUpdateWorkflowHistoryService : IUpdateWorkflowHistoryService
    {
        private readonly ICosmosDbRepository<UpdateWorkflowHistoryModel> updateWorkflowHistoryContainer;

        public CosmosDbUpdateWorkflowHistoryService(ICosmosDbRepository<UpdateWorkflowHistoryModel> updateWorkflowHistoryContainer)
        {
            this.updateWorkflowHistoryContainer = updateWorkflowHistoryContainer;
        }

        public async Task SaveModel(Submission submission, long workinfoId)
        {
            await updateWorkflowHistoryContainer.UpsertItemAsync(new UpdateWorkflowHistoryModel()
            {
                PreferredIswc = submission.Model.Iswc,
                WorkInfoId = workinfoId,
                Model = submission
            });
        }

        public async Task<Submission> GetModel(string preferrdIswc, long? workinfoId)
        {
            return (await updateWorkflowHistoryContainer.GetItemAsync(workinfoId?.ToString(), preferrdIswc)).Model;
        }
    }
}
