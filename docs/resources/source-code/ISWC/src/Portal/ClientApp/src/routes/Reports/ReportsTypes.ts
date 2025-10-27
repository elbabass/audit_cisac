import { IAssignedRoles } from '../../redux/types/RoleTypes';

export interface IReportsState {}

export interface IReportsProps {
  clearReportsError: () => void;
  assignedRoles: IAssignedRoles;
}

export interface IReportsParamFields {
  fromDate?: string;
  toDate?: string;
  agencyName?: string;
  agencyWorkCode?: string;
  transactionSource?: number;
  status?: number;
  mostRecentVersion?: boolean;
  reportType: number;
  agreementFromDate?: string;
  agreementToDate?: string;
  email?: string;
  considerOriginalTitlesOnly?: boolean;
  potentialDuplicatesCreateExtractMode?: boolean;
  creatorNameNumber?: string;
  creatorBaseNumber?: string;
}

export interface IAgencyStatisticsParamFields {
  agencyName: string;
  year: number;
  month: number;
  timePeriod: number;
  transactionSource?: number;
}
