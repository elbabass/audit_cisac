using Microsoft.AspNetCore.Http;
using Xunit;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using xRetry;
using System.Collections.Generic;
using System.Net.Http;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.UpdateTests
{
    public class UpdateExistingWorkFailureTests_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient client;
        public IISWC_SearchClient searchClient;
        public Submission currentSubmission;
        public Submission inelligibleSubmission;
        public HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            long nonAffiliatedNameNumber = 240971278;
            httpClient = await TestBase.GetClient();
            client = new ISWC_SubmissionClient(httpClient);
            searchClient = new ISWC_SearchClient(httpClient);
            currentSubmission = Submissions.EligibleSubmissionAKKA;
            currentSubmission.InterestedParties.Add(InterestedParties.IP_IMRO[0]);
            currentSubmission.InterestedParties.Add(new InterestedParty()
            {
                BaseNumber = "I-001635861-3",
                Role = InterestedPartyRole.C,
                NameNumber = 865900903,
                Name = "PUBLIC DOMAIN"
            });
            currentSubmission.InterestedParties.Add(new InterestedParty
            {
                NameNumber = nonAffiliatedNameNumber,
                Role = InterestedPartyRole.CA
            });
            currentSubmission.PreferredIswc = (await client.AddSubmissionAsync(currentSubmission)).VerifiedSubmission.Iswc.ToString();
            await TestBase.WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);

            inelligibleSubmission = Submissions.EligibleSubmissionSACEM;
            inelligibleSubmission.InterestedParties = currentSubmission.InterestedParties;
            inelligibleSubmission.OriginalTitle = currentSubmission.OriginalTitle;
            inelligibleSubmission.PreferredIswc = currentSubmission.PreferredIswc;
            await client.AddSubmissionAsync(inelligibleSubmission);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }

    public class UpdateExistingWorkFailureTests : TestBase, IClassFixture<UpdateExistingWorkFailureTests_Fixture>
    {
        private readonly IISWC_SubmissionClient client;
        private readonly IISWC_SearchClient searchClient;
        private readonly Submission currentSubmission;
        private readonly Submission inelligibleSubmission;
        private readonly HttpClient httpClient;

        public UpdateExistingWorkFailureTests(UpdateExistingWorkFailureTests_Fixture fixture)
        {
            client = fixture.client;
            currentSubmission = fixture.currentSubmission;
            inelligibleSubmission = fixture.inelligibleSubmission;
            httpClient = fixture.httpClient;
            searchClient = fixture.searchClient;
        }

        /// <summary>
        /// Submitter is ISWC Eligible
        /// ISWC is in invalid format
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkFailureTests_01()
        {
            var sub = Submissions.EligibleSubmissionBMI;
            sub.PreferredIswc = "T203000001";
            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.UpdateSubmissionAsync(sub.PreferredIswc, sub));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("113", GetErrorCode(res.Response));
        }

        /// <summary>
		/// Submitter is ISWC eligible
		/// Submit update for deleted work
		/// </summary>
		[Fact]
        public async void UpdateExistingWorkFailureTests_02()
        {
            var sub = Submissions.EligibleSubmissionAEPI;
            var addRes = await client.AddSubmissionAsync(sub);
            sub.PreferredIswc = addRes.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);
            await client.DeleteSubmissionAsync(sub.PreferredIswc, sub.Agency, sub.Workcode, sub.Sourcedb, "IT_4_7_UpdateExistingWorkFailure");

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.UpdateSubmissionAsync(sub.PreferredIswc, sub));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("131", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Submitting agency is non-eligible
        /// Submission metadata is different to the ISWC level metadata
        /// Update is rejected
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkFailureTests_03()
        {
            var submission = Submissions.EligibleSubmissionBMI;
            var addRes = await client.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.PreferredIswc = addRes.VerifiedSubmission.Iswc.ToString();

            submission.Agency = Submissions.EligibleSubmissionIMRO.Agency;
            submission.Sourcedb = Submissions.EligibleSubmissionIMRO.Sourcedb;
            submission.Workcode = CreateNewWorkCode();
            await client.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            submission.OriginalTitle = CreateNewTitle();
            var updateRes = await Assert.ThrowsAsync<ApiException>(async () => await client.UpdateSubmissionAsync(submission.PreferredIswc, submission));

            Assert.Equal(StatusCodes.Status400BadRequest, updateRes.StatusCode);
            Assert.Equal("144", GetErrorCode(updateRes.Response));
        }

        /// <summary>
        /// CUR is changed to CAR as workcode does not exist.
        /// Transaction fails as no matches are found.
        /// Agency is not ISWC eligible.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkFailureTests_04()
        {
            currentSubmission.Agency = Submissions.EligibleSubmissionASCAP.Agency;
            currentSubmission.Sourcedb = Submissions.EligibleSubmissionASCAP.Sourcedb;
            currentSubmission.Workcode = CreateNewWorkCode();
            currentSubmission.OriginalTitle = CreateNewTitle();
            var updateRes = await Assert.ThrowsAsync<ApiException>(async () => await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission));

            Assert.Equal(StatusCodes.Status400BadRequest, updateRes.StatusCode);
            Assert.Equal("140", GetErrorCode(updateRes.Response));
        }

        /// <summary>
        /// Updating a work that has been created with disambiguated information.
        /// No request-source is set.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkFailureTests_05()
        {
            var submission = Submissions.EligibleSubmissionAEPI;
            var iswcOne = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.Workcode = CreateNewWorkCode();
            submission.DisambiguateFrom = new List<DisambiguateFrom>()
            {
                new DisambiguateFrom
                {
                    Iswc = iswcOne
                }
            };
            submission.Disambiguation = true;
            submission.DisambiguationReason = DisambiguationReason.DIT;
            var disambiguatedIswc = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.DisambiguateFrom = null;
            submission.Disambiguation = false;
            submission.DisambiguationReason = null;
            submission.OriginalTitle = CreateNewTitle();

            var updateRes = await Assert.ThrowsAsync<ApiException>(async () => await client.UpdateSubmissionAsync(disambiguatedIswc, submission));

            Assert.Equal(StatusCodes.Status400BadRequest, updateRes.StatusCode);
            Assert.Equal("167", GetErrorCode(updateRes.Response));
        }

        /// <summary>
        /// Updating a work that has been created with disambiguated information.
        /// request-source is set to CISNET.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkFailureTests_06()
        {
            var http = await GetClient();
            http.DefaultRequestHeaders.Add("request-source", "CISNET");
            var clientWithHeader = new ISWC_SubmissionClient(http);
            var submission = Submissions.EligibleSubmissionAEPI;
            var iswcOne = (await clientWithHeader.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.Workcode = CreateNewWorkCode();
            submission.DisambiguateFrom = new List<DisambiguateFrom>()
            {
                new DisambiguateFrom
                {
                    Iswc = iswcOne
                }
            };
            submission.Disambiguation = true;
            submission.DisambiguationReason = DisambiguationReason.DIT;
            var disambiguatedIswc = (await clientWithHeader.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.DisambiguateFrom = null;
            submission.Disambiguation = false;
            submission.DisambiguationReason = null;
            submission.OriginalTitle = CreateNewTitle();

            var updateRes = await Assert.ThrowsAsync<ApiException>(async () => await clientWithHeader.UpdateSubmissionAsync(disambiguatedIswc, submission));

            Assert.Equal(StatusCodes.Status400BadRequest, updateRes.StatusCode);
            Assert.Equal("167", GetErrorCode(updateRes.Response));
        }

        /// <summary>
        /// Updating a work that has been created with disambiguated information.
        /// Disambiguation data is not included.
        /// request-source is set to CISNET.
        /// No PreferredIswc is given.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkFailureTests_07()
        {
            var http = await GetClient();
            http.DefaultRequestHeaders.Add("request-source", "CISNET");
            var clientWithHeader = new ISWC_SubmissionClient(http);
            var submission = Submissions.EligibleSubmissionAKKA;
            submission.Workcode = CreateNewWorkCode();
            submission.OriginalTitle = currentSubmission.OriginalTitle;
            submission.DisambiguateFrom = new List<DisambiguateFrom>()
            {
                new DisambiguateFrom
                {
                    Iswc = currentSubmission.PreferredIswc
                }
            };
            submission.Disambiguation = true;
            submission.DisambiguationReason = DisambiguationReason.DIT;
            var disambiguatedIswc = (await clientWithHeader.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.DisambiguateFrom = null;
            submission.Disambiguation = false;
            submission.DisambiguationReason = null;
            submission.OriginalTitle = CreateNewTitle();

            var updateRes = await Assert.ThrowsAsync<ApiException>(async () => await clientWithHeader.UpdateSubmissionAsync("", submission));

            Assert.Equal(StatusCodes.Status400BadRequest, updateRes.StatusCode);
            Assert.Equal("167", GetErrorCode(updateRes.Response));
        }


        /// <summary>
        /// Ineligible society cannot remove PD creator.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkFailureTests_08()
        {
            inelligibleSubmission.InterestedParties.Remove(currentSubmission.InterestedParties.First(x => x.NameNumber == 865900903));

            var updateRes = await Assert.ThrowsAsync<ApiException>(async () => await client.UpdateSubmissionAsync(inelligibleSubmission.PreferredIswc, inelligibleSubmission));

            Assert.Equal(StatusCodes.Status400BadRequest, updateRes.StatusCode);
            Assert.Equal("144", GetErrorCode(updateRes.Response));
        }

        /// <summary>
        /// Ineligible society cannot remove Non-Society creator.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkFailureTests_09()
        {
            inelligibleSubmission.InterestedParties = currentSubmission.InterestedParties;
            inelligibleSubmission.InterestedParties.Remove(inelligibleSubmission.InterestedParties.First(x => x.NameNumber == 240971278));

            var updateRes = await Assert.ThrowsAsync<ApiException>(async () => await client.UpdateSubmissionAsync(inelligibleSubmission.PreferredIswc, inelligibleSubmission));

            Assert.Equal(StatusCodes.Status400BadRequest, updateRes.StatusCode);
            Assert.Equal("144", GetErrorCode(updateRes.Response));
        }


        /// <summary>
        /// Eligible society without a submission cannot remove creator.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkFailureTests_10()
        {
            var submission = Submissions.EligibleSubmissionAKKA;
            submission.InterestedParties.Add(InterestedParties.IP_IMRO[0]);
            submission.PreferredIswc = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.Agency = Submissions.EligibleSubmissionIMRO.Agency;
            submission.Sourcedb = Submissions.EligibleSubmissionIMRO.Sourcedb;
            submission.InterestedParties.Remove(submission.InterestedParties.Last());

            var updateRes = await Assert.ThrowsAsync<ApiException>(async () => await client.UpdateSubmissionAsync(submission.PreferredIswc, submission));

            Assert.Equal(StatusCodes.Status400BadRequest, updateRes.StatusCode);
            Assert.Equal("140", GetErrorCode(updateRes.Response));
        }

        /// <summary>
        /// Duplicate submission is rejected if EnableChecksumValidation is enabled.
        /// AKKA-LA (122) should always have EnableChecksumValidation enabled in Lookup.Agency
        /// </summary>
        [SkippableFact]
        public async void UpdateExistingWorkFailureTests_11()
        {
            Skip.IfNot(await IsValidationRuleEnabled("EnableChecksumValidation"), "EnableChecksumValidation is set to FALSE.");
            var submission = Submissions.EligibleSubmissionAKKA;
            submission.PreferredIswc = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.UpdateSubmissionAsync(submission.PreferredIswc, submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("174", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Checksum is updated when a work is updated and subsequent identical updates are rejected.
        /// If EnableChecksumValidation is enabled.
        /// AKKA-LA (122) should always have EnableChecksumValidation enabled in Lookup.Agency
        /// </summary>
        [SkippableFact]
        public async void UpdateExistingWorkFailureTests_12()
        {
            Skip.IfNot(await IsValidationRuleEnabled("EnableChecksumValidation"), "EnableChecksumValidation is set to FALSE.");
            var submission = Submissions.EligibleSubmissionAKKA;
            submission.PreferredIswc = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.OriginalTitle = CreateNewTitle();
            await client.UpdateSubmissionAsync(submission.PreferredIswc, submission);
            await WaitForUpdate(submission.Agency, submission.Workcode, submission.OriginalTitle, httpClient);
            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.UpdateSubmissionAsync(submission.PreferredIswc, submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("174", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Feature 7447: Multiple Agency Workcodes
        /// Test: Update on an ISWC with multi-agency
        /// Failed update only allowed for IAS/IRS
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkFailureTests_13()
        {
            var existingSub = Submissions.EligibleSubmissionECAD;
            existingSub.AdditionalIdentifiers = new AdditionalIdentifiers
            {
                AgencyWorkCodes = new List<AgencyWorkCodes>()
                {
                    new AgencyWorkCodes
                    {
                        Agency = "030",
                        WorkCode = CreateNewWorkCode()
                    }
                }
            };
            var res = await Assert.ThrowsAsync<ApiException>(() => client.UpdateSubmissionAsync(existingSub.PreferredIswc, existingSub));


            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("182", GetErrorCode(res.Response));
        }
    }
}
