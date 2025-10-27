using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator;
using SpanishPoint.Azure.Iswc.Pipelines.PostMatchingPipeline;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingPipeline.Tests
{
    /// <summary>
    /// Tests for PostMatchingPipeline
    /// </summary>
    public class PostMatchingPipelineTests
    {
        /// <summary>
        /// Check ValidationPipeline return type
        /// </summary>
        [Fact]
        public async void CheckValidationPipelineReturnType()
        {
            IEnumerable<Submission> submissions = new List<Submission>();

            var pipelineMock = new Mock<IPostMatchingPipeline>();
            pipelineMock.Setup(p => p.RunPipeline(submissions)).ReturnsAsync(submissions);

            var response = await pipelineMock.Object.RunPipeline(submissions);

            Assert.IsAssignableFrom<IEnumerable<Submission>>(response);
        }

        /// <summary>
        /// Check pipeline executes all required components
        /// </summary>
        [Fact]
        public async void PipelineExecutesAllRequiredComponents()
        {
            IEnumerable<Submission> submissions = new List<Submission>();

            var postMatchingValidatorMock = new Mock<IPostMatchingValidator>();
            var pipelineMock = new Mock<Pipelines.PostMatchingPipeline.PostMatchingPipeline>(postMatchingValidatorMock.Object);

            var response = await pipelineMock.Object.RunPipeline(submissions);

            postMatchingValidatorMock.Verify(s => s.ValidateBatch(submissions), Times.Once);
        }
    }
}
