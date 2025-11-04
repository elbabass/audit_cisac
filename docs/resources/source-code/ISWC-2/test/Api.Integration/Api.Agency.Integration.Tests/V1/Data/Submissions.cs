using System.Linq;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data
{
    /// <summary>
    /// Eligible submissions for specified agencies.
    /// </summary>
    public static class Submissions
    {
        public static Submission EligibleSubmissionAEPI => new Submission
        {
            Workcode = TestBase.CreateNewWorkCode(),
            OriginalTitle = TestBase.CreateNewTitle(),
            Sourcedb = 300,
            Agency = "003",
            Disambiguation = false,
            InterestedParties = InterestedParties.IP_AEPI.Take(2).ToList(),
            AdditionalIdentifiers = new AdditionalIdentifiers()
        };

        public static Submission EligibleSubmissionIMRO => new Submission
        {
            Sourcedb = 128,
            Workcode = TestBase.CreateNewWorkCode(),
            Agency = "128",
            Disambiguation = false,
            OriginalTitle = TestBase.CreateNewTitle(),
            InterestedParties = InterestedParties.IP_IMRO.Take(2).ToList(),
            AdditionalIdentifiers = new AdditionalIdentifiers()
        };

        public static Submission EligibleSubmissionAKKA => new Submission
        {
            Sourcedb = 308,
            Workcode = TestBase.CreateNewWorkCode(),
            Agency = "122",
            OriginalTitle = TestBase.CreateNewTitle(),
            InterestedParties = InterestedParties.IP_AKKA.Take(2).ToList(),
            AdditionalIdentifiers = new AdditionalIdentifiers()
        };

        public static Submission EligibleSubmissionPRS => new Submission
        {
            Sourcedb = 308,
            Agency = "052",
            Workcode = TestBase.CreateNewWorkCode(),
            OriginalTitle = TestBase.CreateNewTitle(),
            InterestedParties = InterestedParties.IP_PRS.Take(2).ToList(),
            AdditionalIdentifiers = new AdditionalIdentifiers()
        };

        public static Submission EligibleSubmissionSACEM => new Submission
        {
            Sourcedb = 58,
            Agency = "058",
            Workcode = TestBase.CreateNewWorkCode(),
            OriginalTitle = TestBase.CreateNewTitle(),
            InterestedParties = InterestedParties.IP_SACEM.Take(2).ToList(),
            AdditionalIdentifiers = new AdditionalIdentifiers()
        };

        public static Submission EligibleSubmissionBMI => new Submission
        {
            Sourcedb = 21,
            Agency = "021",
            Workcode = TestBase.CreateNewWorkCode(),
            OriginalTitle = TestBase.CreateNewTitle(),
            InterestedParties = InterestedParties.IP_BMI.Take(2).ToList(),
            AdditionalIdentifiers = new AdditionalIdentifiers()
        };

        public static Submission EligibleSubmissionASCAP => new Submission
        {
            Sourcedb = 10,
            Agency = "010",
            Workcode = TestBase.CreateNewWorkCode(),
            OriginalTitle = TestBase.CreateNewTitle(),
            InterestedParties = InterestedParties.IP_ASCAP.Take(2).ToList(),
            AdditionalIdentifiers = new AdditionalIdentifiers()
        };

        public static Submission EligibleSubmissionECAD => new Submission
        {
            Sourcedb = 308,
            Agency = "308",
            Workcode = TestBase.CreateNewWorkCode(),
            OriginalTitle = TestBase.CreateNewTitle(),
            InterestedParties = InterestedParties.IP_AMAR.Take(2).ToList(),
            AdditionalIdentifiers = new AdditionalIdentifiers()
        };
    }
}
