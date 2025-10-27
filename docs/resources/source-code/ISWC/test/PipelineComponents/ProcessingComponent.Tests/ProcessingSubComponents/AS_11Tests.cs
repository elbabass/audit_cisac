using System.Collections.Generic;
using AutoMapper;
using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Data;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Processing;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Tests.ProcessingSubComponents
{
    /// <summary>
	/// Checks AS_11
	/// </summary>
    public class AS_11Tests
    {
		/// <summary>
		/// Check full AS_11 mapping from submission
		/// </summary>
		[Fact]
		public void AS_11_Valid()
		{
			var workRepo = new Mock<IWorkRepository>().Object;
			var workManagerMock = new Mock<IWorkManager>().Object;
			var mapperMock = new Mock<IMapper>().Object;
			var as11Mock = new Mock<AS_11>(workManagerMock, new Mock<IIswcRepository>().Object).Object;

			Assert.Contains(TransactionType.DMR, as11Mock.ValidTransactionTypes);
			Assert.True(as11Mock.IsEligible);
			Assert.Equal(nameof(AS_11), as11Mock.Identifier);
		}

		/// <summary>
		/// Checks mapping from submission model to IswcLinkedTo
		/// </summary>
		[Fact]
		public void AS_11_Check_Mapping_ISWCLinkedTo()
		{
			var submission = new Submission()
			{
				Model = new SubmissionModel
				{
					PreferredIswc = "1234",
					WorkNumbersToMerge = new List<WorkNumber>() {
						new WorkNumber { Type = "4" , Number = "57748" },
						new WorkNumber { Type = "4" , Number = "57749" }
					}
				}
			};

			var mappingProfile = new MappingProfile();
			var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(mappingProfile));
			var mapper = new Mapper(mapperConfig);

			var linkedTo = mapper.Map(submission.Model, new IswclinkedTo());

			Assert.Equal(submission.Model.PreferredIswc, linkedTo.LinkedToIswc);
			Assert.True(linkedTo.Status);
		}
	}
}
