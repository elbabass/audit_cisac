using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Integration.Tests.V1.Tests.ResolutionServiceTests
{
    public class ResolveWorkBatchTests_Fixture : IAsyncLifetime
    {
        public IISWC_Allocation_and_ResolutionClient resolutionClient;
        public IISWC_SubmissionClient submissionClient;
        public HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            resolutionClient = new ISWC_Allocation_and_ResolutionClient(httpClient);
            submissionClient = new ISWC_SubmissionClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class ResolveWorkBatchTests : TestBase, IClassFixture<ResolveWorkBatchTests_Fixture>
    {
        private readonly IISWC_Allocation_and_ResolutionClient resolutionClient;
        private readonly IISWC_SubmissionClient submissionClient;
        private readonly HttpClient httpClient;

        public ResolveWorkBatchTests(ResolveWorkBatchTests_Fixture fixture)
        {
            resolutionClient = fixture.resolutionClient;
            submissionClient = fixture.submissionClient;
            httpClient = fixture.httpClient;
        }

        /// <summary>
        /// Resolution request is rejected as submitting society is not CISAC.
        /// Second request is accepted as CISAC is the submitting agency.
        /// </summary>
        [Fact]
        public async void ResolveWorkBatchTests_01()
        {
            var noIPSub = Submissions.EligibleSubmissionPRS;
            var sub = Submissions.EligibleSubmissionPRS;
            List<SubmissionBatch> batch = new List<SubmissionBatch>{
                new SubmissionBatch {
                    SubmissionId = 1,
                    Submission =  noIPSub,
                },
                new SubmissionBatch {
                    SubmissionId = 2,
                    Submission = sub
                }
            };

            var addRes = await submissionClient.AddSubmissionAsync(batch[1].Submission);
            batch[1].Submission.PreferredIswc = addRes.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(batch[1].Submission.Agency, batch[1].Submission.Workcode, httpClient);
            batch[0].Submission.Workcode = CreateNewWorkCode();
            batch[1].Submission.Workcode = CreateNewWorkCode();
            batch[1].Submission.PreviewDisambiguation = true;
            batch[1].Submission.Agency = "312";
            batch[1].Submission.Sourcedb = 315;
            var result = await resolutionClient.AddResolutionBatchAsync(batch);
            Assert.Collection(result,
                elem1 => Assert.Equal("166", elem1.Rejection.Code),
                elem2 => {
                    Assert.NotNull(elem2.Submission.VerifiedSubmission);
                    Assert.False(elem2.Submission.VerifiedSubmission.IswcEligible);
                    Assert.Equal(elem2.Submission.VerifiedSubmission.Iswc.ToString(), batch[1].Submission.PreferredIswc);
                });
        }

        /// <summary>
        /// Test for bug 4133.
        /// </summary>
        [Fact]
        public async void ResolveWorkBatchTests_02()
        {
            var subOne = Submissions.EligibleSubmissionPRS;
            var subTwo = Submissions.EligibleSubmissionPRS;
            subOne.OriginalTitle = CreateNewTitleWithLettersOnly();
            subTwo.OriginalTitle = subOne.OriginalTitle;
            subTwo.InterestedParties.Remove(subTwo.InterestedParties.Last());

            List<SubmissionBatch> batch = new List<SubmissionBatch>{
                new SubmissionBatch {
                    SubmissionId = 1,
                    Submission =  subOne,
                }
            };
            var addResSubOne = await submissionClient.AddSubmissionAsync(subOne);
            var addResSubTwo = await submissionClient.AddSubmissionAsync(subTwo);
            await WaitForSubmission(subOne.Agency, subOne.Workcode, httpClient);
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);
            subOne.PreviewDisambiguation = true;
            subOne.Workcode = CreateNewWorkCode();
            subOne.Agency = "312";
            subOne.Sourcedb = 315;
            var iswcOne = addResSubOne.VerifiedSubmission.Iswc.ToString();

            var result = await resolutionClient.AddResolutionBatchAsync(batch);

            Assert.Equal(result.First().Submission.VerifiedSubmission.Iswc.ToString(), iswcOne);

            Assert.Collection(result,
                elem1 => Assert.Equal(addResSubOne.VerifiedSubmission.Iswc.ToString(), elem1.Submission.VerifiedSubmission.Iswc.ToString()));
        }

        /// <summary>
        /// Test for ISWC match (Feature 11960).
        /// Pass - first submission succeeds matched by worknumber and title threshold
        /// Fail - second submission fails mtached by worknumbers but fails title threshold
        /// </summary>
        [Fact]
        public async void ResolveWorkBatchTests_03()
        {
            var subOne = Submissions.EligibleSubmissionPRS;
            var subTwo = Submissions.EligibleSubmissionPRS;
            var subThree = Submissions.EligibleSubmissionPRS;

            var addResSubOne = await submissionClient.AddSubmissionAsync(subOne);
            var addResSubTwo = await submissionClient.AddSubmissionAsync(subTwo);
            await WaitForSubmission(subOne.Agency, subOne.Workcode, httpClient);
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);

            subOne.PreviewDisambiguation = true;
            subOne.Workcode = CreateNewWorkCode();
            subOne.Agency = "312";
            subOne.Sourcedb = 315;

            subOne.AdditionalIdentifiers = new AdditionalIdentifiers
            {
                AgencyWorkCodes = new List<AgencyWorkCodes>
                {
                    new AgencyWorkCodes
                    {
                         Agency = "052",
                         WorkCode = addResSubOne.VerifiedSubmission.Workcode
                    }
                }
            };

            subThree.PreviewDisambiguation = true;
            subThree.Workcode = CreateNewWorkCode();
            subThree.Agency = "312";
            subThree.Sourcedb = 315;

            subThree.AdditionalIdentifiers = new AdditionalIdentifiers
            {
                AgencyWorkCodes = new List<AgencyWorkCodes>
                {
                    new AgencyWorkCodes
                    {
                         Agency = "052",
                         WorkCode = addResSubTwo.VerifiedSubmission.Workcode
                    },
                    new AgencyWorkCodes
                    {
                         Agency = "052",
                         WorkCode = addResSubOne.VerifiedSubmission.Workcode
                    }
                }
            };

            List<SubmissionBatch> batch = new List<SubmissionBatch>{
                new SubmissionBatch {
                    SubmissionId = 1,
                    Submission =  subOne,
                }
                ,
                new SubmissionBatch {
                    SubmissionId = 2,
                    Submission =  subThree,
                }
            };

            var iswcOne = addResSubOne.VerifiedSubmission.Iswc.ToString();

            var result = await resolutionClient.AddResolutionBatchAsync(batch);

            Assert.Equal(result.First().Submission.VerifiedSubmission.Iswc.ToString(), iswcOne);

            Assert.Collection(result,
                elem1 => Assert.Equal(addResSubOne.VerifiedSubmission.Iswc.ToString(), elem1.Submission.VerifiedSubmission.Iswc.ToString()),
                elem2 => Assert.Equal("163", elem2.Rejection.Code));
        }


        /// <summary>
        /// Test for InEligible work match (Feature 17013) 
        /// Resolution passes and returns ineligible work.
        /// </summary>
        [Fact]
        public async void ResolveWorkBatchTests_04()
        {
            var submission = Submissions.EligibleSubmissionBMI;
            submission.OriginalTitle = CreateNewTitleWithLettersOnly();
            var subOne = await submissionClient.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);


            submission.DisambiguateFrom = null;
            submission.Disambiguation = false;
            submission.DisambiguationReason = null;
            submission.Workcode = CreateNewWorkCode();
            submission.Agency = Submissions.EligibleSubmissionASCAP.Agency;
            submission.Sourcedb = Submissions.EligibleSubmissionASCAP.Sourcedb;


            var inEligibleSub = await submissionClient.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            var iswcInEligibleSub = inEligibleSub.VerifiedSubmission.Iswc.ToString();

            submission.Agency = "312";
            submission.Sourcedb = 315;
            submission.PreviewDisambiguation = true;
            submission.OriginalTitle = CreateNewTitleWithLettersOnly();

            submission.AdditionalIdentifiers = new AdditionalIdentifiers
            {
                AgencyWorkCodes = new List<AgencyWorkCodes>
                {
                    new AgencyWorkCodes
                    {
                         Agency = "010",
                         WorkCode = inEligibleSub.VerifiedSubmission.Workcode
                    }
                }
            };

            List<SubmissionBatch> batch = new List<SubmissionBatch>{
                new SubmissionBatch {
                    SubmissionId = 1,
                    Submission =  submission,
                }
            };


            var result = await resolutionClient.AddResolutionBatchAsync(batch);

            Assert.Collection(result,
                elem1 => {
                    Assert.Equal(iswcInEligibleSub, elem1.Submission.VerifiedSubmission.Iswc);
                    Assert.False(elem1.Submission.VerifiedSubmission.IswcEligible);
                }
                );
        }
    }
}