using AutoMapper;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Services.Matching.MatchingEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Data.Tests.AutomapperTests
{
    /// <summary>
    /// Mapper Profile Tests
    /// </summary>
    public class MapperProfileTests
    {
        /// <summary>
        /// Automapper Test that a Submission maps to InputWorkInfo and All values map as expected for a full Model
        /// </summary>
        [Fact]
        public void Automapper_Map_SubmissionModel_InputWorkInfo_Should_Map_All_Values_MappingProfile()
        {
            // Arrange
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);

            var submission_1 = new Submission
            {
                Model = new SubmissionModel()
                {
                    SourceDb = 1,
                    Titles = new List<Bdo.Work.Title>()
                {
                    new Bdo.Work.Title
                    {
                        Name = "Title1",
                        StandardizedName = "TITLEONE",
                        Type = Bdo.Work.TitleType.TO
                    },
                    new Bdo.Work.Title
                    {
                        Name = "Title2",
                        StandardizedName = "TITLETWO",
                        Type = Bdo.Work.TitleType.OT
                    }
                },
                    InterestedParties = new List<InterestedPartyModel>()
                {
                    new InterestedPartyModel
                    {
                        IpBaseNumber = "IPBN-111111111",
                        Type = InterestedPartyType.C,
                        CisacType = CisacInterestedPartyType.C,
                        Names = new List<NameModel>()
                        {
                            new NameModel
                            {
                                IpNameNumber = 123,
                                FirstName = "IPFirstName",
                                LastName = "IPLastName",
                                CreatedDate = DateTime.UtcNow,
                                TypeCode = NameType.PA,
                                ForwardingNameNumber = 777,
                                Agency = "AgencyCode"
                            }
                        }
                    },
                    new InterestedPartyModel
                    {
                        IpBaseNumber = "IPBN-222222222",
                        Type = InterestedPartyType.AR,
                        CisacType = CisacInterestedPartyType.X,
                        Names = new List<NameModel>()
                        {
                            new NameModel
                            {
                                IpNameNumber = 123,
                                FirstName = "IP2FirstName",
                                LastName = "IP2LastName",
                                CreatedDate = DateTime.UtcNow,
                                TypeCode = NameType.PA,
                                ForwardingNameNumber = 888,
                                Agency =  "AgencyCode2"
                            }
                        }
                    }
                },
                    Iswc = "T-1111111111111",
                    Agency = "AgencyOuterString",
                    WorkNumber = new Bdo.Work.WorkNumber
                    {
                        Type = "WorkNumberAgency",
                        Number = "WorkNumberNumber"
                    },
                    DisambiguationReason = Bdo.Work.DisambiguationReason.DIA,
                    DisambiguateFrom = new List<DisambiguateFrom>()
                {
                    new DisambiguateFrom
                    {
                        Iswc = "T-ISWC-DisambugateFrom"
                    }
                },
                    Disambiguation = false,
                    DerivedFrom = new List<Bdo.Work.DerivedFrom>() {
                    new Bdo.Work.DerivedFrom
                    {
                        Iswc = "T-1111-1111-1111"
                    },
                    new Bdo.Work.DerivedFrom
                    {
                        Iswc = "T-2222-2222-2222"
                    }
                },
                    DerivedWorkType = Bdo.Work.DerivedWorkType.Composite,
                    AdditionalAgencyWorkNumbers = new List<AdditionalAgencyWorkNumber> 
                    {
                        new AdditionalAgencyWorkNumber { WorkNumber = new Bdo.Work.WorkNumber { Number = "12343", Type = "122" }, CurrentIswc = "T12334343", IsEligible = true }
                    }
                }
            };

            // Act
            var inputWorkInfo_1 = mapper.Map<Submission, InputWorkInfo>(submission_1);
            var model = submission_1.Model;

            // Assert
            Assert.NotNull(model.Titles.FirstOrDefault(t => inputWorkInfo_1.Titles.Any(x => x.Title.Equals(t.Name))));


            Assert.Equal(model.Iswc, inputWorkInfo_1.Numbers.ElementAt(0).Number);
            Assert.Equal("ARCHIVED", inputWorkInfo_1.Numbers.ElementAt(0).Type);

            Assert.Equal(model.WorkNumber.Type, inputWorkInfo_1.Numbers.ElementAt(1).Type);
            Assert.Equal(model.WorkNumber.Number, inputWorkInfo_1.Numbers.ElementAt(1).Number);

            Assert.Equal(model.InterestedParties.ElementAt(0).IpBaseNumber, inputWorkInfo_1.Contributors.ElementAt(0).IPIBaseNumber);
            Assert.Equal(model.InterestedParties.ElementAt(0).Type.ToString(), inputWorkInfo_1.Contributors.ElementAt(0).Role.ToString());

            Assert.Equal(model.DisambiguateFrom.ElementAt(0).Iswc, inputWorkInfo_1.DisambiguateFromNumbers.ElementAt(0).Number);

            Assert.Contains("12343", inputWorkInfo_1.Numbers.Select(x=> x.Number));

        }

        /// <summary>
        /// Automapper Test that a Submission maps to WorkIfo for Submission sv and All values map as expected for a full Model
        /// </summary>
        [Fact]
        public void Automapper_Map_SubmissionModel_WorkInfo_Submission_Processing_Scenarios()
        {
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);

            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    PreferredIswc = "T2030000746",
                    Agency = "10",
                    SourceDb = 10,
                    Category = Category.DOM,
                    Disambiguation = true,
                    DisambiguationReason = Bdo.Work.DisambiguationReason.DIA,
                    BVLTR = BVLTR.B,
                    Iswc = "234324",
                    CisnetCreatedDate = DateTime.UtcNow,
                    CisnetLastModifiedDate = DateTime.UtcNow,
                    DerivedWorkType = Bdo.Work.DerivedWorkType.Composite,
                    Performers = new List<Bdo.Work.Performer>()
                    {
                        new Bdo.Work.Performer()
                        {
                            Isni = "0000000123456789",
                            Ipn = 12345678,
                            FirstName = "First Name1",
                            LastName = "Last Name1",
                            Designation = "Main Artist"
                        },
                        new Bdo.Work.Performer()
                        {
                            Isni = "0000000987654321",
                            Ipn = 18765432,
                            FirstName = "First Name2",
                            LastName = "Last Name2",
                            Designation = "Other"
                        }
                    },
                    Titles = new List<Bdo.Work.Title>()
                    {
                        new Bdo.Work.Title()
                        {
                            Name = "Test Title",
                            StandardizedName = "TEST TITLE",
                            Type = Bdo.Work.TitleType.OT
                        }
                    },
                    DisambiguateFrom = new List<DisambiguateFrom>()
                    {
                        new DisambiguateFrom()
                        {
                            Iswc = "ISWC"
                        }
                    },
                    DerivedFrom = new List<Bdo.Work.DerivedFrom>()
                    {
                        new Bdo.Work.DerivedFrom()
                        {
                            Iswc = "ISWC",
                            Title=""
                        }
                    },
                    InterestedParties = new List<InterestedPartyModel>()
                    {
                        new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber1",
                            CisacType = CisacInterestedPartyType.AM,
                            Names = new List<NameModel>()
                            {
                                new NameModel()
                                {
                                    IpNameNumber = 1
                                }
                            },
                            DisplayName = "display name"
                        },
                        new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber2",
                            CisacType = CisacInterestedPartyType.E,
                            Names = new List<NameModel>()
                            {
                                new NameModel()
                                {
                                    IpNameNumber = 2
                                }
                            },
                            DisplayName = "display name"
                        },
                        new InterestedPartyModel()
                        {
                            IpBaseNumber = "IpBaseNumber3",
                            CisacType = CisacInterestedPartyType.AM,
                            Names = new List<NameModel>()
                            {
                                new NameModel()
                                {
                                    IpNameNumber = 3
                                }
                            },
                            DisplayName = "display name"
                        }
                    },
                    WorkNumber = new Bdo.Work.WorkNumber()
                    {
                        Number = "12345"
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
                            RecordingTitle = "title",
                            SubTitle = "subtitle",
                            LabelName = "name",
                            ReleaseEmbargoDate = DateTime.Now,
                        },
                        new Bdo.Work.AdditionalIdentifier
                        {
                            WorkCode = "6789",
                            RecordingTitle = "title2",
                            SubTitle = "subtitle2",
                            LabelName = "name",
                            ReleaseEmbargoDate = DateTime.Now,
                            Performers = new List<Bdo.Work.Performer>
                            {
                                new Bdo.Work.Performer
                                {
                                    Isni = "0000000123456789",
                                    Ipn = 12345678,
                                    FirstName = "First Name1",
                                    LastName = "Last Name1",
                                    Designation = "Main Artist"
                                }
                            }
                        }
                    }
                },
                MatchedResult = new Bdo.MatchingEngine.MatchResult()
                {
                    Matches = new List<Bdo.MatchingEngine.MatchingWork>()
                    {
                         new Bdo.MatchingEngine.MatchingWork()
                         {
                            MatchType = Bdo.MatchingEngine.MatchSource.MatchedByText
                         }
                    }
                }

            };


            var workInfo = new WorkInfo()
            {
                IswcId = 1
            };

            workInfo = mapper.Map(submission, workInfo);
            var publishers = submission.Model.InterestedParties.Where(ip => ip.CisacType == Bdo.Ipi.CisacInterestedPartyType.E || ip.CisacType == Bdo.Ipi.CisacInterestedPartyType.AM);
            var performers = submission.Model.Performers;

            Assert.True(workInfo.Status);
            Assert.Equal(submission.Model.InterestedParties.Count(), workInfo.Ipcount);
            Assert.Equal(submission.Model.Iswc, workInfo.ArchivedIswc);
            Assert.False(workInfo.IsReplaced);
            Assert.Equal((int)submission.MatchedResult.Matches.FirstOrDefault().MatchType, workInfo.MatchTypeId);
            Assert.Equal(submission.Model.Category.ToString(), workInfo.MwiCategory);
            Assert.Equal(submission.Model.Agency, workInfo.AgencyId);
            Assert.Equal(submission.IsEligible, workInfo.IswcEligible);
            Assert.Equal(submission.Model.WorkNumber.Number, workInfo.AgencyWorkCode);
            Assert.Equal(submission.Model.SourceDb, workInfo.SourceDatabase);
            Assert.Equal(submission.Model.Disambiguation, workInfo.Disambiguation);
            Assert.Equal((int)submission.Model.DisambiguationReason, workInfo.DisambiguationReasonId);
            Assert.Equal(submission.Model.BVLTR.ToString().ToCharArray()[0], workInfo.Bvltr.ToCharArray()[0]);
            Assert.Equal((int)submission.Model.DerivedWorkType, workInfo.DerivedWorkTypeId);
            Assert.Equal(submission.Model.Titles.Count(), workInfo.Title.Count);
            Assert.Equal(submission.Model.Titles.Count(), workInfo.Title.Count);
            Assert.Equal(performers.Count(), workInfo.WorkInfoPerformer.Count());
            Assert.Equal(submission.Model.InterestedParties.Count(), workInfo.Publisher.Count());
            Assert.Equal(2, workInfo.AdditionalIdentifier.Where(ai => ai.Recording != null).Count());
            Assert.Single(workInfo.AdditionalIdentifier.Where(ai => ai.Recording != null).Select(ai => ai.Recording).Where(r => r.RecordingArtist.Count() > 0));
            Assert.True(workInfo.WorkInfoPerformer.All(wp => 
                performers.Any(p => 
                    p.Isni == wp.Performer.Isni
                    && p.Ipn == wp.Performer.Ipn
                    && p.FirstName == wp.Performer.FirstName 
                    && p.LastName == wp.Performer.LastName
                )
            ));
            Assert.Equal(1, workInfo.WorkInfoPerformer.ElementAt(0).PerformerDesignationId);
            Assert.Equal(2, workInfo.WorkInfoPerformer.ElementAt(1).PerformerDesignationId);
            Assert.True(workInfo.Publisher.All(wp => 
                publishers.Any(p => 
                    p.IpBaseNumber == wp.IpbaseNumber
                    && (int)p.CisacType == wp.RoleTypeId
                    && p.DisplayName == wp.DisplayName
                )
            ));
            Assert.Equal(submission.Model.DerivedFrom.Count(), workInfo.DerivedFrom.Count());
            Assert.Equal(submission.Model.DisambiguateFrom.Count(), workInfo.DerivedFrom.Count());
            Assert.Equal(4, workInfo.AdditionalIdentifier.Count());
        }

        /// <summary>
        /// Automapper Test that an Iswc with WorkInfos maps to IswcModel VerifiedSubmission, filters PG IPs and adds the PG IP if not in creator.
        /// </summary>
        [Fact]
        public void Automapper_Map_Iswc_IswcModel_Should_filter_PG_IPs()
        {
            // Arrange
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);

            var pgIpName = new DataModels.Name
            {
                FirstName = "CHRIS",
                LastName = "HOFFMANN",
                TypeCode = "PG"
            };


            var iswc = new DataModels.Iswc
            {
                Iswc1 = "T9815805835",
                AgencyId = "080",
                Status = true,
                Title = new List<DataModels.Title>
                {
                    new DataModels.Title
                    {
                        Title1 = "TEST PG PATRONYMS",
                        TitleTypeId = 2
                    }
                },
                WorkInfo = new List<WorkInfo>
                {
                    new WorkInfo
                    {
                        Status = true,
                        Creator = new List<Creator> {
                            new Creator
                            {
                                IpbaseNumber = "I-001135213-7",
                                IpnameNumber = 568562507,
                                Status = true,
                                IpnameNumberNavigation = new DataModels.Name
                                {

                                    NameReference = new List<NameReference>
                                    {
                                        new NameReference
                                        {
                                            IpbaseNumber = "I-001135213-7",
                                            IpnameNumber = 568562507,
                                            IpnameNumberNavigation = pgIpName
                                        },
                                        new NameReference
                                        {
                                            IpbaseNumber = "I-001651337-2",
                                            IpnameNumber = 568562507
                                        }
                                    },
                                    TypeCode = "PG"

                                }
                            },
                            new Creator
                            {
                                IpbaseNumber = "I-001651337-2",
                                IpnameNumber = 251745079,
                                Status = true,
                                IpnameNumberNavigation = new DataModels.Name
                                {
                                    FirstName = "CHRISTIAN",
                                    LastName = "BRUNTHALER",
                                    TypeCode = "PA",

                                }
                            },
                            new Creator
                            {
                                IpbaseNumber = "I-001135213-7",
                                IpnameNumber = 251745888,
                                Status = true,
                                IpnameNumberNavigation = new DataModels.Name
                                {
                                    FirstName = "KURT",
                                    LastName = "BRUNTHALER",
                                    TypeCode = "PA",
                                    IpnameNumber = 251745888
                                }
                            },
                            new Creator
                            {
                                IpbaseNumber = "I-001651337-9",
                                IpnameNumber = 31533812,
                                Status = true,
                                IpnameNumberNavigation = new DataModels.Name
                                {

                                    NameReference = new List<NameReference>
                                    {
                                        new NameReference
                                        {
                                            IpbaseNumber = "I-001651337-9",
                                            IpnameNumber = 31533812,
                                            IpnameNumberNavigation = new DataModels.Name
                                            {
                                                FirstName = "GEORGE",
                                                LastName = "YOUNG",
                                                TypeCode = "PA"
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            };

            // Act
            var verifiedSubmission = mapper.Map<IswcModel>(iswc).VerifiedSubmissions.FirstOrDefault();

            // Assert
            Assert.Collection(verifiedSubmission.InterestedParties,
               elem1 =>
               {

                   Assert.False(elem1.IsPseudonymGroupMember);
               },
               elem2 =>
               {

                   Assert.True(elem2.IsPseudonymGroupMember);

               },
               elem3 =>
               {
                   Assert.True(elem3.IsPseudonymGroupMember);
               },
               elem4 =>
               {
                   Assert.False(elem4.IsPseudonymGroupMember);
               });


            Assert.Equal(verifiedSubmission.InterestedParties.Count(), iswc.WorkInfo.FirstOrDefault().Creator.Count());
        }

        /// <summary>
        /// Automapper Test that a maps WorkInfo to a VerifiedSubmissionModel and All values map as expected for a full Model
        /// </summary>
        [Fact]
        public void Automapper_Map_WorkInfo_VerifiedSubmissionModel_Should_Map_All_Values_MappingProfile()
        {
            // Arrange
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);

            var workInfo = new WorkInfo()
            {
                Iswc = new DataModels.Iswc
                {
                    IswcStatusId = 1,
                    Iswc1 = "Iswc1" ,
                },
                AgencyWorkCode = "ABC123",
                AgencyId = "123",
                Title = new List<DataModels.Title>()
                {
                    new DataModels.Title()
                    {
                        TitleId = 1,
                        Title1 = "title1",
                        StandardizedTitle = "standardtitle",
                        TitleTypeId = 1,
                        Status = true
                    },
                    new DataModels.Title()
                    {
                        TitleId = 2,
                        Title1 = "title2",
                        StandardizedTitle = "standardtitle2",
                        TitleTypeId = 2,
                        Status = true
                    },
                    new DataModels.Title()
                    {
                        TitleId = 3,
                        Title1 = "title3",
                        StandardizedTitle = "standardtitle3",
                        TitleTypeId = 3,
                        Status = false
                    },
                },
                WorkInfoPerformer = new List<WorkInfoPerformer>()
                {

                    new WorkInfoPerformer()
                    {
                        Performer = new DataModels.Performer
                        {
                            Isni = "0000000123456789",
                            Ipn = 12345678,
                            FirstName = "First Name1",
                            LastName = "Last Name1",
                        },
                        PerformerDesignationId = 1
                    },
                    new WorkInfoPerformer()
                    {
                        Performer = new DataModels.Performer
                        {
                            Isni = "0000000987654321",
                            Ipn = 18765432,
                            FirstName = "First Name2",
                            LastName = "Last Name2",
                        },
                        PerformerDesignationId = 2
                    }
                },
                DerivedFrom = new List<DataModels.DerivedFrom>()
                {
                    new DataModels.DerivedFrom()
                    {
                        Iswc = "iswc",
                        Title = "title",
                        Status = true,
                    },
                    new DataModels.DerivedFrom()
                    {
                        Iswc = "iswc",
                        Title = "title",
                        Status = false,
                    }
                },
                AdditionalIdentifier = new List<DataModels.AdditionalIdentifier>()
                {
                    new DataModels.AdditionalIdentifier()
                    {
                        WorkIdentifier = "workidentifier",
                        NumberType = new NumberType() { Code = "code" },
                        Recording = new Recording()
                        {
                            RecordingTitle = "title",
                            SubTitle = "subtitle",
                            LabelName = "label",
                            ReleaseEmbargoDate = DateTime.Now,
                        }
                    },
                    new DataModels.AdditionalIdentifier()
                    {
                        WorkIdentifier = "workidentifier",
                        NumberType = new NumberType() { Code = "code" },
                        Recording = new Recording()
                        {
                            RecordingTitle = "title",
                            SubTitle = "subtitle",
                            LabelName = "label",
                            ReleaseEmbargoDate = DateTime.Now,
                        }
                    },
                },
                Publisher = new List<Publisher> 
                { 
                    new Publisher()
                    {
                        FirstName = "firstname",
                        LastName = "LastName",
                        DisplayName = "DisplayName",
                        Status = true
                    },
                    new Publisher()
                    {
                        FirstName = "firstname",
                        LastName = "LastName",
                        DisplayName = "DisplayName",
                        Status = false
                    }
                },
                Creator = new List<Creator> 
                {
                    new Creator()
                    {
                        FirstName = "firstname",
                        LastName = "LastName",
                        DisplayName = "DisplayNameCreator",
                        Status = true
                    }
                }
                
            };

            var submissionModel = new VerifiedSubmissionModel();

            // Act
            submissionModel = mapper.Map(workInfo, submissionModel);

            // Assert
            Assert.Equal(workInfo.Iswc.Iswc1, submissionModel.Iswc);

            Assert.Equal(submissionModel.WorkNumber.Number, workInfo.AgencyWorkCode);
            Assert.Equal(submissionModel.WorkNumber.Type, workInfo.AgencyId);

            Assert.Equal(submissionModel.Agency, workInfo.AgencyId);

            Assert.Equal(2, submissionModel.Titles.Count());
            Assert.Equal(submissionModel.Titles.ElementAt(0).Name, workInfo.Title.ElementAt(0).Title1);

            Assert.Equal("Preferred", submissionModel.IswcStatus);

            Assert.Equal(2, submissionModel.Performers.Count());
            Assert.Equal(workInfo.WorkInfoPerformer.ElementAt(0).Performer.Isni, submissionModel.Performers.ElementAt(0).Isni);
            Assert.Equal(workInfo.WorkInfoPerformer.ElementAt(0).Performer.Ipn, submissionModel.Performers.ElementAt(0).Ipn);
            Assert.Equal("Main Artist", submissionModel.Performers.ElementAt(0).Designation);
            Assert.Equal("Other", submissionModel.Performers.ElementAt(1).Designation);

            Assert.Single(submissionModel.DerivedFrom);

            Assert.Equal(2, submissionModel.AdditionalIdentifiers.Count());
            Assert.Equal(submissionModel.AdditionalIdentifiers.ElementAt(0).SubmitterCode, workInfo.AdditionalIdentifier.ElementAt(0).NumberType.Code);
            Assert.Equal(submissionModel.AdditionalIdentifiers.ElementAt(0).WorkCode, workInfo.AdditionalIdentifier.ElementAt(0).WorkIdentifier);
            Assert.Equal(submissionModel.AdditionalIdentifiers.ElementAt(0).RecordingTitle, workInfo.AdditionalIdentifier.ElementAt(0).Recording.RecordingTitle);
            Assert.Equal(submissionModel.AdditionalIdentifiers.ElementAt(0).ReleaseEmbargoDate, workInfo.AdditionalIdentifier.ElementAt(0).Recording.ReleaseEmbargoDate);

            Assert.Equal(2, submissionModel.InterestedParties.Count());
            Assert.Equal(submissionModel.InterestedParties.ElementAt(0).DisplayName, workInfo.Creator.ElementAt(0).DisplayName);
            Assert.Equal(submissionModel.InterestedParties.ElementAt(1).DisplayName, workInfo.Publisher.ElementAt(0).DisplayName);
        }
    }
}