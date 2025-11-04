using AutoMapper;
using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.MatchingEngine;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Data.Services.Matching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Business.Tests.Managers
{
    /// <summary>
    /// Tests for the Matching Manager
    /// </summary>
    public class MatchingManagerTests
    {
        /// <summary>
        /// Verifies that the RankMatches method returns an Ienumerable of submissions and sets the ranking score for a single match
        /// </summary>
        [Fact]
        public async Task Ranking_CalculatesRankScore()
        {
            var matchingService = new Mock<IMatchingService>().Object;
            var workManagerMock = new Mock<IWorkManager>().Object;
            var iswcManagerMock = new Mock<IIswcRepository>().Object;
            var mapperMock = new Mock<IMapper>().Object;
            var rulesManagerMock = new Mock<IRulesManager>();
            var workRepositoryMock = new Mock<IWorkRepository>().Object;
            var additionalIdentifierManagerMock = new Mock<IAdditionalIdentifierManager>().Object;

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("CalculateRolledUpRole"))
                .ReturnsAsync("C(C, CA, A);MA(AR, SR);TA(AD, SA, TR);E(E, AM)");

            var matchingManager = new Mock<MatchingManager>(matchingService, workManagerMock, iswcManagerMock, mapperMock, rulesManagerMock.Object, workRepositoryMock, additionalIdentifierManagerMock).Object;

            var work1 = new MatchingWork()
            {
                Id = 0,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.OT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.E } }
            };
            var work2 = new MatchingWork()
            {
                Id = 0,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.OT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.CA } }
            };

            var testSubmission = new List<Submission>() { new Submission()
                {
                    Model = new SubmissionModel()
                    {
                        Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name  = "Test Title", Type = Bdo.Work.TitleType.OT } },
                        InterestedParties = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C } }
                    },
                    MatchedResult = new MatchResult(){ Matches = new List<MatchingWork>(){work1,work2} }
                }
            };

            var response = (await matchingManager.RankMatches(testSubmission));

            Assert.IsAssignableFrom<IEnumerable<Submission>>(response);
            Assert.Equal(work2, response.ElementAt(0).MatchedResult.Matches.ElementAt(0));
        }

        /// <summary>
        /// Verifies that the RankMatches method ranks matches wtih no titles
        /// </summary>
        [Fact]
        public async Task Ranking_With_No_Title_CalculatesRankScore()
        {
            var matchingService = new Mock<IMatchingService>().Object;
            var workManagerMock = new Mock<IWorkManager>().Object;
            var iswcManagerMock = new Mock<IIswcRepository>().Object;
            var mapperMock = new Mock<IMapper>().Object;
            var rulesManagerMock = new Mock<IRulesManager>();
            var workRepositoryMock = new Mock<IWorkRepository>().Object;
            var additionalIdentifierManagerMock = new Mock<IAdditionalIdentifierManager>().Object;

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("CalculateRolledUpRole"))
                .ReturnsAsync("C(C, CA, A);MA(AR, SR);TA(AD, SA, TR);E(E, AM)");

            var matchingManager = new Mock<MatchingManager>(matchingService, workManagerMock, iswcManagerMock, mapperMock, rulesManagerMock.Object, workRepositoryMock, additionalIdentifierManagerMock).Object;

            var work1 = new MatchingWork()
            {
                Id = 1,
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123" } }
            };
            var work2 = new MatchingWork()
            {
                Id = 2,
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "125" }, new InterestedPartyModel() { IpBaseNumber = "124" }
                , new InterestedPartyModel() { IpBaseNumber = "123" } }
            };
            var work3 = new MatchingWork()
            {
                Id = 3,
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "126" }, new InterestedPartyModel() { IpBaseNumber = "124" } }
            };
            var work4 = new MatchingWork()
            {
                Id = 4,
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "124" }, new InterestedPartyModel() { IpBaseNumber = "123" } }
            };

            var testSubmission = new List<Submission>() { new Submission()
                {
                    Model = new SubmissionModel()
                    {
                        Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name  = "Test Title", Type = Bdo.Work.TitleType.OT } },
                        InterestedParties = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123" } }
                    },
                    MatchedResult = new MatchResult(){ Matches = new List<MatchingWork>(){work1, work2, work3, work4} }
                }
            };

            var response = (await matchingManager.RankMatches(testSubmission));

            Assert.IsAssignableFrom<IEnumerable<Submission>>(response);
            Assert.Equal(work1, response.ElementAt(0).MatchedResult.Matches.ElementAt(0));
            Assert.Equal(work2, response.ElementAt(0).MatchedResult.Matches.ElementAt(1));
            Assert.Equal(work4, response.ElementAt(0).MatchedResult.Matches.ElementAt(2));
            Assert.Equal(work3, response.ElementAt(0).MatchedResult.Matches.ElementAt(3));
        }

        /// <summary>
        /// Verifies that the RankMatches method returns an Ienumerable of submissions and sorts by ranking score for multiple matches
        /// </summary>
        [Fact]
        public async Task Ranking_CalculatesRankScore_MultipleMatches()
        {
            var matchingService = new Mock<IMatchingService>().Object;
            var workManagerMock = new Mock<IWorkManager>().Object;
            var iswcManagerMock = new Mock<IIswcRepository>().Object;
            var mapperMock = new Mock<IMapper>().Object;
            var rulesManagerMock = new Mock<IRulesManager>();
            var workRepositoryMock = new Mock<IWorkRepository>().Object;
            var additionalIdentifierManagerMock = new Mock<IAdditionalIdentifierManager>().Object;

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("CalculateRolledUpRole"))
                .ReturnsAsync("C(C, CA, A);MA(AR, SR);TA(AD, SA, TR);E(E, AM)");

            var matchingManager = new Mock<MatchingManager>(matchingService, workManagerMock, iswcManagerMock, mapperMock, rulesManagerMock.Object, workRepositoryMock, additionalIdentifierManagerMock).Object;

            var work1 = new MatchingWork()
            {
                Id = 1,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "TeST TiTLE", Type = Bdo.Work.TitleType.OT } },
                Contributors = new List<InterestedPartyModel>() {
                    new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C }
                }
            };
            var work2 = new MatchingWork()
            {
                Id = 2,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "TEST TiTLE", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() {
                    new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C },
                    new InterestedPartyModel() { IpBaseNumber = "234", Type = InterestedPartyType.C }
                }
            };
            var work3 = new MatchingWork()
            {
                Id = 3,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "TEST TiTLE", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() {
                    new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C },
                    new InterestedPartyModel() { IpBaseNumber = "234", Type = InterestedPartyType.C },
                    new InterestedPartyModel() { IpBaseNumber = "345", Type = InterestedPartyType.C }
                }
            };
            var work4 = new MatchingWork()
            {
                Id = 4,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "TEST TiTLE", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() {
                    new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C },
                    new InterestedPartyModel() { IpBaseNumber = "234", Type = InterestedPartyType.C },
                    new InterestedPartyModel() { IpBaseNumber = "345", Type = InterestedPartyType.C },
                    new InterestedPartyModel() { IpBaseNumber = "456", Type = InterestedPartyType.CA }
                }
            };
            var work5 = new MatchingWork()
            {
                Id = 5,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "tEst tytle", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() {
                 new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C}}
            };

            var testSubmission = new List<Submission>() { new Submission()
                {
                    Model = new SubmissionModel()
                    {
                        Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name  = "TeSt Title", Type = Bdo.Work.TitleType.OT } },
                        InterestedParties = new List<InterestedPartyModel>() {
                            new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C } }
                    },
                    MatchedResult = new MatchResult(){ Matches = new List<MatchingWork>(){work1,work2,work3,work4, work5} }
                }
            };

            var response = (await matchingManager.RankMatches(testSubmission));

            Assert.IsAssignableFrom<IEnumerable<Submission>>(response);
            Assert.Equal(work1, response.ElementAt(0).MatchedResult.Matches.ElementAt(0));
            Assert.Equal(work2, response.ElementAt(0).MatchedResult.Matches.ElementAt(1));
            Assert.Equal(work3, response.ElementAt(0).MatchedResult.Matches.ElementAt(2));
            Assert.Equal(work4, response.ElementAt(0).MatchedResult.Matches.ElementAt(3));
            Assert.Equal(work5, response.ElementAt(0).MatchedResult.Matches.ElementAt(4));
        }

        /// <summary>
        /// Verifies that the RankMatches method returns an Ienumerable of submissions and sorts by ranking score for multiple matches with errors
        /// </summary>
        [Fact]
        public async Task Ranking_CalculatesRankScore_MultipleMatches_WithErrors()
        {
            var matchingService = new Mock<IMatchingService>().Object;
            var workManagerMock = new Mock<IWorkManager>().Object;
            var iswcManagerMock = new Mock<IIswcRepository>().Object;
            var mapperMock = new Mock<IMapper>().Object;
            var rulesManagerMock = new Mock<IRulesManager>();
            var workRepositoryMock = new Mock<IWorkRepository>().Object;
            var additionalIdentifierManagerMock = new Mock<IAdditionalIdentifierManager>().Object;

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("CalculateRolledUpRole"))
                .ReturnsAsync("C(C, CA, A);MA(AR, SR);TA(AD, SA, TR);E(E, AM)");

            var matchingManager = new Mock<MatchingManager>(matchingService, workManagerMock, iswcManagerMock, mapperMock, rulesManagerMock.Object, workRepositoryMock, additionalIdentifierManagerMock).Object;

            var work1 = new MatchingWork()
            {
                Id = 1,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.OT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C } }
            };
            var work4 = new MatchingWork()
            {
                Id = 4,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() { }
            };
            var work5 = new MatchingWork()
            {
                Id = 4,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C } }
            };

            var testSubmission = new List<Submission>() { new Submission()
                {
                    Model = new SubmissionModel()
                    {
                        Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name  = "Test Title", Type = Bdo.Work.TitleType.OT } },
                        InterestedParties = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.CA } }
                    },
                    MatchedResult = new MatchResult(){ Matches = new List<MatchingWork>(){work1,work4,work5} }
                }
            };

            var response = (await matchingManager.RankMatches(testSubmission));

            Assert.IsAssignableFrom<IEnumerable<Submission>>(response);
            Assert.Equal(work1, response.ElementAt(0).MatchedResult.Matches.ElementAt(0));
            Assert.Equal(work5, response.ElementAt(0).MatchedResult.Matches.ElementAt(1));
            Assert.Equal(work4, response.ElementAt(0).MatchedResult.Matches.ElementAt(2));
        }

        /// <summary>
        /// Verifies that MatchAsync returns a list of matches.
        /// </summary>
        [Fact]
        public async Task Returns_MatchResults()
        {
            var matchingService = new Mock<IMatchingService>();
            var workManagerMock = new Mock<IWorkManager>();
            var iswcManagerMock = new Mock<IIswcRepository>();
            var mapperMock = new Mock<IMapper>().Object;
            var rulesManagerMock = new Mock<IRulesManager>();
            var workRepositoryMock = new Mock<IWorkRepository>().Object;
            var additionalIdentifierManagerMock = new Mock<IAdditionalIdentifierManager>().Object;

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("CalculateRolledUpRole"))
                .ReturnsAsync("C(C, CA, A);MA(AR, SR);TA(AD, SA, TR);E(E, AM)");

            var testList = new List<Submission>() { new Submission() { SubmissionId = 1 } };
            matchingService.Setup(s => s.MatchAsync(testList, default)).ReturnsAsync(testList);

            var matchingManager = new Mock<MatchingManager>(matchingService.Object, workManagerMock.Object, iswcManagerMock.Object, mapperMock, rulesManagerMock.Object, workRepositoryMock, additionalIdentifierManagerMock).Object;

            var response = await matchingManager.MatchAsync(testList, default);

            Assert.Equal(1, response.ElementAt(0).SubmissionId);
        }

        /// <summary>
        /// Verifies that the RankMatches method adds one to RankScore of match with oldest ip created date 
        /// </summary>
        [Fact]
        public async Task Ranking_AddToRankScore()
        {
            var matchingService = new Mock<IMatchingService>().Object;
            var workManagerMock = new Mock<IWorkManager>().Object;
            var iswcManagerMock = new Mock<IIswcRepository>().Object;
            var mapperMock = new Mock<IMapper>().Object;
            var rulesManagerMock = new Mock<IRulesManager>();
            var workRepositoryMock = new Mock<IWorkRepository>().Object;
            var additionalIdentifierManagerMock = new Mock<IAdditionalIdentifierManager>().Object;

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("CalculateRolledUpRole"))
                .ReturnsAsync("C(C, CA, A);MA(AR, SR);TA(AD, SA, TR);E(E, AM)");

            var matchingManager = new Mock<MatchingManager>(matchingService, workManagerMock, iswcManagerMock, mapperMock, rulesManagerMock.Object, workRepositoryMock, additionalIdentifierManagerMock).Object;


            var work1 = new MatchingWork()
            {
                Id = 1,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.OT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C } }
            };
            var work4 = new MatchingWork()
            {
                Id = 4,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() {
                    new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.E },
                     new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.A }
                    }
            };
            var work5 = new MatchingWork()
            {
                Id = 4,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "545", Type = InterestedPartyType.ES } }
            };

            var testSubmission = new List<Submission>() { new Submission()
                {
                    Model = new SubmissionModel()
                    {
                        Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name  = "Test Title", Type = Bdo.Work.TitleType.OT } },
                        InterestedParties = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.CA } }
                    },
                    MatchedResult = new MatchResult(){ Matches = new List<MatchingWork>(){work1,work4,work5} }
                }
            };

            var response = (await matchingManager.RankMatches(testSubmission));

            Assert.IsAssignableFrom<IEnumerable<Submission>>(response);
            Assert.Equal(work1, response.ElementAt(0).MatchedResult.Matches.ElementAt(0));
            Assert.Equal(work5, response.ElementAt(0).MatchedResult.Matches.ElementAt(2));
            Assert.Equal(work4, response.ElementAt(0).MatchedResult.Matches.ElementAt(1));
        }

        /// <summary>
        /// Verifies that the matching adds archived iswc to submission iswc property and sets 
        /// preferred iswc to empty for assignment when archived is submitted as preferred
        /// </summary>
        [Fact]
        public async Task Matching_Assigns_Archived_ISWC_To_Iswc_Property()
        {
            var matchingService = new Mock<IMatchingService>();
            var workManagerMock = new Mock<IWorkManager>();
            var iswcManagerMock = new Mock<IIswcRepository>();
            var mapperMock = new Mock<IMapper>().Object;
            var rulesManagerMock = new Mock<IRulesManager>();
            var workRepositoryMock = new Mock<IWorkRepository>();
            var additionalIdentifierManagerMock = new Mock<IAdditionalIdentifierManager>().Object;

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("CalculateRolledUpRole"))
                .ReturnsAsync("C(C, CA, A);MA(AR, SR);TA(AD, SA, TR);E(E, AM)");

            var testList = new List<Submission>() { new Submission()
            { SubmissionId = 1, TransactionType = Bdo.Edi.TransactionType.CAR, Model = new SubmissionModel
                {
                    PreferredIswc = "T11111"
                },
                    MatchedResult = new MatchResult
                    {
                        Matches = new List<MatchingWork>
                        {
                            new MatchingWork()
                            {
                                Id = 1,
                                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.AT } },
                                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "545", Type = InterestedPartyType.ES } }
                            }
                        }
                    }
                }
            };

            workManagerMock.Setup(w => w.CheckIfArchivedIswcAsync("T11111")).ReturnsAsync(true);
            var matchingManager = new Mock<MatchingManager>(matchingService.Object, workManagerMock.Object, iswcManagerMock.Object, mapperMock, rulesManagerMock.Object, workRepositoryMock.Object, additionalIdentifierManagerMock).Object;

            var response = (await matchingManager.MatchAsync(testList));

            Assert.Equal(testList[0].Model.Iswc, response.ElementAt(0).Model.Iswc);
            Assert.Equal(string.Empty, response.ElementAt(0).Model.PreferredIswc);
        }


        /// <summary>
        /// Verifies that the matching does not add archived iswc to submission iswc property  
        /// when submitted preferred iswc is not archived 
        /// </summary>
        [Fact]
        public async Task Matching_Does_Not_Assign_Archived_ISWC_To_Iswc_Property()
        {
            var matchingService = new Mock<IMatchingService>();
            var workManagerMock = new Mock<IWorkManager>();
            var iswcManagerMock = new Mock<IIswcRepository>();
            var mapperMock = new Mock<IMapper>().Object;
            var rulesManagerMock = new Mock<IRulesManager>();
            var workRepositoryMock = new Mock<IWorkRepository>();
            var additionalIdentifierManagerMock = new Mock<IAdditionalIdentifierManager>().Object;

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("CalculateRolledUpRole"))
                .ReturnsAsync("C(C, CA, A);MA(AR, SR);TA(AD, SA, TR);E(E, AM)");

            var testList = new List<Submission>() { new Submission()
            { SubmissionId = 1, TransactionType = Bdo.Edi.TransactionType.CAR, Model = new SubmissionModel
                {
                    PreferredIswc = "T11111"
                },
                    MatchedResult = new MatchResult
                    {
                        Matches = new List<MatchingWork>
                        {
                            new MatchingWork()
                            {
                                Id = 1,
                                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.AT } },
                                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "545", Type = InterestedPartyType.ES } }
                            }
                        }
                    }
                }
            };

            workManagerMock.Setup(w => w.CheckIfArchivedIswcAsync("T11111")).ReturnsAsync(false);
            var matchingManager = new Mock<MatchingManager>(matchingService.Object, workManagerMock.Object, iswcManagerMock.Object, mapperMock, rulesManagerMock.Object, workRepositoryMock.Object, additionalIdentifierManagerMock).Object;

            var response = (await matchingManager.RankMatches(testList));

            Assert.Equal(testList[0].Model.PreferredIswc, response.ElementAt(0).Model.PreferredIswc);
            Assert.True(string.IsNullOrEmpty(response.ElementAt(0).Model.Iswc));
        }

        /// <summary>
        /// Verifies that Eligible Update Submission with no preferred iswc when submitted gets assigned
        /// the iswc from matches found by work code
        /// </summary>
        [Fact]
        public async Task Eligible_Update_Submission_is_Assigned_Preferred_Iswc()
        {
            var matchingService = new Mock<IMatchingService>().Object;
            var workManagerMock = new Mock<IWorkManager>().Object;
            var iswcManagerMock = new Mock<IIswcRepository>().Object;
            var mapperMock = new Mock<IMapper>().Object;
            var rulesManagerMock = new Mock<IRulesManager>();
            var workRepositoryMock = new Mock<IWorkRepository>().Object;
            var additionalIdentifierManagerMock = new Mock<IAdditionalIdentifierManager>().Object;

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("CalculateRolledUpRole"))
                .ReturnsAsync("C(C, CA, A);MA(AR, SR);TA(AD, SA, TR);E(E, AM)");

            var matchingManager = new Mock<MatchingManager>(matchingService, workManagerMock, iswcManagerMock, mapperMock, rulesManagerMock.Object, workRepositoryMock, additionalIdentifierManagerMock).Object;


            var work1 = new MatchingWork()
            {
                Id = 1,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.OT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C } },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "workcode1", Type = "21" }, new WorkNumber { Number = "T1", Type = "ISWC" } }
            };
            var work2 = new MatchingWork()
            {
                Id = 4,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() {
                    new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.E },
                     new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.A }
                    },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "workcode2", Type = "21" }, new WorkNumber { Number = "T2", Type = "ISWC" } }
            };
            var work3 = new MatchingWork()
            {
                Id = 4,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title3", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "545", Type = InterestedPartyType.ES } },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "workcode3", Type = "21" }, new WorkNumber { Number = "T3", Type = "ISWC" } }
            };
            var work4 = new MatchingWork()
            {
                Id = 4,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title3", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "545", Type = InterestedPartyType.ES } },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "", Type = "" }, new WorkNumber { Type = "", Number = "ISWC" } }
            };

            var testSubmission = new List<Submission>() { new Submission()
                {
                    TransactionType = Bdo.Edi.TransactionType.CUR,
                    IsEligible = true,
                    Model = new SubmissionModel()
                    {
                        Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name  = "Test Title", Type = Bdo.Work.TitleType.OT } },
                        InterestedParties = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.CA } },
                        WorkNumber = new WorkNumber{ Number = "workcode3", Type="21" }
                    },
                    MatchedResult = new MatchResult(){ Matches = new List<MatchingWork>(){work1,work2,work3} }
                }
            };

            var response = (await matchingManager.RankMatches(testSubmission));

            Assert.Equal(work3.Numbers.First(x => x.Type == "ISWC").Number, response.First().Model.PreferredIswc);
        }

        /// <summary>
        /// Verifies that Eligible Update Submission with no preferred iswc when submitted gets assigned
        /// the iswc from matches found by work code
        /// </summary>
        [Fact]
        public async Task InEligible_Update_Submission_is_Assigned_Preferred_Iswc()
        {
            var matchingService = new Mock<IMatchingService>().Object;
            var workManagerMock = new Mock<IWorkManager>().Object;
            var iswcManagerMock = new Mock<IIswcRepository>().Object;
            var mapperMock = new Mock<IMapper>().Object;
            var rulesManagerMock = new Mock<IRulesManager>();
            var workRepositoryMock = new Mock<IWorkRepository>().Object;
            var additionalIdentifierManagerMock = new Mock<IAdditionalIdentifierManager>().Object;

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("CalculateRolledUpRole"))
                .ReturnsAsync("C(C, CA, A);MA(AR, SR);TA(AD, SA, TR);E(E, AM)");

            var matchingManager = new Mock<MatchingManager>(matchingService, workManagerMock, iswcManagerMock, mapperMock, rulesManagerMock.Object, workRepositoryMock, additionalIdentifierManagerMock).Object;


            var work1 = new MatchingWork()
            {
                Id = 1,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.OT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C } },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "workcode1", Type = "21" }, new WorkNumber { Number = "T1", Type = "ISWC" } }
            };
            var work2 = new MatchingWork()
            {
                Id = 4,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title3", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "545", Type = InterestedPartyType.ES } },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "workcode2", Type = "21" }, new WorkNumber { Number = "T3", Type = "ISWC" } }
            };

            var testSubmission = new List<Submission>() { new Submission()
                {
                    TransactionType = Bdo.Edi.TransactionType.CUR,
                    IsEligible = false,
                    Model = new SubmissionModel()
                    {
                        Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name  = "Test Title", Type = Bdo.Work.TitleType.OT } },
                        InterestedParties = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.CA } },
                        WorkNumber = new WorkNumber{ Number = "workcode3", Type="21" }
                    },
                    MatchedResult = new MatchResult(){ Matches = new List<MatchingWork>(){work1,work2} }
                }
            };

            var response = (await matchingManager.RankMatches(testSubmission));

            Assert.Equal(work1.Numbers.First(x => x.Type == "ISWC").Number, response.First().Model.PreferredIswc);
        }

        /// <summary>
        /// A submisison with an Isrc match which has a preferred associated Iswc is assigend to the submission model.
        /// The Isrc aligns with a work metadata match so the submission is not flagged.
        /// </summary>
        [Fact(Skip = "Requires updating after ISRC cross-checking changes")]
        public async Task Submission_With_Matching_Isrc_Assigned_Preferred_Iswc_And_Not_Flagged()
        {
            // Arrange
            var testMappedWorkInfo = new SubmissionModel()
            {
                Iswc = "T12345",
                InterestedParties = new List<InterestedPartyModel>() { new InterestedPartyModel() { ContributorID = 1 } },
                Performers = new List<Bdo.Work.Performer>() { new Bdo.Work.Performer() { PerformerID = 1 } },
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.OT } },
            };

            var work1 = new MatchingWork()
            {
                Id = 1,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.OT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C } },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "workcode1", Type = "21" }, new WorkNumber { Number = "T1", Type = "ISWC" } }
            };
            var work2 = new MatchingWork()
            {
                Id = 4,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title3", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "545", Type = InterestedPartyType.ES } },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "workcode2", Type = "21" }, new WorkNumber { Number = "T12345", Type = "ISWC" } }
            };

            var matchingService = new Mock<IMatchingService>().Object;
            var workManagerMock = new Mock<IWorkManager>().Object;
            var iswcManagerMock = new Mock<IIswcRepository>();
            var mapperMock = new Mock<IMapper>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var workRepositoryMock = new Mock<IWorkRepository>();
            var additionalIdentifierManagerMock = new Mock<IAdditionalIdentifierManager>();

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("CalculateRolledUpRole"))
                .ReturnsAsync("C(C, CA, A);MA(AR, SR);TA(AD, SA, TR);E(E, AM)");

            additionalIdentifierManagerMock.Setup(v => v.Exists(It.IsAny<string>()))
                .ReturnsAsync(true);
            additionalIdentifierManagerMock.Setup(v => v.FindManyAsync(It.IsAny<string[]>(), 1))
                .ReturnsAsync(new List<Data.DataModels.AdditionalIdentifier>()
                {
                    new Data.DataModels.AdditionalIdentifier()
                    {
                        WorkInfoId = 12345
                    }
                });

            workRepositoryMock.Setup(v => v.FindManyWorkInfosAsync(It.IsAny<IEnumerable<long>>()))
                .ReturnsAsync(new List<WorkInfo>
                {
                    new Data.DataModels.WorkInfo
                    {
                        WorkInfoId = 98765
                    }
                });

            mapperMock.Setup(v => v.Map<SubmissionModel>(It.IsAny<Data.DataModels.WorkInfo>()))
                .Returns(testMappedWorkInfo);

            iswcManagerMock.Setup(v => v.ExistsAsync(It.IsAny<Expression<Func<Data.DataModels.Iswc, bool>>>()))
                .ReturnsAsync(true);

            var matchingManager = new Mock<MatchingManager>(matchingService, workManagerMock, iswcManagerMock.Object, mapperMock.Object, rulesManagerMock.Object, workRepositoryMock.Object, additionalIdentifierManagerMock.Object).Object;

            var testSubmission = new List<Submission>() { new Submission()
                {
                    TransactionType = Bdo.Edi.TransactionType.CAR,
                    IsEligible = false,
                    Model = new SubmissionModel()
                    {
                        Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name  = "Test Title", Type = Bdo.Work.TitleType.OT } },
                        InterestedParties = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.CA } },
                        WorkNumber = new WorkNumber{ Number = "workcode3", Type="21" },
                        AdditionalIdentifiers = new List<Bdo.Work.AdditionalIdentifier>()
                        {
                            new Bdo.Work.AdditionalIdentifier(){ WorkCode = "12345" }
                        }
                    },
                    MatchedResult = new MatchResult(){ Matches = new List<MatchingWork>(){ work1, work2 } }
                }
            };

            // Act
            var response = await matchingManager.MatchIsrcsAsync(testSubmission);

            //Assert
            Assert.False(response.ElementAt(0).HasAlternateIswcMatches);
            Assert.Single(response.ElementAt(0).IsrcMatchedResult.Matches);
            Assert.Equal(response.ElementAt(0).Model.PreferredIswc, testMappedWorkInfo.Iswc);
        }

        /// <summary>
        /// Verifies that a submission model with multiple Isrc macthes with preferred Iswc statuses 
        /// is flagged with the HasMultipleIswcMatches flag
        /// </summary>
        [Fact(Skip = "Requires updating after ISRC cross-checking changes")]
        public async Task Submission_With_Multiple_Isrc_Iswc_Preferred_Matches_Is_Flagged()
        {
            // Arrange
            var testMappedWorkInfo = new SubmissionModel()
            {
                Iswc = "T12345",
                InterestedParties = new List<InterestedPartyModel>() { new InterestedPartyModel() { ContributorID = 1 } },
                Performers = new List<Bdo.Work.Performer>() { new Bdo.Work.Performer() { PerformerID = 1 } },
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.OT } },
            };

            var matchingService = new Mock<IMatchingService>().Object;
            var workManagerMock = new Mock<IWorkManager>().Object;
            var iswcManagerMock = new Mock<IIswcRepository>();
            var mapperMock = new Mock<IMapper>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var workRepositoryMock = new Mock<IWorkRepository>();
            var additionalIdentifierManagerMock = new Mock<IAdditionalIdentifierManager>();

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("CalculateRolledUpRole"))
                .ReturnsAsync("C(C, CA, A);MA(AR, SR);TA(AD, SA, TR);E(E, AM)");

            additionalIdentifierManagerMock.Setup(v => v.Exists(It.IsAny<string>()))
                .ReturnsAsync(true);
            additionalIdentifierManagerMock.Setup(v => v.FindManyAsync(It.IsAny<string[]>(), 1))
                .ReturnsAsync(new List<Data.DataModels.AdditionalIdentifier>()
                {
                    new Data.DataModels.AdditionalIdentifier()
                    {
                        WorkInfoId = 12345
                    }
                });

            workRepositoryMock.Setup(v => v.FindManyWorkInfosAsync(It.IsAny<IEnumerable<long>>()))
                .ReturnsAsync(new List<WorkInfo>
                {
                    new WorkInfo
                    {
                        WorkInfoId = 98765
                    },
                    new WorkInfo
                    {
                        WorkInfoId = 98767
                    }
                });

            mapperMock.Setup(v => v.Map<SubmissionModel>(It.IsAny<Data.DataModels.WorkInfo>()))
                .Returns(testMappedWorkInfo);

            iswcManagerMock.Setup(v => v.ExistsAsync(It.IsAny<Expression<Func<Data.DataModels.Iswc, bool>>>()))
                .ReturnsAsync(true);

            var matchingManager = new Mock<MatchingManager>(matchingService, workManagerMock, iswcManagerMock.Object, mapperMock.Object, rulesManagerMock.Object, workRepositoryMock.Object, additionalIdentifierManagerMock.Object).Object;


            var work1 = new MatchingWork()
            {
                Id = 1,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.OT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C } },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "workcode1", Type = "21" }, new WorkNumber { Number = "T1", Type = "ISWC" } }
            };
            var work2 = new MatchingWork()
            {
                Id = 4,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title3", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "545", Type = InterestedPartyType.ES } },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "workcode2", Type = "21" }, new WorkNumber { Number = "T3", Type = "ISWC" } }
            };

            var testSubmission = new List<Submission>() { new Submission()
                {
                    TransactionType = Bdo.Edi.TransactionType.CAR,
                    IsEligible = false,
                    Model = new SubmissionModel()
                    {
                        Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name  = "Test Title", Type = Bdo.Work.TitleType.OT } },
                        InterestedParties = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.CA } },
                        WorkNumber = new WorkNumber{ Number = "workcode3", Type="21" },
                        AdditionalIdentifiers = new List<Bdo.Work.AdditionalIdentifier>()
                        {
                            new Bdo.Work.AdditionalIdentifier(){ WorkCode = "12345" }
                        }
                    },
                    MatchedResult = new MatchResult(){ Matches = new List<MatchingWork>(){ work1, work2 } }
                }
            };

            // Act
            var response = await matchingManager.MatchIsrcsAsync(testSubmission);

            //Assert
            Assert.True(response.ElementAt(0).HasAlternateIswcMatches);
            Assert.Equal(2, response.ElementAt(0).IsrcMatchedResult.Matches.Count());
            Assert.Equal(response.ElementAt(0).Model.PreferredIswc, testMappedWorkInfo.Iswc);
        }

        /// <summary>
        /// Verifies that a submission model with an Isrc match with preferred Iswc statuses that 
        ///  does not align with any metadata work match is flagged with the HasMultipleIswcMatches flag
        /// </summary>
        [Fact(Skip = "Requires updating after ISRC cross-checking changes")]
        public async Task Submission_Isrc_Iswc_Does_Not_Match_Metadata_Match_Is_Flagged()
        {
            // Arrange
            var testMappedWorkInfo = new SubmissionModel()
            {
                Iswc = "T12345",
                InterestedParties = new List<InterestedPartyModel>() { new InterestedPartyModel() { ContributorID = 1 } },
                Performers = new List<Bdo.Work.Performer>() { new Bdo.Work.Performer() { PerformerID = 1 } },
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.OT } },
            };

            var matchingService = new Mock<IMatchingService>().Object;
            var workManagerMock = new Mock<IWorkManager>().Object;
            var iswcManagerMock = new Mock<IIswcRepository>();
            var mapperMock = new Mock<IMapper>();
            var rulesManagerMock = new Mock<IRulesManager>();
            var workRepositoryMock = new Mock<IWorkRepository>();
            var additionalIdentifierManagerMock = new Mock<IAdditionalIdentifierManager>();

            rulesManagerMock.Setup(v => v.GetParameterValue<string>("CalculateRolledUpRole"))
                .ReturnsAsync("C(C, CA, A);MA(AR, SR);TA(AD, SA, TR);E(E, AM)");

            additionalIdentifierManagerMock.Setup(v => v.Exists(It.IsAny<string>()))
                .ReturnsAsync(true);
            additionalIdentifierManagerMock.Setup(v => v.FindManyAsync(It.IsAny<string[]>(), 1))
                .ReturnsAsync(new List<Data.DataModels.AdditionalIdentifier>()
                {
                    new Data.DataModels.AdditionalIdentifier()
                    {
                        WorkInfoId = 12345
                    }
                });

            workRepositoryMock.Setup(v => v.FindManyWorkInfosAsync(It.IsAny<IEnumerable<long>>()))
                .ReturnsAsync(new List<WorkInfo>
                {
                    new Data.DataModels.WorkInfo
                    {
                        WorkInfoId = 98765
                    }
                });

            mapperMock.Setup(v => v.Map<SubmissionModel>(It.IsAny<Data.DataModels.WorkInfo>()))
                .Returns(testMappedWorkInfo);

            iswcManagerMock.Setup(v => v.ExistsAsync(It.IsAny<Expression<Func<Data.DataModels.Iswc, bool>>>()))
                .ReturnsAsync(true);

            var matchingManager = new Mock<MatchingManager>(matchingService, workManagerMock, iswcManagerMock.Object, mapperMock.Object, rulesManagerMock.Object, workRepositoryMock.Object, additionalIdentifierManagerMock.Object).Object;


            var work1 = new MatchingWork()
            {
                Id = 1,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.OT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C } },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "workcode1", Type = "21" }, new WorkNumber { Number = "T1", Type = "ISWC" } }
            };
            var work2 = new MatchingWork()
            {
                Id = 4,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title3", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "545", Type = InterestedPartyType.ES } },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "workcode2", Type = "21" }, new WorkNumber { Number = "T3", Type = "ISWC" } }
            };

            var testSubmission = new List<Submission>() { new Submission()
                {
                    TransactionType = Bdo.Edi.TransactionType.CAR,
                    IsEligible = false,
                    Model = new SubmissionModel()
                    {
                        Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name  = "Test Title", Type = Bdo.Work.TitleType.OT } },
                        InterestedParties = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.CA } },
                        WorkNumber = new WorkNumber{ Number = "workcode3", Type="21" },
                        AdditionalIdentifiers = new List<Bdo.Work.AdditionalIdentifier>()
                        {
                            new Bdo.Work.AdditionalIdentifier(){ WorkCode = "12345" }
                        }
                    },
                    MatchedResult = new MatchResult(){ Matches = new List<MatchingWork>(){ work1, work2 } }
                }
            };

            // Act
            var response = await matchingManager.MatchIsrcsAsync(testSubmission);

            //Assert
            Assert.True(response.ElementAt(0).HasAlternateIswcMatches);
            Assert.Single(response.ElementAt(0).IsrcMatchedResult.Matches);
            Assert.Equal(response.ElementAt(0).Model.PreferredIswc, testMappedWorkInfo.Iswc);
        }
    }
}
