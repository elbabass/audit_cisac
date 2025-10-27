using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using xRetry;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Integration.Tests.V1.Tests.AllocationServiceTests
{
    public class AllocateWorkBatchTests : TestBase, IAsyncLifetime
    {
        private IISWC_Allocation_and_ResolutionClient client;
        private HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await GetClient();
            client = new ISWC_Allocation_and_ResolutionClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 16.Allocate Work Batch, Partial Success
        /// </summary>
        [RetryFact]
        public async void AllocateWorkBatchTests_01()
        {
            var noAgreementsSub = Submissions.EligibleSubmissionPRS;
            noAgreementsSub.AdditionalIdentifiers = AdditionalIdentifersData.AI_NoSociety;

            var disambigSub = Submissions.EligibleSubmissionAEPI;
            disambigSub.Disambiguation = true;

            var sub = Submissions.EligibleSubmissionAEPI;
            sub.AdditionalIdentifiers = AdditionalIdentifersData.AI_JASRAC;
            sub.Agency = "038";
            sub.InterestedParties = new List<InterestedParty> { new InterestedParty { NameNumber = 165842942, Role = InterestedPartyRole.CA } };

            var subOnBehalfOf = Submissions.EligibleSubmissionASCAP;
            subOnBehalfOf.Agency = "707";
            subOnBehalfOf.Sourcedb = 707;
            subOnBehalfOf.AdditionalIdentifiers = AdditionalIdentifersData.AI_WARNER;

            List<SubmissionBatch> batch = new List<SubmissionBatch>{
            new SubmissionBatch {
                SubmissionId = 1,
                Submission =  noAgreementsSub,
            },
            new SubmissionBatch {
                SubmissionId = 2,
                Submission = disambigSub
            },
            new SubmissionBatch {
                SubmissionId = 3,
                Submission = sub
            },
            new SubmissionBatch {
                SubmissionId = 4,
                Submission = subOnBehalfOf
            }
        };

            var res = await client.AddAllocationBatchAsync(batch);

            Assert.Collection(res,
                elem1 => Assert.Equal("161", elem1.Rejection.Code),
                elem2 => Assert.Equal("124", elem2.Rejection.Code),
                elem3 => Assert.NotNull(elem3.Submission.VerifiedSubmission),
                elem4 => Assert.NotNull(elem4.Submission.VerifiedSubmission));

        }
    }
}
