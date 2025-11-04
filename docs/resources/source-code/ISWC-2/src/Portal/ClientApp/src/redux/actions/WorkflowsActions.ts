import {
  GET_WORKFLOWS_REQUEST,
  GET_WORKFLOWS_SUCCESS,
  GET_WORKFLOWS_FAILURE,
  UPDATE_WORKFLOW_REQUEST,
  UPDATE_WORKFLOW_SUCCESS,
  UPDATE_WORKFLOW_FAILURE,
  WorkflowsActionTypes,
  CLEAR_WORKFLOW_ERROR,
} from './WorkflowsActionTypes';
import { IWorkflow } from '../types/IswcTypes';

export function getWorkflowsRequest(): WorkflowsActionTypes {
  return {
    type: GET_WORKFLOWS_REQUEST,
  };
}

export function getWorkflowsSuccess(workflows: IWorkflow[]): WorkflowsActionTypes {
  return {
    type: GET_WORKFLOWS_SUCCESS,
    payload: {
      workflows,
    },
  };
}

export function getWorkflowsFailure(error: any): WorkflowsActionTypes {
  return {
    type: GET_WORKFLOWS_FAILURE,
    payload: {
      error,
    },
  };
}

export function updateWorkflowRequest(): WorkflowsActionTypes {
  return {
    type: UPDATE_WORKFLOW_REQUEST,
  };
}

export function updateWorkflowSucess(updatedWorkflows: IWorkflow[]): WorkflowsActionTypes {
  return {
    type: UPDATE_WORKFLOW_SUCCESS,
    payload: {
      updatedWorkflows,
    },
  };
}

export function updateWorkflowsFailure(error: any): WorkflowsActionTypes {
  return {
    type: UPDATE_WORKFLOW_FAILURE,
    payload: {
      error,
    },
  };
}

export function clearWorkflowError(): WorkflowsActionTypes {
  return {
    type: CLEAR_WORKFLOW_ERROR,
  };
}
