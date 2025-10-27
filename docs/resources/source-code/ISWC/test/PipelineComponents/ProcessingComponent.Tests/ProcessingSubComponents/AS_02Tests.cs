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
    /// Checks for processing scenario AS_02
    /// </summary>
    public class AS_02Tests : ProcessingTestBase
    {
        /// <summary>
        /// Checks full AS_02 mapping from submission
        /// </summary>
        [Fact]
        public void AS_02_Valid()
        {
            var workRepo = new Mock<IWorkRepository>().Object;
            var mapper = new Mock<IMapper>().Object;
            var iswcRepo = new Mock<IIswcRepository>().Object;
            var iswcService = new Mock<IIswcService>().Object;
            var instrumentationRepository = new Mock<IInstrumentationRepository>().Object;
            var workManager = new Mock<IWorkManager>();
            var as_10Mock = new Mock<IAS_10>().Object;
            var mapperMock = new Mock<IMapper>();
            var as02Mock = new Mock<AS_02>(workManager.Object, as_10Mock, mapperMock.Object).Object;

            Assert.Equal(PreferedIswcType.Existing, as02Mock.PreferedIswcType);
            Assert.False(as02Mock.IsEligible);
            Assert.Contains(TransactionType.CAR, as02Mock.ValidTransactionTypes);
            Assert.Equal(nameof(AS_02), as02Mock.Identifier);
        }

        /// <summary>
        /// Check WorkInfo mapping from submission
        /// </summary>
        [Fact]
        public void AS_02_CheckMapping_WorkInfo()
        {
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);

            var submission = SubmissionForMapping;

            var workInfo = new WorkInfo()
            {
                IswcId = 1
            };

            workInfo = mapper.Map(submission, workInfo);

            Assert.True(workInfo.Status);
            Assert.Equal(submission.Model.InterestedParties.Count(), workInfo.Ipcount);
            Assert.Equal(submission.Model.Iswc, workInfo.ArchivedIswc);
            Assert.False(workInfo.IsReplaced);
            Assert.Equal((int)submission.MatchedResult.Matches.FirstOrDefault().MatchType, workInfo.MatchTypeId);
            Assert.Equal(submission.Model.Category.ToString(), workInfo.MwiCategory);
            Assert.Equal(submission.Model.Agency, workInfo.AgencyId);
            Assert.Equal(submission.IsEligible, workInfo.IswcEligible);
            Assert.Equal(submission.Model.WorkNumber.Number, workInfo.AgencyWorkCode);
            Assert.Equal(submission.Model.SourceDb, workInfo.SourceDatabase);
            Assert.Equal(submission.Model.Disambiguation, workInfo.Disambiguation);
            Assert.Equal((int)submission.Model.DisambiguationReason, workInfo.DisambiguationReasonId);
            Assert.Equal(submission.Model.BVLTR.ToString().ToCharArray()[0], workInfo.Bvltr.ToCharArray()[0]);
            Assert.Equal((int)submission.Model.DerivedWorkType, workInfo.DerivedWorkTypeId);
        }

        /// <summary>
        /// Check Title mapping from submission
        /// </summary>
        [Fact]
        public void AS_02_CheckMapping_Title()
        {
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
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
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);

            var submission = SubmissionForMapping;
            var workInfo = new WorkInfo();
            var submissionIps = submission.Model.InterestedParties.Where(ip => ip.CisacType == Bdo.Ipi.CisacInterestedPartyType.C);

            workInfo = mapper.Map(submission, workInfo);

            Assert.Equal(submissionIps.Count(), workInfo.Creator.Count);
        }

        /// <summary>
        /// Check WorkInfoPerformers and Performers mapping from submission
        /// </summary> 
        [Fact]
        public void AS_02_CheckMapping_WorkInfoPerformers_And_Performers()
        {
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);

            var submission = SubmissionForMapping;
            var workInfo = new WorkInfo();
            var performers = submission.Model.Performers;

            workInfo = mapper.Map(submission, workInfo);

            Assert.Equal(performers.Count(), workInfo.WorkInfoPerformer.Count());
            Assert.Collection(workInfo.WorkInfoPerformer,
                c => Assert.Equal(performers.First().FirstName, c.Performer.FirstName),
                c => Assert.Equal(performers.Last().LastName, c.Performer.LastName));
        }

        /// <summary>
        /// Check Publishers mapping from submission
        /// </summary> 
        [Fact]
        public void AS_02_CheckMapping_Publishers()
        {
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);

            var submission = SubmissionForMapping;
            var workInfo = new WorkInfo();

            workInfo = mapper.Map(submission, workInfo);

            var publishers = submission.Model.InterestedParties.Where(ip => ip.CisacType == Bdo.Ipi.CisacInterestedPartyType.E
            || ip.CisacType == Bdo.Ipi.CisacInterestedPartyType.AM);

            Assert.Equal(publishers.Count(), workInfo.Publisher.Count());
        }

        /// <summary>
        /// Check DerivedFrom mapping from submission
        /// </summary> 
        [Fact]
        public void AS_02_CheckMapping_DerivedFrom()
        {
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);

            var submission = SubmissionForMapping;
            var workInfo = new WorkInfo();

            workInfo = mapper.Map(submission, workInfo);

            Assert.Equal(submission.Model.DerivedFrom.Count(), workInfo.DerivedFrom.Count());
        }

        /// <summary>
        /// Check DisambiguationISWC mapping from submission
        /// </summary> 
        [Fact]
        public void AS_02_CheckMapping_DisambiguationISWC()
        {
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);

            var submission = SubmissionForMapping;
            var workInfo = new WorkInfo();

            workInfo = mapper.Map(submission, workInfo);

            Assert.Equal(submission.Model.DisambiguateFrom.Count(), workInfo.DerivedFrom.Count());
        }

        /// <summary>
        /// Check WorkInfoInstrumentation mapping from submission
        /// </summary> 
        [Fact]
        public void AS_02_CheckMapping_WorkInfoInstrumentation()
        {
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);

            var submission = SubmissionForMapping;
            var workInfo = new WorkInfo();

            workInfo = mapper.Map(submission, workInfo);

            Assert.Equal(submission.Model.Instrumentation.Count(), workInfo.WorkInfoInstrumentation.Count());
        }
    }
}
