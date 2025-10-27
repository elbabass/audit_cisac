import { IAssignedRoles } from '../../../redux/types/RoleTypes';
import { IReportsParamFields } from '../ReportsTypes';

export interface IAgencyWorkListProps {
  loading: boolean;
  error?: any;
  getAgencyWorkListCachedError?: any;
  extractToFtp: (parmas: IReportsParamFields) => void;
  getDateOfCachedReport: () => void;
  agencyWorkListCachedVersion: string;
  email: string;
  extractToFtpSuccess: boolean;
  assignedRoles: IAssignedRoles;
}

export interface IAgencyWorkListState {
  showExtractToFtpDiv: boolean;
  paramFields: IReportsParamFields;
}
