using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule IV_14
    /// </summary>
    public class IV_14Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if the Preferred ISWC is not in a valid format
        /// </summary>
        [Fact]
        public async void IV_14_Submission_PreferredIswc_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };

            var test = new Mock<IV_14>(GetMessagingManagerMock(ErrorCode._113));

            submission.Model.PreferredIswc = "T203000001";
            submission.Model.Iswc = "T2030000019";

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_14), test.Object.Identifier);
            Assert.Equal(ErrorCode._113, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if the Archived ISWC is not in a valid format
        /// </summary>
        [Fact]
        public async void IV_14_Submission_Iswc_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };

            var test = new Mock<IV_14>(GetMessagingManagerMock(ErrorCode._113));

            submission.Model.PreferredIswc = "T2030000019";
            submission.Model.Iswc = "T203000001";

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_14), test.Object.Identifier);
            Assert.Equal(ErrorCode._113, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if the Preferred ISWC and Archived ISWC are in a valid format
        /// </summary>
        [Fact]
        public async void IV_14_Submission_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };

            var test = new Mock<IV_14>(GetMessagingManagerMock(ErrorCode._113));

            submission.Model.PreferredIswc = "T2030000019";

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_14), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction passes if no Preferred ISWC provided by CUR
        /// </summary>
        [Fact]
        public async void IV_14_No_Preferred_ISWC_CUR_Submission_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = Bdo.Edi.TransactionType.CUR };

            var test = new Mock<IV_14>(GetMessagingManagerMock(ErrorCode._113));

            submission.Model.PreferredIswc = string.Empty;

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_14), test.Object.Identifier);
        }
    }
}
