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
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Tests.ProcessingSubComponents
{
	/// <summary>
	/// Checks AS_12
	/// </summary>
	public class AS_12Tests
	{
		/// <summary>
		/// Check full AS_12 mapping from submission
		/// </summary>
		[Fact]
		public void AS_12_Valid()
		{
			var workRepo = new Mock<IWorkRepository>().Object;
			var workManagerMock = new Mock<IWorkManager>().Object;
			var mapperMock = new Mock<IMapper>().Object;
			var as12Mock = new Mock<AS_12>(workManagerMock, new Mock<IIswcRepository>().Object).Object;

			Assert.Contains(TransactionType.MER, as12Mock.ValidTransactionTypes);
			Assert.True(as12Mock.IsEligible);
			Assert.Equal(nameof(AS_12), as12Mock.Identifier);
		}

		/// <summary>
		/// Checks mapping from submission model to IswcLinkedTo
		/// </summary>
		[Fact]
		public void AS_12_Check_Mapping_ISWCLinkedTo()
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
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetIndexParameters().Length == 0 && p.GetMethod.IsPublic;
                cfg.AddProfile(mappingProfile);
            });
            var mapper = new Mapper(mapperConfig);

			var linkedTo = mapper.Map(submission.Model, new IswclinkedTo());

			Assert.Equal(submission.Model.PreferredIswc, linkedTo.LinkedToIswc);
			Assert.True(linkedTo.Status);
		}
	}
}
