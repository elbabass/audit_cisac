using AutoMapper;
using SpanishPoint.Azure.Iswc.Api.Agency.Configuration;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Tests.Configuration
{
    /// <summary>
    /// Unit tests for SpanishPoint.Azure.Iswc.Api.Configuration.MappingProfile
    /// </summary>
    public class MappingProfileTests
    {
        /// <summary>
        /// Tests that Agency.V1.Submission maps correctly to a SubmissionModel
        /// </summary>
        [Fact]
        public void Map_V1_Submission_To_SubmissionModel()
        {
            // Arrange
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetIndexParameters().Length == 0 && p.GetMethod.IsPublic;
                cfg.AddProfile(myProfile);
            });
            var mapper = new Mapper(configuration);

            var submission = new Agency.V1.Submission()
            {
                Agency = "123",
                Workcode = "10",
                Category = Agency.V1.CommonSubmissionAttributesCategory.INT,
                OriginalTitle = "original title",
                OtherTitles = new List<Agency.V1.Title>()
                {
                    new Agency.V1.Title()
                    {
                        Title1 = "title one",
                        Type = Agency.V1.TitleType.AL
                    }
                },
                InterestedParties = new List<Agency.V1.InterestedParty>()
                {
                    new Agency.V1.InterestedParty()
                    {
                        Role = Agency.V1.InterestedPartyRole.C,
                        BaseNumber = "base number",
                        NameNumber = 11
                    }
                },
                DerivedWorkType = Agency.V1.DerivedWorkType.Excerpt,
                AdditionalIdentifiers = new Agency.V1.AdditionalIdentifiers
                {
                    Isrcs = new List<string> { "ABC456" },
                    PublisherIdentifiers = new List<Agency.V1.PublisherIdentifiers>
                    {
                        new Agency.V1.PublisherIdentifiers{
                        WorkCode = new List<string> { "123456" },
                        NameNumber = 269021863
                        }
                    },
                    AgencyWorkCodes = new List<Agency.V1.AgencyWorkCodes>
                    {
                       new Agency.V1.AgencyWorkCodes{ Agency = "128", WorkCode = "123"},
                       new Agency.V1.AgencyWorkCodes{ Agency = "129", WorkCode = "1234"},
                       new Agency.V1.AgencyWorkCodes{ Agency = "130", WorkCode = "12345"}
                    }
                }
            };

            // Act
            SubmissionModel submissionModel = mapper.Map<SubmissionModel>(submission);

            // Assert
            Assert.Equal(submission.Agency, submissionModel.WorkNumber.Type);
            Assert.Equal(submission.Workcode, submissionModel.WorkNumber.Number);
            Assert.Equal(submission.Agency, submissionModel.Agency);
            Assert.Equal(Bdo.Iswc.Category.INT, submissionModel.Category);
            Assert.Equal(2, submissionModel.Titles.Count);
            Assert.Equal(submission.OtherTitles.ElementAt(0).Title1, submissionModel.Titles.ElementAt(0).Name);
            Assert.Equal(TitleType.AL, submissionModel.Titles.ElementAt(0).Type);
            Assert.Equal(submission.OriginalTitle, submissionModel.Titles.ElementAt(1).Name);
            Assert.Equal(TitleType.OT, submissionModel.Titles.ElementAt(1).Type);
            Assert.Equal(InterestedPartyType.C, submissionModel.InterestedParties.ElementAt(0).Type);
            Assert.Equal(submission.InterestedParties.ElementAt(0).BaseNumber, submissionModel.InterestedParties.ElementAt(0).IpBaseNumber);
            Assert.Equal(submission.InterestedParties.ElementAt(0).NameNumber, submissionModel.InterestedParties.ElementAt(0).IPNameNumber);
            Assert.Equal(DerivedWorkType.Excerpt, submissionModel.DerivedWorkType);
            Assert.Equal(2, submissionModel.AdditionalIdentifiers.Count());
            Assert.Equal(submission.AdditionalIdentifiers.AgencyWorkCodes.Count(), submissionModel.AdditionalAgencyWorkNumbers.Count());
        }

        /// <summary>
        /// Tests that VerifiedSubmission maps correctly to a Agency.V1.VerifiedSubmission
        /// </summary>
        [Fact]
        public void Map_VerifiedSubmissionModel_To_V1_VerifiedSubmission()
        {
            // Arrange
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetIndexParameters().Length == 0 && p.GetMethod.IsPublic;
                cfg.AddProfile(myProfile);
            });
            var mapper = new Mapper(configuration);

            var submissionModel = new VerifiedSubmissionModel()
            {
                Agency = "128",
                WorkNumber = new WorkNumber { Type = "128", Number = "test-01" },
                Titles = new List<Title>() {
                    new Title {Type = TitleType.OT, Name = "original title"},
                    new Title {Type = TitleType.AT, Name = "alt title"}
                },
                InterestedParties = new List<InterestedPartyModel>()
                {
                    new InterestedPartyModel()
                    {
                        CisacType = CisacInterestedPartyType.MA,
                        Type= InterestedPartyType.C,
                        IpBaseNumber = "base number",
                        IPNameNumber= 00123456789
                    }
                },
                AdditionalIdentifiers = new List<Bdo.Work.AdditionalIdentifier>
                    {
                        new Bdo.Work.AdditionalIdentifier
                        {
                            WorkCode = "ABC456",
                            SubmitterCode = "ISRC"
                        },
                        new Bdo.Work.AdditionalIdentifier
                        {
                            WorkCode = "DEF789",
                            NameNumber = 269021863,
                            SubmitterCode = "SA"
                        },
                        new Bdo.Work.AdditionalIdentifier
                        {
                            WorkCode = "12345",
                            NameNumber = 269021863,
                            SubmitterCode = "SA"
                        }
                    }
            };

            // Act
            Agency.V1.VerifiedSubmission submission = mapper.Map<Agency.V1.VerifiedSubmission>(submissionModel);

            // Assert
            Assert.Equal(submission.Agency, submissionModel.WorkNumber.Type);
            Assert.Equal(submission.Workcode, submissionModel.WorkNumber.Number);
            Assert.Equal(submission.Agency, submissionModel.Agency);
            Assert.Equal(Bdo.Iswc.Category.DOM, submissionModel.Category);
            Assert.Equal(2, submissionModel.Titles.Count);
            Assert.Equal(submission.OtherTitles.ElementAt(0).Title1, submissionModel.Titles.ElementAt(1).Name);
            Assert.Equal(TitleType.AT, submissionModel.Titles.ElementAt(1).Type);
            Assert.Equal(submission.OriginalTitle, submissionModel.Titles.ElementAt(0).Name);
            Assert.Equal(TitleType.OT, submissionModel.Titles.ElementAt(0).Type);
            Assert.Equal(CisacInterestedPartyType.MA, submissionModel.InterestedParties.ElementAt(0).CisacType);
            Assert.Equal(InterestedPartyType.C, submissionModel.InterestedParties.ElementAt(0).Type);
            Assert.Equal(submission.InterestedParties.ElementAt(0).BaseNumber, submissionModel.InterestedParties.ElementAt(0).IpBaseNumber);
            Assert.Equal(submission.InterestedParties.ElementAt(0).NameNumber, submissionModel.InterestedParties.ElementAt(0).IPNameNumber);
            Assert.Equal(submission.AdditionalIdentifiers.Isrcs.ElementAt(0), submissionModel.AdditionalIdentifiers.ElementAt(0).WorkCode);
            Assert.Equal(2, submission.AdditionalIdentifiers.PublisherIdentifiers.ElementAt(0).WorkCode.Count());
        }


        /// <summary>
        /// Tests that the mapping for InterestedParties in Agency.V1.ISWCMetadata is correct
        /// Only uses eligible submissions
        /// Takes Authoritative IPs when more than one of the same IpBaseNumber
        /// </summary>
        [Fact]
        public void Map_IswcModel_To_V1_ISWCMetadata()
        {
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetIndexParameters().Length == 0 && p.GetMethod.IsPublic;
                cfg.AddProfile(myProfile);
            });
            var mapper = new Mapper(configuration);

            var iswcModel = new IswcModel()
            {
                VerifiedSubmissions = new List<VerifiedSubmissionModel>()
                {
                    new VerifiedSubmissionModel()
                    {
                        CreatedDate = DateTime.Parse("2020-05-15 15:42:49"),
                        IswcEligible = false,
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel()
                            {
                                IsAuthoritative = false,
                                IPNameNumber = 12345,
                                IpBaseNumber = "I-000254969-9"
                            },
                            new InterestedPartyModel()
                            {
                                IsAuthoritative = false,
                                IPNameNumber = 6436323,
                                IpBaseNumber = "I-001626718-6"
                            }
                        }
                    },

                    new VerifiedSubmissionModel()
                    {
                        CreatedDate = DateTime.Parse("2020-05-17 15:42:49"),
                        IswcEligible = true,
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel()
                            {
                                IsAuthoritative = true,
                                IPNameNumber = 453453333,
                                IpBaseNumber = "I-000254888-6",
                                IsExcludedFromIswc = true
                            }
                        }
                    },
                    new VerifiedSubmissionModel()
                    {
                        CreatedDate = DateTime.Parse("2020-05-16 15:42:49"),
                        IswcEligible = true,
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel()
                            {
                                IsAuthoritative = true,
                                IPNameNumber = 4256547,
                                IpBaseNumber = "I-000254969-9"
                            }
                        }
                    },
                    new VerifiedSubmissionModel()
                    {
                        CreatedDate = DateTime.Parse("2019-05-15 15:42:49"),
                        IswcEligible = true,
                        InterestedParties = new List<InterestedPartyModel>()
                        {
                            new InterestedPartyModel()
                            {
                                IsAuthoritative = true,
                                IPNameNumber = 090886,
                                IpBaseNumber = "I-000254969-9"
                            }
                        }
                    },
                }

            };

            // Act
            Agency.V1.ISWCMetadata iswc = mapper.Map<Agency.V1.ISWCMetadata>(iswcModel);

            // Assert
            Assert.Single(iswc.InterestedParties);
            Assert.Collection(iswc.InterestedParties,
                elem1 =>
                {
                    Assert.Equal(4256547, elem1.NameNumber);
                });

        }

        /// <summary>
        /// Tests that V1.InterestedParty maps correctly to an InterestedPartyModel
        /// </summary>
        [Fact]
        public void Map_V1_InterestedParty_to_InterestedPartyModel()
        {
            // Arrange
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetIndexParameters().Length == 0 && p.GetMethod.IsPublic;
                cfg.AddProfile(myProfile);
            });
            var mapper = new Mapper(configuration);

            var interestedParties = new List<Agency.V1.InterestedParty>
            {
                new Agency.V1.InterestedParty
                {
                    NameNumber = 123456781
                },
                new Agency.V1.InterestedParty
                {
                    NameNumber = 123456782,
                    Role = Agency.V1.InterestedPartyRole.E
                },
                new Agency.V1.InterestedParty
                {
                    NameNumber = 123456783
                }
            };

            // Act
            var interestedPartyModels = mapper.Map<List<InterestedPartyModel>>(interestedParties);

            // Assert
            Assert.Equal(interestedParties.Count, interestedPartyModels.Count);
            Assert.Equal(interestedParties[0].NameNumber, interestedPartyModels[0].IPNameNumber);
            Assert.Equal(InterestedPartyType.C, interestedPartyModels[0].Type);
            Assert.Equal(interestedParties[1].NameNumber, interestedPartyModels[1].IPNameNumber);
            Assert.Equal(InterestedPartyType.E, interestedPartyModels[1].Type);
            Assert.Equal(interestedParties[2].NameNumber, interestedPartyModels[2].IPNameNumber);
            Assert.Equal(InterestedPartyType.C, interestedPartyModels[2].Type);
        }
    }
}
