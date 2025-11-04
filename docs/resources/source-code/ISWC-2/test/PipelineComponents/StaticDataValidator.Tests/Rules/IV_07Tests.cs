using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule IV_07
    /// </summary>
    public class IV_07Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if the work submission does not contain a creator role
        /// </summary>
        [Fact]
        public async Task IV_07_Submission_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();

            rulesManagerMock.Setup(p => p.GetParameterValueEnumerable<InterestedPartyType>("MinOfOneCreatorRole"))
                .ReturnsAsync(new[] { InterestedPartyType.C, InterestedPartyType.CA });

            var test = new Mock<IV_07>(rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._111));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel(){ Type = InterestedPartyType.ES }
            };

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_07), test.Object.Identifier);
            Assert.Equal(ErrorCode._111, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if  the work submission contains at least one creator role
        /// </summary>
        [Fact]
        public async Task IV_07_Submission_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();

            rulesManagerMock.Setup(p => p.GetParameterValueEnumerable<InterestedPartyType>("MinOfOneCreatorRole"))
                .ReturnsAsync(new[] { InterestedPartyType.C, InterestedPartyType.CA });

            var test = new Mock<IV_07>(rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._111));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel(){ Type = InterestedPartyType.CA }
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_07), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction fails if the interested parties is null
        /// </summary>
        [Fact]
        public async Task IV_07_Submission_InValid_null()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();

            rulesManagerMock.Setup(p => p.GetParameterValueEnumerable<InterestedPartyType>("MinOfOneCreatorRole"))
                .ReturnsAsync(new[] { InterestedPartyType.C, InterestedPartyType.CA });

            var test = new Mock<IV_07>(rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._111));

            submission.Model.InterestedParties = null;

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_07), test.Object.Identifier);
            Assert.Equal(ErrorCode._111, response.Submission.Rejection.Code);
        }
    }
}
