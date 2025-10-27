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
    /// IV/59 tests
    /// </summary>
    public class IV_59Tests : TestBase
    {
        private Submission testSubmission = new Submission()
        {
            Model = new SubmissionModel()
            {
                Agency = "122",
                WorkNumber = new WorkNumber { Number = "TestWork1", Type = "122" },
            },
            RequestType = Bdo.Edi.RequestType.Label,
        };


        /// <summary>
        /// check transaction fails if SubmitterDPID is null
        /// </summary>
        [Fact]
        public async void IV_59_SubmitterDpIdIsNull_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            testSubmission.Model.AdditionalIdentifiers = new List<AdditionalIdentifier>
            {
                new AdditionalIdentifier
                {
                    SubmitterDPID = null,
                }
            };

            var test = new Mock<IV_59>(GetMessagingManagerMock(ErrorCode._250), rulesManagerMock.Object);

            var response = await test.Object.IsValid(testSubmission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_59), test.Object.Identifier);
            Assert.Equal(ErrorCode._250, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// check transaction fails if SubmitterDPID is empty
        /// </summary>
        [Fact]
        public async void IV_59_SubmitterDpIdIsEmpty_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            testSubmission.Model.AdditionalIdentifiers = new List<AdditionalIdentifier>
            {
                new AdditionalIdentifier
                {
                    SubmitterDPID = string.Empty,
                }
            };

            var test = new Mock<IV_59>(GetMessagingManagerMock(ErrorCode._250), rulesManagerMock.Object);

            var response = await test.Object.IsValid(testSubmission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_59), test.Object.Identifier);
            Assert.Equal(ErrorCode._250, response.Submission.Rejection.Code);

        }

        /// <summary>
        /// check transaction fails if addtional identifiers are not provided
        /// </summary>
        [Fact]
        public async void IV_59_AddtionalIdentifiersNotProvided_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();

            var test = new Mock<IV_59>(GetMessagingManagerMock(ErrorCode._250), rulesManagerMock.Object);

            var response = await test.Object.IsValid(testSubmission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_59), test.Object.Identifier);
            Assert.Equal(ErrorCode._250, response.Submission.Rejection.Code);

        }

        /// <summary>
        /// check transaction passes if SubmitterDPID is provided
        /// </summary>
        [Fact]
        public async void IV_59_SubmitterDpIdProvided_Valid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            testSubmission.Model.AdditionalIdentifiers = new List<AdditionalIdentifier>
            {
                new AdditionalIdentifier
                {
                    SubmitterDPID = "P123456789",
                }
            };

            var test = new Mock<IV_59>(GetMessagingManagerMock(ErrorCode._250), rulesManagerMock.Object);

            var response = await test.Object.IsValid(testSubmission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_59), test.Object.Identifier);
        }
    }
}
