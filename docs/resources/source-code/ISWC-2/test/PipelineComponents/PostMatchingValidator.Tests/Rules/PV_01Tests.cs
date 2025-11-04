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
    /// Tests rule PV_01
    /// </summary>
    public class PV_01Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if work number does not exist in the database (CDR record)
        /// </summary>
        [Fact]
        public async Task PV_01_Submission_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CDR };
            var workManagerMock = new Mock<IWorkManager>();

            var workNumber = new WorkNumber() { Type = "100", Number = "10" };

            workManagerMock.Setup(a => a.FindAsync(It.IsAny<WorkNumber>()))
                .ReturnsAsync(default(SubmissionModel));

            var test = new Mock<PV_01>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._130));

            submission.Model.WorkNumber = workNumber;

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_01), test.Object.Identifier);
            Assert.Equal(ErrorCode._130, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if work number does exist in the database (CDR record)
        /// </summary>
        [Fact]
        public async Task PV_01_Submission_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CDR };
            var workManagerMock = new Mock<IWorkManager>();

            var workNumber = new WorkNumber() { Type = "71", Number = "00000000869155" };

            workManagerMock.Setup(w => w.FindAsync(It.IsAny<WorkNumber>()))
                .ReturnsAsync(new SubmissionModel() { WorkNumber = workNumber });

            var test = new Mock<PV_01>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._130));

            submission.Model.WorkNumber = workNumber;

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
        }
    }
}
