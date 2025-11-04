using AutoMapper;
using SpanishPoint.Azure.Iswc.Api.Label.Configuration;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.MatchingEngine;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Tests.Configuration
{
    /// <summary>
    /// Unit tests for SpanishPoint.Azure.Iswc.Api.Label.Configuration.LabelMappingProfile
    /// </summary>
    public class LabelMappingProfileTests
    {
        /// <summary>
        /// Tests that Label.V1.Submission maps correctly to a SubmissionModel
        /// </summary>
        [Fact]
        public void Map_Label_V1_Submission_To_SubmissionModel()
        {
            // Arrange
            var myProfile = new LabelMappingProfile();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetIndexParameters().Length == 0 && p.GetMethod.IsPublic;
                cfg.AddProfile(myProfile);
            });
            var mapper = new Mapper(configuration);

            var submission = new Label.V1.Submission()
            {
                Agency = "052",
                OriginalTitle = "original title",
                Performers = new List<Label.V1.Performer>() 
                {
                    new Label.V1.Performer()
                    {
                        Isni = "0000000123456789",
                        Ipn = 12345678,
                        FirstName = "name",
                        LastName = "surname",
                        Designation = Label.V1.PerformerDesignation.Main_Artist
                    },
                    new Label.V1.Performer()
                    {
                        Isni = "0000000123456789",
                        Ipn = 12345678,
                        FirstName = "name",
                        LastName = "surname",
                        Designation = Label.V1.PerformerDesignation.Other
                    },
                    new Label.V1.Performer()
                    {
                        Isni = "0000000987654321",
                        Ipn = 18765432,
                        FirstName = "firstname",
                        LastName = "lastname",
                        Designation = Label.V1.PerformerDesignation.Other
                    }
                },
                InterestedParties = new List<Label.V1.InterestedParty>()
                {
                    new Label.V1.InterestedParty()
                    {
                        Role = Label.V1.InterestedPartyRole.CA,
                        Name = "name",
                        LastName = "surname",
                        DisplayName = "display name"
                    }
                },
                AdditionalIdentifiers = new Label.V1.AdditionalIdentifiers
                {
                    LabelIdentifiers = new List<Label.V1.LabelIdentifiers>()
                    {
                        new Label.V1.LabelIdentifiers() {
                            SubmitterDPID = "dpid",
                            WorkCode = new List<string> { "123456789" }
                        }
                    },
                    Recordings = new List<Label.V1.Recordings>() { 
                        new Label.V1.Recordings()
                        {
                            Isrc = "ABC123",
                            RecordingTitle = "title",
                            SubTitle = "subtitle",
                            LabelName = "name",
                            ReleaseEmbargoDate = DateTime.Now,
                        },
                        new Label.V1.Recordings()
                        {
                            Isrc = "DEF456",
                            RecordingTitle = "title",
                            SubTitle = "subtitle",
                            LabelName = "name",
                            ReleaseEmbargoDate = DateTime.Now,
                        }
                    }
                }
            };

            // Act
            SubmissionModel submissionModel = mapper.Map<SubmissionModel>(submission);

            var x = submission.Performers.ElementAt(0).Designation.Value.ToString();

            // Assert
            Assert.Equal(312, submissionModel.SourceDb);
            Assert.Equal(submission.Agency, submissionModel.Agency);
            Assert.Equal(3, submissionModel.Performers.Count);
            Assert.Equal(submission.Performers.ElementAt(0).Isni, submissionModel.Performers.ElementAt(0).Isni);
            Assert.Equal(submission.Performers.ElementAt(0).Ipn, submissionModel.Performers.ElementAt(0).Ipn);
            Assert.Equal(Label.V1.PerformerDesignation.Main_Artist, submission.Performers.ElementAt(0).Designation);
            Assert.Equal("Other", submissionModel.Performers.ElementAt(1).Designation);
            Assert.Equal(Category.DOM, submissionModel.Category);
            Assert.Single(submissionModel.Titles);
            Assert.Equal(TitleType.OT, submissionModel.Titles.ElementAt(0).Type);
            Assert.Equal(submission.OriginalTitle, submissionModel.Titles.ElementAt(0).Name);
            Assert.Equal(InterestedPartyType.CA, submissionModel.InterestedParties.ElementAt(0).Type);
            Assert.Single(submissionModel.InterestedParties);
            Assert.Equal(submission.InterestedParties.ElementAt(0).Name, submissionModel.InterestedParties.ElementAt(0).Name);
            Assert.Equal(submission.InterestedParties.ElementAt(0).LastName, submissionModel.InterestedParties.ElementAt(0).LastName);
            Assert.Equal(submission.InterestedParties.ElementAt(0).DisplayName, submissionModel.InterestedParties.ElementAt(0).DisplayName);
            Assert.Equal(3, submissionModel.AdditionalIdentifiers.Count());
            Assert.Equal(submission.AdditionalIdentifiers.Recordings.ElementAt(0).Isrc, submissionModel.AdditionalIdentifiers.ElementAt(1).WorkCode);
            Assert.Equal(submission.AdditionalIdentifiers.Recordings.ElementAt(0).RecordingTitle, submissionModel.AdditionalIdentifiers.ElementAt(1).RecordingTitle);
            Assert.Equal(submission.AdditionalIdentifiers.Recordings.ElementAt(0).ReleaseEmbargoDate, submissionModel.AdditionalIdentifiers.ElementAt(1).ReleaseEmbargoDate);
        }

        /// <summary>
        /// Tests that VerifiedSubmission maps correctly to a Label.V1.VerifiedSubmission
        /// </summary>
        [Fact]
        public void Map_VerifiedSubmissionModel_To_V1_VerifiedSubmission()
        {
            // Arrange
            var myProfile = new LabelMappingProfile();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetIndexParameters().Length == 0 && p.GetMethod.IsPublic;
                cfg.AddProfile(myProfile);
            });
            var mapper = new Mapper(configuration);

            var submissionModel = new VerifiedSubmissionModel()
            {
                Titles = new List<Title>() {
                    new Title {Type = TitleType.OT, Name = "original title"},
                },
                Performers = new List<Performer>()
                {
                    new Performer()
                    {
                        Isni = "0000000123456789",
                        Ipn = 12345678,
                        FirstName = "name",
                        LastName = "surname",
                        Designation = "Main Artist"
                    },
                    new Performer()
                    {
                        Isni = "0000000123456789",
                        Ipn = 12345678,
                        FirstName = "name",
                        LastName = "surname",
                        Designation = "Other"
                    },
                    new Performer()
                    {
                        Isni = "0000000987654321",
                        Ipn = 18765432,
                        FirstName = "firstname",
                        LastName = "lastname",
                        Designation = "Other"
                    }
                },
                InterestedParties = new List<InterestedPartyModel>()
                {
                    new InterestedPartyModel()
                    {
                       Type = InterestedPartyType.CA,
                        Name = "name",
                        LastName = "surname",
                        DisplayName = "display name"
                    }
                },
                AdditionalIdentifiers = new List<Bdo.Work.AdditionalIdentifier>
                    {
                        new Bdo.Work.AdditionalIdentifier
                        {
                            WorkCode = "ABC123",
                            RecordingTitle = "title",
                            SubTitle = "subtitle",
                            LabelName = "name",
                            ReleaseEmbargoDate = DateTime.Now,
                            Performers = new List<Performer>()
                            {
                                new Performer()
                                {
                                    Isni = "0000000123456789",
                                    Ipn = 12345678,
                                    FirstName = "name",
                                    LastName = "surname",
                                    Designation = "Main Artist"
                                },
                                new Performer()
                                {
                                    Isni = "0000000123456789",
                                    Ipn = 12345678,
                                    FirstName = "name",
                                    LastName = "surname",
                                    Designation = "Other"
                                }
                            }
                        },
                        new Bdo.Work.AdditionalIdentifier
                        {
                            WorkCode = "DEF456",
                            RecordingTitle = "title",
                            SubTitle = "subtitle",
                            LabelName = "name",
                            ReleaseEmbargoDate = DateTime.Now,
                        }
                    }
            };

            // Act
            Label.V1.VerifiedSubmission submission = mapper.Map<Label.V1.VerifiedSubmission>(submissionModel);

            // Assert
            Assert.Equal(Bdo.Iswc.Category.DOM, submissionModel.Category);
            Assert.Single(submissionModel.Titles);
            Assert.Equal(submission.OriginalTitle, submissionModel.Titles.ElementAt(0).Name);
            Assert.Equal(TitleType.OT, submissionModel.Titles.ElementAt(0).Type);
            Assert.Equal(2, submission.Performers.Count);
            Assert.Equal(submissionModel.Performers.ElementAt(0).Isni, submission.Performers.ElementAt(0).Isni);
            Assert.Equal(submissionModel.Performers.ElementAt(2).Isni, submission.Performers.ElementAt(1).Isni);
            Assert.Equal(submissionModel.Performers.ElementAt(0).Ipn, submission.Performers.ElementAt(0).Ipn);
            Assert.Equal(submissionModel.Performers.ElementAt(2).Ipn, submission.Performers.ElementAt(1).Ipn);
            Assert.Equal(Label.V1.PerformerDesignation.Main_Artist, submission.Performers.ElementAt(0).Designation);
            Assert.Equal(Label.V1.PerformerDesignation.Other, submission.Performers.ElementAt(1).Designation);
            Assert.Single(submission.InterestedParties);
            Assert.Equal(Label.V1.InterestedPartyRole.CA, submission.InterestedParties.ElementAt(0).Role);
            Assert.Equal(submission.InterestedParties.ElementAt(0).Name, submissionModel.InterestedParties.ElementAt(0).Name);
            Assert.Equal(submission.InterestedParties.ElementAt(0).LastName, submissionModel.InterestedParties.ElementAt(0).LastName);
            Assert.Equal(submission.InterestedParties.ElementAt(0).DisplayName, submissionModel.InterestedParties.ElementAt(0).DisplayName);
            Assert.Equal(2, submission.AdditionalIdentifiers.Recordings.Count);
            Assert.Equal(submissionModel.AdditionalIdentifiers.ElementAt(0).WorkCode, submission.AdditionalIdentifiers.Recordings.ElementAt(0).Isrc);
            Assert.Equal(submissionModel.AdditionalIdentifiers.ElementAt(0).RecordingTitle, submission.AdditionalIdentifiers.Recordings.ElementAt(0).RecordingTitle);
            Assert.Equal(submissionModel.AdditionalIdentifiers.ElementAt(0).ReleaseEmbargoDate, submission.AdditionalIdentifiers.Recordings.ElementAt(0).ReleaseEmbargoDate);
            Assert.Equal(2, submission.AdditionalIdentifiers.Recordings.ElementAt(0).Performers.Count);
            Assert.Equal(submissionModel.AdditionalIdentifiers.ElementAt(0).Performers.ElementAt(0).Isni, submission.AdditionalIdentifiers.Recordings.ElementAt(0).Performers.ElementAt(0).Isni);
            Assert.Equal(submissionModel.AdditionalIdentifiers.ElementAt(0).Performers.ElementAt(1).Isni, submission.AdditionalIdentifiers.Recordings.ElementAt(0).Performers.ElementAt(1).Isni);
        }

        /// <summary>
        /// Tests that alternate ISWC matches are mapped when HasAlternateIswcMatches is true
        /// </summary>
        [Fact]
        public void Map_Alternate_Iswc_Matches_To_Response()
        {
            // Arrange
            var myProfile = new LabelMappingProfile();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetIndexParameters().Length == 0 && p.GetMethod.IsPublic;
                cfg.AddProfile(myProfile);
            });
            var mapper = new Mapper(configuration);

            var work1 = new MatchingWork()
            {
                Id = 1,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.OT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C } },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "1", Type = "NOT_ISWC" } }
            };
            var work2 = new MatchingWork()
            {
                Id = 4,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() { },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "T2", Type = "ISWC" } }
            };
            var work3 = new MatchingWork()
            {
                Id = 4,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C } },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "T3", Type = "ISWC" } }
            };

            var work4 = new MatchingWork()
            {
                Id = 4,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C } },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "T4", Type = "ISWC" } }
            };

            var submission = new Submission()
            {
                HasAlternateIswcMatches = true,
                MatchedResult = new MatchResult
                {
                    Matches = new List<MatchingWork> { work1, work2 }
                },
                IsrcMatchedResult = new MatchResult
                {
                    Matches = new List<MatchingWork> { work3, work4 }
                }
            };

            // Act 
            var verifiedSubmission = mapper.Map<Label.V1.VerifiedSubmissionBatch>(submission);

            // Assert
            Assert.Equal(2, verifiedSubmission.AlternateIswcMatches.Count());
            Assert.Equal(work2.Numbers.First(x => x.Type == "ISWC").Number, verifiedSubmission.AlternateIswcMatches.ElementAt(0).Iswc);
            Assert.Equal(work4.Numbers.First(x => x.Type == "ISWC").Number, verifiedSubmission.AlternateIswcMatches.ElementAt(1).Iswc);
        }

        /// <summary>
        /// Tests that alternate ISWC matches are not mapped when HasAlternateIswcMatches is false
        /// </summary>
        [Fact]
        public void Do_Not_Map_Alternate_Iswc_Matches_To_Response()
        {
            // Arrange
            var myProfile = new LabelMappingProfile();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetIndexParameters().Length == 0 && p.GetMethod.IsPublic;
                cfg.AddProfile(myProfile);
            });
            var mapper = new Mapper(configuration);

            var work1 = new MatchingWork()
            {
                Id = 1,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.OT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C } },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "1", Type = "NOT_ISWC" } }
            };
            var work2 = new MatchingWork()
            {
                Id = 4,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() { },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "T2", Type = "ISWC" } }
            };
            var work3 = new MatchingWork()
            {
                Id = 4,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C } },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "T3", Type = "ISWC" } }
            };

            var work4 = new MatchingWork()
            {
                Id = 4,
                Titles = new List<Bdo.Work.Title>() { new Bdo.Work.Title() { Name = "Test Title", Type = Bdo.Work.TitleType.AT } },
                Contributors = new List<InterestedPartyModel>() { new InterestedPartyModel() { IpBaseNumber = "123", Type = InterestedPartyType.C } },
                Numbers = new List<WorkNumber> { new WorkNumber { Number = "T4", Type = "ISWC" } }
            };

            var submission = new Submission()
            {
                HasAlternateIswcMatches = false,
                MatchedResult = new MatchResult
                {
                    Matches = new List<MatchingWork> { work1, work2 }
                },
                IsrcMatchedResult = new MatchResult
                {
                    Matches = new List<MatchingWork> { work3, work4 }
                }
            };

            // Act 
            var verifiedSubmission = mapper.Map<Label.V1.VerifiedSubmissionBatch>(submission);

            // Assert
            Assert.Empty(verifiedSubmission.AlternateIswcMatches);
        }

        /// <summary>
        /// Tests that V1.InterestedParty maps correctly to an InterestedPartyModel
        /// </summary>
        [Fact]
        public void Map_V1_InterestedParty_to_InterestedPartyModel()
        {
            // Arrange
            var myProfile = new LabelMappingProfile();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetIndexParameters().Length == 0 && p.GetMethod.IsPublic;
                cfg.AddProfile(myProfile);
            });
            var mapper = new Mapper(configuration);

            var interestedParties = new List<Label.V1.InterestedParty>
            {
                new Label.V1.InterestedParty
                {
                    NameNumber = 123456781
                },
                new Label.V1.InterestedParty
                {
                    NameNumber = 123456782,
                    Role = Label.V1.InterestedPartyRole.E
                },
                new Label.V1.InterestedParty
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
