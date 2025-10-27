import React from 'react';
import { IPrivateRouteProps } from './PrivateRouteTypes';
import { Route } from 'react-router-dom';
import RedirectRoute from './RedirectRoute';
import Cookies from 'universal-cookie';
import { PUBLIC_MODE, USER_PROFILE_PATH } from '../../consts';
import { UserLoggedIn } from '../../shared/helperMethods';
import { useHistory } from 'react-router-dom';

const PrivateRoute: React.FunctionComponent<IPrivateRouteProps> = ({ path, component, access }) => {
  const cookies = new Cookies();
  const history = useHistory();

  if (process.env.REACT_APP_MODE !== PUBLIC_MODE && UserLoggedIn()) {
    if (access) {
      return <Route exact path={path} component={component} />;
    } else {
      history.push(USER_PROFILE_PATH, { roleAccessMessage: true });
      return <div />;
    }
  } else if (process.env.REACT_APP_MODE === PUBLIC_MODE && cookies.get('recaptchaToken')) {
    return <Route exact path={path} component={component} />;
  }

  const search = window.location.search;
  const params = new URLSearchParams(search);
  const iswc = params.get('iswc');

  return <RedirectRoute redirectUrl={path} iswc={iswc} />;
};

export default PrivateRoute;
