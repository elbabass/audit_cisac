using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule IV_19
    /// </summary>
    public class IV_19Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if the IPNameNumber does not exist in the database
        /// </summary>
        [Fact]
        public async void IV_19_Submission_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };

            var test = new Mock<IV_19>(GetMessagingManagerMock(ErrorCode._102));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel(){Names = new List<NameModel>{ new NameModel { LastName = "name", IpNameNumber = 0 } }, IpBaseNumber ="basenumber",Type = InterestedPartyType.C}
            };

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_19), test.Object.Identifier);
            Assert.Equal(ErrorCode._102, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if the IPNameNumber exists in the database
        /// </summary>
        [Fact]
        public async void IV_19_Submission_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };

            var test = new Mock<IV_19>(GetMessagingManagerMock(ErrorCode._102));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel(){Names = new List<NameModel>{ new NameModel { LastName = "name", IpNameNumber = 345435435 } }, IpBaseNumber ="basenumber",Type = InterestedPartyType.C}
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_19), test.Object.Identifier);
        }
    }
}
