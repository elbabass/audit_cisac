import {
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
import * as actions from '../../../redux/actions/MergeActions';

describe('Merge Actions', () => {
  it('addToMergeList calls ADD_TO_MERGE_LIST', () => {
    const iswcToAdd = {} as IIswcModel;
    const expectedAction = {
      type: ADD_TO_MERGE_LIST,
      payload: {
        iswc: iswcToAdd,
      },
    };
    expect(actions.addToMergeList(iswcToAdd)).toEqual(expectedAction);
  });

  it('removeFromMergeList calls REMOVE_FROM_MERGE_LIST', () => {
    const iswcToRemove = 1;
    const expectedAction = {
      type: REMOVE_FROM_MERGE_LIST,
      payload: {
        iswcToRemove: iswcToRemove,
      },
    };
    expect(actions.removeFromMergeList(iswcToRemove)).toEqual(expectedAction);
  });

  it('mergeRequest calls MERGE_REQUEST', () => {
    const expectedAction = {
      type: MERGE_REQUEST,
    };
    expect(actions.mergeRequest()).toEqual(expectedAction);
  });

  it('mergeSuccess calls MERGE_SUCCESS', () => {
    const expectedAction = {
      type: MERGE_SUCCESS,
    };
    expect(actions.mergeSuccess()).toEqual(expectedAction);
  });

  it('mergeFailure calls MERGE_FAILURE', () => {
    const error = 'error';
    const expectedAction = {
      type: MERGE_FAILURE,
      payload: {
        error,
      },
    };
    expect(actions.mergeFailure(error)).toEqual(expectedAction);
  });

  it('demergeRequest calls DEMERGE_REQUEST', () => {
    const expectedAction = {
      type: DEMERGE_REQUEST,
    };
    expect(actions.demergeRequest()).toEqual(expectedAction);
  });

  it('demergeSuccess calls DEMERGE_SUCCESS', () => {
    const expectedAction = {
      type: DEMERGE_SUCCESS,
    };
    expect(actions.demergeSuccess()).toEqual(expectedAction);
  });

  it('demergeFailure calls DEMERGE_FAILURE', () => {
    const error = 'error';
    const expectedAction = {
      type: DEMERGE_FAILURE,
      payload: {
        error,
      },
    };
    expect(actions.demergeFailure(error)).toEqual(expectedAction);
  });
});
