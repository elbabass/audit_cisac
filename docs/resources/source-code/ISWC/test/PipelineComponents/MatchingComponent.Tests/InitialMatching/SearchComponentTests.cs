using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.MatchingEngine;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.InitialMatching;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.Tests.InitialMatching
{
    /// <summary>
    /// Tests for SearchComponent
    /// </summary>
    public class SearchComponentTests
    {
        /// <summary>
        /// Checks that if a search match result is found, the search result model is populated.
        /// </summary>
        [Fact]
        public async void SearchMatch()
        {
            var matchingManagerMock = new Mock<IMatchingManager>();
            var workManagerMock = new Mock<IWorkManager>();
            var rulesManagerMock = new Mock<IRulesManager>();

            var submission = new List<Submission>() {
                new Submission() {
                    Model = new SubmissionModel() { Iswc = "T9021193920" },
                    TransactionType = TransactionType.CIQ
                }
            };

            matchingManagerMock.Setup(x => x.MatchAsync(submission, It.IsAny<string>()))
                .ReturnsAsync(
                    new List<Submission>() {
                        new Submission()
                        {
                            MatchedResult = new MatchResult()
                            {
                                InputWorkId = 6899037394,
                                Matches = new List<MatchingWork>(){ new MatchingWork(){ Id = 1 } }
                            },
                            Model = new SubmissionModel() { Iswc = "T9021193920" },
                            TransactionType = TransactionType.CIQ
                        }
                    }

                );

            workManagerMock.Setup(x => x.FindManyAsync(It.IsAny<IEnumerable<long>>(), true, DetailLevel.Full))
               .ReturnsAsync(new IswcModel[]{ new IswcModel
               {
                   VerifiedSubmissions = new VerifiedSubmissionModel[] { new VerifiedSubmissionModel() { Iswc = "T9021193920" } }
               } });

            var test = new Mock<SearchComponent>(workManagerMock.Object, matchingManagerMock.Object, rulesManagerMock.Object);
            var response = await test.Object.ProcessSubmissions(submission);

            Assert.Equal("T9021193920", response.FirstOrDefault().SearchedIswcModels.FirstOrDefault().VerifiedSubmissions.First().Iswc);
        }

        /// <summary>
        /// Checks that for no search match found, no search result is returned
        /// </summary>
        [Fact]
        public async void NoSearchMatch()
        {
            var matchingManagerMock = new Mock<IMatchingManager>();
            var workManagerMock = new Mock<IWorkManager>(); 
            var rulesManagerMock = new Mock<IRulesManager>();

            var submission = new List<Submission>() {
                new Submission() {
                    Model = new SubmissionModel() { Iswc = "T9021193920" },
                    TransactionType = TransactionType.CIQ
                }
            };

            matchingManagerMock.Setup(x => x.MatchAsync(submission, It.IsAny<string>()))
                .ReturnsAsync(
                    new List<Submission>() {
                        new Submission()
                        {
                            MatchedResult = new MatchResult()
                            {
                                InputWorkId = 0,
                                Matches=new List<MatchingWork>()
                            },
                            Model = new SubmissionModel() { Iswc = "T9021193920" },
                            TransactionType = TransactionType.CIQ
                        }
                    }

                );

            workManagerMock.Setup(x => x.FindManyAsync(It.IsAny<IEnumerable<long>>(), false, DetailLevel.Full))
               .ReturnsAsync(default(List<IswcModel>));

            var test = new Mock<SearchComponent>(workManagerMock.Object, matchingManagerMock.Object, rulesManagerMock.Object);
            var response = await test.Object.ProcessSubmissions(submission);

            Assert.True(response.FirstOrDefault().SearchedIswcModels.Count == 0);
        }
    }
}
