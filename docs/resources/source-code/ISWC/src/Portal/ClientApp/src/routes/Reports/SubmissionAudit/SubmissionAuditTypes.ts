import {
  ISubmissionAuditReportResult,
  IAgencyStatisticsReportResult,
} from '../../../redux/types/ReportTypes';
import { IAssignedRoles } from '../../../redux/types/RoleTypes';
import { IReportsParamFields, IAgencyStatisticsParamFields } from '../ReportsTypes';

export interface ISubmissionAuditState {
  displayView: number;
  paramFields: IReportsParamFields;
}

export interface ISubmissionAuditProps {
  submissionAuditSearchResults?: ISubmissionAuditReportResult[];
  submissionAuditSearch: (parmas: IReportsParamFields) => void;
  agencyStatisticsSearchResultsSubAudit?: IAgencyStatisticsReportResult[];
  reportsAgencyStatisticsSearch: (params: IAgencyStatisticsParamFields) => void;
  extractToFtp: (parmas: IReportsParamFields) => void;
  loading: boolean;
  email: string;
  error?: any;
  extractToFtpSuccess: boolean;
  assignedRoles: IAssignedRoles;
}
