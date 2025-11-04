import { Reducer } from 'redux';
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
  UPDATE_USER_ROLE_FAILURE,
  GET_ACCESS_REQUESTS_REQUEST,
  GET_ACCESS_REQUESTS_SUCCESS,
  GET_ACCESS_REQUESTS_FAILURE,
} from '../actions/AppActionTypes';
import { IAssignedRoles, IWebUser, IWebUserRole } from '../types/RoleTypes';

export interface IAppReducerState {
  maintenanceMode: boolean;
  email: string;
  agency: string;
  assignedRoles: IAssignedRoles;
  loggedInUserProfileRoles: IWebUserRole[];
  userProfileRoles: IWebUserRole[];
  users: IWebUser[];
  userProfileError?: any;
  userProfileLoading?: boolean;
  agencyUsersError?: any;
  agencyUsersLoading?: boolean;
  updateRoleError?: any;
  getAccessRequestsLoading?: boolean;
  getAccessRequestsError?: any;
  usersWithPendingRequests: IWebUser[];
  accessRequestError?: any;
}

const initialState: IAppReducerState = {
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
};

export const reducer: Reducer<IAppReducerState> = (
  state = initialState,
  action: AppActionTypes,
): IAppReducerState => {
  switch (action.type) {
    case TURN_ON_MAINTENANCE_MODE: {
      return {
        ...state,
        maintenanceMode: true,
      };
    }
    case TURN_OFF_MAINTENANCE_MODE: {
      return {
        ...state,
        maintenanceMode: false,
      };
    }
    case SET_USER_EMAIL: {
      const { email } = action.payload;
      return {
        ...state,
        email,
      };
    }
    case SET_USER_AGENCY: {
      const { agency } = action.payload;
      return {
        ...state,
        agency,
      };
    }
    case SET_ASSIGNED_ROLES: {
      const { assignedRoles } = action.payload;
      return {
        ...state,
        assignedRoles,
      };
    }
    case LOGGED_IN_USER_PROFILE_ROLES_REQUEST: {
      return {
        ...state,
        userProfileError: undefined,
        userProfileLoading: true,
      };
    }
    case LOGGED_IN_USER_PROFILE_ROLES_SUCCESS: {
      const { loggedInUserProfileRoles } = action.payload;
      return {
        ...state,
        loggedInUserProfileRoles,
        userProfileLoading: false,
      };
    }
    case LOGGED_IN_USER_PROFILE_ROLES_FAILURE: {
      const { error } = action.payload;
      return {
        ...state,
        userProfileError: error,
        userProfileLoading: false,
      };
    }
    case GET_ALL_USERS_REQUEST: {
      return {
        ...state,
        agencyUsersError: undefined,
        agencyUsersLoading: true,
      };
    }
    case GET_ALL_USERS_SUCCESS: {
      const { users } = action.payload;
      return {
        ...state,
        users,
        agencyUsersLoading: false,
      };
    }
    case GET_ALL_USERS_FAILURE: {
      const { error } = action.payload;
      return {
        ...state,
        agencyUsersError: error,
        agencyUsersLoading: false,
      };
    }
    case REQUEST_ACCESS_REQUEST: {
      return {
        ...state,
        accessRequestError: undefined,
        userProfileLoading: true,
      };
    }
    case REQUEST_ACCESS_SUCCESS: {
      return {
        ...state,
        userProfileLoading: false,
      };
    }
    case REQUEST_ACCESS_FAILURE: {
      const { error } = action.payload;
      return {
        ...state,
        accessRequestError: error,
        userProfileLoading: false,
      };
    }
    case GET_USER_PROFILE_ROLES_REQUEST: {
      return {
        ...state,
        userProfileLoading: true,
        userProfileError: undefined,
        updateRoleError: undefined,
      };
    }
    case GET_USER_PROFILE_ROLES_SUCCESS: {
      const { userProfileRoles } = action.payload;
      return {
        ...state,
        userProfileLoading: false,
        userProfileRoles,
      };
    }
    case GET_USER_PROFILE_ROLES_FAILURE: {
      const { error } = action.payload;
      return {
        ...state,
        userProfileLoading: false,
        userProfileError: error,
      };
    }
    case UPDATE_USER_ROLE_REQUEST: {
      return {
        ...state,
        updateRoleError: undefined,
      };
    }
    case UPDATE_USER_ROLE_FAILURE: {
      const { error } = action.payload;
      return {
        ...state,
        updateRoleError: error,
      };
    }
    case GET_ACCESS_REQUESTS_REQUEST: {
      return {
        ...state,
        getAccessRequestsLoading: true,
        getAccessRequestsError: undefined,
      };
    }
    case GET_ACCESS_REQUESTS_SUCCESS: {
      const { usersWithPendingRequests } = action.payload;
      return {
        ...state,
        getAccessRequestsLoading: false,
        usersWithPendingRequests,
      };
    }
    case GET_ACCESS_REQUESTS_FAILURE: {
      const { error } = action.payload;
      return {
        ...state,
        getAccessRequestsLoading: false,
        getAccessRequestsError: error,
      };
    }
    default:
      return state;
  }
};
