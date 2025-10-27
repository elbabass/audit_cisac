using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.PipelineComponents.IswcEligibilityValidator.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Submission = SpanishPoint.Azure.Iswc.Bdo.Submissions.Submission;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.IswcEligibilityValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule EL_03
    /// </summary>
    public class EL_03Tests
    {
        private readonly Mock<IRulesManager> rulesManagerMock = new Mock<IRulesManager>();
        private readonly Mock<IAgreementManager> agreementManagerMock = new Mock<IAgreementManager>();
        private readonly Mock<IMessagingManager> messagingManagerMock = new Mock<IMessagingManager>();
        private readonly Mock<IWorkManager> workManager = new Mock<IWorkManager>();

        /// <summary>
        /// Checks that IsEligible is set to true if submitter represents at least one IP with an ISWC eligible role
        /// </summary>
        [Fact]
        public async void EL_03_EligibleOneIpRepresentedBySubmitter()
        {
            var submission = new Submission() { Model = new SubmissionModel() };

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");
            agreementManagerMock.Setup(a => a.FindManyAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(new List<Agreement>() {
                new Agreement { Agency = new Agency { AgencyId = "030" }, IpbaseNumber = "777" } });

            var test = new Mock<EL_03>(rulesManagerMock.Object, agreementManagerMock.Object, workManager.Object).Object;

            submission.Model.InterestedParties = new List<InterestedPartyModel>()
            {
                new InterestedPartyModel(){ CisacType = CisacInterestedPartyType.C, IpBaseNumber = "777"},
                new InterestedPartyModel(){ CisacType = CisacInterestedPartyType.E, IpBaseNumber = "222"},
            };
            submission.Model.Agency = "308";

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(EL_03), test.Identifier);
            Assert.True(submission.IsEligible);
        }

        /// <summary>
        /// Checks that IsEligible is set to false if submitter represents at least one IP but the role type(s) is not an ISWC eligible role
        /// </summary>
        [Fact]
        public async void EL_03_EligibleOneIPRepresentedBySubmitterRoleTypeIneligible()
        {
            var submission = new Submission() { Model = new SubmissionModel() };

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");
            agreementManagerMock.Setup(a => a.FindManyAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(new List<Agreement>() {
                new Agreement { Agency = new Agency { AgencyId = "030" }, IpbaseNumber = "777" } });

            var test = new Mock<EL_03>(rulesManagerMock.Object, agreementManagerMock.Object, workManager.Object).Object;

            submission.Model.InterestedParties = new List<InterestedPartyModel>()
            {
                new InterestedPartyModel(){ CisacType = CisacInterestedPartyType.AM, IpBaseNumber = "777" }
            };
            submission.Model.Agency = "308";

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(EL_03), test.Identifier);
            Assert.True(submission.IsEligible);
        }


        /// <summary>
        /// Checks that IsEligible is set to false if the submitter does not represent at least one IP with an ISWC eligible role and the Creator IP Base Numbers are not Public Domain
        /// </summary>
        [Fact]
        public async void EL_03_Ineligible()
        {
            var submission = new Submission() { Model = new SubmissionModel() };

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");
            agreementManagerMock.Setup(a => a.FindManyAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(new List<Agreement>() {
                new Agreement { Agency = new Agency { AgencyId = "2" }, IpbaseNumber = "12345" } });

            var test = new Mock<EL_03>(rulesManagerMock.Object, agreementManagerMock.Object, workManager.Object).Object;

            submission.Model.InterestedParties = new List<InterestedPartyModel>()
            {
                new InterestedPartyModel(){CisacType = CisacInterestedPartyType.C, IpBaseNumber = "12345" }
            };
            submission.Model.Agency = "1";

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(EL_03), test.Identifier);
            Assert.False(submission.IsEligible);
        }


        /// <summary>
        /// Checks that IsEligible is set to true if all creator IP Base numbers are Public Domain and have ISWC eligible roles
        /// </summary>
        [Fact]
        public async void EL_03_EligibleAllIPBaseNumbersPDAndEligibleRoles()
        {
            var submission = new Submission() { Model = new SubmissionModel() };

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");
            rulesManagerMock.Setup(v => v.GetParameterValue<bool>("AllowPDWorkSubmissions")).ReturnsAsync(true);
            agreementManagerMock.Setup(a => a.FindManyAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(new List<Agreement>() {
                new Agreement { Agency = new Agency { AgencyId = "1" }, IpbaseNumber = "12345" } });

            var test = new Mock<EL_03>(rulesManagerMock.Object, agreementManagerMock.Object, workManager.Object).Object;

            submission.Model.InterestedParties = new List<InterestedPartyModel>()
            {
                new InterestedPartyModel(){IpBaseNumber = CommonIPs.PublicDomain, CisacType = CisacInterestedPartyType.C },
                new InterestedPartyModel(){IpBaseNumber = CommonIPs.TRAD, CisacType = CisacInterestedPartyType.MA },
                 new InterestedPartyModel(){
                    IpBaseNumber = "ipbasenumber", CisacType = CisacInterestedPartyType.C,
                    DeathDate = DateTime.Parse("1902-03-14 09:49:48")
                }
            };
            submission.Model.Agency = "10";

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(EL_03), test.Identifier);
            Assert.True(submission.IsEligible);
        }

        /// <summary>
        /// Checks that IsEligible is set to false if all creator IP Base numbers are Public Domain but role type(s) is not an ISWC eligible role
        /// </summary>
        [Fact]
        public async void EL_03_InEligibleNotAllIPBaseNumbersPDAndEligibleRoles()
        {
            var submission = new Submission() { Model = new SubmissionModel() };

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");
            rulesManagerMock.Setup(v => v.GetParameterValue<bool>("AllowPDWorkSubmissions")).ReturnsAsync(true);
            agreementManagerMock.Setup(a => a.FindManyAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(new List<Agreement>() {
                new Agreement { Agency = new Agency { AgencyId = "1" }, IpbaseNumber = CommonIPs.TRAD } });

            var test = new Mock<EL_03>(rulesManagerMock.Object, agreementManagerMock.Object, workManager.Object).Object;

            submission.Model.InterestedParties = new List<InterestedPartyModel>()
            {
                new InterestedPartyModel(){IpBaseNumber = CommonIPs.PublicDomain, CisacType = CisacInterestedPartyType.C },
                new InterestedPartyModel(){IpBaseNumber = CommonIPs.TRAD, CisacType = CisacInterestedPartyType.MA }
            };
            submission.Model.Agency = "10";

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(EL_03), test.Identifier);
            Assert.False(submission.IsEligible);
        }


        /// <summary>
        /// Checks that IsEligible is set to false if not all creator IPBaseNumbers are Public Domain
        /// </summary>
        [Fact]
        public async void EL_03_InEligibleNotAllIPBaseNumbersPD()
        {
            var submission = new Submission() { Model = new SubmissionModel() };

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");
            rulesManagerMock.Setup(v => v.GetParameterValue<bool>("AllowPDWorkSubmissions")).ReturnsAsync(true);
            agreementManagerMock.Setup(a => a.FindManyAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(new List<Agreement>() {
                new Agreement { Agency = new Agency { AgencyId = "1" }, IpbaseNumber = "12345" } });

            var test = new Mock<EL_03>(rulesManagerMock.Object, agreementManagerMock.Object, workManager.Object).Object;

            submission.Model.InterestedParties = new List<InterestedPartyModel>()
            {
                new InterestedPartyModel(){IpBaseNumber = CommonIPs.PublicDomain, CisacType = CisacInterestedPartyType.C },
                new InterestedPartyModel(){IpBaseNumber = "IPBaseNumber", CisacType = CisacInterestedPartyType.MA }
            };
            submission.Model.Agency = "10";

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(EL_03), test.Identifier);
            Assert.False(submission.IsEligible);
        }

        /// <summary>
        /// Checks that IsEligible is set to true if submitter is a hub society and a child agency represents at least one IP with an ISWC eligible role
        /// </summary>
        [Fact]
        public async void EL_03_EligibleOneIpRepresentedBySubmitterChildAgency()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var agreementManagerMock = new Mock<IAgreementManager>();

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");
            rulesManagerMock.Setup(v => v.GetParameterValue<bool>("AllowPDWorkSubmissions")).ReturnsAsync(true);
            agreementManagerMock.Setup(a => a.FindManyAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(new List<Agreement>() {
                new Agreement { Agency = new Agency { AgencyId = "030" }, IpbaseNumber = "777" } });

            var test = new Mock<EL_03>(rulesManagerMock.Object, agreementManagerMock.Object, workManager.Object).Object;

            submission.Model.InterestedParties = new List<InterestedPartyModel>()
            {
                new InterestedPartyModel(){ CisacType = CisacInterestedPartyType.C, IpBaseNumber = "777"},
                new InterestedPartyModel(){ CisacType = CisacInterestedPartyType.E, IpBaseNumber = "222"},
            };
            submission.Model.Agency = "308";

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(EL_03), test.Identifier);
            Assert.True(submission.IsEligible);
        }

       
    }
}
