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
    /// Tests for rule PV_22
    /// </summary>
    public class PV_22Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if Preferred ISWC does not exist in the db
        /// </summary>
        [Fact]
        public async Task PV_22_PreferredIswcDoesNotExist_Invalid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CUR };
            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(i => i.FindAsync("10", false, false)).ReturnsAsync(default(SubmissionModel));

            var test = new Mock<PV_22>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._117)).Object;

            submission.Model.PreferredIswc = "10";

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_22), test.Identifier);
            Assert.Equal(ErrorCode._117, response.Submission.Rejection.Code);
        }

        /// <summary>
        ///  Check transaction passes if Preferred ISWC exists in the db
        /// </summary>
        [Fact]
        public async Task PV_22_IswcDoesExist_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CUR };
            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(i => i.Exists("T2030000019")).ReturnsAsync(true);

            var test = new Mock<PV_22>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._117)).Object;

            submission.Model.PreferredIswc = "T2030000019";

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_22), test.Identifier);
        }

        /// <summary>
        /// Check transaction fails if Preferred ISWC in the DB does not match the Preferred ISWC in the submission.
        /// </summary>
        [Fact]
        public async Task PV_22_PreferredIswcDoesNotMatch_Invalid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CDR };
            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(i => i.FindAsync("T2030000019", false, false)).
                ReturnsAsync(new SubmissionModel() { Iswc = "T2030000019" });

            workManagerMock.Setup(w => w.FindAsync(It.IsAny<WorkNumber>()))
                .ReturnsAsync(new SubmissionModel() { Iswc = "T2030000097" });

            var test = new Mock<PV_22>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._117)).Object;

            submission.Model.PreferredIswc = "T2030000019";

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._117, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if Preferred ISWC in the DB  matches the Preferred ISWC in the submission.
        /// </summary>
        [Fact]
        public async Task PV_22_Submission_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() { WorkNumber = new WorkNumber() }, TransactionType = TransactionType.CDR };
            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(i => i.Exists("T2030000097")).
                ReturnsAsync(true);

            workManagerMock.Setup(w => w.FindVerifiedAsync(It.IsAny<WorkNumber>()))
                .ReturnsAsync(new VerifiedSubmissionModel() { Iswc = "T2030000097" });

            var test = new Mock<PV_22>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._117)).Object;

            submission.Model.PreferredIswc = "T2030000097";

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check transaction fails if worknumber is null
        /// </summary>
        [Fact]
        public async Task PV_22_Submission_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() , TransactionType = TransactionType.CDR };
            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(i => i.Exists("T2030000097")).
                ReturnsAsync(true);

            var test = new Mock<PV_22>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._117)).Object;

            submission.Model.PreferredIswc = "T2030000097";

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._117, response.Submission.Rejection.Code);
        }
    }
}
