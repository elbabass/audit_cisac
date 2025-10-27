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
    /// Tests rule IV_29
    /// </summary>
    public class IV_29Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if any of the submitted IPNameNumbers feature in the list of IPs to reject
        /// </summary>
        [Fact]
        public async void IV_29_Submission_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();

            rulesManagerMock.Setup(p => p.GetParameterValueEnumerable<string>("RejectIPs")).ReturnsAsync(new List<string>() { "I-000056650-5", "I-000168343-6" });
            interestedPartyManagerMock.Setup(v => v.FindManyByBaseNumber(new List<string>() { "I-000056650-5", "I-000168343-6" })).ReturnsAsync(new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "I-000056650-5", Names = new NameModel[] { new NameModel { IpNameNumber = 39753749 } } } });

            var test = new Mock<IV_29>(rulesManagerMock.Object, interestedPartyManagerMock.Object, GetMessagingManagerMock(ErrorCode._148));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel(){Names = new List<NameModel>{ new NameModel { LastName = "name", IpNameNumber = 39753749 } }, IpBaseNumber ="basenumber",Type = InterestedPartyType.C}
            };

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_29), test.Object.Identifier);
            Assert.Equal(ErrorCode._148, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if none of the submitted IPNameNumbers feature in the list of IPs to reject
        /// </summary>
        [Fact]
        public async void IV_29_Submission_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();

            rulesManagerMock.Setup(p => p.GetParameterValueEnumerable<string>("RejectIPs")).ReturnsAsync(new List<string>() { "I-000056650-5", "I-000168343-6" });
            interestedPartyManagerMock.Setup(v => v.FindManyByBaseNumber(new List<string>() { "I-000056650-5", "I-000168343-6" })).ReturnsAsync(new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "I-000056650-5", Names = new NameModel[] { new NameModel { IpNameNumber = 57763746 } } } });

            var test = new Mock<IV_29>(rulesManagerMock.Object, interestedPartyManagerMock.Object, GetMessagingManagerMock(ErrorCode._148));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel(){Names = new List<NameModel>{ new NameModel { LastName = "name", IpNameNumber = 17763740 } }, IpBaseNumber ="basenumber",Type = InterestedPartyType.C}
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_29), test.Object.Identifier);
        }
    }
}
