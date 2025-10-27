using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.MatchingEngine;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Tests.Rules
{
    /// <summary>
    /// Tests for rule PV_26
    /// </summary>
    public class PV_26Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if PreferredIswc is missing
        /// </summary>
        [Fact]
        public async void PV_26_MissingRequiredFields_InValid()
        {
            var submission = new Submission()
            {
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() {
                        new MatchingWork() { Id = 1, Numbers = new List<WorkNumber>() {
                            new WorkNumber { Type = "ISWC", Number = "12345" } },
                            Contributors = new List<InterestedPartyModel>() {
                                new InterestedPartyModel() {
                                    IpBaseNumber = "I-001696412-6",
                                    IPNameNumber = 139377839,
                                    Names = new List<NameModel>() { new NameModel() { IpNameNumber = 139377839 } }
                                },
                                new InterestedPartyModel() {
                                    IpBaseNumber = "I-001696412-6",
                                    IPNameNumber = 139377839,
                                    Names = new List<NameModel>() { new NameModel() { IpNameNumber = 139377839 } }
                                },
                            } },
                        new MatchingWork() { Id = 2 }
                    }
                },
                Model = new SubmissionModel()
                {
                    PreferredIswc = "",
                    WorkNumber = new WorkNumber()
                    {
                        Type = "1",
                        Number = "123"
                    },
                },
                TransactionType = TransactionType.CUR
            };

            var rulesManagerMock = new Mock<IRulesManager>();

            var test = new Mock<PV_26>(GetMessagingManagerMock(ErrorCode._116), rulesManagerMock.Object);
            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._116, response.Submission.Rejection.Code);
            Assert.Equal(nameof(PV_26), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction fails if workinfo record is not found by work number
        /// </summary>
        [Fact]
        public async void PV_26_WorkNotFoundByWorkNumber_InValid()
        {
            var submission = new Submission()
            {
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() {
                        new MatchingWork() { Id = 1, Numbers = new List<WorkNumber>() {
                            new WorkNumber { Type = "ISWC", Number = "123" } },
                            Contributors = new List<InterestedPartyModel>() {
                                new InterestedPartyModel() {
                                    IpBaseNumber = "I-001696412-6",
                                    IPNameNumber = 139377839,
                                    Names = new List<NameModel>() { new NameModel() { IpNameNumber = 139377839 } }
                                },
                                new InterestedPartyModel() {
                                    IpBaseNumber = "I-001696412-6",
                                    IPNameNumber = 139377839,
                                    Names = new List<NameModel>() { new NameModel() { IpNameNumber = 139377839 } }
                                },
                            } },
                        new MatchingWork() { Id = 2 }
                    }
                },
                Model = new SubmissionModel()
                {
                    PreferredIswc = "123",
                    WorkNumber = new WorkNumber()
                    {
                        Type = "1",
                        Number = "123"
                    },
                },
                TransactionType = TransactionType.CUR,
                ExistingWork = null
            };
            var rulesManagerMock = new Mock<IRulesManager>();

            var test = new Mock<PV_26>(GetMessagingManagerMock(ErrorCode._116), rulesManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._116, response.Submission.Rejection.Code);

        }

        /// <summary>
        /// Check transaction fails if work metadata does not match
        /// </summary>
        [Fact]
        public async void PV_26_WorkMetaDataDoesNotMatch_InValid()
        {
            var submission = new Submission()
            {
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() {
                        new MatchingWork() { Id = 1, Numbers = new List<WorkNumber>() {
                            new WorkNumber { Type = "ISWC", Number = "12345" } },
                            Contributors = new List<InterestedPartyModel>() {
                                new InterestedPartyModel() {
                                    IpBaseNumber = "I-001696412-6",
                                    IPNameNumber = 139377839,
                                    Names = new List<NameModel>() { new NameModel() { IpNameNumber = 139377839 } }
                                },
                                new InterestedPartyModel() {
                                    IpBaseNumber = "I-001696412-6",
                                    IPNameNumber = 139377839,
                                    Names = new List<NameModel>() { new NameModel() { IpNameNumber = 139377839 } }
                                },
                            } },
                        new MatchingWork() { Id = 2 }
                    }
                },
                Model = new SubmissionModel()
                {
                    PreferredIswc = "123",
                    WorkNumber = new WorkNumber()
                    {
                        Type = "1",
                        Number = "123"
                    },
                },
                TransactionType = TransactionType.CUR
            };
            var returnSubmissionModel = new SubmissionModel() { WorkNumber = new WorkNumber() { Type = "1", Number = "2" } };

            var rulesManagerMock = new Mock<IRulesManager>();

            var test = new Mock<PV_26>(GetMessagingManagerMock(ErrorCode._116), rulesManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._116, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if all required fields are present and work metadata matches
        /// </summary>
        [Fact]
        public async void PV_26_Valid()
        {
            var submission = new Submission()
            {
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() {
                        new MatchingWork() { Id = 1, Numbers = new List<WorkNumber>() {
                            new WorkNumber { Type = "ISWC", Number = "12345" } },
                            Contributors = new List<InterestedPartyModel>() {
                                new InterestedPartyModel() {
                                    IpBaseNumber = "I-001696412-6",
                                    IPNameNumber = 139377839,
                                    Names = new List<NameModel>() { new NameModel() { IpNameNumber = 139377839 } }
                                },
                                new InterestedPartyModel() {
                                    IpBaseNumber = "I-001696412-6",
                                    IPNameNumber = 139377839,
                                    Names = new List<NameModel>() { new NameModel() { IpNameNumber = 139377839 } }
                                },
                            } },
                        new MatchingWork() { Id = 2 }
                    }
                },
                Model = new SubmissionModel()
                {
                    PreferredIswc = "12345",
                    WorkNumber = new WorkNumber()
                    {
                        Type = "1",
                        Number = "123"
                    },
                },
                TransactionType = TransactionType.CUR,
                ExistingWork = new SubmissionModel()
                {
                    WorkNumber = new WorkNumber()
                    {
                        Type = "1",
                        Number = "123"
                    }
                }
            };


            var rulesManagerMock = new Mock<IRulesManager>();

            var test = new Mock<PV_26>(GetMessagingManagerMock(ErrorCode._116), rulesManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);

        }

        /// <summary>
        /// Check transaction fails preferred ISWC does not match
        /// </summary>
        [Fact]
        public async void PV_26_PreferredISWCDoesNotMatch_InValid()
        {
            var submission = new Submission()
            {
                MatchedResult = null,
                Model = new SubmissionModel()
                {
                    PreferredIswc = "123",
                    WorkNumber = new WorkNumber()
                    {
                        Type = "1",
                        Number = "123"
                    },
                },
                TransactionType = TransactionType.CUR
            };
            var returnSubmissionModel = new SubmissionModel() { WorkNumber = new WorkNumber() { Type = "1", Number = "2" } };

            var rulesManagerMock = new Mock<IRulesManager>();

            var test = new Mock<PV_26>(GetMessagingManagerMock(ErrorCode._116), rulesManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._116, response.Submission.Rejection.Code);
        }

        /// <summary>
        ///  FR 6437 - EnablePVTitleStandardization is enabled
        /// Check transaction passes if all required fields are present and work metadata matches
        /// standardize titles match
        /// </summary>
        [Fact]
        public async void PV_26_TitleStandardization_Valid()
        {
            var submission = new Submission()
            {
                IsEligible = false,
                MatchedResult = new MatchResult()
                {
                    StandardizedName = "standardized title",
                    Matches = new List<MatchingWork>() {
                        new MatchingWork() { Id = 1, Numbers = new List<WorkNumber>() {
                            new WorkNumber { Type = "ISWC", Number = "T12345" } },
                            Contributors = new List<InterestedPartyModel>() {
                                new InterestedPartyModel() {
                                    IpBaseNumber = "I-001696412-6",
                                    IPNameNumber = 139377839,
                                    ContributorType = ContributorType.Creator,
                                    Names = new List<NameModel>() { new NameModel() { IpNameNumber = 139377839 } }
                                }
                            },
                        Titles = new List<Title>
                        {
                            new Title
                            {
                                Type = TitleType.OT,
                                Name = "title"
                            }
                        },
                        StandardizedTitle = "standardized title"
                    },
                        new MatchingWork() { Id = 2 }
                    }
                },
                Model = new SubmissionModel()
                {
                    PreferredIswc = "T123456",
                    Titles = new List<Title>
                    {
                        new Title
                        {
                            Name = "la Title`"
                        }
                    },
                    WorkNumber = new WorkNumber()
                    {
                        Type = "1",
                        Number = "123"
                    },
                    InterestedParties = new List<InterestedPartyModel>
                        {
                            new InterestedPartyModel
                            {
                                IpBaseNumber = "I-001696412-6",
                                IPNameNumber = 139377839,
                                CisacType = CisacInterestedPartyType.C
                            }
                        }
                },
                TransactionType = TransactionType.CUR,
                ExistingWork = new SubmissionModel()
                {
                    WorkNumber = new WorkNumber()
                    {
                        Type = "1",
                        Number = "123"
                    }
                }
            };


            var rulesManagerMock = new Mock<IRulesManager>();
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("EnablePVTitleStandardization")).ReturnsAsync(true);

            var test = new Mock<PV_26>(GetMessagingManagerMock(ErrorCode._116), rulesManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
        }

        /// <summary>
        ///  FR 6437 - EnablePVTitleStandardization is enabled
        /// Check transaction fails if all required fields are present and work metadata does not match
        /// standardize titles do not match
        /// </summary>
        [Fact]
        public async void PV_26_TitleStandardization_InValid()
        {
            var submission = new Submission()
            {
                IsEligible = false,
                MatchedResult = new MatchResult()
                {
                    StandardizedName = "standardized title 1",
                    Matches = new List<MatchingWork>() {
                        new MatchingWork() { Id = 1, Numbers = new List<WorkNumber>() {
                            new WorkNumber { Type = "ISWC", Number = "T12345" } },
                            Contributors = new List<InterestedPartyModel>() {
                                new InterestedPartyModel() {
                                    IpBaseNumber = "I-001696412-6",
                                    IPNameNumber = 139377839,
                                    ContributorType = ContributorType.Creator,
                                    Names = new List<NameModel>() { new NameModel() { IpNameNumber = 139377839 } }
                                }
                            },
                        Titles = new List<Title>
                        {
                            new Title
                            {
                                Type = TitleType.OT,
                                Name = "la title"
                            }
                        },
                        StandardizedTitle = "standardized title"
                    },
                        new MatchingWork() { Id = 2 }
                    }
                },
                Model = new SubmissionModel()
                {
                    PreferredIswc = "T123456",
                    Titles = new List<Title>
                    {
                        new Title
                        {
                            Name = "la Title`"
                        }
                    },
                    WorkNumber = new WorkNumber()
                    {
                        Type = "1",
                        Number = "123"
                    },
                    InterestedParties = new List<InterestedPartyModel>
                        {
                            new InterestedPartyModel
                            {
                                IpBaseNumber = "I-001696412-6",
                                IPNameNumber = 139377839,
                                CisacType = CisacInterestedPartyType.C
                            }
                        }
                },
                TransactionType = TransactionType.CUR,
                ExistingWork = new SubmissionModel()
                {
                    WorkNumber = new WorkNumber()
                    {
                        Type = "1",
                        Number = "123"
                    }
                }
            };


            var rulesManagerMock = new Mock<IRulesManager>();
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("EnablePVTitleStandardization")).ReturnsAsync(true);

            var test = new Mock<PV_26>(GetMessagingManagerMock(ErrorCode._116), rulesManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
        }
    }
}
