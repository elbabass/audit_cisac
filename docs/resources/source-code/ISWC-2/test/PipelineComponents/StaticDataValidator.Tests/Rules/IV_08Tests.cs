using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule IV_08
    /// </summary>
    public class IV_08Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if combination of submitting agency and source db does not exist in the database
        /// </summary>
        [Fact]
        public async Task IV_08_Submission_InValid()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    SourceDb = 99,
                    Agency = "99"
                }
            };
            var sourceSubmissionManagerMock = new Mock<ISubmissionSourceManager>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var agencyManagerMock = new Mock<IAgencyManager>();

            sourceSubmissionManagerMock.Setup(a => a.FindAsync(99.ToString())).ReturnsAsync(default(SubmissionSourceModel));
            sourceSubmissionManagerMock.Setup(a => a.FindAsync("99")).ReturnsAsync(default(SubmissionSourceModel));
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateSubmittingAgency")).ReturnsAsync(true);
            agencyManagerMock.Setup(a => a.FindAsync("99")).ReturnsAsync("99");

            var test = new Mock<IV_08>(sourceSubmissionManagerMock.Object, GetMessagingManagerMock(ErrorCode._115), rulesManagerMock.Object, agencyManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_08), test.Object.Identifier);
            Assert.Equal(ErrorCode._115, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if combination of submitting agency and source db exists in the database
        /// </summary>
        [Fact]
        public async Task IV_08_Submission_Valid()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    SourceDb = 17,
                    Agency = "169"
                }
            };
            var sourceSubmissionManagerMock = new Mock<ISubmissionSourceManager>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var agencyManagerMock = new Mock<IAgencyManager>();

            sourceSubmissionManagerMock.Setup(a => a.FindAsync(17.ToAgencyCode())).ReturnsAsync(new SubmissionSourceModel() { Code = "017" });
            sourceSubmissionManagerMock.Setup(a => a.FindAsync("169")).ReturnsAsync(new SubmissionSourceModel() { Code = "169" });
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateSubmittingAgency")).ReturnsAsync(true);
            agencyManagerMock.Setup(a => a.FindAsync("169")).ReturnsAsync("169");

            var test = new Mock<IV_08>(sourceSubmissionManagerMock.Object, GetMessagingManagerMock(ErrorCode._115), rulesManagerMock.Object, agencyManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_08), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction passes if rule is false
        /// </summary>
        [Fact]
        public async Task IV_08_Submission_Valid_Rule()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    SourceDb = 17,
                    Agency = "169"
                }
            };
            var sourceSubmissionManagerMock = new Mock<ISubmissionSourceManager>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var agencyManagerMock = new Mock<IAgencyManager>();

            sourceSubmissionManagerMock.Setup(a => a.FindAsync(17.ToAgencyCode())).ReturnsAsync(new SubmissionSourceModel() { Code = "017" });
            sourceSubmissionManagerMock.Setup(a => a.FindAsync("169")).ReturnsAsync(new SubmissionSourceModel() { Code = "169" });
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateSubmittingAgency")).ReturnsAsync(false);
            agencyManagerMock.Setup(a => a.FindAsync("169")).ReturnsAsync("169");

            var test = new Mock<IV_08>(sourceSubmissionManagerMock.Object, GetMessagingManagerMock(ErrorCode._115), rulesManagerMock.Object, agencyManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_08), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction fails if sourceDb is 0
        /// </summary>
        [Fact]
        public async Task IV_08_Submission_InValid_SourceDb_0()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    SourceDb = 0,
                    Agency = "169"
                }
            };
            var sourceSubmissionManagerMock = new Mock<ISubmissionSourceManager>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var agencyManagerMock = new Mock<IAgencyManager>();

            sourceSubmissionManagerMock.Setup(a => a.FindAsync(17.ToAgencyCode())).ReturnsAsync(new SubmissionSourceModel() { Code = "017" });
            sourceSubmissionManagerMock.Setup(a => a.FindAsync("169")).ReturnsAsync(new SubmissionSourceModel() { Code = "169" });
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateSubmittingAgency")).ReturnsAsync(true);
            agencyManagerMock.Setup(a => a.FindAsync("169")).ReturnsAsync("169");


            var test = new Mock<IV_08>(sourceSubmissionManagerMock.Object, GetMessagingManagerMock(ErrorCode._138), rulesManagerMock.Object, agencyManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_08), test.Object.Identifier);
            Assert.Equal(ErrorCode._138, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if agency is not in database
        /// </summary>
        [Fact]
        public async Task IV_08_Submission_InValid_Agency()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    SourceDb = 17,
                    Agency = "169"
                }
            };
            var sourceSubmissionManagerMock = new Mock<ISubmissionSourceManager>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var agencyManagerMock = new Mock<IAgencyManager>();

            sourceSubmissionManagerMock.Setup(a => a.FindAsync(99.ToString())).ReturnsAsync(default(SubmissionSourceModel));
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateSubmittingAgency")).ReturnsAsync(true); ;

            var test = new Mock<IV_08>(sourceSubmissionManagerMock.Object, GetMessagingManagerMock(ErrorCode._103), rulesManagerMock.Object, agencyManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_08), test.Object.Identifier);
            Assert.Equal(ErrorCode._103, response.Submission.Rejection.Code);
        }
    }
}
