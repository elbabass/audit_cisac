using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule PV_13
    /// </summary>
    public class PV_13Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if a deletion submission has no ReasonCode
        /// </summary>
        [Fact]
        public async void PV_13_Invalid()
        {
            var submission = new Submission() { Model = new SubmissionModel() { ReasonCode = "" }, TransactionType = TransactionType.CDR };

            var test = new Mock<PV_13>( GetMessagingManagerMock(ErrorCode._133)).Object;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_13), test.Identifier);
            Assert.Equal(ErrorCode._133, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if a deletion submission has a ReasonCode
        /// </summary>
        [Fact]
        public async void PV_13_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() { ReasonCode = "deletion reason" }, TransactionType = TransactionType.CDR };

            var test = new Mock<PV_13>(GetMessagingManagerMock(ErrorCode._133)).Object;

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_13), test.Identifier);
        }
    }
}
