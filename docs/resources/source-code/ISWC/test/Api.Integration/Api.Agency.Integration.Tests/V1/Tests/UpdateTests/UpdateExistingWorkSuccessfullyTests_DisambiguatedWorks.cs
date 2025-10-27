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
    public class UpdateExistingWorkSuccessfullyTests_DisambiguatedWorks_Fixture : IAsyncLifetime
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
    public class UpdateExistingWorkSuccessfullyTests_DisambiguatedWorks : TestBase, IAsyncLifetime, IClassFixture<UpdateExistingWorkSuccessfullyTests_DisambiguatedWorks_Fixture>
    {
        private IISWC_SubmissionClient client;
        private Submission currentSubmission;
        private IISWC_SearchClient searchClient;
        private readonly HttpClient httpClient;

        public UpdateExistingWorkSuccessfullyTests_DisambiguatedWorks(UpdateExistingWorkSuccessfullyTests_DisambiguatedWorks_Fixture fixture)
        {
            client = fixture.submissionClient;
            searchClient = fixture.searchClient;
            httpClient = fixture.httpClient;
        }

        async Task IAsyncLifetime.InitializeAsync()
        {
            currentSubmission = Submissions.EligibleSubmissionBMI;
            var res = await client.AddSubmissionAsync(currentSubmission);
            currentSubmission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Updating an ISWC that was created with disambiguation information.
        /// request-source is set to PORTAL.
        /// disambiguation is true
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_DisambiguatedWorks_01()
        {
            var http = await GetClient();
            http.DefaultRequestHeaders.Add("request-source", "PORTAL");
            var clientWithHeader = new ISWC_SubmissionClient(http);
            currentSubmission.Workcode = CreateNewWorkCode();
            currentSubmission.DisambiguateFrom = new List<DisambiguateFrom>()
            {
                new DisambiguateFrom
                {
                    Iswc = currentSubmission.PreferredIswc
                }
            };
            currentSubmission.Disambiguation = true;
            currentSubmission.DisambiguationReason = DisambiguationReason.DIT;
            var disambiguatedIswc = (await clientWithHeader.AddSubmissionAsync(currentSubmission)).VerifiedSubmission.Iswc.ToString();
            currentSubmission.PreferredIswc = disambiguatedIswc;
            await WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);
            currentSubmission.DisambiguateFrom = null;
            currentSubmission.Disambiguation = true;
            currentSubmission.DisambiguationReason = null;

            var updateRes = await clientWithHeader.UpdateSubmissionAsync(disambiguatedIswc, currentSubmission);
            Assert.NotNull(updateRes.VerifiedSubmission);
        }

        /// <summary>
        /// Updating an ISWC that was created with disambiguation information.
        /// request-source is set to PORTAL.
        /// disambiguation is false, no metadata change
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_DisambiguatedWorks_02()
        {
            var http = await GetClient();
            http.DefaultRequestHeaders.Add("request-source", "PORTAL");
            var clientWithHeader = new ISWC_SubmissionClient(http);
            currentSubmission.Workcode = CreateNewWorkCode();
            currentSubmission.DisambiguateFrom = new List<DisambiguateFrom>()
            {
                new DisambiguateFrom
                {
                    Iswc = currentSubmission.PreferredIswc
                }
            };
            currentSubmission.Disambiguation = true;
            currentSubmission.DisambiguationReason = DisambiguationReason.DIT;
            var disambiguatedIswc = (await clientWithHeader.AddSubmissionAsync(currentSubmission)).VerifiedSubmission.Iswc.ToString();
            currentSubmission.PreferredIswc = disambiguatedIswc;
            await WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);
            currentSubmission.DisambiguateFrom = null;
            currentSubmission.Disambiguation = false;
            currentSubmission.DisambiguationReason = null;

            var updateRes = await clientWithHeader.UpdateSubmissionAsync(disambiguatedIswc, currentSubmission);
            Assert.NotNull(updateRes.VerifiedSubmission);
        }


        /// <summary>
        /// Updating an ISWC that was created with disambiguation information.
        /// request-source is set to PORTAL.
        /// Metadata change, disambiguation is true
        /// DisambiguateFrom is included.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_DisambiguatedWorks_03()
        {
            var http = await GetClient();
            http.DefaultRequestHeaders.Add("request-source", "PORTAL");
            var clientWithHeader = new ISWC_SubmissionClient(http);
            currentSubmission.Workcode = CreateNewWorkCode();
            var disambiguatedFromIswc = currentSubmission.PreferredIswc;
            currentSubmission.DisambiguateFrom = new List<DisambiguateFrom>()
            {
                new DisambiguateFrom
                {
                    Iswc = disambiguatedFromIswc
                }
            };
            currentSubmission.Disambiguation = true;
            currentSubmission.DisambiguationReason = DisambiguationReason.DIT;
            var disambiguatedIswc = (await clientWithHeader.AddSubmissionAsync(currentSubmission)).VerifiedSubmission.Iswc.ToString();
            currentSubmission.PreferredIswc = disambiguatedIswc;
            await WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);
            currentSubmission.DisambiguationReason = null;
            currentSubmission.OriginalTitle = CreateNewTitle();

            var updateRes = await clientWithHeader.UpdateSubmissionAsync(disambiguatedIswc, currentSubmission);
            Assert.Equal(currentSubmission.OriginalTitle, updateRes.VerifiedSubmission.OriginalTitle.ToString());
            Assert.Equal(disambiguatedFromIswc, updateRes.VerifiedSubmission.DisambiguateFrom.FirstOrDefault().Iswc);
        }

        /// <summary>
        /// Updating an ISWC that was created with disambiguation information.
        /// DisambiguatedFrom ISWC is removed and a new DisambiguatedFrom ISWC is given.
        /// request-source is set to REST.
        /// Old DismabiguatedFrom ISWC is removed and new ISWC is added.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_DisambiguatedWorks_04()
        {
            var subTwo = Submissions.EligibleSubmissionBMI;
            var newDisambiguatedFromIswc = (await client.AddSubmissionAsync(subTwo)).VerifiedSubmission.Iswc.ToString();
            var http = await GetClient();
            http.DefaultRequestHeaders.Add("request-source", "REST");
            var clientWithHeader = new ISWC_SubmissionClient(http);
            currentSubmission.Workcode = CreateNewWorkCode();
            var disambiguatedFromIswc = currentSubmission.PreferredIswc;
            currentSubmission.DisambiguateFrom = new List<DisambiguateFrom>()
            {
                new DisambiguateFrom
                {
                    Iswc = disambiguatedFromIswc
                }
            };
            currentSubmission.Disambiguation = true;
            currentSubmission.DisambiguationReason = DisambiguationReason.DIT;
            var disambiguatedIswc = (await clientWithHeader.AddSubmissionAsync(currentSubmission)).VerifiedSubmission.Iswc.ToString();
            currentSubmission.PreferredIswc = disambiguatedIswc;
            await WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);
            currentSubmission.DisambiguationReason = null;
            currentSubmission.OriginalTitle = subTwo.OriginalTitle;
            currentSubmission.DisambiguateFrom.Clear();
            currentSubmission.DisambiguateFrom.Add(
                new DisambiguateFrom
                {
                    Iswc = newDisambiguatedFromIswc
                });

            var updateRes = await clientWithHeader.UpdateSubmissionAsync(disambiguatedIswc, currentSubmission);
            Assert.Equal(currentSubmission.OriginalTitle, updateRes.VerifiedSubmission.OriginalTitle.ToString());
            Assert.Equal(1, updateRes.VerifiedSubmission.DisambiguateFrom.Count);
            Assert.Equal(newDisambiguatedFromIswc, updateRes.VerifiedSubmission.DisambiguateFrom.First().Iswc);
        }

        /// Updating an ISWC that was created with disambiguation information.
        /// A second DisambiguatedFrom ISWC is added.
        /// request-source is set to REST.
        /// DismabiguatedFrom ISWC contains both ISWC's
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_DisambiguatedWorks_05()
        {
            var subTwo = Submissions.EligibleSubmissionBMI;
            var newDisambiguatedFromIswc = (await client.AddSubmissionAsync(subTwo)).VerifiedSubmission.Iswc.ToString();
            var http = await GetClient();
            http.DefaultRequestHeaders.Add("request-source", "REST");
            var clientWithHeader = new ISWC_SubmissionClient(http);
            currentSubmission.Workcode = CreateNewWorkCode();
            var disambiguatedFromIswc = currentSubmission.PreferredIswc;
            currentSubmission.DisambiguateFrom = new List<DisambiguateFrom>()
            {
                new DisambiguateFrom
                {
                    Iswc = disambiguatedFromIswc
                }
            };
            currentSubmission.Disambiguation = true;
            currentSubmission.DisambiguationReason = DisambiguationReason.DIT;
            var disambiguatedIswc = (await clientWithHeader.AddSubmissionAsync(currentSubmission)).VerifiedSubmission.Iswc.ToString();
            currentSubmission.PreferredIswc = disambiguatedIswc;
            await WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);
            currentSubmission.DisambiguationReason = null;
            currentSubmission.OriginalTitle = subTwo.OriginalTitle;
            currentSubmission.DisambiguateFrom.Add(
                new DisambiguateFrom
                {
                    Iswc = newDisambiguatedFromIswc
                });

            var updateRes = await clientWithHeader.UpdateSubmissionAsync(disambiguatedIswc, currentSubmission);
            Assert.Equal(currentSubmission.OriginalTitle, updateRes.VerifiedSubmission.OriginalTitle.ToString());
            Assert.Equal(2, updateRes.VerifiedSubmission.DisambiguateFrom.Count);
            Assert.Contains(updateRes.VerifiedSubmission.DisambiguateFrom, x => x.Iswc.Equals(disambiguatedFromIswc));
            Assert.Contains(updateRes.VerifiedSubmission.DisambiguateFrom, x => x.Iswc.Equals(newDisambiguatedFromIswc));
        }

        /// Updating an ISWC that was created with disambiguation information.
        /// DisambiguatedFrom data is included
        /// request-source is set to REST.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_DisambiguatedWorks_06()
        {
            var http = await GetClient();
            http.DefaultRequestHeaders.Add("request-source", "REST");
            var clientWithHeader = new ISWC_SubmissionClient(http);
            currentSubmission.Workcode = CreateNewWorkCode();
            var disambiguatedFromIswc = currentSubmission.PreferredIswc;
            currentSubmission.DisambiguateFrom = new List<DisambiguateFrom>()
            {
                new DisambiguateFrom
                {
                    Iswc = disambiguatedFromIswc
                }
            };
            currentSubmission.Disambiguation = true;
            currentSubmission.DisambiguationReason = DisambiguationReason.DIT;
            var disambiguatedIswc = (await clientWithHeader.AddSubmissionAsync(currentSubmission)).VerifiedSubmission.Iswc.ToString();
            currentSubmission.PreferredIswc = disambiguatedIswc;
            await WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);
            currentSubmission.OriginalTitle = CreateNewTitle();

            var updateRes = await clientWithHeader.UpdateSubmissionAsync(disambiguatedIswc, currentSubmission);
            Assert.Equal(currentSubmission.OriginalTitle, updateRes.VerifiedSubmission.OriginalTitle.ToString());
        }
    }
}
