import { IAgencyStatisticsReportResult } from '../../../../redux/types/ReportTypes';

export interface IBarChartState {}

export interface IBarChartProps {
  agencyStatisticsSearchResults: IAgencyStatisticsReportResult[];
  numberOfDaysInMonth: number;
}
