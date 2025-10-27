import {
  AppActionTypes,
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
  SET_ASSIGNED_ROLES,
  SET_USER_AGENCY,
  SET_USER_EMAIL,
  TURN_OFF_MAINTENANCE_MODE,
  TURN_ON_MAINTENANCE_MODE,
  UPDATE_USER_ROLE_FAILURE,
  UPDATE_USER_ROLE_REQUEST,
} from '../../../redux/actions/AppActionTypes';
import { reducer } from '../../../redux/reducers/AppReducer';
import { IWebUser, IWebUserRole } from '../../../redux/types/RoleTypes';

describe('App Reducer', () => {
  it('should return the initial state', () => {
    expect(reducer(undefined, {} as AppActionTypes)).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
    });
  });

  it('should handle TURN_ON_MAINTENANCE_MODE', () => {
    expect(
      reducer(undefined, {
        type: TURN_ON_MAINTENANCE_MODE,
      }),
    ).toEqual({
      maintenanceMode: true,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
    });
  });

  it('should handle TURN_ON_MAINTENANCE_MODE', () => {
    expect(
      reducer(undefined, {
        type: TURN_OFF_MAINTENANCE_MODE,
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
    });
  });

  it('should handle SET_USER_EMAIL', () => {
    const email = 'email';
    expect(
      reducer(undefined, {
        type: SET_USER_EMAIL,
        payload: {
          email,
        },
      }),
    ).toEqual({
      maintenanceMode: false,
      email: email,
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
    });
  });

  it('should handle SET_USER_AGENCY', () => {
    const agency = 'agency';
    expect(
      reducer(undefined, {
        type: SET_USER_AGENCY,
        payload: {
          agency,
        },
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: agency,
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
    });
  });

  it('should handle SET_ASSIGNED_ROLES', () => {
    const assignedRoles = {
      search: true,
      update: true,
      reportBasics: true,
      reportExtracts: false,
      reportAgencyInterest: false,
      reportIswcFullExtract: false,
      manageRoles: false,
    };
    expect(
      reducer(undefined, {
        type: SET_ASSIGNED_ROLES,
        payload: {
          assignedRoles,
        },
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: assignedRoles,
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
    });
  });

  it('should handle LOGGED_IN_USER_PROFILE_ROLES_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: LOGGED_IN_USER_PROFILE_ROLES_REQUEST,
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
      userProfileLoading: true,
    });
  });

  it('should handle LOGGED_IN_USER_PROFILE_ROLES_SUCCESS', () => {
    const loggedInUserProfileRoles: IWebUserRole[] = [{ role: 1 }];
    expect(
      reducer(undefined, {
        type: LOGGED_IN_USER_PROFILE_ROLES_SUCCESS,
        payload: {
          loggedInUserProfileRoles,
        },
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: loggedInUserProfileRoles,
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
      userProfileLoading: false,
    });
  });

  it('should handle LOGGED_IN_USER_PROFILE_ROLES_FAILURE', () => {
    const error = 'error';
    expect(
      reducer(undefined, {
        type: LOGGED_IN_USER_PROFILE_ROLES_FAILURE,
        payload: {
          error,
        },
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
      userProfileLoading: false,
      userProfileError: error,
    });
  });

  it('should handle GET_ALL_USERS_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: GET_ALL_USERS_REQUEST,
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
      agencyUsersLoading: true,
    });
  });

  it('should handle GET_ALL_USERS_SUCCESS', () => {
    const users: IWebUserRole[] = [{ role: 1 }];
    expect(
      reducer(undefined, {
        type: GET_ALL_USERS_SUCCESS,
        payload: {
          users,
        },
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: users,
      usersWithPendingRequests: [],
      agencyUsersLoading: false,
    });
  });

  it('should handle GET_ALL_USERS_FAILURE', () => {
    const error = 'error';
    expect(
      reducer(undefined, {
        type: GET_ALL_USERS_FAILURE,
        payload: {
          error,
        },
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
      agencyUsersLoading: false,
      agencyUsersError: error,
    });
  });

  it('should handle REQUEST_ACCESS_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: REQUEST_ACCESS_REQUEST,
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
      userProfileLoading: true,
    });
  });

  it('should handle REQUEST_ACCESS_FAILURE', () => {
    const error = 'error';
    expect(
      reducer(undefined, {
        type: REQUEST_ACCESS_FAILURE,
        payload: {
          error,
        },
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
      accessRequestError: error,
      userProfileLoading: false,
    });
  });

  it('should handle GET_USER_PROFILE_ROLES_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: GET_USER_PROFILE_ROLES_REQUEST,
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
      userProfileLoading: true,
    });
  });

  it('should handle GET_USER_PROFILE_ROLES_SUCCESS', () => {
    const userProfileRoles: IWebUserRole[] = [{ role: 1 }];
    expect(
      reducer(undefined, {
        type: GET_USER_PROFILE_ROLES_SUCCESS,
        payload: {
          userProfileRoles,
        },
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: userProfileRoles,
      users: [],
      usersWithPendingRequests: [],
      userProfileLoading: false,
    });
  });

  it('should handle GET_USER_PROFILE_ROLES_FAILURE', () => {
    const error: any = 'error';
    expect(
      reducer(undefined, {
        type: GET_USER_PROFILE_ROLES_FAILURE,
        payload: {
          error,
        },
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
      userProfileLoading: false,
      userProfileError: error,
    });
  });

  it('should handle UPDATE_USER_ROLE_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: UPDATE_USER_ROLE_REQUEST,
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
    });
  });

  it('should handle UPDATE_USER_ROLE_FAILURE', () => {
    const error = 'error';
    expect(
      reducer(undefined, {
        type: UPDATE_USER_ROLE_FAILURE,
        payload: {
          error,
        },
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
      updateRoleError: error,
    });
  });

  it('should handle GET_ACCESS_REQUESTS_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: GET_ACCESS_REQUESTS_REQUEST,
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
      getAccessRequestsLoading: true,
    });
  });

  it('should handle GET_ACCESS_REQUESTS_SUCCESS', () => {
    const usersWithPendingRequests: IWebUser[] = [{ email: 'a', agencyId: 'b', webUserRoles: [] }];
    expect(
      reducer(undefined, {
        type: GET_ACCESS_REQUESTS_SUCCESS,
        payload: {
          usersWithPendingRequests,
        },
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      getAccessRequestsLoading: false,
      usersWithPendingRequests: usersWithPendingRequests,
    });
  });

  it('should handle GET_ACCESS_REQUESTS_FAILURE', () => {
    const error = 'error';
    expect(
      reducer(undefined, {
        type: GET_ACCESS_REQUESTS_FAILURE,
        payload: {
          error,
        },
      }),
    ).toEqual({
      maintenanceMode: false,
      email: '',
      agency: '',
      assignedRoles: {
        search: false,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
      loggedInUserProfileRoles: [],
      userProfileRoles: [],
      users: [],
      usersWithPendingRequests: [],
      getAccessRequestsLoading: false,
      getAccessRequestsError: error,
    });
  });
});
