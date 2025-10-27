using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1.Data
{
    public class Submissions
    {
        public static Submission EligibleLabelSubmissionPRS => new Submission
        {
            Workcode = TestBase.CreateNewLabelWorkCode(),
            OriginalTitle = TestBase.CreateNewTitle(),
            Agency = "052",
            InterestedParties = InterestedParties.IP,
            AdditionalIdentifiers = new AdditionalIdentifiers
            {
                LabelIdentifiers = new List<LabelIdentifiers>
                {
                    new LabelIdentifiers
                    {
                        SubmitterDPID = "DPID",
                        WorkCode = new List<string>{ TestBase.CreateNewWorkCode() }
                    }
                },
                Recordings = new List<Recordings>
                {
                    new Recordings
                    {
                        Isrc = TestBase.CreateNewWorkCode(),
                        RecordingTitle = TestBase.CreateNewTitleWithLettersOnly()
                    }
                }
            }
        };

        public static Submission EligibleLabelSubmissionSESAC => new Submission
        {
            Workcode = TestBase.CreateNewLabelWorkCode(),
            OriginalTitle = TestBase.CreateNewTitleWithLettersOnly(),
            Agency = "071",
            InterestedParties = InterestedParties.IP_SESAC,
            AdditionalIdentifiers = new AdditionalIdentifiers
            {
                LabelIdentifiers = new List<LabelIdentifiers>
                {
                    new LabelIdentifiers
                    {
                        SubmitterDPID = "DPID",
                        WorkCode = new List<string>{ TestBase.CreateNewWorkCode() }
                    }
                },
                Recordings = new List<Recordings>
                {
                    new Recordings
                    {
                        Isrc = TestBase.CreateNewWorkCode(),
                        RecordingTitle = TestBase.CreateNewTitleWithLettersOnly()
                    }
                }
            }
        };

        public static Submission EligibleLabelSubmissionCISAC => new Submission
        {
            Workcode = TestBase.CreateNewLabelWorkCode(),
            OriginalTitle = TestBase.CreateNewTitleWithLettersOnly(),
            Agency = "312",
            InterestedParties = InterestedParties.IP_CISAC,
            Performers = Performers.Performers_CISAC,
            AdditionalIdentifiers = new AdditionalIdentifiers
            {
                LabelIdentifiers = new List<LabelIdentifiers>
                {
                    new LabelIdentifiers
                    {
                        SubmitterDPID = "DPID",
                        WorkCode = new List<string>{ TestBase.CreateNewWorkCode() }
                    }
                },
                Recordings = new List<Recordings>
                {
                    new Recordings
                    {
                        Isrc = TestBase.CreateNewWorkCode(),
                        RecordingTitle = TestBase.CreateNewTitleWithLettersOnly(),
                        LabelName = "Columbia"
                    }
                }
            }
        };
    }
}
