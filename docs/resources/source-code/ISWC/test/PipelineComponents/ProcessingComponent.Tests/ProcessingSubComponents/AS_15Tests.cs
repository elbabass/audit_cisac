using AutoMapper;
using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.MatchingEngine;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Processing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Tests.ProcessingSubComponents
{
    /// <summary>
    /// Checks for processing scenario AS_15
    /// </summary>
    public class AS_15Tests : ProcessingTestBase
    {
        /// <summary>
        /// Check full AS_15 mapping from submission
        /// </summary>
        [Fact]
        public void AS_15_Valid()
        {
            // Arrange
            var workManagerMock = new Mock<IWorkManager>().Object;
            var mapperMock = new Mock<IMapper>().Object;
            var validRequestTypes = new List<RequestType>() { RequestType.Label };

            // Act
            var as15Mock = new Mock<AS_15>(workManagerMock, mapperMock).Object;

            // Assert
            Assert.Equal(PreferedIswcType.Existing, as15Mock.PreferedIswcType);
            Assert.Null(as15Mock.IsEligible);
            Assert.Contains(TransactionType.CAR, as15Mock.ValidTransactionTypes);
            Assert.Equal(validRequestTypes, as15Mock.ValidRequestTypes);
            Assert.Equal(nameof(AS_15), as15Mock.Identifier);
        }

        /// <summary>
        /// Check submission with isrc matches is assigned the correct preferred iswc
        /// Also check that match with preferred iswc status is correctly assigned to submission as opposed to provisional status
        /// </summary>
        [Fact]
        public async Task AS_15_Submission_With_Match_Assigned_Preferred_ISWC()
        {
            // Arrange
            var workManagerMock = new Mock<IWorkManager>();
            var mapperMock = new Mock<IMapper>();

            mapperMock.Setup(m => m.Map<VerifiedSubmissionModel>(It.IsAny<SubmissionModel>()))
                .Returns(new VerifiedSubmissionModel() { WorkInfoID = 1 });

            workManagerMock.Setup(m => m.UpdateAsync(It.IsAny<Submission>(), It.IsAny<bool>()))
                .ReturnsAsync(It.IsAny<long>());
            workManagerMock.Setup(m => m.FindManyAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<bool>(), It.IsAny<DetailLevel>()))
                .ReturnsAsync(new List<IswcModel>()
                {
                    new IswcModel()
                    {
                        VerifiedSubmissions = new List<VerifiedSubmissionModel>()
                        {
                            new VerifiedSubmissionModel()
                            {
                                LinkedTo = ""
                            }
                        }
                    }
                });

            var as15Mock = new Mock<AS_15>(workManagerMock.Object, mapperMock.Object).Object;

            var submission = new Submission
            {
                SubmissionId = 1,
                IsEligible = false,
                IsrcMatchedResult = new MatchResult
                {
                    InputWorkId = 1,
                    Matches = new List<MatchingWork>()
                    {
                        new MatchingWork()
                        {
                            Numbers = new List<WorkNumber>() { new WorkNumber() { Number = "T123", Type = "ISWC" } },
                            IswcStatus = 2
                        },
                        new MatchingWork()
                        {
                            Numbers = new List<WorkNumber>() { new WorkNumber() { Number = "T345", Type = "ISWC" } },
                            IswcStatus = 1
                        }
                    }
                },
                Model = new SubmissionModel() { }
            };

            // Act
            var response = await as15Mock.ProcessSubmission(submission);

            // Assert
            mapperMock.Verify(m => m.Map<VerifiedSubmissionModel>(It.IsAny<SubmissionModel>()), Times.Once);
            workManagerMock.Verify(m => m.UpdateAsync(It.IsAny<Submission>(), It.IsAny<bool>()), Times.Once);
            workManagerMock.Verify(m => m.FindManyAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<bool>(), It.IsAny<DetailLevel>()), Times.Once);
            Assert.Equal(response.VerifiedSubmissions.FirstOrDefault().LinkedTo, submission.IsrcMatchedResult.Matches.First(x => x.IswcStatus == 1).Numbers.First(x => x.Type == "ISWC").Number);

        }

        /// <summary>
        /// Check submission with no isrc matches is not reassigned its preferred iswc
        /// </summary>
        [Fact]
        public async Task AS_15_Submission_Without_Match_Not_Assigned_Preferred_ISWC()
        {
            // Arrange
            var workManagerMock = new Mock<IWorkManager>();
            var mapperMock = new Mock<IMapper>();

            mapperMock.Setup(m => m.Map<VerifiedSubmissionModel>(It.IsAny<SubmissionModel>()))
                .Returns(new VerifiedSubmissionModel() { WorkInfoID = 1 });

            workManagerMock.Setup(m => m.UpdateAsync(It.IsAny<Submission>(), It.IsAny<bool>()))
                .ReturnsAsync(It.IsAny<long>());
            workManagerMock.Setup(m => m.FindManyAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<bool>(), It.IsAny<DetailLevel>()))
                .ReturnsAsync(new List<IswcModel>()
                {
                    new IswcModel()
                    {
                        VerifiedSubmissions = new List<VerifiedSubmissionModel>()
                        {
                            new VerifiedSubmissionModel()
                            {
                                LinkedTo = ""
                            }
                        }
                    }
                });

            var as15Mock = new Mock<AS_15>(workManagerMock.Object, mapperMock.Object).Object;

            var submission = new Submission
            {
                SubmissionId = 1,
                IsEligible = false,
                IsrcMatchedResult = new MatchResult
                {
                    InputWorkId = 1,
                },
                Model = new SubmissionModel() { },
                IswcModel = new IswcModel ()
                {
                    VerifiedSubmissions = new List<VerifiedSubmissionModel>()
                    {
                        new VerifiedSubmissionModel()
                        {
                            LinkedTo = "T123"
                        }
                    }
                }
            };

            // Act
            var response = await as15Mock.ProcessSubmission(submission);

            // Assert
            mapperMock.Verify(m => m.Map<VerifiedSubmissionModel>(It.IsAny<SubmissionModel>()), Times.Never);
            workManagerMock.Verify(m => m.UpdateAsync(It.IsAny<Submission>(), It.IsAny<bool>()), Times.Never);
            workManagerMock.Verify(m => m.FindManyAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<bool>(), It.IsAny<DetailLevel>()), Times.Never);
            Assert.Equal(response.VerifiedSubmissions.FirstOrDefault().LinkedTo, submission.IswcModel.VerifiedSubmissions.FirstOrDefault().LinkedTo);
        }
    }
}
