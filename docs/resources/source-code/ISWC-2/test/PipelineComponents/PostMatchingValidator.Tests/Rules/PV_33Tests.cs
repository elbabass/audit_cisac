using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Bdo.MatchingEngine;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Tests.Rules
{
    /// <summary>
    /// Tests for rule PV_33
    /// </summary>
    public class PV_33Tests : TestBase
    {
        private Submission GetValidSubmission()
        {
            return new Submission
            {
                SearchWorks = new List<WorkModel> { new WorkModel() },
                Model = new SubmissionModel
                {
                    InterestedParties = new List<InterestedPartyModel>
                    {
                        new InterestedPartyModel
                        {
                            IpBaseNumber = "IPB123",
                            LastName = "Smith",
                            Age = 30,
                            AgeTolerance = 5
                        }
                    }
                }
            };
        }

        private Submission CreateSubmissionWithInterestedParty(string ipBaseNumber = null, int? ipNameNumber = null, DateTime? birth = null, DateTime? death = null, int? age = null, int? ageTolerance = null, List<string> affiliations = null)
        {
            return new Submission
            {
                Model = new SubmissionModel
                {
                    InterestedParties = new List<InterestedPartyModel>
                    {
                        new InterestedPartyModel
                        {
                            IpBaseNumber = ipBaseNumber,
                            IPNameNumber = ipNameNumber,
                            LastName = "Smith",
                            BirthDate = birth,
                            DeathDate = death,
                            Age = age,
                            AgeTolerance = ageTolerance
                        }
                    },
                    Affiliations = affiliations ?? new List<string>()
                },
                SearchWorks = new List<WorkModel> { new WorkModel() }
            };
        }

        private PV_33 GetRule(List<IswcModel> iswcModels = null,
                              List<InterestedPartyModel> ips = null,
                              List<Agreement> agreements = null)
        {
            return new PV_33(
                GetMessagingManagerMock(ErrorCode._180),
                GetWorkManagerMock(iswcModels).Object,
                GetInterestedPartyManagerMock(ips).Object,
                GetAgreementManagerMock(agreements).Object
            );
        }

        /// <summary>
        /// Check valid match when InterestedParty is found in SearchedIswcModels
        /// </summary>
        [Fact]
        public async Task PV_33_SearchedIswcModels_Contains_ValidMatch()
        {
            var submission = GetValidSubmission();

            var iswc = new IswcModel
            {
                VerifiedSubmissions = new List<VerifiedSubmissionModel>
                {
                    new VerifiedSubmissionModel
                    {
                        InterestedParties = new List<InterestedPartyModel>
                        {
                            new InterestedPartyModel { IpBaseNumber = "IPB123" }
                        }
                    }
                }
            };

            submission.SearchedIswcModels = new List<IswcModel> { iswc };

            var rule = GetRule(
                iswcModels: new List<IswcModel> { iswc },
                ips: new List<InterestedPartyModel> { new InterestedPartyModel { IpBaseNumber = "IPB123" } }
            );

            var result = await rule.IsValid(submission);
            Assert.True(result.IsValid);
        }

        /// <summary>
        /// Check invalid when no InterestedParty matches in SearchedIswcModels
        /// </summary>
        [Fact]
        public async Task PV_33_SearchedIswcModels_NoMatch_Invalid()
        {
            var submission = GetValidSubmission();

            var iswc = new IswcModel
            {
                VerifiedSubmissions = new List<VerifiedSubmissionModel>
                {
                    new VerifiedSubmissionModel
                    {
                        InterestedParties = new List<InterestedPartyModel>
                        {
                            new InterestedPartyModel { IpBaseNumber = "DIFFERENT" }
                        }
                    }
                }
            };

            submission.SearchedIswcModels = new List<IswcModel> { iswc };

            var rule = GetRule(
                iswcModels: new List<IswcModel> { iswc },
                ips: new List<InterestedPartyModel> { new InterestedPartyModel { IpBaseNumber = "IPB123" } }
            );

            var result = await rule.IsValid(submission);
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// Check invalid when InterestedParty birth date does not match
        /// </summary>
        [Fact]
        public async Task PV_33_BirthDateMismatch_Invalid()
        {
            var birthDate = new DateTime(1990, 1, 1);

            var submission = CreateSubmissionWithInterestedParty(ipBaseNumber: "IPB123", birth: birthDate);

            var rule = GetRule(
                iswcModels: new List<IswcModel>
                {
                    new IswcModel
                    {
                        VerifiedSubmissions = new List<VerifiedSubmissionModel>
                        {
                            new VerifiedSubmissionModel
                            {
                                InterestedParties = new List<InterestedPartyModel>
                                {
                                    new InterestedPartyModel { IpBaseNumber = "IPB123" }
                                }
                            }
                        }
                    }
                },
                ips: new List<InterestedPartyModel>
                {
                    new InterestedPartyModel { IpBaseNumber = "IPB123", BirthDate = new DateTime(1980, 1, 1) }
                }
            );

            var result = await rule.IsValid(submission);
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// Check valid when InterestedParty birth date matches
        /// </summary>
        [Fact]
        public async Task PV_33_BirthDateMatch_Valid()
        {
            var birthDate = new DateTime(1990, 1, 1);

            var submission = CreateSubmissionWithInterestedParty(ipBaseNumber: "IPB123", birth: birthDate);

            var iswcModel = new IswcModel
            {
                VerifiedSubmissions = new List<VerifiedSubmissionModel>
                {
                    new VerifiedSubmissionModel
                    {
                        InterestedParties = new List<InterestedPartyModel>
                        {
                            new InterestedPartyModel { IpBaseNumber = "IPB123" }
                        }
                    }
                }
            };

            submission.SearchedIswcModels = new List<IswcModel> { iswcModel };

            var rule = GetRule(
                iswcModels: new List<IswcModel> { iswcModel },
                ips: new List<InterestedPartyModel>
                {
                    new InterestedPartyModel
                    {
                        IpBaseNumber = "IPB123",
                        BirthDate = birthDate
                    }
                }
            );

            var result = await rule.IsValid(submission);
            Assert.True(result.IsValid);
        }

        /// <summary>
        /// Check invalid when IP age difference exceeds tolerance
        /// </summary>
        [Fact]
        public async Task PV_33_AgeMismatchBeyondTolerance_Invalid()
        {
            var submission = CreateSubmissionWithInterestedParty(ipBaseNumber: "IPB123", age: 30, ageTolerance: 2);

            var iswcModel = new IswcModel
            {
                VerifiedSubmissions = new List<VerifiedSubmissionModel>
                {
                    new VerifiedSubmissionModel
                    {
                        InterestedParties = new List<InterestedPartyModel>
                        {
                            new InterestedPartyModel { IpBaseNumber = "IPB123" }
                        }
                    }
                }
            };

            submission.SearchedIswcModels = new List<IswcModel> { iswcModel };

            var rule = GetRule(
                iswcModels: new List<IswcModel> { iswcModel },
                ips: new List<InterestedPartyModel>
                {
                    new InterestedPartyModel { IpBaseNumber = "IPB123", Age = 35 }
                }
            );

            var result = await rule.IsValid(submission);
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// Check valid when IP age difference is within tolerance
        /// </summary>
        [Fact]
        public async Task PV_33_AgeWithinTolerance_Valid()
        {
            var submission = CreateSubmissionWithInterestedParty(ipBaseNumber: "IPB123", age: 30, ageTolerance: 5);

            var iswcModel = new IswcModel
            {
                VerifiedSubmissions = new List<VerifiedSubmissionModel>
                {
                    new VerifiedSubmissionModel
                    {
                        InterestedParties = new List<InterestedPartyModel>
                        {
                            new InterestedPartyModel { IpBaseNumber = "IPB123" }
                        }
                    }
                }
            };

            submission.SearchedIswcModels = new List<IswcModel> { iswcModel };

            var rule = GetRule(
                iswcModels: new List<IswcModel> { iswcModel },
                ips: new List<InterestedPartyModel>
                {
                    new InterestedPartyModel
                    {
                        IpBaseNumber = "IPB123",
                        Age = 33 
                    }
                }
            );

            var result = await rule.IsValid(submission);
            Assert.True(result.IsValid);
        }


        /// <summary>
        /// Check invalid when no IP has a matching affiliation
        /// </summary>
        [Fact]
        public async Task PV_33_AffiliationMismatch_Invalid()
        {
            var submission = CreateSubmissionWithInterestedParty(ipBaseNumber: "IPB123", affiliations: new List<string> { "AGENCY_X" });

            var iswcModel = new IswcModel
            {
                VerifiedSubmissions = new List<VerifiedSubmissionModel>
                {
                    new VerifiedSubmissionModel
                    {
                        InterestedParties = new List<InterestedPartyModel>
                        {
                            new InterestedPartyModel { IpBaseNumber = "IPB123" }
                        }
                    }
                }
            };

            submission.SearchedIswcModels = new List<IswcModel> { iswcModel };

            var rule = GetRule(
                iswcModels: new List<IswcModel> { iswcModel },
                ips: new List<InterestedPartyModel> { new InterestedPartyModel { IpBaseNumber = "IPB123" } },
                agreements: new List<Agreement>
                {
                    new Agreement { IpbaseNumber = "IPB123", AgencyId = "OTHER_AGENCY" }
                }
            );

            var result = await rule.IsValid(submission);
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// Check valid when IP has a matching affiliation
        /// </summary>
        [Fact]
        public async Task PV_33_AffiliationMatch_Valid()
        {
            var submission = CreateSubmissionWithInterestedParty(
                ipBaseNumber: "IPB123",
                affiliations: new List<string> { "AGENCY_X" }
            );

            var iswcModel = new IswcModel
            {
                VerifiedSubmissions = new List<VerifiedSubmissionModel>
                {
                    new VerifiedSubmissionModel
                    {
                        InterestedParties = new List<InterestedPartyModel>
                        {
                            new InterestedPartyModel { IpBaseNumber = "IPB123" }
                        }
                    }
                }
            };

            submission.SearchedIswcModels = new List<IswcModel> { iswcModel };

            var rule = GetRule(
                iswcModels: new List<IswcModel> { iswcModel },
                ips: new List<InterestedPartyModel>
                {
                    new InterestedPartyModel { IpBaseNumber = "IPB123" }
                },
                agreements: new List<Agreement>
                {
                    new Agreement { IpbaseNumber = "IPB123", AgencyId = "AGENCY_X" }
                }
            );

            var result = await rule.IsValid(submission);
            Assert.True(result.IsValid);
        }

        /// <summary>
        /// Check valid match based on matching IPNameNumber
        /// </summary>
        [Fact]
        public async Task PV_33_MatchByIPNameNumber_Valid()
        {
            var submission = CreateSubmissionWithInterestedParty(ipNameNumber: 123456);

            var iswcModel = new IswcModel
            {
                VerifiedSubmissions = new List<VerifiedSubmissionModel>
                {
                    new VerifiedSubmissionModel
                    {
                        InterestedParties = new List<InterestedPartyModel>
                        {
                            new InterestedPartyModel { IPNameNumber = 123456, IpBaseNumber = "IPB123" }
                        }
                    }
                }
            };

            submission.SearchedIswcModels = new List<IswcModel> { iswcModel };

            var rule = GetRule(
                iswcModels: new List<IswcModel> { iswcModel },
                ips: new List<InterestedPartyModel>
                {
                    new InterestedPartyModel { IPNameNumber = 123456, IpBaseNumber = "IPB123" }
                }
            );

            var result = await rule.IsValid(submission);
            Assert.True(result.IsValid);
        }


        /// <summary>
        /// Check valid match based on LastName when IPNameNumber and IpBaseNumber are null
        /// </summary>
        [Fact]
        public async Task PV_33_MatchByLastName_Valid()
        {
            var submission = CreateSubmissionWithInterestedParty(ipNameNumber: null, ipBaseNumber: null);

            var iswcModel = new IswcModel
            {
                VerifiedSubmissions = new List<VerifiedSubmissionModel>
                {
                    new VerifiedSubmissionModel
                    {
                        InterestedParties = new List<InterestedPartyModel>
                        {
                            new InterestedPartyModel { LastName = "Smith", IpBaseNumber = "IPB123" }
                        }
                    }
                }
            };

            submission.SearchedIswcModels = new List<IswcModel> { iswcModel };

            var rule = GetRule(
                iswcModels: new List<IswcModel> { iswcModel },
                ips: new List<InterestedPartyModel>
                {
                    new InterestedPartyModel { LastName = "Smith", IpBaseNumber = "IPB123" }
                }
            );

            var result = await rule.IsValid(submission);
            Assert.True(result.IsValid);
        }

        /// <summary>
        /// Check invalid when SearchedIswcModels is empty
        /// </summary>
        [Fact]
        public async Task PV_33_EmptySearchedIswcModels_Invalid()
        {
            var submission = GetValidSubmission();
            submission.SearchedIswcModels = new List<IswcModel>();

            var rule = GetRule();

            var result = await rule.IsValid(submission);
            Assert.False(result.IsValid);
        }

        /// <summary>
        /// Check that IsrcMatchedResult populates SearchedIswcModels and rule passes
        /// </summary>
        [Fact]
        public async Task PV_33_IsrcMatchedResult_PopulatesSearchedIswcModels_Valid()
        {
            var isrcWork = new IswcModel
            {
                IswcId = 123456789,
                VerifiedSubmissions = new List<VerifiedSubmissionModel>()
                {
                    new VerifiedSubmissionModel
                    {
                        InterestedParties = new List<InterestedPartyModel>
                        {
                            new InterestedPartyModel { IpBaseNumber = "IPB123" }
                        }
                    }
                }
            };

            var submission = GetValidSubmission();
            submission.IsrcMatchedResult = new MatchResult
            {
                Matches = new List<MatchingWork>
                {
                    new MatchingWork
                    {
                        Numbers = new List<WorkNumber> { new WorkNumber { Type = "ISWC", Number = "T0000000001" } }
                    }
                }
            };

            var rule = GetRule(
                iswcModels: new List<IswcModel> { isrcWork },
                ips: new List<InterestedPartyModel> { new InterestedPartyModel { IpBaseNumber = "IPB123" } },
                agreements: new List<Agreement>()
            );

            var result = await rule.IsValid(submission);
            Assert.True(result.IsValid);
            Assert.Contains(result.Submission.SearchedIswcModels, x => x.IswcId == 123456789);
        }

        /// <summary>
        /// Check transaction passes if IsrcMatchedResult is null
        /// </summary>
        [Fact]
        public async Task PV_33_IsrcMatchedResult_Null_Valid()
        {
            var iswcModel = new IswcModel
            {
                IswcId = 123456789,
                VerifiedSubmissions = new List<VerifiedSubmissionModel>
                {
                    new VerifiedSubmissionModel
                    {
                        InterestedParties = new List<InterestedPartyModel>
                        {
                            new InterestedPartyModel { IpBaseNumber = "IPB123" }
                        }
                    }
                }
            };

            var submission = new Submission
            {
                SearchWorks = new List<WorkModel> { new WorkModel() },
                IsrcMatchedResult = null,
                SearchedIswcModels = new List<IswcModel> { iswcModel },
                Model = new SubmissionModel
                {
                    InterestedParties = new List<InterestedPartyModel>
                    {
                        new InterestedPartyModel { IpBaseNumber = "IPB123" },
                    },
                    Affiliations = new List<string> { "AGENCY123" }
                }
            };

            var rule = GetRule(
                iswcModels: new List<IswcModel>(),
                ips: new List<InterestedPartyModel> { new InterestedPartyModel { IpBaseNumber = "IPB123" } },
                agreements: new List<Agreement>
                {
                    new Agreement { IpbaseNumber = "IPB123", AgencyId = "AGENCY123" }
                }
            );

            var response = await rule.IsValid(submission);
            Assert.True(response.IsValid);
        }


        /// <summary>
        /// Check transaction passes if IsrcMatchedResult is empty
        /// </summary>
        [Fact]
        public async Task PV_33_IsrcMatchedResult_EmptyList_Valid()
        {
            var iswcModel = new IswcModel
            {
                IswcId = 123456789,
                VerifiedSubmissions = new List<VerifiedSubmissionModel>
                {
                    new VerifiedSubmissionModel
                    {
                        InterestedParties = new List<InterestedPartyModel>
                        {
                            new InterestedPartyModel { IpBaseNumber = "IPB123" }
                        }
                    }
                }
            };

            var submission = new Submission
            {
                SearchWorks = new List<WorkModel> { new WorkModel() },
                IsrcMatchedResult = new MatchResult
                {
                    Matches = new List<MatchingWork>() 
                },
                SearchedIswcModels = new List<IswcModel> { iswcModel },
                Model = new SubmissionModel
                {
                    InterestedParties = new List<InterestedPartyModel>
                    {
                        new InterestedPartyModel { IpBaseNumber = "IPB123" },
                    },
                    Affiliations = new List<string> { "AGENCY123" }
                }
            };

            var rule = GetRule(
                iswcModels: new List<IswcModel> { iswcModel },
                ips: new List<InterestedPartyModel> { new InterestedPartyModel { IpBaseNumber = "IPB123" } },
                agreements: new List<Agreement>
                {
                    new Agreement { IpbaseNumber = "IPB123", AgencyId = "AGENCY123" }
                }
            );

            var response = await rule.IsValid(submission);

            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check valid match when first name matches partially
        /// </summary>
        [Fact]
        public async Task PV_33_FirstName_PartialMatch_Valid()
        {
            var submission = new Submission
            {
                Model = new SubmissionModel
                {
                    InterestedParties = new List<InterestedPartyModel>
                    {
                        new InterestedPartyModel
                        {
                            IpBaseNumber = "IPB123",
                            LastName = "Smith",
                            Names = new List<NameModel>
                            {
                                new NameModel { FirstName = "John" }
                            }
                        }
                    }
                },
                SearchWorks = new List<WorkModel> { new WorkModel() }
            };

            var iswcModel = new IswcModel
            {
                VerifiedSubmissions = new List<VerifiedSubmissionModel>
                {
                    new VerifiedSubmissionModel
                    {
                        InterestedParties = new List<InterestedPartyModel>
                        {
                            new InterestedPartyModel { IpBaseNumber = "IPB123" }
                        }
                    }
                }
            };

            var matchingIP = new InterestedPartyModel
            {
                IpBaseNumber = "IPB123",
                Names = new List<NameModel>
                {
                    new NameModel { FirstName = "Johnny" }
                }
            };

            submission.SearchedIswcModels = new List<IswcModel> { iswcModel };

            var rule = new PV_33(
                GetMessagingManagerMock(ErrorCode._180),
                GetWorkManagerMock(new List<IswcModel> { iswcModel }).Object,
                GetInterestedPartyManagerMock(new List<InterestedPartyModel> { matchingIP }).Object,
                GetAgreementManagerMock(new List<Agreement>()).Object
            );

            var result = await rule.IsValid(submission);
            Assert.True(result.IsValid);
        }

        /// <summary>
        /// Check invalid match when first name does not match
        /// </summary>
        [Fact]
        public async Task PV_33_FirstName_Mismatch_Invalid()
        {
            var submission = new Submission
            {
                Model = new SubmissionModel
                {
                    InterestedParties = new List<InterestedPartyModel>
                    {
                        new InterestedPartyModel
                        {
                            IpBaseNumber = "IPB123",
                            LastName = "Smith",
                            Names = new List<NameModel>
                            {
                                new NameModel { FirstName = "Michael" }
                            }
                        }
                    }
                },
                SearchWorks = new List<WorkModel> { new WorkModel() }
            };

            var iswcModel = new IswcModel
            {
                VerifiedSubmissions = new List<VerifiedSubmissionModel>
                {
                    new VerifiedSubmissionModel
                    {
                        InterestedParties = new List<InterestedPartyModel>
                        {
                            new InterestedPartyModel { IpBaseNumber = "IPB123" }
                        }
                    }
                }
            };

            var nonMatchingIP = new InterestedPartyModel
            {
                IpBaseNumber = "IPB123",
                Names = new List<NameModel>
                {
                    new NameModel { FirstName = "Sarah" }
                }
            };

            submission.SearchedIswcModels = new List<IswcModel> { iswcModel };

            var rule = new PV_33(
                GetMessagingManagerMock(ErrorCode._180),
                GetWorkManagerMock(new List<IswcModel> { iswcModel }).Object,
                GetInterestedPartyManagerMock(new List<InterestedPartyModel> { nonMatchingIP }).Object,
                GetAgreementManagerMock(new List<Agreement>()).Object
            );

            var result = await rule.IsValid(submission);
            Assert.False(result.IsValid);
        }
    }
}
