using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent;
using SpanishPoint.Azure.Iswc.Pipelines.MatchingPipeline;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MatchingPipeline.Tests
{
    /// <summary>
    /// Tests for MatchingPipeline
    /// </summary>
    public class MatchingPipelineTests
    {
        /// <summary>
        /// Check ValidationPipeline return type
        /// </summary>
        [Fact]
        public async Task CheckValidationPipelineReturnType()
        {
            IEnumerable<Submission> submissions = new List<Submission>();

            var pipelineMock = new Mock<IMatchingPipeline>();
            pipelineMock.Setup(p => p.RunPipeline(submissions)).ReturnsAsync(submissions);

            var response = await pipelineMock.Object.RunPipeline(submissions);

            Assert.IsAssignableFrom<IEnumerable<Submission>>(response);
        }

        /// <summary>
        /// Check pipeline executes all required components
        /// </summary>
        [Fact]
        public async Task PipelineExecutesAllRequiredComponents()
        {
            IEnumerable<Submission> submissions = new List<Submission>();

            var matchingComponent = new Mock<IMatchingComponent>();
            var pipelineMock = new Mock<Pipelines.MatchingPipeline.MatchingPipeline>(matchingComponent.Object);

            var response = await pipelineMock.Object.RunPipeline(submissions);

            matchingComponent.Verify(s => s.ProcessBatch(submissions), Times.Once);
        }
    }
}
