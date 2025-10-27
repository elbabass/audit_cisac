using System.Collections.Generic;
using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using Xunit;
using SpanishPoint.Azure.Iswc.PipelineComponents.MetadataStandardizationValidator.Rules;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MetadataStandardizationValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule MD_18
    /// </summary>
    public class MD_18Tests : TestBase
    {
        /// <summary>
        /// Transaction fails if submission CheckSum already exists with same hash value and fails with error code 174 
        /// </summary>
        [Fact]
        public async void MD_18_Submission_Invalid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManagerMock = new Mock<IWorkManager>();
            var agencyManagerMock = new Mock<IAgencyManager>();

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("EnableChecksumValidation")).ReturnsAsync(true);
            agencyManagerMock.Setup(a => a.ChecksumEnabled("128")).ReturnsAsync(true);

            var submission = new Submission()
            {
                TransactionType = Bdo.Edi.TransactionType.CAR,
                Model = new SubmissionModel()
                {
                    PreferredIswc = string.Empty,
                    Agency = "128",
                    Titles = new List<Title>
                    {
                        new Title
                        {
                            Name = "HASH TEST",
                            Type = TitleType.OT
                        }
                    },
                    WorkNumber = new WorkNumber
                    {
                        Number = "test1",
                        Type = "IMRO"
                    },
                    InterestedParties = new List<InterestedPartyModel>()
                    {
                        new InterestedPartyModel
                        {
                           IPNameNumber = 589238793,
                           Type = InterestedPartyType.C
                        }
                    }
                }
            };

            var checksum = new SubmissionChecksum { Hash = "89ca43e62e24780e37f55a0a1e7ebe21" };

            workManagerMock.Setup(x => x.GetChecksum("IMRO", "test1")).ReturnsAsync(checksum);
            var test = new Mock<MD_18>(GetMessagingManagerMock(ErrorCode._174), workManagerMock.Object, rulesManagerMock.Object, agencyManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            workManagerMock.Verify(s => s.GetChecksum("IMRO", "test1"), Times.Once);

            Assert.Equal(nameof(MD_18), test.Object.Identifier);
            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._174, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Transction succeeds if submission CheckSum doesn't exist and new checksum value added to cosmos db
        /// </summary>
        [Fact]
        public async void MD_18_Submission_Valid_CheckSum_Does_Not_Exist()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManagerMock = new Mock<IWorkManager>();
            var agencyManagerMock = new Mock<IAgencyManager>();

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("EnableChecksumValidation")).ReturnsAsync(true);
            agencyManagerMock.Setup(a => a.ChecksumEnabled("128")).ReturnsAsync(true);

            var submission = new Submission()
            {
                TransactionType = Bdo.Edi.TransactionType.CAR,
                Model = new SubmissionModel()
                {
                    Agency = "128",
                    Titles = new List<Title>
                    {
                        new Title
                        {
                            Name = "HASH TEST",
                            Type = TitleType.OT
                        }
                    },
                    WorkNumber = new WorkNumber
                    {
                        Number = "test1",
                        Type = "IMRO"
                    },
                    InterestedParties = new List<InterestedPartyModel>()
                    {
                        new InterestedPartyModel
                        {
                           IPNameNumber = 589238793,
                           Type = InterestedPartyType.C
                        }
                    }
                }
            };

            var checksum = new SubmissionChecksum { Hash = "" };

            workManagerMock.Setup(x => x.GetChecksum("IMRO", "test1")).ReturnsAsync(checksum);

            var test = new Mock<MD_18>(GetMessagingManagerMock(ErrorCode._174), workManagerMock.Object, rulesManagerMock.Object, agencyManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            workManagerMock.Verify(s => s.GetChecksum("IMRO", "test1"), Times.Once);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_18), test.Object.Identifier);
        }

        /// <summary>
        /// Transction succeeds if submission CheckSum exists but hash values are different and 
        /// checksum hash value is updated in cosmos db
        /// </summary>
        [Fact]
        public async void MD_18_Submission_Valid_CheckSum_Hash_Values_Are_Different()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManagerMock = new Mock<IWorkManager>();
            var agencyManagerMock = new Mock<IAgencyManager>();

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("EnableChecksumValidation")).ReturnsAsync(true);
            agencyManagerMock.Setup(a => a.ChecksumEnabled("128")).ReturnsAsync(true);

            var submission = new Submission()
            {
                TransactionType = Bdo.Edi.TransactionType.CUR,
                Model = new SubmissionModel()
                {
                    PreferredIswc = "T100000",
                    Agency = "128",
                    Titles = new List<Title>
                    {
                        new Title
                        {
                            Name = "HASH TEST",
                            Type = TitleType.OT
                        },
                        new Title
                        {
                            Name = "HASH TEST",
                            Type = TitleType.AT
                        }
                    },
                    WorkNumber = new WorkNumber
                    {
                        Number = "test1",
                        Type = "IMRO"
                    },
                    InterestedParties = new List<InterestedPartyModel>()
                    {
                        new InterestedPartyModel
                        {
                           IPNameNumber = 589238793,
                           Type = InterestedPartyType.C
                        }
                    }
                }
            };

            var checksum = new SubmissionChecksum { Hash = "89ca43e62e24780e37f55a0a1e7ebe21" };

            workManagerMock.Setup(x => x.GetChecksum("IMRO", "test1")).ReturnsAsync(checksum);
            var test = new Mock<MD_18>(GetMessagingManagerMock(ErrorCode._174), workManagerMock.Object, rulesManagerMock.Object, agencyManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            workManagerMock.Verify(s => s.GetChecksum("IMRO", "test1"), Times.Once);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_18), test.Object.Identifier);
        }

        /// <summary>
        /// Checking existing submission returns false with all possible values
        /// </summary>
        [Fact]
        public async void MD_18_AllPossibleValues()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManagerMock = new Mock<IWorkManager>();
            var agencyManagerMock = new Mock<IAgencyManager>();

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("EnableChecksumValidation")).ReturnsAsync(true);
            agencyManagerMock.Setup(a => a.ChecksumEnabled("128")).ReturnsAsync(true);

            var submission = new Submission()
            {
                TransactionType = Bdo.Edi.TransactionType.CUR,
                Model = new SubmissionModel()
                {
                    PreferredIswc = "T100000",
                    Agency = "128",
                    Titles = new List<Title>
                    {
                        new Title
                        {
                            Name = "HASH TEST",
                            Type = TitleType.OT
                        },
                    },
                    WorkNumber = new WorkNumber
                    {
                        Number = "test1",
                        Type = "IMRO"
                    },
                    InterestedParties = new List<InterestedPartyModel>()
                    {
                        new InterestedPartyModel
                        {
                           IPNameNumber = 589238793,
                           Type = InterestedPartyType.C
                        }
                    },
                    Disambiguation = true,
                    DisambiguationReason = DisambiguationReason.DIA,
                    BVLTR = BVLTR.B,
                    
                    DisambiguateFrom = new List<DisambiguateFrom>()
                    {
                        new DisambiguateFrom
                        {
                            Iswc = "T100001"
                        }
                    },

                    Instrumentation = new Instrumentation[]
                    {
                        new Instrumentation ("ABC")
                    },

                    Performers = new List<Performer>()
                    {
                        new Performer
                        {
                            FirstName = "FN",
                            LastName = "LN"
                        }
                    }
                }
            };

            var checksum = new SubmissionChecksum { Hash = "b442d4e8861213969b6e111794da1074" };

            workManagerMock.Setup(x => x.GetChecksum("IMRO", "test1")).ReturnsAsync(checksum);
            var test = new Mock<MD_18>(GetMessagingManagerMock(ErrorCode._174), workManagerMock.Object, rulesManagerMock.Object, agencyManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            workManagerMock.Verify(s => s.GetChecksum("IMRO", "test1"), Times.Once);

            Assert.Equal(nameof(MD_18), test.Object.Identifier);
            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._174, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Transction succeeds if agency has EnableCheckSumValidation set to false
        /// </summary>
        [Fact]
        public async void MD_18_Submission_Valid_Disabled_By_Agency()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManagerMock = new Mock<IWorkManager>();
            var agencyManagerMock = new Mock<IAgencyManager>();

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("EnableChecksumValidation")).ReturnsAsync(true);

            agencyManagerMock.Setup(a => a.ChecksumEnabled("128")).ReturnsAsync(false);

            var submission = new Submission()
            {
                TransactionType = Bdo.Edi.TransactionType.CAR,
                Model = new SubmissionModel { Agency = "128" }
            };

            var test = new Mock<MD_18>(GetMessagingManagerMock(ErrorCode._174), workManagerMock.Object, rulesManagerMock.Object, agencyManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_18), test.Object.Identifier);
        }

        /// <summary>
        /// Check that submission is true when Parameter value is false
        /// </summary>
        [Fact]
        public async void MD_18_SetToFalse()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManagerMock = new Mock<IWorkManager>();
            var agencyManagerMock = new Mock<IAgencyManager>();

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("EnableChecksumValidation")).ReturnsAsync(false);

            agencyManagerMock.Setup(a => a.ChecksumEnabled("128")).ReturnsAsync(false);

            var submission = new Submission()
            {
                TransactionType = Bdo.Edi.TransactionType.CAR,
                Model = new SubmissionModel { Agency = "128" }
            };

            var test = new Mock<MD_18>(GetMessagingManagerMock(ErrorCode._174), workManagerMock.Object, rulesManagerMock.Object, agencyManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_18), test.Object.Identifier);
        }

        /// <summary>
        /// Check that submission is true during Preview Disambiguation
        /// </summary>
        [Fact]
        public async void MD_18_PreviewDisambiguationTrue()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManagerMock = new Mock<IWorkManager>();
            var agencyManagerMock = new Mock<IAgencyManager>();

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("EnableChecksumValidation")).ReturnsAsync(true);

            agencyManagerMock.Setup(a => a.ChecksumEnabled("128")).ReturnsAsync(false);

            var submission = new Submission()
            {
                TransactionType = Bdo.Edi.TransactionType.CAR,
                Model = new SubmissionModel 
                { 
                    Agency = "128", 
                    PreviewDisambiguation = true
                }
            };

            var test = new Mock<MD_18>(GetMessagingManagerMock(ErrorCode._174), workManagerMock.Object, rulesManagerMock.Object, agencyManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(MD_18), test.Object.Identifier);
        }
    }
}
