using Azure.Search.Documents;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.AllocationServiceTests
{

    public class AllocateNewWork_WorkCodes_Fixture : IAsyncLifetime
    {
        public IISWC_Allocation_and_ResolutionClient client;
        public HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            client = new ISWC_Allocation_and_ResolutionClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }


    public class AllocateNewWork_WorkCodes : TestBase, IClassFixture<AllocateNewWork_WorkCodes_Fixture>
    {
        private IISWC_Allocation_and_ResolutionClient client;
        private HttpClient httpClient;

        public AllocateNewWork_WorkCodes(AllocateNewWork_WorkCodes_Fixture fixture)
        {
            client = fixture.client;
            httpClient = fixture.httpClient;
        }

        /// <summary>
        /// Scenario: Allocate new work when work code and additional identifier information are included. 
        /// Expected: New work is created successfully.
        /// </summary>
        [Fact]
        public async void AllocateNewWork_WithWorkCodeAndAdditionalIdentifierInformation_ReturnsSuccess()
        {
            var submission = Submissions.EligibleSubmissionPRS;
            submission.AdditionalIdentifiers = AdditionalIdentifersData.AI_SONY;
            List<SubmissionBatch> batch = new List<SubmissionBatch>{
                new SubmissionBatch {
                    SubmissionId = 1,
                    Submission =  submission
                }
            };

            var result = await client.AddAllocationBatchAsync(batch);

            Assert.Collection(result,
                elem1 => Assert.NotNull(elem1.Submission.VerifiedSubmission));
        }

        /// <summary>
        /// Scenario: Allocate new work when work code is included but additional identifier information is not. 
        /// Expected: New work is created successfully.
        /// </summary>
        [Fact]
        public async void AllocateNewWork_WithWorkCodeWithoutAdditionalIdentifierInformation_ReturnsSuccess()
        {
            var submission = Submissions.EligibleSubmissionPRS;
            List<SubmissionBatch> batch = new List<SubmissionBatch>{
                new SubmissionBatch {
                    SubmissionId = 1,
                    Submission =  submission
                }
            };

            var result = await client.AddAllocationBatchAsync(batch);

            Assert.Collection(result,
                elem1 => Assert.NotNull(elem1.Submission.VerifiedSubmission));
        }

        /// <summary>
        /// Scenario: Allocate new work when additional identifier information is included but work code is not. 
        /// Expected: New work is created successfully.
        /// </summary>
        [Fact]
        public async void AllocateNewWork_WithoutWorkCodeWithAdditionalIdentifierInformation_ReturnsSuccess()
        {
            var submission = Submissions.EligibleSubmissionPRS;
            submission.Workcode = string.Empty;
            submission.AdditionalIdentifiers = AdditionalIdentifersData.AI_SONY;
            List<SubmissionBatch> batch = new List<SubmissionBatch>{
                new SubmissionBatch {
                    SubmissionId = 1,
                    Submission =  submission
                }
            };

            var result = await client.AddAllocationBatchAsync(batch);

            Assert.Collection(result,
                elem1 => Assert.NotNull(elem1.Submission.VerifiedSubmission));
        }

        /// <summary>
        /// Scenario: Allocate new work when work code and additional identifier information are not included. 
        /// Expected: New work is created successfully.
        /// </summary>
        [Fact]
        public async void AllocateNewWork_WithoutWorkCodeAndAdditionalIdentifierInformation_ReturnsSuccess()
        {
            var submission = Submissions.EligibleSubmissionPRS;
            submission.Workcode = string.Empty;
            List<SubmissionBatch> batch = new List<SubmissionBatch>{
                new SubmissionBatch {
                    SubmissionId = 1,
                    Submission =  submission
                }
            };

            var result = await client.AddAllocationBatchAsync(batch);

            Assert.Collection(result,
                elem1 => Assert.NotNull(elem1.Submission.VerifiedSubmission));
        }
    }
}
