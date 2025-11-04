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
    public class SubmitNewWorkSuccessfullyTests_IP_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient client;
        public HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            client = new ISWC_SubmissionClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class SubmitNewWorkSuccessfullyTests_IP : TestBase, IClassFixture<SubmitNewWorkSuccessfullyTests_IP_Fixture>
    {
        private IISWC_SubmissionClient client;
        private HttpClient httpClient;

        public SubmitNewWorkSuccessfullyTests_IP(SubmitNewWorkSuccessfullyTests_IP_Fixture fixture)
        {
            client = fixture.client;
            httpClient = fixture.httpClient;
        }

        /// <summary>
        /// ResolveIPIBaseNumber parameter = TRUE
        /// Submitter is ISWC eligible
        /// Include status 3 IP
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkSuccessfullyTests_IP_01()
        {
            Xunit.Skip.IfNot(await IsValidationRuleEnabled("ResolveIPIBaseNumber"), "ResolveIPIBaseNumber is set to FALSE.");

            var submission = Submissions.EligibleSubmissionAEPI;
            submission.InterestedParties.Add(new InterestedParty()
            {
                BaseNumber = "I-000223516-5",
                NameNumber = 4652217,
                Role = InterestedPartyRole.C,
                Name = "BURY"
            });
            var res = await client.AddSubmissionAsync(submission);

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
        }


        /// <summary>
        /// ResolveIPIBaseNumber parameter = TRUE
        /// Submitter is not ISWC eligible
        /// Include status 3 IP
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkSuccessfullyTests_IP_02()
        {
            Xunit.Skip.IfNot(await IsValidationRuleEnabled("ResolveIPIBaseNumber"), "ResolveIPIBaseNumber is set to FALSE.");

            var submission = Submissions.EligibleSubmissionAKKA;
            submission.InterestedParties = new List<InterestedParty>()
            {
                new InterestedParty()
                {
                    BaseNumber = "I-000223516-5",
                    NameNumber = 4652217,
                    Role = InterestedPartyRole.C,
                    Name = "BURY"
                }
            };
            var res = await client.AddSubmissionAsync(submission);

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
        }

        /// <summary>
        /// ResolveIPIBaseNumber parameter = FALSE
        /// Submitter is ISWC eligible
        /// Submitter is not authoritative for IP
        /// Include status 3 IP
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkSuccessfullyTests_IP_03()
        {
            Xunit.Skip.If(await IsValidationRuleEnabled("ResolveIPIBaseNumber"), "ResolveIPIBaseNumber is set to TRUE.");

            var submission = Submissions.EligibleSubmissionAEPI;
            submission.InterestedParties = new List<InterestedParty>()
            {
                new InterestedParty()
                {
                    BaseNumber = "I-001544287-0",
                    NameNumber = 67819439,
                    Name = "DERVENIOTIS",
                    Role = InterestedPartyRole.C
                },
                new InterestedParty()
                {
                    BaseNumber = "I-000226471-1",
                    NameNumber = 15871486,
                    Name = "KARANIKOLAS",
                    Role = InterestedPartyRole.C
                }
            };

            submission.InterestedParties.Add(new InterestedParty()
            {
                BaseNumber = "I-001084226-7",
                NameNumber = 33994952,
                Role = InterestedPartyRole.C,
                Name = "KANVIOTIS"
            });

            var res = await client.AddSubmissionAsync(submission);

            Assert.NotNull(res.VerifiedSubmission);
        }


        /// <summary>
        /// ResolveIPIBaseNumber parameter = FALSE
        /// Submitter is not ISWC eligible
        /// Include status 3 IP
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkSuccessfullyTests_IP_04()
        {
            Xunit.Skip.If(await IsValidationRuleEnabled("ResolveIPIBaseNumber"), "ResolveIPIBaseNumber is set to TRUE.");

            var submission = Submissions.EligibleSubmissionAKKA;
            submission.InterestedParties = new List<InterestedParty>()
             {
                new InterestedParty() { BaseNumber = "I-000225797-6", Role = InterestedPartyRole.C, NameNumber = 278445528, Name = "KALNINS" },
                new InterestedParty() { BaseNumber = "I-001626855-4", Role = InterestedPartyRole.C, NameNumber = 86909626, Name = "PAULS" },
                new InterestedParty() { BaseNumber = "I-001635861-3", Role = InterestedPartyRole.C, NameNumber = 865900903, Name = "PUBLIC DOMAIN" },
                new InterestedParty() { BaseNumber = "I-000229599-8", Role = InterestedPartyRole.C, NameNumber = 4636507, Name = "MIECZYSLAW BUROWSKI" }
            };
            var addRes = await client.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.InterestedParties.Add(new InterestedParty()
            {
                BaseNumber = "I-000223516-5",
                NameNumber = 4652217,
                Role = InterestedPartyRole.C,
                Name = "BURY"
            });
            submission.Agency = "124";
            submission.PreferredIswc = addRes.VerifiedSubmission.Iswc.ToString();

            var res = await client.AddSubmissionAsync(submission);

            Assert.NotNull(res.VerifiedSubmission);
        }

        /// <summary>
        /// AllowPDWorkSubmissions parameter = TRUE
        /// Submitter is not authoritative for Creator IPs
        /// All Creator IPs are PD
        /// At least one Creator is not one of the generic four IPs(Public Domain, DP, TRAD, Unknown Composer Author)
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkSuccessfullyTests_IP_05()
        {
            Xunit.Skip.IfNot(await IsValidationRuleEnabled("AllowPDWorkSubmissions"), "AllowPDWorkSubmissions is set to FALSE.");

            var submission = Submissions.EligibleSubmissionAKKA;
            submission.InterestedParties = new List<InterestedParty>()
            {
                new InterestedParty() { BaseNumber = "I-001635861-3", Role = InterestedPartyRole.C, NameNumber = 865900903, Name = "PUBLIC DOMAIN" },
                new InterestedParty() { BaseNumber = "I-000059818-3", Role = InterestedPartyRole.C, NameNumber = 17323020, Name = "LARRA WETORET" }
            };

            var res = await client.AddSubmissionAsync(submission);

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
        }

        /// <summary>
        /// AllowNonAffiliatedSubmissions parameter = TRUE
        /// Submitter is considered ISWC eligible
        /// No IPs are affiliated with a Society
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkSuccessfullyTests_IP_06()
        {
            Xunit.Skip.IfNot(await IsValidationRuleEnabled("AllowNonAffiliatedSubmissions"), "AllowNonAffiliatedSubmissions is set to FALSE.");

            var submission = Submissions.EligibleSubmissionAKKA;
            submission.InterestedParties = new List<InterestedParty>()
            {
                new InterestedParty() { BaseNumber = "I-004730176-6", Role = InterestedPartyRole.C, NameNumber = 862598198, Name = "M.S AND SONS PRODUCTIONS LTD" }
            };

            var res = await client.AddSubmissionAsync(submission);

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
        }

        /// <summary>
        /// IP's have an agreement with an agency included in IncludeAgenciesInEligibilityCheck.
        /// ECAD is submitting agency
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkSuccessfullyTests_IP_07()
        {
            Xunit.Skip.IfNot((await GetValidationParameterValue("IncludeAgenciesInEligibilityCheck")).Contains("030"), "AMAR not included in IncludeAgenciesInEligibilityCheck.");
            var noIswcSubmission = Submissions.EligibleSubmissionECAD;

            var res = await client.AddSubmissionAsync(noIswcSubmission);

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
            Assert.NotNull(res.VerifiedSubmission.Iswc);
        }


        /// <summary>
        /// Invalid publisher is excluded from submission if no NameNumber is given.
        /// Applies to E and AM roles.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfullyTests_IP_08()
        {
            var submission = Submissions.EligibleSubmissionSACEM;
            submission.InterestedParties = InterestedParties.IP_SACEM;
            submission.InterestedParties.ElementAt(1).Name = "Publisher Unknown";
            submission.InterestedParties.ElementAt(1).Role = InterestedPartyRole.E;
            submission.InterestedParties.ElementAt(1).BaseNumber = null;
            submission.InterestedParties.ElementAt(1).NameNumber = null;

            submission.InterestedParties.ElementAt(2).Name = "Publisher Unknown";
            submission.InterestedParties.ElementAt(2).Role = InterestedPartyRole.AM;
            submission.InterestedParties.ElementAt(2).BaseNumber = null;
            submission.InterestedParties.ElementAt(2).NameNumber = null;

            var res = await client.AddSubmissionAsync(submission);
            Assert.False(string.IsNullOrWhiteSpace(res.VerifiedSubmission.Iswc.ToString()));
            Assert.Equal(1, res.VerifiedSubmission.InterestedParties.Count);
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// One IP is not affiliated with any society.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfullyTests_IP_09()
        {
            long nonAffiliatedNameNumber = 240971278;
            var sub = Submissions.EligibleSubmissionAKKA;
            sub.InterestedParties.Add(new InterestedParty
            {
                NameNumber = nonAffiliatedNameNumber,
                Role = InterestedPartyRole.CA
            });
            var res = await client.AddSubmissionAsync(sub);

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Equal("099", res.VerifiedSubmission.InterestedParties.Where(
                x => x.NameNumber == nonAffiliatedNameNumber).FirstOrDefault().Affiliation);
        }

        /// <summary>
        ///  ISWC exists
        ///  Submission has publishers not in original submission
        ///  Submitter is not ISWC eligible
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfullyTests_IP_10()
        {
            var submission = Submissions.EligibleSubmissionASCAP;
            var iswc = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.Workcode = CreateNewWorkCode();
            submission.Agency = Submissions.EligibleSubmissionBMI.Agency;
            submission.Sourcedb = Submissions.EligibleSubmissionBMI.Sourcedb;
            var publisher = InterestedParties.IP_BMI[0];
            publisher.Role = InterestedPartyRole.E;
            submission.InterestedParties.Add(publisher);
            var res = await client.AddSubmissionAsync(submission);
            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
            Assert.Equal(iswc, res.VerifiedSubmission.Iswc.ToString());
            Assert.Contains(res.VerifiedSubmission.InterestedParties, x => x.Role.Equals(InterestedPartyRole.E) && x.NameNumber.Equals(publisher.NameNumber));
        }


        /// <summary>
        /// PG IP is included in submission.
        /// IP's within the group are not returned in verified submission.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfullyTests_IP_11()
        {
            var submission = Submissions.EligibleSubmissionSACEM;
            submission.InterestedParties.Add(new InterestedParty
            {
                NameNumber = 568562507,
                Role = InterestedPartyRole.CA,
                BaseNumber = "I-001651337-2"
            });

            var res = await client.AddSubmissionAsync(submission);

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Equal(3, res.VerifiedSubmission.InterestedParties.Count);
        }

        /// <summary>
        /// PG IP is included in submission.
        /// Follow-up submission matches to existing based on PG IP.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfullyTests_IP_12()
        {
            var submission = Submissions.EligibleSubmissionSACEM;
            submission.InterestedParties.Add(new InterestedParty
            {
                NameNumber = 568562507,
                Role = InterestedPartyRole.CA,
                BaseNumber = "I-001651337-2"
            });

            var iswcOne = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.Workcode = CreateNewWorkCode();
            var iswcTwo = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();

            Assert.Equal(iswcOne, iswcTwo);
        }

        /// <summary>
        /// PG IP is included in submission.
        /// Follow-up submission matches to existing based on IP's within the PG group.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfullyTests_IP_13()
        {
            var submission = Submissions.EligibleSubmissionSACEM;
            submission.InterestedParties.Add(new InterestedParty
            {
                NameNumber = 568562507,
                Role = InterestedPartyRole.CA,
                BaseNumber = "I-001651337-2"
            });

            var iswcOne = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.Workcode = CreateNewWorkCode();
            submission.InterestedParties.Remove(submission.InterestedParties.First(x => x.NameNumber == 568562507));
            submission.InterestedParties.Add(new InterestedParty
            {
                NameNumber = 45529476,
                Role = InterestedPartyRole.CA,
                BaseNumber = "I-001135213-7"
            });
            submission.InterestedParties.Add(new InterestedParty
            {
                NameNumber = 251745079,
                Role = InterestedPartyRole.CA,
                BaseNumber = "I-001651337-2"
            });
            var iswcTwo = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();

            Assert.Equal(iswcOne, iswcTwo);
        }
    }
}