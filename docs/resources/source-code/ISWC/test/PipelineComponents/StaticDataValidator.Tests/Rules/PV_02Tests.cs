using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
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
    /// Tests rule PV_02
    /// </summary>
    public class PV_02Tests : TestBase
    {
        /// <summary>
        /// Check transaction passes validation if no work number provided (CAR record)
        /// </summary>
        [Fact]
        public async void PV_02_NoMatchesToCheck_Valid()
        {
            var submission = new Submission() {
                Model = new SubmissionModel()
                {
                    AdditionalIdentifiers = new List<AdditionalIdentifier>()
                }, 
                TransactionType = TransactionType.CAR, 
                SubmissionId = 1 
            };
            var workManagerMock = new Mock<IWorkManager>();
            var additionalIdentifierMock = new Mock<IAdditionalIdentifierManager>();
            var numberTypeMock = new Mock<INumberTypeManager>();
            var test = new Mock<PV_02>(workManagerMock.Object, additionalIdentifierMock.Object, numberTypeMock.Object, GetMessagingManagerMock(ErrorCode._168));

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check transaction is converted to CUR transaction if work number exists in db
        /// </summary>
        [Fact]
        public async void PV_02_WorkNumberAlreadyExistsInDb_Valid()
        {
            var testWorkNumber = new WorkNumber() { Number = "1" };
            var submission = new Submission()
            {
                Model = new SubmissionModel() {
                    WorkNumber = new WorkNumber() { Number = "1" },
                    AdditionalIdentifiers = new List<AdditionalIdentifier>()
                },
                TransactionType = TransactionType.CAR,
                ToBeProcessed = false
            };

            var workManagerMock = new Mock<IWorkManager>();
            var additionalIdentifierMock = new Mock<IAdditionalIdentifierManager>();
            var numberTypeMock = new Mock<INumberTypeManager>();
            workManagerMock.Setup(w => w.Exists(It.IsAny<WorkNumber>()))
                .ReturnsAsync(true);
            workManagerMock.Setup(w => w.FindAsync(It.IsAny<WorkNumber>()))
                .ReturnsAsync(new SubmissionModel() { WorkNumber = testWorkNumber });

            var test = new Mock<PV_02>(workManagerMock.Object, additionalIdentifierMock.Object, numberTypeMock.Object, GetMessagingManagerMock(ErrorCode._168));

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_02), test.Object.Identifier);
            Assert.Equal(TransactionType.CUR, response.Submission.TransactionType);
            Assert.True(response.Submission.ToBeProcessed);
        }

        /// <summary>
        /// Check transaction passes if work number does not exist in db
        /// </summary>
        [Fact]
        public async void PV_02_WorkNumberDoesNotExistInDb_Valid()
        {
            var testWorkNumber = new WorkNumber() { Number = "1" };
            var submission = new Submission()
            {
                Model = new SubmissionModel() 
                {
                    WorkNumber = new WorkNumber() { Number = "5" },
                    AdditionalIdentifiers = new List<AdditionalIdentifier>()
                },
                TransactionType = TransactionType.CAR,
                ToBeProcessed = false
            };

            var workManagerMock = new Mock<IWorkManager>();
            var additionalIdentifierMock = new Mock<IAdditionalIdentifierManager>();
            var numberTypeMock = new Mock<INumberTypeManager>();
            workManagerMock.Setup(w => w.FindManyAsync(It.IsAny<IEnumerable<Data.DataModels.AdditionalIdentifier>>(), false, DetailLevel.Full))
                .ReturnsAsync(new List<SubmissionModel>() { new SubmissionModel { WorkNumber = null } });

            var test = new Mock<PV_02>(workManagerMock.Object, additionalIdentifierMock.Object, numberTypeMock.Object, GetMessagingManagerMock(ErrorCode._168));

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_02), test.Object.Identifier);
            Assert.Equal(TransactionType.CAR, response.Submission.TransactionType);
            Assert.False(response.Submission.ToBeProcessed);
        }

        /// <summary>
        /// Check transaction is rejected if work number exists in db and disableAddUpdateSwitching is true
        /// </summary>
        [Fact]
        public async void PV_02_DisableAddUpdateSwitching_Invalid()
        {
            var testWorkNumber = new WorkNumber() { Number = "1" };
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    WorkNumber = new WorkNumber() { Number = "1" },
                    AdditionalIdentifiers = new List<AdditionalIdentifier>(),
                    DisableAddUpdateSwitching = true 
                },
                TransactionType = TransactionType.CAR,
                ToBeProcessed = false
            };


            var workManagerMock = new Mock<IWorkManager>();
            var additionalIdentifierMock = new Mock<IAdditionalIdentifierManager>();
            var numberTypeMock = new Mock<INumberTypeManager>();
            workManagerMock.Setup(w => w.Exists(It.IsAny<WorkNumber>()))
                .ReturnsAsync(true);
            workManagerMock.Setup(w => w.FindAsync(It.IsAny<WorkNumber>()))
                .ReturnsAsync(new SubmissionModel() { WorkNumber = testWorkNumber });

            var test = new Mock<PV_02>(workManagerMock.Object, additionalIdentifierMock.Object, numberTypeMock.Object, GetMessagingManagerMock(ErrorCode._168));

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_02), test.Object.Identifier);
            Assert.Equal(ErrorCode._168, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if work number does not exist in db and disableAddUpdateSwitching is true
        /// </summary>
        [Fact]
        public async void PV_02_DisableAddUpdateSwitching_Valid()
        {
            var testWorkNumber = new WorkNumber() { Number = "5" };
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    WorkNumber = new WorkNumber() { Number = "1" },
                    AdditionalIdentifiers = new List<AdditionalIdentifier>(),
                    DisableAddUpdateSwitching = true 
                },
                TransactionType = TransactionType.CAR,
                ToBeProcessed = false
            };

            var workManagerMock = new Mock<IWorkManager>();
            var additionalIdentifierMock = new Mock<IAdditionalIdentifierManager>();
            var numberTypeMock = new Mock<INumberTypeManager>();
            workManagerMock.Setup(w => w.FindManyAsync(It.IsAny<IEnumerable<Data.DataModels.AdditionalIdentifier>>(), false, DetailLevel.Full))
                .ReturnsAsync(new List<SubmissionModel>() { new SubmissionModel { WorkNumber = null } });

            var test = new Mock<PV_02>(workManagerMock.Object, additionalIdentifierMock.Object, numberTypeMock.Object, GetMessagingManagerMock(ErrorCode._168));

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_02), test.Object.Identifier);
            Assert.Equal(TransactionType.CAR, response.Submission.TransactionType);
            Assert.False(response.Submission.ToBeProcessed);
        }

        /// <summary>
        /// Check transaction is converted to CUR if the label identifier work code exists in the database
        /// </summary>
        [Fact]
        public async void PV_02_LabelIdentifierWorkCodeAlreadyExistsInDb_Valid()
        {
            // arrange
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    WorkNumber = new WorkNumber() { Number = "1" },
                    AdditionalIdentifiers = new List<AdditionalIdentifier>()
                    {
                        new AdditionalIdentifier()
                        {
                            WorkCode = "1",
                            SubmitterDPID = "dpid"
                        }
                    },
                    DisableAddUpdateSwitching = false,
                },
                TransactionType = TransactionType.CAR,
                ToBeProcessed = false
            };

            var labelIdentifierWorkCode = new WorkNumber() { Number = "12345" };
            var workManagerMock = new Mock<IWorkManager>();
            workManagerMock.Setup(w => w.Exists(It.IsAny<WorkNumber>()))
                 .ReturnsAsync(false);
            workManagerMock.Setup(w => w.FindManyAsync(It.IsAny<IEnumerable<Data.DataModels.AdditionalIdentifier>>(), false, DetailLevel.Full))
                .ReturnsAsync(new List<SubmissionModel>() { new SubmissionModel { WorkNumber = labelIdentifierWorkCode } });

            var additionalIdentifierManagerMock = new Mock<IAdditionalIdentifierManager>();
            additionalIdentifierManagerMock.Setup(w => w.Exists(It.IsAny<string>()))
                .ReturnsAsync(true);
            additionalIdentifierManagerMock.Setup(a => a.FindManyAsync(It.IsAny<string[]>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Data.DataModels.AdditionalIdentifier>() { new Data.DataModels.AdditionalIdentifier() });

            var numberTypeManagerMock = new Mock<INumberTypeManager>();
            numberTypeManagerMock.Setup(n => n.Exists(It.IsAny<string>()))
                .ReturnsAsync(true);
            numberTypeManagerMock.Setup(n => n.FindAsync(It.IsAny<string>()))
                .ReturnsAsync(new Data.DataModels.NumberType());

            var test = new Mock<PV_02>(workManagerMock.Object, additionalIdentifierManagerMock.Object, numberTypeManagerMock.Object, GetMessagingManagerMock(ErrorCode._168));

            // act
            var response = await test.Object.IsValid(submission);

            //assert
            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_02), test.Object.Identifier);
            Assert.Equal(TransactionType.CUR, response.Submission.TransactionType);
            Assert.True(response.Submission.ToBeProcessed);
            Assert.Equal(response.Submission.Model.WorkNumber, labelIdentifierWorkCode);
        }

        /// <summary>
        /// Check transaction passes if label identifier work code does not exist in db
        /// </summary>
        [Fact]
        public async void PV_02_LabelIdentifierWorkCodeDoesNotExistInDb_Valid()
        {
            // arrange
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    WorkNumber = new WorkNumber() { Number = "1" },
                    AdditionalIdentifiers = new List<AdditionalIdentifier>()
                    {
                        new AdditionalIdentifier()
                        {
                            WorkCode = "1",
                            SubmitterDPID = "dpid"
                        }
                    },
                    DisableAddUpdateSwitching = false,
                },
                TransactionType = TransactionType.CAR,
                ToBeProcessed = false
            };

            var workManagerMock = new Mock<IWorkManager>();
            workManagerMock.Setup(w => w.Exists(It.IsAny<WorkNumber>()))
                 .ReturnsAsync(false);

            var additionalIdentifierManagerMock = new Mock<IAdditionalIdentifierManager>();
            additionalIdentifierManagerMock.Setup(w => w.Exists(It.IsAny<string>()))
                .ReturnsAsync(false);

            var numberTypeManagerMock = new Mock<INumberTypeManager>();
            numberTypeManagerMock.Setup(n => n.Exists(It.IsAny<string>()))
                .ReturnsAsync(false);

            var test = new Mock<PV_02>(workManagerMock.Object, additionalIdentifierManagerMock.Object, numberTypeManagerMock.Object, GetMessagingManagerMock(ErrorCode._168));

            // act
            var response = await test.Object.IsValid(submission);

            //assert
            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_02), test.Object.Identifier);
            Assert.Equal(TransactionType.CAR, response.Submission.TransactionType);
            Assert.False(response.Submission.ToBeProcessed);
            Assert.Equal("1", submission.Model.WorkNumber.Number);
        }

        /// <summary>
        /// Check transaction is rejected if label identifier work code exists in db and disableAddUpdateSwitching is true
        /// </summary>
        [Fact]
        public async void PV_02_LabelIdentifierWorkCodeDisableUpdateSwitching_Invalid()
        {
            // arrange
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    WorkNumber = new WorkNumber() { Number = "1" },
                    AdditionalIdentifiers = new List<AdditionalIdentifier>()
                    {
                        new AdditionalIdentifier()
                        {
                            WorkCode = "1",
                            SubmitterDPID = "dpid"
                        }
                    },
                    DisableAddUpdateSwitching = true,
                },
                TransactionType = TransactionType.CAR,
                ToBeProcessed = false
            };

            var labelIdentifierWorkCode = new WorkNumber() { Number = "12345" };
            var workManagerMock = new Mock<IWorkManager>();
            workManagerMock.Setup(w => w.Exists(It.IsAny<WorkNumber>()))
                 .ReturnsAsync(false);
            workManagerMock.Setup(w => w.FindManyAsync(It.IsAny<IEnumerable<Data.DataModels.AdditionalIdentifier>>(), false, DetailLevel.Full))
                .ReturnsAsync(new List<SubmissionModel>() { new SubmissionModel { WorkNumber = labelIdentifierWorkCode } });

            var additionalIdentifierManagerMock = new Mock<IAdditionalIdentifierManager>();
            additionalIdentifierManagerMock.Setup(w => w.Exists(It.IsAny<string>()))
                .ReturnsAsync(true);
            additionalIdentifierManagerMock.Setup(a => a.FindManyAsync(It.IsAny<string[]>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Data.DataModels.AdditionalIdentifier>() { new Data.DataModels.AdditionalIdentifier() });

            var numberTypeManagerMock = new Mock<INumberTypeManager>();
            numberTypeManagerMock.Setup(n => n.Exists(It.IsAny<string>()))
                .ReturnsAsync(true);
            numberTypeManagerMock.Setup(n => n.FindAsync(It.IsAny<string>()))
                .ReturnsAsync(new Data.DataModels.NumberType());

            var test = new Mock<PV_02>(workManagerMock.Object, additionalIdentifierManagerMock.Object, numberTypeManagerMock.Object, GetMessagingManagerMock(ErrorCode._168));

            // act
            var response = await test.Object.IsValid(submission);

            //assert
            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_02), test.Object.Identifier);
            Assert.Equal(ErrorCode._168, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if the submission contains multiple label identifiers
        /// </summary>
        [Fact]
        public async void PV_02_MultipleLabelIdentifiers_Invalid()
        {
            // arrange
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    WorkNumber = new WorkNumber() { Number = "1" },
                    AdditionalIdentifiers = new List<AdditionalIdentifier>()
                    {
                        new AdditionalIdentifier()
                        {
                            WorkCode = "1",
                            SubmitterDPID = "dpid"
                        },
                        new AdditionalIdentifier()
                        {
                            WorkCode = "2",
                            SubmitterDPID = "dpid"
                        }
                    },
                    DisableAddUpdateSwitching = false,
                },
                TransactionType = TransactionType.CAR,
                ToBeProcessed = false
            };

            var additionalIdentifierManagerMock = new Mock<IAdditionalIdentifierManager>();
            var numberTypeManagerMock = new Mock<INumberTypeManager>();
            var workManagerMock = new Mock<IWorkManager>();
            workManagerMock.Setup(w => w.Exists(It.IsAny<WorkNumber>()))
                 .ReturnsAsync(false);


            var test = new Mock<PV_02>(workManagerMock.Object, additionalIdentifierManagerMock.Object, numberTypeManagerMock.Object, GetMessagingManagerMock(ErrorCode._248));

            // act
            var response = await test.Object.IsValid(submission);

            //assert
            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_02), test.Object.Identifier);
            Assert.Equal(ErrorCode._248, response.Submission.Rejection.Code);
        }
    }
}
