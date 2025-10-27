import * as actions from '../../../redux/actions/SubmissionHistoryActions';
import {
  GET_SUBMISSION_HISTORY_REQUEST,
  GET_SUBMISSION_HISTORY_SUCCESS,
  GET_SUBMISSION_HISTORY_FAILURE,
} from '../../../redux/actions/SubmissionHistoryActionTypes';
import { IAuditHistoryResult } from '../../../redux/types/IswcTypes';

describe('SubmissionHistory Actions', () => {
  it('getSubmissionHistoryRequest calls GET_SUBMISSION_HISTORY_REQUEST', () => {
    const expectedAction = {
      type: GET_SUBMISSION_HISTORY_REQUEST,
    };
    expect(actions.getSubmissionHistoryRequest()).toEqual(expectedAction);
  });

  it('getSubmissionHistorySuccess calls GET_SUBMISSION_HISTORY_SUCCESS', () => {
    const submissionHistory = {} as IAuditHistoryResult[];
    const expectedAction = {
      type: GET_SUBMISSION_HISTORY_SUCCESS,
      payload: {
        submissionHistory,
      },
    };
    expect(actions.getSubmissionHistorySuccess(submissionHistory)).toEqual(expectedAction);
  });

  it('getSubmissionHistoryFailure calls GET_SUBMISSION_HISTORY_FAILURE', () => {
    const error = 'error';
    const expectedAction = {
      type: GET_SUBMISSION_HISTORY_FAILURE,
      payload: {
        error,
      },
    };
    expect(actions.getSubmissionHistoryFailure(error)).toEqual(expectedAction);
  });
});
