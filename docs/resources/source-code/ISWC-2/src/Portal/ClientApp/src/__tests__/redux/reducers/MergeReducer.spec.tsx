import { reducer } from '../../../redux/reducers/MergeReducer';
import {
  MergeActionTypes,
  ADD_TO_MERGE_LIST,
  REMOVE_FROM_MERGE_LIST,
  MERGE_REQUEST,
  MERGE_SUCCESS,
  MERGE_FAILURE,
  DEMERGE_REQUEST,
  DEMERGE_SUCCESS,
  DEMERGE_FAILURE,
} from '../../../redux/actions/MergeActionTypes';
import { IIswcModel } from '../../../redux/types/IswcTypes';

describe('Merge Reducer', () => {
  it('should return the initial state', () => {
    expect(reducer(undefined, {} as MergeActionTypes)).toEqual({
      mergeList: [],
      mergedSuccessfully: undefined,
      demergedSuccessfully: undefined,
      processing: false,
      error: undefined,
    });
  });

  it('should handle ADD_TO_MERGE_LIST', () => {
    expect(
      reducer(undefined, {
        type: ADD_TO_MERGE_LIST,
        payload: {
          iswc: {} as IIswcModel,
        },
      }),
    ).toEqual({
      mergeList: [{} as IIswcModel],
      mergedSuccessfully: undefined,
      demergedSuccessfully: undefined,
      processing: false,
      error: undefined,
    });
  });

  it('should handle REMOVE_FROM_MERGE_LIST', () => {
    expect(
      reducer(undefined, {
        type: REMOVE_FROM_MERGE_LIST,
        payload: {
          iswcToRemove: 1,
        },
      }),
    ).toEqual({
      mergeList: [],
      mergedSuccessfully: undefined,
      demergedSuccessfully: undefined,
      processing: false,
      error: undefined,
    });
  });

  it('should handle MERGE_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: MERGE_REQUEST,
      }),
    ).toEqual({
      mergeList: [],
      mergedSuccessfully: undefined,
      demergedSuccessfully: undefined,
      processing: true,
      error: undefined,
    });
  });

  it('should handle MERGE_SUCCESS', () => {
    expect(
      reducer(undefined, {
        type: MERGE_SUCCESS,
      }),
    ).toEqual({
      mergeList: [],
      mergedSuccessfully: true,
      demergedSuccessfully: undefined,
      processing: false,
      error: undefined,
    });
  });

  it('should handle MERGE_FAILURE', () => {
    const error = 'error';
    expect(
      reducer(undefined, {
        type: MERGE_FAILURE,
        payload: {
          error,
        },
      }),
    ).toEqual({
      mergeList: [],
      mergedSuccessfully: false,
      demergedSuccessfully: undefined,
      processing: false,
      error,
    });
  });

  it('should handle DEMERGE_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: DEMERGE_REQUEST,
      }),
    ).toEqual({
      mergeList: [],
      mergedSuccessfully: undefined,
      demergedSuccessfully: undefined,
      processing: true,
      error: undefined,
    });
  });

  it('should handle DEMERGE_SUCCESS', () => {
    expect(
      reducer(undefined, {
        type: DEMERGE_SUCCESS,
      }),
    ).toEqual({
      mergeList: [],
      mergedSuccessfully: undefined,
      demergedSuccessfully: true,
      processing: false,
      error: undefined,
    });
  });

  it('should handle DEMERGE_FAILURE', () => {
    const error = 'error';
    expect(
      reducer(undefined, {
        type: DEMERGE_FAILURE,
        payload: {
          error,
        },
      }),
    ).toEqual({
      mergeList: [],
      mergedSuccessfully: undefined,
      demergedSuccessfully: false,
      processing: false,
      error,
    });
  });
});
