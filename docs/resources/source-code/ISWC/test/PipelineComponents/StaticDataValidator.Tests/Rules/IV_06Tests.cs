using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule IV_06
    /// </summary>
    public class IV_06Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if more than one OT title exists in the work submission
        /// </summary>
        [Fact]
        public async void IV_06_Submission_InValid()
        {
			var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("OnlyOneOriginalTitle")).ReturnsAsync(true);

			var test = new Mock<IV_06>(GetMessagingManagerMock(ErrorCode._110), rulesManagerMock.Object);

            submission.Model.Titles = new List<Title> {
                new Title() { Name = "Test Title", Type = TitleType.OT },
                new Title() { Name = "Test Title", Type = TitleType.AT },
                new Title() { Name = "Test Title", Type = TitleType.OT }
            };

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_06), test.Object.Identifier);
            Assert.Equal(ErrorCode._110, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if only one OT title exists in the work submission
        /// </summary>
        [Fact]
        public async void IV_06_Submission_Valid()
        {
			var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("OnlyOneOriginalTitle")).ReturnsAsync(true);

			var test = new Mock<IV_06>(GetMessagingManagerMock(ErrorCode._110), rulesManagerMock.Object);

            submission.Model.Titles = new List<Title> {
                new Title() { Name = "Test Title", Type = TitleType.OT },
                new Title() { Name = "Test Title", Type = TitleType.AT }
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_06), test.Object.Identifier);
        }
    }
}
