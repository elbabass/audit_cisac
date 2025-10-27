using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.MetadataStandardizationValidator.Rules;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MetadataStandardizationValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule MD_06
    /// </summary>
    public class MD_06Tests : TestBase
    {
        /// <summary>
        /// Check that special characters are removed from standardised title
        /// </summary>
        [Fact]
        public async void MD_06_SpecialCharactersRemoved()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("RemoveTitleCharacters")).ReturnsAsync("[^0-9a-zA-Z ]+");

            var test = new Mock<MD_06>(rulesManagerMock.Object);

            submission.Model.Titles = new List<Title>()
            {
                new Title(){ Name = "Hi Ji,m", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "This is a ,.,song' title!!!", StandardizedName = "", Type=TitleType.AT},
                new Title(){ Name = "12345 six seven eight*&#", StandardizedName = "", Type=TitleType.CT}
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_06), test.Object.Identifier);
            Assert.Equal("Hi Jim", response.Submission.Model.Titles.ElementAt(0).StandardizedName);
            Assert.Equal("This is a song title", response.Submission.Model.Titles.ElementAt(1).StandardizedName);
            Assert.Equal("12345 six seven eight", response.Submission.Model.Titles.ElementAt(2).StandardizedName);
            Assert.Equal("Hi Ji,m", response.Submission.Model.Titles.ElementAt(0).Name);
            Assert.Equal("This is a ,.,song' title!!!", response.Submission.Model.Titles.ElementAt(1).Name);
            Assert.Equal("12345 six seven eight*&#", response.Submission.Model.Titles.ElementAt(2).Name);
        }
    }
}
