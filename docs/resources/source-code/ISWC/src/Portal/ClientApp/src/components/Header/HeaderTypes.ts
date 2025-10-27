import { IAssignedRoles, IWebUserRole } from '../../redux/types/RoleTypes';

export interface IHeaderProps {
  renderHeaderItem: (path: string, icon: string, text: string, newTab?: boolean) => void;
  toggle: () => void;
  toggleSettings?: () => void;
  logout?: () => void;
  isOpen: boolean;
  showSettings?: boolean;
  assignedRoles?: IAssignedRoles;
}

export interface IHeaderState {
  isOpen: boolean;
  showSettings: boolean;
}
