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
    /// IV/57 tests
    /// </summary>
    public class IV_57Tests : TestBase
    {
        private Submission testSubmission = new Submission()
        {
            Model = new SubmissionModel()
            {
                Agency = "122",
                WorkNumber = new WorkNumber { Number = "TestWork1", Type = "122" },
                AdditionalAgencyWorkNumbers = new List<AdditionalAgencyWorkNumber>
                {
                    new AdditionalAgencyWorkNumber { WorkNumber = new WorkNumber { Number = "TestWork2", Type = "123" }},
                    new AdditionalAgencyWorkNumber { WorkNumber = new WorkNumber { Number = "TestWork3", Type = "124" }}
                }
            }
        };

        /// <summary>
        /// check CAR transaction fails if addtional workcoes provided
        /// </summary>
        [Fact]
        public async void IV_57_InValid_CAR()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var workmanagerMock = new Mock<IWorkManager>();
            var workNumbers = testSubmission.Model.AdditionalAgencyWorkNumbers.Select(x => x.WorkNumber).ToList();

            workmanagerMock.Setup(x => x.AnyExistAsync(workNumbers)).ReturnsAsync(true);
            testSubmission.TransactionType = Bdo.Edi.TransactionType.CAR;

            var test = new Mock<IV_57>(workmanagerMock.Object, rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._182));

            var response = await test.Object.IsValid(testSubmission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_57), test.Object.Identifier);
            Assert.Equal(ErrorCode._182, response.Submission.Rejection.Code);

        }

        /// <summary>
        /// check CAR transaction passes if IAS and has addtional workcoes 
        /// </summary>
        [Fact]
        public async void IV_57_Valid_IAS_CAR()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var workmanagerMock = new Mock<IWorkManager>();
            var workNumbers = testSubmission.Model.AdditionalAgencyWorkNumbers.Select(x => x.WorkNumber).ToList();
            workNumbers.Add(testSubmission.Model.WorkNumber);

            workmanagerMock.Setup(x => x.AnyExistAsync(workNumbers)).ReturnsAsync(false);
            testSubmission.TransactionType = Bdo.Edi.TransactionType.CAR;
            testSubmission.TransactionSource = Bdo.Reports.TransactionSource.Publisher;
            var test = new Mock<IV_57>(workmanagerMock.Object, rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._176));

            var response = await test.Object.IsValid(testSubmission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_57), test.Object.Identifier);
        }
    }
}
