using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule IV_44
    /// </summary>
    public class IV_44Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if no PreferredISWC is provided
        /// </summary>
        [Fact]
        public async void IV_44_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var test = new Mock<IV_44>(GetMessagingManagerMock(ErrorCode._107));

            submission.Model.PreferredIswc = "";

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_44), test.Object.Identifier);
            Assert.Equal(ErrorCode._107, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if PreferredISWC is provided
        /// </summary>
        [Fact]
        public async void IV_44_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var test = new Mock<IV_44>(GetMessagingManagerMock(ErrorCode._107));

            submission.Model.PreferredIswc = "value";

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_44), test.Object.Identifier);
        }
    }
}
