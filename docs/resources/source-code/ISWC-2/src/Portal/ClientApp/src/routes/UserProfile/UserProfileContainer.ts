import { connect } from 'react-redux';
import { ApplicationState } from '../../redux/store/portal';
import { Dispatch } from 'redux';
import UserProfile from './UserProfile';
import { getStrings } from '../../configuration/Localization';
import { getLoggedInUserProfileRolesThunk, requestAccessThunk } from '../../redux/thunks/AppThunks';
import { RouteComponentProps } from 'react-router';
import { IWebUserRole } from '../../redux/types/RoleTypes';
import { UserRoles } from '../../consts';

const { MY_PROFILE } = getStrings();

export default connect(
  (state: ApplicationState, routerState: RouteComponentProps) => ({
    header: MY_PROFILE,
    username: state.appReducer.email,
    agency: state.appReducer.agency,
    roles: [
      UserRoles.SEARCH_ROLE,
      UserRoles.UPDATE_ROLE,
      UserRoles.REPORT_BASICS_ROLE,
      UserRoles.REPORT_EXTRACT_ROLE,
      UserRoles.REPORT_AGENCY_INTEREST_ROLE,
      UserRoles.REPORT_ISWC_FULL_EXTRACT_ROLE,
      UserRoles.MANAGE_ROLES_ROLE,
    ],
    user: {
      email: state.appReducer.email,
      agencyId: state.appReducer.agency,
      webUserRoles: state.appReducer.loggedInUserProfileRoles,
    },
    router: routerState,
    userProfileError: state.appReducer.userProfileError,
    userProfileLoading: state.appReducer.userProfileLoading,
    accessRequestError: state.appReducer.accessRequestError,
    accessRequestLoading: state.appReducer.userProfileLoading,
  }),
  (dispatch: Dispatch) => ({
    getLoggedInUserProfileRoles: () => dispatch<any>(getLoggedInUserProfileRolesThunk()),
    requestAccess: (role: IWebUserRole) => dispatch<any>(requestAccessThunk(role)),
  }),
)(UserProfile);
