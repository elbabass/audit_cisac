using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.MatchingEngine;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.PostMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.Tests.PostMatching
{
    /// <summary>
    /// Tests for MatchingAlterIps post matching compontent
    /// </summary>
    public class MatchingAlterIpsTests
    {
        /// <summary>
        /// Check that if there are no initial matches found, the rule removes Public Domain IPs and IPs with date of death over 80 years ago and then attempts matching again
        /// </summary>
        [Fact]
        public async Task MatchAsync_Called()
        {
            var matchingManagerMock = new Mock<IMatchingManager>();

            const string ip1 = "I-000009793-6";
            const string ip2 = CommonIPs.DP;
            const string ip3 = "I-000024999-8";

            var submission = new List<Submission>()
            {
                new Submission()
                {
                    SubmissionId = 0,
                    MatchedResult = new MatchResult()
                    {
                        Matches = new List<MatchingWork>()
                    },
                    Rejection = null,
                    Model = new SubmissionModel()
                    {
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel()
                            {
                                IpBaseNumber = ip1
                            },
                            new InterestedPartyModel()
                            {
                                IpBaseNumber = ip2
                            },
                            new InterestedPartyModel()
                            {
                                DeathDate = DateTime.Parse("1894-01-01 00:00:00"),
                                IpBaseNumber = ip3
                            },
                        }
                    }
                }
            };

            matchingManagerMock.Setup(x => x.MatchAsync(It.IsAny<IEnumerable<Submission>>(), It.IsAny<string>()))
              .ReturnsAsync(new List<Submission>() {
                    new Submission()
                    {
                        SubmissionId = 0,
                        MatchedResult = new MatchResult()
                        {
                            Matches = new List<MatchingWork>(){ new MatchingWork() { } }
                        },
                        Rejection = null,
                         Model = new SubmissionModel()
                        {
                            InterestedParties = new List<InterestedPartyModel>()
                            {
                                new InterestedPartyModel()
                                {
                                    IpBaseNumber = ip1
                                }
                            }
                        }
                    }}
              );

            var test = new MatchingAlterIps(matchingManagerMock.Object);
            var response = await test.ProcessSubmissions(submission);

            matchingManagerMock.Verify(m => m.MatchAsync(It.IsAny<IEnumerable<Submission>>(), It.IsAny<string>()), Times.Once());
            Assert.Equal(3, response.ElementAt(0).Model.InterestedParties.Count());
            Assert.Single(response.ElementAt(0).MatchedResult.Matches);
            Assert.Equal(ip1, submission.ElementAt(0).Model.InterestedParties.ElementAt(0).IpBaseNumber);
        }

        /// <summary>
        /// Check that if there are no initial matches found, but there all IPs are Public Domain the rule does not remove IPs and attempt matching again.
        /// </summary>
        [Fact]
        public async Task No_Ips_Removed_NoPdIps_AllPDIPs()
        {
            var matchingManagerMock = new Mock<IMatchingManager>();

            const string ip3 = "I-000024999-8";

            var submission = new List<Submission>()
            {
                new Submission()
                {
                    SubmissionId = 0,
                    MatchedResult = new MatchResult()
                    {
                        Matches = new List<MatchingWork>()
                    },
                    Rejection = null,
                    Model = new SubmissionModel()
                    {
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel()
                            {
                                IpBaseNumber = CommonIPs.DP
                            },
                            new InterestedPartyModel()
                            {
                                DeathDate = DateTime.Parse("1894-01-01 00:00:00"),
                                IpBaseNumber = ip3
                            },
                        }
                    }
                },
            };

            matchingManagerMock.Setup(x => x.MatchAsync(It.IsAny<IEnumerable<Submission>>(), It.IsAny<string>()));

            var test = new MatchingAlterIps(matchingManagerMock.Object);
            var response = await test.ProcessSubmissions(submission);

            matchingManagerMock.Verify(m => m.MatchAsync(It.IsAny<IEnumerable<Submission>>(), It.IsAny<string>()), Times.Never());
            Assert.Equal(2, response.ElementAt(0).Model.InterestedParties.Count());
        }

        /// <summary>
        /// Check that if there are no initial matches found, but there are no Public Domain IPs the rule does not remove IPs and attempt matching again.
        /// </summary>
        [Fact]
        public async Task No_Ips_Removed_NoPdIps()
        {
            var matchingManagerMock = new Mock<IMatchingManager>();

            const string ip1 = "I-000009793-6";

            var submission = new List<Submission>()
            {
                new Submission()
                {
                    SubmissionId = 0,
                    MatchedResult = new MatchResult()
                    {
                        Matches = new List<MatchingWork>()
                    },
                    Rejection = null,
                    Model = new SubmissionModel()
                    {
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel()
                            {
                                IpBaseNumber = ip1
                            }
                        }
                    }
                },
            };

            matchingManagerMock.Setup(x => x.MatchAsync(It.IsAny<IEnumerable<Submission>>(), It.IsAny<string>()));

            var test = new MatchingAlterIps(matchingManagerMock.Object);
            var response = await test.ProcessSubmissions(submission);

            matchingManagerMock.Verify(m => m.MatchAsync(It.IsAny<IEnumerable<Submission>>(), It.IsAny<string>()), Times.Never());
            Assert.Single(response.ElementAt(0).Model.InterestedParties);
            Assert.Equal(ip1, response.ElementAt(0).Model.InterestedParties.ElementAt(0).IpBaseNumber);
        }


        /// <summary>
        /// Check that if there are matches, the rule does not remove IPs and attempt matching again.
        /// </summary>
        [Fact]
        public async Task No_Ips_Removed_Matches()
        {
            var matchingManagerMock = new Mock<IMatchingManager>();

            var submission = new List<Submission>() {
                new Submission() {
                    SubmissionId = 0,
                    MatchedResult = new MatchResult()
                    {
                        Matches = new List<MatchingWork>() { new MatchingWork() { } }
                    },
                    Rejection = null,
                    Model = new SubmissionModel(){
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel()
                            {
                                IpBaseNumber = "I-000009793-8"
                            },
                            new InterestedPartyModel()
                            {
                                IpBaseNumber = "I-001635861-0"
                            },
                            new InterestedPartyModel()
                            {
                                DeathDate = DateTime.Parse("2000-01-01 00:00:00"),
                                IpBaseNumber = "I-000024999-8"
                            },
                        }
                    }
                },
            };

            var test = new MatchingAlterIps(matchingManagerMock.Object);
            var response = await test.ProcessSubmissions(submission);

            matchingManagerMock.Verify(m => m.MatchAsync(It.IsAny<IEnumerable<Submission>>(), It.IsAny<string>()), Times.Never());
            Assert.Equal(3, response.FirstOrDefault().Model.InterestedParties.Count());
        }

        /// <summary>
        /// Check that if there are no initial matches found in one submission but there are in another that has Public Domain IPs, the rule removes Public Domain IPs and IPs with date of death over 80 years ago and then attempts matching again
        /// </summary>
        [Fact]
        public async Task MatchAsync_Called_SubmissionBatch()
        {
            var matchingManagerMock = new Mock<IMatchingManager>();

            const string ip1 = "I-000009793-6";
            const string ip2 = CommonIPs.DP;
            const string ip3 = "I-000024999-8";

            var submission = new List<Submission>()
            {
               new Submission()
                {
                    SubmissionId = 0,
                    MatchedResult = new MatchResult()
                    {
                        Matches = new List<MatchingWork>()
                    },
                    Rejection = null,
                    Model = new SubmissionModel()
                    {
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel()
                            {
                                IpBaseNumber = ip1
                            },
                            new InterestedPartyModel()
                            {
                                IpBaseNumber = ip2
                            },
                            new InterestedPartyModel()
                            {
                                DeathDate = DateTime.Parse("1894-01-01 00:00:00"),
                                IpBaseNumber = ip3
                            },
                        }
                    }
                },
                new Submission() {
                    SubmissionId = 1,
                    MatchedResult = new MatchResult()
                    {
                        Matches = new List<MatchingWork>() { new MatchingWork() { } }
                    },
                    Rejection = null,
                    Model = new SubmissionModel(){
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel()
                            {
                                IpBaseNumber = "I-000009793-8"
                            },
                            new InterestedPartyModel()
                            {
                                IpBaseNumber = "I-001635861-0"
                            }
                        }
                    }
                }
            };

            matchingManagerMock.Setup(x => x.MatchAsync(It.IsAny<IEnumerable<Submission>>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Submission>() {
                    new Submission()
                    {
                        SubmissionId = 0,
                        MatchedResult = new MatchResult()
                        {
                            Matches = new List<MatchingWork>(){ new MatchingWork() { }, new MatchingWork() { } }
                        },
                        Rejection = null,
                        Model = new SubmissionModel()
                        {
                            InterestedParties = new List<InterestedPartyModel>()
                            {
                                new InterestedPartyModel()
                                {
                                    IpBaseNumber = ip1
                                }
                            }
                        }
                    }}
                );

            var test = new MatchingAlterIps(matchingManagerMock.Object);
            var response = await test.ProcessSubmissions(submission);

            matchingManagerMock.Verify(m => m.MatchAsync(It.IsAny<IEnumerable<Submission>>(), It.IsAny<string>()), Times.Once());
            Assert.Collection(response,
                elem1 => Assert.Equal(2, elem1.MatchedResult.Matches.Count()),
                elem2 => Assert.Single(elem2.MatchedResult.Matches));

        }
    }
}
