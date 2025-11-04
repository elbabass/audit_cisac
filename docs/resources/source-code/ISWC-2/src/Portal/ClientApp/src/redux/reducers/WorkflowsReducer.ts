import { Reducer } from 'redux';
import { IWorkflow } from '../types/IswcTypes';
import {
  WorkflowsActionTypes,
  GET_WORKFLOWS_REQUEST,
  GET_WORKFLOWS_SUCCESS,
  GET_WORKFLOWS_FAILURE,
  UPDATE_WORKFLOW_REQUEST,
  UPDATE_WORKFLOW_SUCCESS,
  UPDATE_WORKFLOW_FAILURE,
} from '../actions/WorkflowsActionTypes';

export interface IWorkflowsReducerState {
  workflows?: IWorkflow[];
  loading: boolean;
  updating?: boolean;
  error?: any;
  updateSuccessful?: boolean;
}

const initialState: IWorkflowsReducerState = {
  workflows: undefined,
  loading: false,
};

export const reducer: Reducer<IWorkflowsReducerState> = (
  state = initialState,
  action: WorkflowsActionTypes,
): IWorkflowsReducerState => {
  if (state === undefined) {
    return initialState;
  }
  switch (action.type) {
    case GET_WORKFLOWS_REQUEST:
      return {
        ...state,
        loading: true,
        workflows: undefined,
        error: undefined,
      };
    case GET_WORKFLOWS_SUCCESS:
      const { workflows } = action.payload;
      return {
        ...state,
        loading: false,
        workflows,
      };
    case GET_WORKFLOWS_FAILURE:
      const { error } = action.payload;
      return {
        ...state,
        loading: false,
        error,
      };
    case UPDATE_WORKFLOW_REQUEST:
      return {
        ...state,
        updating: true,
        updateSuccessful: undefined,
      };
    case UPDATE_WORKFLOW_SUCCESS:
      const { updatedWorkflows } = action.payload;
      return {
        ...state,
        updating: false,
        workflows: updatedWorkflows,
        updateSuccessful: true,
      };
    case UPDATE_WORKFLOW_FAILURE:
      return {
        ...state,
        updating: false,
        error: action.payload.error,
        updateSuccessful: false,
      };

    default:
      return { ...state };
  }
};
