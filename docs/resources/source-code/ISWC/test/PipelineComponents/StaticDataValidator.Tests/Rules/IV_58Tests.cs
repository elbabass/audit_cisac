using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// IV/58 tests
    /// </summary>
    public class IV_58Tests : TestBase
    {
        private Submission testSubmission = new Submission()
        {
            Model = new SubmissionModel()
            {
                Agency = "122",
                WorkNumber = new WorkNumber { Number = "TestWork1", Type = "122" },
                PreviewDisambiguation = true
            },
            RequestSource = Framework.Http.Requests.RequestSource.REST

        };


        /// <summary>
        /// check CAR transaction fails if rquest is not from agency portal
        /// </summary>
        [Fact]
        public async void IV_58_InValid_CAR()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            testSubmission.TransactionType = Bdo.Edi.TransactionType.CAR;

            var test = new Mock<IV_58>(GetMessagingManagerMock(ErrorCode._181), rulesManagerMock.Object);

            var response = await test.Object.IsValid(testSubmission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_58), test.Object.Identifier);
            Assert.Equal(ErrorCode._181, response.Submission.Rejection.Code);

        }

        /// <summary>
        /// check CAR transaction passes if rquest is from agency portal
        /// </summary>
        [Fact]
        public async void IV_58_Valid_CAR()
        {
            var rulesManagerMock = new Mock<IRulesManager>();

            testSubmission.TransactionType = Bdo.Edi.TransactionType.CAR;
            testSubmission.RequestSource = Framework.Http.Requests.RequestSource.PORTAL;

            var test = new Mock<IV_58>(GetMessagingManagerMock(ErrorCode._181), rulesManagerMock.Object);

            var response = await test.Object.IsValid(testSubmission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_58), test.Object.Identifier);
        }

        /// <summary>
        /// check CUR transaction fails if rquest is not from agency portal
        /// </summary>
        [Fact]
        public async void IV_58_InValid_CUR()
        {
            var rulesManagerMock = new Mock<IRulesManager>();

            testSubmission.TransactionType = Bdo.Edi.TransactionType.CUR;

            var test = new Mock<IV_58>(GetMessagingManagerMock(ErrorCode._181), rulesManagerMock.Object);

            var response = await test.Object.IsValid(testSubmission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_58), test.Object.Identifier);
        }
        
        /// <summary>
        /// check CUR transaction passes if rquest is from agency portal
        /// </summary>
        [Fact]
        public async void IV_58_Valid_CUR()
        {
            var rulesManagerMock = new Mock<IRulesManager>();

            testSubmission.TransactionType = Bdo.Edi.TransactionType.CUR;
            testSubmission.RequestSource = Framework.Http.Requests.RequestSource.PORTAL;

            var test = new Mock<IV_58>(GetMessagingManagerMock(ErrorCode._181), rulesManagerMock.Object);

            var response = await test.Object.IsValid(testSubmission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_58), test.Object.Identifier);
        }
    }
}
