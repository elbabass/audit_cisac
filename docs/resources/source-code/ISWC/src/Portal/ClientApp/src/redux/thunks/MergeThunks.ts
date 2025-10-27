import { Action, Dispatch } from 'redux';
import { ThunkAction } from 'redux-thunk';
import { ApplicationState } from '../store/portal';
import {
  mergeRequest,
  mergeSuccess,
  mergeFailure,
  demergeRequest,
  demergeSuccess,
  demergeFailure,
} from '../actions/MergeActions';
import { mergeIswcs, demergeIswc } from '../services/MergeService';
import { IMergeBody } from '../types/IswcTypes';
import { IDemergeIswc } from '../../routes/Search/Demerge/DemergeTypes';
import { turnOnMaintenanceMode } from '../actions/AppActions';

export const mergeIswcsThunk = (
  preferredIswc?: string,
  agency?: string,
  mergeBody?: IMergeBody,
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(mergeRequest());
    return mergeIswcs(preferredIswc, agency, mergeBody)
      .then(() => {
        dispatch(mergeSuccess());
      })
      .catch((err) => {
        if (err.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(mergeFailure(err));
      });
  };
};

export const demergeIswcThunk = (
  preferredIswc: string,
  iswcsToDemerge: IDemergeIswc[],
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(demergeRequest());
    return demergeIswc(preferredIswc, iswcsToDemerge)
      .then(() => {
        dispatch(demergeSuccess());
      })
      .catch((err) => {
        if (err.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(demergeFailure(err));
      });
  };
};
