using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule IV_02
    /// </summary>
    public class IV_11Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if no Society Code provided
        /// </summary>
        [Fact]
        public async void IV_11_Submission_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel() };

            var test = new Mock<IV_11>(GetMessagingManagerMock(ErrorCode._136));

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_11), test.Object.Identifier);
            Assert.Equal(ErrorCode._136, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if Society Code provided
        /// </summary>
        [Fact]
        public async void IV_11_Submission_Valid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission() { Model = new SubmissionModel { WorkNumber = new Bdo.Work.WorkNumber { Type = "12", Number = "003432" } } };

            var test = new Mock<IV_11>(GetMessagingManagerMock(ErrorCode._136));

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_11), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction fails if any worknumber not prodived in Additional agency workcodes
        /// </summary>
        [Fact]
        public async void IV_11_Submission_InValid_Additionial_Agency_Workcodes_()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    WorkNumber = new Bdo.Work.WorkNumber { Type = "1", Number = "1224" },
                    AdditionalAgencyWorkNumbers = new List<AdditionalAgencyWorkNumber>
                    {
                        new AdditionalAgencyWorkNumber{ WorkNumber = new WorkNumber{ Type = "2" , Number = string.Empty} }
                    }
                }, 
                TransactionSource = Bdo.Reports.TransactionSource.Publisher
            };

            var test = new Mock<IV_11>(GetMessagingManagerMock(ErrorCode._136));

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_11), test.Object.Identifier);
            Assert.Equal(ErrorCode._136, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if all worknumbers prodived in Additional agency workcodes
        /// </summary>
        [Fact]
        public async void IV_11_Submission_Valid_Additionial_Agency_Workcodes_()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    WorkNumber = new Bdo.Work.WorkNumber { Type = "1", Number = "1224" },
                    AdditionalAgencyWorkNumbers = new List<AdditionalAgencyWorkNumber>
                    {
                        new AdditionalAgencyWorkNumber { WorkNumber = new WorkNumber { Type = "2", Number = "1225" } }
                    }
                },
                TransactionSource = Bdo.Reports.TransactionSource.Publisher
            };

            var test = new Mock<IV_11>(GetMessagingManagerMock(ErrorCode._136));

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_11), test.Object.Identifier);
        }
    }
}
