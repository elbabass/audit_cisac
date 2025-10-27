import React, { Component, createContext } from 'react';
import { Route, Switch } from 'react-router-dom';
import Layout from '../../components/Layout/Layout';
import Loader from '../../components/Loader/Loader';
import {
  ClientAppConfiguration,
  config,
  fetchPortalMessages,
  loadClientAppConfiguration,
} from '../../configuration/Configuration';
import { fetchStringsFromDb, getStrings, stringsObj } from '../../configuration/Localization';
import { PUBLIC_LANDING_PAGE, PUBLIC_SEARCH_PATH, PUBLIC_USER_GUIDE_PATH } from '../../consts';
import '../../global.scss';
import LandingPage from '../../routes/LandingPage/LandingPage';
import PrivateRoute from '../../routes/PrivateRoute/PrivateRoute';
import SearchContainer from '../../routes/Search/PublicPortalSearchContainer';
import { addAuthTokenToCookies, addApplicationInsights } from '../../shared/helperMethods';
import styles from '../App.module.scss';
import { getLookupData, checkApiStatus } from '../../redux/services/LookupService';
import Maintenance from '../../components/Maintenance/Maintenance';

export const LocalizationContext = createContext({ strings: [] });

export interface IAppProps {
  history?: any;
  maintenanceMode: boolean;
  turnOnMaintenanceMode: () => void;
  turnOffMaintenanceMode: () => void;
}

interface IAppState {
  strings: stringsObj;
  configuration: ClientAppConfiguration;
  loading: boolean;
  error: any;
  culture: string | null;
}

export default class App extends Component<IAppProps, IAppState> {
  polling: any;
  lookupDataLoaded: boolean;

  constructor(props: IAppProps) {
    super(props);
    this.state = {
      strings: getStrings(),
      configuration: config(),
      loading: false,
      error: null,
      culture: sessionStorage.getItem('culture'),
    };

    this.lookupDataLoaded = false;
  }

  componentDidMount() {
    this.configureApp();
  }

  componentWillUnmount() {
    clearInterval(this.polling);
  }

  componentDidUpdate(prevProps: IAppProps, prevState: IAppState) {
    const { culture } = this.state;
    const { maintenanceMode } = this.props;

    if ((prevState.culture == null && culture != null) || prevState.culture !== culture) {
      this.configureApp();
    }
    if (!prevProps.maintenanceMode && maintenanceMode) {
      this.polling = setInterval(() => this._pollingCheckApiStatus(), 60000);
    }
  }

  configureApp() {
    let culture = sessionStorage.getItem('culture') || undefined;
    this.setState(() => ({ loading: true }));

    fetchStringsFromDb(culture)
      .then(
        (res: stringsObj) => {},
        (error: any) => {
          this.setState({ error });
        },
      )
      .then(() =>
        loadClientAppConfiguration().then(
          () => {
            if (process.env.NODE_ENV === 'production') addApplicationInsights(this.props.history);
          },
          (error: any) => {
            this.setState({ error, loading: false });
          },
        ),
      )
      .then(() => this._getLookupData())
      .then(() => {
        if (culture !== undefined) this._fetchPortalMessages(culture);
      });
  }

  _fetchPortalMessages = (culture?: string) => {
    this.setState({ loading: true });
    fetchPortalMessages(culture).then(() => {
      this.setState({ loading: false });
    });
  };

  _getLookupData = () => {
    const { turnOnMaintenanceMode } = this.props;

    getLookupData().then(
      () => {
        this.setState({ loading: false });
        this.lookupDataLoaded = true;
      },
      (error: any) => {
        if (error === 503) {
          turnOnMaintenanceMode();
          this.polling = setInterval(() => this._pollingCheckApiStatus(), 60000);
        } else {
          this.setState({ error, loading: false });
        }

        this.lookupDataLoaded = false;
      },
    );
  };

  _pollingCheckApiStatus = () => {
    const { turnOffMaintenanceMode } = this.props;
    checkApiStatus().then(
      () => {
        turnOffMaintenanceMode();
        clearInterval(this.polling);
        if (!this.lookupDataLoaded) {
          this._getLookupData();
        }
      },
      (error: any) => {},
    );
  };

  setCulture = (culture: string) => {
    this.setState(() => ({ culture }));
  };

  render() {
    const { strings, loading, error, configuration } = this.state;
    const { maintenanceMode } = this.props;

    addAuthTokenToCookies();

    if (maintenanceMode) {
      return (
        <Layout>
          <Maintenance />
        </Layout>
      );
    }
    if (loading) {
      return (
        <div className={styles.loaderDiv}>
          <Loader />
        </div>
      );
    } else if (!loading && error === null && strings !== null && configuration !== null) {
      return (
        <Layout>
          <Switch>
            <Route exact path={'/'} render={() => <LandingPage setCulture={this.setCulture} />} />
            <Route
              exact
              path={PUBLIC_LANDING_PAGE}
              render={() => <LandingPage setCulture={this.setCulture} />}
            />
            <PrivateRoute path={PUBLIC_SEARCH_PATH} component={SearchContainer} />
            <PrivateRoute path={PUBLIC_USER_GUIDE_PATH} component={SearchContainer} />
          </Switch>
        </Layout>
      );
    } else if (error !== null) {
      return <h1>An Error has occurred.</h1>;
    } else return <div></div>;
  }
}
