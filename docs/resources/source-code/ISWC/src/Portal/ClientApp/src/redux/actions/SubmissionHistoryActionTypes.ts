import { IAuditHistoryResult } from '../types/IswcTypes';

export const GET_SUBMISSION_HISTORY_REQUEST = 'GET_SUBMISSION_HISTORY_REQUEST';
export const GET_SUBMISSION_HISTORY_SUCCESS = 'GET_SUBMISSION_HISTORY_SUCCESS';
export const GET_SUBMISSION_HISTORY_FAILURE = 'GET_SUBMISSION_HISTORY_FAILURE';
export const CLEAR_SUBMISSION_HISTORY_ERROR = 'CLEAR_SUBMISSION_HISTORY_ERROR';

export interface GetSubmissionHistoryRequest {
  type: typeof GET_SUBMISSION_HISTORY_REQUEST;
}

export interface GetSubmissionHistorySuccess {
  type: typeof GET_SUBMISSION_HISTORY_SUCCESS;
  payload: {
    submissionHistory: IAuditHistoryResult[];
  };
}

export interface GetSubmissionHistoryFailure {
  type: typeof GET_SUBMISSION_HISTORY_FAILURE;
  payload: { error: any };
}

export interface ClearSubmissionHistoryError {
  type: typeof CLEAR_SUBMISSION_HISTORY_ERROR;
}

export type SubmissionHistoryActionTypes =
  | GetSubmissionHistoryRequest
  | GetSubmissionHistorySuccess
  | GetSubmissionHistoryFailure
  | ClearSubmissionHistoryError;
