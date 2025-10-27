import { IWorkflow, IWorkflowSearchModel } from '../../redux/types/IswcTypes';

export interface IWorkflowsProps {
  loading: boolean;
  workflows?: IWorkflow[];
  error?: any;
  updating?: boolean;
  updateSuccessful?: boolean;
  getWorkflows: (filters: IWorkflowSearchModel) => void;
  updateWorkflows: (agency?: string, body?: any) => void;
  clearWorkflowError: () => void;
}

export interface IWorkflowsState {
  statusFilters: IStatusFiltersType;
  showAssigned: boolean;
  showAdvancedSearch: boolean;
  advancedSearchFilters: {
    iswc?: string;
    workCodes?: string;
    agency?: string;
    workflowType?: string;
  };
  dates: {
    fromDate?: string;
    toDate?: string;
  };
  workflowsToDisplay?: IWorkflow[];
  startIndex: number;
  pageLength: number;
  numberOfLastPage: number;
  lastPage: boolean;
}

export interface IWorkflowsHeaderProps {
  statusFilters: {
    pending: boolean;
    approved: boolean;
    rejected: boolean;
  };
  showAssigned?: boolean;
  fromDate?: string;
  toDate?: string;
  loading: boolean;
  updateStatusFilters: (filter: string) => void;
  changeWorkFlowsDisplayed?: () => void;
  updateDates?: (event: any) => void;
}

export interface IStatusFiltersType {
  pending: boolean;
  approved: boolean;
  rejected: boolean;
}
