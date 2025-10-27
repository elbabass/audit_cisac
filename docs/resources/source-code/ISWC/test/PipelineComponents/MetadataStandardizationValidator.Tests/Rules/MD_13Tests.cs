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
    /// Tests rule MD_13
    /// </summary>
    public class MD_13Tests : TestBase
    {
        /// <summary>
        /// Check that title characters IZE, YZE and PART were converted to ISE, YSE and PT when the title was standardised
        /// </summary>
        [Fact]
        public async void MD_13_TitleToBeStandardised()
        {
			var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("StandardizeTitleZ")).ReturnsAsync(true);

			var test = new Mock<MD_13>(rulesManagerMock.Object).Object;

            submission.Model.Titles = new List<Title>()
            {
                new Title(){ Name = "ize IzE IZE IZEABC !iZE", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "PART PARtabc ParT", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "Part 2 Analyze! £$% REALIZE mobilIZe", StandardizedName = "", Type=TitleType.OT}
            };

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_13), test.Identifier);
            Assert.Equal("ise IsE ISE ISEABC !iSE", response.Submission.Model.Titles.ElementAt(0).StandardizedName);
            Assert.Equal("PT PTabc PT", response.Submission.Model.Titles.ElementAt(1).StandardizedName);
            Assert.Equal("Pt 2 Analyse! £$% REALISE mobilISe", response.Submission.Model.Titles.ElementAt(2).StandardizedName);
        }

        /// <summary>
        /// Check that title characters IZE, YZE and PART were not converted to ISE, YSE and PT where the title was not standardised
        /// </summary>
        [Fact]
        public async void MD_13_TitleNotToBeStandardised()
        {
			var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("StandardizeTitleZ")).ReturnsAsync(true);

			var test = new Mock<MD_13>(rulesManagerMock.Object).Object;

            submission.Model.Titles = new List<Title>()
            {
                new Title(){ Name = "Test string 123", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "!£$£^$%^&%^^(^%&%&$|||???", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "", StandardizedName = "", Type=TitleType.OT}
            };

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_13), test.Identifier);
            Assert.Equal("Test string 123", response.Submission.Model.Titles.ElementAt(0).StandardizedName);
            Assert.Equal("!£$£^$%^&%^^(^%&%&$|||???", response.Submission.Model.Titles.ElementAt(1).StandardizedName);
            Assert.Equal("", response.Submission.Model.Titles.ElementAt(2).StandardizedName);
        }
    }
}
