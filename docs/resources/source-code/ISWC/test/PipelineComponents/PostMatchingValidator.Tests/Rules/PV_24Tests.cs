using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using Xunit;
using Moq;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Framework.Tests;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Tests.Rules
{
    /// <summary>
    /// Tests for rule PV_24
    /// </summary>
    public class PV_24Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if PreferredIswc does not exist in the db
        /// </summary>
        [Fact]
        public async void PV_24_PreferredIswcDoesNotExist_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CAR };
            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(i => i.Exists("10")).ReturnsAsync(false);

            var test = new Mock<PV_24>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._117)).Object;

            submission.Model.PreferredIswc = "10";

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_24), test.Identifier);
            Assert.Equal(ErrorCode._117, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if PreferredIswc does not match archived Iswc
        /// </summary>
        [Fact]
        public async void PV_24_PreferredIswcDoesNotMatchArchived_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CAR };
            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(i => i.Exists("T2030000031")).ReturnsAsync(true);
            workManagerMock.Setup(i => i.Exists("T2030000035")).ReturnsAsync(true);

            var test = new Mock<PV_24>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._117)).Object;

            submission.Model.PreferredIswc = "T2030000031";
            submission.Model.Iswc = "T2030000035";

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_24), test.Identifier);
            Assert.Equal(ErrorCode._117, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if PreferredIswc exists
        /// </summary>
        [Fact]
        public async void PV_24_PreferredIswcDoesExist_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CAR };
            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(i => i.Exists("T2030000031")).ReturnsAsync(true);

            var test = new Mock<PV_24>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._117)).Object;

            submission.Model.PreferredIswc = "T2030000031";

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_24), test.Identifier);
        }

        /// <summary>
        /// Check transaction fails if AllowProvidedIswc is true, and the provided Iswc already exists
        /// </summary>
        [Fact]
        public async void PV_24_LocallyAssignedIswcDoesExist_Invalid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CAR };
            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(i => i.Exists("T2030000031")).ReturnsAsync(true);

            var test = new Mock<PV_24>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._169)).Object;

            submission.Model.Iswc = "T2030000031";
            submission.Model.AllowProvidedIswc = true;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_24), test.Identifier);
            Assert.Equal(ErrorCode._169, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if AllowProvidedIswc is true, and the provided Iswc does not exist.
        /// </summary>
        [Fact]
        public async void PV_24_LocallyAssignedIswcDoesNotExist_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CAR };
            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(i => i.Exists("T2030000031")).ReturnsAsync(false);

            var test = new Mock<PV_24>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._117)).Object;

            submission.Model.Iswc = "T2030000031";
            submission.Model.AllowProvidedIswc = true;

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_24), test.Identifier);
        }
    }
}
