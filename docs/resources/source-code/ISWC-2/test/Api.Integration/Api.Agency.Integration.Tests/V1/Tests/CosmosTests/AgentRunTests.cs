using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.Cosmos;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.CosmosTests
{
    public class AgentRunTests_Fixture : IAsyncLifetime
    {
        public ISWC_CosmosClient cosmosClient;
        public HttpClient httpClient;
        public IISWC_AgentClient agentClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            cosmosClient = new ISWC_CosmosClient();
            agentClient = new ISWC_AgentClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class AgentRunTests : TestBase, IClassFixture<AgentRunTests_Fixture>
    {
        private ISWC_CosmosClient cosmosClient;
        private HttpClient httpClient;
        private IISWC_AgentClient agentClient;

        public AgentRunTests(AgentRunTests_Fixture fixture)
        {
            cosmosClient = fixture.cosmosClient;
            httpClient = fixture.httpClient;
            agentClient = fixture.agentClient;
        }

        [Fact]
        public async Task AgentRunTests_01()
        {
            var run = new AgentRun
            {
                AgentVersion = "1.0.0.0",
                AgencyCode = "128",
                UpdateRecordCount = 1,
                NewRecordCount = 1,
                OverallRejected = 0,
                OverallSent = 0,
                SuccessfulCount = 0,
                BusinessRejectionCount = 0,
                TechnicalRejectionCount = 0
            };
            var runID = (await agentClient.UpdateAgentRunAsync(run)).RunId;

            run.RunId = runID;
            run.OverallRejected = 0;
            run.OverallSent = 2;
            run.SuccessfulCount = 2;
            await agentClient.UpdateAgentRunAsync(run);

            var record = await cosmosClient.GetAgentRunRecord(runID);

            Assert.True(record.RunCompleted);
            Assert.NotNull(record.RunEndDate);
            Assert.Equal(run.AgentVersion, record.AgentVersion);
            Assert.Equal(run.AgencyCode, record.AgencyCode);
            Assert.Equal(run.UpdateRecordCount, record.UpdateRecordCount);
            Assert.Equal(run.NewRecordCount, record.NewRecordCount);
            Assert.Equal(run.OverallRejected, record.OverallRejected);
            Assert.Equal(run.OverallSent, record.OverallSent);
            Assert.Equal(run.BusinessRejectionCount, record.BusinessRejectionCount);
            Assert.Equal(run.TechnicalRejectionCount, record.TechnicalRejectionCount);
            Assert.Equal(runID, record.RunId);
        }
    }
}
