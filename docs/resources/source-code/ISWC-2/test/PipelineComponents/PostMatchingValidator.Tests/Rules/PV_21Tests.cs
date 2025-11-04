using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.MatchingEngine;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule PV_21
    /// </summary>
    public class PV_21Tests : TestBase
    {
        /// <summary>
        /// Checks that transaction fails if no matches are found
        /// </summary>
        [Fact]
        public async Task PV_21_NoMatchesFound_Invalid()
        {
            var submission = new Submission()
            {
                TransactionType = TransactionType.CUR,
                MatchedResult = new MatchResult()
            };

            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>().Object;
            var workManagerMock = new Mock<IWorkManager>().Object;
            var rulesManagerMock = new Mock<IRulesManager>();

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            var test = new Mock<PV_21>(interestedPartyManagerMock, workManagerMock, GetMessagingManagerMock(ErrorCode._144), rulesManagerMock.Object).Object;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._144, response.Submission.Rejection.Code);
            Assert.Equal(nameof(PV_21), test.Identifier);
        }


        /// <summary>
        /// Checks that transaction passes if a single match is found
        /// </summary>
        [Fact]
        public async Task PV_21_SingleMatchFound_Valid()
        {
            var submission = new Submission()
            {
                TransactionType = TransactionType.CUR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() { new MatchingWork() { Id = 1 } }
                }
            };

            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>().Object;
            var workManagerMock = new Mock<IWorkManager>().Object;
            var rulesManagerMock = new Mock<IRulesManager>();

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");
            
            var test = new Mock<PV_21>(interestedPartyManagerMock, workManagerMock, GetMessagingManagerMock(ErrorCode._144), rulesManagerMock.Object).Object;

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Checks that transaction fails if multiple matches are found and submitter is not authoritative
        /// </summary>
        [Fact]
        public async Task PV_21_MultipleMatchesAndNotAuthoritative_Invalid()
        {
            var submission = new Submission()
            {
                TransactionType = TransactionType.CUR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() {
                        new MatchingWork() { Id = 1, Numbers = new List<WorkNumber>(){
                            new WorkNumber { Type =  "4", Number = "12345" },
                            new WorkNumber { Type =  "ISWC", Number = "12345" }},

                        Contributors = new List<InterestedPartyModel>(){
                        new InterestedPartyModel() {
                            IpBaseNumber = "I-001696412-6",
                            IPNameNumber = 139377839,
                            Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } }
                        },
                        new InterestedPartyModel() {
                            IpBaseNumber = "I-001696412-7",
                            IPNameNumber = 139377840,
                            Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377840 } }
                        },
                        } },
                        new MatchingWork() { Id = 2,Numbers = new List<WorkNumber>(){
                            new WorkNumber { Type =  "4", Number = "12345" },
                            new WorkNumber { Type =  "ISWC", Number = "12345" }},
                        Contributors = new List<InterestedPartyModel>(){
                        new InterestedPartyModel() {
                            IpBaseNumber = "I-001696412-6",
                            IPNameNumber = 139377839,
                            Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } }
                        },
                        new InterestedPartyModel() {
                            IpBaseNumber = "I-001696412-7",
                            IPNameNumber = 139377840,
                            Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377840 } }
                        },
                        } }
                    }
                },
                Model = new SubmissionModel()
                {
                    PreferredIswc = "12345",
                    WorkNumber = new WorkNumber { Type = "10", Number = "12345" },
                    InterestedParties = new List<InterestedPartyModel>() {
                        new InterestedPartyModel() {
                            IpBaseNumber = "I-001696412-6",
                            IPNameNumber = 139377839,
                            Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } }
                        }
                    }
                }
            };

            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
            var workManagerMock = new Mock<IWorkManager>();
            var rulesManagerMock = new Mock<IRulesManager>();

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            interestedPartyManagerMock
                .Setup(ip => ip.IsAuthoritative(It.IsAny<InterestedPartyModel>(), It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(false);
            workManagerMock
                .Setup(w => w.HasOtherEligibleWorks(submission.Model.PreferredIswc, submission.Model.Agency, true))
                .ReturnsAsync(true);

            workManagerMock.Setup(w => w.FindAsync(submission.Model.PreferredIswc, true, true))
                .ReturnsAsync(new SubmissionModel() { WorkNumber = new WorkNumber() { Type = "4" },
                    InterestedParties = new List<InterestedPartyModel>() {
                                new InterestedPartyModel() {
                                    IPNameNumber = 139377840,
                                    Type = InterestedPartyType.C,

                                },
                            }
                });

            workManagerMock.Setup(w => w.FindAsync(submission.Model.WorkNumber))
                .ReturnsAsync(new SubmissionModel()
                {
                    WorkNumber = new WorkNumber() { Type = "4" },
                    InterestedParties = new List<InterestedPartyModel>() {
                        new InterestedPartyModel() {
                            IpBaseNumber = "I-001696412-6",
                            IPNameNumber = 139377839,
                        }
                    }
                });

            var test = new Mock<PV_21>(interestedPartyManagerMock.Object, workManagerMock.Object, GetMessagingManagerMock(ErrorCode._145), rulesManagerMock.Object).Object;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._145, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Checks that transaction passes if multiple matches are found and submitter is authoritative
        /// </summary>
        [Fact]
        public async Task PV_21_MultipleMatchesAndAuthoritative_Valid()
        {
            var submission = new Submission()
            {
                TransactionType = TransactionType.CUR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() {
                        new MatchingWork() { Id = 1, Numbers = new List<WorkNumber>(){
                            new WorkNumber { Type =  "4" , Number = "12345" },
                            new WorkNumber { Type =  "ISWC", Number = "12345" }},
                        Contributors = new List<InterestedPartyModel>(){
                        new InterestedPartyModel() {
                            IpBaseNumber = "I-001696412-6",
                            IPNameNumber = 139377839,
                            Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } }
                        },
                        new InterestedPartyModel() {
                            IpBaseNumber = "I-001696412-6",
                            IPNameNumber = 139377839,
                            Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } }
                        },
                        } },
                        new MatchingWork() { Id = 2,Numbers = new List<WorkNumber>(){
                            new WorkNumber { Type =  "4", Number = "12345" },
                            new WorkNumber { Type =  "ISWC", Number = "12345" }},
                        Contributors = new List<InterestedPartyModel>(){
                        new InterestedPartyModel() {
                            IpBaseNumber = "I-001696412-6",
                            IPNameNumber = 139377839,
                            Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } }
                        },
                        new InterestedPartyModel() {
                            IpBaseNumber = "I-001696412-7",
                            IPNameNumber = 139377840,
                            Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377840 } }
                        },
                        } }
                    }
                },
                Model = new SubmissionModel()
                {
                    Agency = "4",
                    WorkNumber = new WorkNumber { Type = "4", Number = "12345" },
                    InterestedParties = new List<InterestedPartyModel>() {
                        new InterestedPartyModel() {
                            IpBaseNumber = "I-001696412-6",
                            IPNameNumber = 139377839,
                            Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } }
                        }
                    }
                }
            };

            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
            var workManagerMock = new Mock<IWorkManager>();
            var rulesManagerMock = new Mock<IRulesManager>();

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            interestedPartyManagerMock.Setup(ip => ip.IsAuthoritative(It.IsAny<InterestedPartyModel>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(true);
            workManagerMock.Setup(w => w.FindVerifiedAsync(It.IsAny<WorkNumber>())).ReturnsAsync(new VerifiedSubmissionModel() { IswcEligible = true, WorkNumber = new WorkNumber() { Type = "4" } });
            var test = new Mock<PV_21>(interestedPartyManagerMock.Object, workManagerMock.Object, GetMessagingManagerMock(ErrorCode._145), rulesManagerMock.Object).Object;

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Checks that transaction passes if multiple matches are found, submitter is not authoritative but IP was on most recent submission
        /// </summary>
        [Fact]
        public async Task PV_21_MultipleMatchesAndNotAuthoritative_Valid()
        {
            var submission = new Submission()
            {
                TransactionType = TransactionType.CUR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() {
                        new MatchingWork() { Id = 1, Numbers = new List<WorkNumber>(){
                            new WorkNumber { Type =  "4", Number = "12345" },
                            new WorkNumber { Type =  "ISWC", Number = "12345" }},

                        Contributors = new List<InterestedPartyModel>(){
                        new InterestedPartyModel() {
                            IpBaseNumber = "I-001696412-6",
                            IPNameNumber = 139377839,
                            Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } }
                        },
                        new InterestedPartyModel() {
                            IpBaseNumber = "I-001696412-7",
                            IPNameNumber = 139377840,
                            Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377840 } }
                        },
                        } },
                        new MatchingWork() { Id = 2,Numbers = new List<WorkNumber>(){
                            new WorkNumber { Type =  "4", Number = "12345" },
                            new WorkNumber { Type =  "ISWC", Number = "12345" }},
                        Contributors = new List<InterestedPartyModel>(){
                        new InterestedPartyModel() {
                            IpBaseNumber = "I-001696412-6",
                            IPNameNumber = 139377839,
                            Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } }
                        },
                        new InterestedPartyModel() {
                            IpBaseNumber = "I-001696412-7",
                            IPNameNumber = 139377840,
                            Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377840 } }
                        },
                        } }
                    }
                },
                Model = new SubmissionModel()
                {
                    PreferredIswc = "12345",
                    WorkNumber = new WorkNumber { Type = "10", Number = "12345" },
                    InterestedParties = new List<InterestedPartyModel>() {
                        new InterestedPartyModel() {
                            IpBaseNumber = "I-001696412-6",
                            IPNameNumber = 139377839,
                            Names = new List<NameModel>(){ new NameModel() { IpNameNumber = 139377839 } }
                        }
                    }
                }
            };

            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>();
            var workManagerMock = new Mock<IWorkManager>();
            var rulesManagerMock = new Mock<IRulesManager>();

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            interestedPartyManagerMock
                .Setup(ip => ip.IsAuthoritative(It.IsAny<InterestedPartyModel>(), It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(false);
            workManagerMock
                .Setup(w => w.HasOtherEligibleWorks(submission.Model.PreferredIswc, submission.Model.Agency, true))
                .ReturnsAsync(true);

            workManagerMock.Setup(w => w.FindAsync(submission.Model.PreferredIswc, true, true))
                .ReturnsAsync(new SubmissionModel()
                {
                    WorkNumber = new WorkNumber() { Type = "4" },
                    InterestedParties = new List<InterestedPartyModel>() {
                                new InterestedPartyModel() {
                                    IPNameNumber = 139377840,
                                    Type = InterestedPartyType.C,

                                },
                            }
                });

            workManagerMock.Setup(w => w.FindAsync(submission.Model.WorkNumber))
                .ReturnsAsync(new SubmissionModel()
                {
                    WorkNumber = new WorkNumber() { Type = "4" },
                    InterestedParties = new List<InterestedPartyModel>() {
                        new InterestedPartyModel() {
                            IPNameNumber = 139377840,
                            Type = InterestedPartyType.C,
                        }
                    }
                });

            var test = new Mock<PV_21>(interestedPartyManagerMock.Object, workManagerMock.Object, GetMessagingManagerMock(ErrorCode._145), rulesManagerMock.Object).Object;

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Checks that transaction passes preferred ISWC is null
        /// </summary>
        [Fact]
        public async Task PV_21_preferredISWC_null_valid()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel
                {
                    PreferredIswc = null
                },
                TransactionType = TransactionType.CUR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() { new MatchingWork() { Id = 1 } }
                }
            };

            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>().Object;
            var workManagerMock = new Mock<IWorkManager>().Object;
            var rulesManagerMock = new Mock<IRulesManager>();

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            var test = new Mock<PV_21>(interestedPartyManagerMock, workManagerMock, GetMessagingManagerMock(ErrorCode._144), rulesManagerMock.Object).Object;

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
        }

        /// <summary>
        /// 
        /// Checks that transaction fails if the submitting society is not eligible
        /// </summary>
        [Fact]
        public async Task PV_21_UpdateAllocatedIswc_InValid()
        {
            var submission = new Submission()
            {
                UpdateAllocatedIswc = true,
                IsEligible = false,
                TransactionType = TransactionType.CUR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() { new MatchingWork() { Id = 1 } }
                }
            };

            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>().Object;
            var workManagerMock = new Mock<IWorkManager>().Object;
            var rulesManagerMock = new Mock<IRulesManager>();

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            var test = new Mock<PV_21>(interestedPartyManagerMock, workManagerMock, GetMessagingManagerMock(ErrorCode._144), rulesManagerMock.Object).Object;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._144, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// 
        /// Checks that transaction fails if the submitting society is not eligible
        /// </summary>
        [Fact]
        public async Task PV_21_UpdateAllocatedIswc_Valid()
        {
            var submission = new Submission()
            {
                UpdateAllocatedIswc = true,
                IsEligible = true,
                TransactionType = TransactionType.CUR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() { new MatchingWork() { Id = 1 } }
                }
            };

            var interestedPartyManagerMock = new Mock<IInterestedPartyManager>().Object;
            var workManagerMock = new Mock<IWorkManager>().Object;
            var rulesManagerMock = new Mock<IRulesManager>();

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck")).ReturnsAsync("345(030, 098);308(030, 062, 086, 093, 219, 201, 189, 066)");

            var test = new Mock<PV_21>(interestedPartyManagerMock, workManagerMock, GetMessagingManagerMock(ErrorCode._144), rulesManagerMock.Object).Object;

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
        }
    }
}
