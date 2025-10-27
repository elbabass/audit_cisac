import { Action, Dispatch } from 'redux';
import { ThunkAction } from 'redux-thunk';
import { ApplicationState } from '../store/portal';
import {
  getSubmissionHistoryRequest,
  getSubmissionHistorySuccess,
  getSubmissionHistoryFailure,
} from '../actions/SubmissionHistoryActions';
import { getSubmissionHistory } from '../services/SubmissionHistoryService';
import { turnOnMaintenanceMode } from '../actions/AppActions';

export const getSubmissionHistoryThunk = (
  preferredIswc?: string,
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(getSubmissionHistoryRequest());
    return getSubmissionHistory(preferredIswc)
      .then((res) => {
        let result = res.reverse();
        dispatch(getSubmissionHistorySuccess(result));
      })
      .catch((err) => {
        if (err.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(getSubmissionHistoryFailure(err));
      });
  };
};
