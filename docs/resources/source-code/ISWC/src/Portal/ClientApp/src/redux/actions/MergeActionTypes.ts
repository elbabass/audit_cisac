import { IIswcModel } from '../types/IswcTypes';

export const ADD_TO_MERGE_LIST = 'ADD_TO_MERGE_LIST';
export const REMOVE_FROM_MERGE_LIST = 'REMOVE_FROM_MERGE_LIST';
export const MERGE_REQUEST = 'MERGE_REQUEST';
export const MERGE_SUCCESS = 'MERGE_SUCCESS';
export const MERGE_FAILURE = 'MERGE_FAILURE';
export const DEMERGE_REQUEST = 'DEMERGE_REQUEST';
export const DEMERGE_SUCCESS = 'DEMERGE_SUCCESS';
export const DEMERGE_FAILURE = 'DEMERGE_FAILURE';
export const CLEAR_MERGE_ERROR = 'CLEAR_MERGE_ERROR';

export interface AddToMergeList {
  type: typeof ADD_TO_MERGE_LIST;
  payload: {
    iswc: IIswcModel;
  };
}

export interface RemoveFromMergeList {
  type: typeof REMOVE_FROM_MERGE_LIST;
  payload: {
    iswcToRemove: number | string;
  };
}

export interface MergeRequest {
  type: typeof MERGE_REQUEST;
}

export interface MergeSuccess {
  type: typeof MERGE_SUCCESS;
}

export interface MergeFailure {
  type: typeof MERGE_FAILURE;
  payload: {
    error: string;
  };
}

export interface DemergeRequest {
  type: typeof DEMERGE_REQUEST;
}

export interface DemergeSuccess {
  type: typeof DEMERGE_SUCCESS;
}

export interface DemergeFailure {
  type: typeof DEMERGE_FAILURE;
  payload: {
    error: any;
  };
}

export interface ClearMergeError {
  type: typeof CLEAR_MERGE_ERROR;
}

export type MergeActionTypes =
  | AddToMergeList
  | RemoveFromMergeList
  | MergeRequest
  | MergeSuccess
  | MergeFailure
  | DemergeRequest
  | DemergeSuccess
  | DemergeFailure
  | ClearMergeError;
