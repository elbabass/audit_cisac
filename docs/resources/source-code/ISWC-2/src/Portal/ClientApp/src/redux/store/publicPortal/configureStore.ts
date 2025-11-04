import { applyMiddleware, combineReducers, compose, createStore } from 'redux';
import thunk from 'redux-thunk';
import { createLogger } from 'redux-logger';
import { connectRouter, routerMiddleware } from 'connected-react-router';
import * as storage from 'redux-storage';
import filter from 'redux-storage-decorator-filter';
import createEngine from 'redux-storage-engine-localstorage';
import { stateFilters, eventFilters } from './filters';
import { History } from 'history';
import { ApplicationState, reducers } from './index';

export default function configureStore(history: History, initialState?: ApplicationState) {
  const engine = filter(createEngine('engine-key'), stateFilters);
  const storageMiddleware = storage.createMiddleware(engine, [], eventFilters);

  const middleware = [thunk, routerMiddleware(history), storageMiddleware];

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
