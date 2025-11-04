using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.MetadataStandardizationValidator.Rules;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MetadataStandardizationValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule MD_08
    /// </summary>
    public class MD_08Tests : TestBase
    {
        /// <summary>
        /// Check that numbers are converted to words and a space is inserted on either side of the number in standardised title
        /// </summary>
        [Fact]
        public async Task MD_08_ConvertNumbers()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
			var rulesManagerMock = new Mock<IRulesManager>();
			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ConvertENTitleNumbersToWords")).ReturnsAsync(true);
            var test = new Mock<MD_08>(rulesManagerMock.Object).Object;

            submission.Model.Titles = new List<Title>()
            {
                new Title(){ Name = "123", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "PART PAR5tabc", StandardizedName = "", Type=TitleType.AT},
                new Title(){ Name = "Part 2 Analyse!", StandardizedName = "", Type=TitleType.AL},
                new Title(){ Name = "Pa1t 2 An7lyse!098", StandardizedName = "", Type=TitleType.OA},
                new Title(){ Name = "123 456 78Hi9AuRa", StandardizedName = "", Type=TitleType.OA},
                new Title(){ Name = "one1two2three3", StandardizedName = "", Type=TitleType.OA},
                new Title(){ Name = "title", StandardizedName = "one1two2three3", Type=TitleType.OA}
            };

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_08), test.Identifier);
            Assert.Equal("ONE TWO THREE", response.Submission.Model.Titles.ElementAt(0).StandardizedName);
            Assert.Equal("PART PAR FIVE tabc", response.Submission.Model.Titles.ElementAt(1).StandardizedName);
            Assert.Equal("Part TWO Analyse!", response.Submission.Model.Titles.ElementAt(2).StandardizedName);
            Assert.Equal("Pa ONE t TWO An SEVEN lyse! ZERO NINE EIGHT", response.Submission.Model.Titles.ElementAt(3).StandardizedName);
            Assert.Equal("ONE TWO THREE FOUR FIVE SIX SEVEN EIGHT Hi NINE AuRa", response.Submission.Model.Titles.ElementAt(4).StandardizedName);
            Assert.Equal("one ONE two TWO three THREE", response.Submission.Model.Titles.ElementAt(5).StandardizedName);
            Assert.Equal("one ONE two TWO three THREE", response.Submission.Model.Titles.ElementAt(6).StandardizedName);
            Assert.Equal("title", response.Submission.Model.Titles.ElementAt(6).Name);
            Assert.Equal("PART PAR5tabc", response.Submission.Model.Titles.ElementAt(1).Name);
        }

        /// <summary>
        /// Check that submission is true when Parameter value is false
        /// </summary>
        [Fact]
        public async Task MD_08_SetToFalse()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ConvertENTitleNumbersToWords")).ReturnsAsync(false);
            var test = new Mock<MD_08>(rulesManagerMock.Object).Object;

            submission.Model.Titles = new List<Title>()
            {
                new Title(){ Name = "123", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "PART PAR5tabc", StandardizedName = "", Type=TitleType.AT},
                new Title(){ Name = "Part 2 Analyse!", StandardizedName = "", Type=TitleType.AL},
                new Title(){ Name = "Pa1t 2 An7lyse!098", StandardizedName = "", Type=TitleType.OA},
                new Title(){ Name = "123 456 78Hi9AuRa", StandardizedName = "", Type=TitleType.OA},
                new Title(){ Name = "one1two2three3", StandardizedName = "", Type=TitleType.OA},
                new Title(){ Name = "title", StandardizedName = "one1two2three3", Type=TitleType.OA}
            };

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_08), test.Identifier);
        }
    }
}