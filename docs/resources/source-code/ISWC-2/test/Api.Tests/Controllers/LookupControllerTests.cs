using Microsoft.AspNetCore.Mvc;
using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Api.Agency.Controllers;
using SpanishPoint.Azure.Iswc.Business.Managers;
using System.Collections.Generic;
using Xunit;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using System;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Api.Tests.Controllers
{
    /// <summary>
    /// Tests for LookupController
    /// </summary>
    public class LookupControllerTests
    {
        /// <summary>
        /// Test LookupController to get lookup data
        /// </summary>
        [Fact]
        public async Task GetLookupData_Ok()
        {
            var lookupManager = new Mock<ILookupManager>();
            var messagingManager = new Mock<IMessagingManager>();

            lookupManager.Setup(p => p.GetLookups())
                .ReturnsAsync(Array.Empty<LookupData>());

            var test = new LookupController(lookupManager.Object, messagingManager.Object);
            var response = await test.GetLookupData();

            Assert.IsType<OkObjectResult>(response.Result);
        }

        /// <summary>
        /// Test LookupController where IP found
        /// </summary>
        [Fact]
        public async Task GetIps_Found()
        {
            var lookupManager = new Mock<ILookupManager>();
            var messagingManager = new Mock<IMessagingManager>();

            var ips = new InterestedPartySearchModel
            {
                Name = "Lennon",
                ContributorType = ContributorType.Creator
            };

            lookupManager.Setup(x => x.GetIps(ips))
                .ReturnsAsync(new List<InterestedPartyModel> { new InterestedPartyModel() });

            var test = new LookupController(lookupManager.Object, messagingManager.Object);
            var response = await test.GetIps(ips);

            Assert.IsType<OkObjectResult>(response.Result);
        }
    }
}
