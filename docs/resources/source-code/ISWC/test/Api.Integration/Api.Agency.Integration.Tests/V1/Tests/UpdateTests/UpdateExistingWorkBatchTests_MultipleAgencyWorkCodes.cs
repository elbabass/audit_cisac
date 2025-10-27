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
    public class UpdateExistingWorkBatchTests_MultipleAgencyWorkCodes : TestBase, IAsyncLifetime
    {
        private IISWC_SubmissionClient client;
        private HttpClient httpClient;

        private readonly List<SubmissionBatch> batch = new List<SubmissionBatch>{
            new SubmissionBatch {
                SubmissionId = 1,
                Submission =  Submissions.EligibleSubmissionBMI,
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
        /// 8.Update Existing Work Batch, Multiple AgencyWorkcodes only main workcode exists
        /// other two mulitleworkcodes are changed to CAR and added to same pregerred iswc 
        /// </summary>
        [RetryFact]
        public async void UpdateExistingWorkBatchTests_01()
        {
            var singleBatch = batch;

            var addRes = await client.AddSubmissionAsync(singleBatch.FirstOrDefault().Submission);


            await WaitForSubmission(batch[batch.Count - 1].Submission.Agency, batch[batch.Count - 1].Submission.Workcode, httpClient);
            await WaitForSubmission(batch[0].Submission.Agency, batch[0].Submission.Workcode, httpClient);


            singleBatch.FirstOrDefault().MultipleAgencyWorkCodes = new List<MultipleAgencyWorkCodes2>
            {
                new MultipleAgencyWorkCodes2
                {
                    Agency = batch[0].Submission.Agency,
                    WorkCode = CreateNewWorkCode()
                },
                new MultipleAgencyWorkCodes2
                {
                    Agency = "128",
                    WorkCode = CreateNewWorkCode()
                }
            };

            var updateRes = await client.UpdateSubmissionBatchAsync(singleBatch);

            var preferredIswc = updateRes.FirstOrDefault().Submission.VerifiedSubmission.Iswc;


            Assert.NotNull(updateRes);
            Assert.Null(updateRes.FirstOrDefault().Rejection);
            Assert.Single(updateRes);
            Assert.True(updateRes.FirstOrDefault().Submission.MultipleAgencyWorkCodes.Count == 2);
            Assert.Equal(addRes.VerifiedSubmission.Iswc, preferredIswc);
            Assert.Collection(updateRes.FirstOrDefault().Submission.MultipleAgencyWorkCodes,
                elem1 => Assert.Null(elem1.Rejection),
                elem2 => Assert.Null(elem2.Rejection));

        }

        /// <summary>
        /// 8.Update Existing Work Batch, Multiple AgencyWorkcodes only main workcode exists
        /// other two mulitleworkcodes are changed to CAR bug main submisssion fails and none are processed
        /// </summary>
        [RetryFact]
        public async void UpdateExistingWorkBatchTests_02()
        {
            var singleBatch = batch;

            singleBatch.FirstOrDefault().Submission.Agency = "021";
            var addRes = await client.AddSubmissionAsync(singleBatch.FirstOrDefault().Submission);


            await WaitForSubmission(batch[batch.Count - 1].Submission.Agency, batch[batch.Count - 1].Submission.Workcode, httpClient);
            await WaitForSubmission(batch[0].Submission.Agency, batch[0].Submission.Workcode, httpClient);


            singleBatch.FirstOrDefault().MultipleAgencyWorkCodes = new List<MultipleAgencyWorkCodes2>
            {
                new MultipleAgencyWorkCodes2
                {
                    Agency = batch[0].Submission.Agency,
                    WorkCode = CreateNewWorkCode()
                },
                new MultipleAgencyWorkCodes2
                {
                    Agency = "052",
                    WorkCode = CreateNewWorkCode()
                }
            };

            singleBatch.FirstOrDefault().Submission.OriginalTitle = singleBatch.FirstOrDefault().Submission.OriginalTitle + "updated title";

            singleBatch.FirstOrDefault().Submission.InterestedParties = Submissions.EligibleSubmissionSACEM.InterestedParties;
            var updateRes = await client.UpdateSubmissionBatchAsync(singleBatch);


            Assert.NotNull(updateRes);
            Assert.Single(updateRes);
            Assert.True(updateRes.FirstOrDefault().Submission.MultipleAgencyWorkCodes.Count == 2);
            Assert.NotNull(updateRes.FirstOrDefault().Rejection);
            Assert.Collection(updateRes.FirstOrDefault().Submission.MultipleAgencyWorkCodes,
                elem1 => Assert.Null(elem1.Rejection),
                elem2 => Assert.Null(elem2.Rejection));

        }
    }
}