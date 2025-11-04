using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SpanishPoint.Azure.Iswc.Api.ThirdParty.Managers;
using SpanishPoint.Azure.Iswc.Api.ThirdParty.V1.Controllers;
using SpanishPoint.Azure.Iswc.Api.ThirdParty.V1;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SpanishPoint.Azure.Iswc.Framework.Http.Responses;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Api.Tests.Controllers
{
    /// <summary>
    /// Tests for ThirdPartyController
    /// </summary>
    public class ThirdPartyControllerTests
    {
        /// <summary>
		/// Tests Search by Iswc Batch Multistatus response
		/// </summary>
		[Fact]
        public async Task SearchByISWCBatch()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>();
            var contextAcessorMock = new Mock<IHttpContextAccessor>();

            pipelineManagerMock.Setup(x => x.RunPipelines(It.IsAny<IEnumerable<Bdo.Submissions.Submission>>()))
                .ReturnsAsync(
                    new List<Bdo.Submissions.Submission>()
                    { 
                        new Bdo.Submissions.Submission()
                        {
                            Rejection = null,
                            SearchedIswcModels = new List<IswcModel>() { new IswcModel() }
                        }
                        
                    }.AsEnumerable());

            mapperMock.Setup(x => x.Map<ISWCMetadata>(It.IsAny<IswcModel>()))
                .Returns(It.IsAny<ISWCMetadata>());

            var test = new ThirdPartyController(pipelineManagerMock.Object, mapperMock.Object, contextAcessorMock.Object);
            var response = await test.ThirdPartySearchByISWCBatchAsync(new List<IswcSearchModel>());

            Assert.IsType<MultiStatusObjectResult>(response.Result);
        }

        /// <summary>
		/// Tests Search by Agency Work code Multistatus response
		/// </summary>
		[Fact]
        public async Task SearchByAgencyWorkCodeBatch()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>();
            var contextAcessorMock = new Mock<IHttpContextAccessor>();

            pipelineManagerMock.Setup(x => x.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                    new Bdo.Submissions.Submission()
                    {
                        Rejection = null,
                        SearchedIswcModels = new List<IswcModel>() { new IswcModel() }
                    });

            mapperMock.Setup(x => x.Map<ISWCMetadata>(It.IsAny<IswcModel>()))
                .Returns(It.IsAny<ISWCMetadata>());

            var test = new ThirdPartyController(pipelineManagerMock.Object, mapperMock.Object, contextAcessorMock.Object);
            var response = await test.ThirdPartySearchByAgencyWorkCodeBatchAsync(new List<AgencyWorkCodeSearchModel>());

            Assert.IsType<MultiStatusObjectResult>(response.Result);
        }

        /// <summary>
		/// Tests Search by Title and Contributor Multistatus response
		/// </summary>
		[Fact]
        public async Task SearchByTitleAndContributorBatch()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>();
            var contextAcessorMock = new Mock<IHttpContextAccessor>();

            pipelineManagerMock.Setup(x => x.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                    new Bdo.Submissions.Submission()
                    {
                        Rejection = null,
                        SearchedIswcModels = new List<IswcModel>() { new IswcModel() }
                    });

            mapperMock.Setup(x => x.Map<ISWCMetadata>(It.IsAny<IswcModel>()))
                .Returns(It.IsAny<ISWCMetadata>());

            var test = new ThirdPartyController(pipelineManagerMock.Object, mapperMock.Object, contextAcessorMock.Object);
            var response = await test.ThirdPartySearchByTitleAndContributorBatchAsync(new List<TitleAndContributorSearchModel>());

            Assert.IsType<MultiStatusObjectResult>(response.Result);
        }
    }
}
