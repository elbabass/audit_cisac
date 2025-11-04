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
    /// Tests rule MD_09
    /// </summary>
    public class MD_09Tests : TestBase
    {
        /// <summary>
        /// Check that title's spelling was standardised
        /// </summary>
        [Fact]
        public async Task MD_09_TitleToBeStandardised()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var standardizedTitleManagerMock = new Mock<IStandardizedTitleManager>();
            var rulesManagerMock = new Mock<IRulesManager>();

            standardizedTitleManagerMock.Setup(a => a.FindManyAsync(new List<string>() { "AESTHETICS" }))
                .ReturnsAsync(new List<Data.DataModels.StandardizedTitle>() {
                    new Data.DataModels.StandardizedTitle() {StandardizedTitleId = 1, Society = "-1", SearchPattern = "ESTHETICS", ReplacePattern = "AESTHETICS" } });

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("StandardizeTitleWordSpelling")).ReturnsAsync(true);
            var test = new Mock<MD_09>(standardizedTitleManagerMock.Object, rulesManagerMock.Object).Object;


            submission.Model.Titles = new List<Title>()
            {
                new Title(){ Name = "AESTHETICS", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "AESThETICS", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "!AESTHETICS ", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "aesthetics", StandardizedName = "", Type=TitleType.OT},
            };

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_09), test.Identifier);
            Assert.Equal("ESTHETICS", response.Submission.Model.Titles.ElementAt(0).StandardizedName);
            Assert.Equal("ESTHETICS", response.Submission.Model.Titles.ElementAt(1).StandardizedName);
            Assert.Equal("!ESTHETICS ", response.Submission.Model.Titles.ElementAt(2).StandardizedName);
            Assert.Equal("ESTHETICS", response.Submission.Model.Titles.ElementAt(3).StandardizedName);
        }

        /// <summary>
        /// Check that title's spelling was standardised (where many words need to be standardised)
        /// </summary>
        [Fact]
        public async Task MD_09_TitleManyWordsToBeStandardised()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var standardizedTitleManagerMock = new Mock<IStandardizedTitleManager>();
            var rulesManagerMock = new Mock<IRulesManager>();

            standardizedTitleManagerMock.Setup(a => a.FindManyAsync(new List<string>() { "AESTHETICS", "ALUMINIUM" }))
                .ReturnsAsync(new List<Data.DataModels.StandardizedTitle>() {
                    new Data.DataModels.StandardizedTitle() {StandardizedTitleId = 1, Society = "-1", SearchPattern = "ESTHETICS", ReplacePattern = "AESTHETICS" },
                    new Data.DataModels.StandardizedTitle() {StandardizedTitleId = 4, Society = "-1", SearchPattern = "ALUMINUM", ReplacePattern = "ALUMINIUM" }
                });
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("StandardizeTitleWordSpelling")).ReturnsAsync(true);

            var test = new Mock<MD_09>(standardizedTitleManagerMock.Object, rulesManagerMock.Object).Object;


            submission.Model.Titles = new List<Title>()
            {
                new Title(){ Name = "aesthetics ALUMINIUM", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "aesthetics aesthetics ALUMINIUM ALUMINIUM", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "!AESTHETICS, ALUMINIUM; AESTHETICS/ALUMINIUM", StandardizedName = "", Type=TitleType.OT}
            };

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_09), test.Identifier);
            Assert.Equal("ESTHETICS ALUMINUM", response.Submission.Model.Titles.ElementAt(0).StandardizedName);
            Assert.Equal("ESTHETICS ESTHETICS ALUMINUM ALUMINUM", response.Submission.Model.Titles.ElementAt(1).StandardizedName);
            Assert.Equal("!ESTHETICS, ALUMINUM; ESTHETICS/ALUMINUM", response.Submission.Model.Titles.ElementAt(2).StandardizedName);
        }

        /// <summary>
        /// Check that title's spelling was not standardised
        /// </summary>
        [Fact]
        public async Task MD_09_TitleNotToBeStandardised()
        {
            var submission = new Submission() { Model = new SubmissionModel() };

            var standardizedTitleManagerMock = new Mock<IStandardizedTitleManager>();
            var rulesManagerMock = new Mock<IRulesManager>();

            standardizedTitleManagerMock.Setup(a => a.FindManyAsync(new List<string>() { }));
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("StandardizeTitleWordSpelling")).ReturnsAsync(true);

            var test = new Mock<MD_09>(standardizedTitleManagerMock.Object, rulesManagerMock.Object).Object;

            submission.Model.Titles = new List<Title>()
            {
                new Title(){ Name = "Test string 123", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "!£$£^$%^&%^^(^%&%&$|||???", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "", StandardizedName = "", Type=TitleType.OT}
            };

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_09), test.Identifier);
            Assert.Equal("Test string 123", response.Submission.Model.Titles.ElementAt(0).StandardizedName);
            Assert.Equal("!£$£^$%^&%^^(^%&%&$|||???", response.Submission.Model.Titles.ElementAt(1).StandardizedName);
            Assert.Equal("", response.Submission.Model.Titles.ElementAt(2).StandardizedName);
        }

        /// <summary>
        /// Check that submission is true when Parameter value is false
        /// </summary>
        [Fact]
        public async Task MD_09_SetToFalse()
        {
            var submission = new Submission() { Model = new SubmissionModel() };

            var standardizedTitleManagerMock = new Mock<IStandardizedTitleManager>();
            var rulesManagerMock = new Mock<IRulesManager>();

            standardizedTitleManagerMock.Setup(a => a.FindManyAsync(new List<string>() { }));
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("StandardizeTitleWordSpelling")).ReturnsAsync(false);

            var test = new Mock<MD_09>(standardizedTitleManagerMock.Object, rulesManagerMock.Object).Object;

            submission.Model.Titles = new List<Title>()
            {
                new Title(){ Name = "Test string 123", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "!£$£^$%^&%^^(^%&%&$|||???", StandardizedName = "", Type=TitleType.OT},
                new Title(){ Name = "", StandardizedName = "", Type=TitleType.OT}
            };

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_09), test.Identifier);
        }
    }
}
