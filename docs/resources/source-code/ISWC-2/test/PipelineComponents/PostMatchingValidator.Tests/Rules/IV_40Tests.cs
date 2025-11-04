using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
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
    /// Tests rule IV_40
    /// </summary>
    public class IV_40Tests : TestBase
    {
        /// <summary>
        /// Check rule passes if no disambiguateFromIswcs are provided
        /// </summary>
        [Fact]
        public async Task IV_40_NoDisambiguateFromIswcsProvided_Valid()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel() { DisambiguateFrom = new List<DisambiguateFrom>() },
                TransactionType = TransactionType.CAR
            };

            var rulesManagerMock = new Mock<IRulesManager>();
            var workManagerMock = new Mock<IWorkManager>().Object;

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateDisambiguationISWCs")).ReturnsAsync(true);

            var test = new Mock<IV_40>(rulesManagerMock.Object, workManagerMock, GetMessagingManagerMock(ErrorCode._129)).Object;

            var response = await test.IsValid(submission);
            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_40), test.Identifier);
        }

        /// <summary>
        /// Check rule passes if parameter value is set to false
        /// </summary>
        [Fact]
        public async Task IV_40_ParameterValueIsFalse_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel(), TransactionType = TransactionType.CAR };

            var rulesManagerMock = new Mock<IRulesManager>();
            var workManagerMock = new Mock<IWorkManager>().Object;

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateDisambiguationISWCs")).ReturnsAsync(false);

            var test = new Mock<IV_40>(rulesManagerMock.Object, workManagerMock, GetMessagingManagerMock(ErrorCode._129)).Object;

            var response = await test.IsValid(submission);
            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_40), test.Identifier);
        }

        /// <summary>
        /// Check rule fails if any disambiguateFromIswcs do not have a matching preferredIswc
        /// </summary>
        [Fact]
        public async Task IV_40_NotAllDisambiguateFromIswcsMatch_Invalid()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    DisambiguateFrom = new List<DisambiguateFrom>() {
                        new DisambiguateFrom { Iswc = "17"},
                        new DisambiguateFrom { Iswc="18"}
                    }
                },
                TransactionType = TransactionType.CAR
            };

            var rulesManagerMock = new Mock<IRulesManager>();
            var workManagerMock = new Mock<IWorkManager>();

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateDisambiguationISWCs")).ReturnsAsync(true);

            workManagerMock.Setup(i => i.FindAsync("17", false, false)).ReturnsAsync(new SubmissionModel() { Iswc = "171717" });
            workManagerMock.Setup(i => i.FindAsync("18", false, false)).ReturnsAsync(default(SubmissionModel));

            var test = new Mock<IV_40>(rulesManagerMock.Object, workManagerMock.Object, GetMessagingManagerMock(ErrorCode._129)).Object;

            var response = await test.IsValid(submission);
            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._129, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check rule passes if all disambiguateFromIswcs have a matching preferredIswc
        /// </summary>
        [Fact]
        public async Task IV_40_AllDisambiguateFromIswcsMatch_Valid()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    DisambiguateFrom = new List<DisambiguateFrom>() {
                        new DisambiguateFrom { Iswc="17"},
                        new DisambiguateFrom { Iswc="18"}
                    }
                },
                TransactionType = TransactionType.CAR
            };

            var rulesManagerMock = new Mock<IRulesManager>();
            var workManagerMock = new Mock<IWorkManager>();

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateDisambiguationISWCs")).ReturnsAsync(true);

            workManagerMock.Setup(i => i.FindAsync("17", false, false)).ReturnsAsync(new SubmissionModel() { Iswc = "171717" });
            workManagerMock.Setup(i => i.FindAsync("18", false, false)).ReturnsAsync(new SubmissionModel() { Iswc = "181818" });

            var test = new Mock<IV_40>(rulesManagerMock.Object, workManagerMock.Object, GetMessagingManagerMock(ErrorCode._129)).Object;

            var response = await test.IsValid(submission);
            Assert.True(response.IsValid);
        }
    }
}
