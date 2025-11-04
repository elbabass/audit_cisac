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
    /// Tests rule MD_11
    /// </summary>
    public class MD_11Tests : TestBase
    {
        /// <summary>
        /// Check that the letter 'S' has been dropped from words ending in 'S' when the title has been standardised
        /// </summary>
        [Fact]
        public async Task MD_11_TitleToBeStandardised()
        {
			var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("StandardizeTitleENPlurals")).ReturnsAsync(true);

			var test = new Mock<MD_11>(rulesManagerMock.Object).Object;


            submission.Model.Titles = new List<Title>()
            {
                new Title(){ Name = "Books looks truckS ducks", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "Sss S has is waS glass", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "!!s!^as S!!", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "!!s!^as S!!", StandardizedName = "BOOKs", Type=TitleType.OT}

            };

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_11), test.Identifier);
            Assert.Equal("Book look truck duck", response.Submission.Model.Titles.ElementAt(0).StandardizedName);
            Assert.Equal("Ss  ha i wa glas", response.Submission.Model.Titles.ElementAt(1).StandardizedName);
            Assert.Equal("!!!^a !!", response.Submission.Model.Titles.ElementAt(2).StandardizedName);
            Assert.Equal("BOOK", response.Submission.Model.Titles.ElementAt(3).StandardizedName);
        }

        /// <summary>
        /// Check that the letter 'S' has not been dropped from words ending in 'S' when the title has not been standardised
        /// </summary>
        [Fact]
        public async Task MD_11_TitleNotToBeStandardised()
        {
			var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("StandardizeTitleENPlurals")).ReturnsAsync(true);

			var test = new Mock<MD_11>(rulesManagerMock.Object).Object;


            submission.Model.Titles = new List<Title>()
            {
                new Title(){ Name = "Shock sock surprise", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "%^&%^^(^%&%&$|||???", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "", StandardizedName = "", Type=TitleType.OT}
            };

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_11), test.Identifier);
            Assert.Equal("Shock sock surprise", response.Submission.Model.Titles.ElementAt(0).StandardizedName);
            Assert.Equal("%^&%^^(^%&%&$|||???", response.Submission.Model.Titles.ElementAt(1).StandardizedName);
            Assert.Equal("", response.Submission.Model.Titles.ElementAt(2).StandardizedName);
        }
    }
}
