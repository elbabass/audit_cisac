using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Processing;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Tests.ProcessingSubComponents
{
    /// <summary>
    /// Checks AS_09
    /// </summary>
    public class AS_09Tests
	{
        /// <summary>
        /// Check full AS_09 mapping from submission
        /// </summary>
        [Fact]
		public void AS_09_Valid()
		{
			var workManagerMock = new Mock<IWorkManager>().Object;
			var as09Mock = new Mock<AS_09>(workManagerMock).Object;

			Assert.Equal(PreferedIswcType.Existing, as09Mock.PreferedIswcType);
			Assert.Contains(TransactionType.CDR, as09Mock.ValidTransactionTypes);
			Assert.Null(as09Mock.IsEligible);
			Assert.Equal(nameof(AS_09), as09Mock.Identifier);
		}
	}
}
