using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SpanishPoint.Azure.Iswc.Api.Agency.Managers;
using SpanishPoint.Azure.Iswc.Api.Agency.V1;
using SpanishPoint.Azure.Iswc.Api.Agency.V1.Controllers;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Framework.Http.Responses;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Tests.Controllers
{
    /// <summary>
    /// Tests submission controller
    /// </summary>
    public class SubmissionControllerTests
    {
        /// <summary>
        /// Tests Add submission ok response
        /// </summary>
        [Fact]
        public async void AddSubmission_Ok()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var contextAcessorMock = new Mock<IHttpContextAccessor>();
            var logger = new Mock<ILogger<SubmissionController>>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                    new Bdo.Submissions.Submission()
                    {
                        Rejection = null
                    });

            var test = new SubmissionController(pipelineManagerMock.Object, mapperMock, contextAcessorMock.Object, logger.Object);
            var response = await test.AddSubmissionAsync(new Submission());

            Assert.IsType<CreatedResult>(response.Result);
        }

        /// <summary>
        /// Tests Add submission Bad Request response
        /// </summary>
        [Fact]
        public async void AddSubmission_BadRequest()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var contextAcessorMock = new Mock<IHttpContextAccessor>();
            var logger = new Mock<ILogger<SubmissionController>>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                    new Bdo.Submissions.Submission()
                    {
                        Rejection = new Bdo.Submissions.Rejection(Bdo.Rules.ErrorCode._100, "Error")
                    });

            var test = new SubmissionController(pipelineManagerMock.Object, mapperMock, contextAcessorMock.Object, logger.Object);
            var response = await test.AddSubmissionAsync(new Submission());

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        /// <summary>
        /// Tests Add Submission Batch Multistatus response
        /// </summary>
        [Fact]
        public async void AddSubmissionBatch()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var contextAcessorMock = new Mock<IHttpContextAccessor>();
            var logger = new Mock<ILogger<SubmissionController>>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                new Bdo.Submissions.Submission()
                {
                    Rejection = null,
                    IswcModel = new IswcModel()
                });

            var test = new SubmissionController(pipelineManagerMock.Object, mapperMock, contextAcessorMock.Object, logger.Object);
            var response = await test.AddSubmissionBatchAsync(new List<SubmissionBatch>() { new SubmissionBatch(), new SubmissionBatch() });

            Assert.IsType<MultiStatusObjectResult>(response.Result);
        }

        /// <summary>
        /// Tests Update submission Ok response
        /// </summary>
        [Fact]
        public async void UpdateSubmission_Ok()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var contextAcessorMock = new Mock<IHttpContextAccessor>();
            var logger = new Mock<ILogger<SubmissionController>>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                    new Bdo.Submissions.Submission()
                    {
                        Rejection = null
                    });

            var test = new SubmissionController(pipelineManagerMock.Object, mapperMock, contextAcessorMock.Object, logger.Object);
            var response = await test.UpdateSubmissionAsync("12345", new Submission());

            Assert.IsType<OkObjectResult>(response.Result);
        }

        /// <summary>
        /// Tests Update submission Ok response with no preferred iswc
        /// </summary>
        [Fact]
        public async void UpdateSubmission_No_Preferred_ISWC_Ok()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var contextAcessorMock = new Mock<IHttpContextAccessor>();
            var logger = new Mock<ILogger<SubmissionController>>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                    new Bdo.Submissions.Submission()
                    {
                        Rejection = null
                    });

            var test = new SubmissionController(pipelineManagerMock.Object, mapperMock, contextAcessorMock.Object, logger.Object);
            var response = await test.UpdateSubmissionAsync(null, new Submission());

            Assert.IsType<OkObjectResult>(response.Result);
        }

        /// <summary>
        /// Tests Update submission bad request response
        /// </summary>
        [Fact]
        public async void UpdateSubmission_BadRequest()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var contextAcessorMock = new Mock<IHttpContextAccessor>();
            var logger = new Mock<ILogger<SubmissionController>>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                new Bdo.Submissions.Submission()
                {
                    Rejection = new Bdo.Submissions.Rejection(Bdo.Rules.ErrorCode._100, "Error"),
                });

            var test = new SubmissionController(pipelineManagerMock.Object, mapperMock, contextAcessorMock.Object, logger.Object);
            var response = await test.UpdateSubmissionAsync("12345", new Submission());

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        /// <summary>
        /// Tests Update Submission Batch Multistatus response
        /// </summary>
        [Fact]
        public async void UpdateSubmissionBatch()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var contextAcessorMock = new Mock<IHttpContextAccessor>();
            var logger = new Mock<ILogger<SubmissionController>>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                new Bdo.Submissions.Submission()
                {
                    Rejection = null,
                    IswcModel = new IswcModel(),
                    DetailLevel = Bdo.Submissions.DetailLevel.Full
                });

            var test = new SubmissionController(pipelineManagerMock.Object, mapperMock, contextAcessorMock.Object, logger.Object);
            var response = await test.UpdateSubmissionBatchAsync(new List<SubmissionBatch>() { new SubmissionBatch(), new SubmissionBatch() });

            Assert.IsType<MultiStatusObjectResult>(response.Result);
        }


        /// <summary>
        /// Tests Delete Submission No Content response
        /// </summary>
        [Fact]
        public async void DeleteSubmission_NoContent()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var contextAcessorMock = new Mock<IHttpContextAccessor>();
            var logger = new Mock<ILogger<SubmissionController>>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                new Bdo.Submissions.Submission()
                {
                    Rejection = null,
                    IswcModel = new IswcModel()
                });

            var test = new SubmissionController(pipelineManagerMock.Object, mapperMock, contextAcessorMock.Object, logger.Object);
            var response = await test.DeleteSubmissionAsync("12345", "7", "54321", 100, "Test");

            Assert.IsType<NoContentResult>(response);
        }

        /// <summary>
        /// Tests Delete Submission Not Found response
        /// </summary>
        [Fact]
        public async void DeleteSubmission_NotFound()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var contextAcessorMock = new Mock<IHttpContextAccessor>();
            var logger = new Mock<ILogger<SubmissionController>>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                new Bdo.Submissions.Submission()
                {
                    Rejection = null,
                    IswcModel = null
                });

            var test = new SubmissionController(pipelineManagerMock.Object, mapperMock, contextAcessorMock.Object, logger.Object);
            var response = await test.DeleteSubmissionAsync("12345", "7", "54321", 100, "Test");

            Assert.IsType<NotFoundObjectResult>(response);
        }

        /// <summary>
        /// Tests Delete Submission Bad Request response
        /// </summary>
        [Fact]
        public async void DeleteSubmission_BadRequest()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var contextAcessorMock = new Mock<IHttpContextAccessor>();
            var logger = new Mock<ILogger<SubmissionController>>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                new Bdo.Submissions.Submission()
                {
                    Rejection = new Bdo.Submissions.Rejection(Bdo.Rules.ErrorCode._100, "Error"),
                    IswcModel = null
                });

            var test = new SubmissionController(pipelineManagerMock.Object, mapperMock, contextAcessorMock.Object, logger.Object);
            var response = await test.DeleteSubmissionAsync("12345", "7", "54321", 100, "Test");

            Assert.IsType<BadRequestObjectResult>(response);
        }
    }
}
