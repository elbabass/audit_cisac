import { IWebUser, IWebUserRole } from '../../redux/types/RoleTypes';

export interface IManageUserRolesState {
  showUserProfile: boolean;
  user: IWebUser;
}

export interface IManageUserRolesProps {
  users: IWebUser[];
  agencyUsersLoading?: boolean;
  agencyUsersError?: any;
  userProfileError?: any;
  userProfileLoading?: boolean;
  userProfileRoles?: IWebUserRole[];
  updateRoleError?: any;
  usersWithPendingRequests: IWebUser[];
  getAccessRequestsLoading?: boolean;
  getAccessRequestsError?: any;
  updateUserRole: (user: IWebUser) => void;
  getAllUsers: () => void;
  getUserProfileRoles: (user: IWebUser) => void;
  getAccessRequests: () => void;
}
