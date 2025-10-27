using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.MatchingEngine;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Tests
{
    /// <summary>
    /// Processing test base class
    /// </summary>
    public class ProcessingTestBase
    {
        /// <summary>
        /// Test Submission
        /// </summary>
        public Submission SubmissionForMapping
        {
            get
            {
                return new Submission()
                {
                    Model = new SubmissionModel()
                    {
                        PreferredIswc = "T2030000746",
                        Agency = "10",
                        SourceDb = 10,
                        Category = Category.DOM,
                        Disambiguation = true,
                        DisambiguationReason = DisambiguationReason.DIA,
                        BVLTR = BVLTR.B,
                        Iswc = "234324",
                        CisnetCreatedDate = DateTime.UtcNow,
                        CisnetLastModifiedDate = DateTime.UtcNow,
                        DerivedWorkType = DerivedWorkType.Composite,
                        Instrumentation = new List<Instrumentation>()
                        {
                            new Instrumentation("3")

                        },
                        Performers = new List<Performer>()
                        {
                            new Performer()
                            {
                                FirstName = "Frist Name1",
                                LastName = "Last Name1"
                            },
                            new Performer()
                            {
                                FirstName = "Frist Name2",
                                LastName = "Last Name2"
                            }
                        },
                        Titles = new List<Title>()
                        {
                            new Title()
                            {
                                Name = "Test Title",
                                StandardizedName = "TEST TITLE",
                                Type = TitleType.OT
                            }
                        },
                        DisambiguateFrom = new List<DisambiguateFrom>()
                        {
                            new DisambiguateFrom()
                            {
                                Iswc = "ISWC"
                            }
                        },
                        DerivedFrom = new List<DerivedFrom>()
                        {
                            new DerivedFrom()
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
                            }
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
                            }
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
                            }
                        },
                    },
                        WorkNumber = new Bdo.Work.WorkNumber()
                        {
                            Number = "12345"
                        },
                    AdditionalIdentifiers = new List<AdditionalIdentifier>
                    {
                        new AdditionalIdentifier
                        {
                            WorkCode = "123ABC456",
                            SubmitterCode = "ISRC"
                        }
                    }
                    },
                    MatchedResult = new MatchResult()
                    {
                        Matches = new List<MatchingWork>()
                    {
                         new MatchingWork()
                         {
                            MatchType = MatchSource.MatchedByText
                         }
                    }
                    }

                };
            }
        }

        /// <summary>
        /// Test submission for scenario AS_10 
        /// </summary>
        public Submission SubmissionAS_10
        {
            get
            {
                return new Submission
                {
                    Model = new SubmissionModel
                    {
                        Agency = "1",
                        InterestedParties = new List<InterestedPartyModel>
                        {
                        new InterestedPartyModel
                            {
                                IPNameNumber = 1,
                                CisacType = CisacInterestedPartyType.C,
                                IsAuthoritative = false
                            }
                        }
                    }
                };
            }
        }

        /// <summary>
        /// ISWC object for mapping test AS_01 
        /// </summary>
        public Data.DataModels.Iswc IswcMapping
        {
            get
            {
                return new Data.DataModels.Iswc()
                {
                    Status = true,
                    LastModifiedDate = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    AgencyId = "1",
                    LastModifiedUserId = 5,
                    Iswc1 = "T0001"
                };
            }
        }
    }
}
