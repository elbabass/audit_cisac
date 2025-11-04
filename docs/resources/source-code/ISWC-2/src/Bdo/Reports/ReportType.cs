using System;
using System.Collections.Generic;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Bdo.Reports
{
    public enum ReportType
    {
        /// <summary>
        /// Agency Interest Extract 
        /// </summary>
        AgencyInterestExtract,
        /// <summary>
        /// ISWC Full Extract
        /// </summary>
        IswcFullExtract,
        /// <summary>
        /// Submission Audit
        /// </summary>
        SubmissionAudit,
        /// <summary>
        ///File Submission Audit
        /// </summary>
        FileSubmissionAudit,
        /// <summary>
        ///  Agency Statistics
        /// </summary>
        AgencyStatistics,
        /// <summary>
        /// Publisher ISWC Tracking
        /// </summary>
        PublisherIswcTracking,
        /// <summary>
        /// Potential Duplicates
        /// </summary>
        PotentialDuplicates,
        /// <summary>
        /// Agency Work List
        /// </summary>
        AgencyWorkList,
        /// <summary>
        /// Agency Work List
        /// </summary>
        CreatorReport
    }
}
