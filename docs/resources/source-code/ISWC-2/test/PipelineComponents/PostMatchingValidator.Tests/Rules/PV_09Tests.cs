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
    /// Tests for rule PV_09
    /// </summary>
    public class PV_09Tests : TestBase
	{
        /// <summary>
        /// Check transaction passes if work code is used to identify Iswc
        /// </summary>
        [Fact]
		public async Task PV_09_FindWithWorkCode_Valid()
		{
			var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.MER };
			var workManagerMock = new Mock<IWorkManager>();


            workManagerMock.Setup(w => w.FindAsync(It.IsAny<string>(), false, false))
                .ReturnsAsync(new SubmissionModel() { Iswc = "T2030000042", IsReplaced = false });

            workManagerMock.Setup(w => w.FindAsync(It.IsAny<WorkNumber>()))
				.ReturnsAsync(new SubmissionModel() { Iswc = "T2030000042", IsReplaced = false });

			var test = new Mock<PV_09>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._132)).Object;

			submission.Model.IswcsToMerge = new List<string>() { "T2030000042", "T203000043" };
			submission.Model.WorkNumbersToMerge = new List<WorkNumber>() { new WorkNumber() { Type = "4" , Number = "T0370006431" } };

			var response = await test.IsValid(submission);

			Assert.True(response.IsValid);
			Assert.Equal(nameof(PV_09), test.Identifier);

		}

        /// <summary>
        /// Check transaction passes if PreferredIswc is used to identify Iswc
        /// </summary>
        [Fact]
		public async Task PV_09_FindWithPreferredIswc_Valid()
		{
			var submission = new Submission() { Model = new SubmissionModel() { PreferredIswc= "T2030000042" }, TransactionType = TransactionType.MER };
			var workManagerMock = new Mock<IWorkManager>();

			workManagerMock.Setup(w => w.FindAsync("T2030000042", false, false))
				.ReturnsAsync(new SubmissionModel() { Iswc = "T2030000042", IsReplaced = false });
			workManagerMock.Setup(w => w.FindAsync("T203000043", false, false))
				.ReturnsAsync(new SubmissionModel() { Iswc = "T2030000043", IsReplaced = false });

			var test = new Mock<PV_09>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._132)).Object;

			submission.Model.IswcsToMerge = new List<string>() { "T2030000042", "T203000043" };

			var response = await test.IsValid(submission);

			Assert.True(response.IsValid);
			Assert.Equal(nameof(PV_09), test.Identifier);
		}

        /// <summary>
        /// Check transaction fails if ISWC has been deleted
        /// </summary>
        [Fact]
		public async Task PV_09_IswcIsDeleted_InValid()
		{
			var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.MER };
			var workManagerMock = new Mock<IWorkManager>();

			workManagerMock.Setup(i => i.FindAsync("T2030000042", false, false)).ReturnsAsync(new SubmissionModel() { Iswc = "T2030000053", IsReplaced = true });
			workManagerMock.Setup(i => i.FindAsync("T2030000043", false, false)).ReturnsAsync(new SubmissionModel() { Iswc = "T2030000053", IsReplaced = false });

			var test = new Mock<PV_09>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._132)).Object;

			submission.Model.IswcsToMerge = new List<string>() { "T2030000042", "T203000043" };

			var response = await test.IsValid(submission);

			Assert.False(response.IsValid);
			Assert.Equal(nameof(PV_09), test.Identifier);
			Assert.Equal(ErrorCode._132, response.Submission.Rejection.Code);
		}

        /// <summary>
        /// Check transaction fails if workinfo record does not exist
        /// </summary>
        [Fact]
		public async Task PV_09_WorkDoesNotExist_InValid()
		{
			var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.MER };
			var workManagerMock = new Mock<IWorkManager>();

			workManagerMock.Setup(w => w.FindAsync(It.IsAny<WorkNumber>())).ReturnsAsync(default(SubmissionModel));

			var test = new Mock<PV_09>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._132)).Object;

			submission.Model.WorkNumbersToMerge = new List<WorkNumber>() {
				new WorkNumber() {
					Type =  "4" ,
					Number = "T0370006431" },
				new WorkNumber() };

			var response = await test.IsValid(submission);

			Assert.False(response.IsValid);
			Assert.Equal(nameof(PV_09), test.Identifier);
			Assert.Equal(ErrorCode._132, response.Submission.Rejection.Code);
		}

        /// <summary>
        /// Check transaction fails if ISWC does not exist
        /// </summary>
        [Fact]
		public async Task PV_09_IswcDoesNotExist_InValid()
		{
			var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.MER };
			var workManagerMock = new Mock<IWorkManager>();

			workManagerMock.Setup(w => w.FindAsync(It.IsAny<WorkNumber>()))
				.ReturnsAsync(new SubmissionModel() { Iswc = "T2030000042" });

			var test = new Mock<PV_09>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._132)).Object;

			submission.Model.IswcsToMerge = new List<string>() { "T2030000042", "T203000043" };

			var response = await test.IsValid(submission);

			Assert.False(response.IsValid);
			Assert.Equal(nameof(PV_09), test.Identifier);
			Assert.Equal(ErrorCode._132, response.Submission.Rejection.Code);
		}

		/// <summary>
		/// Check transaction passes if transaction type is DMR
		/// </summary>
		[Fact]
		public async Task PV_09_DMR_Valid()
		{
			var submission = new Submission() { Model = new SubmissionModel() { PreferredIswc = "T2030000042" }, TransactionType = TransactionType.DMR };
			var workManagerMock = new Mock<IWorkManager>();

			workManagerMock.Setup(w => w.FindAsync("T2030000042", false, false))
				.ReturnsAsync(new SubmissionModel() { Iswc = "T2030000042", IsReplaced = false });

			var test = new Mock<PV_09>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._132)).Object;

			var response = await test.IsValid(submission);

			Assert.True(response.IsValid);
			Assert.Equal(nameof(PV_09), test.Identifier);
		}

		/// <summary>
		/// Check transaction fails if transaction type is DMR and iswc is null
		/// </summary>
		[Fact]
		public async Task PV_09_DMR_InValid()
		{
			var submission = new Submission() { Model = new SubmissionModel() { PreferredIswc = "T2030000042" }, TransactionType = TransactionType.DMR };
			var workManagerMock = new Mock<IWorkManager>();

			var test = new Mock<PV_09>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._153)).Object;

			submission.Model.IswcsToMerge = new List<string>() { "T2030000042", "T203000043" };

			var response = await test.IsValid(submission);

			Assert.False(response.IsValid);
			Assert.Equal(nameof(PV_09), test.Identifier);
			Assert.Equal(ErrorCode._153, response.Submission.Rejection.Code);
		}

		/// <summary>
		/// Check transaction fails if isReplaced is set to true in DB for iswc
		/// </summary>
		[Fact]
		public async Task PV_09_IsReplaced_InValid()
		{
			var submission = new Submission() { Model = new SubmissionModel() { PreferredIswc = "T2030000042" }, TransactionType = TransactionType.MER };
			var workManagerMock = new Mock<IWorkManager>();

			workManagerMock.Setup(w => w.FindAsync("T2030000042", false, false))
				.ReturnsAsync(new SubmissionModel() { Iswc = "T2030000042", IsReplaced = true });
			workManagerMock.Setup(w => w.FindAsync("T203000043", false, false))
				.ReturnsAsync(new SubmissionModel() { Iswc = "T2030000043", IsReplaced = false });

			var test = new Mock<PV_09>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._132)).Object;

			submission.Model.IswcsToMerge = new List<string>() { "T2030000042", "T203000043" };

			var response = await test.IsValid(submission);

			Assert.False(response.IsValid);
			Assert.Equal(nameof(PV_09), test.Identifier);
			Assert.Equal(ErrorCode._132, response.Submission.Rejection.Code);
		}

		/// <summary>
		/// Check transaction fails if work code IsReplaced is set to true
		/// </summary>
		[Fact]
		public async Task PV_09_FindWithWorkCode_InValid()
		{
			var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.MER };
			var workManagerMock = new Mock<IWorkManager>();


			workManagerMock.Setup(w => w.FindAsync(It.IsAny<string>(), false, false))
				.ReturnsAsync(new SubmissionModel() { Iswc = "T2030000042", IsReplaced = false });

			workManagerMock.Setup(w => w.FindAsync(It.IsAny<WorkNumber>()))
				.ReturnsAsync(new SubmissionModel() { Iswc = "T2030000042", IsReplaced = true });

			var test = new Mock<PV_09>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._132)).Object;

			submission.Model.IswcsToMerge = new List<string>() { "T2030000042", "T203000043" };
			submission.Model.WorkNumbersToMerge = new List<WorkNumber>() { new WorkNumber() { Type = "4", Number = "T0370006431"} };

			var response = await test.IsValid(submission);

			Assert.False(response.IsValid);
			Assert.Equal(nameof(PV_09), test.Identifier);
			Assert.Equal(ErrorCode._132, response.Submission.Rejection.Code);

		}
	}
}
