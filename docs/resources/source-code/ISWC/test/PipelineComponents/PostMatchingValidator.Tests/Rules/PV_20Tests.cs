using AutoMapper;
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
    /// Tests for rule PV_20
    /// </summary>
    public class PV_20Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if no matches are found for the preferred Iswc
        /// </summary>
        [Fact]
        public async void PV_20_NoMatchesFound_InValid()
        {
            var submission = new Submission()
            {
                TransactionType = TransactionType.CUR,
                MatchedResult = new MatchResult()
            };

            var workManagerMock = new Mock<IWorkManager>();
            var rulesmanagerMock = new Mock<IRulesManager>();
            var mapperMock = new Mock<IMapper>();

            var test = new Mock<PV_20>(GetMessagingManagerMock(ErrorCode._144), workManagerMock.Object, rulesmanagerMock.Object, mapperMock.Object).Object;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_20), test.Identifier);
        }


        /// <summary>
        /// Check transaction fails if the submission is ineligible and the preferred iswc from the submission does not match with the preferred iswc from the database
        /// Title on sub not on match
        /// </summary>
        [Fact]
        public async Task PV_20_PreferredIswcsDoNotMatchTitles_InValid()
        {
            const string agency = "1";
            const string number = "2";

            var mapperMock = new Mock<IMapper>();
            var rulesmanagerMock = new Mock<IRulesManager>();
            var workManagerMock = new Mock<IWorkManager>();

            var submission = new Submission()
            {
                TransactionType = TransactionType.CUR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() { CreatMatchingWork(1, agency, number) }
                },
                Model = new SubmissionModel
                {
                    Agency = agency,
                    WorkNumber = new WorkNumber() { Type = agency, Number = number },
                    Iswc = "T24454",
                    PreferredIswc = "T24454",
                    InterestedParties = new List<InterestedPartyModel>() {
                        new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber1",
                            CisacType = CisacInterestedPartyType.C
                        }
                    },
                    Titles = new List<Title>() {
                        new Title()
                        {
                            Name = "Title Name",
                            Type = TitleType.OT
                        },
                        new Title()
                        {
                            Name = "Name1",
                            Type = TitleType.AT
                        }
                    },
                }
            };

            workManagerMock.Setup(x => x.FindIswcModelAsync(submission.Model.WorkNumber, false, DetailLevel.Full)).ReturnsAsync(
                new Bdo.Iswc.IswcModel()
                {
                    Iswc = "T24454",
                }
            );

            mapperMock.Setup(m => m.Map<ISWCMetadataModel>(It.IsAny<IswcModel>()))
                .Returns(new ISWCMetadataModel()
                {
                    InterestedParties = new List<InterestedPartyModel>()
                            {
                                new InterestedPartyModel()
                                {
                                    IpBaseNumber = "IpBaseNumber1",
                                    CisacType = CisacInterestedPartyType.C
                                }
                            },
                    Titles = new List<Title>()
                            {
                                new Title()
                                {
                                    Name = "Title Name",
                                    Type = TitleType.OT
                                }
                            }
                });

            var test = new Mock<PV_20>(GetMessagingManagerMock(ErrorCode._144), workManagerMock.Object, rulesmanagerMock.Object, mapperMock.Object).Object;
            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
        }

        /// <summary>
        /// Check transaction fails if the submission is ineligible and the preferred iswc from the submission does not match with the preferred iswc from the database
        /// Different IPs
        /// </summary>
        [Fact]
        public async Task PV_20_PreferredIswcsDoNotMatchIPs_InValid()
        {
            const string agency = "1";
            const string number = "2";

            var mapperMock = new Mock<IMapper>();
            var rulesmanagerMock = new Mock<IRulesManager>();
            var workManagerMock = new Mock<IWorkManager>();

            var submission = new Submission()
            {
                TransactionType = TransactionType.CUR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() { CreatMatchingWork(1, agency, number) }
                },
                Model = new SubmissionModel
                {
                    Agency = agency,
                    WorkNumber = new WorkNumber() { Type = agency, Number = number },
                    Iswc = "T24454",
                    PreferredIswc = "T24454",
                    InterestedParties = new List<InterestedPartyModel>() {
                        new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber1",
                            CisacType = CisacInterestedPartyType.C
                        },
                        new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber2",
                            CisacType = CisacInterestedPartyType.C
                        },
                    },
                    Titles = new List<Title>() {
                        new Title()
                        {
                            Name = "Title Name",
                            Type = TitleType.OT
                        },
                    },
                }
            };

            workManagerMock.Setup(x => x.FindIswcModelAsync(submission.Model.WorkNumber, false, DetailLevel.Full)).ReturnsAsync(
                new Bdo.Iswc.IswcModel()
                {
                    Iswc = "T24454",
                }
            );

            mapperMock.Setup(m => m.Map<ISWCMetadataModel>(It.IsAny<IswcModel>()))
                .Returns(new ISWCMetadataModel()
                {
                    InterestedParties = new List<InterestedPartyModel>()
                            {
                                new InterestedPartyModel()
                                {
                                    IpBaseNumber = "IpBaseNumber1",
                                    CisacType = CisacInterestedPartyType.C
                                }
                            },
                    Titles = new List<Title>()
                            {
                                new Title()
                                {
                                    Name = "Title Name",
                                    Type = TitleType.OT
                                }
                            }
                });

            var test = new Mock<PV_20>(GetMessagingManagerMock(ErrorCode._144), workManagerMock.Object, rulesmanagerMock.Object, mapperMock.Object).Object;
            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
        }

        /// <summary>
        /// Check transaction passes if the submission is ineligible and the preferred iswc from the submission does not match with the preferred iswc from the database
        /// Different titles
        /// </summary>
        [Fact]
        public async Task PV_20_PreferredIswcsMatch_Valid()
        {
            const string agency = "1";
            const string number = "2";

            var mapperMock = new Mock<IMapper>();
            var rulesmanagerMock = new Mock<IRulesManager>();
            var workManagerMock = new Mock<IWorkManager>();

            var submission = new Submission()
            {
                TransactionType = TransactionType.CUR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() { CreatMatchingWork(1, agency, number) }
                },
                Model = new SubmissionModel
                {
                    Agency = agency,
                    WorkNumber = new WorkNumber() { Type = agency, Number = number },
                    Iswc = "T24454",
                    PreferredIswc = "T24454",
                    InterestedParties = new List<InterestedPartyModel>() {
                        new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber1",
                            CisacType = CisacInterestedPartyType.C
                        },
                        new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber2",
                            CisacType = CisacInterestedPartyType.C
                        }
                    },
                    Titles = new List<Title>() {
                        new Title()
                        {
                            Name = "Title Name",
                            Type = TitleType.OT
                        },
                    },
                }
            };

            workManagerMock.Setup(x => x.FindIswcModelAsync(submission.Model.WorkNumber, false, DetailLevel.Full)).ReturnsAsync(
                new Bdo.Iswc.IswcModel()
                {
                    Iswc = "T24454",
                }
            );

            mapperMock.Setup(m => m.Map<ISWCMetadataModel>(It.IsAny<IswcModel>()))
                .Returns(new ISWCMetadataModel()
                {
                    InterestedParties = new List<InterestedPartyModel>()
                    {
                        new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber1",
                            CisacType = CisacInterestedPartyType.C
                        },
                            new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber2",
                            CisacType = CisacInterestedPartyType.C
                        },
                        new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber2",
                            CisacType = CisacInterestedPartyType.C
                        }
                    },
                    Titles = new List<Title>()
                    {
                        new Title()
                        {
                            Name = "Title Name",
                            Type = TitleType.OT
                        },
                        new Title()
                        {
                            Name = "Hello",
                            Type = TitleType.AT
                        },
                    }
                });

            var test = new Mock<PV_20>(GetMessagingManagerMock(ErrorCode._144), workManagerMock.Object, rulesmanagerMock.Object, mapperMock.Object).Object;
            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
        }


        private MatchingWork CreatMatchingWork(long id, string agency, string number)
        {
            return new MatchingWork()
            {
                Id = id,
                Numbers = new List<WorkNumber>()
                {
                    new WorkNumber{ Type =  agency , Number=number }
                }
            };
        }

        /// <summary>
        /// Check transaction fails if the submission is ineligible and the preferred iswc from the submission does not match with the preferred iswc from the database
        /// Titles not set to match
        /// </summary>
        [Fact]
        public async Task PV_20_DoNotMatchTitles_InValid()
        {
            const string agency = "1";
            const string number = "2";

            var mapperMock = new Mock<IMapper>();
            var rulesmanagerMock = new Mock<IRulesManager>();
            var workManagerMock = new Mock<IWorkManager>();

            var submission = new Submission()
            {
                TransactionType = TransactionType.CUR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() { CreatMatchingWork(1, agency, number) }
                },
                Model = new SubmissionModel
                {
                    Agency = agency,
                    WorkNumber = new WorkNumber() { Type = agency, Number = number },
                    Iswc = "T24454",
                    PreferredIswc = "T24454",
                    InterestedParties = new List<InterestedPartyModel>() {
                        new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber1",
                            CisacType = CisacInterestedPartyType.C
                        }
                    },
                    Titles = new List<Title>() {
                        new Title()
                        {
                            Name = "Title Name",
                            Type = TitleType.OT
                        },
                        new Title()
                        {
                            Name = "Name1",
                            Type = TitleType.AT
                        }
                    },
                }
            };

            workManagerMock.Setup(x => x.FindIswcModelAsync(submission.Model.WorkNumber, false, DetailLevel.Full)).ReturnsAsync(
                new Bdo.Iswc.IswcModel()
                {
                    Iswc = "T24454",
                }
            );

            mapperMock.Setup(m => m.Map<ISWCMetadataModel>(It.IsAny<IswcModel>()))
                .Returns(new ISWCMetadataModel()
                {
                    InterestedParties = new List<InterestedPartyModel>()
                            {
                                new InterestedPartyModel()
                                {
                                    IpBaseNumber = "IpBaseNumber1",
                                    CisacType = CisacInterestedPartyType.C
                                }
                            },
                });

            var test = new Mock<PV_20>(GetMessagingManagerMock(ErrorCode._144), workManagerMock.Object, rulesmanagerMock.Object, mapperMock.Object).Object;
            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
        }

        /// <summary>
        /// FR - 6437 Check transaction fails if EnablePVTitleStandardization is enabled
        /// The submission is ineligible and the preferred iswc from the submission does not match with the preferred iswc from the database
        /// standardized titles don't match 
        /// </summary>
        [Fact]
        public async Task PV_20_StandardizeTitlesMatch_InValid()
        {
            const string agency = "1";
            const string number = "2";

            var mapperMock = new Mock<IMapper>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManagerMock = new Mock<IWorkManager>();

            var submission = new Submission()
            {
                IsEligible = false,
                TransactionType = TransactionType.CUR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() { CreatMatchingWork(1, agency, number) }
                },
                Model = new SubmissionModel
                {
                    Agency = agency,
                    WorkNumber = new WorkNumber() { Type = agency, Number = number },
                    Iswc = "T244545",
                    PreferredIswc = "T244545",
                    InterestedParties = new List<InterestedPartyModel>() {
                        new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber1",
                            CisacType = CisacInterestedPartyType.C
                        },
                        new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber2",
                            CisacType = CisacInterestedPartyType.C
                        }
                    },
                    Titles = new List<Title>() {
                        new Title()
                        {
                            Name = "Title Name",
                            Type = TitleType.OT
                        },
                    },
                }
            };

            workManagerMock.Setup(x => x.FindIswcModelAsync(submission.Model.WorkNumber, false, DetailLevel.Full)).ReturnsAsync(
                new Bdo.Iswc.IswcModel()
                {
                    Iswc = "T24454",
                }
            );

            mapperMock.Setup(m => m.Map<ISWCMetadataModel>(It.IsAny<IswcModel>()))
                .Returns(new ISWCMetadataModel() { });

            submission.MatchedResult.StandardizedName = "standardized title";
            submission.MatchedResult.Matches = new List<MatchingWork>
            {
                new MatchingWork
                {
                    Titles= new List<Title>
                    {
                        new Title
                        {
                            StandardizedName = "standardized title1",
                            Type = TitleType.OT,
                            Name = "Title Name"
                        }
                    },
                    Numbers = new List<WorkNumber>
                    {
                        new WorkNumber
                        {
                            Number = "T244545",
                            Type = "ISWC"
                        }
                    },
                    Contributors = new List<InterestedPartyModel>
                    {
                         new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber1",
                            CisacType = CisacInterestedPartyType.C,
                            ContributorType = ContributorType.Creator
                        },
                            new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber2",
                            CisacType = CisacInterestedPartyType.C,
                            ContributorType = ContributorType.Creator
                        }
                    }
                }
            };



            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("EnablePVTitleStandardization")).ReturnsAsync(true);

            var test = new Mock<PV_20>(GetMessagingManagerMock(ErrorCode._127), workManagerMock.Object, rulesManagerMock.Object, mapperMock.Object).Object;
            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            //  Assert.Equal(ErrorCode._127, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// FR - 6437
        /// Check transaction passes if the submission is ineligible and the preferred iswc from the submission does not match with the preferred iswc from the database
        /// standardized titles match
        /// </summary>
        [Fact]
        public async Task PV_20_StandardizeTitlesMatch_Valid()
        {
            const string agency = "1";
            const string number = "2";

            var mapperMock = new Mock<IMapper>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManagerMock = new Mock<IWorkManager>();

            var submission = new Submission()
            {
                IsEligible = false,
                TransactionType = TransactionType.CUR,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() { CreatMatchingWork(1, agency, number) }
                },
                Model = new SubmissionModel
                {
                    Agency = agency,
                    WorkNumber = new WorkNumber() { Type = agency, Number = number },
                    Iswc = "T244545",
                    PreferredIswc = "T244545",
                    InterestedParties = new List<InterestedPartyModel>() {
                        new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber1",
                            CisacType = CisacInterestedPartyType.C
                        },
                        new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber2",
                            CisacType = CisacInterestedPartyType.C
                        }
                    },
                    Titles = new List<Title>() {
                        new Title()
                        {
                            Name = "Title Name",
                            Type = TitleType.OT
                        },
                    },
                }
            };

            workManagerMock.Setup(x => x.FindIswcModelAsync(submission.Model.WorkNumber, false, DetailLevel.Full)).ReturnsAsync(
                new Bdo.Iswc.IswcModel()
                {
                    Iswc = "T24454",
                }
            );

            mapperMock.Setup(m => m.Map<ISWCMetadataModel>(It.IsAny<IswcModel>()))
                .Returns(new ISWCMetadataModel() { });

            submission.MatchedResult.StandardizedName = "standardized title";
            submission.MatchedResult.Matches = new List<MatchingWork>
            {
                new MatchingWork
                {
                    Titles= new List<Title>
                    {
                        new Title
                        {
                            StandardizedName = "standardized title",
                            Type = TitleType.OT,
                            Name = "Title Name"
                        }
                    },
                    Numbers = new List<WorkNumber>
                    {
                        new WorkNumber
                        {
                            Number = "T244545",
                            Type = "ISWC"
                        }
                    },
                    Contributors = new List<InterestedPartyModel>
                    {
                         new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber1",
                            CisacType = CisacInterestedPartyType.C,
                            ContributorType = ContributorType.Creator
                        },
                            new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber2",
                            CisacType = CisacInterestedPartyType.C,
                            ContributorType = ContributorType.Creator
                        }
                    },
                    StandardizedTitle =  "standardized title"
                }
            };



            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("EnablePVTitleStandardization")).ReturnsAsync(true);

            var test = new Mock<PV_20>(GetMessagingManagerMock(ErrorCode._127), workManagerMock.Object, rulesManagerMock.Object, mapperMock.Object).Object;
            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
        }


      
    }
}
