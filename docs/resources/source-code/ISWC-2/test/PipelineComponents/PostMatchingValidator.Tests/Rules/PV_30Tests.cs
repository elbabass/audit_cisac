using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Tests.Rules
{
    /// <summary>
    /// Tests for rule PV_30
    /// </summary>
    public class PV_30Tests: TestBase
	{
        /// <summary>
        /// Check transaction passes if no DisambiguateFrom Iswcs are provided and Disambiguation is False
        /// </summary>
        [Fact]
		public async Task PV_30_NoDisambiguateFromIswcsProvided_Valid()
		{
			var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CAR };
			var workManager = new Mock<IWorkManager>().Object;

			var test = new Mock<PV_30>(workManager, GetMessagingManagerMock(ErrorCode._128)).Object;

			submission.Model.DisambiguateFrom = new List<DisambiguateFrom> { };
            submission.Model.Disambiguation = false;

            var response = await test.IsValid(submission);

			Assert.Equal(nameof(PV_30), test.Identifier);
			Assert.True(response.IsValid);
		}

        /// <summary>
        /// Check transaction fails if DisambiguateFrom Iswc is null
        /// </summary>
        [Fact]
		public async Task PV_30_IswcIsNull_InValid()
		{
			var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CAR };
			var workManager = new Mock<IWorkManager>().Object;

			var test = new Mock<PV_30>(workManager, GetMessagingManagerMock(ErrorCode._128)).Object;

			submission.Model.DisambiguateFrom = new List<DisambiguateFrom> { new DisambiguateFrom() { Iswc = null} };
            submission.Model.Disambiguation = true;

			var response = await test.IsValid(submission);

			Assert.False(response.IsValid);
			Assert.Equal(ErrorCode._128, response.Submission.Rejection.Code);
		}

        /// <summary>
        /// Check transaction fails if DisambiguateFrom Iswc does not exist in db
        /// </summary>
        [Fact]
		public async Task PV_30_IswcDoesNotExist_InValid()
		{
			var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CAR };
			var workManager = new Mock<IWorkManager>();

            workManager.Setup(i => i.FindAsync("20", false, false)).ReturnsAsync(default(SubmissionModel));

			var test = new Mock<PV_30>(workManager.Object, GetMessagingManagerMock(ErrorCode._128)).Object;

			submission.Model.DisambiguateFrom = new List<DisambiguateFrom>() { new DisambiguateFrom { Iswc = "20"} };
            submission.Model.Disambiguation = true;
			var response = await test.IsValid(submission);

			Assert.False(response.IsValid);
			Assert.Equal(ErrorCode._128, response.Submission.Rejection.Code);
		}

        /// <summary>
        /// Check transaction fails if one DisambiguateFrom Iswc does not exist in db
        /// </summary>
		[Fact]
		public async Task PV_30_OneIswcDoesNotExist_InValid()
		{
			var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CAR };
			var workManager = new Mock<IWorkManager>();

            workManager.Setup(i => i.FindAsync("40", false, false)).ReturnsAsync(default(SubmissionModel));
            workManager.Setup(i => i.FindAsync("50", false, false)).ReturnsAsync(new SubmissionModel() { Iswc = "50" });

			var test = new Mock<PV_30>(workManager.Object, GetMessagingManagerMock(ErrorCode._128)).Object;

			submission.Model.DisambiguateFrom = new List<DisambiguateFrom>() {
				new DisambiguateFrom() { Iswc = "40" },
				new DisambiguateFrom() { Iswc = "50"} };
            submission.Model.Disambiguation = true;

			var response = await test.IsValid(submission);

			Assert.False(response.IsValid);
			Assert.Equal(ErrorCode._128, response.Submission.Rejection.Code);
		}

        /// <summary>
        /// Check transaction passes if all DisambiguateFrom Iswcs exist in db
        /// </summary>
		[Fact]
		public async Task PV_30_AllIswcsProvidedExist_Valid()
		{
			var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CAR };
			var workManager = new Mock<IWorkManager>();

            workManager.Setup(i => i.FindAsync("20", false, false)).ReturnsAsync(new SubmissionModel() { Iswc = "20" });
            workManager.Setup(i => i.FindAsync("30", false, false)).ReturnsAsync(new SubmissionModel() { Iswc = "30" });

			var test = new Mock<PV_30>(workManager.Object, GetMessagingManagerMock(ErrorCode._128)).Object;

			submission.Model.DisambiguateFrom = new List<DisambiguateFrom>() {
				new DisambiguateFrom() { Iswc = "20" },
				new DisambiguateFrom() { Iswc = "30"} };
            submission.Model.Disambiguation = true;

            var response = await test.IsValid(submission);

			Assert.True(response.IsValid);
		}
	}
}
