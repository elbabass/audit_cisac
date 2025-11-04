using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SpanishPoint.Azure.Iswc.Api.Agency.Managers;
using SpanishPoint.Azure.Iswc.Api.Agency.V1;
using SpanishPoint.Azure.Iswc.Api.Agency.V1.Controllers;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Framework.Http.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Tests.Controllers
{
	/// <summary>
	/// Tests allocation and resolution controller
	/// </summary>
	public class AllocationAndResolutionControllerTests
	{
		/// <summary>
		/// Tests Allocate submission batch multistatus response
		/// </summary>
		[Fact]
		public async Task AllocateSubmissionBatch()
		{
			var pipelineManagerMock = new Mock<IPipelineManager>();
			var mapperMock = new Mock<IMapper>().Object;
			var contextAcessorMock = new Mock<IHttpContextAccessor>().Object;

			pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
				.ReturnsAsync(
				new Bdo.Submissions.Submission()
				{
					Rejection = null,
					IswcModel = new IswcModel()
				});

			var test = new AllocationAndResolutionController(pipelineManagerMock.Object, mapperMock, contextAcessorMock);
			var response = await test.AddAllocationBatchAsync(new List<SubmissionBatch>() { new SubmissionBatch(), new SubmissionBatch() });

			Assert.IsType<MultiStatusObjectResult>(response.Result);
		}

		/// <summary>
		/// Tests Resolve submission batch multistatus response
		/// </summary>
		[Fact]
		public async Task ResolveSubmissionBatch()
		{
			var pipelineManagerMock = new Mock<IPipelineManager>();
			var mapperMock = new Mock<IMapper>().Object;
			var contextAcessorMock = new Mock<IHttpContextAccessor>().Object;

			pipelineManagerMock.Setup(p => p.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
				.ReturnsAsync(
				new Bdo.Submissions.Submission()
				{
					Rejection = new Bdo.Submissions.Rejection(Bdo.Rules.ErrorCode._100, "Error"),
				});

			var test = new AllocationAndResolutionController(pipelineManagerMock.Object, mapperMock, contextAcessorMock);
			var response = await test.AddResolutionBatchAsync(new List<SubmissionBatch>() { new SubmissionBatch(), new SubmissionBatch() });

			Assert.IsType<MultiStatusObjectResult>(response.Result);
		}
	}
}
