using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.PipelineComponents.IswcEligibilityValidator;
using SpanishPoint.Azure.Iswc.PipelineComponents.LookupDataValidator;
using SpanishPoint.Azure.Iswc.PipelineComponents.MetadataStandardizationValidator;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ValidationPipeline.Tests
{
    /// <summary>
    /// Tests for ValidationPipeline
    /// </summary>
    public class ValidationPipelineTests
    {
        /// <summary>
        /// Check validation pipeline return type
        /// </summary>
        [Fact]
        public async Task CheckValidationPipelineReturnType()
        {
            IEnumerable<Submission> submissions = new List<Submission>();

            var pipelineMock = new Mock<Pipelines.ValidationPipeline.IValidationPipeline>();
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
            IEnumerable<Submission> submissions = new List<Submission> { new Submission() };

            var staticDataValidatorMock = new Mock<IStaticDataValidator>();
            var metadataStandardizationValidatorMock = new Mock<IMetadataStandardizationValidator>();
            var lookupDataValidatorMock = new Mock<ILookupDataValidator>();
            var iswcEligibilityValidatorMock = new Mock<IIswcEligibilityValidator>();

            var pipelineMock = new Mock<Pipelines.ValidationPipeline.ValidationPipeline>(staticDataValidatorMock.Object,
                metadataStandardizationValidatorMock.Object, lookupDataValidatorMock.Object, iswcEligibilityValidatorMock.Object);

            var response = await pipelineMock.Object.RunPipeline(submissions);

            staticDataValidatorMock.Verify(s => s.ValidateBatch(It.IsAny<IEnumerable<Submission>>()), Times.Once);
            metadataStandardizationValidatorMock.Verify(s => s.ValidateBatch(It.IsAny<IEnumerable<Submission>>()), Times.Once);
            lookupDataValidatorMock.Verify(s => s.ValidateBatch(It.IsAny<IEnumerable<Submission>>()), Times.Once);
            iswcEligibilityValidatorMock.Verify(s => s.ValidateBatch(It.IsAny<IEnumerable<Submission>>()), Times.Once);

        }
    }
}
