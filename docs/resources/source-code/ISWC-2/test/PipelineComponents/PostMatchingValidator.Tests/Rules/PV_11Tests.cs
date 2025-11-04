using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Tests.Rules
{
    /// <summary>
    /// Tests for rule PV_11
    /// </summary>
    public class PV_11Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if agency is null and transcation is COA
        /// </summary>
        [Fact]
        public async Task PV_11_Agency_Null_COA()
        {
            var agencyManagerMock = new Mock<IAgencyManager>();
            var workflowManagerMock = new Mock<IWorkflowManager>();
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    Agency = "99"
                },
                TransactionType = TransactionType.COA
            };

            var test = new Mock<PV_11>(agencyManagerMock.Object, workflowManagerMock.Object, GetMessagingManagerMock(ErrorCode._151));

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_11), test.Object.Identifier);
            Assert.Equal(ErrorCode._151, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if agency is null and transaction is COR
        /// </summary>
        [Fact]
        public async Task PV_11_Agency_Null_COR()
        {
            var agencyManagerMock = new Mock<IAgencyManager>();
            var workflowManagerMock = new Mock<IWorkflowManager>();
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    Agency = "99"
                },
                WorkflowSearchModel = new WorkflowSearchModel()
                {
                    Agency = null
                },
                TransactionType = TransactionType.COR
            };

            var test = new Mock<PV_11>(agencyManagerMock.Object, workflowManagerMock.Object, GetMessagingManagerMock(ErrorCode._151));

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_11), test.Object.Identifier);
            Assert.Equal(ErrorCode._151, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction succeeds if agency is present and transactions is COR
        /// </summary>
        [Fact]
        public async Task PV_11_WorkflowTasks_COR()
        {
            var agencyManagerMock = new Mock<IAgencyManager>();
            var workflowManagerMock = new Mock<IWorkflowManager>();
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    Agency = null,
                },
                WorkflowSearchModel = new WorkflowSearchModel()
                {
                    Agency = "99",
                },
                TransactionType = TransactionType.COR
            };

            agencyManagerMock.Setup(a => a.FindAsync("99")).ReturnsAsync("99");
            var test = new Mock<PV_11>(agencyManagerMock.Object, workflowManagerMock.Object, GetMessagingManagerMock(ErrorCode._151));

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_11), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction succeeds if agency is present and transactions is COR
        /// </summary>
        [Fact]
        public async Task PV_11_WorkflowTasks_COA()
        {
            var agencyManagerMock = new Mock<IAgencyManager>();
            var workflowManagerMock = new Mock<IWorkflowManager>();
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    Agency = "99"
                },
                TransactionType = TransactionType.COA
            };

            agencyManagerMock.Setup(a => a.FindAsync("99")).ReturnsAsync("99");
            var test = new Mock<PV_11>(agencyManagerMock.Object, workflowManagerMock.Object, GetMessagingManagerMock(ErrorCode._151));

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_11), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction succeeds if transaction type is not COA or COR
        /// </summary>
        [Fact]
        public async Task PV_11_DMR_Valid()
        {
            var agencyManagerMock = new Mock<IAgencyManager>();
            var workflowManagerMock = new Mock<IWorkflowManager>();
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    Agency = "99"
                },
                TransactionType = TransactionType.DMR
            };

            agencyManagerMock.Setup(a => a.FindAsync("99")).ReturnsAsync("99");
            var test = new Mock<PV_11>(agencyManagerMock.Object, workflowManagerMock.Object, GetMessagingManagerMock(ErrorCode._151));

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_11), test.Object.Identifier);
        }
    }
}
