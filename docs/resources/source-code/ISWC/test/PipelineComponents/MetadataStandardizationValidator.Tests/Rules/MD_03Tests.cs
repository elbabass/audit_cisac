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
    /// Tests rule MD_03
    /// </summary>
    public class MD_03Tests : TestBase
    {
        /// <summary>
        /// Checks that the title types listed in the rule's parameter values are not saved in core ISWC data
        /// </summary>
        [Fact]
        public async void MD_03_TitlesRemoved()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();

            rulesManagerMock.Setup(v => v.GetParameterValueEnumerable<TitleType>("ExcludeTitleTypes"))
                .ReturnsAsync(new List<TitleType>() { TitleType.PT, TitleType.CT });

            var test = new Mock<MD_03>(rulesManagerMock.Object);

            submission.Model.Titles = new List<Title>()
            {
                new Title(){ Name = "Hi Ji,m", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "This is a ,.,song' title!!!", StandardizedName = "", Type=TitleType.PT},
                new Title(){ Name = "12345 six seven eight*&#", StandardizedName = "", Type=TitleType.CT}
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_03), test.Object.Identifier);
            Assert.Single(submission.Model.Titles);

        }

        /// <summary>
        /// Checks all title types are saved in core ISWC data as rule's parameter values are empty
        /// </summary>
        [Fact]
        public async void MD_03_NoTitlesRemoved()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();

            rulesManagerMock.Setup(v => v.GetParameterValueEnumerable<string>("ExcludeTitleTypes"))
                .ReturnsAsync(new List<string>() { "PT", "CT" });

            var test = new Mock<MD_03>(rulesManagerMock.Object);

            submission.Model.Titles = new List<Title>()
            {
                new Title(){ Name = "Hi Ji,m", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "This is a ,.,song' title!!!", StandardizedName = "", Type=TitleType.AT},
                new Title(){ Name = "12345 six seven eight*&#", StandardizedName = "", Type=TitleType.OL}
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_03), test.Object.Identifier);
            Assert.Equal(3, submission.Model.Titles.Count());

        }
    }
}
