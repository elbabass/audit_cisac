import { IWebUser } from '../../../redux/types/RoleTypes';

export interface IAgencyUsersState {
  username: string;
  users: IWebUser[];
}

export interface IAgencyUsersProps {
  users: IWebUser[];
  getAllUsers: () => void;
  error?: any;
  loading?: boolean;
  manageRolesAction: (user: IWebUser) => void;
}
