using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SpanishPoint.Azure.Iswc.Api.Agency.Managers;
using SpanishPoint.Azure.Iswc.Api.Agency.V1;
using SpanishPoint.Azure.Iswc.Api.Agency.V1.Controllers;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Framework.Http.Responses;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Tests.Controllers
{
    /// <summary>
    /// Tests for SearchController
    /// </summary>
    public class SearchControllerTests
    {
        /// <summary>
        /// Tests SearchByAgencyWorkCode Ok response
        /// </summary>
        [Fact]
        public async void SearchByAgencyWorkCodeOk()
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

            var test = new SearchController(pipelineManagerMock.Object, mapperMock.Object, contextAcessorMock.Object);
            var response = await test.SearchByAgencyWorkCodeAsync("agency", "workcode", DetailLevel.Full);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        /// <summary>
        /// Tests SearchByAgencyWorkCode Not Found response
        /// </summary>
        [Fact]
        public async void SearchByAgencyWorkCodeNotFound()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>();
            var contextAcessorMock = new Mock<IHttpContextAccessor>();

            pipelineManagerMock.Setup(x => x.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                    new Bdo.Submissions.Submission()
                    {
                        Rejection = new Bdo.Submissions.Rejection(Bdo.Rules.ErrorCode._180, "Error"),
                        IswcModel = null
                    });

            mapperMock.Setup(x => x.Map<ISWCMetadata>(It.IsAny<IswcModel>()))
                .Returns(It.IsAny<ISWCMetadata>());

            var test = new SearchController(pipelineManagerMock.Object, mapperMock.Object, contextAcessorMock.Object);
            var response = await test.SearchByAgencyWorkCodeAsync("agency", "workcode", DetailLevel.Full);

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        /// <summary>
        /// Tests SearchByAgencyWorkCode Bad Request response
        /// </summary>
        [Fact]
        public async void SearchByAgencyWorkCodeBadRequestd()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>();
            var contextAcessorMock = new Mock<IHttpContextAccessor>();

            pipelineManagerMock.Setup(x => x.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                    new Bdo.Submissions.Submission()
                    {
                        Rejection = new Bdo.Submissions.Rejection(Bdo.Rules.ErrorCode._100, "Error")
                    });

            mapperMock.Setup(x => x.Map<ISWCMetadata>(It.IsAny<IswcModel>()))
                .Returns(It.IsAny<ISWCMetadata>());

            var test = new SearchController(pipelineManagerMock.Object, mapperMock.Object, contextAcessorMock.Object);
            var response = await test.SearchByAgencyWorkCodeAsync("agency", "workcode", DetailLevel.Full);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        /// <summary>
        /// Tests SearchByIswc Ok response
        /// </summary>
        [Fact]
        public async void SearchByIswcOk()
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

            var test = new SearchController(pipelineManagerMock.Object, mapperMock.Object, contextAcessorMock.Object);
            var response = await test.SearchByISWCAsync("iswc");

            Assert.IsType<OkObjectResult>(response.Result);
        }

        /// <summary>
        /// Tests SearchByIswc Not Found response
        /// </summary>
        [Fact]
        public async void SearchByIswcNotFound()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>();
            var contextAcessorMock = new Mock<IHttpContextAccessor>();

            pipelineManagerMock.Setup(x => x.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                    new Bdo.Submissions.Submission()
                    {
                        Rejection = new Bdo.Submissions.Rejection(Bdo.Rules.ErrorCode._180, "Error"),
                        SearchedIswcModels = null
                    });

            mapperMock.Setup(x => x.Map<ISWCMetadata>(It.IsAny<IswcModel>()))
                .Returns(It.IsAny<ISWCMetadata>());

            var test = new SearchController(pipelineManagerMock.Object, mapperMock.Object, contextAcessorMock.Object);
            var response = await test.SearchByISWCAsync("iswc");

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        /// <summary>
        /// Tests SearchByIswc Bad Request response
        /// </summary>
        [Fact]
        public async void SearchByIswcBadRequestd()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>();
            var contextAcessorMock = new Mock<IHttpContextAccessor>();

            pipelineManagerMock.Setup(x => x.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                    new Bdo.Submissions.Submission()
                    {
                        Rejection = new Bdo.Submissions.Rejection(Bdo.Rules.ErrorCode._100, "Error")
                    });

            mapperMock.Setup(x => x.Map<ISWCMetadata>(It.IsAny<IswcModel>()))
                .Returns(It.IsAny<ISWCMetadata>());

            var test = new SearchController(pipelineManagerMock.Object, mapperMock.Object, contextAcessorMock.Object);
            var response = await test.SearchByISWCAsync("iswc");

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        /// <summary>
        /// Tests SearchByTitleAndContributor Ok response
        /// </summary>
        [Fact]
        public async void SearchByTitleAndContributorOk()
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

            mapperMock.Setup(x => x.Map<ICollection<InterestedPartyModel>>(It.IsAny<List<InterestedParty>>()))
                 .Returns(It.IsAny<ICollection<InterestedPartyModel>>());
            mapperMock.Setup(x => x.Map<ICollection<Bdo.Work.Title>>(It.IsAny<List<Title>>()))
                .Returns(It.IsAny<ICollection<Bdo.Work.Title>>());
            mapperMock.Setup(x => x.Map<ISWCMetadata>(It.IsAny<List<IswcModel>>()))
                .Returns(It.IsAny<ISWCMetadata>());

            var test = new SearchController(pipelineManagerMock.Object, mapperMock.Object, contextAcessorMock.Object);
            var response = await test.SearchByTitleAndContributorAsync(new TitleAndContributorSearchModel());

            Assert.IsType<OkObjectResult>(response.Result);
        }

        /// <summary>
        /// Tests SearchByTitleAndContributor Not Found response
        /// </summary>
        [Fact]
        public async void SearchByTitleAndContributorNotFound()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>();
            var contextAcessorMock = new Mock<IHttpContextAccessor>();

            pipelineManagerMock.Setup(x => x.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                    new Bdo.Submissions.Submission()
                    {
                        Rejection = new Bdo.Submissions.Rejection(Bdo.Rules.ErrorCode._180, "Error"),
                        SearchedIswcModels = null
                    });

            mapperMock.Setup(x => x.Map<ICollection<InterestedPartyModel>>(It.IsAny<List<InterestedParty>>()))
                 .Returns(It.IsAny<ICollection<InterestedPartyModel>>());
            mapperMock.Setup(x => x.Map<ICollection<Bdo.Work.Title>>(It.IsAny<List<Title>>()))
                .Returns(It.IsAny<ICollection<Bdo.Work.Title>>());
            mapperMock.Setup(x => x.Map<ISWCMetadata>(It.IsAny<IswcModel>()))
                .Returns(It.IsAny<ISWCMetadata>());


            var test = new SearchController(pipelineManagerMock.Object, mapperMock.Object, contextAcessorMock.Object);
            var response = await test.SearchByTitleAndContributorAsync(new TitleAndContributorSearchModel());

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        /// <summary>
        /// Tests SearchByTitleAndContributor Bad Request response
        /// </summary>
        [Fact]
        public async void SearchByTitleAndContributorBadRequestd()
        {
            var pipelineManagerMock = new Mock<IPipelineManager>();
            var mapperMock = new Mock<IMapper>();
            var contextAcessorMock = new Mock<IHttpContextAccessor>();

            pipelineManagerMock.Setup(x => x.RunPipelines(It.IsAny<Bdo.Submissions.Submission>()))
                .ReturnsAsync(
                    new Bdo.Submissions.Submission()
                    {
                        Rejection = new Bdo.Submissions.Rejection(Bdo.Rules.ErrorCode._100, "Error")
                    });

            mapperMock.Setup(x => x.Map<ICollection<InterestedPartyModel>>(It.IsAny<List<InterestedParty>>()))
                .Returns(It.IsAny<ICollection<InterestedPartyModel>>());
            mapperMock.Setup(x => x.Map<ICollection<Bdo.Work.Title>>(It.IsAny<List<Title>>()))
                .Returns(It.IsAny<ICollection<Bdo.Work.Title>>());
            mapperMock.Setup(x => x.Map<ISWCMetadata>(It.IsAny<IswcModel>()))
                .Returns(It.IsAny<ISWCMetadata>());

            var test = new SearchController(pipelineManagerMock.Object, mapperMock.Object, contextAcessorMock.Object);
            var response = await test.SearchByTitleAndContributorAsync(new TitleAndContributorSearchModel());

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

		/// <summary>
		/// Tests Search by Agency Work code Multistatus response
		/// </summary>
		[Fact]
		public async void SearchByAgencyWorkCodeBatch()
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

            var test = new SearchController(pipelineManagerMock.Object, mapperMock.Object, contextAcessorMock.Object);
            var response = await test.SearchByAgencyWorkCodeBatchAsync(new List<AgencyWorkCodeSearchModel>());

			Assert.IsType<MultiStatusObjectResult>(response.Result);
		}

		/// <summary>
		/// Tests Search by Iswc Multistatus response
		/// </summary>
		[Fact]
		public async void SearchByISWCBatch()
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

            var test = new SearchController(pipelineManagerMock.Object, mapperMock.Object, contextAcessorMock.Object);
            var response = await test.SearchByISWCBatchAsync(new List<IswcSearchModel>());

			Assert.IsType<MultiStatusObjectResult>(response.Result);
		}

        /// <summary>
        /// Tests SearchByTitleAndContributor Multistatus response
        /// </summary>
        [Fact]
        public async void SearchByTitleAndContributorBatch()
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

            mapperMock.Setup(x => x.Map<ICollection<InterestedPartyModel>>(It.IsAny<List<InterestedParty>>()))
                .Returns(It.IsAny<ICollection<InterestedPartyModel>>());
            mapperMock.Setup(x => x.Map<ICollection<Bdo.Work.Title>>(It.IsAny<List<Title>>()))
                .Returns(It.IsAny<ICollection<Bdo.Work.Title>>());
            mapperMock.Setup(x => x.Map<ISWCMetadata>(It.IsAny<IswcModel>()))
                .Returns(It.IsAny<ISWCMetadata>());

            var test = new SearchController(pipelineManagerMock.Object, mapperMock.Object, contextAcessorMock.Object);
            var response = await test.SearchByTitleAndContributorBatchAsync(new List<TitleAndContributorSearchModel>());

            Assert.IsType<MultiStatusObjectResult>(response.Result);
        }
    }
}
