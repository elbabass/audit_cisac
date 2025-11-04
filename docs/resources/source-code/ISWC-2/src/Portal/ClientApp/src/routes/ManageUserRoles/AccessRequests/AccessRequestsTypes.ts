import { IWebUser } from '../../../redux/types/RoleTypes';

export interface IAccessRequestsState {}

export interface IAccessRequestsProps {
  usersWithPendingRequests: IWebUser[];
  loading?: boolean;
  error?: any;
  manageRolesAction: (user: IWebUser) => void;
  getAccessRequests: () => void;
}
