import { IAssignedRoles } from '../../../redux/types/RoleTypes';
import { IReportsParamFields } from '../ReportsTypes';

export interface IPotentialDuplicatesProps {
  extractToFtp: (parmas: IReportsParamFields) => void;
  loading: boolean;
  error?: any;
  extractToFtpSuccess: boolean;
  getPotentialDuplicatesCachedError?: any;
  potentialDuplicatesCachedVersion: string;
  getDateOfCachedReport: () => void;
  email: string;
  assignedRoles: IAssignedRoles;
}

export interface IPotentialDuplicatesState {
  showExtractToFtpDiv: boolean;
  paramFields: IReportsParamFields;
}
