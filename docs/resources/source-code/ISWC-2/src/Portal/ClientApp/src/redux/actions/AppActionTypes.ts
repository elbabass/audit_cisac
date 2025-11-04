import { IAssignedRoles, IWebUser, IWebUserRole } from '../types/RoleTypes';

export const REDUX_STORAGE_LOAD = 'REDUX_STORAGE_LOAD';

export const TURN_ON_MAINTENANCE_MODE = 'TURN_ON_MAINTENANCE_MODE';
export const TURN_OFF_MAINTENANCE_MODE = 'TURN_OFF_MAINTENANCE_MODE';

export const SET_USER_EMAIL = 'SET_USER_EMAIL';

export const SET_USER_AGENCY = 'SET_USER_AGENCY';

export const SET_ASSIGNED_ROLES = 'SET_ASSIGNED_ROLES';

export const LOGGED_IN_USER_PROFILE_ROLES_REQUEST = 'LOGGED_IN_USER_PROFILE_ROLES_REQUEST';
export const LOGGED_IN_USER_PROFILE_ROLES_SUCCESS = 'LOGGED_IN_USER_PROFILE_ROLES_SUCCESS';
export const LOGGED_IN_USER_PROFILE_ROLES_FAILURE = 'LOGGED_IN_USER_PROFILE_ROLES_FAILURE';

export const GET_ALL_USERS_REQUEST = 'GET_ALL_USERS_REQUEST';
export const GET_ALL_USERS_SUCCESS = 'GET_ALL_USERS_SUCCESS';
export const GET_ALL_USERS_FAILURE = 'GET_ALL_USERS_FAILURE';

export const REQUEST_ACCESS_REQUEST = 'REQUEST_ACCESS_REQUEST';
export const REQUEST_ACCESS_SUCCESS = 'REQUEST_ACCESS_SUCCESS';
export const REQUEST_ACCESS_FAILURE = 'REQUEST_ACCESS_FAILURE';

export const GET_USER_PROFILE_ROLES_REQUEST = 'GET_USER_PROFILE_ROLES_REQUEST';
export const GET_USER_PROFILE_ROLES_SUCCESS = 'GET_USER_PROFILE_ROLES_SUCCESS';
export const GET_USER_PROFILE_ROLES_FAILURE = 'GET_USER_PROFILE_ROLES_FAILURE';

export const UPDATE_USER_ROLE_REQUEST = 'UPDATE_USER_ROLE_REQUEST';
export const UPDATE_USER_ROLE_SUCCESS = 'UPDATE_USER_ROLE_SUCCESS';
export const UPDATE_USER_ROLE_FAILURE = 'UPDATE_USER_ROLE_FAILURE';

export const GET_ACCESS_REQUESTS_REQUEST = 'GET_ACCESS_REQUESTS_REQUEST';
export const GET_ACCESS_REQUESTS_SUCCESS = 'GET_ACCESS_REQUESTS_SUCCESS';
export const GET_ACCESS_REQUESTS_FAILURE = 'GET_ACCESS_REQUESTS_FAILURE';

export interface TurnOnMaintenanceMode {
  type: typeof TURN_ON_MAINTENANCE_MODE;
}

export interface TurnOffMaintenanceMode {
  type: typeof TURN_OFF_MAINTENANCE_MODE;
}

export interface SetUserEmail {
  type: typeof SET_USER_EMAIL;
  payload: {
    email: string;
  };
}

export interface SetUserAgency {
  type: typeof SET_USER_AGENCY;
  payload: {
    agency: string;
  };
}

export interface SetAssignedRoles {
  type: typeof SET_ASSIGNED_ROLES;
  payload: {
    assignedRoles: IAssignedRoles;
  };
}

export interface LoggedInUserProfileRolesRequest {
  type: typeof LOGGED_IN_USER_PROFILE_ROLES_REQUEST;
}

export interface LoggedInUserProfileRolesSuccess {
  type: typeof LOGGED_IN_USER_PROFILE_ROLES_SUCCESS;
  payload: {
    loggedInUserProfileRoles: IWebUserRole[];
  };
}

export interface LoggedInUserProfileRolesError {
  type: typeof LOGGED_IN_USER_PROFILE_ROLES_FAILURE;
  payload: {
    error: any;
  };
}

export interface GetAllUsersRequest {
  type: typeof GET_ALL_USERS_REQUEST;
}

export interface GetAllUsersSuccess {
  type: typeof GET_ALL_USERS_SUCCESS;
  payload: {
    users: IWebUser[];
  };
}

export interface GetAllUsersError {
  type: typeof GET_ALL_USERS_FAILURE;
  payload: {
    error: any;
  };
}

export interface RequestAccessRequest {
  type: typeof REQUEST_ACCESS_REQUEST;
}

export interface RequestAccessSuccess {
  type: typeof REQUEST_ACCESS_SUCCESS;
}

export interface RequestAccessError {
  type: typeof REQUEST_ACCESS_FAILURE;
  payload: {
    error: any;
  };
}

export interface GetUserProfileRolesRequest {
  type: typeof GET_USER_PROFILE_ROLES_REQUEST;
}

export interface GetUserProfileRolesSuccess {
  type: typeof GET_USER_PROFILE_ROLES_SUCCESS;
  payload: {
    userProfileRoles: IWebUserRole[];
  };
}

export interface GetUserProfileRolesError {
  type: typeof GET_USER_PROFILE_ROLES_FAILURE;
  payload: {
    error: any;
  };
}

export interface UpdateUserRoleRequest {
  type: typeof UPDATE_USER_ROLE_REQUEST;
}

export interface UpdateUserRoleSuccess {
  type: typeof UPDATE_USER_ROLE_SUCCESS;
  payload: {
    user: IWebUser;
  };
}

export interface UpdateUserRoleError {
  type: typeof UPDATE_USER_ROLE_FAILURE;
  payload: {
    error: any;
  };
}

export interface GetAccessRequestsRequest {
  type: typeof GET_ACCESS_REQUESTS_REQUEST;
}

export interface GetAccessRequestsSuccess {
  type: typeof GET_ACCESS_REQUESTS_SUCCESS;
  payload: {
    usersWithPendingRequests: IWebUser[];
  };
}

export interface GetAccessRequestsError {
  type: typeof GET_ACCESS_REQUESTS_FAILURE;
  payload: {
    error: any;
  };
}

export type AppActionTypes =
  | TurnOnMaintenanceMode
  | TurnOffMaintenanceMode
  | SetUserEmail
  | SetUserAgency
  | SetAssignedRoles
  | LoggedInUserProfileRolesRequest
  | LoggedInUserProfileRolesSuccess
  | LoggedInUserProfileRolesError
  | GetAllUsersRequest
  | GetAllUsersSuccess
  | GetAllUsersError
  | RequestAccessRequest
  | RequestAccessSuccess
  | RequestAccessError
  | GetUserProfileRolesRequest
  | GetUserProfileRolesSuccess
  | GetUserProfileRolesError
  | UpdateUserRoleRequest
  | UpdateUserRoleSuccess
  | UpdateUserRoleError
  | GetAccessRequestsRequest
  | GetAccessRequestsSuccess
  | GetAccessRequestsError;
