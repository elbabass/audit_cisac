import { IWebUser, IWebUserRole } from '../../../redux/types/RoleTypes';

export interface IAssignedRolesGridProps {
  roles: number[];
  manageMode?: boolean;
  requestAccess?: (role: IWebUserRole) => void;
  getLoggedInUserProfileRoles?: () => void;
  updateUserRole?: (user: IWebUser) => void;
  user: IWebUser;
}
