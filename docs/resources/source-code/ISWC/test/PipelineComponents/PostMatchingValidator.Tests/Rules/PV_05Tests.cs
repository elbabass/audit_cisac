using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule PV_05
    /// </summary>
    public class PV_05Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if TransactionType is CUR and workinfo record is deleted
        /// </summary>
        [Fact]
        public async void PV_05_Submission_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CUR };
            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(w => w.Exists(It.IsAny<WorkNumber>()))
                .ReturnsAsync(false);

            var test = new Mock<PV_05>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._131));

            submission.Model.WorkNumber = new WorkNumber() { Type = "004", Number = "T0370006431" };

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_05), test.Object.Identifier);
            Assert.Equal(ErrorCode._131, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if TransactionType is CUR and workinfo record is not deleted
        /// </summary>
        [Fact]
        public async void PV_05_Submission_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CUR };
            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(w => w.Exists(It.IsAny<WorkNumber>()))
                .ReturnsAsync(true);

            var test = new Mock<PV_05>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._131));

            submission.Model.WorkNumber = new WorkNumber() { Type = "71", Number = "00000000869155" };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_05), test.Object.Identifier);
        }
    }
}
