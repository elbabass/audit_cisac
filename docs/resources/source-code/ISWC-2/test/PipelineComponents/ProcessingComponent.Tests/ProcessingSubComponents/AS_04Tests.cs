using AutoMapper;
using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Data;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Data.Services.IswcService;
using SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Processing;
using System.Linq;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Tests.ProcessingSubComponents
{
    /// <summary>
    /// Checks for processing scenario AS_04
    /// </summary>
    public class AS_04Tests : ProcessingTestBase
    {
        /// <summary>
        /// Check full AS_04 mapping from submission
        /// </summary>
        [Fact]
        public void AS_04_Valid()
        {
            var workRepo = new Mock<IWorkRepository>().Object;
            var mapper = new Mock<IMapper>().Object;
            var iswcRepo = new Mock<IIswcRepository>().Object;
            var iswcService = new Mock<IIswcService>().Object;
            var instrumentationRepository = new Mock<IInstrumentationRepository>().Object;
            var workManager = new Mock<IWorkManager>();
            var mapperMock = new Mock<IMapper>();
            var as_10Mock = new Mock<IAS_10>();
            var as04Mock = new Mock<AS_04>(workManager.Object, as_10Mock.Object, mapperMock.Object).Object;

            Assert.Equal(PreferedIswcType.Existing, as04Mock.PreferedIswcType);
            Assert.True(as04Mock.IsEligible);
            Assert.Contains(TransactionType.CUR, as04Mock.ValidTransactionTypes);
            Assert.Equal(nameof(AS_04), as04Mock.Identifier);
        }

        /// <summary>
        /// Check Title mapping from submission
        /// </summary>
        [Fact]
        public void AS_02_CheckMapping_Title()
        {
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetIndexParameters().Length == 0 && p.GetMethod.IsPublic;
                cfg.AddProfile(myProfile);
            });
            var mapper = new Mapper(configuration);

            var submission = SubmissionForMapping;
            var workInfo = new WorkInfo();

            workInfo = mapper.Map(submission, workInfo);

            Assert.Equal(submission.Model.Titles.Count(), workInfo.Title.Count);
        }

        /// <summary>
        /// Check Creator mapping from submission
        /// </summary> 
        [Fact]
        public void AS_02_CheckMapping_Creator()
        {
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetIndexParameters().Length == 0 && p.GetMethod.IsPublic;
                cfg.AddProfile(myProfile);
            });
            var mapper = new Mapper(configuration);

            var submission = SubmissionForMapping;
            var workInfo = new WorkInfo();
            var submissionIps = submission.Model.InterestedParties.Where(ip => ip.CisacType == Bdo.Ipi.CisacInterestedPartyType.C);

            workInfo = mapper.Map(submission, workInfo);

            Assert.Equal(submissionIps.Count(), workInfo.Creator.Count);
        }

        /// <summary>
        /// Check Publishers mapping from submission
        /// </summary> 
        [Fact]
        public void AS_02_CheckMapping_Publishers()
        {
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetIndexParameters().Length == 0 && p.GetMethod.IsPublic;
                cfg.AddProfile(myProfile);
            });
            var mapper = new Mapper(configuration);

            var submission = SubmissionForMapping;
            var workInfo = new WorkInfo();

            workInfo = mapper.Map(submission, workInfo);

            var publishers = submission.Model.InterestedParties.Where(ip => ip.CisacType == Bdo.Ipi.CisacInterestedPartyType.E
            || ip.CisacType == Bdo.Ipi.CisacInterestedPartyType.AM);

            Assert.Equal(publishers.Count(), workInfo.Publisher.Count());
        }
    }

}
