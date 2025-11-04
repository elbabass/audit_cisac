using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.IswcEligibilityValidator.Rules;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.IswcEligibilityValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule EL_04
    /// </summary>
    public class EL_04Tests : TestBase
    {
        private readonly Mock<IAgreementManager> agreementManagerMock = new Mock<IAgreementManager>();
        private readonly Mock<IInterestedPartyManager> interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
        private readonly Mock<IMessagingManager> messagingManagerMock = new Mock<IMessagingManager>();
        private readonly Mock<IRulesManager> rulesManagerMock = new Mock<IRulesManager>();

        /// <summary>
        /// Checks rule passes if submitting publisher has agreement with submitting agency
        /// </summary>
        [Fact]
        public async Task EL_04_PublisherRepresentedBySubmitter_Valid()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    AdditionalIdentifiers = new List<Bdo.Work.AdditionalIdentifier>
                {
                    new Bdo.Work.AdditionalIdentifier{NameNumber = 1234, WorkCode="el-04workcode" }
                },
                    Agency = "052"
                }
            };


            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            agreementManagerMock.Setup(a => a.FindManyAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(new List<Agreement> {
                new Agreement{Agency = new Agency{ AgencyId = "052" }, IpbaseNumber = "12345" }
            });
            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<InterestedPartyModel>
            {
                new InterestedPartyModel
                {
                    IPNameNumber = 1234,
                    IpBaseNumber = "12345",
                    CisacType = CisacInterestedPartyType.E
                }
            });

            var test = new Mock<EL_04>(agreementManagerMock.Object, messagingManagerMock.Object, interestedPartyManagerMock.Object, rulesManagerMock.Object).Object;

            var response = await test.IsValid(submission);
            Assert.True(response.IsValid);
            Assert.Equal(nameof(EL_04), test.Identifier);
        }

        /// <summary>
        /// Checks rule fails if submitting publisher is not a member of submitting agency
        /// </summary>
        [Fact]
        public async Task EL_04_PublisherNotRepresentedBySubmitter_InValid()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    AdditionalIdentifiers = new List<Bdo.Work.AdditionalIdentifier>
                    {
                        new Bdo.Work.AdditionalIdentifier{NameNumber = 1234, WorkCode="el-04workcode" }
                    },
                    Agency = "053"
                }
            };

            agreementManagerMock.Setup(a => a.FindManyAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(new List<Agreement>
            {
                new Agreement{ Agency = new Agency{ AgencyId = "128" }, IpbaseNumber = "98765" }
            });

            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<InterestedPartyModel>
            {
                new InterestedPartyModel
                {
                    IPNameNumber = 1234,
                    IpBaseNumber = "12345",
                    CisacType = CisacInterestedPartyType.E
                }
            });


            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("707(10,021)");

            var test = new Mock<EL_04>(agreementManagerMock.Object, GetMessagingManagerMock(ErrorCode._161), interestedPartyManagerMock.Object, rulesManagerMock.Object).Object;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(EL_04), test.Identifier);
            Assert.Equal(ErrorCode._161, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Checks that rule passes if it is a normal CAR transaction (i.e. has no additionalIdentifier info)
        /// </summary>
        [Fact]
        public async Task EL_04_NoAdditionalIdentifierProvided_Valid()
        {
            var submission = new Submission
            {
                Model = new SubmissionModel { Agency = "052" }
            };

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("707(10,021)");
            var test = new Mock<EL_04>(agreementManagerMock.Object, GetMessagingManagerMock(ErrorCode._161), interestedPartyManagerMock.Object, rulesManagerMock.Object).Object;

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(EL_04), test.Identifier);
        }

        /// <summary>
        /// Checks that rule fails if submitting publisher does not exist
        /// </summary>
        [Fact]
        public async Task EL_04_InterestedPartyDoesNotExist_Invalid()
        {
            var submission = new Submission
            {
                Model = new SubmissionModel()
                {
                    AdditionalIdentifiers = new List<Bdo.Work.AdditionalIdentifier>
                    {
                        new Bdo.Work.AdditionalIdentifier{NameNumber = 1234, WorkCode="el-04workcode" }
                    },
                    Agency = "052"
                }
            };


            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("707(010)");
            agreementManagerMock.Setup(a => a.FindManyAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(new List<Agreement>
            {
                new Agreement{ Agency = new Agency{ AgencyId = "128" }, IpbaseNumber = "98765" }
            });

            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<InterestedPartyModel> { });

            var test = new Mock<EL_04>(agreementManagerMock.Object, GetMessagingManagerMock(ErrorCode._161), interestedPartyManagerMock.Object, rulesManagerMock.Object).Object;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(EL_04), test.Identifier);
            Assert.Equal(ErrorCode._161, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Checks that rule passes if submitting publisher is submitting on behalf of agency i.e Music Mark (707)
        /// </summary>
        [Fact]
        public async Task EL_04_Publisher_IsEligible_to_submit_for_agency_valid()
        {
            var submission = new Submission
            {
                Model = new SubmissionModel()
                {
                    AdditionalIdentifiers = new List<Bdo.Work.AdditionalIdentifier>
                    {
                        new Bdo.Work.AdditionalIdentifier{NameNumber = 1234, WorkCode="el-04workcode" }
                    },
                    Agency = "707",
                    InterestedParties = new List<InterestedPartyModel> { new InterestedPartyModel { IpBaseNumber = "12345", Agency = "010" }, new InterestedPartyModel
                {
                    IPNameNumber = 1234,
                    IpBaseNumber = "12345",
                    CisacType = CisacInterestedPartyType.E
                } }
                }
            };
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("707(010)");
            agreementManagerMock.Setup(a => a.FindManyAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(new List<Agreement>
            {
                new Agreement{ Agency = new Agency{ AgencyId = "010" }, IpbaseNumber = "12345" }
            });

            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<InterestedPartyModel> { new InterestedPartyModel { Agency = "707", IpBaseNumber = "12345" } });

            var test = new Mock<EL_04>(agreementManagerMock.Object, messagingManagerMock.Object, interestedPartyManagerMock.Object, rulesManagerMock.Object).Object;

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid); 
            Assert.Equal(nameof(EL_04), test.Identifier);
        }
    }
}
