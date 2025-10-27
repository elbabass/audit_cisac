using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using xRetry;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.UpdateTests
{
    /// <summary>
    /// 8. Update Work Batch 
    /// </summary>
    public class UpdateExistingWorkBatchTests : TestBase, IAsyncLifetime
    {
        private IISWC_SubmissionClient client;
        private HttpClient httpClient;

        private readonly List<SubmissionBatch> batch = new List<SubmissionBatch>{
            new SubmissionBatch {
                SubmissionId = 1,
                Submission =  Submissions.EligibleSubmissionBMI,
            },
            new SubmissionBatch {
                SubmissionId = 2,
                Submission = Submissions.EligibleSubmissionBMI
            },
            new SubmissionBatch {
                SubmissionId = 3,
                Submission = Submissions.EligibleSubmissionBMI
            },
            new SubmissionBatch {
                SubmissionId = 4,
                Submission = Submissions.EligibleSubmissionBMI
            }
        };

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await GetClient();
            client = new ISWC_SubmissionClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 8.Update Existing Work Batch, Partial Success submitter is ISWC eligible Contains some defective works
        /// </summary>
        [RetryFact]
        public async void UpdateExistingWorkBatchTests_01()
        {
            for (int i = 0; i < batch.Count; i++)
            {
                var addRes = await client.AddSubmissionAsync(batch[i].Submission);
                batch[i].Submission.PreferredIswc = addRes.VerifiedSubmission.Iswc.ToString();
            }
            await WaitForSubmission(batch[batch.Count - 1].Submission.Agency, batch[batch.Count - 1].Submission.Workcode, httpClient);
            await WaitForSubmission(batch[0].Submission.Agency, batch[0].Submission.Workcode, httpClient);
            batch[0].Submission.OtherTitles = new List<Title> {
                new Title { Title1 = $"{batch[0].Submission.OriginalTitle}_int", Type = TitleType.AT }
            };
            batch[1].Submission.PreferredIswc = "R9026079436";
            batch[2].Submission.InterestedParties.Add(InterestedParties.IP_IMRO[0]);
            batch[3].Submission.PreferredIswc = "T9800017603";

            var updateRes = await client.UpdateSubmissionBatchAsync(batch);

            Assert.Collection(updateRes,
                elem1 => Assert.Null(elem1.Rejection),
                elem2 => Assert.Equal("113", elem2.Rejection.Code),
                elem3 => Assert.True(elem3.Submission.VerifiedSubmission.InterestedParties.Count(x => x.NameNumber == batch[2].Submission.InterestedParties.ElementAt(2).NameNumber) > 0),
                elem4 => Assert.Equal("141", elem4.Rejection.Code));

        }
    }
}
