using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
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
    /// Tests for rule PV_34
    /// </summary>
    public class PV_34Tests: TestBase
	{
        /// <summary>
        /// Check transaction passes if there is a metadata match and no ISRC match
        /// </summary>
        [Fact]
        public async void PV_34_MetadataMatchNoIsrcMatch_Valid()
        {
            var submission = new Submission() 
            {
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>() 
                    {
                        new MatchingWork()
                        {
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber() { Type = "ISWC", Number = "T0000000000" }
                            }
                        }
                    }
                }, 
                TransactionType = TransactionType.CAR 
            };

            var test = new Mock<PV_34>(GetMessagingManagerMock(ErrorCode._249)).Object;

            submission.Model.DisambiguateFrom = new List<DisambiguateFrom> { };
            submission.Model.Disambiguation = false;

            var response = await test.IsValid(submission);

            Assert.Equal(nameof(PV_34), test.Identifier);
            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check transaction passes if there is a ISRC match and no metadata match
        /// </summary>
        [Fact]
        public async void PV_34_IsrcMatchNoMetadataMatch_Valid()
        {
            var submission = new Submission()
            {
                IsrcMatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>()
                    {
                        new MatchingWork()
                        {
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber() { Type = "ISWC", Number = "T0000000000" }
                            }
                        }
                    }
                },
                TransactionType = TransactionType.CAR
            };

            var test = new Mock<PV_34>(GetMessagingManagerMock(ErrorCode._249)).Object;

            submission.Model.DisambiguateFrom = new List<DisambiguateFrom> { };
            submission.Model.Disambiguation = false;

            var response = await test.IsValid(submission);

            Assert.Equal(nameof(PV_34), test.Identifier);
            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check transaction passes if there is no metadata match and no ISRC match
        /// </summary>
        [Fact]
        public async void PV_34_NoMetadataMatchNoIsrcMatch_Valid()
        {
            var submission = new Submission()
            {
                TransactionType = TransactionType.CAR
            };

            var test = new Mock<PV_34>(GetMessagingManagerMock(ErrorCode._249)).Object;

            submission.Model.DisambiguateFrom = new List<DisambiguateFrom> { };
            submission.Model.Disambiguation = false;

            var response = await test.IsValid(submission);

            Assert.Equal(nameof(PV_34), test.Identifier);
            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check transaction passes if metadata match and ISRC match are the same
        /// </summary>
        [Fact]
        public async void PV_34_PV_34_SameMetadataMatchAsIsrcMatch_Valid()
        {
            var submission = new Submission()
            {
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>()
                    {
                        new MatchingWork()
                        {
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber() { Type = "ISWC", Number = "T0000000000" }
                            }
                        }
                    }
                },
                IsrcMatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>()
                    {
                        new MatchingWork()
                        {
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber() { Type = "ISWC", Number = "T0000000000" }
                            }
                        }
                    }
                },
                TransactionType = TransactionType.CAR
            };

            var test = new Mock<PV_34>(GetMessagingManagerMock(ErrorCode._249)).Object;

            submission.Model.DisambiguateFrom = new List<DisambiguateFrom> { };
            submission.Model.Disambiguation = false;

            var response = await test.IsValid(submission);

            Assert.Equal(nameof(PV_34), test.Identifier);
            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check transaction fails if metadata match and ISRC match are different
        /// </summary>
        [Fact]
		public async void PV_34_DifferentMetadataMatchToIsrcMatch_InValid()
		{
            var submission = new Submission()
            {
                MatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>()
                    {
                        new MatchingWork()
                        {
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber() { Type = "ISWC", Number = "T0000000000" }
                            }
                        }
                    }
                },
                IsrcMatchedResult = new MatchResult()
                {
                    Matches = new List<MatchingWork>()
                    {
                        new MatchingWork()
                        {
                            Numbers = new List<WorkNumber>()
                            {
                                new WorkNumber() { Type = "ISWC", Number = "T0000000001" }
                            }
                        }
                    }
                },
                TransactionType = TransactionType.CAR
            };

			var test = new Mock<PV_34>(GetMessagingManagerMock(ErrorCode._249)).Object;

			var response = await test.IsValid(submission);

			Assert.False(response.IsValid);
			Assert.Equal(ErrorCode._249, response.Submission.Rejection.Code);
		}
	}
}
