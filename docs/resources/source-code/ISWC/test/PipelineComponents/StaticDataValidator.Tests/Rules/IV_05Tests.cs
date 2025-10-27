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
    /// Tests rule IV_05
    /// </summary>
    public class IV_05Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if no OT provided
        /// </summary>
        [Fact]
        public async void IV_05_Submission_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    Titles = new List<Title> {
                        new Title { Name = "title 1", Type = TitleType.ET },
                         new Title { Name = "title 2", Type = TitleType.IT },
                         new Title { Name = " ", Type = TitleType.OT }
                    }
                }
            };

            var test = new Mock<IV_05>(GetMessagingManagerMock(ErrorCode._109));

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_05), test.Object.Identifier);
            Assert.Equal(ErrorCode._109, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if OT provided
        /// </summary>
        [Fact]
        public async void IV_05_Submission_Valid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    Titles = new List<Title> {
                        new Title { Name = "title 1", Type = TitleType.OT },
                         new Title { Name = "title 2", Type = TitleType.AT }
                    }
                }
            };

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("MustHaveOneIP")).ReturnsAsync(true);

            var test = new Mock<IV_05>(GetMessagingManagerMock(ErrorCode._109));


            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_05), test.Object.Identifier);
        }

    }
}
