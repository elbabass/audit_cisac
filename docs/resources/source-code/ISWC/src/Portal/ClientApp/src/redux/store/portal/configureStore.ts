import {
  applyMiddleware,
  combineReducers,
  compose,
  createStore,
  Middleware,
  Dispatch,
} from 'redux';
import thunk from 'redux-thunk';
import { createLogger } from 'redux-logger';
import { connectRouter, routerMiddleware } from 'connected-react-router';
import * as storage from 'redux-storage';
import filter from 'redux-storage-decorator-filter';
import createEngine from 'redux-storage-engine-localstorage';
import { stateFilters, eventFilters } from './filters';
import { History } from 'history';
import Cookies from 'universal-cookie';
import { ApplicationState, reducers } from '.';
import { logOutUser } from '../../../App/Portal/LogOut';
import {
  TURN_ON_MAINTENANCE_MODE,
  TURN_OFF_MAINTENANCE_MODE,
  REDUX_STORAGE_LOAD,
  SET_USER_EMAIL,
  SET_USER_AGENCY,
  SET_ASSIGNED_ROLES,
} from '../../actions/AppActionTypes';

export default function configureStore(history: History, initialState?: ApplicationState) {
  const engine = filter(createEngine('engine-key'), stateFilters);
  const storageMiddleware = storage.createMiddleware(engine, [], eventFilters);

  const authMiddleware: Middleware<{}, ApplicationState> = (store) => (next: Dispatch) => (
    action,
  ) => {
    if (
      action.payload?.isFirstRendering ||
      action.type === REDUX_STORAGE_LOAD ||
      action.type === TURN_ON_MAINTENANCE_MODE ||
      action.type === TURN_OFF_MAINTENANCE_MODE ||
      action.type === SET_USER_EMAIL ||
      action.type === SET_USER_AGENCY ||
      action.type === SET_ASSIGNED_ROLES
    ) {
      return next(action);
    } else {
      const cookies = new Cookies();
      const token = cookies.get('authToken');

      if (token) {
        return next(action);
      } else {
        return logOutUser();
      }
    }
  };

  const middleware = [thunk, routerMiddleware(history), storageMiddleware, authMiddleware];

  if (process.env.NODE_ENV === 'development') {
    middleware.push(createLogger());
  }

  const rootReducer = combineReducers({
    ...reducers,
    router: connectRouter(history),
  });

  const store = createStore(
    storage.reducer(rootReducer),
    initialState,
    compose(applyMiddleware(...middleware)),
  );

  const loader = storage.createLoader(engine);
  loader(store);

  return store;
}
