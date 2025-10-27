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
    /// Tests rule IV_25
    /// </summary>
    public class IV_25Tests : TestBase
    {
        /// <summary>
        /// Check that an IP is removed from submission if it belongs to a work submissions with Unknown Publisher, IPBaseNumber “I-001631070-4”
        /// </summary>
        [Fact]
        public async void IV_25_InterestedParty_Removed()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();

            rulesManagerMock.Setup(p => p.GetParameterValueEnumerable<string>("IgnoreIPsForMatchingAndEligibility")).ReturnsAsync(new List<string>() { "I-001631070-4" });
            interestedPartyManagerMock.Setup(v => v.FindManyByBaseNumber(new List<string>() { "I-001631070-4" })).ReturnsAsync(new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "I-001631070-4", Names = new NameModel[] { new NameModel { IpNameNumber = 216156786 }, new NameModel { IpNameNumber = 288936892 } } } });

            var test = new Mock<IV_25>(rulesManagerMock.Object, interestedPartyManagerMock.Object, GetMessagingManagerMock(ErrorCode._104));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel(){Names = new List<NameModel>{ new NameModel { LastName = "name", IpNameNumber = 216156786 }, new NameModel { LastName = "name", IpNameNumber = 1234567 } }, IpBaseNumber ="basenumber",Type = InterestedPartyType.C},
                new InterestedPartyModel(){Names = new List<NameModel>{ new NameModel { LastName = "name", IpNameNumber = 76543210 }, new NameModel { LastName = "name", IpNameNumber = 1234567 } }, IpBaseNumber ="basenumber",Type = InterestedPartyType.C}
            };
            var response = await test.Object.IsValid(submission);

            Assert.Equal(1, response.Submission.Model.InterestedParties.Count);
            Assert.Equal(nameof(IV_25), test.Object.Identifier);
            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check that an IP is not removed from submission if it doesn't belong to a work submissions with Unknown Publisher( IPBaseNumber “I-001631070-4”)
        /// </summary>
        [Fact]
        public async void IV_25_InterestedParty_Not_Removed()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();

            rulesManagerMock.Setup(p => p.GetParameterValueEnumerable<string>("IgnoreIPsForMatchingAndEligibility")).ReturnsAsync(new List<string>() { "I-001631070-4" });
            interestedPartyManagerMock.Setup(v => v.FindManyByBaseNumber(new List<string>() { "I-001631070-4" })).ReturnsAsync(new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "I-001631070-4", Names = new NameModel[] { new NameModel { IpNameNumber = 216156786 }, new NameModel { IpNameNumber = 288936892 } } } });

            var test = new Mock<IV_25>(rulesManagerMock.Object, interestedPartyManagerMock.Object, GetMessagingManagerMock(ErrorCode._104));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel(){Names = new List<NameModel>{ new NameModel { LastName = "name", IpNameNumber = 55555555 }, new NameModel { LastName = "name", IpNameNumber = 1234567 } }, IpBaseNumber ="basenumber",Type = InterestedPartyType.C},
                new InterestedPartyModel(){Names = new List<NameModel>{ new NameModel { LastName = "name", IpNameNumber = 76543210 }, new NameModel { LastName = "name", IpNameNumber = 1234567 } }, IpBaseNumber ="basenumber",Type = InterestedPartyType.C}
            };
            var response = await test.Object.IsValid(submission);

            Assert.Equal(2, response.Submission.Model.InterestedParties.Count);
            Assert.Equal(nameof(IV_25), test.Object.Identifier);
            Assert.True(response.IsValid);
        }
    }
}
