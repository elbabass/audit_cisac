using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.SubmitNewWorkTests
{
    /// <summary>
    /// 7. Submit Work Batch
    /// </summary>
    public class SubmitExistingWorkBatchTests_MultipleAgencyWorkCodes : TestBase, IAsyncLifetime
    {
        private IISWC_SubmissionClient client;
        private HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await GetClient();
            client = new ISWC_SubmissionClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 7.Submit Work Batch, Partial Success
        /// </summary>
        [Fact]
        public async void SubmitNewWorkBatchTests_MultipleAgencyWorkCodes_01()
        {
            var sub = Submissions.EligibleSubmissionSACEM;


            List<SubmissionBatch> batch = new List<SubmissionBatch>{
            new SubmissionBatch {
                SubmissionId = 1,
                Submission =  sub,
                MultipleAgencyWorkCodes = new List<MultipleAgencyWorkCodes2>
                {
                    new MultipleAgencyWorkCodes2() {
                        Agency = "128",
                        WorkCode = CreateNewWorkCode()
                    },
                    new MultipleAgencyWorkCodes2() {
                        Agency = "021",
                        WorkCode = CreateNewWorkCode()
                    }
                }
            }
        };

            var submitRes = await client.AddSubmissionBatchAsync(batch);


            Assert.NotNull(submitRes);
            Assert.Null(submitRes.FirstOrDefault().Rejection);
            Assert.Single(submitRes);
            Assert.True(submitRes.FirstOrDefault().Submission.MultipleAgencyWorkCodes.Count() == 2);
            Assert.Collection(submitRes.FirstOrDefault().Submission.MultipleAgencyWorkCodes,
                elem1 => Assert.Null(elem1.Rejection),
                elem2 => Assert.Null(elem2.Rejection)
               );

        }

        [Fact]
        public async void SubmitNewWorkBatchTests_MultipleAgencyWorkCodes_02()
        {
            var sub = Submissions.EligibleSubmissionIMRO;

            sub.Agency = "052";
            sub.Sourcedb = 52;
            List<SubmissionBatch> batch = new List<SubmissionBatch>{
            new SubmissionBatch {
                SubmissionId = 1,
                Submission =  sub,
                MultipleAgencyWorkCodes = new List<MultipleAgencyWorkCodes2>
                {
                    new MultipleAgencyWorkCodes2() {
                        Agency = "128",
                        WorkCode = CreateNewWorkCode()
                    },
                    new MultipleAgencyWorkCodes2() {
                        Agency = "021",
                        WorkCode = CreateNewWorkCode()
                    }
                }
            }
        };

            var submitRes = await client.AddSubmissionBatchAsync(batch);


            Assert.NotNull(submitRes);
            Assert.Single(submitRes);
            Assert.NotNull(submitRes.FirstOrDefault().Rejection);
            Assert.True(submitRes.FirstOrDefault().Submission.MultipleAgencyWorkCodes.Count() == 2);
            Assert.Collection(submitRes.FirstOrDefault().Submission.MultipleAgencyWorkCodes,
                elem1 => Assert.Null(elem1.Rejection),
                elem2 => Assert.Null(elem2.Rejection)
               );

        }

        /// <summary>
        /// Create a new ISWC with a single submission, then submit an update to add new submissions to the ISWC, 
        /// and update the existing submission metadata
        /// </summary>
        [Fact]
        public async void SubmitNewWorkBatchTests_MultipleAgencyWorkCodes_03()
        {
            var sub = Submissions.EligibleSubmissionIMRO;

            List<SubmissionBatch> batch = new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission =  sub,
                    MultipleAgencyWorkCodes = new List<MultipleAgencyWorkCodes2>()
                }
            };

            var submitRes = await client.AddSubmissionBatchAsync(batch);
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);

            Assert.Null(submitRes.FirstOrDefault().Rejection);
            batch.FirstOrDefault().Submission.Performers = new List<Performer>
            {
                new Performer { FirstName = "Test", LastName = "Performer" }
            };
            batch.FirstOrDefault().MultipleAgencyWorkCodes.Add(new MultipleAgencyWorkCodes2()
            {
                Agency = "021",
                WorkCode = CreateNewWorkCode()
            });

            var submitRes2 = await client.AddSubmissionBatchAsync(batch);
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);

            Assert.NotNull(submitRes2);
            Assert.Null(submitRes2.FirstOrDefault().Rejection);

            Assert.Single(submitRes2.FirstOrDefault().Submission.MultipleAgencyWorkCodes);
            Assert.Equal("021", submitRes2.FirstOrDefault().Submission.MultipleAgencyWorkCodes.FirstOrDefault().Agency);
            Assert.Equal("Test", submitRes2.FirstOrDefault().Submission.VerifiedSubmission.Performers.FirstOrDefault().FirstName);
            Assert.Equal("Performer", submitRes2.FirstOrDefault().Submission.VerifiedSubmission.Performers.FirstOrDefault().LastName);
        }
    }
}
