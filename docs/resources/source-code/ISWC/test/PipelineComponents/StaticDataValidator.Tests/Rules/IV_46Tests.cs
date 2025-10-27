using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using System.Collections.Generic;
using Xunit;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Bdo.Work;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule IV_46
    /// </summary>
    public class IV_46Tests : TestBase
    {

        /// <summary>
        /// Check transaction passes
        /// </summary>
        [Fact]
        public async void IV_46_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var workManager = new Mock<IWorkManager>();
            submission.Model.WorkNumber.Number = "31242142";
            submission.Model.PreferredIswc = "T203000239";
            submission.TransactionType = Bdo.Edi.TransactionType.CUR;
            submission.ExistingWork = new SubmissionModel();


            workManager.Setup(x => x.FindWorkForValidationAsync(new Bdo.Work.WorkNumber())).ReturnsAsync(new SubmissionModel());
            var test = new Mock<IV_46>(GetMessagingManagerMock(ErrorCode._159), workManager.Object);

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_46), test.Object.Identifier);
            workManager.Verify(x => x.FindWorkForValidationAsync(It.IsAny<WorkNumber>()), Times.Once);
        }

        /// <summary>
        /// Check transaction passes if CUR has no preferred iswc
        /// </summary>
        [Fact]
        public async void IV_46_CUR_No_Preferred_ISWC_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var workManager = new Mock<IWorkManager>();
            submission.Model.WorkNumber.Number = "31242142";
            submission.Model.PreferredIswc = "";
            submission.TransactionType = Bdo.Edi.TransactionType.CUR;


            workManager.Setup(x => x.FindWorkForValidationAsync(new Bdo.Work.WorkNumber())).ReturnsAsync(new SubmissionModel());

            var test = new Mock<IV_46>(GetMessagingManagerMock(ErrorCode._159), workManager.Object);

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_46), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction passes while setting existing work preferred iswc to empty
        /// </summary>
        [Fact]
        public async void IV_46_Valid_all_conditions_met()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var workManager = new Mock<IWorkManager>();

            submission.Model.WorkNumber.Number = "31242142";
            submission.Model.WorkNumber.Type = "CA";
            submission.Model.PreferredIswc = "T203000239";
            submission.TransactionType = Bdo.Edi.TransactionType.CUR;

            submission.ExistingWork = new SubmissionModel();
            submission.ExistingWork.Iswc = "T203000238";
            submission.ExistingWork.WorkNumber.Number = "31242142";
            submission.ExistingWork.WorkNumber.Type = "CA";
            submission.ExistingWork.PreferredIswc = "T203000236";


            workManager.Setup(x => x.FindWorkForValidationAsync(submission.Model.WorkNumber)).ReturnsAsync(submission.ExistingWork);
            var test = new Mock<IV_46>(GetMessagingManagerMock(ErrorCode._159), workManager.Object);

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_46), test.Object.Identifier);
            workManager.Verify(x => x.FindWorkForValidationAsync(It.IsAny<WorkNumber>()), Times.Once);
        }
    }
}
