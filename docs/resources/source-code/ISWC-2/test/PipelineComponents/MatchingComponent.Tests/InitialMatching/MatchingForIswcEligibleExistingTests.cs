using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.InitialMatching;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.Tests.InitialMatching
{
    /// <summary>
    /// Tests for the MatchingForIswcEligibleExisting component
    /// </summary>
    public class MatchingForIswcEligibleExistingTests
    {
        /// <summary>
        /// Checks that submissions get matched and returned by the matching engine
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ProcessSubmissions()
        {
            var manager = new Mock<IMatchingManager>();
            var component = new MatchingForIswcEligibleExisting(manager.Object);

            var submissions = new List<Submission>()
            {
                new Submission() { SubmissionId = 1, IsEligible = true }
            };

            manager.Setup(m => m.MatchAsync(It.IsAny<IEnumerable<Submission>>(), It.IsAny<string>())).ReturnsAsync(submissions);

            var result = await component.ProcessSubmissions(submissions);

            Assert.Same(submissions, result);
            manager.Verify(m => m.MatchAsync(submissions, It.IsAny<string>()), Times.Once);
        }
    }
}
