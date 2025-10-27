using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using xRetry;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.UpdateTests
{
    public class UpdateExistingWorkSuccessfullyTests_IP_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient submissionClient;
        public IISWC_SearchClient searchClient;
        public HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            submissionClient = new ISWC_SubmissionClient(httpClient);
            searchClient = new ISWC_SearchClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// 2. Update existing ISWC eligible work successfully
    /// </summary>
    public class UpdateExistingWorkSuccessfullyTests_IP : TestBase, IAsyncLifetime, IClassFixture<UpdateExistingWorkSuccessfullyTests_IP_Fixture>
    {
        private IISWC_SubmissionClient client;
        private Submission currentSubmission;
        private IISWC_SearchClient searchClient;
        private readonly HttpClient httpClient;

        public UpdateExistingWorkSuccessfullyTests_IP(UpdateExistingWorkSuccessfullyTests_IP_Fixture fixture)
        {
            client = fixture.submissionClient;
            searchClient = fixture.searchClient;
            httpClient = fixture.httpClient;
        }

        async Task IAsyncLifetime.InitializeAsync()
        {
            long nonAffiliatedNameNumber = 240971278;
            currentSubmission = Submissions.EligibleSubmissionBMI;
            currentSubmission.InterestedParties.Add(InterestedParties.IP_IMRO.First());
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
            var res = await client.AddSubmissionAsync(currentSubmission);
            currentSubmission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 2.1 Add Creator IP affiliated with the submitter
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_IP_01()
        {
            currentSubmission.InterestedParties.Add(InterestedParties.IP_BMI[2]);
            var updateRes = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);

            Assert.Equal(currentSubmission.InterestedParties.Count, updateRes.VerifiedSubmission.InterestedParties.Where(x => x.Role == InterestedPartyRole.C).Count());

        }

        /// <summary>
        /// 2.2 Edit Creator IP affiliated with the submitter
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_IP_02()
        {
            currentSubmission.InterestedParties.Where(i => i.NameNumber == currentSubmission.InterestedParties.First().NameNumber).Select(c =>
            {
                c.Role = InterestedPartyRole.AM;
                return c;
            }).ToList();

            var updateRes = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);

            Assert.True(updateRes.VerifiedSubmission.InterestedParties.Where(x => x.NameNumber == currentSubmission.InterestedParties.First().NameNumber && x.Role == InterestedPartyRole.E).Count() > 0);
        }

        /// <summary>
        /// 2.3 Delete Creator IP affiliated with the submitter
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_IP_03()
        {
            var removeIP = currentSubmission.InterestedParties.First();
            currentSubmission.InterestedParties = currentSubmission.InterestedParties.Where(x => x.NameNumber != removeIP.NameNumber).ToList();

            var updateRes = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);
            Assert.All(updateRes.VerifiedSubmission.InterestedParties, ip =>
            {
                Assert.False(ip.NameNumber == removeIP.NameNumber);
            });
            Assert.NotNull(updateRes.VerifiedSubmission.InterestedParties.FirstOrDefault(ip => ip.NameNumber == currentSubmission.InterestedParties.First().NameNumber));

        }

        /// <summary>
        /// 2.7 Update a Creator role code to a role not within the same role class (IP affiliated with the submitter)
        /// Match for updated metadata is found within system
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_IP_04()
        {
            currentSubmission.InterestedParties.Where(i => i.NameNumber == currentSubmission.InterestedParties.First().NameNumber).Select(c =>
            {
                c.Role = InterestedPartyRole.E;
                return c;
            }).ToList();

            var sub = Submissions.EligibleSubmissionBMI;
            sub.InterestedParties = currentSubmission.InterestedParties;
            sub.OriginalTitle = currentSubmission.OriginalTitle;
            var res = await client.AddSubmissionAsync(sub);
            sub.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);

            var updateRes = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);

            Assert.Equal(2, updateRes.PotentialMatches.Count);
            Assert.True(updateRes.VerifiedSubmission.InterestedParties.Where(x => x.NameNumber == currentSubmission.InterestedParties.First().NameNumber
            && x.Role == InterestedPartyRole.E).Count() > 0);
        }

        /// <summary>
        /// Eligible submitter adds an IP affiliated with another agency in update.
        /// Now eligible agency can update their originally non-eligible submission.
        /// Bug 3493
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_IP_05()
        {
            var workCodeOne = currentSubmission.Workcode;

            currentSubmission.Agency = Submissions.EligibleSubmissionAEPI.Agency;
            currentSubmission.Sourcedb = Submissions.EligibleSubmissionAEPI.Sourcedb;
            currentSubmission.Workcode = CreateNewWorkCode();
            var workCodeTwo = currentSubmission.Workcode;
            await client.AddSubmissionAsync(currentSubmission);
            await WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);

            currentSubmission.Agency = Submissions.EligibleSubmissionBMI.Agency;
            currentSubmission.Sourcedb = Submissions.EligibleSubmissionBMI.Sourcedb;
            currentSubmission.Workcode = workCodeOne;
            currentSubmission.InterestedParties.Add(InterestedParties.IP_AEPI[0]);
            await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);
            await Task.Delay(2000);

            currentSubmission.Agency = Submissions.EligibleSubmissionAEPI.Agency;
            currentSubmission.Sourcedb = Submissions.EligibleSubmissionAEPI.Sourcedb;
            currentSubmission.Workcode = workCodeTwo;
            currentSubmission.OriginalTitle = CreateNewTitle();
            var updateRes = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);

            Assert.Null(updateRes.VerifiedSubmission.Rejection);
            Assert.Equal(currentSubmission.OriginalTitle, updateRes.VerifiedSubmission.OriginalTitle);
        }

        /// <summary>
        /// Parent agency in IncludeAgenciesInEligibilityCheck can update a submission to remove a child agencies IP.
        /// </summary>
        [SkippableFact]
        public async void UpdateExistingWorkSuccessfullyTests_IP_06()
        {
            Skip.IfNot((await GetValidationParameterValue("IncludeAgenciesInEligibilityCheck")).Contains("030"), "AMAR not included in IncludeAgenciesInEligibilityCheck.");
            var sub = Submissions.EligibleSubmissionECAD;
            var addRes = await client.AddSubmissionAsync(sub);
            sub.PreferredIswc = addRes.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);
            var removedNameNumber = sub.InterestedParties.Last().NameNumber;
            sub.InterestedParties.Remove(sub.InterestedParties.Last());
            var res = await client.UpdateSubmissionAsync(sub.PreferredIswc, sub);

            Assert.DoesNotContain(res.VerifiedSubmission.InterestedParties, x => x.NameNumber == removedNameNumber);
        }

        /// <summary>
        /// Test for Bug 4258
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_IP_07()
        {
            var sub = Submissions.EligibleSubmissionBMI;
            sub.InterestedParties.Add(InterestedParties.IP_BMI[2]);
            sub.InterestedParties.ElementAt(2).Role = InterestedPartyRole.E;

            var res = await client.AddSubmissionAsync(sub);
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);

            sub.OriginalTitle = currentSubmission.OriginalTitle;
            await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, sub);

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
            Assert.NotNull(res.VerifiedSubmission.Iswc);
        }

        /// <summary>
        /// Multiple eligible works on the ISWC
        /// Update agency's most recent submission to delete an IP not affiliated with submitting society
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_IP_08()
        {
            var subTwo = Submissions.EligibleSubmissionIMRO;
            subTwo.OriginalTitle = currentSubmission.OriginalTitle;
            subTwo.InterestedParties = currentSubmission.InterestedParties;
            subTwo.PreferredIswc = currentSubmission.PreferredIswc;
            await client.AddSubmissionAsync(subTwo);
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);
            subTwo.InterestedParties.Remove(
                subTwo.InterestedParties.First(x => x.NameNumber == InterestedParties.IP_BMI.First().NameNumber));

            var res = await client.UpdateSubmissionAsync(subTwo.PreferredIswc, subTwo);

            Assert.DoesNotContain(res.VerifiedSubmission.InterestedParties, x => x.NameNumber == InterestedParties.IP_BMI.First().NameNumber);
        }

        /// <summary>
        /// Multiple eligible works on the ISWC
        /// Update agency's most recent submission to delete public domain IP.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_IP_09()
        {
            var subTwo = Submissions.EligibleSubmissionIMRO;
            subTwo.OriginalTitle = currentSubmission.OriginalTitle;
            subTwo.InterestedParties = currentSubmission.InterestedParties;
            subTwo.PreferredIswc = currentSubmission.PreferredIswc;
            await client.AddSubmissionAsync(subTwo);
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);
            currentSubmission.InterestedParties.Remove(
                currentSubmission.InterestedParties.First(x => x.NameNumber == 865900903));

            var res = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);

            Assert.DoesNotContain(res.VerifiedSubmission.InterestedParties, x => x.NameNumber == 865900903);
        }

        /// <summary>
        /// Multiple eligible works on the ISWC
        /// Delete an IP not affiliated with submitting society
        /// Submitting agency is parent agency under IncludeAgenciesInEligibilityCheck
        /// </summary>
        [SkippableFact]
        public async void UpdateExistingWorkSuccessfullyTests_IP_10()
        {
            Skip.IfNot((await GetValidationParameterValue("IncludeAgenciesInEligibilityCheck")).Contains("030"), "AMAR not included in IncludeAgenciesInEligibilityCheck.");
            var sub = Submissions.EligibleSubmissionECAD;
            var workcode = sub.Workcode;
            sub.InterestedParties.Add(InterestedParties.IP_AKKA.First());
            var addRes = await client.AddSubmissionAsync(sub);
            sub.PreferredIswc = addRes.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);
            sub.Agency = Submissions.EligibleSubmissionAKKA.Agency;
            sub.Sourcedb = Submissions.EligibleSubmissionAKKA.Sourcedb;
            sub.Workcode = CreateNewWorkCode();
            await client.AddSubmissionAsync(sub);
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);
            sub.Agency = Submissions.EligibleSubmissionECAD.Agency;
            sub.Sourcedb = Submissions.EligibleSubmissionECAD.Sourcedb;
            sub.InterestedParties.Remove(sub.InterestedParties.Last());
            sub.Workcode = workcode;
            var res = await client.UpdateSubmissionAsync(sub.PreferredIswc, sub);

            Assert.DoesNotContain(res.VerifiedSubmission.InterestedParties, x => x.NameNumber == InterestedParties.IP_AKKA.First().NameNumber);
        }

        /// <summary>
        /// Multiple eligible works on the ISWC
        /// Update agency's most recent submission to delete non-society IP
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_IP_11()
        {
            var subTwo = Submissions.EligibleSubmissionIMRO;
            subTwo.OriginalTitle = currentSubmission.OriginalTitle;
            subTwo.InterestedParties = currentSubmission.InterestedParties;
            subTwo.PreferredIswc = currentSubmission.PreferredIswc;
            await client.AddSubmissionAsync(subTwo);
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);
            currentSubmission.InterestedParties.Remove(
                currentSubmission.InterestedParties.First(x => x.NameNumber == 240971278));

            var res = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);

            Assert.DoesNotContain(res.VerifiedSubmission.InterestedParties, x => x.NameNumber == 240971278);
        }

        /// <summary>
        /// Work is updated to remove only eligible creator and add an inelligible creator.
        /// Updated work matches ISWC from another agency.
        /// Inellgibile work is moved to that ISWC instead of merging.
        /// currentSubmission is not applicable in this test.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_IP_12()
        {
            var subOne = Submissions.EligibleSubmissionBMI;
            subOne.InterestedParties.Remove(subOne.InterestedParties.Last());
            subOne.PreferredIswc = (await client.AddSubmissionAsync(subOne)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(subOne.Agency, subOne.Workcode, httpClient);

            var subTwo = Submissions.EligibleSubmissionIMRO;
            subTwo.InterestedParties.Remove(subTwo.InterestedParties.Last());
            subTwo.PreferredIswc = (await client.AddSubmissionAsync(subTwo)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);

            subTwo.OriginalTitle = subOne.OriginalTitle;
            subTwo.InterestedParties = subOne.InterestedParties;
            subTwo.PreferredIswc = subOne.PreferredIswc;
            var updateRes = await client.UpdateSubmissionAsync(subTwo.PreferredIswc, subTwo);
            await Task.Delay(2000);

            var searchRes = await searchClient.SearchByISWCAsync(subOne.PreferredIswc);
            Assert.NotNull(searchRes.Works.FirstOrDefault(x => x.Workcode == subTwo.Workcode));
            Assert.Equal(2, searchRes.Works.Count);
        }

        /// <summary>
        /// Eligible society can remove PD creator.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_IP_13()
        {
            currentSubmission.InterestedParties.Remove(currentSubmission.InterestedParties.First(x => x.NameNumber == 865900903));

            var updateRes = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);

            Assert.Null(updateRes.VerifiedSubmission.InterestedParties.FirstOrDefault(x => x.NameNumber == 865900903));
        }

        /// <summary>
        /// Eligible society can remove Non-Society creator.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_IP_14()
        {
            currentSubmission.InterestedParties.Remove(currentSubmission.InterestedParties.First(x => x.NameNumber == 240971278));

            var updateRes = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);

            Assert.Null(updateRes.VerifiedSubmission.InterestedParties.FirstOrDefault(x => x.NameNumber == 240971278));
        }

        /// <summary>
        /// Eligible society can remove Non-Afilliated creator since it is on their most recent submission.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_IP_15()
        {
            currentSubmission.InterestedParties.Remove(currentSubmission.InterestedParties.First(
                x => x.NameNumber == InterestedParties.IP_IMRO.First().NameNumber));

            var updateRes = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);

            Assert.Null(updateRes.VerifiedSubmission.InterestedParties.FirstOrDefault(
                x => x.NameNumber == InterestedParties.IP_IMRO.First().NameNumber));
        }
    }
}
