using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using xRetry;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.SubmitNewWorkTests
{
    public class SubmitNewWorkFailureTests_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient client;
        public HttpClient httpClient;
        public IISWC_SearchClient searchClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            client = new ISWC_SubmissionClient(httpClient);
            searchClient = new ISWC_SearchClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class SubmitNewWorkFailureTests : TestBase, IClassFixture<SubmitNewWorkFailureTests_Fixture>
    {
        private IISWC_SubmissionClient client;
        private HttpClient httpClient;
        private IISWC_SearchClient searchClient;

        public SubmitNewWorkFailureTests(SubmitNewWorkFailureTests_Fixture fixture)
        {
            client = fixture.client;
            httpClient = fixture.httpClient;
            searchClient = fixture.searchClient;
        }

        /// <summary>
        /// Submitter is not ISWC eligible
        /// Multiple potential ISWCs already exist
        /// Metadata is not an exact match
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_1()
        {
            var submission = Submissions.EligibleSubmissionBMI;
            submission.OriginalTitle = CreateNewTitleWithLettersOnly();
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
            await client.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            submission.DisambiguateFrom = null;
            submission.Disambiguation = false;
            submission.DisambiguationReason = null;
            submission.OriginalTitle += "a";
            submission.Workcode = CreateNewWorkCode();
            submission.Agency = Submissions.EligibleSubmissionASCAP.Agency;
            submission.Sourcedb = Submissions.EligibleSubmissionASCAP.Sourcedb;
            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("160", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Submitter is not ISWC eligible
        /// No ISWC already exists
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_2()
        {
            var submission = Submissions.EligibleSubmissionBMI;
            submission.InterestedParties = InterestedParties.IP_AEPI;

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("140", GetErrorCode(res.Response));
        }

        /// <summary>
        /// AllowPDWorkSubmissions parameter = TRUE
        /// Submitter is not authoritative for Creator IPs
        /// All Creator IPs are one of the generic four IPs(Public Domain, DP, TRAD, Unknown Composer Author)
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkFailureTests_3()
        {
            Skip.IfNot(await IsValidationRuleEnabled("AllowPDWorkSubmissions"), "AllowPDWorkSubmissions is set to FALSE.");
            var submission = Submissions.EligibleSubmissionAEPI;
            submission.InterestedParties = new List<InterestedParty>()
            {
                new InterestedParty() { BaseNumber = "I-000241755-0", Role = InterestedPartyRole.C, NameNumber = 865900903, Name = "PUBLIC DOMAIN" }
            };

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("108", GetErrorCode(res.Response));
        }

        /// <summary>
        /// AllowPDWorkSubmissions parameter = FALSE
        /// Submitter is not authoritative for Creator IPs
        /// All Creator IPs are PD
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkFailureTests_4()
        {
            Skip.If(await IsValidationRuleEnabled("AllowPDWorkSubmissions"), "AllowPDWorkSubmissions is set to TRUE.");

            var submission = Submissions.EligibleSubmissionAEPI;
            submission.InterestedParties = new List<InterestedParty>()
                {
                    new InterestedParty() { BaseNumber = "I-003717061-7", Role = InterestedPartyRole.C, NameNumber = 705510283, Name = "BERNARDO MARTIN NEUMANN" },
                    new InterestedParty() { BaseNumber = "I-001635861-3", Role = InterestedPartyRole.C, NameNumber = 39657154, Name = "DP" }
                };

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("154", GetErrorCode(res.Response));
        }

        /// <summary>
        /// AllowNonAffiliatedSubmissions parameter = FALSE
        /// Submitter is not ISWC eligible
        /// No IPs are affiliated with a Society
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkFailureTests_5()
        {
            Skip.If(await IsValidationRuleEnabled("AllowNonAffiliatedSubmissions"), "AllowNonAffiliatedSubmissions is set to TRUE.");

            var submission = Submissions.EligibleSubmissionAEPI;
            submission.InterestedParties = new List<InterestedParty>()
                {
                    new InterestedParty() { BaseNumber = "I-003717061-7", Role = InterestedPartyRole.C, NameNumber = 705510283, Name = "BERNARDO MARTIN NEUMANN" }
                };

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("154", GetErrorCode(res.Response));
        }


        /// <summary>
        /// Submitter is not ISWC eligible
        /// Invalid Submitting Agency
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_6()
        {
            var submission = Submissions.EligibleSubmissionSACEM;
            submission.Agency = "123";

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("115", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Submitter is not ISWC eligible
        /// Invalid Agency Code
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkFailureTests_7()
        {
            Skip.IfNot(await IsValidationRuleEnabled("ValidateSubmittingAgency"), "ValidateSubmittingAgency is set to FALSE.");
            var submission = Submissions.EligibleSubmissionASCAP;
            submission.Agency = "999";

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("103", GetErrorCode(res.Response));
        }

        /// <summary>
        /// ResolveIPIBaseNumber parameter = FALSE
        /// Submitter is ISWC eligible
        /// Submitter is authoritative for IP
        /// Include status 3 IP
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkFailureTests_8()
        {
            Skip.If(await IsValidationRuleEnabled("ResolveIPIBaseNumber"), "ResolveIPIBaseNumber is set to TRUE.");

            var submission = Submissions.EligibleSubmissionAEPI;
            submission.Agency = "105";
            submission.Sourcedb = 308;
            submission.InterestedParties = new List<InterestedParty>()
                {
                    new InterestedParty() { BaseNumber = "I-000241755-0", Role = InterestedPartyRole.C, NameNumber = 259509339, Name = "STANKEVICS" },
                    new InterestedParty() { BaseNumber = "I-000210733-5", Role = InterestedPartyRole.C, NameNumber = 463887311, Name = "MUIZNIECE" },
                    new InterestedParty() { BaseNumber = "I-000229718-7", Role = InterestedPartyRole.C, NameNumber = 233795943, Name = "GOBINDRAM" },
                };

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("137", GetErrorCode(res.Response));
        }


        /// <summary>
        /// Submitter is ISWC eligible
        /// No Creator IP provided
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_9()
        {
            var submission = Submissions.EligibleSubmissionASCAP;
            submission.InterestedParties.Select(x =>
            {
                x.Role = InterestedPartyRole.E;
                return x;
            }).ToList();

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("111", GetErrorCode(res.Response));
        }


        /// <summary>
        /// Submitter is ISWC eligible
        /// More than one Original Title provided
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_10()
        {
            var submission = Submissions.EligibleSubmissionSACEM;
            submission.OtherTitles = new List<Title>() { new Title() { Title1 = "title", Type = TitleType.OT } };

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("110", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// Missing Source DB code
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkFailureTests_11()
        {
            Skip.IfNot(await IsValidationRuleEnabled("ValidateSubmittingAgency"), "ValidateSubmittingAgency is set to FALSE.");
            var sub = Submissions.EligibleSubmissionAEPI;
            var submission = new Submission
            {
                Workcode = sub.Workcode,
                OriginalTitle = sub.OriginalTitle,
                Agency = sub.Agency,
                InterestedParties = sub.InterestedParties
            };

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("138", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// Missing IP Name Number
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_12()
        {
            var submission = Submissions.EligibleSubmissionAEPI;
            submission.InterestedParties.Add(new InterestedParty
            {
                Name = InterestedParties.IP_AEPI[2].Name,
                Role = InterestedPartyRole.C,
                BaseNumber = InterestedParties.IP_AEPI[2].BaseNumber,
                NameNumber = 0
            });

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("102", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// IP Name Number is invalid
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_13()
        {
            var submission = Submissions.EligibleSubmissionAEPI;
            submission.InterestedParties.First().NameNumber = 1;

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("137", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// Inclusion of an IP which is not accepted
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_14()
        {
            var submission = Submissions.EligibleSubmissionAEPI;
            submission.InterestedParties.Add(new InterestedParty()
            {
                BaseNumber = "I-001635620-8",
                Role = InterestedPartyRole.C,
                NameNumber = 288936400,
                Name = "UNKNOWN Composer"
            });

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("148", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// Excerpt work without ISWC or title of original work
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkFailureTests_15()
        {
            var paramValue = await GetValidationParameterValue("ValidateExcerpt");
            Skip.If(paramValue != "true", "ValidateExcerpt not set to true");
            var submission = Submissions.EligibleSubmissionAEPI;
            submission.DerivedFromIswcs = new List<DerivedFrom>()
            {
                new DerivedFrom() { Iswc = "", Title = "", }
            };
            submission.DerivedWorkType = DerivedWorkType.Excerpt;

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("120", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// Composite work without ISWC or title of original work
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkFailureTests_16()
        {
            var paramValue = await GetValidationParameterValue("ValidateComposite");
            Skip.If(paramValue != "true", "ValidateComposite not set to true");
            var submission = Submissions.EligibleSubmissionAEPI;
            submission.DerivedFromIswcs = new List<DerivedFrom>()
            {
                new DerivedFrom() { Iswc = "", Title = "", }
            };
            submission.DerivedWorkType = DerivedWorkType.Composite;

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("120", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// Disambiguation flag = TRUE
        /// No disambiguation reason provided
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_17()
        {
            var submission = Submissions.EligibleSubmissionAEPI;
            submission.Disambiguation = true;
            submission.DisambiguateFrom = new List<DisambiguateFrom>()
            {
                new DisambiguateFrom() { Iswc = "T2010000015" }
            };

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("123", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Submitter is not ISWC eligible
        /// No ISWC already exists
        /// </summary>
        [RetryFact]
        public async void SubmitNewWorkFailureTests_18()
        {
            var submission = Submissions.EligibleSubmissionAEPI;
            submission.Agency = "128";
            submission.Sourcedb = 128;
            submission.PreferredIswc = "T2010000117";
            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("117", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Split Copyright Work
        /// Submitter is not ISWC eligible
        /// No ISWC already exists
        /// </summary>
        [RetryFact]
        public async void SubmitNewWorkFailureTests_19()
        {
            var submission = Submissions.EligibleSubmissionAEPI;
            submission.Agency = "128";
            submission.Sourcedb = 128;
            submission.PreferredIswc = "T2010000117";
            submission.InterestedParties.Add(InterestedParties.IP_PRS[0]);

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("117", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// Excerpt work with non-existent ISWC
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_20()
        {
            var submission = Submissions.EligibleSubmissionAEPI;
            submission.DerivedFromIswcs = new List<DerivedFrom>()
            {
                new DerivedFrom() { Iswc = "T2000000018", Title = "", }
            };
            submission.DerivedWorkType = DerivedWorkType.Excerpt;

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("119", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Submitter is not ISWC eligible
        /// Multiple potential ISWCs already exist
        /// Metadata is an exact match
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_21()
        {
            var submission = Submissions.EligibleSubmissionBMI;
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
            await client.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            submission.DisambiguateFrom = null;
            submission.Disambiguation = false;
            submission.DisambiguationReason = null;
            submission.Workcode = CreateNewWorkCode();
            submission.Agency = Submissions.EligibleSubmissionASCAP.Agency;
            submission.Sourcedb = Submissions.EligibleSubmissionASCAP.Sourcedb;
            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("160", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Workcode already exists
        /// CAR is changed to CUR
        /// Agency is not eligible
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_22()
        {
            var sub = Submissions.EligibleSubmissionSACEM;
            sub.PreferredIswc = (await client.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);
            sub.Workcode = CreateNewWorkCode();
            sub.Agency = Submissions.EligibleSubmissionAKKA.Agency;
            sub.Sourcedb = Submissions.EligibleSubmissionAKKA.Sourcedb;
            await client.AddSubmissionAsync(sub);
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);

            sub.OriginalTitle = CreateNewTitle();
            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(sub));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("144", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Agency is not authoratative for IP's
        /// Agency is a parent agency under IncludeAgenciesInEligibilityCheck.
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkFailureTests_23()
        {
            Skip.IfNot((await GetValidationParameterValue("IncludeAgenciesInEligibilityCheck")).Contains("030"), "AMAR not included in IncludeAgenciesInEligibilityCheck.");
            var sub = Submissions.EligibleSubmissionECAD;
            sub.InterestedParties = InterestedParties.IP_ASCAP;
            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(sub));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("140", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Invalid Publisher is rejected if NameNumber has a value.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_24()
        {
            var submission = Submissions.EligibleSubmissionSACEM;
            submission.InterestedParties.ElementAt(1).Name = "Publisher Unknown";
            submission.InterestedParties.ElementAt(1).Role = InterestedPartyRole.E;
            submission.InterestedParties.ElementAt(1).BaseNumber = null;
            submission.InterestedParties.ElementAt(1).NameNumber = 111;

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("137", GetErrorCode(res.Response));
        }

        /// <summary>
        /// NameNumber must be given if role is not E or AM.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_25()
        {
            var submission = Submissions.EligibleSubmissionSACEM;
            submission.InterestedParties.ElementAt(1).Name = "Artist Unknown";
            submission.InterestedParties.ElementAt(1).BaseNumber = null;
            submission.InterestedParties.ElementAt(1).NameNumber = null;

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("137", GetErrorCode(res.Response));
        }

        /// <summary>
        /// NameNumber must be valid if role is not E or AM.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_26()
        {
            var submission = Submissions.EligibleSubmissionSACEM;
            submission.InterestedParties.ElementAt(1).Name = "Artist Unknown";
            submission.InterestedParties.ElementAt(1).BaseNumber = null;
            submission.InterestedParties.ElementAt(1).NameNumber = 111;

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("137", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Submission matches existing work based on AT.
        /// OT's are not above a 70% match.
        /// Agency is not eligible.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_27()
        {
            var otherTitle = CreateNewTitleWithLettersOnly();
            var submission = Submissions.EligibleSubmissionBMI;
            submission.OriginalTitle = CreateNewTitleWithLettersOnly();
            submission.OtherTitles = new List<Title>() {
                new Title
                {
                    Title1 = otherTitle,
                    Type = TitleType.AT
                }
            };

            var iswcone = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.Workcode = CreateNewWorkCode();
            submission.OriginalTitle = CreateNewTitleWithLettersOnly();
            submission.Agency = Submissions.EligibleSubmissionAKKA.Agency;
            submission.Sourcedb = Submissions.EligibleSubmissionAKKA.Sourcedb;

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("140", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Submission matches existing ISWC's OT above 90%
        /// No title is an exact match.
        /// Agency is not eligible.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_28()
        {
            var otherTitle = CreateNewTitleWithLettersOnly();
            var submission = Submissions.EligibleSubmissionBMI;
            submission.OriginalTitle = CreateNewTitleWithLettersOnly();
            submission.OtherTitles = new List<Title>() {
                new Title
                {
                    Title1 = otherTitle,
                    Type = TitleType.AT
                }
            };

            var iswcone = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.Workcode = CreateNewWorkCode();
            submission.OriginalTitle += " x";
            submission.OtherTitles.First().Title1 += " x";
            submission.Agency = Submissions.EligibleSubmissionAKKA.Agency;
            submission.Sourcedb = Submissions.EligibleSubmissionAKKA.Sourcedb;

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("127", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Workcode already exists
        /// DisableAddUpdateSwitching is true so submission is rejected.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_29()
        {
            var sub = Submissions.EligibleSubmissionSACEM;
            sub.DisableAddUpdateSwitching = true;
            sub.PreferredIswc = (await client.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);

            sub.OriginalTitle = CreateNewTitle();
            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(sub));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("168", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Iswc field is set to an existing Iswc.
        /// AllowProvidedIswc is not set so the submission is rejected with 117.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_30()
        {
            var sub = Submissions.EligibleSubmissionSACEM;
            var existingIswc = (await client.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);

            sub.Workcode = CreateNewWorkCode();
            sub.Iswc = existingIswc;
            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(sub));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("117", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Iswc field is set to an existing Iswc.
        /// AllowProvidedIswc is true so the submission is rejected with 169.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_31()
        {
            var sub = Submissions.EligibleSubmissionSACEM;
            var existingIswc = (await client.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);

            sub.Workcode = CreateNewWorkCode();
            sub.Iswc = existingIswc;
            sub.AllowProvidedIswc = true;
            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(sub));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("169", GetErrorCode(res.Response));
        }

        /// <summary>
        /// ValidateAgentVersion is enabled
        /// Agent Version is on the approved List
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkFailureTests_32()
        {
            Skip.IfNot(await IsValidationRuleEnabled("ValidateAgentVersion"), "ValidateAgentVersion is set to FALSE.");
            var http = await GetClient();
            http.DefaultRequestHeaders.Add("agent-version", "NOT EXIST");
            var clientWithHeader = new ISWC_SubmissionClient(http);
            var submission = Submissions.EligibleSubmissionSACEM;
            var res = await Assert.ThrowsAsync<ApiException>(async () => await clientWithHeader.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("173", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Duplicate submission is rejected if EnableChecksumValidation is enabled.
        /// AKKA-LA (122) should always have EnableChecksumValidation enabled in Lookup.Agency
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkFailureTests_33()
        {
            Skip.IfNot(await IsValidationRuleEnabled("EnableChecksumValidation"), "EnableChecksumValidation is set to FALSE.");
            var submission = Submissions.EligibleSubmissionAKKA;
            await client.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("174", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Feature 7447 - Multiple society workcodes
        /// Given a submission with multiple values here, a fail of the main submission on a validation rule 
        /// should cause the additional submissions to also fail
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_34()
        {
            var sub = Submissions.EligibleSubmissionECAD;
            sub.AdditionalIdentifiers = new AdditionalIdentifiers
            {
                AgencyWorkCodes = new List<AgencyWorkCodes>()
                {
                    new AgencyWorkCodes
                    {
                        Agency = "030",
                        WorkCode = CreateNewWorkCode()
                    },
                    new AgencyWorkCodes
                    {
                        Agency = "062",
                        WorkCode = CreateNewWorkCode()
                    }
                }
            };


            sub.AdditionalIdentifiers.AgencyWorkCodes.FirstOrDefault().WorkCode += "12344321abcddcba";

            var res = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(sub));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("158", GetErrorCode(res.Response));

            var body = new List<AgencyWorkCodeSearchModel>
            {
                new AgencyWorkCodeSearchModel
                {
                    Agency = sub.AdditionalIdentifiers.AgencyWorkCodes.ElementAt(0).Agency,
                    WorkCode = sub.AdditionalIdentifiers.AgencyWorkCodes.ElementAt(0).WorkCode
                },
                new AgencyWorkCodeSearchModel { }
            };
            ICollection<ISWCMetadataBatch> searchRes = await searchClient.SearchByAgencyWorkCodeBatchAsync(body);

            foreach (var searchResult in searchRes)
            {
                Assert.Null(searchResult.SearchResults.FirstOrDefault());
            }
        }
        /// <summary>
        /// Feature 7447 - Multiple society workcodes
        /// Given a submission with multiple values here, one of the additional workcode already existing on a work should fail the submission
        /// </summary>
        [Fact]
        public async void SubmitNewWorkFailureTests_35()
        {
            var existingSub = Submissions.EligibleSubmissionECAD;

            var existingRes = await client.AddSubmissionAsync(existingSub);

            Assert.Null(existingRes.VerifiedSubmission.Rejection);

            await WaitForSubmission(existingSub.Agency, existingSub.Workcode, httpClient);

            var newSub = Submissions.EligibleSubmissionECAD;
            newSub.AdditionalIdentifiers = new AdditionalIdentifiers
            {
                AgencyWorkCodes = new List<AgencyWorkCodes>()
                {
                    new AgencyWorkCodes
                    {
                        Agency = existingSub.Agency,
                        WorkCode = existingSub.Workcode
                    },
                    new AgencyWorkCodes
                    {
                        Agency = "030",
                        WorkCode = CreateNewWorkCode()
                    }
                }
            };

            var newRes = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(newSub));

            Assert.Equal(StatusCodes.Status400BadRequest, newRes.StatusCode);
            Assert.Equal("182", GetErrorCode(newRes.Response));
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// PreviewDisambiguation is true and transaction is rejection
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_16()
        {
            var submission = Submissions.EligibleSubmissionSACEM;
            submission.OriginalTitle = CreateNewTitleWithLettersOnly();
            await client.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.PreviewDisambiguation = true;
            submission.Workcode = CreateNewWorkCode();

            var newRes = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));
            Assert.Equal(StatusCodes.Status400BadRequest, newRes.StatusCode);
            Assert.Equal("181", GetErrorCode(newRes.Response));
        }

    }
}
