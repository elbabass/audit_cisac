namespace SpanishPoint.Azure.Iswc.Bdo.Work
{
    public enum InstanceStatus
    {
        /// <summary>
        /// Workflow Instance status is Outstanding
        /// </summary>
        Outstanding = 0,

        /// <summary>
        /// Workflow Instance status is Approved
        /// </summary>
        Approved = 1,

        /// <summary>
        /// Workflow Instance status is Rejected
        /// </summary>
        Rejected = 2,

        /// <summary>
        /// Workflow Instance status is Cancelled
        /// </summary>
        Cancelled = 3
    }
}
