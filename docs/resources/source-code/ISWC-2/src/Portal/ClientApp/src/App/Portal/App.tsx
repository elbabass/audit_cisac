import React, { Component, createContext } from 'react';
import { Route, Switch } from 'react-router-dom';
import Layout from '../../components/Layout/Layout';
import Loader from '../../components/Loader/Loader';
import { loadClientAppConfiguration } from '../../configuration/Configuration';
import { fetchStringsFromDb } from '../../configuration/Localization';
import {
  DEMERGE_PATH,
  MERGE_PATH,
  SEARCH_DEV_PROD_PATH,
  SUBMISSION_HISTORY_PATH,
  SUBMISSION_PATH,
  WORKFLOWS_PATH,
  REPORTS_PATH,
  MANAGE_USER_ROLES_PATH,
  USER_PROFILE_PATH,
} from '../../consts';
import '../../global.scss';
import HomeContainer from '../../routes/Home/HomeContainer';
import PrivateRoute from '../../routes/PrivateRoute/PrivateRoute';
import DemergeContainer from '../../routes/Search/Demerge/DemergeContainer';
import MergeContainer from '../../routes/Search/Merge/MergeContainer';
import SearchContainer from '../../routes/Search/PortalSearchContainer';
import SubmissionHistoryContainer from '../../routes/Search/SubmissionHistory/SubmissionHistoryContainer';
import SubmissionContainer from '../../routes/Submission/SubmissionContainer';
import WorkflowsContainer from '../../routes/Workflows/WorkflowsContainer';
import {
  addAuthTokenToCookies,
  addApplicationInsights,
  getUserEmailFromToken,
  getUserAgencyFromToken,
  getAssignedRolesFromToken,
} from '../../shared/helperMethods';
import styles from '../App.module.scss';
import { getLookupData, checkApiStatus } from '../../redux/services/LookupService';
import Maintenance from '../../components/Maintenance/Maintenance';
import ReportsContainer from '../../routes/Reports/ReportsContainer';
import ManageUserRolesContainer from '../../routes/ManageUserRoles/ManageUserRolesContainer';
import UserProfileContainer from '../../routes/UserProfile/UserProfileContainer';
import { IAssignedRoles } from '../../redux/types/RoleTypes';

export const LocalizationContext = createContext({ strings: [] });

export interface IAppProps {
  history?: any;
  maintenanceMode: boolean;
  assignedRoles: IAssignedRoles;
  turnOnMaintenanceMode: () => void;
  turnOffMaintenanceMode: () => void;
  setUserEmail: (email: string) => void;
  setUserAgency: (agency: string) => void;
  setAssignedRoles: (assignedRoles: IAssignedRoles) => void;
}

interface IAppState {
  loading: boolean;
  error: any;
}

export default class App extends Component<IAppProps, IAppState> {
  polling: any;
  lookupDataLoaded: boolean;

  constructor(props: IAppProps) {
    super(props);
    this.state = {
      loading: true,
      error: null,
    };

    this.lookupDataLoaded = false;
  }

  componentDidMount() {
    fetchStringsFromDb()
      .then(
        () => {},
        (error: any) => {
          this.setState({ error, loading: false });
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
      .then(() => {
        this._getLookupData();
        this._setLoggedInUserData();
      });
  }

  componentWillUnmount() {
    clearInterval(this.polling);
  }

  componentDidUpdate(prevProps: IAppProps) {
    const { maintenanceMode } = this.props;

    if (!prevProps.maintenanceMode && maintenanceMode && this.polling === undefined) {
      this.polling = setInterval(() => this._pollingCheckApiStatus(), 60000);
    }
  }

  _setLoggedInUserData = () => {
    const { setUserEmail, setUserAgency, setAssignedRoles } = this.props;
    setUserEmail(getUserEmailFromToken());
    setUserAgency(getUserAgencyFromToken());
    setAssignedRoles(getAssignedRolesFromToken());
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
          this.polling = setInterval(() => this._pollingCheckApiStatus(), 60000);
          turnOnMaintenanceMode();
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

  render() {
    const { loading, error } = this.state;
    const { maintenanceMode, assignedRoles } = this.props;

    addAuthTokenToCookies();

    if (maintenanceMode) {
      return (
        <Layout assignedRoles={assignedRoles}>
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
    } else if (!loading && error === null) {
      return (
        <Layout assignedRoles={assignedRoles}>
          <Switch>
            {process.env.NODE_ENV === 'development' && (
              <Route exact path="/" component={HomeContainer} />
            )}
            {process.env.NODE_ENV === 'development' && (
              <PrivateRoute
                path={SEARCH_DEV_PROD_PATH}
                component={SearchContainer}
                access={assignedRoles.search}
              />
            )}
            {process.env.NODE_ENV !== 'development' && (
              <PrivateRoute
                path={SEARCH_DEV_PROD_PATH}
                component={SearchContainer}
                access={assignedRoles.search}
              />
            )}
            <PrivateRoute
              path={SUBMISSION_PATH}
              component={SubmissionContainer}
              access={assignedRoles.update}
            />
            <PrivateRoute
              path={WORKFLOWS_PATH}
              component={WorkflowsContainer}
              access={assignedRoles.update}
            />
            <PrivateRoute
              path={DEMERGE_PATH}
              component={DemergeContainer}
              access={assignedRoles.update}
            />
            <PrivateRoute
              path={MERGE_PATH}
              component={MergeContainer}
              access={assignedRoles.update}
            />
            <PrivateRoute
              path={SUBMISSION_HISTORY_PATH}
              component={SubmissionHistoryContainer}
              access
            />
            <PrivateRoute
              path={REPORTS_PATH}
              component={ReportsContainer}
              access={assignedRoles.reportBasics}
            />
            <PrivateRoute
              path={MANAGE_USER_ROLES_PATH}
              component={ManageUserRolesContainer}
              access={assignedRoles.manageRoles}
            />
            <PrivateRoute path={USER_PROFILE_PATH} component={UserProfileContainer} access />
          </Switch>
        </Layout>
      );
    } else if (error !== null) {
      return <h1>An Error has occurred.</h1>;
    } else return <div></div>;
  }
}
