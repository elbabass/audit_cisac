import {
  SUBMISSION_AUDIT_SEARCH_REQUEST,
  SUBMISSION_AUDIT_SEARCH_SUCCESS,
  SUBMISSION_AUDIT_SEARCH_FAILURE,
  EXTRACT_TO_FTP_REQUEST,
  EXTRACT_TO_FTP_SUCCESS,
  EXTRACT_TO_FTP_FAILURE,
  AGENCY_STATISTICS_SEARCH_REQUEST,
  AGENCY_STATISTICS_SEARCH_SUCCESS,
  AGENCY_STATISTICS_SEARCH_FAILURE,
  CLEAR_REPORTS_ERROR,
  GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_REQUEST,
  GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_SUCCESS,
  GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_FAILURE,
  GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_REQUEST,
  GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_SUCCESS,
  GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_FAILURE,
  GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_REQUEST,
  GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_SUCCESS,
  GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_FAILURE
} from '../../../redux/actions/ReportsActionTypes';
import * as actions from '../../../redux/actions/ReportsActions';
import {
  ISubmissionAuditReportResult,
  IAgencyStatisticsReportResult,
} from '../../../redux/types/ReportTypes';

describe('Reports Actions', () => {
  it('submissionAuditSearchRequest calls SUBMISSION_AUDIT_SEARCH_REQUEST', () => {
    const expectedAction = {
      type: SUBMISSION_AUDIT_SEARCH_REQUEST,
    };
    expect(actions.submissionAuditSearchRequest()).toEqual(expectedAction);
  });

  it('submissionAuditSearchSuccess calls SUBMISSION_AUDIT_SEARCH_SUCCESS', () => {
    const submissionAuditSearchResults: ISubmissionAuditReportResult[] = [];
    const expectedAction = {
      type: SUBMISSION_AUDIT_SEARCH_SUCCESS,
      payload: {
        submissionAuditSearchResults,
      },
    };
    expect(actions.submissionAuditSearchSuccess(submissionAuditSearchResults)).toEqual(
      expectedAction,
    );
  });

  it('submissionAuditSearchFailure calls SUBMISSION_AUDIT_SEARCH_FAILURE', () => {
    const error: any = 'error';
    const expectedAction = {
      type: SUBMISSION_AUDIT_SEARCH_FAILURE,
      payload: {
        error: error,
      },
    };
    expect(actions.submissionAuditSearchFailure(error)).toEqual(expectedAction);
  });

  it('extractToFtpRequest calls EXTRACT_TO_FTP_REQUEST', () => {
    const expectedAction = {
      type: EXTRACT_TO_FTP_REQUEST,
    };
    expect(actions.extractToFtpRequest()).toEqual(expectedAction);
  });

  it('extractToFtpSuccess calls EXTRACT_TO_FTP_SUCCESS', () => {
    const expectedAction = {
      type: EXTRACT_TO_FTP_SUCCESS,
    };
    expect(actions.extractToFtpSuccess()).toEqual(expectedAction);
  });

  it('extractToFtpFailure calls EXTRACT_TO_FTP_FAILURE', () => {
    const error: any = 'error';
    const expectedAction = {
      type: EXTRACT_TO_FTP_FAILURE,
      payload: {
        extractError: error,
      },
    };
    expect(actions.extractToFtpFailure(error)).toEqual(expectedAction);
  });

  it('agecnyStatisticsSearchRequest calls AGENCY_STATISTICS_SEARCH_REQUEST', () => {
    const expectedAction = {
      type: AGENCY_STATISTICS_SEARCH_REQUEST,
    };
    expect(actions.agecnyStatisticsSearchRequest()).toEqual(expectedAction);
  });

  it('agecnyStatisticsSearchSuccess calls AGENCY_STATISTICS_SEARCH_SUCCESS', () => {
    const agencyStatisticsSearchResults: IAgencyStatisticsReportResult[] = [];
    const expectedAction = {
      type: AGENCY_STATISTICS_SEARCH_SUCCESS,
      payload: {
        agencyStatisticsSearchResults,
      },
    };
    expect(actions.agecnyStatisticsSearchSuccess(agencyStatisticsSearchResults)).toEqual(
      expectedAction,
    );
  });

  it('agecnyStatisticsSearchFailure calls AGENCY_STATISTICS_SEARCH_FAILURE', () => {
    const error: any = 'error';
    const expectedAction = {
      type: AGENCY_STATISTICS_SEARCH_FAILURE,
      payload: {
        agencyStatisticsError: error,
      },
    };
    expect(actions.agecnyStatisticsSearchFailure(error)).toEqual(expectedAction);
  });

  it('clearReportsError calls CLEAR_REPORTS_ERROR', () => {
    const expectedAction = {
      type: CLEAR_REPORTS_ERROR,
    };
    expect(actions.clearReportsError()).toEqual(expectedAction);
  });

  it('getDateOfCachedIswcFullExtractReportRequest calls GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_REQUEST', () => {
    const expectedAction = {
      type: GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_REQUEST,
    };
    expect(actions.getDateOfCachedIswcFullExtractReportRequest()).toEqual(expectedAction);
  });

  it('getDateOfCachedIswcFullExtractReportSuccess calls GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_SUCCESS', () => {
    const fullExtractCachedVersion = 'version';
    const expectedAction = {
      type: GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_SUCCESS,
      payload: {
        fullExtractCachedVersion: fullExtractCachedVersion,
      },
    };
    expect(actions.getDateOfCachedIswcFullExtractReportSuccess(fullExtractCachedVersion)).toEqual(
      expectedAction,
    );
  });

  it('getDateOfCachedIswcFullExtractReportFailure calls GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_FAILURE', () => {
    const error = 'version';
    const expectedAction = {
      type: GET_DATE_OF_CACHED_FULL_EXTRACT_REPORT_FAILURE,
      payload: {
        getFullExtractCachedError: error,
      },
    };
    expect(actions.getDateOfCachedIswcFullExtractReportFailure(error)).toEqual(expectedAction);
  });

  it('getDateOfCachedPotentialDuplicatesReportRequest calls GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_REQUEST', () => {
    const expectedAction = {
      type: GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_REQUEST,
    };
    expect(actions.getDateOfCachedPotentialDuplicatesReportRequest()).toEqual(expectedAction);
  });

  it('getDateOfCachedPotentialDuplicatesReportSuccess calls GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_SUCCESS', () => {
    const potentialDuplicatesCachedVersion = 'version';
    const expectedAction = {
      type: GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_SUCCESS,
      payload: {
        potentialDuplicatesCachedVersion: potentialDuplicatesCachedVersion,
      },
    };
    expect(
      actions.getDateOfCachedPotentialDuplicatesReportSuccess(potentialDuplicatesCachedVersion),
    ).toEqual(expectedAction);
  });

  it('getDateOfCachedPotentialDuplicatesReportFailure calls GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_FAILURE', () => {
    const error = 'version';
    const expectedAction = {
      type: GET_DATE_OF_CACHED_POTENTIAL_DUPLICATES_REPORT_FAILURE,
      payload: {
        getPotentialDuplicatesCachedError: error,
      },
    };
    expect(actions.getDateOfCachedPotentialDuplicatesReportFailure(error)).toEqual(expectedAction);
  });

  it('getDateOfCachedAgencyWorkListReportRequest calls GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_REQUEST', () => {
    const expectedAction = {
      type: GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_REQUEST,
    };
    expect(actions.getDateOfCachedAgencyWorkListReportRequest()).toEqual(expectedAction);
  });

    it('getDateOfCachedAgencyWorkListReportSuccess calls GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_SUCCESS', () => {
    const agencyWorkListCachedVersion = 'version';
    const expectedAction = {
      type: GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_SUCCESS,
      payload: {
        agencyWorkListCachedVersion: agencyWorkListCachedVersion,
      },
    };
    expect(actions.getDateOfCachedAgencyWorkListReportSuccess(agencyWorkListCachedVersion)).toEqual(
      expectedAction,
    );
  });

    it('getDateOfCachedAgencyWorkListReportFailure calls GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_FAILURE', () => {
    const error = 'version';
    const expectedAction = {
      type: GET_DATE_OF_CACHED_AGENCY_WORK_LIST_REPORT_FAILURE,
      payload: {
        getAgencyWorkListCachedError: error,
      },
    };
    expect(actions.getDateOfCachedAgencyWorkListReportFailure(error)).toEqual(expectedAction);
  });
});
