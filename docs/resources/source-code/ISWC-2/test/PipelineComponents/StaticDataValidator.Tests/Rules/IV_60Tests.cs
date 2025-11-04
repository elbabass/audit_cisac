using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;


namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// IV/60 tests
    /// </summary>
    public class IV_60Tests : TestBase
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
        /// check transaction fails if name and last name are null
        /// </summary>
        [Fact]
        public async Task IV_60_IpNameAndLastNameAreNull_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            testSubmission.Model.InterestedParties = new List<InterestedPartyModel>
            {
                new InterestedPartyModel
                {
                    IPNameNumber = 123456789,
                    Name = null,
                    LastName = null
                },
                new InterestedPartyModel
                {
                    IPNameNumber = 987654321,
                    Name = "Test Name",
                    LastName = "Test Last Name"
                }
            };

            var test = new Mock<IV_60>(GetMessagingManagerMock(ErrorCode._251), rulesManagerMock.Object);

            var response = await test.Object.IsValid(testSubmission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_60), test.Object.Identifier);
            Assert.Equal(ErrorCode._251, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// check transaction fails if name is null
        /// </summary>
        [Fact]
        public async Task IV_60_IpNameIsNull_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            testSubmission.Model.InterestedParties = new List<InterestedPartyModel>
            {
                new InterestedPartyModel
                {
                    IPNameNumber = 123456789,
                    Name = null,
                    LastName = "Test Last Name"
                },
                new InterestedPartyModel
                {
                    IPNameNumber = 987654321,
                    Name = "Test Name",
                    LastName = "Test Last Name"
                }
            };

            var test = new Mock<IV_60>(GetMessagingManagerMock(ErrorCode._251), rulesManagerMock.Object);

            var response = await test.Object.IsValid(testSubmission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_60), test.Object.Identifier);
            Assert.Equal(ErrorCode._251, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// check transaction fails if last name is null
        /// </summary>
        [Fact]
        public async Task IV_60_IpLastNameIsNull_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            testSubmission.Model.InterestedParties = new List<InterestedPartyModel>
            {
                new InterestedPartyModel
                {
                    IPNameNumber = 123456789,
                    Name = "Test Name",
                    LastName = null
                },
                new InterestedPartyModel
                {
                    IPNameNumber = 987654321,
                    Name = "Test Name",
                    LastName = "Test Last Name"
                }
            };

            var test = new Mock<IV_60>(GetMessagingManagerMock(ErrorCode._251), rulesManagerMock.Object);

            var response = await test.Object.IsValid(testSubmission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_60), test.Object.Identifier);
            Assert.Equal(ErrorCode._251, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// check transaction fails if name and last name are empty
        /// </summary>
        [Fact]
        public async Task IV_60_IpNameAndLastNameAreEmpty_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            testSubmission.Model.InterestedParties = new List<InterestedPartyModel>
            {
                new InterestedPartyModel
                {
                    IPNameNumber = 123456789,
                    Name = string.Empty,
                    LastName = string.Empty
                },
                new InterestedPartyModel
                {
                    IPNameNumber = 987654321,
                    Name = "Test Name",
                    LastName = "Test Last Name"
                }
            };

            var test = new Mock<IV_60>(GetMessagingManagerMock(ErrorCode._251), rulesManagerMock.Object);

            var response = await test.Object.IsValid(testSubmission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_60), test.Object.Identifier);
            Assert.Equal(ErrorCode._251, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// check transaction fails if name is empty
        /// </summary>
        [Fact]
        public async Task IV_60_IpNameIsEmpty_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            testSubmission.Model.InterestedParties = new List<InterestedPartyModel>
            {
                new InterestedPartyModel
                {
                    IPNameNumber = 123456789,
                    Name = string.Empty,
                    LastName = "Test Last Name"
                },
                new InterestedPartyModel
                {
                    IPNameNumber = 987654321,
                    Name = "Test Name",
                    LastName = "Test Last Name"
                }
            };

            var test = new Mock<IV_60>(GetMessagingManagerMock(ErrorCode._251), rulesManagerMock.Object);

            var response = await test.Object.IsValid(testSubmission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_60), test.Object.Identifier);
            Assert.Equal(ErrorCode._251, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// check transaction fails if last name is empty
        /// </summary>
        [Fact]
        public async Task IV_60_IpLastNameIsEmpty_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            testSubmission.Model.InterestedParties = new List<InterestedPartyModel>
            {
                new InterestedPartyModel
                {
                    IPNameNumber = 123456789,
                    Name = "Test Name",
                    LastName = string.Empty
                },
                new InterestedPartyModel
                {
                    IPNameNumber = 987654321,
                    Name = "Test Name",
                    LastName = "Test Last Name"
                }
            };

            var test = new Mock<IV_60>(GetMessagingManagerMock(ErrorCode._251), rulesManagerMock.Object);

            var response = await test.Object.IsValid(testSubmission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_60), test.Object.Identifier);
            Assert.Equal(ErrorCode._251, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// check transaction passes if name and last name are provided
        /// </summary>
        [Fact]
        public async Task IV_60_IpNameAndLastNameAreProvided_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            testSubmission.Model.InterestedParties = new List<InterestedPartyModel>
            {
                new InterestedPartyModel
                {
                    IPNameNumber = 123456789,
                    Name = "Test Name",
                    LastName = "Test Last Name"
                },
                new InterestedPartyModel
                {
                    IPNameNumber = 987654321,
                    Name = "Test Name",
                    LastName = "Test Last Name"
                }
            };

            var test = new Mock<IV_60>(GetMessagingManagerMock(ErrorCode._251), rulesManagerMock.Object);

            var response = await test.Object.IsValid(testSubmission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_60), test.Object.Identifier);
        }
    }
}
