import { IReportsParamFields } from '../ReportsTypes';

export interface IAgencyInterestExtractState {
  showExtractToFtpDiv: boolean;
  paramFields: IReportsParamFields;
}

export interface IAgencyInterestExtractProps {
  extractToFtp: (parmas: IReportsParamFields) => void;
  loading: boolean;
  email: string;
  error?: any;
  extractToFtpSuccess: boolean;
}
