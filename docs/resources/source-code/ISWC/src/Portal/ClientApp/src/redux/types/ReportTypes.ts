export interface ISubmissionAuditReportResult {
  auditRequestId: string;
  auditId: string;
  recordId: string;
  agencyCode: string;
  createdDate: string;
  isProcessingError: boolean;
  isProcessingFinished: boolean;
  code: string;
  message: string;
  transactionType: string;
  preferredIswc: string;
  agencyWorkCode: string;
  sourcedb: number;
  originalTitle: string;
  creatorNames: string;
  creatorNameNumbers: string;
  publisherNameNumber: number;
  publisherWorkNumber: string;
}

export interface IFileAuditReportResult {
  auditId: string;
  agencyCode: string;
  datePickedUp: string;
  dateAckGenerated: string;
  fileName: string;
  ackFileName: string;
  status: string;
  publisherNameNumber: string;
}

export interface IAgencyStatisticsReportResult {
  day: number;
  month: number;
  year: number;
  agencyCode?: string;
  validSubmissions: number;
  invalidSubmissions: number;
  eligibleIswcSubmissions: number;
  eligibleIswcSubmissionsWithDisambiguation: number;
  inEligibleIswcSubmissions: number;
  inEligibleIswcSubmissionsWithDisambiguation: number;
  workflowTasksAssigned: number;
  workflowTasksAssignedApproved: number;
  workflowTasksAssignedPending: number;
  workflowTasksAssignedRejected: number;
}
