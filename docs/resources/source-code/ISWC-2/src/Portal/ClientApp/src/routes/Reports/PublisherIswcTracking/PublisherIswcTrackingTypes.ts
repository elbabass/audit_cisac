import { IReportsParamFields } from '../ReportsTypes';

export interface IPublisherIswcTrackingState {
  showExtractToFtpDiv: boolean;
  paramFields: IReportsParamFields;
}

export interface IPublisherIswcTrackingProps {
  loading: boolean;
  error?: any;
  extractToFtp: (parmas: IReportsParamFields) => void;
  email: string;
  extractToFtpSuccess: boolean;
}
