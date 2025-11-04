using AutoMapper;
using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Processing;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Tests.ProcessingSubComponents
{
    /// <summary>
    /// Checks for processing scenario AS_14
    /// </summary>
    public class AS_14Tests : ProcessingTestBase
    {
        /// <summary>
        /// Check full AS_14 mapping from submission
        /// </summary>
        [Fact]
        public void AS_14_Valid()
        {
            // Arrange
            var validRequestTypes = new List<RequestType> { RequestType.Agency, RequestType.Label, RequestType.Publisher, RequestType.ThirdParty };

            var workManager = new Mock<IWorkManager>();
            var workRepository = new Mock<IWorkRepository>();
            var as_10Mock = new Mock<IAS_10>();
            var mapperMock = new Mock<IMapper>();

            // Act
            var as14Mock = new Mock<AS_14>(workManager.Object, workRepository.Object, mapperMock.Object, as_10Mock.Object).Object;

            // Assert
            Assert.Equal(PreferedIswcType.Existing, as14Mock.PreferedIswcType);
            Assert.False(as14Mock.IsEligible);
            Assert.Contains(TransactionType.CUR, as14Mock.ValidTransactionTypes);
            Assert.Equal(nameof(AS_14), as14Mock.Identifier);
            Assert.Equal(validRequestTypes, as14Mock.ValidRequestTypes);

        }
    }
}
