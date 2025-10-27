using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SpanishPoint.Azure.Iswc.Api.Publisher.Integration.Tests.V1;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using xRetry;
using Xunit;
using agency = SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace SpanishPoint.Azure.Iswc.Api.Publisher.Integration.Tests.V1.Tests.SearchTests
{

    public class ContextSearchTests_Fixture : IAsyncLifetime
    {
        public agency.IISWC_SubmissionClient submissionClient;
        public IISWC_SearchClient searchClient;
        private HttpClient httpAgencyClient;
        private HttpClient httpSearchClient;
        public agency.Submission submissionOne;
        public agency.Submission submissionTwo;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpAgencyClient = await TestBase.GetAgencyClient();
            httpSearchClient = await TestBase.GetClient();
            submissionClient = new agency.ISWC_SubmissionClient(httpAgencyClient);
            searchClient = new ISWC_SearchClient(httpSearchClient);
            submissionOne = Submissions.EligibleSubmissionIMRO;
            submissionOne.PreferredIswc = (await submissionClient.AddSubmissionAsync(submissionOne)).VerifiedSubmission.Iswc.ToString();
            await TestBase.WaitForSubmission(submissionOne.Agency, submissionOne.Workcode, httpAgencyClient);

            submissionTwo = Submissions.EligibleSubmissionBMI;
            submissionTwo.PreferredIswc = (await submissionClient.AddSubmissionAsync(submissionTwo)).VerifiedSubmission.Iswc.ToString();
            await TestBase.WaitForSubmission(submissionTwo.Agency, submissionTwo.Workcode, httpAgencyClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpAgencyClient.Dispose();
            httpSearchClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class ContextSearchTests : TestBase, IClassFixture<ContextSearchTests_Fixture>
    {
        private readonly IISWC_SearchClient searchClient;
        private readonly agency.Submission submissionOne;
        private readonly agency.Submission submissionTwo;

        public ContextSearchTests(ContextSearchTests_Fixture fixture)
        {
            submissionOne = fixture.submissionOne;
            submissionTwo = fixture.submissionTwo;
            searchClient = fixture.searchClient;
        }

        /// <summary>
        /// IP Context Search by Last Name and Name Number
        /// </summary>
        [Fact]
        public async void ContextSearchTests_WithLastNameAndNameNumber_ReturnsSuccess()
        {
            var title = submissionOne.OriginalTitle;
            var ip = submissionOne.InterestedParties.ElementAt(0);
            var searchModel = new IPContextSearchModel
            {
                LastName = ip.LastName,
                NameNumber = ip.NameNumber,
                Works = new List<Works>
                {
                    new Works
                    {
                        Titles = new List<Title>
                        {
                            new Title { Title1 = title, Type = TitleType.OT }
                        }
                    }
                }
            };

            var res = await searchClient.ContextSearchAsync(searchModel);
 
            Assert.NotNull(res);
            Assert.Equal(ip.LastName, res.LastName);
            Assert.Equal(ip.NameNumber, res.NameNumber);
            Assert.NotEmpty(res.MatchingIswcs);
            Assert.Single(res.MatchingIswcs);
            var matchingIswc = res.MatchingIswcs.First();
            Assert.Equal(title, matchingIswc.OriginalTitle);
        }

        /// <summary>
        /// IP Context Search by Last Name
        /// </summary>
        [Fact]
        public async void ContextSearchTests_WithLastName_ReturnsSuccess()
        {
            var title = submissionOne.OriginalTitle;
            var ip = submissionOne.InterestedParties.ElementAt(0);
            var searchModel = new IPContextSearchModel
            {
                LastName = ip.LastName,
                Works = new List<Works>
                {
                    new Works
                    {
                        Titles = new List<Title>
                        {
                            new Title { Title1 = title, Type = TitleType.OT }
                        }
                    }
                }
            };

            var res = await searchClient.ContextSearchAsync(searchModel);

            Assert.NotNull(res);
            Assert.Equal(ip.LastName, res.LastName);
            Assert.Equal(ip.NameNumber, res.NameNumber);
            Assert.NotEmpty(res.MatchingIswcs);
            Assert.Single(res.MatchingIswcs);
            var matchingIswc = res.MatchingIswcs.First();
            Assert.Equal(title, matchingIswc.OriginalTitle);
        }

        /// <summary>
        /// IP Context Search where IP is not found
        /// </summary>
        [Fact]
        public async void ContextSearchTests_WithLastName_ReturnsRejection()
        {
            var title = submissionOne.OriginalTitle;
            var ip = submissionOne.InterestedParties.ElementAt(0);
            var searchModel = new IPContextSearchModel
            {
                LastName = "Nonexistent",
                Works = new List<Works>
                {
                    new Works
                    {
                        Titles = new List<Title>
                        {
                            new Title { Title1 = title, Type = TitleType.OT }
                        }
                    }
                }
            };

            var res = await Assert.ThrowsAsync<ApiException>(
                async () => await searchClient.ContextSearchAsync(searchModel));

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Contains("IP not found.", res.Response);
        }

        /// <summary>
        /// IP Context Search for multiple IPs that exist
        /// </summary>
        [Fact]
        public async void ContextBatchSearchBatchTests_WithMultipleSubmissions_ReturnsSuccess()
        {
            var searchModels = new List<IPContextSearchModel>
            {
                new IPContextSearchModel
                {
                    LastName = submissionOne.InterestedParties.First().LastName,
                    NameNumber = submissionOne.InterestedParties.First().NameNumber,
                    Works = new List<Works>
                    {
                        new Works
                        {
                            Titles = new List<Title>
                            {
                                new Title { Title1 = submissionOne.OriginalTitle, Type = TitleType.OT }
                            }
                        }
                    }
                },
                new IPContextSearchModel
                {
                    LastName = submissionTwo.InterestedParties.First().LastName,
                    NameNumber = submissionTwo.InterestedParties.First().NameNumber,
                    Works = new List<Works>
                    {
                        new Works
                        {
                            Titles = new List<Title>
                            {
                                new Title { Title1 = submissionTwo.OriginalTitle, Type = TitleType.OT }
                            }
                        }
                    }
                }
            };

            var batchRes = await searchClient.ContextSearchBatchAsync(searchModels);

            Assert.NotNull(batchRes);
            Assert.Equal(2, batchRes.Count);

            var firstBatchResult = batchRes.First();
            Assert.NotNull(firstBatchResult.SearchResult);
            Assert.Equal(submissionOne.InterestedParties.First().LastName, firstBatchResult.SearchResult.LastName);
            Assert.Contains(firstBatchResult.SearchResult.MatchingIswcs, iswc => iswc.OriginalTitle == submissionOne.OriginalTitle);
            Assert.Null(firstBatchResult.Rejection);

            var secondBatchResult = batchRes.Skip(1).First();
            Assert.NotNull(secondBatchResult.SearchResult);
            Assert.Equal(submissionTwo.InterestedParties.First().LastName, secondBatchResult.SearchResult.LastName);
            Assert.Contains(secondBatchResult.SearchResult.MatchingIswcs, iswc => iswc.OriginalTitle == submissionTwo.OriginalTitle);
            Assert.Null(secondBatchResult.Rejection); 
        }

        /// <summary>
        /// IP Context Batch Search for multiple IPs where one exists and the other is not found
        /// </summary>
        [Fact]
        public async void ContextSearchBatchTests_WithMultipleSubmissions_ReturnsMultipleResponse()
        {
            var searchModels = new List<IPContextSearchModel>
            {
                new IPContextSearchModel
                {
                    LastName = submissionOne.InterestedParties.First().LastName,
                    NameNumber = submissionOne.InterestedParties.First().NameNumber,
                    Works = new List<Works>
                    {
                        new Works
                        {
                            Titles = new List<Title>
                            {
                                new Title { Title1 = submissionOne.OriginalTitle, Type = TitleType.OT }
                            }
                        }
                    }
                },
                new IPContextSearchModel
                {
                    LastName = "Nonexistent",
                    Works = new List<Works>
                    {
                        new Works
                        {
                            Titles = new List<Title>
                            {
                                new Title { Title1 = submissionTwo.OriginalTitle, Type = TitleType.OT }
                            }
                        }
                    }
                }
            };

            var batchRes = await searchClient.ContextSearchBatchAsync(searchModels);

            Assert.NotNull(batchRes);
            Assert.Equal(2, batchRes.Count);

            var firstBatchResult = batchRes.First();
            Assert.NotNull(firstBatchResult.SearchResult);
            Assert.Equal(submissionOne.InterestedParties.First().LastName, firstBatchResult.SearchResult.LastName);
            Assert.Contains(firstBatchResult.SearchResult.MatchingIswcs, iswc => iswc.OriginalTitle == submissionOne.OriginalTitle);
            Assert.Null(firstBatchResult.Rejection);

            var secondBatchResult = batchRes.Skip(1).First();
            Assert.Null(secondBatchResult.SearchResult.MatchingIswcs);
            Assert.NotNull(secondBatchResult.Rejection);
            Assert.Equal("180", secondBatchResult.Rejection.Code);
        }
    }
}
