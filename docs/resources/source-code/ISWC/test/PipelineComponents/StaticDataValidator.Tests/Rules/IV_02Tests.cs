using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule IV_02
    /// </summary>
    public class IV_02Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if no IP provided
        /// </summary>
        [Fact]
        public async void IV_02_Submission_InValid()
        {
			var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("MustHaveOneIP")).ReturnsAsync(true);

			var test = new Mock<IV_02>(GetMessagingManagerMock(ErrorCode._104), rulesManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_02), test.Object.Identifier);
            Assert.Equal(ErrorCode._104, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if an IP is provided
        /// </summary>
        [Fact]
        public async void IV_02_Submission_Valid()
        {
			var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("MustHaveOneIP")).ReturnsAsync(true);

			var test = new Mock<IV_02>(GetMessagingManagerMock(ErrorCode._104), rulesManagerMock.Object);

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel(){Names = new List<NameModel>{ new NameModel { LastName = "name", IpNameNumber = 345435435 } }, IpBaseNumber ="basenumber",Type = InterestedPartyType.C}
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_02), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction passes if rule is false
        /// </summary>
        [Fact]
        public async void IV_02_Submission_ValidRule()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("MustHaveOneIP")).ReturnsAsync(false);

            var test = new Mock<IV_02>(GetMessagingManagerMock(ErrorCode._104), rulesManagerMock.Object);

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel(){Names = new List<NameModel>{ new NameModel { LastName = "name", IpNameNumber = 345435435 } }, IpBaseNumber ="basenumber",Type = InterestedPartyType.C}
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_02), test.Object.Identifier);
        }
    }
}
