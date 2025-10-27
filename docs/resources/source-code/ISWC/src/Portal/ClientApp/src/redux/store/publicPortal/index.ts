import * as SearchReducer from '../../reducers/SearchReducer';
import * as AppReducer from '../../reducers/AppReducer';

export interface ApplicationState {
  searchReducer: SearchReducer.ISearchReducerState;
  appReducer: AppReducer.IAppReducerState;
}

export const reducers = {
  searchReducer: SearchReducer.reducer,
  appReducer: AppReducer.reducer,
};

export interface AppThunkAction<TAction> {
  (dispatch: (action: TAction) => void, getState: () => ApplicationState): void;
}
