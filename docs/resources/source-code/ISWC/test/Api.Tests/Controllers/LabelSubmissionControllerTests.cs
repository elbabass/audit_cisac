using AutoMapper;
using Moq;
using SpanishPoint.Azure.Iswc.Api.Label.V1;
using SpanishPoint.Azure.Iswc.Api.Label.V1.Controllers;
using SpanishPoint.Azure.Iswc.Api.Label.Managers;
using System.Collections.Generic;
using Xunit;
using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Framework.Http.Responses;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using Submission = SpanishPoint.Azure.Iswc.Api.Label.V1.Submission;

namespace SpanishPoint.Azure.Iswc.Api.Tests.Controllers
{
    /// <summary>
    /// Tests for Label Submission Controller
    /// </summary>
    public class LabelSubmissionControllerTests
    {
        /// <summary>
        /// Tests AddLabelSubmissionBatchAsync returns multi-status response
        /// </summary>
        [Fact]
        public async void AddLabelSubmissionBatchAsync_MultiStatusResponse()
        {
            // arrange
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>();
            var contextAccessorMock = new Mock<IHttpContextAccessor>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                    new Bdo.Submissions.Submission()
                    {
                        Rejection = null
                    }
                );

            mapperMock.Setup(x => x.Map<SubmissionModel>(It.IsAny<Submission>()))
                .Returns(It.IsAny<SubmissionModel>());

            var test = new LabelSubmissionController(pipelineManagerMock.Object, mapperMock.Object, contextAccessorMock.Object);

            // act
            var response = await test.AddLabelSubmissionBatchAsync(new List<SubmissionBatch>());

            //assert
            Assert.IsType<MultiStatusObjectResult>(response.Result);
        }
    }
}
