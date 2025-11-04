using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using xRetry;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.SubmitNewWorkTests
{
    public class SubmitNewWorkSuccessfully_Fixture : IAsyncLifetime
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

    public class SubmitNewWorkSuccessfullyTests : TestBase, IClassFixture<SubmitNewWorkSuccessfully_Fixture>
    {
        private IISWC_SubmissionClient client;
        private HttpClient httpClient;
        private IISWC_SearchClient searchClient;

        public SubmitNewWorkSuccessfullyTests(SubmitNewWorkSuccessfully_Fixture fixture)
        {
            client = fixture.client;
            searchClient = fixture.searchClient;
            httpClient = fixture.httpClient;
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// No ISWC already exists
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_01()
        {
            var noIswcSubmission = Submissions.EligibleSubmissionSACEM;

            var res = await client.AddSubmissionAsync(noIswcSubmission);

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
            Assert.NotNull(res.VerifiedSubmission.Iswc);
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// ISWC already exists
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_02()
        {
            var sub = Submissions.EligibleSubmissionAKKA;
            var addRes = await client.AddSubmissionAsync(sub);
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);
            sub.PreferredIswc = addRes.VerifiedSubmission.Iswc.ToString();
            sub.Workcode = CreateNewWorkCode();
            var res = await client.AddSubmissionAsync(sub);

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Equal(sub.PreferredIswc, res.VerifiedSubmission.Iswc.ToString());
            Assert.Null(res.VerifiedSubmission.Rejection);
        }

        /// <summary>
        /// Submitter is ISWC eligible
        ///	Multiple ISWCs already exist
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_03()
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
            await client.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            submission.DisambiguateFrom = null;
            submission.Disambiguation = false;
            submission.DisambiguationReason = null;
            submission.Workcode = CreateNewWorkCode();


            var res = await client.AddSubmissionAsync(submission);

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
        }

        /// <summary>
        /// Submitter is not ISWC eligible
        /// ISWC already exists
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_04()
        {
            var sub = Submissions.EligibleSubmissionPRS;
            var addRes = await client.AddSubmissionAsync(sub);
            sub.PreferredIswc = addRes.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);
            sub.Workcode = CreateNewWorkCode();
            await client.AddSubmissionAsync(sub);
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);

            sub.Workcode = CreateNewWorkCode();
            sub.Agency = Submissions.EligibleSubmissionAKKA.Agency;
            sub.Sourcedb = Submissions.EligibleSubmissionAKKA.Sourcedb;

            var res = await client.AddSubmissionAsync(sub);

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
        }

        /// <summary>
        /// Split Copyright Work
        /// Submitter is ISWC eligible
        /// No ISWC already exists
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_05()
        {
            var submission = Submissions.EligibleSubmissionAKKA;
            submission.InterestedParties.Add(InterestedParties.IP_IMRO[0]);

            var res = await client.AddSubmissionAsync(submission);

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
            Assert.NotNull(res.VerifiedSubmission.Iswc);
        }

        /// <summary>
        /// Split Copyright Work
        /// Submitter is ISWC eligible
        /// ISWC already exists
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_06()
        {
            var submission = Submissions.EligibleSubmissionAKKA;
            submission.InterestedParties.Add(InterestedParties.IP_IMRO[0]);
            var addRes = await client.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.PreferredIswc = addRes.VerifiedSubmission.Iswc.ToString();
            submission.Workcode = CreateNewWorkCode();
            var res = await client.AddSubmissionAsync(submission);

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
        }

        /// <summary>
        /// Split Copyright Work
        /// Submitter is ISWC eligible
        /// Multiple ISWCs already exist
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_07()
        {
            var submission = Submissions.EligibleSubmissionAEPI;
            submission.InterestedParties.Add(InterestedParties.IP_IMRO[0]);
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
            var res = await client.AddSubmissionAsync(submission);

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
        }

        /// <summary>
        /// Split Copyright Work
        /// Submitter is not ISWC eligible
        /// ISWC already exists
        /// </summary>
        [RetryFact]
        public async void SubmitNewWorkSuccessfully_Tests_08()
        {
            var submission = Submissions.EligibleSubmissionAKKA;
            submission.InterestedParties.Add(InterestedParties.IP_IMRO[0]);
            var addRes = await client.AddSubmissionAsync(submission);
            submission.PreferredIswc = addRes.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            submission.Agency = "124";
            var res = await client.AddSubmissionAsync(submission);

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
        }


        /// <summary>
        ///  Derived work type ModifiedVersion and Derived Work Title given without an ISWC specified.
        ///  Submission is ISWC eligible.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_09()
        {
            var submission = Submissions.EligibleSubmissionIMRO;
            submission.DerivedWorkType = DerivedWorkType.ModifiedVersion;
            submission.DerivedFromIswcs = new List<DerivedFrom>
            {
                new DerivedFrom
                {
                    Title = CreateNewTitle()
                }
            };
            var res = await client.AddSubmissionAsync(submission);
            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
        }

        /// <summary>
        ///  Derived work type ModifiedVersion and no Derived Work info given.
        ///  Submission is ISWC eligible.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_10()
        {
            var submission = Submissions.EligibleSubmissionIMRO;
            submission.DerivedWorkType = DerivedWorkType.ModifiedVersion;
            submission.DerivedFromIswcs = new List<DerivedFrom>()
            {
                new DerivedFrom() { }
            };
            var res = await client.AddSubmissionAsync(submission);
            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
        }

        /// <summary>
        ///  Derived work type Excerpt and valid Derived Work info given.
        ///  Submission is ISWC eligible.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_11()
        {
            var submission = Submissions.EligibleSubmissionIMRO;
            submission.OriginalTitle = CreateNewTitleWithLettersOnly();
            var title = submission.OriginalTitle;
            var iswc = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.Workcode = CreateNewWorkCode();
            submission.DerivedWorkType = DerivedWorkType.Excerpt;
            submission.DerivedFromIswcs = new List<DerivedFrom>()
            {
                new DerivedFrom() {Iswc = iswc, Title = title }
            };
            var res = await client.AddSubmissionAsync(submission);
            Assert.NotNull(res.VerifiedSubmission);
            Assert.Empty(res.PotentialMatches);
            Assert.Null(res.VerifiedSubmission.Rejection);
        }

        /// <summary>
        /// Workcode already exists
        /// CAR is changed to CUR
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_12()
        {
            var sub = Submissions.EligibleSubmissionSACEM;
            sub.PreferredIswc = (await client.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);

            sub.OriginalTitle = CreateNewTitle();
            var res = await client.AddSubmissionAsync(sub);

            Assert.Equal(sub.Workcode, res.VerifiedSubmission.Workcode);
            Assert.Equal(sub.PreferredIswc, res.VerifiedSubmission.Iswc.ToString());
            Assert.Equal(sub.OriginalTitle, res.VerifiedSubmission.OriginalTitle);
        }


        /// <summary>
        /// Submitter is ISWC eligible
        /// Submission does not match to works with digits in titles if EnableWorkTitleContainsNumberExactMatch is enabled
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkSuccessfully_Tests_13()
        {
            Xunit.Skip.IfNot(await IsMatchingRuleEnabled("EnableWorkTitleContainsNumberExactMatch"));
            var submission = Submissions.EligibleSubmissionSACEM;
            await client.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            submission.Workcode = CreateNewWorkCode();
            submission.OriginalTitle += " a";
            var res = await client.AddSubmissionAsync(submission);

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Empty(res.PotentialMatches);
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// Submission includes Instrumentation records
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_15()
        {
            var submission = Submissions.EligibleSubmissionSACEM;
            var iswcOne = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            submission.DisambiguateFrom = new List<DisambiguateFrom>()
            {
                new DisambiguateFrom
                {
                    Iswc = iswcOne
                }
            };
            submission.Disambiguation = true;
            submission.DisambiguationReason = DisambiguationReason.DIT;
            submission.Instrumentation = new List<Instrumentation>()
            {
                new Instrumentation{Code = "BNJ"},
                new Instrumentation{Code = "BAR"}
            };
            submission.Workcode = CreateNewWorkCode();
            var res = await client.AddSubmissionAsync(submission);

            Assert.Collection(res.VerifiedSubmission.Instrumentation,
                elem1 =>
                {
                    Assert.Equal("BNJ", elem1.Code);
                },
                elem2 =>
                {
                    Assert.Equal("BAR", elem2.Code);
                });
        }

        /// <summary>
        /// Submission matches existing work based on AT.
        /// OT's are not above a 70% match.
        /// New ISWC is created for submission.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_18()
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

            var res = await client.AddSubmissionAsync(submission);
            Assert.NotEqual(iswcone, res.VerifiedSubmission.Iswc.ToString());
            Assert.Equal(0, res.PotentialMatches.Count);
        }

        /// <summary>
        /// Submission matches existing work based on AT.
        /// OT's are above a 70% match.
        /// Submission is added to existing work.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_19()
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
            submission.OriginalTitle += " aaa";

            var res = await client.AddSubmissionAsync(submission);
            Assert.Equal(iswcone, res.VerifiedSubmission.Iswc.ToString());
        }

        /// <summary>
        /// Submission matches existing work based on AT.
        /// OT's are above a 70% match.
        /// Submission is added to existing work.
        /// Agency is not eligible.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_20()
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
            submission.OriginalTitle += " aaa";
            submission.Agency = Submissions.EligibleSubmissionAKKA.Agency;
            submission.Sourcedb = Submissions.EligibleSubmissionAKKA.Sourcedb;

            var res = await client.AddSubmissionAsync(submission);
            Assert.Equal(iswcone, res.VerifiedSubmission.Iswc.ToString());
        }

        /// <summary>
        /// Submission matches existing work based on AT.
        /// OT's are above a 90% match but contain a number.
        /// New ISWC is created.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_21()
        {
            var otherTitle = CreateNewTitleWithLettersOnly();
            var submission = Submissions.EligibleSubmissionBMI;
            submission.OriginalTitle = $"{CreateNewTitleWithLettersOnly()} 1";
            submission.OtherTitles = new List<Title>() {
                new Title
                {
                    Title1 = otherTitle,
                    Type = TitleType.AT
                }
            };

            var iswcOne = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.Workcode = CreateNewWorkCode();
            submission.OriginalTitle += "2";

            var res = await client.AddSubmissionAsync(submission);
            Assert.NotEqual(iswcOne, res.VerifiedSubmission.Iswc.ToString());
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// ISWC already exists
        /// PreferredIswc is not set.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_22()
        {
            var sub = Submissions.EligibleSubmissionAKKA;
            var addFirstSubRes = await client.AddSubmissionAsync(sub);
            var firstSubIswc = addFirstSubRes.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);
            sub.Workcode = CreateNewWorkCode();
            var secondSubRes = await client.AddSubmissionAsync(sub);

            Assert.NotNull(secondSubRes.VerifiedSubmission);
            Assert.Equal(firstSubIswc, secondSubRes.VerifiedSubmission.Iswc.ToString());
            Assert.Null(secondSubRes.VerifiedSubmission.Rejection);
        }

        /// <summary>
        /// ValidateAgentVersion is enabled
        /// Agent Version is on the approved List
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkSuccessfully_Tests_23()
        {
            Xunit.Skip.IfNot(await IsValidationRuleEnabled("ValidateAgentVersion"), "ValidateAgentVersion is set to FALSE.");
            var http = await GetClient();
            http.DefaultRequestHeaders.Add("agent-version", "INT TEST");
            var clientWithHeader = new ISWC_SubmissionClient(http);
            var submission = Submissions.EligibleSubmissionSACEM;
            var res = await clientWithHeader.AddSubmissionAsync(submission);

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
        }

        /// <summary>
        /// Can resubmit deleted work.
        /// Checksum rule is enabled but checksum is deleted when work is deleted.
        /// AKKA-LA (122) should always have EnableChecksumValidation enabled in Lookup.Agency
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkSuccessfully_Tests_24()
        {
            Xunit.Skip.IfNot(await IsValidationRuleEnabled("EnableChecksumValidation"), "EnableChecksumValidation is set to FALSE.");
            var submission = Submissions.EligibleSubmissionAKKA;
            await client.AddSubmissionAsync(submission);
            var iswc = (await WaitForSubmission(submission.Agency, submission.Workcode, httpClient)).Iswc;
            await client.DeleteSubmissionAsync(iswc, submission.Agency, submission.Workcode, submission.Sourcedb, "INT TEST");
            await Task.Delay(2000);
            var res = await client.AddSubmissionAsync(submission);

            Assert.NotNull(res.VerifiedSubmission);
        }

        /// <summary>
        /// Duplicate submission is not rejected if EnableChecksumValidation rule is is enabled but EnableChecksumValidation=0 for the Agency.
        /// For test purposes AEPI (003) should never have checksum validation enabled in Lookup.Agency.
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkSuccessfully_Tests_25()
        {
            Xunit.Skip.IfNot(await IsValidationRuleEnabled("EnableChecksumValidation"), "EnableChecksumValidation is set to FALSE.");
            var submission = Submissions.EligibleSubmissionAEPI;
            var iswc = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            var res = await client.AddSubmissionAsync(submission);

            Assert.Null(res.VerifiedSubmission.Rejection);
            Assert.Equal(iswc, res.VerifiedSubmission.Iswc.ToString());
        }

        /// <summary>
        /// SubmissionDetailLevel Full returns LinkedISWCs
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_26()
        {
            var mergeClient = new ISWC_MergeClient(httpClient);
            var parentSubmission = Submissions.EligibleSubmissionAEPI;
            var parentIswc = (await client.AddSubmissionAsync(parentSubmission)).VerifiedSubmission.Iswc.ToString();

            var childSubmission = Submissions.EligibleSubmissionAEPI;
            var childIswc = (await client.AddSubmissionAsync(childSubmission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(childSubmission.Agency, childSubmission.Workcode, httpClient);
            var body = new Body
            {
                Iswcs = new string[] { childIswc }
            };
            await mergeClient.MergeISWCMetadataAsync(parentIswc, childSubmission.Agency, body);
            await Task.Delay(3000);

            parentSubmission.DetailLevel = SubmissionDetailLevel.Full;
            parentSubmission.Workcode = CreateNewWorkCode();
            var res = await client.AddSubmissionAsync(parentSubmission);

            Assert.True(res.LinkedIswcs.Count == 1);
        }

        /// <summary>
        /// SubmissionDetailLevel Core does not return LinkedISWCs
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_27()
        {
            var mergeClient = new ISWC_MergeClient(httpClient);
            var parentSubmission = Submissions.EligibleSubmissionAEPI;
            var parentIswc = (await client.AddSubmissionAsync(parentSubmission)).VerifiedSubmission.Iswc.ToString();

            var childSubmission = Submissions.EligibleSubmissionAEPI;
            var childIswc = (await client.AddSubmissionAsync(childSubmission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(childSubmission.Agency, childSubmission.Workcode, httpClient);
            var body = new Body
            {
                Iswcs = new string[] { childIswc }
            };
            await mergeClient.MergeISWCMetadataAsync(parentIswc, childSubmission.Agency, body);
            await Task.Delay(3000);

            parentSubmission.DetailLevel = SubmissionDetailLevel.Core;
            parentSubmission.Workcode = CreateNewWorkCode();
            var res = await client.AddSubmissionAsync(parentSubmission);

            Assert.Empty(res.LinkedIswcs);
        }

        /// <summary>
        /// Feature 7448 - Inclusion of Publisher Identifers in ISWC submissions
        /// Test: Submit a new eligible submission that contains multiple Publisher Identifier publishers with multiple workcodes
        /// Submission passes validation, additional identifiers present on created ISWC
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfully_Tests_29()
        {
            var sub = Submissions.EligibleSubmissionIMRO;
            sub.AdditionalIdentifiers = AdditionalIdentifersData.AI_PUB_MultipleCodes(2, 269137346, "SA");
            sub.AdditionalIdentifiers.PublisherIdentifiers.Add(AdditionalIdentifersData.AI_PUB_MultipleCodes(2, 76448253, "WB").PublisherIdentifiers.First());

            var res = await client.AddSubmissionAsync(sub);

            Assert.Null(res.VerifiedSubmission.Rejection);
            Assert.NotNull(res.VerifiedSubmission.Iswc);
            Assert.Collection(res.VerifiedSubmission.AdditionalIdentifiers.PublisherIdentifiers.OrderBy(x => x.NameNumber),
                pub1 =>
                {
                    Assert.Equal(sub.AdditionalIdentifiers.PublisherIdentifiers.OrderBy(x => x.NameNumber).First().NameNumber, pub1.NameNumber);
                    Assert.Equal(sub.AdditionalIdentifiers.PublisherIdentifiers.OrderBy(x => x.NameNumber).First().WorkCode.OrderBy(x => x).First(), pub1.WorkCode.OrderBy(x => x).First());
                    Assert.Equal(sub.AdditionalIdentifiers.PublisherIdentifiers.OrderBy(x => x.NameNumber).First().WorkCode.OrderBy(x => x).Last(), pub1.WorkCode.OrderBy(x => x).Last());
                },
                pub2 =>
                {
                    Assert.Equal(sub.AdditionalIdentifiers.PublisherIdentifiers.OrderBy(x => x.NameNumber).Last().NameNumber, pub2.NameNumber);
                    Assert.Equal(sub.AdditionalIdentifiers.PublisherIdentifiers.OrderBy(x => x.NameNumber).Last().WorkCode.OrderBy(x => x).First(), pub2.WorkCode.OrderBy(x => x).First());
                    Assert.Equal(sub.AdditionalIdentifiers.PublisherIdentifiers.OrderBy(x => x.NameNumber).Last().WorkCode.OrderBy(x => x).Last(), pub2.WorkCode.OrderBy(x => x).Last());
                });
            Assert.Equal(sub.AdditionalIdentifiers.Isrcs.First(), res.VerifiedSubmission.AdditionalIdentifiers.Isrcs.First());
            Assert.NotNull(res.VerifiedSubmission.OriginalTitle);
            Assert.Equal(sub.OriginalTitle, res.VerifiedSubmission.OriginalTitle);
            Assert.Equal(sub.InterestedParties.Count(), res.VerifiedSubmission.InterestedParties.Count());
            Assert.Collection(res.VerifiedSubmission.InterestedParties.OrderBy(x => x.BaseNumber),
                ip1 =>
                {
                    Assert.Equal(sub.InterestedParties.OrderBy(x => x.BaseNumber).First().NameNumber, ip1.NameNumber);
                    Assert.Equal(sub.InterestedParties.OrderBy(x => x.BaseNumber).First().BaseNumber, ip1.BaseNumber);
                    Assert.Equal(sub.InterestedParties.OrderBy(x => x.BaseNumber).First().Name, ip1.Name);
                },
                ip2 =>
                {
                    Assert.Equal(sub.InterestedParties.OrderBy(x => x.BaseNumber).Last().NameNumber, ip2.NameNumber);
                    Assert.Equal(sub.InterestedParties.OrderBy(x => x.BaseNumber).Last().BaseNumber, ip2.BaseNumber);
                    Assert.Equal(sub.InterestedParties.OrderBy(x => x.BaseNumber).Last().Name, ip2.Name);
                });
        }


    }
}