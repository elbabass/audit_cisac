using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Tests.Rules
{
    /// <summary>
    /// Tests for rule PV_25
    /// </summary>
    public class PV_25Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if Society work code does not exist in system
        /// </summary>
        [Fact]
        public async void PV_25_InvalidWorkNumberProvided_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CDR };
            var workManagerMock = new Mock<IWorkManager>().Object;

            var test = new Mock<PV_25>(workManagerMock, GetMessagingManagerMock(ErrorCode._103)).Object;

            submission.Model.WorkNumber = new WorkNumber() { Type = "", Number = "" };

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._103, response.Submission.Rejection.Code);
            Assert.Equal(nameof(PV_25), test.Identifier);
        }

        /// <summary>
        /// Check transaction fails if Society work code exists but there is no matching work info record
        /// </summary>
        [Fact]
        public async void PV_25_WorkInfoDoesNotExist_WorkNumber_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CDR };
            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(w => w.FindAsync(It.IsAny<WorkNumber>())).ReturnsAsync(default(SubmissionModel));

            var test = new Mock<PV_25>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._143)).Object;

            submission.Model.WorkNumber = new WorkNumber() { Type = "10", Number = "00000032" };

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
        }

        /// <summary>
        /// Check transaction fails if Preferred ISWC does not exist in system
        /// </summary>
        [Fact]
        public async void PV_25_WorkInfoDoesNotExist_PreferredIswc_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CDR };
            var workManagerMock = new Mock<IWorkManager>();

            var workNumber = new WorkNumber() { Type = "10", Number = "00000032" };

            workManagerMock.Setup(w => w.FindVerifiedAsync("1111", It.IsAny<string>())).ReturnsAsync(default(VerifiedSubmissionModel));
            workManagerMock.Setup(w => w.FindVerifiedAsync(It.IsAny<WorkNumber>())).ReturnsAsync(new VerifiedSubmissionModel() { WorkNumber = workNumber, Iswc = "1111" });

            var test = new Mock<PV_25>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._117)).Object;

            submission.Model.PreferredIswc = "1111";
            submission.Model.WorkNumber = workNumber;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._117, response.Submission.Rejection.Code);
        }


        /// <summary>
        /// Check transaction fails if Preferred ISWC and Society Work Code exist in the system but the work info records do not match each other
        /// </summary>
        [Fact]
        public async void PV_25_WorkInfoDoesNotMatch_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CDR };
            var workManagerMock = new Mock<IWorkManager>();

            var workNumber = new WorkNumber() { Type = "10", Number = "00000032" };
            var workNumber2 = new WorkNumber() { Type = "11", Number = "00000035" };

            workManagerMock.Setup(w => w.FindVerifiedAsync("1111", It.IsAny<string>())).ReturnsAsync(new VerifiedSubmissionModel() { WorkNumber = workNumber2, Iswc = "1111" });
            workManagerMock.Setup(w => w.FindVerifiedAsync(It.IsAny<WorkNumber>())).ReturnsAsync(new VerifiedSubmissionModel() { WorkNumber = workNumber, Iswc = "1234" });

            var test = new Mock<PV_25>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._117)).Object;

            submission.Model.PreferredIswc = "1111";
            submission.Model.WorkNumber = workNumber;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._117, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if Preferred ISWC and Society Work Code exist in the system and the work info records match each other
        /// </summary>
        [Fact]
        public async void PV_25_WorkInfoDoesMatch_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() { Agency = "10" }, TransactionType = TransactionType.CDR };
            var workManagerMock = new Mock<IWorkManager>();

            var workNumber = new WorkNumber() { Type = "10", Number = "00000032" };

            workManagerMock.Setup(w => w.FindVerifiedAsync("1111", It.IsAny<string>())).ReturnsAsync(new VerifiedSubmissionModel() { WorkNumber = workNumber, Iswc = "1111" });
            workManagerMock.Setup(w => w.FindVerifiedAsync(It.IsAny<WorkNumber>())).ReturnsAsync(new VerifiedSubmissionModel() { WorkNumber = workNumber, Iswc = "1111" });

            var test = new Mock<PV_25>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._117)).Object;

            submission.Model.PreferredIswc = "1111";
            submission.Model.WorkNumber = workNumber;

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check transaction fails if work number is null
        /// </summary>
        [Fact]
        public async void PV_25_WorkInfoDoesMatch_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() { Agency = "10" }, TransactionType = TransactionType.CDR };
            var workManagerMock = new Mock<IWorkManager>();

            var workNumber = new WorkNumber() { Type = "10", Number = "00000032" };

            workManagerMock.Setup(w => w.FindVerifiedAsync("1111", It.IsAny<string>())).ReturnsAsync(new VerifiedSubmissionModel() { WorkNumber = workNumber, Iswc = "1111" });
            
            var test = new Mock<PV_25>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._143)).Object;

            submission.Model.PreferredIswc = "1111";
            submission.Model.WorkNumber = workNumber;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._143, response.Submission.Rejection.Code);
        }
    }
}
