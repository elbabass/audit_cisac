import { RouteComponentProps } from 'react-router';
import { IWebUser, IWebUserRole } from '../../redux/types/RoleTypes';

export interface IUserProfileProps {
  header: string;
  manageMode?: boolean;
  user: IWebUser;
  roles: number[];
  loggedInUserProfileRoles?: IWebUserRole[];
  router?: RouteComponentProps;
  userProfileError?: any;
  userProfileLoading?: boolean;
  updateRoleError?: any;
  accessRequestError?: any;
  accessRequestLoading?: boolean;
  goBack?: () => void;
  getLoggedInUserProfileRoles?: () => void;
  requestAccess?: (role: IWebUserRole) => void;
  getUserProfileRoles?: (user: IWebUser) => void;
  updateUserRole?: (user: IWebUser) => void;
}

export interface IUserProfileState {
  isModalOpen: boolean;
  requestedRole?: IWebUserRole;
}
