import { IIswcModel, IAuditHistoryResult } from '../../../redux/types/IswcTypes';
import { RouteComponentProps } from 'react-router-dom';

export interface ISubmissionHistoryProps {
  preferredIswc: IIswcModel;
  getSubmissionHistory: (iswc: string) => void;
  submissionHistory?: IAuditHistoryResult[];
  loading: boolean;
  error: any;
  clearSubmissionHistoryError: () => void;
  router: RouteComponentProps;
}

export interface ISubmissioHistoryState {
  iswc?: string;
}
