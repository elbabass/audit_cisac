using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.IswcEligibilityValidator.Rules;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Submission = SpanishPoint.Azure.Iswc.Bdo.Submissions.Submission;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.IswcEligibilityValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule EL_02
    /// </summary>
    public class EL_02Tests : TestBase
    {
        /// <summary>
        /// Checks that IsEligible is set to true if none of the IpBaseNumbers are affiliated with an agency
        /// </summary>
        [Fact]
        public async void EL_02_EligibleNoAffiliatedAgencies()
        {
            var submission = new Submission() { Model = new SubmissionModel(), IsEligible = false };
            var agreementManagerMock = new Mock<IAgreementManager>();
            var rulesManagerMock = new Mock<IRulesManager>();

            var listOfIpBaseNumbers = new List<string>();
            listOfIpBaseNumbers.Add("123");
            listOfIpBaseNumbers.Add("456");

            submission.Model.Agency = "100";
            submission.Model.InterestedParties = new List<InterestedPartyModel>()
            {
                new InterestedPartyModel() { IpBaseNumber = "123", CisacType = CisacInterestedPartyType.C},
                new InterestedPartyModel() { IpBaseNumber = "456", CisacType = CisacInterestedPartyType.C},
                new InterestedPartyModel() { IpBaseNumber = "I-001635861-3", CisacType = CisacInterestedPartyType.E},
            };

            agreementManagerMock.Setup(a => a.FindManyAsync(listOfIpBaseNumbers)).ReturnsAsync(new Agreement[] { });
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("AllowNonAffiliatedSubmissions")).ReturnsAsync(true);


            var test = new Mock<EL_02>(agreementManagerMock.Object, rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._154)).Object;

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(EL_02), test.Identifier);
            Assert.True(submission.IsEligible);
        }

        /// <summary>
        /// Checks that IsEligible is set to false if one of the IpBaseNumbers is affiliated with an agency which is not the submitting agency
        /// </summary>
        [Fact]
        public async void EL_02_Ineligible()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var agreementManagerMock = new Mock<IAgreementManager>();
            var rulesManagerMock = new Mock<IRulesManager>();

            var listOfIpBaseNumbers = new List<string>();
            listOfIpBaseNumbers.Add("I-000000001-9");
            listOfIpBaseNumbers.Add("I-000000008-6");

            submission.Model.Agency = "100";
            submission.Model.InterestedParties = new List<InterestedPartyModel>()
            {
                new InterestedPartyModel() { IpBaseNumber = "I-000000001-9", CisacType = CisacInterestedPartyType.C},
                new InterestedPartyModel() { IpBaseNumber = "I-000000008-6", CisacType = CisacInterestedPartyType.C},
                new InterestedPartyModel() { IpBaseNumber = "I-001635861-3", CisacType = CisacInterestedPartyType.E},
            };

            agreementManagerMock.Setup(a => a.FindManyAsync(listOfIpBaseNumbers))
                .ReturnsAsync(new Agreement[] {
                    new Agreement() { IpbaseNumber = "I-000000001-9", AgencyId = "20" },
                    new Agreement() { IpbaseNumber = "I-645645646-9", AgencyId = "100" }
                });

			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("AllowNonAffiliatedSubmissions")).ReturnsAsync(true);


			var test = new Mock<EL_02>(agreementManagerMock.Object, rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._154)).Object;

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(EL_02), test.Identifier);
            Assert.False(submission.IsEligible);
        }


        /// <summary>
        /// Checks that IsEligible is set to true if all of the IpBaseNumbers are affiliated with just the submitting agency
        /// </summary>
        [Fact]
        public async void EL_02_EligibleAllSubmittingAgency()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var agreementManagerMock = new Mock<IAgreementManager>();
            var rulesManagerMock = new Mock<IRulesManager>();

            var listOfIpBaseNumbers = new List<string>();
            listOfIpBaseNumbers.Add("I-000000001-9");
            listOfIpBaseNumbers.Add("I-000000008-6");

            submission.Model.Agency = "100";
            submission.Model.InterestedParties = new List<InterestedPartyModel>()
            {
                new InterestedPartyModel() { IpBaseNumber = "I-000000001-9", CisacType = CisacInterestedPartyType.C},
                new InterestedPartyModel() { IpBaseNumber = "I-000000008-6", CisacType = CisacInterestedPartyType.C},
                new InterestedPartyModel() { IpBaseNumber = "I-001635861-3", CisacType = CisacInterestedPartyType.E},
            };

            agreementManagerMock.Setup(a => a.FindManyAsync(listOfIpBaseNumbers))
                .ReturnsAsync(new Agreement[] {
                    new Agreement() { IpbaseNumber = "I-000000001-9", AgencyId = "100" },
                    new Agreement() { IpbaseNumber = "I-645645646-9", AgencyId = "100" }
                });

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("AllowNonAffiliatedSubmissions")).ReturnsAsync(true);


            var test = new Mock<EL_02>(agreementManagerMock.Object, rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._154)).Object;

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(EL_02), test.Identifier);
            Assert.True(submission.IsEligible);
        }


        /// <summary>
        /// Checks that transaction is reject of there are no affiliated agencies. Param = False
        /// </summary>
        [Fact]
        public async void EL_02_FalseParamNoAffiliatedAgencies()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var agreementManagerMock = new Mock<IAgreementManager>();
            var rulesManagerMock = new Mock<IRulesManager>();

            var listOfIpBaseNumbers = new List<string>();
            listOfIpBaseNumbers.Add("I-000000001-9");

            submission.Model.Agency = "100";
            submission.Model.InterestedParties = new List<InterestedPartyModel>()
            {
                new InterestedPartyModel() { IpBaseNumber = "I-000000001-9", CisacType = CisacInterestedPartyType.C}
            };

            agreementManagerMock.Setup(a => a.FindManyAsync(listOfIpBaseNumbers))
                .ReturnsAsync(new List<Agreement>(){ });

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("AllowNonAffiliatedSubmissions")).ReturnsAsync(false);


            var test = new Mock<EL_02>(agreementManagerMock.Object, rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._154)).Object;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(EL_02), test.Identifier);
            Assert.Equal(ErrorCode._154, response.Submission.Rejection.Code);
        }

       

    }
}
