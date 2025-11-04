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
    /// Tests rule IV_10
    /// </summary>
    public class IV_10Tests : TestBase
    {
        /// <summary>
        /// Check that transaction fails if the AgencyID provided is not in the database
        /// </summary>
        [Fact]
        public async Task IV_10_Submission_InValid()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    Agency = "100"
                }
            };
            var agencyManagerMock = new Mock<IAgencyManager>();
            var rulesManagerMock = new Mock<IRulesManager>();

            agencyManagerMock.Setup(a => a.FindAsync("100")).ReturnsAsync((string)null);
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateAgencyCode")).ReturnsAsync(true);

            var test = new Mock<IV_10>(agencyManagerMock.Object, rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._103));

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_10), test.Object.Identifier);
            Assert.Equal(ErrorCode._103, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check that transaction passes if the AgencyID provided is in the database
        /// </summary>
        [Fact]
        public async Task IV_10_Submission_Valid()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    Agency = "80"
                }
            };
            var agencyManagerMock = new Mock<IAgencyManager>();
            var rulesManagerMock = new Mock<IRulesManager>();

            agencyManagerMock.Setup(a => a.FindAsync("80")).ReturnsAsync("80");
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateAgencyCode")).ReturnsAsync(true);

            var test = new Mock<IV_10>(agencyManagerMock.Object, rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._103));

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_10), test.Object.Identifier);
        }

        /// <summary>
        /// Check that transaction passes if rule is false
        /// </summary>
        [Fact]
        public async Task IV_10_Submission_Valid_Rule()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    Agency = "80"
                }
            };
            var agencyManagerMock = new Mock<IAgencyManager>();
            var rulesManagerMock = new Mock<IRulesManager>();

            agencyManagerMock.Setup(a => a.FindAsync("80")).ReturnsAsync("80");
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateAgencyCode")).ReturnsAsync(false);

            var test = new Mock<IV_10>(agencyManagerMock.Object, rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._103));

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_10), test.Object.Identifier);
        }
    }
}
