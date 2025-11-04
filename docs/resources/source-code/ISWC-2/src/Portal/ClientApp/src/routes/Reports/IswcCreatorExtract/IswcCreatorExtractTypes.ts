import { IAssignedRoles } from '../../../redux/types/RoleTypes';
import { IReportsParamFields } from '../ReportsTypes';

export interface IIswcCreatorExtractProps {
  loading: boolean;
  error?: any;
  getCreatorExtractCachedError?: any;
  extractToFtp: (parmas: IReportsParamFields) => void;
  getDateOfCachedReport: () => void;
  creatorExtractCachedVersion: string;
  email: string;
  extractToFtpSuccess: boolean;
  assignedRoles: IAssignedRoles;
}

export interface IIswcCreatorExtractState {
  showExtractToFtpDiv: boolean;
  paramFields: IReportsParamFields;
}
