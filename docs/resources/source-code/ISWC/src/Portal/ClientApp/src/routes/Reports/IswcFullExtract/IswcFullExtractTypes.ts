import { IAssignedRoles } from '../../../redux/types/RoleTypes';
import { IReportsParamFields } from '../ReportsTypes';

export interface IIswcFullExtractProps {
  loading: boolean;
  error?: any;
  getFullExtractCachedError?: any;
  extractToFtp: (parmas: IReportsParamFields) => void;
  getDateOfCachedReport: () => void;
  fullExtractCachedVersion: string;
  email: string;
  extractToFtpSuccess: boolean;
  assignedRoles: IAssignedRoles;
}

export interface IIswcFullExtractState {
  showExtractToFtpDiv: boolean;
  paramFields: IReportsParamFields;
}
