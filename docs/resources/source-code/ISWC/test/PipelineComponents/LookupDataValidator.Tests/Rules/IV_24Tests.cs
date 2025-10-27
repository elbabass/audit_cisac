using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.LookupDataValidator.Rules;
using System;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.LookupDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule IV_24
    /// </summary>
    public class IV_24Tests : TestBase
    {
        /// <summary>
        /// Check that transaction passes if submission does not have all PD Creators. Parameter = True
        /// </summary>
        [Fact]
        public async void IV_24_Submission_TrueParam_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<bool>("AllowPDWorkSubmissions")).ReturnsAsync(true);

            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(It.IsAny<IEnumerable<long>>()))
              .ReturnsAsync(new List<InterestedPartyModel>() {
                    new InterestedPartyModel() {
                        IpBaseNumber = "I-000004772-1",
                        Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 23899459 } },
                        DeathDate = DateTime.Parse("1906-03-01 00:00:00")
                    },
                    new InterestedPartyModel() {
                        IpBaseNumber = "I-002296966-8",
                        Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 473321567 } },
                    },
                    new InterestedPartyModel() {
                        IpBaseNumber = "I-000000015-5",
                        Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139855142 } },
                        DeathDate = DateTime.Parse("2019-03-01 00:00:00")
                    }
              });


            var test = new Mock<IV_24>(rulesManagerMock.Object, interestedPartyManagerMock.Object, GetMessagingManagerMock(ErrorCode._108));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel(){
                    Names = new List<NameModel>{ new NameModel { IpNameNumber = 23899459 } },
                    IpBaseNumber ="basenumber"
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_24), test.Object.Identifier);
        }

        /// <summary>
        /// Check that transaction fails if submission contains all PD creators who are all identified using the generic IPs (Public Domain, DP, TRAD or Unknown Composer Author)
        /// </summary>
        [Fact]
        public async void IV_24_Submission_TrueParam_AllPublicDomainCreators_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<bool>("AllowPDWorkSubmissions")).ReturnsAsync(true);

            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(It.IsAny<IEnumerable<long>>()))
              .ReturnsAsync(new List<InterestedPartyModel>() {
                    new InterestedPartyModel() {
                        IpBaseNumber = CommonIPs.PublicDomain,
                        Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 23899459 } }
                    },
                    new InterestedPartyModel() {
                        IpBaseNumber = CommonIPs.TRAD,
                        Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 473321567 } },
                    }
              });


            var test = new Mock<IV_24>(rulesManagerMock.Object, interestedPartyManagerMock.Object, GetMessagingManagerMock(ErrorCode._108));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel(){
                    Names = new List<NameModel>{ new NameModel { IpNameNumber = 23899459 } },
                    IpBaseNumber ="basenumber"
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_24), test.Object.Identifier);
            Assert.Equal(ErrorCode._108, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check that transaction passes if submission contains all PD creators, at least one of which is not one of the generic IPs (Public Domain, DP, TRAD, Unknown Composer Author)
        /// </summary>
        [Fact]
        public async void IV_24_Submission_TrueParam_AllPublicDomainCreators_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<bool>("AllowPDWorkSubmissions")).ReturnsAsync(true);

            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(It.IsAny<IEnumerable<long>>()))
              .ReturnsAsync(new List<InterestedPartyModel>() {
                    new InterestedPartyModel() {
                        IpBaseNumber = "I-000004772-1",
                        Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 23899459 } },
                        DeathDate = DateTime.Parse("1906-03-01 00:00:00")
                    },
                    new InterestedPartyModel() {
                        IpBaseNumber = CommonIPs.TRAD,
                        Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 473321567 } },
                    }
              });


            var test = new Mock<IV_24>(rulesManagerMock.Object, interestedPartyManagerMock.Object, GetMessagingManagerMock(ErrorCode._108));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel(){
                    Names = new List<NameModel>{ new NameModel { IpNameNumber = 23899459 } },
                    IpBaseNumber ="basenumber"
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_24), test.Object.Identifier);
        }

        /// <summary>
        /// Check that transaction passes if submission does not have all PD Creators. Parameter = Fase
        /// </summary>
        [Fact]
        public async void IV_24_Submission_FalseParam_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<bool>("AllowPDWorkSubmissions")).ReturnsAsync(false);

            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(It.IsAny<IEnumerable<long>>()))
              .ReturnsAsync(new List<InterestedPartyModel>() {
                    new InterestedPartyModel() {
                        IpBaseNumber = "I-000004772-1",
                        Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 23899459 } },
                        DeathDate = DateTime.Parse("1906-03-01 00:00:00")
                    },
                    new InterestedPartyModel() {
                        IpBaseNumber = "I-002296966-8",
                        Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 473321567 } },
                    },
                    new InterestedPartyModel() {
                        IpBaseNumber = "I-000000015-5",
                        Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139855142 } },
                        DeathDate = DateTime.Parse("2019-03-01 00:00:00")
                    }
              });


            var test = new Mock<IV_24>(rulesManagerMock.Object, interestedPartyManagerMock.Object, GetMessagingManagerMock(ErrorCode._108));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel(){
                    Names = new List<NameModel>{ new NameModel { IpNameNumber = 23899459 } },
                    IpBaseNumber ="basenumber"
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_24), test.Object.Identifier);
        }

        /// <summary>
        /// Check that transaction fails if submission's creators are all PD. Parameter = False
        /// </summary>
        [Fact]
        public async void IV_24_Submission_FalseParam_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<bool>("AllowPDWorkSubmissions")).ReturnsAsync(false);

            interestedPartyManagerMock.Setup(i => i.FindManyByNameNumber(It.IsAny<IEnumerable<long>>()))
              .ReturnsAsync(new List<InterestedPartyModel>() {
                    new InterestedPartyModel() {
                        IpBaseNumber = "I-000004772-1",
                        Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 23899459 } },
                        DeathDate = DateTime.Parse("1906-03-01 00:00:00")
                    }
              });


            var test = new Mock<IV_24>(rulesManagerMock.Object, interestedPartyManagerMock.Object, GetMessagingManagerMock(ErrorCode._108));

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel(){
                    Names = new List<NameModel>{ new NameModel { IpNameNumber = 23899459 } },
                    IpBaseNumber ="basenumber"
                }
            };

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_24), test.Object.Identifier);
            Assert.Equal(ErrorCode._108, response.Submission.Rejection.Code);
        }      
    }
}
