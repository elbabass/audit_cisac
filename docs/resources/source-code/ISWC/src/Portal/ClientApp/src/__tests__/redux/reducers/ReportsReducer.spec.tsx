import { reducer } from '../../../redux/reducers/ReportsReducer';
import {
  ReportsActionTypes,
  SUBMISSION_AUDIT_SEARCH_REQUEST,
  SUBMISSION_AUDIT_SEARCH_SUCCESS,
  SUBMISSION_AUDIT_SEARCH_FAILURE,
  EXTRACT_TO_FTP_REQUEST,
  EXTRACT_TO_FTP_SUCCESS,
  EXTRACT_TO_FTP_FAILURE,
  AGENCY_STATISTICS_SEARCH_REQUEST,
  AGENCY_STATISTICS_SEARCH_SUCCESS,
  AGENCY_STATISTICS_SEARCH_FAILURE,
  AGENCY_STATISTICS_SEARCH_SUCCESS_SUB_AUDIT,
  GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_REQUEST,
  GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_SUCCESS,
  GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_FAILURE,
  GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_REQUEST,
  GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_SUCCESS,
  GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_FAILURE,
  GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_REQUEST,
  GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_SUCCESS,
  GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_FAILURE,
} from '../../../redux/actions/ReportsActionTypes';
import {
  ISubmissionAuditReportResult,
  IAgencyStatisticsReportResult,
} from '../../../redux/types/ReportTypes';

describe('Reports Reducer', () => {
  it('should return the initial state', () => {
    expect(reducer(undefined, {} as ReportsActionTypes)).toEqual({
      loading: false,
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
    });
  });

  it('should handle SUBMISSION_AUDIT_SEARCH_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: SUBMISSION_AUDIT_SEARCH_REQUEST,
      }),
    ).toEqual({
      loading: true,
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
    });
  });

  it('should handle SUBMISSION_AUDIT_SEARCH_SUCCESS', () => {
    expect(
      reducer(undefined, {
        type: SUBMISSION_AUDIT_SEARCH_SUCCESS,
        payload: {
          submissionAuditSearchResults: [{} as ISubmissionAuditReportResult],
        },
      }),
    ).toEqual({
      loading: false,
      submissionAuditSearchResults: [{} as ISubmissionAuditReportResult],
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
    });
  });

  it('should handle SUBMISSION_AUDIT_SEARCH_FAILURE', () => {
    const error = 'error';
    expect(
      reducer(undefined, {
        type: SUBMISSION_AUDIT_SEARCH_FAILURE,
        payload: {
          error,
        },
      }),
    ).toEqual({
      loading: false,
      error,
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
    });
  });

  it('should handle EXTRACT_TO_FTP_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: EXTRACT_TO_FTP_REQUEST,
      }),
    ).toEqual({
      loading: true,
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
    });
  });

  it('should handle EXTRACT_TO_FTP_SUCCESS', () => {
    expect(
      reducer(undefined, {
        type: EXTRACT_TO_FTP_SUCCESS,
      }),
    ).toEqual({
      loading: false,
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: true,
    });
  });

  it('should handle EXTRACT_TO_FTP_FAILURE', () => {
    const error = 'error';
    expect(
      reducer(undefined, {
        type: EXTRACT_TO_FTP_FAILURE,
        payload: {
          extractError: error,
        },
      }),
    ).toEqual({
      loading: false,
      error,
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
    });
  });

  it('should handle AGENCY_STATISTICS_SEARCH_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: AGENCY_STATISTICS_SEARCH_REQUEST,
      }),
    ).toEqual({
      loading: true,
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
    });
  });

  it('should handle AGENCY_STATISTICS_SEARCH_SUCCESS', () => {
    expect(
      reducer(undefined, {
        type: AGENCY_STATISTICS_SEARCH_SUCCESS,
        payload: {
          agencyStatisticsSearchResults: [{} as IAgencyStatisticsReportResult],
        },
      }),
    ).toEqual({
      loading: false,
      agencyStatisticsSearchResults: [{} as IAgencyStatisticsReportResult],
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
    });
  });

  it('should handle AGENCY_STATISTICS_SEARCH_SUCCESS_SUB_AUDIT', () => {
    expect(
      reducer(undefined, {
        type: AGENCY_STATISTICS_SEARCH_SUCCESS_SUB_AUDIT,
        payload: {
          agencyStatisticsSearchResultsSubAudit: [{} as IAgencyStatisticsReportResult],
        },
      }),
    ).toEqual({
      loading: false,
      agencyStatisticsSearchResultsSubAudit: [{} as IAgencyStatisticsReportResult],
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
    });
  });

  it('should handle AGENCY_STATISTICS_SEARCH_FAILURE', () => {
    const error = 'error';
    expect(
      reducer(undefined, {
        type: AGENCY_STATISTICS_SEARCH_FAILURE,
        payload: {
          agencyStatisticsError: error,
        },
      }),
    ).toEqual({
      loading: false,
      error,
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
    });
  });

  it('should handle GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_REQUEST,
      }),
    ).toEqual({
      loading: false,
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
    });
  });

  it('should handle GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_SUCCESS', () => {
    const version = 'version';
    expect(
      reducer(undefined, {
        type: GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_SUCCESS,
        payload: {
          fullExtractCachedVersion: version,
        },
      }),
    ).toEqual({
      loading: false,
      fullExtractCachedVersion: version,
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
    });
  });

  it('should handle GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_FAILURE', () => {
    const error = 'error';
    expect(
      reducer(undefined, {
        type: GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_FAILURE,
        payload: {
          getFullExtractCachedError: error,
        },
      }),
    ).toEqual({
      loading: false,
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
      getFullExtractCachedError: error,
    });
  });

  it('should handle GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_REQUEST,
      }),
    ).toEqual({
      loading: false,
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
    });
  });

  it('should handle GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_SUCCESS', () => {
    const version = 'version';
    expect(
      reducer(undefined, {
        type: GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_SUCCESS,
        payload: {
          potentialDuplicatesCachedVersion: version,
        },
      }),
    ).toEqual({
      loading: false,
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: version,
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
    });
  });

  it('should handle GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_FAILURE', () => {
    const error = 'error';
    expect(
      reducer(undefined, {
        type: GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_FAILURE,
        payload: {
          getPotentialDuplicatesCachedError: error,
        },
      }),
    ).toEqual({
      loading: false,
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
      getPotentialDuplicatesCachedError: error,
    });
  });

  it('should handle GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_REQUEST,
      }),
    ).toEqual({
      loading: false,
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
    });
  });

  it('should handle GET_DATE_OF_CACHED_AGENCY_WORK_LISTS_REPORT_SUCCESS', () => {
    const version = 'version';
    expect(
      reducer(undefined, {
        type: GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_SUCCESS,
        payload: {
          agencyWorkListCachedVersion: version,
        },
      }),
    ).toEqual({
      loading: false,
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: version,
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
    });
  });

  it('should handle GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_FAILURE', () => {
    const error = 'error';
    expect(
      reducer(undefined, {
        type: GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_FAILURE,
        payload: {
          getAgencyWorkListCachedError: error,
        },
      }),
    ).toEqual({
      loading: false,
      fullExtractCachedVersion: '',
      agencyWorkListCachedVersion: '',
      potentialDuplicatesCachedVersion: '',
      creatorExtractCachedVersion: '',
      extractToFtpSuccess: false,
      getAgencyWorkListCachedError: error,
    });
  });
});
