using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule PV_03
    /// </summary>
    public class PV_03Tests : TestBase
    {
        /// <summary>
        /// Check transaction passes validation if no work number provided
        /// </summary>
        [Fact]
        public async Task PV_03_NoMatchesToCheck_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CAR, SubmissionId = 1 };
            var workManagerMock = new Mock<IWorkManager>();
            var test = new Mock<PV_03>(workManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check transaction passes if work number exists in db
        /// </summary>
        [Fact]
        public async Task PV_03_WorkNumberAlreadyExistsInDb_Valid()
        {
            var testWorkNumber = new WorkNumber() { Number = "1" };
            var submission = new Submission()
            {
                Model = new SubmissionModel() { WorkNumber = testWorkNumber },
                TransactionType = TransactionType.CUR,
                ToBeProcessed = false,
                ExistingWork = new SubmissionModel() { WorkNumber = testWorkNumber, IsReplaced = true }
            };

            var workManagerMock = new Mock<IWorkManager>();
            var test = new Mock<PV_03>(workManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_03), test.Object.Identifier);
            Assert.Equal(TransactionType.CUR, response.Submission.TransactionType);
            Assert.False(response.Submission.ToBeProcessed);
        }

        /// <summary>
        /// Check transaction is converted to CAR transaction if work number does not exist in db
        /// </summary>
        [Fact]
        public async Task PV_03_WorkNumberDoesNotExistInDb_Valid()
        {
            var testWorkNumber = new WorkNumber() { Number = "1" };
            var submission = new Submission()
            {
                Model = new SubmissionModel() { WorkNumber = testWorkNumber },
                TransactionType = TransactionType.CUR,
                ToBeProcessed = false,
                ExistingWork = null
            };

            var workManagerMock = new Mock<IWorkManager>();
            var test = new Mock<PV_03>(workManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_03), test.Object.Identifier);
            Assert.Equal(TransactionType.CAR, response.Submission.TransactionType);
            Assert.True(response.Submission.ToBeProcessed);
        }
    }
}
