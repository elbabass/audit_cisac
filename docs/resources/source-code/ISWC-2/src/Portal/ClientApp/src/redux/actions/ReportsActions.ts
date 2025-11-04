import {
  ReportsActionTypes,
  SUBMISSION_AUDIT_SEARCH_REQUEST,
  SUBMISSION_AUDIT_SEARCH_SUCCESS,
  SUBMISSION_AUDIT_SEARCH_FAILURE,
  FILE_SUBMISSION_AUDIT_SEARCH_REQUEST,
  FILE_SUBMISSION_AUDIT_SEARCH_SUCCESS,
  FILE_SUBMISSION_AUDIT_SEARCH_FAILURE,
  EXTRACT_TO_FTP_REQUEST,
  EXTRACT_TO_FTP_SUCCESS,
  EXTRACT_TO_FTP_FAILURE,
  AGENCY_STATISTICS_SEARCH_REQUEST,
  AGENCY_STATISTICS_SEARCH_SUCCESS,
  AGENCY_STATISTICS_SEARCH_FAILURE,
  CLEAR_REPORTS_ERROR,
  GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_SUCCESS,
  GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_REQUEST,
  GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_FAILURE,
  AGENCY_STATISTICS_SEARCH_SUCCESS_SUB_AUDIT,
  GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_REQUEST,
  GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_SUCCESS,
  GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_FAILURE,
  GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_SUCCESS,
  GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_REQUEST,
  GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_FAILURE,
  GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_REQUEST,
  GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_SUCCESS,
  GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_FAILURE
} from './ReportsActionTypes';
import {
  ISubmissionAuditReportResult,
  IFileAuditReportResult,
  IAgencyStatisticsReportResult,
} from '../types/ReportTypes';

export function submissionAuditSearchRequest(): ReportsActionTypes {
  return {
    type: SUBMISSION_AUDIT_SEARCH_REQUEST,
  };
}

export function submissionAuditSearchSuccess(
  submissionAuditSearchResults: ISubmissionAuditReportResult[],
): ReportsActionTypes {
  return {
    type: SUBMISSION_AUDIT_SEARCH_SUCCESS,
    payload: {
      submissionAuditSearchResults,
    },
  };
}

export function submissionAuditSearchFailure(error: any): ReportsActionTypes {
  return {
    type: SUBMISSION_AUDIT_SEARCH_FAILURE,
    payload: {
      error,
    },
  };
}

export function fileSubmissionAuditSearchRequest(): ReportsActionTypes {
  return {
    type: FILE_SUBMISSION_AUDIT_SEARCH_REQUEST,
  };
}

export function fileSubmissionAuditSearchSuccess(
  fileSubmissionAuditSearchResults: IFileAuditReportResult[],
): ReportsActionTypes {
  return {
    type: FILE_SUBMISSION_AUDIT_SEARCH_SUCCESS,
    payload: {
      fileSubmissionAuditSearchResults,
    },
  };
}

export function fileSubmissionAuditSearchFailure(error: any): ReportsActionTypes {
  return {
    type: FILE_SUBMISSION_AUDIT_SEARCH_FAILURE,
    payload: {
      error,
    },
  };
}
export function extractToFtpRequest(): ReportsActionTypes {
  return {
    type: EXTRACT_TO_FTP_REQUEST,
  };
}

export function extractToFtpSuccess(): ReportsActionTypes {
  return {
    type: EXTRACT_TO_FTP_SUCCESS,
  };
}

export function extractToFtpFailure(extractError: any): ReportsActionTypes {
  return {
    type: EXTRACT_TO_FTP_FAILURE,
    payload: {
      extractError,
    },
  };
}

export function agecnyStatisticsSearchRequest(): ReportsActionTypes {
  return {
    type: AGENCY_STATISTICS_SEARCH_REQUEST,
  };
}

export function agecnyStatisticsSearchSuccess(
  agencyStatisticsSearchResults: IAgencyStatisticsReportResult[],
): ReportsActionTypes {
  return {
    type: AGENCY_STATISTICS_SEARCH_SUCCESS,
    payload: {
      agencyStatisticsSearchResults,
    },
  };
}

export function agecnyStatisticsSearchSuccessSubAudit(
  agencyStatisticsSearchResultsSubAudit: IAgencyStatisticsReportResult[],
): ReportsActionTypes {
  return {
    type: AGENCY_STATISTICS_SEARCH_SUCCESS_SUB_AUDIT,
    payload: {
      agencyStatisticsSearchResultsSubAudit,
    },
  };
}

export function agecnyStatisticsSearchFailure(agencyStatisticsError: any): ReportsActionTypes {
  return {
    type: AGENCY_STATISTICS_SEARCH_FAILURE,
    payload: {
      agencyStatisticsError,
    },
  };
}

export function clearReportsError(): ReportsActionTypes {
  return {
    type: CLEAR_REPORTS_ERROR,
  };
}

export function getDateOfCachedIswcFullExtractReportRequest(): ReportsActionTypes {
  return {
    type: GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_REQUEST,
  };
}

export function getDateOfCachedIswcFullExtractReportSuccess(
  fullExtractCachedVersion: string,
): ReportsActionTypes {
  return {
    type: GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_SUCCESS,
    payload: {
      fullExtractCachedVersion,
    },
  };
}

export function getDateOfCachedIswcFullExtractReportFailure(
  getFullExtractCachedError: any,
): ReportsActionTypes {
  return {
    type: GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_FAILURE,
    payload: {
      getFullExtractCachedError,
    },
  };
}

export function getDateOfCachedIswcCreatorExtractReportRequest(): ReportsActionTypes {
    return {
        type: GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_REQUEST,
    };
}

export function getDateOfCachedIswcCreatorExtractReportSuccess(
    creatorExtractCachedVersion: string,
): ReportsActionTypes {
    return {
        type: GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_SUCCESS,
        payload: {
            creatorExtractCachedVersion,
        },
    };
}

export function getDateOfCachedIswcCreatorExtractReportFailure(
    getCreatorExtractCachedError: any,
): ReportsActionTypes {
    return {
        type: GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_FAILURE,
        payload: {
            getCreatorExtractCachedError,
        },
    };
}

export function getDateOfCachedAgencyWorkListReportRequest(): ReportsActionTypes {
  return {
    type: GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_REQUEST,
  };
}

export function getDateOfCachedAgencyWorkListReportSuccess(
  agencyWorkListCachedVersion: string,
): ReportsActionTypes {
  return {
    type: GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_SUCCESS,
    payload: {
      agencyWorkListCachedVersion,
    },
  };
}

export function getDateOfCachedAgencyWorkListReportFailure(
  getAgencyWorkListCachedError: any,
): ReportsActionTypes {
  return {
    type: GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_FAILURE,
    payload: {
      getAgencyWorkListCachedError,
    },
  };
}

export function getDateOfCachedPotentialDuplicatesReportRequest(): ReportsActionTypes {
  return {
    type: GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_REQUEST,
  };
}

export function getDateOfCachedPotentialDuplicatesReportSuccess(
  potentialDuplicatesCachedVersion: string,
): ReportsActionTypes {
  return {
    type: GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_SUCCESS,
    payload: {
      potentialDuplicatesCachedVersion,
    },
  };
}

export function getDateOfCachedPotentialDuplicatesReportFailure(
  getPotentialDuplicatesCachedError: any,
): ReportsActionTypes {
  return {
    type: GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_FAILURE,
    payload: {
      getPotentialDuplicatesCachedError,
    },
  };
}
