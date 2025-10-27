using System.Collections.Generic;
using Xunit;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using Moq;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using System.Linq;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// IV/56 tests
    /// </summary>
    public class IV_56Tests
    {
        /// <summary>
        /// Check case when tab character is present in the name
        /// </summary>
        [Fact]
        public async void IV_56_TabChar()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };
            var test = new Mock<IV_56>(rulesManagerMock.Object);

            submission.Model.Titles = new List<Title>(){
                new Title() {Type = TitleType.OT, Name = "Test Title\t"},
            };

            var response = await test.Object.IsValid(submission);
            var expectedResult = new SubmissionModel
            {
                Titles = new List<Title>() {
                    new Title() { Type = TitleType.OT, Name = "Test Title" },
                }
            };

            Assert.Equal(expectedResult.Titles.First().Name, submission.Model.Titles.First().Name);
            Assert.Equal(nameof(IV_56), test.Object.Identifier);
            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check case when line feed character is present in the name
        /// </summary>
        [Fact]
        public async void IV_56_LineFeedChar()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };
            var test = new Mock<IV_56>(rulesManagerMock.Object);

            submission.Model.Titles = new List<Title>(){
                new Title() {Type = TitleType.OT, Name = "Test Title\n"},
            };

            var response = await test.Object.IsValid(submission);
            var expectedResult = new SubmissionModel
            {
                Titles = new List<Title>() {
                    new Title() { Type = TitleType.OT, Name = "Test Title" },
                }
            };

            Assert.Equal(expectedResult.Titles.First().Name, submission.Model.Titles.First().Name);
            Assert.Equal(nameof(IV_56), test.Object.Identifier);
            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check case when carraige return character is present in the name
        /// </summary>
        [Fact]
        public async void IV_56_CarraigeReturnChar()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };
            var test = new Mock<IV_56>(rulesManagerMock.Object);

            submission.Model.Titles = new List<Title>(){
                new Title() {Type = TitleType.OT, Name = "Test Title\r"},
            };

            var response = await test.Object.IsValid(submission);
            var expectedResult = new SubmissionModel
            {
                Titles = new List<Title>() {
                    new Title() { Type = TitleType.OT, Name = "Test Title" },
                }
            };

            Assert.Equal(expectedResult.Titles.First().Name, submission.Model.Titles.First().Name);
            Assert.Equal(nameof(IV_56), test.Object.Identifier);
            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check case when a string to replace is added using rules manager
        /// </summary>
        [Fact]
        public async void IV_56_AddedString()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };
            var test = new Mock<IV_56>(rulesManagerMock.Object);

            rulesManagerMock.Setup(r => r.GetParameterValueEnumerable<string>("StripCharactersFromTitles")).ReturnsAsync(new[] { "e", "l" });

            submission.Model.Titles = new List<Title>(){
                new Title() {Type = TitleType.OT, Name = "Test Title\r"},
            };

            var response = await test.Object.IsValid(submission);
            var expectedResult = new SubmissionModel
            {
                Titles = new List<Title>() {
                    new Title() { Type = TitleType.OT, Name = "Tst Tit" },
                }
            };

            Assert.Equal(expectedResult.Titles.First().Name, submission.Model.Titles.First().Name);
            Assert.Equal(nameof(IV_56), test.Object.Identifier);
            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Remove any special chars form Agency Work Code
        /// </summary>
        [Theory]
        [InlineData("TestWork\t")]
        [InlineData("TestWork\r")]
        [InlineData("TestWork\n")]
        [InlineData("TestWorkxyz")]
        public async void IV_56_RemoveSpecialCharsFromAgencyWorkcode(string testValue)
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission
            {
                Model = new SubmissionModel
                {
                    WorkNumber = new WorkNumber { Number = testValue, Type = "128" }
                }
            };

            var test = new Mock<IV_56>(rulesManagerMock.Object);
            rulesManagerMock.Setup(r => r.GetParameterValueEnumerable<string>("StripCharactersFromTitles")).ReturnsAsync(new[] { "x", "y", "z" });

            var response = await test.Object.IsValid(submission);

            Assert.Equal("TestWork", submission.Model.WorkNumber.Number);
            Assert.Equal(nameof(IV_56), test.Object.Identifier);
            Assert.True(response.IsValid);

        }

        /// <summary>
        /// Remove any special chars form Additional Agency Work Codes
        /// </summary>
        [Theory]
        [InlineData("TestWork\t")]
        [InlineData("TestWork\r")]
        [InlineData("TestWork\n")]
        [InlineData("TestWorkxyz")]
        public async void IV_56_RemoveSpecialCharsFromAAdditionalAgencyWorkcodes(string testValue)
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission
            {
                Model = new SubmissionModel
                {
                    WorkNumber = new WorkNumber { Number = "testwork", Type = "128" },
                    AdditionalAgencyWorkNumbers = new List<AdditionalAgencyWorkNumber>
                    {
                        new AdditionalAgencyWorkNumber { WorkNumber = new WorkNumber { Number = testValue , Type="22"} }
                    }
                }
            };

            var test = new Mock<IV_56>(rulesManagerMock.Object);
            rulesManagerMock.Setup(r => r.GetParameterValueEnumerable<string>("StripCharactersFromTitles")).ReturnsAsync(new[] { "x", "y", "z" });

            var response = await test.Object.IsValid(submission);

            Assert.Equal("TestWork", submission.Model.AdditionalAgencyWorkNumbers.FirstOrDefault().WorkNumber.Number);
            Assert.Equal(nameof(IV_56), test.Object.Identifier);
            Assert.True(response.IsValid);

        }
    }
}
