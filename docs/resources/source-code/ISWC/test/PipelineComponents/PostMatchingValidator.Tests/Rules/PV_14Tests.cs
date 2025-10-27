using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Tests.Rules
{
    /// <summary>
    /// Tests for rule PV_14
    /// </summary>
    public class PV_14Tests : TestBase
    {
        /// <summary>
        /// Checks that the workflow flag is set to true if there are other participating agencies.
        /// </summary>
        [Fact]
        public async void PV_14_ParticipatingAgencies()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    ReasonCode = "",
                    Agency = "100",
                    InterestedParties = new List<InterestedPartyModel>() {
                        new InterestedPartyModel(){
                            CisacType = CisacInterestedPartyType.C
                        }
                    }

                },
                TransactionType = TransactionType.CDR,
                IsEligible = true
            };
            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(x => x.FindIswcModelAsync(It.IsAny<string>(), false, DetailLevel.Core))
                .ReturnsAsync(new IswcModel() {
                    VerifiedSubmissions = new List<VerifiedSubmissionModel> 
                    {
                        new VerifiedSubmissionModel()
                        {
                            Agency = "100"
                        },
                        new VerifiedSubmissionModel()
                        {
                            Agency = "3"
                        },
                        new VerifiedSubmissionModel()
                        {
                            Agency = "5"
                        },
                        new VerifiedSubmissionModel()
                        {
                            Agency = "7"
                        },
                        new VerifiedSubmissionModel()
                        {
                            Agency = "3"
                        }
                    } 
                });

            var test = new Mock<PV_14>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._117));

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.True(submission.Model.ApproveWorkflowTasks);
            Assert.Equal(nameof(PV_14), test.Object.Identifier);
        }

        /// <summary>
        /// Checks that the workflow flag is set to false if there are no other participating agencies.
        /// </summary>
        [Fact]
        public async void PV_14_NoParticipatingAgencies()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    ReasonCode = "",
                    Agency = "100",
                    InterestedParties = new List<InterestedPartyModel>() {
                        new InterestedPartyModel(){
                            CisacType = CisacInterestedPartyType.C
                        }
                    }

                },
                TransactionType = TransactionType.CDR,
                IsEligible = true
            };
            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(x => x.FindIswcModelAsync(It.IsAny<string>(), false, DetailLevel.Core))
               .ReturnsAsync(new IswcModel());

            var test = new Mock<PV_14>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._117));

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.False(submission.Model.ApproveWorkflowTasks);
            Assert.Equal(nameof(PV_14), test.Object.Identifier);
        }

        /// <summary>
        /// Checks that the workflow flag is set to false if the submission is not Iswc Eligible
        /// </summary>
        [Fact]
        public async void PV_14_NotEligible()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    ReasonCode = "",
                    Agency = "100",
                    InterestedParties = new List<InterestedPartyModel>() {
                        new InterestedPartyModel(){
                            CisacType = CisacInterestedPartyType.X
                        }
                    }

                },
                TransactionType = TransactionType.CDR,
                IsEligible = false
            };
            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(x => x.FindIswcModelAsync(It.IsAny<string>(), false, DetailLevel.Core))
                .ReturnsAsync(new IswcModel()
                {
                    VerifiedSubmissions = new List<VerifiedSubmissionModel>
                    {
                        new VerifiedSubmissionModel()
                        {
                            Agency = "100"
                        },
                        new VerifiedSubmissionModel()
                        {
                            Agency = "3"
                        },
                        new VerifiedSubmissionModel()
                        {
                            Agency = "5"
                        },
                        new VerifiedSubmissionModel()
                        {
                            Agency = "7"
                        },
                        new VerifiedSubmissionModel()
                        {
                            Agency = "3"
                        }
                    }
                });

            var test = new Mock<PV_14>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._117));

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.False(submission.Model.ApproveWorkflowTasks);
            Assert.Equal(nameof(PV_14), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction fails if Preferred ISWC does not exist in the db
        /// </summary>
        [Fact]
        public async void PV_14_PreferredIswcDoesNotExist_Invalid()
        {
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    PreferredIswc = "T2030000019"
                },
                TransactionType = TransactionType.CDR,
                IsEligible = true
            };

            var workManagerMock = new Mock<IWorkManager>();

            workManagerMock.Setup(i => i.Exists("T2030000019")).ReturnsAsync(false);

            var test = new Mock<PV_14>(workManagerMock.Object, GetMessagingManagerMock(ErrorCode._117));

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_14), test.Object.Identifier);
            Assert.Equal(ErrorCode._117, response.Submission.Rejection.Code);
        }
    }
}
