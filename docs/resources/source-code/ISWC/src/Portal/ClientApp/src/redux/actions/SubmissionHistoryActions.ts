import {
  SubmissionHistoryActionTypes,
  CLEAR_SUBMISSION_HISTORY_ERROR,
} from './SubmissionHistoryActionTypes';
import {
  GET_SUBMISSION_HISTORY_REQUEST,
  GET_SUBMISSION_HISTORY_SUCCESS,
  GET_SUBMISSION_HISTORY_FAILURE,
} from './SubmissionHistoryActionTypes';
import { IAuditHistoryResult } from '../types/IswcTypes';

export function getSubmissionHistoryRequest(): SubmissionHistoryActionTypes {
  return {
    type: GET_SUBMISSION_HISTORY_REQUEST,
  };
}

export function getSubmissionHistorySuccess(
  submissionHistory: IAuditHistoryResult[],
): SubmissionHistoryActionTypes {
  return {
    type: GET_SUBMISSION_HISTORY_SUCCESS,
    payload: {
      submissionHistory,
    },
  };
}

export function getSubmissionHistoryFailure(error: any): SubmissionHistoryActionTypes {
  return {
    type: GET_SUBMISSION_HISTORY_FAILURE,
    payload: { error },
  };
}

export function clearSubmissionHistoryError(): SubmissionHistoryActionTypes {
  return {
    type: CLEAR_SUBMISSION_HISTORY_ERROR,
  };
}
