import { IAgencyStatisticsParamFields } from '../ReportsTypes';
import { IAgencyStatisticsReportResult } from '../../../redux/types/ReportTypes';

export interface IAgencyStatisticsState {
  paramFields: IAgencyStatisticsParamFields;
}

export interface IAgencyStatisticsProps {
  loading: boolean;
  reportsAgencyStatisticsSearch: (params: IAgencyStatisticsParamFields) => void;
  agencyStatisticsSearchResults?: IAgencyStatisticsReportResult[];
  error?: any;
}
