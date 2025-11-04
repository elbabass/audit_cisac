import { Action, Dispatch } from 'redux';
import { ThunkAction } from 'redux-thunk';
import { ApplicationState } from '../store/portal';
import {
  submissionAuditSearchRequest,
  submissionAuditSearchSuccess,
  submissionAuditSearchFailure,
  fileSubmissionAuditSearchRequest,
  fileSubmissionAuditSearchSuccess,
  fileSubmissionAuditSearchFailure,
  extractToFtpRequest,
  extractToFtpSuccess,
  extractToFtpFailure,
  agecnyStatisticsSearchFailure,
  agecnyStatisticsSearchRequest,
  agecnyStatisticsSearchSuccess,
  getDateOfCachedIswcFullExtractReportRequest,
  getDateOfCachedIswcFullExtractReportSuccess,
  getDateOfCachedIswcFullExtractReportFailure,
  agecnyStatisticsSearchSuccessSubAudit,
  getDateOfCachedPotentialDuplicatesReportRequest,
  getDateOfCachedPotentialDuplicatesReportSuccess,
  getDateOfCachedPotentialDuplicatesReportFailure,
  getDateOfCachedAgencyWorkListReportRequest,
  getDateOfCachedAgencyWorkListReportSuccess,
  getDateOfCachedAgencyWorkListReportFailure,
  getDateOfCachedIswcCreatorExtractReportRequest,
  getDateOfCachedIswcCreatorExtractReportSuccess,
  getDateOfCachedIswcCreatorExtractReportFailure
} from '../actions/ReportsActions';
import {
  reportsSearch,
  reportsAgencyStatisticsSearch,
  getDateOfCachedReport,
} from '../services/ReportsService';
import { turnOnMaintenanceMode } from '../actions/AppActions';
import {
  IReportsParamFields,
  IAgencyStatisticsParamFields,
} from '../../routes/Reports/ReportsTypes';
import { getLoggedInAgencyId } from '../../shared/helperMethods';

export const submissionAuditSearchThunk = (
  params: IReportsParamFields,
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(submissionAuditSearchRequest());
    return reportsSearch(params, 'Report/SearchSubmissionAudit')
      .then((res) => {
        dispatch(submissionAuditSearchSuccess(res.data));
      })
      .catch((error: any) => {
        if (error.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(submissionAuditSearchFailure(error));
      });
  };
};

export const extractToFtpThunk = (
  params: IReportsParamFields,
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(extractToFtpRequest());
    return reportsSearch(params, 'Report/ExtractFtp')
      .then((res) => dispatch(extractToFtpSuccess()))
      .catch((error: any) => {
        if (error.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(extractToFtpFailure(error));
      });
  };
};

export const fileSubmissionAuditSearchThunk = (
  params: IReportsParamFields,
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(fileSubmissionAuditSearchRequest());
    return reportsSearch(params, 'Report/SearchFileSubmissionAudit')
      .then((res) => {
        dispatch(fileSubmissionAuditSearchSuccess(res.data));
      })
      .catch((err: any) => {
        if (err.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(fileSubmissionAuditSearchFailure(err));
      });
  };
};

export const agencyStatisticsSearchThunk = (
  params: IAgencyStatisticsParamFields,
  subAuditSearch = false,
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(agecnyStatisticsSearchRequest());
    return reportsAgencyStatisticsSearch(params, 'Report/GetAgencyStatistics')
      .then((res) => {
        if (subAuditSearch) {
          dispatch(agecnyStatisticsSearchSuccessSubAudit(res.data));
        } else {
          dispatch(agecnyStatisticsSearchSuccess(res.data));
        }
      })
      .catch((err: any) => {
        if (err.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(agecnyStatisticsSearchFailure(err));
      });
  };
};

export const getDateOfCachedIswcFullExtractReportThunk = (): ThunkAction<
  void,
  ApplicationState,
  null,
  Action<string>
> => {
  return (dispatch: Dispatch) => {
    dispatch(getDateOfCachedIswcFullExtractReportRequest());
    return getDateOfCachedReport('IswcFullExtract/')
      .then((res) => {
        dispatch(getDateOfCachedIswcFullExtractReportSuccess(res.data));
      })
      .catch((err: any) => {
        if (err.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(getDateOfCachedIswcFullExtractReportFailure(err));
      });
  };
};

export const getDateOfCachedIswcCreatorExtractReportThunk = (): ThunkAction<
    void,
    ApplicationState,
    null,
    Action<string>
> => {
    return (dispatch: Dispatch) => {
        dispatch(getDateOfCachedIswcCreatorExtractReportRequest());
        return getDateOfCachedReport('IswcCreatorExtract/')
            .then((res) => {
                dispatch(getDateOfCachedIswcCreatorExtractReportSuccess(res.data));
            })
            .catch((err: any) => {
                if (err.response?.status === 503) {
                    dispatch(turnOnMaintenanceMode());
                }
                dispatch(getDateOfCachedIswcCreatorExtractReportFailure(err));
            });
    };
};

export const getDateOfCachedAgencyWorkListReportThunk = (): ThunkAction<
  void,
  ApplicationState,
  null,
  Action<string>
> => {
  return (dispatch: Dispatch) => {
    dispatch(getDateOfCachedAgencyWorkListReportRequest());
    return getDateOfCachedReport('AgencyWorkList/' + getLoggedInAgencyId())
      .then((res) => {
        dispatch(getDateOfCachedAgencyWorkListReportSuccess(res.data));
      })
      .catch((err: any) => {
        if (err.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(getDateOfCachedAgencyWorkListReportFailure(err));
      });
  };
};

export const getDateOfCachedPotentialDuplicatesReportThunk = (): ThunkAction<
  void,
  ApplicationState,
  null,
  Action<string>
> => {
  return (dispatch: Dispatch) => {
    dispatch(getDateOfCachedPotentialDuplicatesReportRequest());
    return getDateOfCachedReport('PotentialDuplicates/')
      .then((res) => {
        dispatch(getDateOfCachedPotentialDuplicatesReportSuccess(res.data));
      })
      .catch((err: any) => {
        if (err.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(getDateOfCachedPotentialDuplicatesReportFailure(err));
      });
  };
};
