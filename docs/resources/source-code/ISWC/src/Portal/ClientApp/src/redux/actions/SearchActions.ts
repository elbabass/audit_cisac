import {
  SEARCH_REQUEST,
  SEARCH_SUCCESS,
  SEARCH_FAILURE,
  SearchActionTypes,
  BATCH_SEARCH_SUCCESS,
  CLEAR_SEARCH,
  CLEAR_SEARCH_ERROR,
} from './SearchActionTypes';
import { IIswcModel } from '../types/IswcTypes';

export function searchRequest(): SearchActionTypes {
  return {
    type: SEARCH_REQUEST,
  };
}

export function searchSuccess(searchResults: IIswcModel[]): SearchActionTypes {
  return {
    type: SEARCH_SUCCESS,
    payload: {
      searchResults,
    },
  };
}

export function searchFailure(error: any): SearchActionTypes {
  return {
    type: SEARCH_FAILURE,
    payload: {
      error,
    },
  };
}

export function batchSearchSuccess(batchResults: IIswcModel[]): SearchActionTypes {
  return {
    type: BATCH_SEARCH_SUCCESS,
    payload: {
      batchResults,
    },
  };
}

export function clearSearch(): SearchActionTypes {
  return {
    type: CLEAR_SEARCH,
  };
}

export function clearSearchError(): SearchActionTypes {
  return {
    type: CLEAR_SEARCH_ERROR,
  };
}
