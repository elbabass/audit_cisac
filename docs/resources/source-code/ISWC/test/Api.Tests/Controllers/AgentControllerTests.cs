using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SpanishPoint.Azure.Iswc.Api.Agency.V1;
using SpanishPoint.Azure.Iswc.Api.Agency.V1.Controllers;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Data.Services.Notifications;
using System;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Tests.Controllers
{
    /// <summary>
    /// Tests for Agent Controller
    /// </summary>
    public class AgentControllerTests
    {
        /// <summary>
        /// Tests UpdateAgentRun Ok response
        /// </summary>
        [Fact]
        public async void UpdateAgentRun_Ok()
        {
            var agentManagerMock = new Mock<IAgentManager>();
            var mapperMock = new Mock<IMapper>();

            agentManagerMock.Setup(x => x.UpdateAgentRun(It.IsAny<Bdo.Agent.AgentRun>()))
                .ReturnsAsync("run id");
            mapperMock.Setup(x => x.Map<Bdo.Agent.AgentRun>(It.IsAny<AgentRun>()))
                .Returns(It.IsAny<Bdo.Agent.AgentRun>());

            var test = new AgentController(agentManagerMock.Object, mapperMock.Object);
            var response = await test.UpdateAgentRunAsync(new AgentRun());

            Assert.IsType<OkObjectResult>(response.Result);
        }

        /// <summary>
        /// Tests UpdateAgentRun Not Found response
        /// </summary>
        [Fact]
        public async void UpdateAgentRun_NotFound()
        {
            var agentManagerMock = new Mock<IAgentManager>();
            var mapperMock = new Mock<IMapper>();

            agentManagerMock.Setup(x => x.UpdateAgentRun(It.IsAny<Bdo.Agent.AgentRun>()))
                .ReturnsAsync(default(string));
            mapperMock.Setup(x => x.Map<Bdo.Agent.AgentRun>(It.IsAny<AgentRun>()))
                .Returns(It.IsAny<Bdo.Agent.AgentRun>);

            var test = new AgentController(agentManagerMock.Object, mapperMock.Object);
            var response = await test.UpdateAgentRunAsync(new AgentRun() { RunId = "run id" });

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        /// <summary>
        /// Tests Get Notifications Ok response
        /// </summary>
        [Fact]
        public async void UpdateNotifications_Ok()
        {
            var agentManagerMock = new Mock<IAgentManager>();
            var mapperMock = new Mock<IMapper>();
            var notifications = new List<Bdo.Agent.CsnNotification>() { new Bdo.Agent.CsnNotification { } };
            var agentNotification = new Bdo.Agent.AgentNotification() { CsnNotifications = notifications, ContinuationToken = string.Empty };


            agentManagerMock.Setup(x => x.GetNotification("128", Bdo.Edi.TransactionType.MER, default, 1, null))
                .ReturnsAsync(agentNotification);

            var test = new AgentController(agentManagerMock.Object, mapperMock.Object);
            var response = await test.GetNotificationsAsync("128", CsnTransactionType.MER, null, 1, null);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        /// <summary>
        /// Tests Get Notifications Bad Request
        /// </summary>
        [Fact]
        public async void UpdateNotifications_BadRequest()
        {
            var agentManagerMock = new Mock<IAgentManager>();
            var mapperMock = new Mock<IMapper>();
            var notification = new Bdo.Agent.AgentNotification() { Rejection = new Bdo.Submissions.Rejection(Bdo.Rules.ErrorCode._175, "") };

            agentManagerMock.Setup(x => x.GetNotification("128", Bdo.Edi.TransactionType.MER, default, 1, null))
                .ReturnsAsync(notification);

            var test = new AgentController(agentManagerMock.Object, mapperMock.Object);
            var response = await test.GetNotificationsAsync("128", CsnTransactionType.MER, null, 1, null);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

    }
}
