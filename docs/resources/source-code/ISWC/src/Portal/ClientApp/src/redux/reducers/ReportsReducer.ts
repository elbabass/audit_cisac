import { Reducer } from 'redux';
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
  GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_FAILURE,
  GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_REQUEST,
  GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_SUCCESS,
  GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_FAILURE,
  GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_REQUEST,
  AGENCY_STATISTICS_SEARCH_SUCCESS_SUB_AUDIT,
  GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_REQUEST,
  GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_SUCCESS,
  GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_FAILURE,
  GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_REQUEST,
  GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_SUCCESS,
  GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_FAILURE
} from '../actions/ReportsActionTypes';
import {
  ISubmissionAuditReportResult,
  IFileAuditReportResult,
  IAgencyStatisticsReportResult,
} from '../types/ReportTypes';

export interface IReportsReducerState {
  loading: boolean;
  error?: any;
  getFullExtractCachedError?: any;
  getAgencyWorkListCachedError?: any;
  getPotentialDuplicatesCachedError?: any;
  submissionAuditSearchResults?: ISubmissionAuditReportResult[];
  fileSubmissionAuditSearchResults?: IFileAuditReportResult[];
  agencyStatisticsSearchResults?: IAgencyStatisticsReportResult[];
  agencyStatisticsSearchResultsSubAudit?: IAgencyStatisticsReportResult[];
  fullExtractCachedVersion: string;
  agencyWorkListCachedVersion: string;
  potentialDuplicatesCachedVersion: string;
  extractToFtpSuccess: boolean;
  getCreatorExtractCachedError?: any;
  creatorExtractCachedVersion: string;
}

const initialState: IReportsReducerState = {
  loading: false,
  fullExtractCachedVersion: '',
  agencyWorkListCachedVersion: '',
  potentialDuplicatesCachedVersion: '',
  creatorExtractCachedVersion: '',
  extractToFtpSuccess: false,
};

export const reducer: Reducer<IReportsReducerState> = (
  state = initialState,
  action: ReportsActionTypes,
): IReportsReducerState => {
  switch (action.type) {
    case SUBMISSION_AUDIT_SEARCH_REQUEST:
      return {
        ...state,
        loading: true,
        error: undefined,
      };
    case SUBMISSION_AUDIT_SEARCH_SUCCESS:
      const { submissionAuditSearchResults } = action.payload;
      return {
        ...state,
        loading: false,
        submissionAuditSearchResults: submissionAuditSearchResults,
      };
    case SUBMISSION_AUDIT_SEARCH_FAILURE:
      const { error } = action.payload;
      return {
        ...state,
        loading: false,
        error: error,
      };
    case FILE_SUBMISSION_AUDIT_SEARCH_REQUEST:
      return {
        ...state,
        loading: true,
        error: undefined,
      };
    case FILE_SUBMISSION_AUDIT_SEARCH_SUCCESS:
      const { fileSubmissionAuditSearchResults } = action.payload;
      return {
        ...state,
        loading: false,
        fileSubmissionAuditSearchResults,
      };
    case FILE_SUBMISSION_AUDIT_SEARCH_FAILURE:
      return {
        ...state,
        loading: false,
        error: action.payload.error,
      };
    case EXTRACT_TO_FTP_REQUEST:
      return {
        ...state,
        loading: true,
        error: undefined,
        extractToFtpSuccess: false,
      };
    case EXTRACT_TO_FTP_SUCCESS:
      return {
        ...state,
        loading: false,
        extractToFtpSuccess: true,
      };
    case EXTRACT_TO_FTP_FAILURE:
      const { extractError } = action.payload;

      return {
        ...state,
        loading: false,
        error: extractError,
      };
    case AGENCY_STATISTICS_SEARCH_REQUEST:
      return {
        ...state,
        loading: true,
        error: undefined,
      };
    case AGENCY_STATISTICS_SEARCH_SUCCESS:
      const { agencyStatisticsSearchResults } = action.payload;
      return {
        ...state,
        loading: false,
        agencyStatisticsSearchResults: agencyStatisticsSearchResults,
      };
    case AGENCY_STATISTICS_SEARCH_SUCCESS_SUB_AUDIT:
      const { agencyStatisticsSearchResultsSubAudit } = action.payload;
      return {
        ...state,
        loading: false,
        agencyStatisticsSearchResultsSubAudit: agencyStatisticsSearchResultsSubAudit,
      };
    case AGENCY_STATISTICS_SEARCH_FAILURE:
      const { agencyStatisticsError } = action.payload;
      return {
        ...state,
        loading: false,
        error: agencyStatisticsError,
      };
    case CLEAR_REPORTS_ERROR:
      return {
        ...state,
        error: undefined,
        extractToFtpSuccess: false,
      };
    case GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_REQUEST:
      return {
        ...state,
        error: undefined,
        getFullExtractCachedError: undefined,
      };
    case GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_SUCCESS:
      const { fullExtractCachedVersion } = action.payload;
      return {
        ...state,
        fullExtractCachedVersion: fullExtractCachedVersion,
        getFullExtractCachedError: undefined,
      };
    case GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_FAILURE:
      const { getFullExtractCachedError } = action.payload;
      return {
        ...state,
        getFullExtractCachedError: getFullExtractCachedError,
      };
    case GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_REQUEST:
      return {
        ...state,
        error: undefined,
        getCreatorExtractCachedError: undefined,
      };
    case GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_SUCCESS:
      const { creatorExtractCachedVersion } = action.payload;
      return {
        ...state,
        creatorExtractCachedVersion: creatorExtractCachedVersion,
        getCreatorExtractCachedError: undefined,
      };
      case GET_DATE_OF_CACHED_CREATOR_EXTRACT_REPORT_FAILURE:
      const { getCreatorExtractCachedError } = action.payload;
      return {
        ...state,
        getCreatorExtractCachedError: getCreatorExtractCachedError,
      };
    case GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_REQUEST:
      return {
        ...state,
        error: undefined,
        getFullExtractCachedError: undefined,
      };
    case GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_SUCCESS:
      const { agencyWorkListCachedVersion } = action.payload;
      return {
        ...state,
        agencyWorkListCachedVersion: agencyWorkListCachedVersion,
        getFullExtractCachedError: undefined,
      };
    case GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_FAILURE:
      const { getAgencyWorkListCachedError } = action.payload;
      return {
        ...state,
        getAgencyWorkListCachedError: getAgencyWorkListCachedError,
      }; 
    case GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_REQUEST:
      return {
        ...state,
        error: undefined,
        getPotentialDuplicatesCachedError: undefined,
      };
    case GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_SUCCESS:
      const { potentialDuplicatesCachedVersion } = action.payload;
      return {
        ...state,
        potentialDuplicatesCachedVersion: potentialDuplicatesCachedVersion,
        getPotentialDuplicatesCachedError: undefined,
      };
    case GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_FAILURE:
      const { getPotentialDuplicatesCachedError } = action.payload;
      return {
        ...state,
        getPotentialDuplicatesCachedError: getPotentialDuplicatesCachedError,
      };
    default:
      return state;
  }
};
