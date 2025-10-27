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
    /// Tests for rule PV_10
    /// </summary>
    public class PV_10Tests : TestBase
    {
        /// <summary>
        /// Check transaction passes submitter is eligible for at least one work being merged/demerged
        /// </summary>
        [Fact]
        public async void PV_10_Eligible_Valid()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel() { Agency = "3" },
                TransactionType = TransactionType.MER,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() {
                        new MatchingWork()
                        {
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber()
                                {
                                    Type = "ISWC",
                                    Number = "iswc1"
                                }
                            }
                        },
                        new MatchingWork()
                        {
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber()
                                {
                                    Type = "ISWC",
                                    Number = "iswc2"
                                }
                            }
                        }
                    }
                }
            };

            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(w => w.FindManyAsync(It.IsAny<List<string>>(), true, DetailLevel.Full))
             .ReturnsAsync(new List<VerifiedSubmissionModel>() {
                    new VerifiedSubmissionModel()
                    {
                        Agency = "3",
                        Iswc = "iswc1",
                        IsReplaced = false,
                        InterestedParties = new List<InterestedPartyModel> { new InterestedPartyModel() { CisacType = CisacInterestedPartyType.C, Agency = "3", IpBaseNumber = "basenumber" } }
                    }
             });

            var test = new Mock<PV_10>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._150));

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_10), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction fails if submitter is not eligible for any works being merged/demerged
        /// </summary>
        [Fact]
        public async void PV_10_InEligible_InValid()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel() { Agency = "3" },
                TransactionType = TransactionType.MER,
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() {
                        new MatchingWork()
                        {
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber()
                                {
                                    Type = "ISWC",
                                    Number = "iswc1"
                                }
                            }
                        },
                        new MatchingWork()
                        {
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber()
                                {
                                    Type = "ISWC",
                                    Number = "iswc2"
                                }
                            }
                        }
                    }
                }
            };

            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(w => w.FindManyAsync(It.IsAny<List<string>>(), true, DetailLevel.Full))
             .ReturnsAsync(new List<VerifiedSubmissionModel>() {
                    new VerifiedSubmissionModel()
                    {
                        Agency = "4",
                        Iswc = "iswc2",
                        IswcEligible = true,
                        IsReplaced = false,
                        InterestedParties = new List<InterestedPartyModel> { new InterestedPartyModel() { CisacType = CisacInterestedPartyType.C, Agency = "3", IpBaseNumber = "basenumber" } }
                    }
             });

            var test = new Mock<PV_10>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._150));

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_10), test.Object.Identifier);
            Assert.Equal(ErrorCode._150, response.Submission.Rejection.Code);
        }
    }
}
