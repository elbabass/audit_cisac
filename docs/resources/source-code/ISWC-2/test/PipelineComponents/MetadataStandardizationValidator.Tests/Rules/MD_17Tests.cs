using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
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
    /// This tests rule MD_17
    /// </summary>
    public class MD_17Tests : TestBase
    {
        /// <summary>
        /// Checks that correct amount of ips are added if PG ip is present
        /// </summary>
        [Fact]
        public async Task MD_17_Submission_TrueParam()
        {
            var submission = new Submission() { Model = new SubmissionModel() { Agency = "80" } };
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var agreementManagerMock = new Mock<IAgreementManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<bool>("ResolveIPIGroupPseudonym"))
                .ReturnsAsync(true);
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            interestedPartyManagerMock.Setup(i => i.CheckForPgIps(It.IsAny<IEnumerable<long>>())).
                ReturnsAsync(new List<long> { 274075462 });
            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(It.IsAny<IEnumerable<long>>()))
                .ReturnsAsync(new List<InterestedPartyModel>() {
                    new InterestedPartyModel() {
                        Agency = "80" ,
                        IpBaseNumber = "I-000056242-3",
                        Names = new List<NameModel>(){
                            new NameModel
                            {
                                Agency =  "80" ,
                                FirstName = "MARK KLAUS",
                                IpNameNumber = 274075364,
                                LastName = "NOAK",
                                TypeCode = NameType.PA
                            },
                             new NameModel
                            {
                                Agency =  "80" ,
                                IpNameNumber = 274075462,
                                LastName = "SONS OF TROUT",
                                TypeCode = NameType.PG
                            }
                        }
                    },
                    new InterestedPartyModel() {
                        Agency =  "80" ,
                        IpBaseNumber = "I-002418350-2",
                        Names = new List<NameModel>(){
                            new NameModel
                            {
                                Agency =  "80" ,
                                IpNameNumber = 274075462,
                                LastName = "SONS OF TROUT",
                                TypeCode = NameType.PG
                            },
                             new NameModel
                            {
                                Agency =  "80" ,
                                FirstName = "DIRK THEODORUS",
                                IpNameNumber = 494281332,
                                LastName = "BISSCHOFF",
                                TypeCode = NameType.PA
                            },
                            new NameModel
                            {
                                Agency =  "80" ,
                                IpNameNumber = 494282231,
                                LastName = "CUTTING JADE",
                                TypeCode = NameType.PG
                            },
                        }
                    },
                });

            var test = new Mock<MD_17>(rulesManagerMock.Object, interestedPartyManagerMock.Object, agreementManagerMock.Object, GetMessagingManagerMock(ErrorCode._154));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel()
                {
                    IpBaseNumber = "I-000056242-3",
                    IPNameNumber = 274075462,
                    Names = new List<NameModel>() { new NameModel() { IpNameNumber = 274075462 } },
                    CisacType = CisacInterestedPartyType.C
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_17), test.Object.Identifier);
            Assert.Equal(3, submission.Model.InterestedParties.Count());
        }

        /// <summary>
        /// Checks that no ips are added if there is no PG ip
        /// </summary>
        [Fact]
        public async Task MD_17_Submission_TrueParam_NoIpsAdded()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var agreementManagerMock = new Mock<IAgreementManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<bool>("ResolveIPIGroupPseudonym"))
                .ReturnsAsync(true);
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(new List<long>() { 274075462 }))
                .ReturnsAsync(new List<InterestedPartyModel>() {
                    new InterestedPartyModel() {
                        Agency =  "80" ,
                        IpBaseNumber = "I-000056242-3",
                        Names = new List<NameModel>(){
                            new NameModel
                            {
                                Agency =  "80" ,
                                FirstName = "MARK KLAUS",
                                IpNameNumber = 274075364,
                                LastName = "NOAK",
                                TypeCode = NameType.PA
                            },
                             new NameModel
                            {
                                Agency =  "80" ,
                                IpNameNumber = 274075462,
                                LastName = "SONS OF TROUT",
                                TypeCode = NameType.OR
                            }
                        }
                    },
                    new InterestedPartyModel() {
                        Agency =  "80" ,
                        IpBaseNumber = "I-002418350-2",
                        Names = new List<NameModel>(){
                            new NameModel
                            {
                                Agency =  "80" ,
                                IpNameNumber = 274075462,
                                LastName = "SONS OF TROUT",
                                TypeCode = NameType.MO
                            },
                             new NameModel
                            {
                                Agency =  "80" ,
                                FirstName = "DIRK THEODORUS",
                                IpNameNumber = 494281332,
                                LastName = "BISSCHOFF",
                                TypeCode = NameType.PA
                            },
                            new NameModel
                            {
                                Agency =  "80" ,
                                IpNameNumber = 494282231,
                                LastName = "CUTTING JADE",
                                TypeCode = NameType.MO
                            },
                        }
                    },
                });

            var test = new Mock<MD_17>(rulesManagerMock.Object, interestedPartyManagerMock.Object, agreementManagerMock.Object, GetMessagingManagerMock(ErrorCode._154));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel()
                {
                    IpBaseNumber = "I-000056242-3",
                    Names = new List<NameModel>() { new NameModel() { IpNameNumber = 274075462 } }
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_17), test.Object.Identifier);
            Assert.Single(submission.Model.InterestedParties);
        }

        /// <summary>
        /// Checks that the IPs have the correct Authoritative values when PG IP is not Authoritative
        /// </summary>
        [Fact]
        public async Task MD_17_Submission_TrueParam_NotAuthAndEligible()
        {
            var submission = new Submission() { Model = new SubmissionModel() { Agency = "80" } };
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var agreementManagerMock = new Mock<IAgreementManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<bool>("ResolveIPIGroupPseudonym"))
                .ReturnsAsync(true);
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            interestedPartyManagerMock.Setup(i => i.CheckForPgIps(It.IsAny<IEnumerable<long>>())).
                ReturnsAsync(new List<long> { 274075462 });
            interestedPartyManagerMock.Setup(i => i.IsAuthoritative(It.IsAny<InterestedPartyModel>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(false);
            interestedPartyManagerMock.Setup(i => i.IsAuthoritative(It.IsAny<InterestedPartyModel>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(true);

            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(new List<long>() { 274075462 }))
                .ReturnsAsync(new List<InterestedPartyModel>() {
                    new InterestedPartyModel() {
                        Agency =  "80" ,
                        IpBaseNumber = "I-000056242-3",
                        Names = new List<NameModel>(){
                            new NameModel
                            {
                                Agency =  "80" ,
                                FirstName = "MARK KLAUS",
                                IpNameNumber = 274075364,
                                LastName = "NOAK",
                                TypeCode = NameType.PA
                            },
                             new NameModel
                            {
                                Agency =  "80" ,
                                IpNameNumber = 274075462,
                                LastName = "SONS OF TROUT",
                                TypeCode = NameType.PG
                            }
                        }
                    },
                    new InterestedPartyModel() {
                        Agency =  "80" ,
                        IpBaseNumber = "I-002418350-2",
                        Names = new List<NameModel>(){
                            new NameModel
                            {
                                Agency =  "80" ,
                                IpNameNumber = 274075462,
                                LastName = "SONS OF TROUT",
                                TypeCode = NameType.PG
                            },
                             new NameModel
                            {
                                Agency =  "80" ,
                                FirstName = "DIRK THEODORUS",
                                IpNameNumber = 494281332,
                                LastName = "BISSCHOFF",
                                TypeCode = NameType.PA
                            },
                            new NameModel
                            {
                                Agency =  "80" ,
                                IpNameNumber = 494282231,
                                LastName = "CUTTING JADE",
                                TypeCode = NameType.PG
                            },
                        }
                    },
                });

            var test = new Mock<MD_17>(rulesManagerMock.Object, interestedPartyManagerMock.Object, agreementManagerMock.Object, GetMessagingManagerMock(ErrorCode._154));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel()
                {
                    IpBaseNumber = "I-000056242-3",
                    IPNameNumber = 274075462,
                    Names = new List<NameModel>() { new NameModel() { IpNameNumber = 274075462 } },
                    CisacType = CisacInterestedPartyType.C,
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_17), test.Object.Identifier);
            Assert.Equal(3, submission.Model.InterestedParties.Count());
            Assert.False(submission.Model.InterestedParties.FirstOrDefault(x => x.Names.FirstOrDefault().TypeCode == NameType.PG).IsAuthoritative);
            Assert.True(submission.Model.InterestedParties.Where(x => x.Names.FirstOrDefault().TypeCode == NameType.PA).All(z => (bool)z.IsAuthoritative));

        }

        /// <summary>
        /// Checks that the IPs have the correct Authoritative values when PG IP is Authoritative
        /// </summary>
        [Fact]
        public async Task MD_17_Submission_TrueParam_AuthAndEligible()
        {
            var submission = new Submission() { Model = new SubmissionModel() { Agency = "80" } };
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var agreementManagerMock = new Mock<IAgreementManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<bool>("ResolveIPIGroupPseudonym"))
                .ReturnsAsync(true);
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("AllowNonAffiliatedSubmissions")).ReturnsAsync(true);
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            interestedPartyManagerMock.Setup(i => i.CheckForPgIps(It.IsAny<IEnumerable<long>>())).
                ReturnsAsync(new List<long> { 274075462 });
            interestedPartyManagerMock.Setup(i => i.IsAuthoritative(It.IsAny<InterestedPartyModel>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(true);
            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(new List<long>() { 274075462 }))
                .ReturnsAsync(new List<InterestedPartyModel>() {
                    new InterestedPartyModel() {
                        Agency =  "80" ,
                        IpBaseNumber = "I-000056242-3",
                        Names = new List<NameModel>(){
                            new NameModel
                            {
                                Agency =  "80" ,
                                FirstName = "MARK KLAUS",
                                IpNameNumber = 274075364,
                                LastName = "NOAK",
                                TypeCode = NameType.PA
                            },
                             new NameModel
                            {
                                Agency =  "80" ,
                                IpNameNumber = 274075462,
                                LastName = "SONS OF TROUT",
                                TypeCode = NameType.PG
                            }
                        }
                    },
                    new InterestedPartyModel() {
                        Agency =  "80" ,
                        IpBaseNumber = "I-002418350-2",
                        Names = new List<NameModel>(){
                            new NameModel
                            {
                                Agency =  "80" ,
                                IpNameNumber = 274075462,
                                LastName = "SONS OF TROUT",
                                TypeCode = NameType.PG
                            },
                             new NameModel
                            {
                                Agency =  "80" ,
                                FirstName = "DIRK THEODORUS",
                                IpNameNumber = 494281332,
                                LastName = "BISSCHOFF",
                                TypeCode = NameType.PA
                            },
                            new NameModel
                            {
                                Agency =  "80" ,
                                IpNameNumber = 494282231,
                                LastName = "CUTTING JADE",
                                TypeCode = NameType.PG
                            },
                        }
                    },
                });

            var test = new Mock<MD_17>(rulesManagerMock.Object, interestedPartyManagerMock.Object, agreementManagerMock.Object, GetMessagingManagerMock(ErrorCode._154));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel()
                {
                    IpBaseNumber = "I-000056242-3",
                    IPNameNumber = 274075462,
                    Names = new List<NameModel>() { new NameModel() { IpNameNumber = 274075462 } },
                    CisacType = CisacInterestedPartyType.C,
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_17), test.Object.Identifier);
            Assert.Equal(3, submission.Model.InterestedParties.Count());
            Assert.True(submission.Model.InterestedParties.FirstOrDefault(x => x.Names.FirstOrDefault().TypeCode == NameType.PG).IsAuthoritative);
            Assert.False(submission.Model.InterestedParties.Where(x => x.Names.FirstOrDefault().TypeCode == NameType.PA).All(z => (bool)z.IsAuthoritative));

        }

        /// <summary>
        /// Checks number of IPs stay the same as per submission. Parameter = False
        /// </summary>
        [Fact]
        public async Task MD_17_Submission_FalseParam()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var agreementManagerMock = new Mock<IAgreementManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<bool>("ResolveIPIGroupPseudonym"))
                .ReturnsAsync(false);
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(new List<long>() { 274075462 }))
                .ReturnsAsync(default(IEnumerable<InterestedPartyModel>));

            var test = new Mock<MD_17>(rulesManagerMock.Object, interestedPartyManagerMock.Object, agreementManagerMock.Object, GetMessagingManagerMock(ErrorCode._154));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel()
                {
                    IpBaseNumber = "I-000056242-3",
                    Names = new List<NameModel>() { new NameModel() { IpNameNumber = 274075462 } }
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_17), test.Object.Identifier);
            Assert.Single(submission.Model.InterestedParties);
        }

        /// <summary>
        /// Checks that correct amount of ips are present if no ips are returned from provided IpBasenumber
        /// </summary>
        [Fact]
        public async Task MD_17_Submission_TrueParam_NoIps()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var agreementManagerMock = new Mock<IAgreementManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<bool>("ResolveIPIGroupPseudonym"))
                .ReturnsAsync(true);
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(new List<long>() { 274075462 }))
                .ReturnsAsync(default(IEnumerable<InterestedPartyModel>));

            var test = new Mock<MD_17>(rulesManagerMock.Object, interestedPartyManagerMock.Object, agreementManagerMock.Object, GetMessagingManagerMock(ErrorCode._154));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                    new InterestedPartyModel()
                    {
                        IpBaseNumber = "I-000056242-3",
                        Names = new List<NameModel>() { new NameModel() { IpNameNumber = 274075462 } },
                        CisacType = CisacInterestedPartyType.C
                    }
                };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_17), test.Object.Identifier);
            Assert.Single(submission.Model.InterestedParties);
        }
    }
}
