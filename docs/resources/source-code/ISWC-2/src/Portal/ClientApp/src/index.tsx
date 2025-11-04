import 'react-app-polyfill/ie11';
import 'react-app-polyfill/stable';
import 'bootstrap/dist/css/bootstrap.css';

import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import { ConnectedRouter } from 'connected-react-router';
import { createBrowserHistory } from 'history';
import configurePortalStore from './redux/store/portal/configureStore';
import configurePublicPortalStore from './redux/store/publicPortal/configureStore';
import register from './registerServiceWorker';
import axios from 'axios';
import { _findGetParameter } from './shared/helperMethods';
import PortalAppContainer from './App/Portal/AppContainer';
import PublicPortalAppContainer from './App/PublicPortal/AppContainer';
import { PUBLIC_MODE } from './consts';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href') as string;
const history = createBrowserHistory({ basename: baseUrl });
const store =
  process.env.REACT_APP_MODE === PUBLIC_MODE
    ? configurePublicPortalStore(history)
    : configurePortalStore(history);
const token = _findGetParameter('token_id');

if (token) {
  axios.defaults.headers.common['Authorization'] = 'Bearer ' + token;
}

if (process.env.NODE_ENV === 'development' && process.env.REACT_APP_MODE === PUBLIC_MODE) {
     axios.defaults.headers.common['Public-Portal'] = 'true';
 }

axios.defaults.headers.common['Request-Source'] = 'PORTAL';

history.listen((location, action) => {
  if (action === 'PUSH') {
    window.scrollTo(0, 0);
  }
});

register();

ReactDOM.render(
  <Provider store={store}>
    <ConnectedRouter history={history}>
      {process.env.REACT_APP_MODE === PUBLIC_MODE ? (
        <PublicPortalAppContainer history={history} />
      ) : (
        <PortalAppContainer history={history} />
      )}
    </ConnectedRouter>
  </Provider>,
  document.getElementById('root'),
);
