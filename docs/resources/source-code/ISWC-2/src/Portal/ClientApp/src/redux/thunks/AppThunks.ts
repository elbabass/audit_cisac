import { Action, Dispatch } from 'redux';
import { ThunkAction } from 'redux-thunk';
import {
  getAllUsersError,
  getAllUsersRequest,
  getAllUsersSuccess,
  loggedInUserProfileRolesError,
  loggedInUserProfileRolesRequest,
  loggedInUserProfileRolesSuccess,
  requestAccessRequest,
  requestAccessSuccess,
  requestAccessError,
  turnOnMaintenanceMode,
  getUserProfileRolesRequest,
  getUserProfileRolesSuccess,
  getUserProfileRolesError,
  updateUserRoleRequest,
  updateUserRoleSuccess,
  updateUserRoleError,
  getAccessRequestsRequest,
  getAccessRequestsSuccess,
  getAccessRequestsError,
} from '../actions/AppActions';
import {
  getAccessRequests,
  getAllUsers,
  getLoggedInUserProfileRoles,
  getUserProfile,
  requestAccess,
  updateUserRole,
} from '../services/AppService';
import { ApplicationState } from '../store/portal';
import { IWebUser, IWebUserRole } from '../types/RoleTypes';

export const getLoggedInUserProfileRolesThunk = (): ThunkAction<
  void,
  ApplicationState,
  null,
  Action<string>
> => {
  return (dispatch: Dispatch) => {
    dispatch(loggedInUserProfileRolesRequest());
    return getLoggedInUserProfileRoles()
      .then((result: any) => {
        dispatch(loggedInUserProfileRolesSuccess(result.data));
      })
      .catch((error: any) => {
        if (error.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(loggedInUserProfileRolesError(error));
      });
  };
};

export const getAllUsersThunk = (): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(getAllUsersRequest());
    return getAllUsers()
      .then((result: any) => {
        dispatch(getAllUsersSuccess(result.data));
      })
      .catch((error: any) => {
        if (error.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(getAllUsersError(error));
      });
  };
};

export const requestAccessThunk = (
  role: IWebUserRole,
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(requestAccessRequest());
    return requestAccess(role)
      .then((result: any) => {
        dispatch(requestAccessSuccess());
        dispatch<any>(getLoggedInUserProfileRolesThunk());
      })
      .catch((error: any) => {
        if (error.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(requestAccessError(error));
      });
  };
};

export const getUserProfileRolesThunk = (
  user: IWebUser,
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(getUserProfileRolesRequest());
    return getUserProfile(user)
      .then((result: any) => {
        dispatch(getUserProfileRolesSuccess(result.data.webUserRoles));
      })
      .catch((error: any) => {
        if (error.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(getUserProfileRolesError(error));
      });
  };
};

export const updateUserRoleThunk = (
  user: IWebUser,
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(updateUserRoleRequest());
    return updateUserRole(user)
      .then((result: any) => {
        dispatch(updateUserRoleSuccess(result.data));
        dispatch<any>(getUserProfileRolesThunk(user));
      })
      .catch((error: any) => {
        if (error.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(updateUserRoleError(error));
      });
  };
};

export const getAccessRequestsThunk = (): ThunkAction<
  void,
  ApplicationState,
  null,
  Action<string>
> => {
  return (dispatch: Dispatch) => {
    dispatch(getAccessRequestsRequest());
    return getAccessRequests()
      .then((result: any) => {
        dispatch(getAccessRequestsSuccess(result.data));
      })
      .catch((error: any) => {
        if (error.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(getAccessRequestsError(error));
      });
  };
};
