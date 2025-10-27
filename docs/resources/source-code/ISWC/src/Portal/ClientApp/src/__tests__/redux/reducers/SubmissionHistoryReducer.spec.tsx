import { reducer } from '../../../redux/reducers/SubmissionHistoryReducer';
import { SubmissionActionTypes } from '../../../redux/actions/SubmissionActionTypes';
import {
  GET_SUBMISSION_HISTORY_REQUEST,
  GET_SUBMISSION_HISTORY_SUCCESS,
  GET_SUBMISSION_HISTORY_FAILURE,
} from '../../../redux/actions/SubmissionHistoryActionTypes';
import { IAuditHistoryResult } from '../../../redux/types/IswcTypes';

describe('SubmissionHistory Reducer', () => {
  it('should return the initial state', () => {
    expect(reducer(undefined, {} as SubmissionActionTypes)).toEqual({
      loading: false,
      submissionHistory: undefined,
    });
  });

  it('should handle GET_SUBMISSION_HISTORY_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: GET_SUBMISSION_HISTORY_REQUEST,
      }),
    ).toEqual({
      submissionHistory: undefined,
      error: undefined,
      loading: true,
    });
  });

  it('should handle GET_SUBMISSION_HISTORY_SUCCESS', () => {
    const submissionHistory = {} as IAuditHistoryResult;
    expect(
      reducer(undefined, {
        type: GET_SUBMISSION_HISTORY_SUCCESS,
        payload: {
          submissionHistory,
        },
      }),
    ).toEqual({
      submissionHistory,
      error: undefined,
      loading: false,
    });
  });

  it('should handle GET_SUBMISSION_HISTORY_FAILURE', () => {
    const error = 'error';
    expect(
      reducer(undefined, {
        type: GET_SUBMISSION_HISTORY_FAILURE,
        payload: {
          error,
        },
      }),
    ).toEqual({
      submissionHistory: undefined,
      error,
      loading: false,
    });
  });
});
