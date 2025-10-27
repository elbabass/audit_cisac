import { IWorkflow } from '../types/IswcTypes';

export const GET_WORKFLOWS_REQUEST = 'GET_WORKFLOWS_REQUEST';
export const GET_WORKFLOWS_SUCCESS = 'GET_WORKFLOWS_SUCCESS';
export const GET_WORKFLOWS_FAILURE = 'GET_WORKFLOWS_FAILURE';

export const UPDATE_WORKFLOW_REQUEST = 'UPDATE_WORKFLOW_REQUEST';
export const UPDATE_WORKFLOW_SUCCESS = 'UPDATE_WORKFLOW_SUCCESS';
export const UPDATE_WORKFLOW_FAILURE = 'UPDATE_WORKFLOW_FAILURE';

export const CLEAR_WORKFLOW_ERROR = 'CLEAR_WORKFLOW_ERROR';

export interface GetWorkflowsRequest {
  type: typeof GET_WORKFLOWS_REQUEST;
}

export interface GetWorkflowsSuccess {
  type: typeof GET_WORKFLOWS_SUCCESS;
  payload: {
    workflows: IWorkflow[];
  };
}

export interface GetWorkflowsFailure {
  type: typeof GET_WORKFLOWS_FAILURE;
  payload: {
    error: any;
  };
}

export interface UpdateWorkflowRequest {
  type: typeof UPDATE_WORKFLOW_REQUEST;
}

export interface UpdateWorkflowSuccess {
  type: typeof UPDATE_WORKFLOW_SUCCESS;
  payload: {
    updatedWorkflows: IWorkflow[];
  };
}

export interface UpdateWorkflowFailure {
  type: typeof UPDATE_WORKFLOW_FAILURE;
  payload: {
    error: any;
  };
}

export interface ClearWorkflowError {
  type: typeof CLEAR_WORKFLOW_ERROR;
}

export type WorkflowsActionTypes =
  | GetWorkflowsRequest
  | GetWorkflowsSuccess
  | GetWorkflowsFailure
  | UpdateWorkflowRequest
  | UpdateWorkflowSuccess
  | UpdateWorkflowFailure
  | ClearWorkflowError;
