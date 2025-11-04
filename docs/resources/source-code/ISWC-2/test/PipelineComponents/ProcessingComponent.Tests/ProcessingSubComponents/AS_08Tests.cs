using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs;
using SpanishPoint.Azure.Iswc.Data.Services.Checksum;
using SpanishPoint.Azure.Iswc.Data.Services.IswcService;
using SpanishPoint.Azure.Iswc.Data.Services.Notifications;
using SpanishPoint.Azure.Iswc.Data.Services.Search;
using SpanishPoint.Azure.Iswc.Data.Services.UpdateWorkflowHistory;
using SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Tests.ProcessingSubComponents
{
    /// <summary>
    /// Checks for processing scenario AS_08
    /// </summary>
    public class AS_08Tests : ProcessingTestBase
    {
        /// <summary>
        /// Check full AS_08 mapping from submission
        /// </summary>
        [Fact]
        public void AS_08_Valid()
        {
            var workManager = new Mock<IWorkManager>();
            var as_10Mock = new Mock<IAS_10>();
            var mapperMock = new Mock<IMapper>();
            var as08Mock = new Mock<AS_08>(workManager.Object, mapperMock.Object, as_10Mock.Object).Object;

            Assert.Equal(PreferedIswcType.Different, as08Mock.PreferedIswcType);
            Assert.False(as08Mock.IsEligible);
            Assert.Contains(TransactionType.CUR, as08Mock.ValidTransactionTypes);
            Assert.Equal(nameof(AS_08), as08Mock.Identifier);
        }

        /// <summary>
        /// Check ISWC value has been changed to the new Preferred ISWC in the WorkInfo 
        /// and Title, DisambiguationIswc, DerivedFrom, Creator, Publisher tables.
        /// </summary>
        [Fact]
        public void AS_08_CheckUpdateISWC()
        {
            var workRepository = new Mock<IWorkRepository>();
            var iswcRepository = new Mock<IIswcRepository>();
            var mapper = new Mock<IMapper>();
            var iswcService = new Mock<IIswcService>();
            var instrumentationRepository = new Mock<IInstrumentationRepository>();
            var mergeRequestRepository = new Mock<IMergeRequestRepository>();
            var searchService = new Mock<ISearchService>();
            var notificationService = new Mock<INotificationService>();
            var IUpdateWorkflowHistoryService = new Mock<IUpdateWorkflowHistoryService>();
            var workflowRepository = new Mock<IWorkflowRepository>();
            var performerRepository = new Mock<IPerformerRepository>();
            var publisherCodeRepository = new Mock<IPublisherCodeRespository>();
            var recordingRepository = new Mock<IRecordingRepository>();
            var numberTypeRespository = new Mock<INumberTypeRepository>();
            var httpContext = new Mock<IHttpContextAccessor>();
            var iswcLinkedToRespository = new Mock<IIswcLinkedToRepository>();
            var checksumService = new Mock<IChecksumService>();
            var cacheIswcService = new Mock<ICacheIswcService>();

            var workManager = new Mock<WorkManager>(workRepository.Object, iswcRepository.Object, mapper.Object, iswcService.Object,
                instrumentationRepository.Object, mergeRequestRepository.Object, searchService.Object, notificationService.Object, workflowRepository.Object,
                IUpdateWorkflowHistoryService.Object, performerRepository.Object, publisherCodeRepository.Object, recordingRepository.Object,
                numberTypeRespository.Object, httpContext.Object, iswcLinkedToRespository.Object, checksumService.Object, cacheIswcService.Object);

            var iswc = new Data.DataModels.Iswc
            {
                Iswc1 = "T123",
                IswcId = 2
            };

            var workInfo = new WorkInfo
            {
                IswcId = 1,
                Title = new List<Title> { new Title { IswcId = 1 } },
                Creator = new List<Creator> { new Creator { IswcId = 1 } },
                Publisher = new List<Publisher> { new Publisher { IswcId = 1 } },
                DerivedFrom = new List<DerivedFrom> { new DerivedFrom { Iswc = "T122" } },
                DisambiguationIswc = new List<DisambiguationIswc> { new DisambiguationIswc { Iswc = "T122" } }
            };

            var result = workManager.Object.ChangeWorkInfoIswcDetails(workInfo, iswc, DateTime.UtcNow);

            Assert.Equal(iswc.IswcId, result.IswcId);
            Assert.Equal(iswc.IswcId, result.Title.First().IswcId);
            Assert.Equal(iswc.IswcId, result.Creator.First().IswcId);
            Assert.Equal(iswc.IswcId, result.Publisher.First().IswcId);
            Assert.Equal(iswc.Iswc1, result.DerivedFrom.First().Iswc);
            Assert.Equal(iswc.Iswc1, result.DisambiguationIswc.First().Iswc);
        }
    }
}
