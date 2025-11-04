using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule IV_15
    /// </summary>
    public class IV_15Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if the check digit formula is not met for ISWC
        /// </summary>
        [Fact]
        public async Task IV_15_Submission_InValid()
        {
			var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateISWCCheckDigit")).ReturnsAsync(true);

			var test = new Mock<IV_15>(GetMessagingManagerMock(ErrorCode._141), rulesManagerMock.Object);

            submission.Model.Iswc = "T0345246802";
            submission.Model.PreferredIswc = "T0345246801";

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_15), test.Object.Identifier);
            Assert.Equal(ErrorCode._141, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check that the transaction fails if the ISWC is not in a valid format
        /// </summary>
        [Fact]
        public async Task IV_15_Submission_IswcFormat_InValid()
        {
			var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateISWCCheckDigit")).ReturnsAsync(true);

			var test = new Mock<IV_15>(GetMessagingManagerMock(ErrorCode._113), rulesManagerMock.Object);

            submission.Model.Iswc = "T034524";
            submission.Model.PreferredIswc = "T0345246801";

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_15), test.Object.Identifier);
            Assert.Equal(ErrorCode._113, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if the check digit formula is met
        /// </summary>
        [Fact]
        public async Task IV_15_Submission_Valid()
        {
			var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateISWCCheckDigit")).ReturnsAsync(true);

			var test = new Mock<IV_15>(GetMessagingManagerMock(ErrorCode._141), rulesManagerMock.Object);

            submission.Model.Iswc = "T0345246801";
            submission.Model.PreferredIswc = "T0625246801";

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_15), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction passes if rule is false
        /// </summary>
        [Fact]
        public async Task IV_15_Submission_Valid_Rule()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateISWCCheckDigit")).ReturnsAsync(false);

            var test = new Mock<IV_15>(GetMessagingManagerMock(ErrorCode._141), rulesManagerMock.Object);

            submission.Model.Iswc = "T0345246801";
            submission.Model.PreferredIswc = "T0625246801";

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_15), test.Object.Identifier);
        }

        /// <summary>
        /// Check that the transaction fails if the preferred ISWC is not in a valid format
        /// </summary>
        [Fact]
        public async Task IV_15_Submission_Preferred_IswcFormat_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateISWCCheckDigit")).ReturnsAsync(true);

            var test = new Mock<IV_15>(GetMessagingManagerMock(ErrorCode._113), rulesManagerMock.Object);

            submission.Model.Iswc = "T0345246801";
            submission.Model.PreferredIswc = "T034524";

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_15), test.Object.Identifier);
            Assert.Equal(ErrorCode._113, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if the check digit formula is not met for preferred ISWC
        /// </summary>
        [Fact]
        public async Task IV_15_Submission_InValid_PreferredISWC()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateISWCCheckDigit")).ReturnsAsync(true);

            var test = new Mock<IV_15>(GetMessagingManagerMock(ErrorCode._141), rulesManagerMock.Object);

            submission.Model.Iswc = "T0345246801";
            submission.Model.PreferredIswc = "T0345246802";

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_15), test.Object.Identifier);
            Assert.Equal(ErrorCode._141, response.Submission.Rejection.Code);
        }
    }
}
