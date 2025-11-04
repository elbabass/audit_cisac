import * as actions from '../../../redux/actions/AppActions';
import {
  GET_ACCESS_REQUESTS_FAILURE,
  GET_ACCESS_REQUESTS_REQUEST,
  GET_ACCESS_REQUESTS_SUCCESS,
  GET_ALL_USERS_FAILURE,
  GET_ALL_USERS_REQUEST,
  GET_ALL_USERS_SUCCESS,
  GET_USER_PROFILE_ROLES_FAILURE,
  GET_USER_PROFILE_ROLES_REQUEST,
  GET_USER_PROFILE_ROLES_SUCCESS,
  LOGGED_IN_USER_PROFILE_ROLES_FAILURE,
  LOGGED_IN_USER_PROFILE_ROLES_REQUEST,
  LOGGED_IN_USER_PROFILE_ROLES_SUCCESS,
  REQUEST_ACCESS_FAILURE,
  REQUEST_ACCESS_REQUEST,
  REQUEST_ACCESS_SUCCESS,
  SET_ASSIGNED_ROLES,
  SET_USER_AGENCY,
  SET_USER_EMAIL,
  TURN_OFF_MAINTENANCE_MODE,
  TURN_ON_MAINTENANCE_MODE,
  UPDATE_USER_ROLE_FAILURE,
  UPDATE_USER_ROLE_REQUEST,
  UPDATE_USER_ROLE_SUCCESS,
} from '../../../redux/actions/AppActionTypes';
import { IWebUser, IWebUserRole } from '../../../redux/types/RoleTypes';

describe('App Actions', () => {
  it('turnOnMaintenanceMode calls TURN_ON_MAINTENANCE_MODE', () => {
    const expectedAction = {
      type: TURN_ON_MAINTENANCE_MODE,
    };
    expect(actions.turnOnMaintenanceMode()).toEqual(expectedAction);
  });

  it('turnOffMaintenanceMode calls TURN_OFF_MAINTENANCE_MODE', () => {
    const expectedAction = {
      type: TURN_OFF_MAINTENANCE_MODE,
    };
    expect(actions.turnOffMaintenanceMode()).toEqual(expectedAction);
  });

  it('setUserEmail calls SET_USER_EMAIL', () => {
    const email = 'email';
    const expectedAction = {
      type: SET_USER_EMAIL,
      payload: {
        email,
      },
    };
    expect(actions.setUserEmail(email)).toEqual(expectedAction);
  });

  it('setUserAgency calls SET_USER_AGENCY', () => {
    const agency = 'agency';
    const expectedAction = {
      type: SET_USER_AGENCY,
      payload: {
        agency,
      },
    };
    expect(actions.setUserAgency(agency)).toEqual(expectedAction);
  });

  it('setAssignedRoles calls SET_ASSIGNED_ROLES', () => {
    const assignedRoles = {
      search: false,
      update: false,
      reportBasics: false,
      reportExtracts: false,
      reportAgencyInterest: false,
      reportIswcFullExtract: false,
      manageRoles: false,
    };
    const expectedAction = {
      type: SET_ASSIGNED_ROLES,
      payload: {
        assignedRoles,
      },
    };
    expect(actions.setAssignedRoles(assignedRoles)).toEqual(expectedAction);
  });

  it('loggedInUserProfileRolesRequest calls LOGGED_IN_USER_PROFILE_ROLES_REQUEST', () => {
    const expectedAction = {
      type: LOGGED_IN_USER_PROFILE_ROLES_REQUEST,
    };
    expect(actions.loggedInUserProfileRolesRequest()).toEqual(expectedAction);
  });

  it('loggedInUserProfileRolesSuccess calls LOGGED_IN_USER_PROFILE_ROLES_SUCCESS', () => {
    const loggedInUserProfileRoles: IWebUserRole[] = [];
    const expectedAction = {
      type: LOGGED_IN_USER_PROFILE_ROLES_SUCCESS,
      payload: { loggedInUserProfileRoles },
    };
    expect(actions.loggedInUserProfileRolesSuccess(loggedInUserProfileRoles)).toEqual(
      expectedAction,
    );
  });

  it('loggedInUserProfileRolesError calls LOGGED_IN_USER_PROFILE_ROLES_FAILURE', () => {
    const error = 'error';
    const expectedAction = {
      type: LOGGED_IN_USER_PROFILE_ROLES_FAILURE,
      payload: { error },
    };
    expect(actions.loggedInUserProfileRolesError(error)).toEqual(expectedAction);
  });

  it('getAllUsersRequest calls GET_ALL_USERS_REQUEST', () => {
    const expectedAction = {
      type: GET_ALL_USERS_REQUEST,
    };
    expect(actions.getAllUsersRequest()).toEqual(expectedAction);
  });

  it('getAllUsersSuccess calls GET_ALL_USERS_SUCCESS', () => {
    const users: IWebUser[] = [];
    const expectedAction = {
      type: GET_ALL_USERS_SUCCESS,
      payload: { users },
    };
    expect(actions.getAllUsersSuccess(users)).toEqual(expectedAction);
  });

  it('getAllUsersError calls GET_ALL_USERS_FAILURE', () => {
    const error = 'error';
    const expectedAction = {
      type: GET_ALL_USERS_FAILURE,
      payload: { error },
    };
    expect(actions.getAllUsersError(error)).toEqual(expectedAction);
  });

  it('requestAccessRequest calls REQUEST_ACCESS_REQUEST', () => {
    const expectedAction = {
      type: REQUEST_ACCESS_REQUEST,
    };
    expect(actions.requestAccessRequest()).toEqual(expectedAction);
  });

  it('requestAccessSuccess calls REQUEST_ACCESS_SUCCESS', () => {
    const expectedAction = {
      type: REQUEST_ACCESS_SUCCESS,
    };
    expect(actions.requestAccessSuccess()).toEqual(expectedAction);
  });

  it('requestAccessError calls REQUEST_ACCESS_FAILURE', () => {
    const error = 'error';
    const expectedAction = {
      type: REQUEST_ACCESS_FAILURE,
      payload: {
        error,
      },
    };
    expect(actions.requestAccessError(error)).toEqual(expectedAction);
  });

  it('getUserProfileRolesRequest calls GET_USER_PROFILE_ROLES_REQUEST', () => {
    const expectedAction = {
      type: GET_USER_PROFILE_ROLES_REQUEST,
    };
    expect(actions.getUserProfileRolesRequest()).toEqual(expectedAction);
  });

  it('getUserProfileRolesSuccess calls GET_USER_PROFILE_ROLES_SUCCESS', () => {
    const userProfileRoles: IWebUserRole[] = [];
    const expectedAction = {
      type: GET_USER_PROFILE_ROLES_SUCCESS,
      payload: {
        userProfileRoles,
      },
    };
    expect(actions.getUserProfileRolesSuccess(userProfileRoles)).toEqual(expectedAction);
  });

  it('getUserProfileRolesError calls GET_USER_PROFILE_ROLES_FAILURE', () => {
    const error = 'error';
    const expectedAction = {
      type: GET_USER_PROFILE_ROLES_FAILURE,
      payload: {
        error,
      },
    };
    expect(actions.getUserProfileRolesError(error)).toEqual(expectedAction);
  });

  it('updateUserRoleRequest calls UPDATE_USER_ROLE_REQUEST', () => {
    const expectedAction = {
      type: UPDATE_USER_ROLE_REQUEST,
    };
    expect(actions.updateUserRoleRequest()).toEqual(expectedAction);
  });

  it('updateUserRoleSuccess calls UPDATE_USER_ROLE_SUCCESS', () => {
    const user: IWebUser = {
      email: 'string',
      agencyId: 'string',
      webUserRoles: [],
    };
    const expectedAction = {
      type: UPDATE_USER_ROLE_SUCCESS,
      payload: {
        user,
      },
    };
    expect(actions.updateUserRoleSuccess(user)).toEqual(expectedAction);
  });

  it('updateUserRoleError calls UPDATE_USER_ROLE_FAILURE', () => {
    const error = 'error';
    const expectedAction = {
      type: UPDATE_USER_ROLE_FAILURE,
      payload: {
        error,
      },
    };
    expect(actions.updateUserRoleError(error)).toEqual(expectedAction);
  });

  it('getAccessRequestsRequest calls GET_ACCESS_REQUESTS_REQUEST', () => {
    const expectedAction = {
      type: GET_ACCESS_REQUESTS_REQUEST,
    };
    expect(actions.getAccessRequestsRequest()).toEqual(expectedAction);
  });

  it('getAccessRequestsSuccess calls GET_ACCESS_REQUESTS_SUCCESS', () => {
    const usersWithPendingRequests: IWebUser[] = [];
    const expectedAction = {
      type: GET_ACCESS_REQUESTS_SUCCESS,
      payload: {
        usersWithPendingRequests,
      },
    };
    expect(actions.getAccessRequestsSuccess(usersWithPendingRequests)).toEqual(expectedAction);
  });

  it('getAccessRequestsError calls GET_ACCESS_REQUESTS_FAILURE', () => {
    const error = 'error';
    const expectedAction = {
      type: GET_ACCESS_REQUESTS_FAILURE,
      payload: {
        error,
      },
    };
    expect(actions.getAccessRequestsError(error)).toEqual(expectedAction);
  });
});
