using Microsoft.AspNetCore.Mvc;
using Moq;
using SpanishPoint.Azure.Iswc.Api.Agency.Controllers;
using SpanishPoint.Azure.Iswc.Bdo.Audit;
using SpanishPoint.Azure.Iswc.Business.Managers;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Tests.Controllers
{
    /// <summary>
    /// Tests for AuditController
    /// </summary>
    public class AuditControllerTests
    {
        /// <summary>
        /// Test Audit search response where records are found
        /// </summary>
        [Fact]
        public async void GetInstrumentationCodes_Ok()
        {
            var auditManager = new Mock<IAuditManager>();

            auditManager.Setup(p => p.Search("T12345656"))
                .ReturnsAsync(
                    new List<AuditHistoryResult>
                    {
                     new AuditHistoryResult()
                        {
                            SubmittingAgency = "IMRO"
                        }
                    });

            var test = new AuditController(auditManager.Object);
            var response = await test.Search("T12345656");

            Assert.IsType<OkObjectResult>(response.Result);
        }

        /// <summary>
        /// Test Audit search response where no records found
        /// </summary>
        [Fact]
        public async void GetInstrumentationCodes_NotFound()
        {
            var auditManager = new Mock<IAuditManager>();

            auditManager.Setup(p => p.Search("T12345656"))
                .ReturnsAsync(new List<AuditHistoryResult>());

            var test = new AuditController(auditManager.Object);
            var response = await test.Search("T12345656");

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }
    }
}
