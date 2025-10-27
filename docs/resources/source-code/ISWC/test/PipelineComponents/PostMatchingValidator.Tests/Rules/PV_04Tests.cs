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
    /// Tests rule PV_04
    /// </summary>
    public class PV_04Tests : TestBase
    {
        /// <summary>
        /// Check rule passes if no matches have been found.
        /// </summary>
        [Fact]
        public async void PV_04_NoMatchesToCheck_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CAR, SubmissionId = 1 };
            var rulesManagerMock = new Mock<IRulesManager>();
            var test = new Mock<PV_04>(GetMessagingManagerMock(ErrorCode._127), rulesManagerMock.Object);
            var response = await test.Object.IsValid(submission);
            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check transaction fails if submitted IP Name Numbers do not match current ISWC metadata
        /// </summary>
        [Fact]
        public async void PV_04_IPNumbersDoNotMatch_InValid()
        {
            var matchedResult = new MatchResult();
            matchedResult.Matches = new List<MatchingWork>() {
                new MatchingWork() {
                    Contributors = new List<InterestedPartyModel>() {
                        new InterestedPartyModel() { IpBaseNumber = "12345678", Type = InterestedPartyType.E, IPNameNumber = 5, ContributorType = ContributorType.Publisher },
                        new InterestedPartyModel() { IpBaseNumber = "12345679",Type = InterestedPartyType.CA, IPNameNumber = 3, ContributorType = ContributorType.Creator}
                    },
                    Titles = new List<Title>(){ new Title() { Name = "title", Type = TitleType.AT } },
                    Numbers = new WorkNumber[]{ new WorkNumber { Number="T1234", Type = "ISWC"} }
                }
            };

            var submissionModel = new SubmissionModel()
            {
                PreferredIswc = "T1234",
                Titles = new List<Title>() { new Title() { Name = "title1", StandardizedName = "TITLE", Type = TitleType.AT } },
                InterestedParties = new List<InterestedPartyModel>() {
                    new InterestedPartyModel() { IpBaseNumber = "12345678", Type = InterestedPartyType.E },
                    new InterestedPartyModel() { IpBaseNumber = "12345677", Type = InterestedPartyType.CA }
                }
            };

            var submission = new Submission() { Model = submissionModel, TransactionType = TransactionType.CAR, SubmissionId = 1, MatchedResult = matchedResult };
            var rulesManagerMock = new Mock<IRulesManager>();
            rulesManagerMock.Setup(x => x.GetParameterValueEnumerable<TitleType>("ExcludeTitleTypes")).ReturnsAsync(new List<TitleType> { TitleType.CT });

            var test = new Mock<PV_04>(GetMessagingManagerMock(ErrorCode._247), rulesManagerMock.Object);
            var response = await test.Object.IsValid(submission);
            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._247, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if submitted titles do not match current ISWC metadata
        /// </summary>
        [Fact]
        public async void PV_04_TitlesDoNotMatch_InValid()
        {
            var matchedResult = new MatchResult();
            matchedResult.Matches = new List<MatchingWork>() {
                new MatchingWork() {
                    Numbers = new List<WorkNumber>
                    {
                        new WorkNumber{ Number = "2", Type = "ISWC"}
                    },
                    Contributors = new List<InterestedPartyModel>() {
                        new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.C},
                        new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.AM}
                    },
                    Titles = new List<Title>(){ new Title() { Name = "title", Type = TitleType.AT } }
                }
            };

            var submissionModel = new SubmissionModel()
            {
                PreferredIswc = "2",
                Titles = new List<Title>() { new Title() { Name = "TITLE1", StandardizedName = "TITLE1", Type = TitleType.AT } },
                InterestedParties = new List<InterestedPartyModel>() {
                    new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.C },
                    new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.AM }
                }
            };

            var submission = new Submission() { Model = submissionModel, TransactionType = TransactionType.CAR, SubmissionId = 1, MatchedResult = matchedResult };
            var rulesManagerMock = new Mock<IRulesManager>().Object;
            var test = new Mock<PV_04>(GetMessagingManagerMock(ErrorCode._127), rulesManagerMock);
            var response = await test.Object.IsValid(submission);
            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._127, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if submitted title types do not match current ISWC metadata
        /// </summary>
        [Fact]
        public async void PV_04_TitleTypeDoesNotMatch_InValid()
        {
            var matchedResult = new MatchResult();
            matchedResult.Matches = new List<MatchingWork>() {
                new MatchingWork() {
                    Numbers = new List<WorkNumber>
                    {
                        new WorkNumber{ Number = "2", Type = "ISWC"}
                    },
                    Contributors = new List<InterestedPartyModel>() {
                        new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.C},
                        new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.AM}
                    },
                    Titles = new List<Title>(){ new Title() { Name = "tItle", Type = TitleType.AT } }
                }
            };
            var submissionModel = new SubmissionModel()
            {
                PreferredIswc = "2",
                Titles = new List<Title>() { new Title() { Name = "titLE", StandardizedName = "TITLE", Type = TitleType.ET } },
                InterestedParties = new List<InterestedPartyModel>() {
                    new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.C },
                    new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.AM }
                }
            };
            var submission = new Submission() { Model = submissionModel, TransactionType = TransactionType.CAR, SubmissionId = 1, MatchedResult = matchedResult };
            var rulesManagerMock = new Mock<IRulesManager>().Object;
            var test = new Mock<PV_04>(GetMessagingManagerMock(ErrorCode._127), rulesManagerMock);
            var response = await test.Object.IsValid(submission);
            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._127, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if all submitted IP and title data match current ISWC metadata
        /// </summary>
        [Fact]
        public async void PV_04_Valid()
        {
            var matchedResult = new MatchResult();
            matchedResult.Matches = new List<MatchingWork>() {
                new MatchingWork() {
                    Numbers = new List<WorkNumber>
                    {
                        new WorkNumber{ Number = "2", Type = "ISWC"}
                    },
                    Contributors = new List<InterestedPartyModel>() {
                        new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.C},
                        new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.AM}
                    },
                    Titles = new List<Title>(){ new Title() { Name = "TITlE", Type = TitleType.AT } }
                }
            };
            var submissionModel = new SubmissionModel()
            {
                PreferredIswc = "2",
                Titles = new List<Title>() { new Title() { Name = "tItlE", StandardizedName = "TITLE", Type = TitleType.AT } },
                InterestedParties = new List<InterestedPartyModel>() {
                    new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.C },
                    new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.AM }
                }
            };
            var submission = new Submission() { Model = submissionModel, TransactionType = TransactionType.CAR, SubmissionId = 1, MatchedResult = matchedResult };
            var rulesManagerMock = new Mock<IRulesManager>().Object;
            var test = new Mock<PV_04>(GetMessagingManagerMock(ErrorCode._127), rulesManagerMock).Object;
            var response = await test.IsValid(submission);
            Assert.Equal(nameof(PV_04), test.Identifier);
            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check that rule is run if multiple matches found, but all matches have same ISWC
        /// </summary>
        [Fact]
        public async void PV_04_RunIfOnlyOneIswcInMatches()
        {
            var matchedResult = new MatchResult();
            matchedResult.Matches = new List<MatchingWork>() {
                new MatchingWork() {
                    Numbers = new List<WorkNumber>
                    {
                        new WorkNumber{ Number = "T0101629860", Type="ISWC"}
                    },
                    Contributors = new List<InterestedPartyModel>() {
                        new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.C, ContributorType = ContributorType.Creator},
                        new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.AM, ContributorType = ContributorType.Creator}
                    },
                    Titles = new List<Title>(){ new Title() { Name = "title", Type = TitleType.AT } }
                },
                new MatchingWork() {
                    Numbers = new List<WorkNumber>
                    {
                        new WorkNumber{ Number = "T0101629860", Type="ISWC"}
                    },
                    Contributors = new List<InterestedPartyModel>() {
                        new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.C, ContributorType = ContributorType.Creator},
                        new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.AM, ContributorType = ContributorType.Creator}
                    },
                    Titles = new List<Title>(){ new Title() { Name = "title", Type = TitleType.AT } }
                }
            };

            var submissionModel = new SubmissionModel()
            {
                PreferredIswc = "T0101629860",
                Titles = new List<Title>() { new Title() { Name = "tItlE", StandardizedName = "TITLE", Type = TitleType.AT } },
                InterestedParties = new List<InterestedPartyModel>() {
                    new InterestedPartyModel() { IpBaseNumber = "1234", CisacType = CisacInterestedPartyType.C, ContributorType = ContributorType.Creator }
                }
            };

            var submission = new Submission() { Model = submissionModel, TransactionType = TransactionType.CAR, SubmissionId = 1, MatchedResult = matchedResult };
            var rulesManagerMock = new Mock<IRulesManager>().Object;
            var test = new Mock<PV_04>(GetMessagingManagerMock(ErrorCode._247), rulesManagerMock);
            var response = await test.Object.IsValid(submission);
            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._247, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if submitted titles do not match current ISWC metadata but PreviewDisambiguation is set to true
        /// </summary>
        [Fact]
        public async void PV_04_TitlesDoNotMatchPreviewDisambiguationTrue_Valid()
        {
            var matchedResult = new MatchResult();
            matchedResult.Matches = new List<MatchingWork>() {
                new MatchingWork() {
                    Numbers = new List<WorkNumber>
                    {
                        new WorkNumber{ Number = "2", Type = "ISWC"}
                    },
                    Contributors = new List<InterestedPartyModel>() {
                        new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.C},
                        new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.AM}
                    },
                    Titles = new List<Title>(){ new Title() { Name = "title", Type = TitleType.AT } }
                }
            };

            var submissionModel = new SubmissionModel()
            {
                PreferredIswc = "2",
                Titles = new List<Title>() { new Title() { Name = "TITLE1", StandardizedName = "TITLE1", Type = TitleType.AT } },
                InterestedParties = new List<InterestedPartyModel>() {
                    new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.C },
                    new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.AM }
                },
                PreviewDisambiguation = true
            };

            var submission = new Submission() { Model = submissionModel, TransactionType = TransactionType.CAR, SubmissionId = 1, MatchedResult = matchedResult };
            var rulesManagerMock = new Mock<IRulesManager>().Object;
            var test = new Mock<PV_04>(GetMessagingManagerMock(ErrorCode._127), rulesManagerMock).Object;
            var response = await test.IsValid(submission);
            Assert.Equal(nameof(PV_04), test.Identifier);
            Assert.True(response.IsValid);
        }


        /// <summary>
        ///  FR - 6437 EnablePVTitleStandardization is enabled
        ///  Check transaction passes if standardized titles match 
        /// </summary>
        [Fact]
        public async void PV_04_PVTitleStandardization_Valid()
        {
            var matchedResult = new MatchResult();
            matchedResult.StandardizedName = "title";
            matchedResult.Matches = new List<MatchingWork>() {
                new MatchingWork() {
                    Numbers = new List<WorkNumber>
                    {
                        new WorkNumber{ Number = "2", Type = "ISWC"}
                    },
                    Contributors = new List<InterestedPartyModel>() {
                        new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.C},
                        new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.AM}
                    },
                    Titles = new List<Title>(){ new Title() { Name = "title'", Type = TitleType.OT} },
                    StandardizedTitle = "title"
                },
            };

            var submissionModel = new SubmissionModel()
            {
                PreferredIswc = "2",
                Titles = new List<Title>() { new Title() { Name = "TITLE1", StandardizedName = "TITLE1", Type = TitleType.OT } },
                InterestedParties = new List<InterestedPartyModel>() {
                    new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.C },
                    new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.AM }
                }
            };

            var submission = new Submission() { IsEligible = false, Model = submissionModel, TransactionType = TransactionType.CAR, SubmissionId = 1, MatchedResult = matchedResult };

            var rulesManagerMock = new Mock<IRulesManager>();

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("EnablePVTitleStandardization")).ReturnsAsync(true);

            var test = new Mock<PV_04>(GetMessagingManagerMock(ErrorCode._127), rulesManagerMock.Object);
            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
        }

        /// <summary>
        ///  FR - 6437 EnablePVTitleStandardization is enabled
        ///   Check transaction fails if standardized titles do not match 
        /// </summary>
        [Fact]
        public async void PV_04_PVTitleStandardization_InValid()
        {
            var matchedResult = new MatchResult();
            matchedResult.StandardizedName = "title";
            matchedResult.Matches = new List<MatchingWork>() {
                new MatchingWork() {
                    Numbers = new List<WorkNumber>
                    {
                        new WorkNumber{ Number = "2", Type = "ISWC"}
                    },
                    Contributors = new List<InterestedPartyModel>() {
                        new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.C},
                        new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.AM}
                    },
                    Titles = new List<Title>(){ new Title() { Name = "titlea'", Type = TitleType.OT, StandardizedName = "titlea"} },
                    StandardizedTitle = "title 1"
                },
            };

            var submissionModel = new SubmissionModel()
            {
                PreferredIswc = "2",
                Titles = new List<Title>() { new Title() { Name = "TITLE1", StandardizedName = "TITLE1", Type = TitleType.OT } },
                InterestedParties = new List<InterestedPartyModel>() {
                    new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.C },
                    new InterestedPartyModel() { IpBaseNumber = "12345678", CisacType = CisacInterestedPartyType.AM }
                }
            };

            var submission = new Submission() { IsEligible = false, Model = submissionModel, TransactionType = TransactionType.CAR, SubmissionId = 1, MatchedResult = matchedResult };
            var rulesManagerMock = new Mock<IRulesManager>();
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("EnablePVTitleStandardization")).ReturnsAsync(true);

            var test = new Mock<PV_04>(GetMessagingManagerMock(ErrorCode._127), rulesManagerMock.Object);
            var response = await test.Object.IsValid(submission);
            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._127, response.Submission.Rejection.Code);
        }
    }
}