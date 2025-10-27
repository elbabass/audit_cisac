import { IWorkflow } from '../../../redux/types/IswcTypes';
import { IStatusFiltersType } from '../WorkflowsTypes';

export interface IWorkflowsGridProps {
  workflows: IWorkflow[];
  updateWorkflows: (agency?: string, body?: IWorkflowUpdateRequestBody[]) => void;
  showAssigned: boolean;
  updating?: boolean;
  updateSuccessful?: boolean;
  error: any;
  search: () => void;
  filters: IStatusFiltersType;
  hideSelectButtons?: boolean;
  rowsPerPage?: number;
  startIndex?: number;
  onClickNext?: () => void;
  onClickPrevious?: () => void;
  changeRowsPerPage?: (rowsPerPage: number) => void;
  numberOfLastPage?: number;
  lastPage?: boolean;
  setNumberOfLastPage?: (numberOfLastPage: number) => void;
  setLastPage?: (lastPage: boolean) => void;
}

export interface IWorkflowsGridState {
  selectedRows: number[];
  allSelected: boolean;
  workflowsToUpdate: IWorkflowUpdateRequestBody[];
  isModalOpen: boolean;
  responseModalOpen: boolean;
  statusToUpdateTo: string;
  updateSuccessMessage: string;
  workflowMessage?: string;
}

export interface IWorkflowUpdateRequestBody {
  taskId: number;
  workflowType: string;
  status: string;
  workflowMessage?: string;
}
