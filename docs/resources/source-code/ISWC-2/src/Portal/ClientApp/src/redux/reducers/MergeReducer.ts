import { Reducer } from 'redux';
import {
  ADD_TO_MERGE_LIST,
  REMOVE_FROM_MERGE_LIST,
  MergeActionTypes,
  MERGE_REQUEST,
  MERGE_SUCCESS,
  MERGE_FAILURE,
  DEMERGE_REQUEST,
  DEMERGE_SUCCESS,
  DEMERGE_FAILURE,
  CLEAR_MERGE_ERROR,
} from '../actions/MergeActionTypes';
import { IIswcModel } from '../types/IswcTypes';

export interface IMergeReducerState {
  mergeList: IIswcModel[];
  mergedSuccessfully?: boolean;
  demergedSuccessfully?: boolean;
  processing?: boolean;
  error: any;
}

const initialState: IMergeReducerState = {
  mergeList: [],
  mergedSuccessfully: undefined,
  demergedSuccessfully: undefined,
  processing: false,
  error: undefined,
};

export const reducer: Reducer<IMergeReducerState> = (
  state = initialState,
  action: MergeActionTypes,
): IMergeReducerState => {
  if (state === undefined) {
    return initialState;
  }
  switch (action.type) {
    case ADD_TO_MERGE_LIST:
      const { iswc } = action.payload;
      return {
        ...state,
        mergeList: [...state.mergeList, iswc],
      };
    case REMOVE_FROM_MERGE_LIST:
      const { iswcToRemove } = action.payload;
      let i = iswcToRemove;
      if (typeof i == 'string') {
        i = state.mergeList.findIndex((x) => x.iswc === iswcToRemove);
      }
      return {
        ...state,
        mergeList: [...state.mergeList.slice(0, i), ...state.mergeList.slice(i + 1)],
      };
    case MERGE_REQUEST:
      return {
        ...state,
        processing: true,
        error: undefined,
        mergedSuccessfully: undefined,
      };
    case MERGE_SUCCESS:
      return {
        ...state,
        processing: false,
        mergedSuccessfully: true,
        error: undefined,
        mergeList: [],
      };
    case MERGE_FAILURE:
      const { error } = action.payload;
      return {
        ...state,
        processing: false,
        mergedSuccessfully: false,
        error,
      };
    case DEMERGE_REQUEST:
      return {
        ...state,
        processing: true,
        error: undefined,
      };
    case DEMERGE_SUCCESS:
      return {
        ...state,
        processing: false,
        demergedSuccessfully: true,
        error: undefined,
      };
    case DEMERGE_FAILURE:
      return {
        ...state,
        processing: false,
        demergedSuccessfully: false,
        error: action.payload.error,
      };
    case CLEAR_MERGE_ERROR:
      return {
        ...state,
        error: undefined,
      };
    default:
      return state;
  }
};
