using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SpanishPoint.Azure.Iswc.Api.Agency.Managers;
using SpanishPoint.Azure.Iswc.Api.Agency.V1;
using SpanishPoint.Azure.Iswc.Api.Agency.V1.Controllers;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Tests.Controllers
{
    /// <summary>
    /// Tests for Merge Controller
    /// </summary>
    public class MergeControllerTests
    {
        /// <summary>
        /// Tests Merge submission No Content response
        /// </summary>
        [Fact]
        public async Task MergeSubmission_NoContent()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var contextAcessorMock = new Mock<IHttpContextAccessor>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                new Bdo.Submissions.Submission()
                {
                    Rejection = null,
                    IswcModel = new IswcModel()
                });

            var test = new MergeController(pipelineManagerMock.Object, mapperMock, contextAcessorMock.Object);
            var response = await test.MergeISWCMetadataAsync("12345", "1",
                new Body
                {
                    Iswcs = new List<string> { },
                    AgencyWorks = new List<WorkNumber> { }
                });

            Assert.IsType<NoContentResult>(response);
        }

        /// <summary>
        /// Tests Merge submission Not Found response
        /// </summary>
        [Fact]
        public async Task MergeSubmission_NotFound()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var contextAcessorMock = new Mock<IHttpContextAccessor>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                new Bdo.Submissions.Submission()
                {
                    Rejection = null,
                    IswcModel = null
                });

            var test = new MergeController(pipelineManagerMock.Object, mapperMock, contextAcessorMock.Object);
            var response = await test.MergeISWCMetadataAsync("12345", "1",
                new Body
                {
                    Iswcs = new List<string> { },
                    AgencyWorks = new List<WorkNumber> { }
                });

            Assert.IsType<NotFoundObjectResult>(response);
        }

        /// <summary>
        /// Tests Merge submission Bad Request response
        /// </summary>
        [Fact]
        public async Task MergeSubmission_BadRequest()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var contextAcessorMock = new Mock<IHttpContextAccessor>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                new Bdo.Submissions.Submission()
                {
                    Rejection = new Bdo.Submissions.Rejection(Bdo.Rules.ErrorCode._100, "Error"),
                    IswcModel = null
                });

            var test = new MergeController(pipelineManagerMock.Object, mapperMock, contextAcessorMock.Object);
            var response = await test.MergeISWCMetadataAsync("12345", "1",
                new Body
                {
                    Iswcs = new List<string> { },
                    AgencyWorks = new List<WorkNumber> { }
                });

            Assert.IsType<BadRequestObjectResult>(response);
        }

        /// <summary>
        /// Tests Demerge submission No Content response
        /// </summary>
        [Fact]
        public async Task DemergeSubmission_NoContent()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var contextAcessorMock = new Mock<IHttpContextAccessor>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                new Bdo.Submissions.Submission()
                {
                    Rejection = null,
                    IswcModel = new IswcModel()
                });

            var test = new MergeController(pipelineManagerMock.Object, mapperMock, contextAcessorMock.Object);
            var response = await test.DemergeISWCMetadataAsync("9876", "11", "9846");

            Assert.IsType<NoContentResult>(response);
        }

        /// <summary>
        /// Tests Demerge submission Not Found response
        /// </summary>
        [Fact]
        public async Task DemergeSubmission_NotFound()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var contextAcessorMock = new Mock<IHttpContextAccessor>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                new Bdo.Submissions.Submission()
                {
                    Rejection = null,
                    IswcModel = null
                });

            var test = new MergeController(pipelineManagerMock.Object, mapperMock, contextAcessorMock.Object);
            var response = await test.DemergeISWCMetadataAsync("9876", "11", "9846");

            Assert.IsType<NotFoundObjectResult>(response);
        }

        /// <summary>
        /// Tests Demerge submission Bad Request response
        /// </summary>
        [Fact]
        public async Task DemergeSubmission_BadRequest()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var contextAcessorMock = new Mock<IHttpContextAccessor>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                new Bdo.Submissions.Submission()
                {
                    Rejection = new Bdo.Submissions.Rejection(Bdo.Rules.ErrorCode._100, "Error"),
                    IswcModel = null
                });

            var test = new MergeController(pipelineManagerMock.Object, mapperMock, contextAcessorMock.Object);
            var response = await test.DemergeISWCMetadataAsync("9876", "11", "9846");

            Assert.IsType<BadRequestObjectResult>(response);
        }

    }
}
