import { Reducer } from 'redux';
import {
  SubmissionHistoryActionTypes,
  GET_SUBMISSION_HISTORY_REQUEST,
  GET_SUBMISSION_HISTORY_FAILURE,
  GET_SUBMISSION_HISTORY_SUCCESS,
  CLEAR_SUBMISSION_HISTORY_ERROR,
} from '../actions/SubmissionHistoryActionTypes';
import { IAuditHistoryResult } from '../types/IswcTypes';

export interface ISubmissionHistoryReducerState {
  loading: boolean;
  submissionHistory?: IAuditHistoryResult[];
  error?: any;
}

const initialState: ISubmissionHistoryReducerState = {
  loading: false,
  submissionHistory: undefined,
};

export const reducer: Reducer<ISubmissionHistoryReducerState> = (
  state = initialState,
  action: SubmissionHistoryActionTypes,
): ISubmissionHistoryReducerState => {
  if (state === undefined) {
    return initialState;
  }

  switch (action.type) {
    case GET_SUBMISSION_HISTORY_REQUEST:
      return {
        ...state,
        submissionHistory: undefined,
        error: undefined,
        loading: true,
      };
    case GET_SUBMISSION_HISTORY_SUCCESS:
      const { submissionHistory } = action.payload;
      return {
        ...state,
        loading: false,
        submissionHistory,
      };
    case GET_SUBMISSION_HISTORY_FAILURE:
      const { error } = action.payload;
      return {
        ...state,
        loading: false,
        error,
      };
    case CLEAR_SUBMISSION_HISTORY_ERROR:
      return {
        ...state,
        error: undefined,
      };
    default:
      return state;
  }
};
