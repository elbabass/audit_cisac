import React from 'react';
import { config } from '../../configuration/Configuration';
import { SEARCH_DEV_PROD_PATH, SEARCH_PATH, PUBLIC_MODE } from '../../consts';

interface IRedirectRouteProps {
  redirectUrl: string;
  iswc: string | null;
}

export default class RedirectRoute extends React.Component<IRedirectRouteProps> {
  componentDidMount = () => {
    const { redirectUrl, iswc } = this.props;
    let newRedirectUrl;
    if (redirectUrl === SEARCH_DEV_PROD_PATH) {
      newRedirectUrl = SEARCH_PATH;
    } else {
      newRedirectUrl = redirectUrl;
    }

    if (iswc !== null) {
      newRedirectUrl = `${newRedirectUrl}?iswc=${iswc}`;
    }

    if (process.env.REACT_APP_MODE === PUBLIC_MODE) {
      window.location.href = '/';
    } else {
      window.location.href = `${config().loginRedirectUri}?redirecturl=${newRedirectUrl}`;
    }
  };

  render() {
    return <div />;
  }
}
