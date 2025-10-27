using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule IV_13
    /// </summary>
    public class IV_13Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if the Preferred ISWC is not provided
        /// </summary>
        [Fact]
        public async void IV_13_Submission_InValid()
        {
			var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("PreferredISWCRequiredforUpdate")).ReturnsAsync(true);

			var test = new Mock<IV_13>(GetMessagingManagerMock(ErrorCode._107), rulesManagerMock.Object);

            submission.Model.PreferredIswc = string.Empty;

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_13), test.Object.Identifier);
            Assert.Equal(ErrorCode._107, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if the Preferred ISWC is provided
        /// </summary>
        [Fact]
        public async void IV_13_Submission_Valid()
        {
			var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("PreferredISWCRequiredforUpdate")).ReturnsAsync(true);

			var test = new Mock<IV_13>(GetMessagingManagerMock(ErrorCode._107), rulesManagerMock.Object);

            submission.Model.PreferredIswc = "value";

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_13), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction passes if rule is false
        /// </summary>
        [Fact]
        public async void IV_13_Submission_Valid_rule()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("PreferredISWCRequiredforUpdate")).ReturnsAsync(false);

            var test = new Mock<IV_13>(GetMessagingManagerMock(ErrorCode._107), rulesManagerMock.Object);

            submission.Model.PreferredIswc = "value";

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_13), test.Object.Identifier);
        }
    }
}
