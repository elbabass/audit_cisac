import { Reducer } from 'redux';
import {
  SEARCH_REQUEST,
  SEARCH_SUCCESS,
  SEARCH_FAILURE,
  SearchActionTypes,
  BATCH_SEARCH_SUCCESS,
  CLEAR_SEARCH,
  CLEAR_SEARCH_ERROR,
} from '../actions/SearchActionTypes';
import { IIswcModel } from '../types/IswcTypes';

export interface ISearchReducerState {
  isSearching: boolean;
  searchResults?: IIswcModel[];
  error?: any;
  batchResults?: IIswcModel[];
}

const initialState: ISearchReducerState = {
  isSearching: false,
  searchResults: undefined,
};

export const reducer: Reducer<ISearchReducerState> = (
  state = initialState,
  action: SearchActionTypes,
): ISearchReducerState => {
  if (state === undefined) {
    return initialState;
  }

  switch (action.type) {
    case SEARCH_REQUEST:
      return {
        ...state,
        searchResults: undefined,
        error: undefined,
        isSearching: true,
      };
    case SEARCH_SUCCESS:
      const { searchResults } = action.payload;
      return {
        ...state,
        isSearching: false,
        searchResults,
      };
    case SEARCH_FAILURE:
      const { error } = action.payload;
      return {
        ...state,
        isSearching: false,
        error,
      };
    case BATCH_SEARCH_SUCCESS:
      const { batchResults } = action.payload;
      return {
        ...state,
        isSearching: false,
        batchResults,
      };
    case CLEAR_SEARCH:
      return {
        ...state,
        error: undefined,
        searchResults: undefined,
        batchResults: undefined,
      };
    case CLEAR_SEARCH_ERROR:
      return {
        ...state,
        error: undefined,
      };
    default:
      return state;
  }
};
