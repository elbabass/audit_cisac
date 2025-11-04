using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.PostMatching;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.Tests.LabelPostMatching
{
    /// <summary>
    /// Tests for MatchIsrcs Component
    /// </summary>
    public class MatchIsrcsTests
    {
        /// <summary>
        /// Checks that if an Isrc match is found the submission is populated and returned
        /// </summary>
        [Fact]
        public async Task MatchIsrcs()
        {
            // Arrange
            var submissions = new List<Submission>()
            {
                new Submission() 
                { 
                    SubmissionId = 1, 
                    IsEligible = false, 
                    Model = new SubmissionModel 
                    {
                        AdditionalIdentifiers = new List<AdditionalIdentifier>
                        {
                                new AdditionalIdentifier { SubmitterCode = "ISRC", WorkCode = "GBUM71000001" }
                        }
                    } 
                }
            };

            var matchingManager = new Mock<IMatchingManager>();
            matchingManager.Setup(m => m.MatchIsrcsAsync(It.IsAny<IEnumerable<Submission>>())).ReturnsAsync(submissions);

            var additionalIdentifierManager = new Mock<IAdditionalIdentifierManager>().Object;
            var workManager = new Mock<IWorkManager>().Object;

            var component = new MatchIsrcs(additionalIdentifierManager, matchingManager.Object, workManager);

            // Act
            var result = await component.ProcessSubmissions(submissions);

            // Assert
            Assert.Same(submissions, result);
            matchingManager.Verify(m => m.MatchIsrcsAsync(submissions), Times.Once);
        }
    }
}
