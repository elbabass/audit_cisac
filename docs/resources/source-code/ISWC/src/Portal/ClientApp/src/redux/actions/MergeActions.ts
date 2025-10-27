import {
  ADD_TO_MERGE_LIST,
  REMOVE_FROM_MERGE_LIST,
  MERGE_REQUEST,
  MERGE_SUCCESS,
  MERGE_FAILURE,
  MergeActionTypes,
  DEMERGE_REQUEST,
  DEMERGE_SUCCESS,
  DEMERGE_FAILURE,
  CLEAR_MERGE_ERROR,
} from './MergeActionTypes';
import { IIswcModel } from '../types/IswcTypes';

export function addToMergeList(iswcToAdd: IIswcModel): MergeActionTypes {
  return {
    type: ADD_TO_MERGE_LIST,
    payload: {
      iswc: iswcToAdd,
    },
  };
}

export function removeFromMergeList(iswcToRemove: number | string): MergeActionTypes {
  return {
    type: REMOVE_FROM_MERGE_LIST,
    payload: {
      iswcToRemove: iswcToRemove,
    },
  };
}

export function mergeRequest(): MergeActionTypes {
  return {
    type: MERGE_REQUEST,
  };
}

export function mergeSuccess(): MergeActionTypes {
  return {
    type: MERGE_SUCCESS,
  };
}

export function mergeFailure(error: string): MergeActionTypes {
  return {
    type: MERGE_FAILURE,
    payload: {
      error,
    },
  };
}

export function demergeRequest(): MergeActionTypes {
  return {
    type: DEMERGE_REQUEST,
  };
}

export function demergeSuccess(): MergeActionTypes {
  return {
    type: DEMERGE_SUCCESS,
  };
}

export function demergeFailure(error: any): MergeActionTypes {
  return {
    type: DEMERGE_FAILURE,
    payload: {
      error,
    },
  };
}

export function clearMergeError(): MergeActionTypes {
  return {
    type: CLEAR_MERGE_ERROR,
  };
}
