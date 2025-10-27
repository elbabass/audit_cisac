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
    /// IV/54 tests
    /// </summary>
    public class IV_54Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if rule is enabled but agent version provided does not exist in Iswc database.
        /// </summary>
        [Fact]
        public async void IV_54_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var agentVersionManger = new Mock<IAgentManager>();

            submission.AgentVersion = "1.0.0.0";

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateAgentVersion")).ReturnsAsync(true);
            agentVersionManger.Setup(a => a.CheckAgentVersionAsync(submission.AgentVersion)).ReturnsAsync(false);

            var test = new Mock<IV_54>(GetMessagingManagerMock(ErrorCode._173), rulesManagerMock.Object, agentVersionManger.Object);

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_54), test.Object.Identifier);
            Assert.Equal(ErrorCode._173, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if rule enabled and agent version provided exists in the iswc database
        /// </summary>
        [Fact]
        public async void IV_54_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var agentVersionManger = new Mock<IAgentManager>();

            submission.AgentVersion = "1.0.0.0";

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateAgentVersion")).ReturnsAsync(true);
            agentVersionManger.Setup(a => a.CheckAgentVersionAsync(submission.AgentVersion)).ReturnsAsync(true);

            var test = new Mock<IV_54>(GetMessagingManagerMock(ErrorCode._173), rulesManagerMock.Object, agentVersionManger.Object);

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_54), test.Object.Identifier);
        }
    }
}
