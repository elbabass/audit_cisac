import { IFileAuditReportResult } from '../../../redux/types/ReportTypes';
import { IReportsParamFields } from '../ReportsTypes';

export interface IFileSubmissionAuditState {
  paramFields: IReportsParamFields;
}

export interface IFileSubmissionAuditProps {
  fileSubmissionAuditSearchResults?: IFileAuditReportResult[];
  fileSubmissionAuditSearch: (parmas: IReportsParamFields) => void;
  loading: boolean;
  error?: any;
}
