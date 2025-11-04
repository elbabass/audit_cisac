import { Action, Dispatch } from 'redux';
import { ThunkAction } from 'redux-thunk';
import { ApplicationState } from '../store/portal';
import {
  searchRequest,
  searchSuccess,
  searchFailure,
  batchSearchSuccess,
} from '../actions/SearchActions';
import {
  searchByIswc,
  searchByWorkCode,
  searchByTitleAndContributors,
  searchByIswcBatch,
} from '../services/SearchService';
import { IBatchSearchIswc } from '../types/IswcTypes';
import { turnOnMaintenanceMode } from '../actions/AppActions';

export const searchByIswcThunk = (
  iswc?: string,
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(searchRequest());
    return searchByIswc(iswc)
      .then((res) => {
        dispatch(searchSuccess(res));
      })
      .catch((err: any) => {
        if (err.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(searchFailure(err));
      });
  };
};

export const searchByWorkCodeThunk = (
  agency?: string,
  workCode?: string,
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(searchRequest());
    return searchByWorkCode(agency, workCode)
      .then((res) => {
        dispatch(searchSuccess(res));
      })
      .catch((err: any) => {
        if (err.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(searchFailure(err));
      });
  };
};

export const searchByTitleAndContributorThunk = (
  title?: string,
  surnames?: string,
  nameNumbers?: string,
  baseNumbers?: string,
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(searchRequest());
    return searchByTitleAndContributors(title, surnames, nameNumbers, baseNumbers)
      .then((res) => {
        dispatch(searchSuccess(res));
      })
      .catch((err: any) => {
        if (err.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(searchFailure(err));
      });
  };
};

export const searchByIswcBatchThunk = (
  iswcs: IBatchSearchIswc[],
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(searchRequest());
    return searchByIswcBatch(iswcs)
      .then((res) => {
        dispatch(batchSearchSuccess(res));
      })
      .catch((err) => {
        if (err.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(searchFailure(err));
      });
  };
};
