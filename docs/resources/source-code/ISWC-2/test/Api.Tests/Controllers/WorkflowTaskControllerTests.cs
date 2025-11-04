using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SpanishPoint.Azure.Iswc.Api.Agency.Managers;
using SpanishPoint.Azure.Iswc.Api.Agency.V1;
using SpanishPoint.Azure.Iswc.Api.Agency.V1.Controllers;
using SpanishPoint.Azure.Iswc.Framework.Http.Responses;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Tests.Controllers
{
    /// <summary>
    /// Tests WorkflowTasks Controller
    /// </summary>
    public class WorkflowTasksControllerTests
    {
        /// <summary>
        /// Test WorkflowTasksController to FindWorkflowTask - No Agency Found should return 404 and 'Agency not found' message
        /// </summary>
        [Fact]
        public async Task FindWorkflowTasks_AgencyNotFound()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>();
            var logger = new Mock<ILogger<WorkflowTasksController>>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                    new Bdo.Submissions.Submission()
                    {
                        Rejection = new Bdo.Submissions.Rejection(Bdo.Rules.ErrorCode._100, "Error")
                    });

            var test = new WorkflowTasksController(pipelineManagerMock.Object, mapperMock.Object, logger.Object);
            IEnumerable<WorkflowStatus> statuses = new List<WorkflowStatus>() { WorkflowStatus.Approved };
            var response = await test.FindWorkflowTasksAsync(
                "Agency Provided Won't Be Found", ShowWorkflows.AssignedToMe, WorkflowType.UpdateApproval, statuses, null, null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);


            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        /// <summary>
        /// Test WorkflowTasksController to FindWorkflowTask - Should Find WorkflowTasks and return with 200 response
        /// </summary>
        [Fact]
        public async Task FindWorkflowTasks_Ok()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var logger = new Mock<ILogger<WorkflowTasksController>>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                    new Bdo.Submissions.Submission()
                    {
                        Rejection = null
                    });

            var test = new WorkflowTasksController(pipelineManagerMock.Object, mapperMock, logger.Object);
            IEnumerable<WorkflowStatus> statuses = new List<WorkflowStatus>() { WorkflowStatus.Approved };
            var response = await test.FindWorkflowTasksAsync(
                "3", ShowWorkflows.AssignedToMe, WorkflowType.UpdateApproval, statuses, 0, 1, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        /// <summary>
        /// Test WorkflowTasksController to UpdateWorkflowTask - Agency not found
        /// </summary>
        [Fact]
        public async Task UpdateWorkflowTaskAsync_AgencyNotFound()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var logger = new Mock<ILogger<WorkflowTasksController>>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                    new Bdo.Submissions.Submission()
                    {
                        Rejection = new Bdo.Submissions.Rejection(Bdo.Rules.ErrorCode._100, "Error")
                    });

            IEnumerable<WorkflowTaskUpdate> body = new List<WorkflowTaskUpdate>();

            var test = new WorkflowTasksController(pipelineManagerMock.Object, mapperMock, logger.Object);
            var response = await test.UpdateWorkflowTaskAsync("Agency Provided Won't Be Found", body);

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        /// <summary>
        /// Test WorkflowTasksController to UpdateWorkflowTask
        /// </summary>
        [Fact]
        public async Task UpdateWorkflowTaskAsync()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>().Object;
            var logger = new Mock<ILogger<WorkflowTasksController>>();

            pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                    new Bdo.Submissions.Submission()
                    {
                        Rejection = null
                    });

            IEnumerable<WorkflowTaskUpdate> body = new List<WorkflowTaskUpdate>();

            var test = new WorkflowTasksController(pipelineManagerMock.Object, mapperMock, logger.Object);
            var response = await test.UpdateWorkflowTaskAsync("Agency Provided Won't Be Found", body);

            Assert.IsType<MultiStatusObjectResult>(response.Result);
        }
    }
}
