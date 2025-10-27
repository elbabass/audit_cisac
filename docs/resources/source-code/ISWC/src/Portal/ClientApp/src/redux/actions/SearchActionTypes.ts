import { IIswcModel } from '../types/IswcTypes';

export const SEARCH_REQUEST = 'SEARCH_REQUEST';
export const SEARCH_SUCCESS = 'SEARCH_SUCCESS';
export const SEARCH_FAILURE = 'SEARCH_FAILURE';
export const BATCH_SEARCH_SUCCESS = 'BATCH_SEARCH_SUCCESS';
export const CLEAR_SEARCH = 'CLEAR_SEARCH';
export const CLEAR_SEARCH_ERROR = 'CLEAR_SEARCH_ERROR';

export interface SearchRequest {
  type: typeof SEARCH_REQUEST;
}

export interface SearchSuccess {
  type: typeof SEARCH_SUCCESS;
  payload: {
    searchResults: IIswcModel[];
  };
}

export interface SearchFailure {
  type: typeof SEARCH_FAILURE;
  payload: {
    error: any;
  };
}

export interface BatchSearchSuccess {
  type: typeof BATCH_SEARCH_SUCCESS;
  payload: {
    batchResults: IIswcModel[];
  };
}

export interface ClearSearch {
  type: typeof CLEAR_SEARCH;
}

export interface ClearSearchError {
  type: typeof CLEAR_SEARCH_ERROR;
}

export type SearchActionTypes =
  | SearchRequest
  | SearchSuccess
  | SearchFailure
  | BatchSearchSuccess
  | ClearSearch
  | ClearSearchError;
