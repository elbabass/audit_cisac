import { IAssignedRoles, IWebUser, IWebUserRole } from '../types/RoleTypes';
import {
  AppActionTypes,
  TURN_ON_MAINTENANCE_MODE,
  TURN_OFF_MAINTENANCE_MODE,
  SET_USER_EMAIL,
  SET_USER_AGENCY,
  SET_ASSIGNED_ROLES,
  LOGGED_IN_USER_PROFILE_ROLES_REQUEST,
  LOGGED_IN_USER_PROFILE_ROLES_SUCCESS,
  LOGGED_IN_USER_PROFILE_ROLES_FAILURE,
  GET_ALL_USERS_REQUEST,
  GET_ALL_USERS_SUCCESS,
  GET_ALL_USERS_FAILURE,
  REQUEST_ACCESS_REQUEST,
  REQUEST_ACCESS_SUCCESS,
  REQUEST_ACCESS_FAILURE,
  GET_USER_PROFILE_ROLES_REQUEST,
  GET_USER_PROFILE_ROLES_SUCCESS,
  GET_USER_PROFILE_ROLES_FAILURE,
  UPDATE_USER_ROLE_REQUEST,
  UPDATE_USER_ROLE_SUCCESS,
  UPDATE_USER_ROLE_FAILURE,
  GET_ACCESS_REQUESTS_REQUEST,
  GET_ACCESS_REQUESTS_SUCCESS,
  GET_ACCESS_REQUESTS_FAILURE,
} from './AppActionTypes';

export function turnOnMaintenanceMode(): AppActionTypes {
  return {
    type: TURN_ON_MAINTENANCE_MODE,
  };
}

export function turnOffMaintenanceMode(): AppActionTypes {
  return {
    type: TURN_OFF_MAINTENANCE_MODE,
  };
}

export function setUserEmail(email: string): AppActionTypes {
  return {
    type: SET_USER_EMAIL,
    payload: {
      email,
    },
  };
}

export function setUserAgency(agency: string): AppActionTypes {
  return {
    type: SET_USER_AGENCY,
    payload: {
      agency,
    },
  };
}

export function setAssignedRoles(assignedRoles: IAssignedRoles): AppActionTypes {
  return {
    type: SET_ASSIGNED_ROLES,
    payload: {
      assignedRoles,
    },
  };
}

export function loggedInUserProfileRolesRequest(): AppActionTypes {
  return {
    type: LOGGED_IN_USER_PROFILE_ROLES_REQUEST,
  };
}

export function loggedInUserProfileRolesSuccess(
  loggedInUserProfileRoles: IWebUserRole[],
): AppActionTypes {
  return {
    type: LOGGED_IN_USER_PROFILE_ROLES_SUCCESS,
    payload: {
      loggedInUserProfileRoles,
    },
  };
}

export function loggedInUserProfileRolesError(error: any): AppActionTypes {
  return {
    type: LOGGED_IN_USER_PROFILE_ROLES_FAILURE,
    payload: {
      error,
    },
  };
}

export function getAllUsersRequest(): AppActionTypes {
  return {
    type: GET_ALL_USERS_REQUEST,
  };
}

export function getAllUsersSuccess(users: IWebUser[]): AppActionTypes {
  return {
    type: GET_ALL_USERS_SUCCESS,
    payload: {
      users,
    },
  };
}

export function getAllUsersError(error: any): AppActionTypes {
  return {
    type: GET_ALL_USERS_FAILURE,
    payload: {
      error,
    },
  };
}

export function requestAccessRequest(): AppActionTypes {
  return {
    type: REQUEST_ACCESS_REQUEST,
  };
}

export function requestAccessSuccess(): AppActionTypes {
  return {
    type: REQUEST_ACCESS_SUCCESS,
  };
}

export function requestAccessError(error: any): AppActionTypes {
  return {
    type: REQUEST_ACCESS_FAILURE,
    payload: {
      error,
    },
  };
}

export function getUserProfileRolesRequest(): AppActionTypes {
  return {
    type: GET_USER_PROFILE_ROLES_REQUEST,
  };
}

export function getUserProfileRolesSuccess(userProfileRoles: IWebUserRole[]): AppActionTypes {
  return {
    type: GET_USER_PROFILE_ROLES_SUCCESS,
    payload: {
      userProfileRoles,
    },
  };
}

export function getUserProfileRolesError(error: any): AppActionTypes {
  return {
    type: GET_USER_PROFILE_ROLES_FAILURE,
    payload: {
      error,
    },
  };
}

export function updateUserRoleRequest(): AppActionTypes {
  return {
    type: UPDATE_USER_ROLE_REQUEST,
  };
}

export function updateUserRoleSuccess(user: IWebUser): AppActionTypes {
  return {
    type: UPDATE_USER_ROLE_SUCCESS,
    payload: {
      user,
    },
  };
}

export function updateUserRoleError(error: any): AppActionTypes {
  return {
    type: UPDATE_USER_ROLE_FAILURE,
    payload: {
      error,
    },
  };
}

export function getAccessRequestsRequest(): AppActionTypes {
  return {
    type: GET_ACCESS_REQUESTS_REQUEST,
  };
}

export function getAccessRequestsSuccess(usersWithPendingRequests: IWebUser[]): AppActionTypes {
  return {
    type: GET_ACCESS_REQUESTS_SUCCESS,
    payload: {
      usersWithPendingRequests,
    },
  };
}

export function getAccessRequestsError(error: any): AppActionTypes {
  return {
    type: GET_ACCESS_REQUESTS_FAILURE,
    payload: {
      error,
    },
  };
}
