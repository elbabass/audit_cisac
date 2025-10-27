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
    /// Tests rule MD_10
    /// </summary>
    public class MD_10Tests : TestBase
    {
        /// <summary>
        /// Check that the letter 'G' was dropped in standardised titles for words that end in 'ING'
        /// </summary>
        [Fact]
        public async void MD_10_TitleToBeStandardised()
        {
			var rulesManagerMock = new Mock<IRulesManager>();
			var submission = new Submission() { Model = new SubmissionModel() };

			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("StandardizeTitleWordEnding")).ReturnsAsync(true);

			var test = new Mock<MD_10>(rulesManagerMock.Object).Object;

			submission.Model.Titles = new List<Title>()
            {
                new Title(){ Name = "Wheeling stealiNg dealINg thinG beING", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "looking!singing!£££Az ing MIND(ING)", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "ING ming dinger wing", StandardizedName = "", Type=TitleType.OT}
            };

			var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_10), test.Identifier);
            Assert.Equal("Wheelin stealiN dealIN thin beIN", response.Submission.Model.Titles.ElementAt(0).StandardizedName);
            Assert.Equal("lookin!singin!£££Az ing MIND(ING)", response.Submission.Model.Titles.ElementAt(1).StandardizedName);
            Assert.Equal("ING min dinger win", response.Submission.Model.Titles.ElementAt(2).StandardizedName);
        }

        /// <summary>
        /// When the title word endings should not be standardised, check that the letter 'G' was not dropped for words which end in 'ING'
        /// </summary>
        [Fact]
        public async void MD_10_TitleNotToBeStandardised()
        {
			var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("StandardizeTitleWordEnding")).ReturnsAsync(true);

			var test = new Mock<MD_10>(rulesManagerMock.Object).Object;


            submission.Model.Titles = new List<Title>()
            {
                new Title(){ Name = "Wings Shingles dog", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "!£$£^$%^&%^^(^%&%&$|||???", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "", StandardizedName = "", Type=TitleType.OT}
            };

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_10), test.Identifier);
            Assert.Equal("Wings Shingles dog", response.Submission.Model.Titles.ElementAt(0).StandardizedName);
            Assert.Equal("!£$£^$%^&%^^(^%&%&$|||???", response.Submission.Model.Titles.ElementAt(1).StandardizedName);
            Assert.Equal("", response.Submission.Model.Titles.ElementAt(2).StandardizedName);
        }
    }
}
