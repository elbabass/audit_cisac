using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Tests.Rules
{
    /// <summary>
    /// Tests for rule PV_23
    /// </summary>
    public class PV_23Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if Archived ISWC does not match the archived ISWC in the retrieved workinfo record
        /// </summary>
        [Fact]
        public async Task PV_23_IswcDoesNotMatch_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CUR };
            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(i => i.FindVerifiedAsync(It.IsAny<WorkNumber>()))
                .ReturnsAsync(new VerifiedSubmissionModel() { 
                    ArchivedIswc = "T0000000000"
                });

            var test = new Mock<PV_23>(GetMessagingManagerMock(ErrorCode._116), workManagerMock.Object).Object;

            submission.Model.Iswc = "T2030000371";

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._116, response.Submission.Rejection.Code);
            Assert.Equal(nameof(PV_23), test.Identifier);
        }

        /// <summary>
        /// Check transaction passes if Archived ISWC matches the archived ISWC in the retrieved workinfo record
        /// </summary>
        [Fact]
        public async Task PV_23_IswcDoesMatch_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() { WorkNumber = new WorkNumber() }, TransactionType = TransactionType.CUR };
            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(i => i.FindVerifiedAsync(It.IsAny<WorkNumber>()))
                .ReturnsAsync(new VerifiedSubmissionModel()
                {
                    ArchivedIswc = "T2030007474"
                });

            var test = new Mock<PV_23>(GetMessagingManagerMock(ErrorCode._116), workManagerMock.Object).Object;

            submission.Model.Iswc = "T2030007474";

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check transaction fails if Archived ISWC does not exist
        /// </summary>
        [Fact]
        public async Task PV_23_IswcDoesNotExist_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CUR };

            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(i => i.FindVerifiedAsync(It.IsAny<WorkNumber>()))
                .ReturnsAsync(new VerifiedSubmissionModel(){ });

            var test = new Mock<PV_23>(GetMessagingManagerMock(ErrorCode._116), workManagerMock.Object).Object;

            submission.Model.Iswc = "T2030007474";

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._116, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if Iswc is not provided
        /// </summary>
        [Fact]
        public async Task PV_23_IswcNotProvided_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CUR };

            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(i => i.FindVerifiedAsync(It.IsAny<WorkNumber>()))
                .ReturnsAsync(new VerifiedSubmissionModel(){ });

            var test = new Mock<PV_23>(GetMessagingManagerMock(ErrorCode._116), workManagerMock.Object).Object;

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
        }
    }
}
