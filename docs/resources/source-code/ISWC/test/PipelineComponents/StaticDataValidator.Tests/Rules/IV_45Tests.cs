using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using Xunit;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using System.Collections.Generic;
using SpanishPoint.Azure.Iswc.Bdo.Work;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule IV_45
    /// </summary>
    public class IV_45Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if no Work Code is above 20 characters
        /// </summary>
        [Fact]
        public async void IV_45_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var test = new Mock<IV_45>(GetMessagingManagerMock(ErrorCode._158));

            submission.Model.WorkNumber.Number = "342543634643643434543";

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_45), test.Object.Identifier);
            Assert.Equal(ErrorCode._158, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if Work Code is below 20 characters
        /// </summary>
        [Fact]
        public async void IV_45_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var test = new Mock<IV_45>(GetMessagingManagerMock(ErrorCode._158));

            submission.Model.WorkNumber.Number = "31242142";

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_45), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction passes if all Additional Work Codes is below 20 characters
        /// </summary>
        [Fact]
        public async void IV_45_AdditionalWorkcodes_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var test = new Mock<IV_45>(GetMessagingManagerMock(ErrorCode._158));

            submission.Model.WorkNumber.Number = "31242142";
            submission.Model.AdditionalAgencyWorkNumbers = new List<AdditionalAgencyWorkNumber>
            {
                new AdditionalAgencyWorkNumber{ WorkNumber = new WorkNumber{Number = "dsfsfsfsdfdsf"}},
                new AdditionalAgencyWorkNumber{ WorkNumber = new WorkNumber{Number = "dsfsfsdfsfsfsfsfsdf"}}
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_45), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction fails if any Additional Work Codes is over 20 characters
        /// </summary>
        [Fact]
        public async void IV_45_AdditionalWorkcodes_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var test = new Mock<IV_45>(GetMessagingManagerMock(ErrorCode._158));

            submission.Model.WorkNumber.Number = "31242142";
            submission.Model.AdditionalAgencyWorkNumbers = new List<AdditionalAgencyWorkNumber>
            {
                new AdditionalAgencyWorkNumber{ WorkNumber = new WorkNumber{Number = "dsfsfsfsdfdsf"}},
                new AdditionalAgencyWorkNumber{ WorkNumber = new WorkNumber{Number = "342543634643643434543"}}
            };

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_45), test.Object.Identifier);
            Assert.Equal(ErrorCode._158, response.Submission.Rejection.Code);
        }
    }
}
