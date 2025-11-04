using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using System.Threading.Tasks;
using Xunit;


namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// IV/53 tests
    /// </summary>
    public class IV_53Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if ISWC begins with T3 and AllowProvidedIswc is true
        /// </summary>
        [Fact]
        public async Task IV_53_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var test = new Mock<IV_53>(GetMessagingManagerMock(ErrorCode._171));

            submission.Model.AllowProvidedIswc = true;
            submission.Model.Iswc = "T3000000001";

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_53), test.Object.Identifier);
            Assert.Equal(ErrorCode._171, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes AllowProvidedIswc is false
        /// </summary>
        [Fact]
        public async Task IV_53_Valid_AllowProvidedIswc()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var test = new Mock<IV_53>(GetMessagingManagerMock(ErrorCode._171));

            submission.Model.AllowProvidedIswc = false;
            submission.Model.Iswc = "T0714193302";

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_53), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction passes if ISWC doesn't begin with T3
        /// </summary>
        [Theory]
        [InlineData("T4000000001")]
        [InlineData("T2000000001")]
        [InlineData("T1000000001")]
        [InlineData("")]
        [InlineData(null)]
        public async Task IV_53_Valid_Iswc(string iswc)
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var test = new Mock<IV_53>(GetMessagingManagerMock(ErrorCode._171));

            submission.Model.AllowProvidedIswc = true;
            submission.Model.Iswc = iswc;

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_53), test.Object.Identifier);
        }
    }
}
