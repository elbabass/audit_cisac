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
    /// IV/49 tests
    /// </summary>
    public class IV_49Tests : TestBase
    {
        /// <summary>
        /// Check rejection message when agency is not present
        /// </summary>
        [Fact]
        public async Task IV_49_AgencyNull()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    Agency = ""
                }
            };

            var test = new Mock<IV_49>(GetMessagingManagerMock(ErrorCode._166));

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_49), test.Object.Identifier);
            Assert.Equal(ErrorCode._166, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check submission when agency is 312
        /// </summary>
        [Fact]
        public async Task IV_49_AgencyPresent()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    Agency = "312"
                }
            }; 

            var test = new Mock<IV_49>(GetMessagingManagerMock(ErrorCode._166));

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_49), test.Object.Identifier);
        }
    }
}
