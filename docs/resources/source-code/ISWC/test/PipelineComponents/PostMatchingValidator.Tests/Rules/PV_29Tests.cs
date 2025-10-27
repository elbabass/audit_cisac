using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.MatchingEngine;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Tests.Rules
{
    /// <summary>
    /// Tests for rule PV_29
    /// </summary>
    public class PV_29Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if submission is not Iswc eligible and there are no matching works
        /// </summary>
        [Fact]
        public async void PV_29_NotIswcEligibleAndNoMatchingWorks_Invalid()
        {
            var submission = new Submission()
            {
                IsEligible = false,
                Model = new SubmissionModel()
                {
                    PreferredIswc = "123456"
                },
                TransactionType = TransactionType.CAR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() { }
                }
            };

            var test = new Mock<PV_29>(GetMessagingManagerMock(ErrorCode._140)).Object;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_29), test.Identifier);
            Assert.Equal(TransactionType.CAR, response.Submission.TransactionType);
            Assert.Equal(ErrorCode._140, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if submission is not Iswc eligible and there are multiple matching works
        /// </summary>
        [Fact]
        public async void PV_29_NotIswcEligibleAndMultipleMatchingWorks_Invalid()
        {
            var submission = new Submission()
            {
                IsEligible = false,
                Model = new SubmissionModel()
                {
                    PreferredIswc = "123456"
                },
                TransactionType = TransactionType.CAR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() {
                        new MatchingWork(){
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber()
                                {
                                    Type = "ISWC",
                                    Number = "T0000000000"
                                }
                            },
                            Contributors = new List<InterestedPartyModel>()
                            {
                                new InterestedPartyModel(){}
                            }
                        },
                        new MatchingWork(){
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber()
                                {
                                    Type = "ISWC",
                                    Number = "T1111111111"
                                }
                            },
                            Contributors = new List<InterestedPartyModel>()
                            {
                                new InterestedPartyModel(){}
                            }
                        },
                    }
                }
            };

            var test = new Mock<PV_29>(GetMessagingManagerMock(ErrorCode._160)).Object;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_29), test.Identifier);
            Assert.Equal(TransactionType.CAR, response.Submission.TransactionType);
            Assert.Equal(ErrorCode._160, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if submission is Iswc eligible
        /// </summary>
        [Fact]
        public async void PV_29_Valid()
        {
            var submission = new Submission()
            {
                IsEligible = true,
                Model = new SubmissionModel()
                {
                    PreferredIswc = "123456"
                },
                TransactionType = TransactionType.CAR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() { new MatchingWork() }
                }
            };

            var test = new Mock<PV_29>(GetMessagingManagerMock(ErrorCode._140)).Object;

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_29), test.Identifier);
            Assert.Equal(TransactionType.CAR, response.Submission.TransactionType);
            Assert.Null(response.Submission.Rejection);
        }


        /// <summary>
        /// Check transaction passes if submission is not Iswc eligible and there is one match
        /// </summary>
        [Fact]
        public async void PV_29_OneMatch_Valid()
        {
            var submission = new Submission()
            {
                IsEligible = false,
                Model = new SubmissionModel()
                {
                    PreferredIswc = "123456"
                },
                TransactionType = TransactionType.CAR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() {
                        new MatchingWork() {
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber()
                                {
                                    Type = "ISWC",
                                    Number = "T0000000000"
                                }
                            },
                            Contributors = new List<InterestedPartyModel>()
                            {
                                new InterestedPartyModel(){}
                            }

                        },
                        new MatchingWork() {
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber()
                                {
                                    Type = "ISWC",
                                    Number = "T0000000000"
                                }
                            },
                            Contributors = new List<InterestedPartyModel>()
                            {
                                new InterestedPartyModel(){}
                            }

                        },
                    }
                }
            };

            var test = new Mock<PV_29>(GetMessagingManagerMock(ErrorCode._140)).Object;

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_29), test.Identifier);
            Assert.Equal(TransactionType.CAR, response.Submission.TransactionType);
            Assert.Null(response.Submission.Rejection);
        }


        /// <summary>
        /// Check transaction fails if submission is not Iswc eligible and there is no ISWC type matches
        /// </summary>
        [Fact]
        public async void PV_29_NoMatchNoIswcMatches_InValid()
        {
            var submission = new Submission()
            {
                IsEligible = false,
                Model = new SubmissionModel()
                {
                    PreferredIswc = "123456"
                },
                TransactionType = TransactionType.CAR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() {
                        new MatchingWork() {
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber()
                                {
                                    Type = "058",
                                    Number = "425325534"
                                }
                            },
                            Contributors = new List<InterestedPartyModel>()
                            {
                                new InterestedPartyModel(){}
                            }

                        },
                        new MatchingWork() {
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber()
                                {
                                    Type = "058",
                                    Number = "43256654"
                                }
                            },
                            Contributors = new List<InterestedPartyModel>()
                            {
                                new InterestedPartyModel(){}
                            }

                        },
                    }
                }
            };

            var test = new Mock<PV_29>(GetMessagingManagerMock(ErrorCode._140)).Object;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_29), test.Identifier);
            Assert.Equal(TransactionType.CAR, response.Submission.TransactionType);
            Assert.Equal(ErrorCode._140, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if transaction is Resolution and there are no ISWC type matches
        /// </summary>
        [Fact]
        public async void PV_29_Resolution_NoMatchNoIswcMatches_InValid()
        {
            var submission = new Submission()
            {
                IsEligible = false,
                Model = new SubmissionModel()
                {
                    PreferredIswc = "123456"
                },
                TransactionType = TransactionType.FSQ,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() {
                        new MatchingWork() {
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber()
                                {
                                    Type = "058",
                                    Number = "425325534"
                                }
                            },
                            Contributors = new List<InterestedPartyModel>()
                            {
                                new InterestedPartyModel(){}
                            }

                        },
                        new MatchingWork() {
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber()
                                {
                                    Type = "058",
                                    Number = "43256654"
                                }
                            },
                            Contributors = new List<InterestedPartyModel>()
                            {
                                new InterestedPartyModel(){}
                            }

                        },
                    }
                }
            };

            var test = new Mock<PV_29>(GetMessagingManagerMock(ErrorCode._163)).Object;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_29), test.Identifier);
            Assert.Equal(TransactionType.FSQ, response.Submission.TransactionType);
            Assert.Equal(ErrorCode._163, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if transaction is Resolution and there are multiple matching works
        /// </summary>
        [Fact]
        public async void PV_29_Resolution_MultipleMatchingWorks_Invalid()
        {
            var submission = new Submission()
            {
                IsEligible = false,
                Model = new SubmissionModel()
                {
                    PreferredIswc = "123456"
                },
                TransactionType = TransactionType.FSQ,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() {
                        new MatchingWork(){
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber()
                                {
                                    Type = "ISWC",
                                    Number = "T0000000000"
                                }
                            },
                            Contributors = new List<InterestedPartyModel>()
                            {
                                new InterestedPartyModel(){}
                            }
                        },
                        new MatchingWork(){
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber()
                                {
                                    Type = "ISWC",
                                    Number = "T1111111111"
                                }
                            },
                            Contributors = new List<InterestedPartyModel>()
                            {
                                new InterestedPartyModel(){}
                            }
                        },
                    }
                }
            };

            var test = new Mock<PV_29>(GetMessagingManagerMock(ErrorCode._164)).Object;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_29), test.Identifier);
            Assert.Equal(TransactionType.FSQ, response.Submission.TransactionType);
            Assert.Equal(ErrorCode._164, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if submission is eligible and preferred ISWC is null
        /// </summary>
        [Fact]
        public async void PV_29_Valid_PreferredISWC_null()
        {
            var submission = new Submission()
            {
                IsEligible = true,
                Model = new SubmissionModel()
                {
                    PreferredIswc = null
                },
                TransactionType = TransactionType.CAR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() { new MatchingWork() }
                }
            };

            var test = new Mock<PV_29>(GetMessagingManagerMock(ErrorCode._140)).Object;

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_29), test.Identifier);
            Assert.Equal(TransactionType.CAR, response.Submission.TransactionType);
            Assert.Null(response.Submission.Rejection);
        }

        /// <summary>
        /// Check transaction fails if submission is not Iswc eligible and there are no matching works. FSQ transaction
        /// </summary>
        [Fact]
        public async void PV_29_NotIswcEligibleAndNoMatchingWorks_Invalid_FSQ()
        {
            var submission = new Submission()
            {
                IsEligible = false,
                Model = new SubmissionModel()
                {
                    PreferredIswc = "123456"
                },
                TransactionType = TransactionType.FSQ,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() { }
                }
            };

            var test = new Mock<PV_29>(GetMessagingManagerMock(ErrorCode._163)).Object;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_29), test.Identifier);
            Assert.Equal(TransactionType.FSQ, response.Submission.TransactionType);
            Assert.Equal(ErrorCode._163, response.Submission.Rejection.Code);
        }
    }
}
