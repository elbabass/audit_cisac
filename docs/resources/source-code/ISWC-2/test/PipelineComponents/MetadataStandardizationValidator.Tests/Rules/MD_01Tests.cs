using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
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
    /// Tests rule MD_01
    /// </summary>
    public class MD_01Tests : TestBase
    {
        /// <summary>
        /// Check that ip roles are mapped correctly
        /// </summary>
        [Fact]
        public async Task MD_01_RolesMapped()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("CalculateRolledUpRole"))
                .ReturnsAsync("C(C, CA, A);MA(AR, SR);TA(AD, SA, TR);E(E, AM)");

            var test = new Mock<MD_01>(rulesManagerMock.Object);

            submission.Model.InterestedParties = new List<InterestedPartyModel>()
            {
                new InterestedPartyModel(){
                    Type = InterestedPartyType.C
                },
                new InterestedPartyModel(){
                   Type= InterestedPartyType.CA
                },
                 new InterestedPartyModel(){
                    Type = InterestedPartyType.AR
                },
                new InterestedPartyModel(){
                   Type= InterestedPartyType.A
                },
                 new InterestedPartyModel(){
                    Type = InterestedPartyType.E
                },
                new InterestedPartyModel(){
                   Type= InterestedPartyType.AD
                },
                 new InterestedPartyModel(){
                   Type= InterestedPartyType.SE
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.True(test.Object.Identifier == nameof(MD_01));
            Assert.Equal(CisacInterestedPartyType.C, response.Submission.Model.InterestedParties.ElementAt(0).CisacType);
            Assert.Equal(CisacInterestedPartyType.C, response.Submission.Model.InterestedParties.ElementAt(1).CisacType);
            Assert.Equal(CisacInterestedPartyType.MA, response.Submission.Model.InterestedParties.ElementAt(2).CisacType);
            Assert.Equal(CisacInterestedPartyType.C, response.Submission.Model.InterestedParties.ElementAt(3).CisacType);
            Assert.Equal(CisacInterestedPartyType.E, response.Submission.Model.InterestedParties.ElementAt(4).CisacType);
            Assert.Equal(CisacInterestedPartyType.TA, response.Submission.Model.InterestedParties.ElementAt(5).CisacType);
            Assert.Equal(CisacInterestedPartyType.X, response.Submission.Model.InterestedParties.ElementAt(6).CisacType);
        }

        /// <summary>
        /// Scenario: TA role code is provided in the submission
        /// Expected: CISAC type is set to TA and Role type is set to first value in the TA list (AD)
        /// </summary>
        [Fact]
        public async Task MD_02_RolesMapped()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("CalculateRolledUpRole"))
                .ReturnsAsync("C(C, CA, A);MA(AR, SR);TA(AD, SA, TR);E(E, AM)");

            var test = new Mock<MD_01>(rulesManagerMock.Object);

            submission.Model.InterestedParties = new List<InterestedPartyModel>()
            {
                new InterestedPartyModel(){
                    Type = InterestedPartyType.TA
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.True(test.Object.Identifier == nameof(MD_01));
            Assert.Equal(CisacInterestedPartyType.TA, response.Submission.Model.InterestedParties.ElementAt(0).CisacType);
            Assert.Equal(InterestedPartyType.AD, response.Submission.Model.InterestedParties.ElementAt(0).Type);
        }

        /// <summary>
        /// Scenario: MA role code is provided in the submission
        /// Expected: CISAC type is set to MA and Role type is set to first value in the MA list (AR)
        /// </summary>
        [Fact]
        public async Task MD_03_RolesMapped()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("CalculateRolledUpRole"))
                .ReturnsAsync("C(C, CA, A);MA(AR, SR);TA(AD, SA, TR);E(E, AM)");

            var test = new Mock<MD_01>(rulesManagerMock.Object);

            submission.Model.InterestedParties = new List<InterestedPartyModel>()
            {
                new InterestedPartyModel(){
                    Type = InterestedPartyType.MA
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.True(test.Object.Identifier == nameof(MD_01));
            Assert.Equal(CisacInterestedPartyType.MA, response.Submission.Model.InterestedParties.ElementAt(0).CisacType);
            Assert.Equal(InterestedPartyType.AR, response.Submission.Model.InterestedParties.ElementAt(0).Type);
        }
    }
}