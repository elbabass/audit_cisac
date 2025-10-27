using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.MetadataStandardizationValidator.Rules;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MetadataStandardizationValidator.Tests.Rules
{
    /// <summary>
    /// This tests rule MD_16
    /// </summary>
    public class MD_16Tests : TestBase
    {
        /// <summary>
        /// Check that the transaction passes and the new Status 1 IP Base number is set if submitted IP is Status 3. Parameter = True
        /// </summary>
        [Fact]
        public async void MD_16_Submission_TrueParam_NewIpBaseNumber()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var agreementManagerMock = new Mock<IAgreementManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<bool>("ResolveIPIBaseNumber"))
                .ReturnsAsync(true);
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(new List<long>() { 139377839 }))
                .ReturnsAsync(new List<InterestedPartyModel>() {
                    new InterestedPartyModel() {
                        IpBaseNumber = "I-001696412-6",
                        Status = new StatusModel() {
                            ForwardingBaseNumber = "I-001629862-5",
                            StatusCode = IpStatus.DeletionCrossReference,
                            IpbaseNumber = "I-001696412-6"
                        },
                        Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } }
                    }
                });

			interestedPartyManagerMock.Setup(x => x.FollowChainForStatus(It.IsAny<InterestedPartyModel>()))
				  .ReturnsAsync(new InterestedPartyModel()
				  {
					  IpBaseNumber = "I-001629862-5",
					  Names = new List<NameModel>() { new NameModel() { IpNameNumber = 139377839, TypeCode = NameType.PA } },
					  Status = new StatusModel
					  {
						  StatusCode = IpStatus.SelfReferencedValidIp,
						  ForwardingBaseNumber = "I-001629862-5",
						  IpbaseNumber = "I-001629862-5"
					  },
					  CisacType = CisacInterestedPartyType.C,
					  Type = InterestedPartyType.C
				  });

			var test = new Mock<MD_16>(rulesManagerMock.Object, interestedPartyManagerMock.Object,
                agreementManagerMock.Object, GetMessagingManagerMock(ErrorCode._137));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel() {
                    IpBaseNumber = "I-001696412-6",
                    Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } }
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_16), test.Object.Identifier);
            Assert.Equal("I-001629862-5", response.Submission.Model.InterestedParties.ElementAt(0).IpBaseNumber);
        }

        /// <summary>
        /// Check that transaction fails if IP is status 3 and submitter is ISWC eligible and authoritative for the status 3 IP. Parameter = False
        /// </summary>
        [Fact]
        public async void MD_16_Submission_FalseParam_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var agreementManagerMock = new Mock<IAgreementManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<bool>("ResolveIPIBaseNumber"))
                .ReturnsAsync(false);
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("AllowNonAffiliatedSubmissions")).ReturnsAsync(true);
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(new List<long>() { 139377839 }))
                .ReturnsAsync(new List<InterestedPartyModel>() {
                    new InterestedPartyModel() {
                        IpBaseNumber = "I-001696412-6",
                        Status = new StatusModel() {
                            ForwardingBaseNumber = "I-001629862-5",
                            StatusCode = IpStatus.DeletionCrossReference,
                            IpbaseNumber = "I-001696412-6"
                        },
                        Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } }
                    }
                });

			interestedPartyManagerMock.Setup(x => x.FollowChainForStatus(It.IsAny<InterestedPartyModel>()))
				.ReturnsAsync(new InterestedPartyModel()
				{
					Status = new StatusModel
					{
						 StatusCode = IpStatus.SelfReferencedValidIp,
						 ForwardingBaseNumber = "I-001629862-5",
						 IpbaseNumber = "I-001629862-5"
					}
				 });

			interestedPartyManagerMock.Setup(i => i.IsAuthoritative(It.IsAny<InterestedPartyModel>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(true);

            var test = new Mock<MD_16>(rulesManagerMock.Object, interestedPartyManagerMock.Object,
                agreementManagerMock.Object, GetMessagingManagerMock(ErrorCode._137));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel() {
                    IpBaseNumber = "I-001696412-6",
                    Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } },
                    CisacType = CisacInterestedPartyType.C
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(MD_16), test.Object.Identifier);
            Assert.Equal(ErrorCode._137, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check the transaction passes and new status 1 IP Base number is set if the submitter is ISWC eligible but not authoritative for the status 3 IP. Parameter = False
        /// </summary>
        [Fact]
        public async void MD_16_Submission_FalseParam_IswcEligibleNewIpBaseNumber()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var agreementManagerMock = new Mock<IAgreementManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<bool>("ResolveIPIBaseNumber"))
                .ReturnsAsync(false);
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("AllowNonAffiliatedSubmissions")).ReturnsAsync(true);
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(new List<long>() { 139377839 }))
                .ReturnsAsync(new List<InterestedPartyModel>() {
                    new InterestedPartyModel() {
                        IpBaseNumber = "I-001696412-6",
                        Status = new StatusModel() {
                            ForwardingBaseNumber = "I-001629862-5",
                            StatusCode = IpStatus.DeletionCrossReference,
                            IpbaseNumber = "I-001696412-6"
                        },
                        Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } }
                    }
                });

			interestedPartyManagerMock.Setup(x => x.FollowChainForStatus(It.IsAny<InterestedPartyModel>()))
				.ReturnsAsync(new InterestedPartyModel()
				{
					IpBaseNumber = "I-001629862-5",
					Names = new List<NameModel>() { new NameModel() { IpNameNumber = 139377839, TypeCode = NameType.PA } },
					Status = new StatusModel
					{
						StatusCode = IpStatus.SelfReferencedValidIp,
						ForwardingBaseNumber = "I-001629862-5",
						IpbaseNumber = "I-001629862-5"
					},
					CisacType = CisacInterestedPartyType.C,
					Type = InterestedPartyType.C
				});
			interestedPartyManagerMock.Setup(i => i.IsAuthoritative(It.IsAny<InterestedPartyModel>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(false);

            var test = new Mock<MD_16>(rulesManagerMock.Object, interestedPartyManagerMock.Object,
                agreementManagerMock.Object, GetMessagingManagerMock(ErrorCode._137));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel() {
                    IpBaseNumber = "I-001696412-6",
                    Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } },
                    CisacType = CisacInterestedPartyType.C
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_16), test.Object.Identifier);
            Assert.Equal("I-001629862-5", response.Submission.Model.InterestedParties.ElementAt(0).IpBaseNumber);
        }

        /// <summary>
        /// Checks that rule passes and new IpBaseNumber is set if ip has a status 1
        /// </summary>
        [Fact]
        public async void MD_16_Submission_FalseParam_NotStatus3_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var agreementManagerMock = new Mock<IAgreementManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<bool>("ResolveIPIBaseNumber"))
                .ReturnsAsync(false);
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(new List<long>() { 139377839 }))
                .ReturnsAsync(new List<InterestedPartyModel>() {
                    new InterestedPartyModel() {
                        IpBaseNumber = "I-001696412-6",
                        Status = new StatusModel() {
                            ForwardingBaseNumber = "I-001696412-6",
                            StatusCode = IpStatus.SelfReferencedValidIp,
                            IpbaseNumber = "I-001696412-6"
                        },
                        Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839, TypeCode = NameType.PA } }
                    }
                });

			interestedPartyManagerMock.Setup(i => i.IsAuthoritative(It.IsAny<InterestedPartyModel>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(false);

            var test = new Mock<MD_16>(rulesManagerMock.Object, interestedPartyManagerMock.Object,
                agreementManagerMock.Object, GetMessagingManagerMock(ErrorCode._137));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel() {
                    IpBaseNumber = "I-001696412-6",
                    Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839, TypeCode = NameType.PA } },
                    CisacType = CisacInterestedPartyType.C
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_16), test.Object.Identifier);
            Assert.Equal("I-001696412-6", response.Submission.Model.InterestedParties.ElementAt(0).IpBaseNumber);
        }

        /// <summary>
        /// Check that transaction passes if IP is status 3 and submitter is not ISWC eligible. Parameter = False
        /// </summary>
        [Fact]
        public async void MD_16_Submission_FalseParam_ISWCIneligible_NewIpBaseNumber()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var agreementManagerMock = new Mock<IAgreementManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<bool>("ResolveIPIBaseNumber"))
                .ReturnsAsync(false);
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(new List<long>() { 139377839 }))
                .ReturnsAsync(new List<InterestedPartyModel>() {
                    new InterestedPartyModel() {
                        IpBaseNumber = "I-001696412-6",
                        Status = new StatusModel() {
                            ForwardingBaseNumber = "I-001629862-5",
                            StatusCode = IpStatus.DeletionCrossReference,
                            IpbaseNumber = "I-001696412-6"
                        },
                        Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } }
                    }
                });

			interestedPartyManagerMock.Setup(x => x.FollowChainForStatus(It.IsAny<InterestedPartyModel>()))
				.ReturnsAsync(new InterestedPartyModel()
				{
					IpBaseNumber = "I-001629862-5",
					Names = new List<NameModel>() { new NameModel() { IpNameNumber = 139377839, TypeCode = NameType.PA } },
					Status = new StatusModel
					{
						StatusCode = IpStatus.SelfReferencedValidIp,
						ForwardingBaseNumber = "I-001629862-5",
						IpbaseNumber = "I-001629862-5"
					},
					CisacType = CisacInterestedPartyType.C,
					Type = InterestedPartyType.C
				});
			interestedPartyManagerMock.Setup(i => i.IsAuthoritative(It.IsAny<InterestedPartyModel>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(false);

            var test = new Mock<MD_16>(rulesManagerMock.Object, interestedPartyManagerMock.Object,
                agreementManagerMock.Object, GetMessagingManagerMock(ErrorCode._137));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel() {
                    IpBaseNumber = "I-001696412-6",
                    Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } },
                    CisacType = CisacInterestedPartyType.C
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_16), test.Object.Identifier);
            Assert.Equal("I-001629862-5", response.Submission.Model.InterestedParties.ElementAt(0).IpBaseNumber);
        }

        /// <summary>
        /// Check that the transaction passes if no Name Number for a CIQ transaction
        /// </summary>
        [Fact]
        public async void MD_16_Submission_TrueParam_CIQ_Transaction()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var agreementManagerMock = new Mock<IAgreementManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<bool>("ResolveIPIBaseNumber"))
                .ReturnsAsync(true);
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            var test = new Mock<MD_16>(rulesManagerMock.Object, interestedPartyManagerMock.Object,
                agreementManagerMock.Object, GetMessagingManagerMock(ErrorCode._137));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel() {
                    IpBaseNumber = "I-001696412-6",
                }
            };
            submission.TransactionType = Bdo.Edi.TransactionType.CIQ;

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_16), test.Object.Identifier);
        }

        /// <summary>
        /// Check that the transaction fails when new Ip status code is set to DeletionCrossReference
        /// </summary>
        [Fact]
        public async void MD_16_Submission_FalseParam_NewIpBaseNumber()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var agreementManagerMock = new Mock<IAgreementManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<bool>("ResolveIPIBaseNumber"))
                .ReturnsAsync(true);
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("ISWCEligibleRoles")).ReturnsAsync("(C,MA,TA),(E)");
            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(new List<long>() { 139377839 }))
                .ReturnsAsync(new List<InterestedPartyModel>() {
                    new InterestedPartyModel() {
                        IpBaseNumber = "I-001696412-6",
                        Status = new StatusModel() {
                            ForwardingBaseNumber = "I-001629862-5",
                            StatusCode = IpStatus.DeletionCrossReference,
                            IpbaseNumber = "I-001696412-6"
                        },
                        Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } }
                    }
                });

            interestedPartyManagerMock.Setup(x => x.FollowChainForStatus(It.IsAny<InterestedPartyModel>()))
                  .ReturnsAsync(new InterestedPartyModel()
                  {
                      IpBaseNumber = "I-001629862-5",
                      Names = new List<NameModel>() { new NameModel() { IpNameNumber = 139377839, TypeCode = NameType.PA } },
                      Status = new StatusModel
                      {
                          StatusCode = IpStatus.DeletionCrossReference,
                          ForwardingBaseNumber = "I-001629862-5",
                          IpbaseNumber = "I-001629862-5"
                      },
                      CisacType = CisacInterestedPartyType.C,
                      Type = InterestedPartyType.C
                  });

            var test = new Mock<MD_16>(rulesManagerMock.Object, interestedPartyManagerMock.Object,
                agreementManagerMock.Object, GetMessagingManagerMock(ErrorCode._137));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel() {
                    IpBaseNumber = "I-001696412-6",
                    Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } }
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(MD_16), test.Object.Identifier);
            Assert.Equal(ErrorCode._137, response.Submission.Rejection.Code);

        }
    }
}
