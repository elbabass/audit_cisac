import { ISubmissionAuditReportResult, IFileAuditReportResult } from '../types/ReportTypes';

export const SUBMISSION_AUDIT_SEARCH_REQUEST = 'SUBMISSION_AUDIT_SEARCH_REQUEST';
export const SUBMISSION_AUDIT_SEARCH_SUCCESS = 'SUBMISSION_AUDIT_SEARCH_SUCCESS';
export const SUBMISSION_AUDIT_SEARCH_FAILURE = 'SUBMISSION_AUDIT_SEARCH_FAILURE';

export const FILE_SUBMISSION_AUDIT_SEARCH_REQUEST = 'FILE_SUBMISSION_AUDIT_SEARCH_REQUEST';
export const FILE_SUBMISSION_AUDIT_SEARCH_SUCCESS = 'FILE_SUBMISSION_AUDIT_SEARCH_SUCCESS';
export const FILE_SUBMISSION_AUDIT_SEARCH_FAILURE = 'FILE_SUBMISSION_AUDIT_SEARCH_FAILURE';

export const EXTRACT_TO_FTP_REQUEST = 'EXTRACT_TO_FTP_REQUEST';
export const EXTRACT_TO_FTP_SUCCESS = 'EXTRACT_TO_FTP_SUCCESS';
export const EXTRACT_TO_FTP_FAILURE = 'EXTRACT_TO_FTP_FAILURE';

export const AGENCY_STATISTICS_SEARCH_REQUEST = 'AGENCY_STATISTICS_SEARCH_REQUEST';
export const AGENCY_STATISTICS_SEARCH_SUCCESS = 'AGENCY_STATISTICS_SEARCH_SUCCESS';
export const AGENCY_STATISTICS_SEARCH_SUCCESS_SUB_AUDIT =
  'AGENCY_STATISTICS_SEARCH_SUCCESS_SUB_AUDIT';
export const AGENCY_STATISTICS_SEARCH_FAILURE = 'AGENCY_STATISTICS_SEARCH_FAILURE';

export const GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_REQUEST =
  'GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_REQUEST';
export const GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_SUCCESS =
  'GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_SUCCESS';
export const GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_FAILURE =
  'GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_FAILURE';

export const GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_REQUEST =
    'GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_REQUEST';
export const GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_SUCCESS =
    'GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_SUCCESS';
export const GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_FAILURE =
    'GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_FAILURE';

export const GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_REQUEST =
  'GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_REQUEST';
export const GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_SUCCESS =
  'GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_SUCCESS';
export const GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_FAILURE =
  'GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_FAILURE';

export const GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_REQUEST =
  'GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_REQUEST';
export const GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_SUCCESS =
  'GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_SUCCESS';
export const GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_FAILURE =
  'GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_FAILURE';

export const CLEAR_REPORTS_ERROR = 'CLEAR_REPORTS_ERROR';

export interface SubmissionAuditSearchRequest {
  type: typeof SUBMISSION_AUDIT_SEARCH_REQUEST;
}

export interface SubmissionAuditSearchSuccess {
  type: typeof SUBMISSION_AUDIT_SEARCH_SUCCESS;
  payload: {
    submissionAuditSearchResults: ISubmissionAuditReportResult[];
  };
}

export interface SubmissionAuditSearchFailure {
  type: typeof SUBMISSION_AUDIT_SEARCH_FAILURE;
  payload: {
    error: any;
  };
}

export interface FileSubmissionAuditSearchRequest {
  type: typeof FILE_SUBMISSION_AUDIT_SEARCH_REQUEST;
}

export interface FileSubmissionAuditSearchSuccess {
  type: typeof FILE_SUBMISSION_AUDIT_SEARCH_SUCCESS;
  payload: {
    fileSubmissionAuditSearchResults: IFileAuditReportResult[];
  };
}

export interface FileSubmissionAuditSearchFailure {
  type: typeof FILE_SUBMISSION_AUDIT_SEARCH_FAILURE;
  payload: {
    error: any;
  };
}
export interface ExtractToFtpRequest {
  type: typeof EXTRACT_TO_FTP_REQUEST;
}

export interface ExtractToFtpSuccess {
  type: typeof EXTRACT_TO_FTP_SUCCESS;
}

export interface ExtractToFtpFailure {
  type: typeof EXTRACT_TO_FTP_FAILURE;
  payload: {
    extractError: any;
  };
}

export interface AgencyStatisticsSearchRequest {
  type: typeof AGENCY_STATISTICS_SEARCH_REQUEST;
}

export interface AgencyStatisticsSearchSuccess {
  type: typeof AGENCY_STATISTICS_SEARCH_SUCCESS;
  payload: {
    agencyStatisticsSearchResults: any;
  };
}

export interface AgencyStatisticsSearchSuccessSubAudit {
  type: typeof AGENCY_STATISTICS_SEARCH_SUCCESS_SUB_AUDIT;
  payload: {
    agencyStatisticsSearchResultsSubAudit: any;
  };
}

export interface AgencyStatisticsSearchFailure {
  type: typeof AGENCY_STATISTICS_SEARCH_FAILURE;
  payload: {
    agencyStatisticsError: any;
  };
}

export interface ClearReportsError {
  type: typeof CLEAR_REPORTS_ERROR;
}

export interface getDateOfCachedIswcFullExtractReportRequest {
  type: typeof GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_REQUEST;
}

export interface getDateOfCachedIswcFullExtractReportSuccess {
  type: typeof GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_SUCCESS;
  payload: {
    fullExtractCachedVersion: string;
  };
}

export interface getDateOfCachedIswcFullExtractReportFailure {
  type: typeof GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_FAILURE;
  payload: {
    getFullExtractCachedError: any;
  };
}

export interface getDateOfCachedIswcCreatorExtractReportRequest {
    type: typeof GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_REQUEST;
}

export interface getDateOfCachedIswcCreatorExtractReportSuccess {
    type: typeof GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_SUCCESS;
    payload: {
        creatorExtractCachedVersion: string;
    };
}

export interface getDateOfCachedIswcCreatorExtractReportFailure {
    type: typeof GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_FAILURE;
    payload: {
        getCreatorExtractCachedError: any;
    };
}

export interface getDateOfCachedAgencyWorkListReportRequest {
  type: typeof GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_REQUEST;
}

export interface getDateOfCachedAgencyWorkListReportSuccess {
  type: typeof GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_SUCCESS;
  payload: {
    agencyWorkListCachedVersion: string;
  };
}

export interface getDateOfCachedAgencyWorkListReportFailure {
  type: typeof GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_FAILURE;
  payload: {
    getAgencyWorkListCachedError: any;
  };
}

export interface GetDateOfCachedPotentialDuplicatesReportRequest {
  type: typeof GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_REQUEST;
}

export interface GetDateOfCachedPotentialDuplicatesReportSuccess {
  type: typeof GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_SUCCESS;
  payload: {
    potentialDuplicatesCachedVersion: string;
  };
}

export interface GetDateOfCachedPotentialDuplicatesReportFailure {
  type: typeof GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_FAILURE;
  payload: {
    getPotentialDuplicatesCachedError: any;
  };
}

export type ReportsActionTypes =
  | SubmissionAuditSearchRequest
  | SubmissionAuditSearchSuccess
  | SubmissionAuditSearchFailure
  | FileSubmissionAuditSearchRequest
  | FileSubmissionAuditSearchSuccess
  | FileSubmissionAuditSearchFailure
  | ExtractToFtpRequest
  | ExtractToFtpSuccess
  | ExtractToFtpFailure
  | AgencyStatisticsSearchRequest
  | AgencyStatisticsSearchSuccess
  | AgencyStatisticsSearchSuccessSubAudit
  | AgencyStatisticsSearchFailure
  | ClearReportsError
  | getDateOfCachedIswcFullExtractReportRequest
  | getDateOfCachedIswcFullExtractReportSuccess
  | getDateOfCachedIswcFullExtractReportFailure
  | GetDateOfCachedPotentialDuplicatesReportRequest
  | GetDateOfCachedPotentialDuplicatesReportSuccess
  | GetDateOfCachedPotentialDuplicatesReportFailure
  | getDateOfCachedAgencyWorkListReportRequest
  | getDateOfCachedAgencyWorkListReportSuccess
  | getDateOfCachedAgencyWorkListReportFailure
  | getDateOfCachedIswcCreatorExtractReportRequest
  | getDateOfCachedIswcCreatorExtractReportSuccess
  | getDateOfCachedIswcCreatorExtractReportFailure;
