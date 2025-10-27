using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent;
using SpanishPoint.Azure.Iswc.Pipelines.ProcessingPipeline;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingPipeline.Tests
{
    /// <summary>
    /// Tests for ProcessingPipelineTests
    /// </summary>
    public class ProcessingPipelineTests
    {
        /// <summary>
        /// Check ValidationPipeline return type
        /// </summary>
        [Fact]
        public async void CheckValidationPipelineReturnType()
        {
            IEnumerable<Submission> submissions = new List<Submission>();

            var pipelineMock = new Mock<IProcessingPipeline>();
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

            var processingComponent = new Mock<IProcessingComponent>();
            var pipelineMock = new Mock<Pipelines.ProcessingPipeline.ProcessingPipeline>(processingComponent.Object);
            
            var response = await pipelineMock.Object.RunPipeline(submissions);

            processingComponent.Verify(s => s.ProcessBatch(submissions), Times.Once);
        }
    }
}
