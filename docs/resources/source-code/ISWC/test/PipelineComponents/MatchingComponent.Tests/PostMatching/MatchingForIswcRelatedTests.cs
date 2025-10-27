using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.PostMatching;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.Tests.PostMatching
{
    /// <summary>
    /// Tests for Matching for ISWC Related - 3.10 in Specification Document
    /// </summary>
    public class MatchingForIswcRelatedTests : TestBase
    {
        /// <summary>
        /// When Derived From ISWCs are provided but no matches are found, submission should be rejected.
        /// </summary>
        [Fact]
        public async void DerivedFromIswc_NoMatches_Returns_Rejection()
        {
            var matchingManagerMock = new Mock<IMatchingManager>();
            var messagingManagerMock = new Mock<IMessagingManager>();

            var submissions = new List<Submission>() {
                new Submission() {
                    TransactionType = Bdo.Edi.TransactionType.CAR,
                    MatchedResult = null,
                    Rejection = null,
                    Model = new SubmissionModel(){
                        SourceDb = 300,
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 181375270,
                                IpBaseNumber = "I-001108569-9"
                            },
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 160831587,
                                IpBaseNumber = "I-001109742-8"
                            }
                        },
                        DerivedFrom = new List<DerivedFrom>()
                        {
                            new DerivedFrom
                            {
                                Iswc = "T0600646056",
                                Title = "STAY"
                            }
                        },
                        DerivedWorkType = DerivedWorkType.ModifiedVersion,
                        IswcsToMerge = new List<string>()
                        {
                            "T0616277190", "T0616835654", "T0616835665", "T0616835723"
                        },
                        WorkNumbersToMerge = new List<WorkNumber>()
                        {
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835654"
                            },
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835665"
                            },
                            new WorkNumber
                            {
                                Type = "WorkInfoID",
                                Number = "11840158725"
                            },
                            new WorkNumber
                            {
                                Type = "WIType4",
                                Number = "11840158731"
                            }
                        }
                    }
                }
            };

            IEnumerable<Submission> returnedSubmissions_NoMatchResult = new List<Submission>() {
                new Submission() {
                    TransactionType = Bdo.Edi.TransactionType.CAR,
                    MatchedResult = new Bdo.MatchingEngine.MatchResult() { Matches = new List<Bdo.MatchingEngine.MatchingWork>() },
                    Rejection = null,
                    Model = new SubmissionModel(){
                        SourceDb = 300,
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 181375270,
                                IpBaseNumber = "I-001108569-9"
                            },
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 160831587,
                                IpBaseNumber = "I-001109742-8"
                            }
                        },
                        DerivedFrom = new List<DerivedFrom>()
                        {
                            new DerivedFrom
                            {
                                Iswc = "T0600646056",
                                Title = "STAY"
                            }
                        },
                        DerivedWorkType = DerivedWorkType.ModifiedVersion,
                        IswcsToMerge = new List<string>()
                        {
                            "T0616277190", "T0616835654", "T0616835665", "T0616835723"
                        },
                        WorkNumbersToMerge = new List<WorkNumber>()
                        {
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835654"
                            },
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835665"
                            },
                            new WorkNumber
                            {
                                Type = "WorkInfoID",
                                Number = "11840158725"
                            },
                            new WorkNumber
                            {
                                Type = "WIType4",
                                Number = "11840158731"
                            }
                        }
                    }
                }
            };
            matchingManagerMock.Setup(x => x.MatchAsync(It.IsAny<IEnumerable<Submission>>(), It.IsAny<string>())).ReturnsAsync(returnedSubmissions_NoMatchResult);

            var test = new Mock<MatchingForIswcRelated>(matchingManagerMock.Object, GetMessagingManagerMock(ErrorCode._201)).Object;
            var result = await test.ProcessSubmissions(submissions);

            Assert.NotNull(result.FirstOrDefault().Rejection);
            Assert.Equal(ErrorCode._201, result.FirstOrDefault().Rejection.Code);
        }

        /// <summary>
        /// When Derived From ISWCs are provided and multiple matches are found, the submission should be rejected
        /// </summary>
        [Fact]
        public async void DerivedFromIswc_MultipleMatches_Returns_Rejection()
        {

            var matchingManagerMock = new Mock<IMatchingManager>();
            var messagingManagerMock = new Mock<IMessagingManager>();

            var submissions = new List<Submission>() {
                new Submission() {
                    TransactionType = Bdo.Edi.TransactionType.CAR,
                    MatchedResult = null,
                    Rejection = null,
                    Model = new SubmissionModel(){
                        SourceDb = 300,
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 181375270,
                                IpBaseNumber = "I-001108569-9"
                            },
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 160831587,
                                IpBaseNumber = "I-001109742-8"
                            }
                        },
                        DerivedFrom = new List<DerivedFrom>()
                        {
                            new DerivedFrom
                            {
                                Iswc = "T0600646056",
                                Title = "STAY"
                            }
                        },
                        DerivedWorkType = DerivedWorkType.ModifiedVersion,
                        IswcsToMerge = new List<string>()
                        {
                            "T0616277190", "T0616835654", "T0616835665", "T0616835723"
                        },
                        WorkNumbersToMerge = new List<WorkNumber>()
                        {
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835654"
                            },
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835665"
                            },
                            new WorkNumber
                            {
                                Type = "WorkInfoID",
                                Number = "11840158725"
                            },
                            new WorkNumber
                            {
                                Type = "WIType4",
                                Number = "11840158731"
                            }
                        }
                    }
                }
            };
            IEnumerable<Submission> returnedSubmissions_WithMatchResults = new List<Submission>() {
                new Submission() {
                    TransactionType = Bdo.Edi.TransactionType.CAR,
                    MatchedResult = new Bdo.MatchingEngine.MatchResult {
                        Matches = new List<Bdo.MatchingEngine.MatchingWork>
                        {
                            new Bdo.MatchingEngine.MatchingWork
                            {
                                Artists = new List<Performer>
                                {
                                    new Performer
                                    {
                                        FirstName = "Okay",
                                        LastName = "Dokay"
                                    },
                                    new Performer
                                    {
                                        FirstName = "2Get2",
                                        LastName = "TheOtherSide"
                                    }
                                },
                                Contributors = new List<InterestedPartyModel>
                                {
                                    new InterestedPartyModel
                                    {
                                        Agency = "350",
                                        IPNameNumber = 1234567,
                                        Names = new List<NameModel>
                                        {
                                            new NameModel { IpNameNumber = 1234567 },
                                            new NameModel { IpNameNumber = 7654321 }
                                        }
                                    },
                                    new InterestedPartyModel
                                    {
                                        Agency = "351",
                                        IPNameNumber = 1234888,
                                        Names = new List<NameModel>
                                        {
                                            new NameModel { IpNameNumber = 12347888 },
                                            new NameModel { IpNameNumber = 76541888 }
                                        }
                                    }
                                },
                                Id = 0,
                                MatchType = Bdo.MatchingEngine.MatchSource.MatchedByText
                            },
                            new Bdo.MatchingEngine.MatchingWork
                            {
                                Artists = new List<Performer>
                                {
                                    new Performer
                                    {
                                        FirstName = "Okay",
                                        LastName = "Dokay"
                                    },
                                    new Performer
                                    {
                                        FirstName = "2Get2",
                                        LastName = "TheOtherSide"
                                    }
                                },
                                Contributors = new List<InterestedPartyModel>
                                {
                                    new InterestedPartyModel
                                    {
                                        Agency = "350",
                                        IPNameNumber = 1234567,
                                        Names = new List<NameModel>
                                        {
                                            new NameModel { IpNameNumber = 1234567 },
                                            new NameModel { IpNameNumber = 7654321 }
                                        }
                                    },
                                    new InterestedPartyModel
                                    {
                                        Agency = "351",
                                        IPNameNumber = 1234888,
                                        Names = new List<NameModel>
                                        {
                                            new NameModel { IpNameNumber = 12347888 },
                                            new NameModel { IpNameNumber = 76541888 }
                                        }
                                    }
                                },
                                Id = 0,
                                MatchType = Bdo.MatchingEngine.MatchSource.MatchedByText
                            }
                        }
                    },
                    Rejection = null,
                    Model = new SubmissionModel(){
                        SourceDb = 300,
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 181375270,
                                IpBaseNumber = "I-001108569-9"
                            },
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 160831587,
                                IpBaseNumber = "I-001109742-8"
                            }
                        },
                        DerivedFrom = new List<DerivedFrom>()
                        {
                            new DerivedFrom
                            {
                                Iswc = "T0600646056",
                                Title = "STAY"
                            }
                        },
                        DerivedWorkType = DerivedWorkType.ModifiedVersion,
                        IswcsToMerge = new List<string>()
                        {
                            "T0616277190", "T0616835654", "T0616835665", "T0616835723"
                        },
                        WorkNumbersToMerge = new List<WorkNumber>()
                        {
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835654"
                            },
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835665"
                            },
                            new WorkNumber
                            {
                                Type = "WorkInfoID",
                                Number = "11840158725"
                            },
                            new WorkNumber
                            {
                                Type = "WIType4",
                                Number = "11840158731"
                            }
                        }
                    }
                }
            };

            matchingManagerMock.Setup(x => x.MatchAsync(It.IsAny<IEnumerable<Submission>>(), It.IsAny<string>())).ReturnsAsync(returnedSubmissions_WithMatchResults);

            var test = new Mock<MatchingForIswcRelated>(matchingManagerMock.Object, GetMessagingManagerMock(ErrorCode._201)).Object;
            var result = await test.ProcessSubmissions(submissions);

            Assert.NotNull(result.FirstOrDefault().Rejection);
            Assert.Equal(ErrorCode._201, result.FirstOrDefault().Rejection.Code);
        }

        /// <summary>
        /// When Merge ISWCs are provided but no matches are found, the submission should be rejected
        /// </summary>
        [Fact]
        public async void MergeIswc_NoMatches_Returns_Rejection()
        {
            var matchingManagerMock = new Mock<IMatchingManager>();
            var messagingManagerMock = new Mock<IMessagingManager>();

            var submissions = new List<Submission>() {
                new Submission() {
                    TransactionType = Bdo.Edi.TransactionType.MER,
                    MatchedResult = null,
                    Rejection = null,
                    Model = new SubmissionModel(){
                        SourceDb = 300,
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 181375270,
                                IpBaseNumber = "I-001108569-9"
                            },
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 160831587,
                                IpBaseNumber = "I-001109742-8"
                            }
                        },
                        DerivedFrom = new List<DerivedFrom>()
                        {
                            new DerivedFrom
                            {
                                Iswc = "T0600646056",
                                Title = "STAY"
                            }
                        },
                        DerivedWorkType = DerivedWorkType.ModifiedVersion,
                        IswcsToMerge = new List<string>()
                        {
                            "T0616277190", "T0616835654", "T0616835665", "T0616835723"
                        },
                        WorkNumbersToMerge = new List<WorkNumber>()
                        {
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835654"
                            },
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835665"
                            },
                            new WorkNumber
                            {
                                Type = "WorkInfoID",
                                Number = "11840158725"
                            },
                            new WorkNumber
                            {
                                Type = "WIType4",
                                Number = "11840158731"
                            }
                        }
                    }
                }
            };

            IEnumerable<Submission> returnedSubmissions_NoMatchResult = new List<Submission>() {
                new Submission() {
                    TransactionType = Bdo.Edi.TransactionType.MER,
                    MatchedResult = new Bdo.MatchingEngine.MatchResult() { Matches = new List<Bdo.MatchingEngine.MatchingWork>() },
                    Rejection = null,
                    Model = new SubmissionModel(){
                        SourceDb = 300,
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 181375270,
                                IpBaseNumber = "I-001108569-9"
                            },
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 160831587,
                                IpBaseNumber = "I-001109742-8"
                            }
                        },
                        DerivedFrom = new List<DerivedFrom>()
                        {
                            new DerivedFrom
                            {
                                Iswc = "T0600646056",
                                Title = "STAY"
                            }
                        },
                        DerivedWorkType = DerivedWorkType.ModifiedVersion,
                        IswcsToMerge = new List<string>()
                        {
                            "T0616277190", "T0616835654", "T0616835665", "T0616835723"
                        },
                        WorkNumbersToMerge = new List<WorkNumber>()
                        {
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835654"
                            },
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835665"
                            },
                            new WorkNumber
                            {
                                Type = "WorkInfoID",
                                Number = "11840158725"
                            },
                            new WorkNumber
                            {
                                Type = "WIType4",
                                Number = "11840158731"
                            }
                        }
                    }
                }
            };
            matchingManagerMock.Setup(x => x.MatchAsync(It.IsAny<IEnumerable<Submission>>(), It.IsAny<string>())).ReturnsAsync(returnedSubmissions_NoMatchResult);

            var test = new Mock<MatchingForIswcRelated>(matchingManagerMock.Object, GetMessagingManagerMock(ErrorCode._202)).Object;
            var result = await test.ProcessSubmissions(submissions);

            Assert.NotNull(result.FirstOrDefault().Rejection);
            Assert.Equal(ErrorCode._202, result.FirstOrDefault().Rejection.Code);
        }
        /// <summary>
        /// When Merge ISWCs are provided and multiple matches are found, the submission should be rejected
        /// </summary>
        [Fact]
        public async void MergeIswc_MultipleMatches_Returns_Rejection()
        {

            var matchingManagerMock = new Mock<IMatchingManager>();
            var messagingManagerMock = new Mock<IMessagingManager>();

            var submissions = new List<Submission>() {
                new Submission() {
                    TransactionType = Bdo.Edi.TransactionType.MER,
                    MatchedResult = null,
                    Rejection = null,
                    Model = new SubmissionModel(){
                        SourceDb = 300,
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 181375270,
                                IpBaseNumber = "I-001108569-9"
                            },
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 160831587,
                                IpBaseNumber = "I-001109742-8"
                            }
                        },
                        DerivedFrom = new List<DerivedFrom>()
                        {
                            new DerivedFrom
                            {
                                Iswc = "T0600646056",
                                Title = "STAY"
                            }
                        },
                        DerivedWorkType = DerivedWorkType.ModifiedVersion,
                        IswcsToMerge = new List<string>()
                        {
                            "T0616277190", "T0616835654", "T0616835665", "T0616835723"
                        }
                    }
                }
            };
            IEnumerable<Submission> returnedSubmissions_WithMatchResults = new List<Submission>() {
                new Submission() {
                    TransactionType = Bdo.Edi.TransactionType.MER,
                    MatchedResult = new Bdo.MatchingEngine.MatchResult {
                        Matches = new List<Bdo.MatchingEngine.MatchingWork>
                        {
                            new Bdo.MatchingEngine.MatchingWork
                            {
                                Artists = new List<Performer>
                                {
                                    new Performer
                                    {
                                        FirstName = "Okay",
                                        LastName = "Dokay"
                                    },
                                    new Performer
                                    {
                                        FirstName = "2Get2",
                                        LastName = "TheOtherSide"
                                    }
                                },
                                Contributors = new List<InterestedPartyModel>
                                {
                                    new InterestedPartyModel
                                    {
                                        Agency = "350",
                                        IPNameNumber = 1234567,
                                        Names = new List<NameModel>
                                        {
                                            new NameModel { IpNameNumber = 1234567 },
                                            new NameModel { IpNameNumber = 7654321 }
                                        }
                                    },
                                    new InterestedPartyModel
                                    {
                                        Agency = "351",
                                        IPNameNumber = 1234888,
                                        Names = new List<NameModel>
                                        {
                                            new NameModel { IpNameNumber = 12347888 },
                                            new NameModel { IpNameNumber = 76541888 }
                                        }
                                    }
                                },
                                Id = 0,
                                MatchType = Bdo.MatchingEngine.MatchSource.MatchedByText
                            },
                            new Bdo.MatchingEngine.MatchingWork
                            {
                                Artists = new List<Performer>
                                {
                                    new Performer
                                    {
                                        FirstName = "Okay",
                                        LastName = "Dokay"
                                    },
                                    new Performer
                                    {
                                        FirstName = "2Get2",
                                        LastName = "TheOtherSide"
                                    }
                                },
                                Contributors = new List<InterestedPartyModel>
                                {
                                    new InterestedPartyModel
                                    {
                                        Agency = "350",
                                        IPNameNumber = 1234567,
                                        Names = new List<NameModel>
                                        {
                                            new NameModel { IpNameNumber = 1234567 },
                                            new NameModel { IpNameNumber = 7654321 }
                                        }
                                    },
                                    new InterestedPartyModel
                                    {
                                        Agency = "351",
                                        IPNameNumber = 1234888,
                                        Names = new List<NameModel>
                                        {
                                            new NameModel { IpNameNumber = 12347888 },
                                            new NameModel { IpNameNumber = 76541888 }
                                        }
                                    }
                                },
                                Id = 0,
                                MatchType = Bdo.MatchingEngine.MatchSource.MatchedByText
                            }
                        }
                    },
                    Rejection = null,
                    Model = new SubmissionModel(){
                        SourceDb = 300,
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 181375270,
                                IpBaseNumber = "I-001108569-9"
                            },
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 160831587,
                                IpBaseNumber = "I-001109742-8"
                            }
                        },
                        DerivedFrom = new List<DerivedFrom>()
                        {
                            new DerivedFrom
                            {
                                Iswc = "T0600646056",
                                Title = "STAY"
                            }
                        },
                        DerivedWorkType = DerivedWorkType.ModifiedVersion,
                        IswcsToMerge = new List<string>()
                        {
                            "T0616277190", "T0616835654", "T0616835665", "T0616835723"
                        }
                    }
                }
            };

            matchingManagerMock.Setup(x => x.MatchAsync(It.IsAny<IEnumerable<Submission>>(), It.IsAny<string>())).ReturnsAsync(returnedSubmissions_WithMatchResults);

            var test = new Mock<MatchingForIswcRelated>(matchingManagerMock.Object, GetMessagingManagerMock(ErrorCode._202)).Object;
            var result = await test.ProcessSubmissions(submissions);

            Assert.NotNull(result.FirstOrDefault().Rejection);
            Assert.Equal(ErrorCode._202, result.FirstOrDefault().Rejection.Code);

        }

        /// <summary>
        ///  When Agency Work Codes are provided for merging and no matches are found, the submission should be rejected
        /// </summary>
        [Fact]
        public async void WorkNumber_NoMatches_Returns_Rejection()
        {
            var matchingManagerMock = new Mock<IMatchingManager>();
            var messagingManagerMock = new Mock<IMessagingManager>();

            var submissions = new List<Submission>() {
                new Submission() {
                    TransactionType = Bdo.Edi.TransactionType.MER,
                    MatchedResult = null,
                    Rejection = null,
                    Model = new SubmissionModel(){
                        SourceDb = 300,
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 181375270,
                                IpBaseNumber = "I-001108569-9"
                            },
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 160831587,
                                IpBaseNumber = "I-001109742-8"
                            }
                        },
                        DerivedFrom = new List<DerivedFrom>()
                        {
                            new DerivedFrom
                            {
                                Iswc = "T0600646056",
                                Title = "STAY"
                            }
                        },
                        DerivedWorkType = DerivedWorkType.ModifiedVersion,
                        WorkNumbersToMerge = new List<WorkNumber>()
                        {
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835654"
                            },
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835665"
                            },
                            new WorkNumber
                            {
                                Type = "WorkInfoID",
                                Number = "11840158725"
                            },
                            new WorkNumber
                            {
                                Type = "WIType4",
                                Number = "11840158731"
                            }
                        }
                    }
                }
            };

            IEnumerable<Submission> returnedSubmissions_NoMatchResult = new List<Submission>() {
                new Submission() {
                    TransactionType = Bdo.Edi.TransactionType.MER,
                    MatchedResult = new Bdo.MatchingEngine.MatchResult() { Matches = new List<Bdo.MatchingEngine.MatchingWork>() },
                    Rejection = null,
                    Model = new SubmissionModel(){
                        SourceDb = 300,
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 181375270,
                                IpBaseNumber = "I-001108569-9"
                            },
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 160831587,
                                IpBaseNumber = "I-001109742-8"
                            }
                        },
                        DerivedFrom = new List<DerivedFrom>()
                        {
                            new DerivedFrom
                            {
                                Iswc = "T0600646056",
                                Title = "STAY"
                            }
                        },
                        DerivedWorkType = DerivedWorkType.ModifiedVersion,
                        WorkNumbersToMerge = new List<WorkNumber>()
                        {
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835654"
                            },
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835665"
                            },
                            new WorkNumber
                            {
                                Type = "WorkInfoID",
                                Number = "11840158725"
                            },
                            new WorkNumber
                            {
                                Type = "WIType4",
                                Number = "11840158731"
                            }
                        }
                    }
                }
            };
            matchingManagerMock.Setup(x => x.MatchAsync(It.IsAny<IEnumerable<Submission>>(), It.IsAny<string>())).ReturnsAsync(returnedSubmissions_NoMatchResult);

            var test = new Mock<MatchingForIswcRelated>(matchingManagerMock.Object, GetMessagingManagerMock(ErrorCode._203)).Object;
            var result = await test.ProcessSubmissions(submissions);

            Assert.NotNull(result.FirstOrDefault().Rejection);
            Assert.Equal(ErrorCode._203, result.FirstOrDefault().Rejection.Code);
        }

        /// <summary>
        ///  When Agency Work Codes are provided for merging and multiple matches are found, the submission should be rejected
        /// </summary>
        [Fact]
        public async void WorkNumber_MultipleMatches_Returns_Rejection()
        {
            var matchingManagerMock = new Mock<IMatchingManager>();
            var messagingManagerMock = new Mock<IMessagingManager>();

            var submissions = new List<Submission>() {
                new Submission() {
                    TransactionType = Bdo.Edi.TransactionType.MER,
                    MatchedResult = null,
                    Rejection = null,
                    Model = new SubmissionModel(){
                        SourceDb = 300,
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 181375270,
                                IpBaseNumber = "I-001108569-9"
                            },
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 160831587,
                                IpBaseNumber = "I-001109742-8"
                            }
                        },
                        DerivedFrom = new List<DerivedFrom>()
                        {
                            new DerivedFrom
                            {
                                Iswc = "T0600646056",
                                Title = "STAY"
                            }
                        },
                        DerivedWorkType = DerivedWorkType.ModifiedVersion,
                        WorkNumbersToMerge = new List<WorkNumber>()
                        {
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835654"
                            },
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835665"
                            },
                            new WorkNumber
                            {
                                Type = "WorkInfoID",
                                Number = "11840158725"
                            },
                            new WorkNumber
                            {
                                Type = "WIType4",
                                Number = "11840158731"
                            }
                        }
                    }
                }
            };

            IEnumerable<Submission> returnedSubmissions_WithMatchResults = new List<Submission>() {
                new Submission() {
                    TransactionType = Bdo.Edi.TransactionType.MER,
                    MatchedResult = new Bdo.MatchingEngine.MatchResult {
                        Matches = new List<Bdo.MatchingEngine.MatchingWork>
                        {
                            new Bdo.MatchingEngine.MatchingWork
                            {
                                Artists = new List<Performer>
                                {
                                    new Performer
                                    {
                                        FirstName = "Okay",
                                        LastName = "Dokay"
                                    },
                                    new Performer
                                    {
                                        FirstName = "2Get2",
                                        LastName = "TheOtherSide"
                                    }
                                },
                                Contributors = new List<InterestedPartyModel>
                                {
                                    new InterestedPartyModel
                                    {
                                        Agency = "350",
                                        IPNameNumber = 1234567,
                                        Names = new List<NameModel>
                                        {
                                            new NameModel { IpNameNumber = 1234567 },
                                            new NameModel { IpNameNumber = 7654321 }
                                        }
                                    },
                                    new InterestedPartyModel
                                    {
                                        Agency = "351",
                                        IPNameNumber = 1234888,
                                        Names = new List<NameModel>
                                        {
                                            new NameModel { IpNameNumber = 12347888 },
                                            new NameModel { IpNameNumber = 76541888 }
                                        }
                                    }
                                },
                                Id = 0,
                                MatchType = Bdo.MatchingEngine.MatchSource.MatchedByText
                            },
                            new Bdo.MatchingEngine.MatchingWork
                            {
                                Artists = new List<Performer>
                                {
                                    new Performer
                                    {
                                        FirstName = "Okay",
                                        LastName = "Dokay"
                                    },
                                    new Performer
                                    {
                                        FirstName = "2Get2",
                                        LastName = "TheOtherSide"
                                    }
                                },
                                Contributors = new List<InterestedPartyModel>
                                {
                                    new InterestedPartyModel
                                    {
                                        Agency = "350",
                                        IPNameNumber = 1234567,
                                        Names = new List<NameModel>
                                        {
                                            new NameModel { IpNameNumber = 1234567 },
                                            new NameModel { IpNameNumber = 7654321 }
                                        }
                                    },
                                    new InterestedPartyModel
                                    {
                                        Agency = "351",
                                        IPNameNumber = 1234888,
                                        Names = new List<NameModel>
                                        {
                                            new NameModel { IpNameNumber = 12347888 },
                                            new NameModel { IpNameNumber = 76541888 }
                                        }
                                    }
                                },
                                Id = 0,
                                MatchType = Bdo.MatchingEngine.MatchSource.MatchedByText
                            }
                        }
                    },
                    Rejection = null,
                    Model = new SubmissionModel(){
                        SourceDb = 300,
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 181375270,
                                IpBaseNumber = "I-001108569-9"
                            },
                            new InterestedPartyModel
                            {
                                Name ="STANKEVICS",
                                IPNameNumber = 160831587,
                                IpBaseNumber = "I-001109742-8"
                            }
                        },
                        DerivedFrom = new List<DerivedFrom>()
                        {
                            new DerivedFrom
                            {
                                Iswc = "T0600646056",
                                Title = "STAY"
                            }
                        },
                        DerivedWorkType = DerivedWorkType.ModifiedVersion,
                        WorkNumbersToMerge = new List<WorkNumber>()
                        {
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835654"
                            },
                            new WorkNumber
                            {
                                Type = "ISWC",
                                Number = "T0616835665"
                            },
                            new WorkNumber
                            {
                                Type = "WorkInfoID",
                                Number = "11840158725"
                            },
                            new WorkNumber
                            {
                                Type = "WIType4",
                                Number = "11840158731"
                            }
                        }
                    }
                }
            };

            matchingManagerMock.Setup(x => x.MatchAsync(It.IsAny<IEnumerable<Submission>>(), It.IsAny<string>())).ReturnsAsync(returnedSubmissions_WithMatchResults);

            var test = new Mock<MatchingForIswcRelated>(matchingManagerMock.Object, GetMessagingManagerMock(ErrorCode._203)).Object;
            var result = await test.ProcessSubmissions(submissions);

            Assert.NotNull(result.FirstOrDefault().Rejection);
            Assert.Equal(ErrorCode._203, result.FirstOrDefault().Rejection.Code);

        }

    }
}
