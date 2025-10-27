import * as actions from '../../../redux/actions/SearchActions';
import {
  SEARCH_REQUEST,
  SEARCH_SUCCESS,
  SEARCH_FAILURE,
  BATCH_SEARCH_SUCCESS,
} from '../../../redux/actions/SearchActionTypes';
import { IIswcModel } from '../../../redux/types/IswcTypes';

describe('Search Actions', () => {
  it('searchRequest calls SEARCH_REQUEST', () => {
    const expectedAction = {
      type: SEARCH_REQUEST,
    };
    expect(actions.searchRequest()).toEqual(expectedAction);
  });

  it('searchSuccess calls SEARCH_SUCCESS', () => {
    const searchResults = {} as IIswcModel[];
    const expectedAction = {
      type: SEARCH_SUCCESS,
      payload: {
        searchResults,
      },
    };
    expect(actions.searchSuccess(searchResults)).toEqual(expectedAction);
  });

  it('searchFailure calls SEARCH_FAILURE', () => {
    const error = 'error';
    const expectedAction = {
      type: SEARCH_FAILURE,
      payload: {
        error,
      },
    };
    expect(actions.searchFailure(error)).toEqual(expectedAction);
  });

  it('batchSearchSuccess calls BATCH_SEARCH_SUCCESS', () => {
    const batchResults = {} as IIswcModel[];
    const expectedAction = {
      type: BATCH_SEARCH_SUCCESS,
      payload: {
        batchResults,
      },
    };
    expect(actions.batchSearchSuccess(batchResults)).toEqual(expectedAction);
  });
});
