using Moq;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Processing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Tests.ProcessingSubComponents
{
    /// <summary>
    /// Checks AS_10
    /// </summary>
    public class AS_10Tests : ProcessingTestBase
    {
        /// <summary>
        /// Test AS_10 recalculate Authoritative flag is true test
        /// </summary>
        [Fact]
        public async Task AS_10_AuthoritativeFlagIsTrue()
        {
            var submission = SubmissionAS_10;

            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
            interestedPartyManagerMock.Setup(x => x.IsAuthoritative(submission.Model.InterestedParties.First(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(true);
            var rulesManagerMock = new Mock<IRulesManager>();
            rulesManagerMock.Setup(x => x.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("308(201)");

            var as_10Mock = new Mock<AS_10>(interestedPartyManagerMock.Object, rulesManagerMock.Object);
            var result = await as_10Mock.Object.RecaculateAuthoritativeFlag(submission);

            Assert.True(result.Model.InterestedParties.First().IsAuthoritative);
        }

        /// <summary>
        /// Test AS_10 recalculate Authoritative flag is false test
        /// </summary>
        [Fact]
        public async Task AS_10_AuthoritativeFlagIsFalse()
        {
            var submission = SubmissionAS_10;

            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
            interestedPartyManagerMock
                .Setup(x => x.IsAuthoritative(submission.Model.InterestedParties.First(), It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(false);
            var rulesManagerMock = new Mock<IRulesManager>();
            rulesManagerMock.Setup(x => x.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("308(201)");

            var as_10Mock = new Mock<AS_10>(interestedPartyManagerMock.Object, rulesManagerMock.Object);
            var result = await as_10Mock.Object.RecaculateAuthoritativeFlag(submission);

            Assert.False(result.Model.InterestedParties.First().IsAuthoritative);
        }
    }
}
