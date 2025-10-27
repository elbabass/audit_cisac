import { reducer } from '../../../redux/reducers/SearchReducer';
import { SubmissionActionTypes } from '../../../redux/actions/SubmissionActionTypes';
import {
  SEARCH_REQUEST,
  SEARCH_SUCCESS,
  SEARCH_FAILURE,
  BATCH_SEARCH_SUCCESS,
} from '../../../redux/actions/SearchActionTypes';
import { IIswcModel } from '../../../redux/types/IswcTypes';

describe('Search Reducer', () => {
  it('should return the initial state', () => {
    expect(reducer(undefined, {} as SubmissionActionTypes)).toEqual({
      isSearching: false,
      searchResults: undefined,
    });
  });

  it('should handle SEARCH_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: SEARCH_REQUEST,
      }),
    ).toEqual({
      searchResults: undefined,
      error: undefined,
      isSearching: true,
    });
  });

  it('should handle SEARCH_SUCCESS', () => {
    const searchResults = {} as IIswcModel[];
    expect(
      reducer(undefined, {
        type: SEARCH_SUCCESS,
        payload: {
          searchResults,
        },
      }),
    ).toEqual({
      searchResults,
      error: undefined,
      isSearching: false,
    });
  });

  it('should handle SEARCH_FAILURE', () => {
    const error = 'error';
    expect(
      reducer(undefined, {
        type: SEARCH_FAILURE,
        payload: {
          error,
        },
      }),
    ).toEqual({
      searchResults: undefined,
      error,
      isSearching: false,
    });
  });

  it('should handle BATCH_SEARCH_SUCCESS', () => {
    const batchResults = {} as IIswcModel[];
    expect(
      reducer(undefined, {
        type: BATCH_SEARCH_SUCCESS,
        payload: {
          batchResults,
        },
      }),
    ).toEqual({
      searchResults: undefined,
      error: undefined,
      isSearching: false,
      batchResults,
    });
  });
});
