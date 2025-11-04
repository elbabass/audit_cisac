namespace SpanishPoint.Azure.Iswc.Bdo.Edi
{
    public enum TransactionType
    {
        /// <summary>
        /// Add Submission
        /// </summary>
        CAR,
        /// <summary>
        /// Update Submission
        /// </summary>
        CUR,
        /// <summary>
        /// Delete Submission
        /// </summary>
        CDR,
        /// <summary>
        /// Iswc Metadata Query (Search by Iswc)
        /// </summary>
        CMQ,
        /// <summary>
        /// Iswc Query (Search by Agency Work Code or Title/Contributor)
        /// </summary>
        CIQ,
        /// <summary>
        /// Merge Submission
        /// </summary>
        MER,
        /// <summary>
        /// De-merge Submission
        /// </summary>
        DMR,
		/// <summary>
		/// Update Workflow task
		/// </summary>
		COA,
		/// <summary>
		/// Find workflow tasks
		/// </summary>
		COR,
        /// <summary>
		/// Resolve submission
		/// </summary>
        FSQ
    }
}
