using AutoMapper;
using SpanishPoint.Azure.Iswc.Api.Publisher.Configuration;
using SpanishPoint.Azure.Iswc.Api.Publisher.V1;
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
    public class PublisherMappingProfileTests
    {
        [Fact]
        public void Map_V1_InterestedParty_to_InterestedPartyModel()
        {
            // Arrange
            var myProfile = new PublisherMappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);

            var interestedParties = new List<Publisher.V1.InterestedParty>
            {
                new Publisher.V1.InterestedParty
                {
                    NameNumber = 123456781
                },
                new Publisher.V1.InterestedParty
                {
                    NameNumber = 123456782,
                    Role = Publisher.V1.InterestedPartyRole.E
                },
                new Publisher.V1.InterestedParty
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

        [Fact]
        public void Map_WorkNumber_To_V1_WorkNumber()
        {
            // Arrange
            var config = new MapperConfiguration(cfg => cfg.AddProfile<PublisherMappingProfile>());
            var mapper = config.CreateMapper();

            var source = new Bdo.Work.WorkNumber
            {
                Type = "ASCAP",
                Number = "W123456789"
            };

            // Act
            var result = mapper.Map<Publisher.V1.WorkNumber>(source);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(source.Type, result.AgencyCode);
            Assert.Equal(source.Number, result.AgencyWorkCode);
        }

        [Fact]
        public void Map_IPContextSearchModel_To_SubmissionModel()
        {
            // Arrange
            var config = new MapperConfiguration(cfg => cfg.AddProfile<PublisherMappingProfile>());
            var mapper = config.CreateMapper();

            var source = new IPContextSearchModel
            {
                FirstName = "ANDREW JOHN",
                LastName = "HOZIER BYRNE",
                NameNumber = 589238793,
                Age = 34,
                AgeTolerance = 1,
                DateOfBirth = new DateTimeOffset(new DateTime(1990, 3, 17)),
                DateOfDeath = null,
                Affiliations = new List<Affiliations>
                {
                    new Affiliations { Affiliation = "PRS" },
                    new Affiliations { Affiliation = "ASCAP" },
                    new Affiliations { Affiliation = " " } 
                },
                AdditionalIdentifiers = new AdditionalIdentifiers2
                {
                    Isrcs = new List<string> { "ISRC12345", "ISRC67890" }
                }
            };

            // Act
            var result = mapper.Map<SubmissionModel>(source);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.InterestedParties);
            var ip = result.InterestedParties.First();
            Assert.Equal(589238793, ip.IPNameNumber);
            Assert.Equal("HOZIER BYRNE ANDREW JOHN", ip.Name);
            Assert.Equal("HOZIER BYRNE", ip.LastName);
            Assert.Equal(34, ip.Age);
            Assert.Equal(1, ip.AgeTolerance);
            Assert.Equal(new DateTime(1990, 3, 17), ip.BirthDate);

            Assert.NotNull(ip.Names);
            Assert.Single(ip.Names);
            Assert.Equal("ANDREW JOHN", ip.Names.First().FirstName);
            Assert.Equal("HOZIER BYRNE", ip.Names.First().LastName);

            Assert.Equal(2, result.Affiliations.Count);
            Assert.Contains("PRS", result.Affiliations);
            Assert.Contains("ASCAP", result.Affiliations);

            Assert.NotNull(result.AdditionalIdentifiers);
            Assert.Collection(result.AdditionalIdentifiers,
                id => Assert.Equal("ISRC12345", id.WorkCode),
                id => Assert.Equal("ISRC67890", id.WorkCode));
        }

        [Fact]
        public void Map_V1_WorkNumber_To_AdditionalAgencyWorkNumber()
        {
            // Arrange
            var config = new MapperConfiguration(cfg => cfg.AddProfile<PublisherMappingProfile>());
            var mapper = config.CreateMapper();

            var v1WorkNumber = new Publisher.V1.WorkNumber
            {
                AgencyCode = "128",
                AgencyWorkCode = "R123456789"
            };

            // Act
            var result = mapper.Map<AdditionalAgencyWorkNumber>(v1WorkNumber);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.WorkNumber);
            Assert.Equal("128", result.WorkNumber.Type);
            Assert.Equal("R123456789", result.WorkNumber.Number);
        }

        [Fact]
        public void Map_V1_Works_To_WorkModel()
        {
            // Arrange
            var config = new MapperConfiguration(cfg => cfg.AddProfile<PublisherMappingProfile>());
            var mapper = config.CreateMapper();

            var source = new Publisher.V1.Works
            {
                Iswc = "T1234567890",
                Titles = new List<Publisher.V1.Title>
                {
                    new Publisher.V1.Title { Title1 = "Test Title", Type = Publisher.V1.TitleType.OT }
                },
                WorkCodes = new List<Publisher.V1.WorkNumber>
                {
                    new Publisher.V1.WorkNumber { AgencyCode = "ASCAP", AgencyWorkCode = "W001" }
                }
            };

            // Act
            var result = mapper.Map<WorkModel>(source);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(source.Iswc, result.PreferredIswc);
            Assert.NotNull(result.Titles);
            Assert.Single(result.Titles);
            Assert.Equal("Test Title", result.Titles.First().Name);
            Assert.NotNull(result.WorkNumbers);
            Assert.Single(result.WorkNumbers);
            Assert.Equal("ASCAP", result.WorkNumbers.First().WorkNumber.Type);
            Assert.Equal("W001", result.WorkNumbers.First().WorkNumber.Number);
        }

        [Fact]
        public void Map_Submission_To_IPContextSearchResponseBatch_With_MatchingIswcs_From_SearchedIswcModels()
        {
            // Arrange
            var config = new MapperConfiguration(cfg => cfg.AddProfile<PublisherMappingProfile>());
            var mapper = config.CreateMapper();

            var submission = new Submission
            {
                SubmissionId = 1010,
                SearchedIswcModels = new List<IswcModel>
                {
                    new IswcModel
                    {
                        Iswc = "T0301600225",
                        Agency = "128",
                        VerifiedSubmissions = new List<VerifiedSubmissionModel>
                        {
                            new VerifiedSubmissionModel
                            {
                                WorkNumber = new Bdo.Work.WorkNumber { Type = "128", Number = "R16760727" },
                                Titles = new List<Bdo.Work.Title>
                                {
                                    new Bdo.Work.Title { Name = "WORK SONG", Type = Bdo.Work.TitleType.OT }
                                },
                                IswcStatus = "Preferred",
                                IswcEligible = true,
                                InterestedParties = new List<InterestedPartyModel>()
                            }
                        }
                    },
                    new IswcModel
                    {
                        Iswc = "T0314269049",
                        Agency = "128",
                        VerifiedSubmissions = new List<VerifiedSubmissionModel>
                        {
                            new VerifiedSubmissionModel
                            {
                                WorkNumber = new Bdo.Work.WorkNumber { Type = "128", Number = "R15646076" },
                                Titles = new List<Bdo.Work.Title>
                                {
                                    new Bdo.Work.Title { Name = "FROM EDEN", Type = Bdo.Work.TitleType.OT }
                                },
                                IswcStatus = "Preferred",
                                IswcEligible = true,
                                InterestedParties = new List<InterestedPartyModel>()
                            }
                        }
                    }
                },
                Model = new SubmissionModel
                {
                    InterestedParties = new List<InterestedPartyModel>
                    {
                        new InterestedPartyModel
                        {
                            IPNameNumber = 589238793,
                            Names = new List<NameModel>
                            {
                                new NameModel
                                {
                                    IpNameNumber = 589238793,
                                    FirstName = "ANDREW JOHN",
                                    LastName = "HOZIER BYRNE"
                                }
                            }
                        }
                    }
                },
                SocietyWorkCodes = true
            };

            // Act
            var result = mapper.Map<IPContextSearchResponseBatch>(submission);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(submission.SubmissionId, result.SearchId);
            Assert.NotNull(result.SearchResult);
            Assert.Equal("ANDREW JOHN", result.SearchResult.FirstName);
            Assert.Equal("HOZIER BYRNE", result.SearchResult.LastName);
            Assert.Equal(589238793, result.SearchResult.NameNumber);

            var matchingIswcs = result.SearchResult.MatchingIswcs;
            Assert.NotNull(matchingIswcs);
            Assert.Equal(2, matchingIswcs.Count);

            var workSong = matchingIswcs.FirstOrDefault(x => x.Iswc == "T0301600225");
            var fromEden = matchingIswcs.FirstOrDefault(x => x.Iswc == "T0314269049");

            Assert.NotNull(workSong);
            Assert.Equal("WORK SONG", workSong.OriginalTitle);
            Assert.Equal("128", workSong.Agency);
            Assert.Contains(workSong.WorkNumbers, x => x.AgencyWorkCode == "R16760727");

            Assert.NotNull(fromEden);
            Assert.Equal("FROM EDEN", fromEden.OriginalTitle);
            Assert.Equal("128", fromEden.Agency);
            Assert.Contains(fromEden.WorkNumbers, x => x.AgencyWorkCode == "R15646076");
        }
    }
}
