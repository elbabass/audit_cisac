import * as actions from '../../../redux/actions/WorkflowsActions';
import {
  GET_WORKFLOWS_REQUEST,
  GET_WORKFLOWS_SUCCESS,
  GET_WORKFLOWS_FAILURE,
  UPDATE_WORKFLOW_FAILURE,
  UPDATE_WORKFLOW_SUCCESS,
  UPDATE_WORKFLOW_REQUEST,
} from '../../../redux/actions/WorkflowsActionTypes';
import { IWorkflow } from '../../../redux/types/IswcTypes';

describe('Workflows Actions', () => {
  it('getWorkflowsRequest calls GET_WORKFLOWS_REQUEST', () => {
    const expectedAction = {
      type: GET_WORKFLOWS_REQUEST,
    };
    expect(actions.getWorkflowsRequest()).toEqual(expectedAction);
  });

  it('getWorkflowsSuccess calls GET_WORKFLOWS_SUCCESS', () => {
    const workflows = {} as IWorkflow[];
    const expectedAction = {
      type: GET_WORKFLOWS_SUCCESS,
      payload: {
        workflows,
      },
    };
    expect(actions.getWorkflowsSuccess(workflows)).toEqual(expectedAction);
  });

  it('getWorkflowsFailure calls GET_WORKFLOWS_FAILURE', () => {
    const error = 'error';
    const expectedAction = {
      type: GET_WORKFLOWS_FAILURE,
      payload: {
        error,
      },
    };
    expect(actions.getWorkflowsFailure(error)).toEqual(expectedAction);
  });

  it('updateWorkflowsRequest calls UPDATE_WORKFLOWS_REQUEST', () => {
    const expectedAction = {
      type: UPDATE_WORKFLOW_REQUEST,
    };
    expect(actions.updateWorkflowRequest()).toEqual(expectedAction);
  });

  it('updateWorkflowsSuccess calls UPDATE_WORKFLOWS_SUCCESS', () => {
    const updatedWorkflows = {} as IWorkflow[];
    const expectedAction = {
      type: UPDATE_WORKFLOW_SUCCESS,
      payload: {
        updatedWorkflows,
      },
    };
    expect(actions.updateWorkflowSucess(updatedWorkflows)).toEqual(expectedAction);
  });

  it('updateWorkflowsFailure calls UPDATE_WORKFLOWS_FAILURE', () => {
    const error = 'error';
    const expectedAction = {
      type: UPDATE_WORKFLOW_FAILURE,
      payload: {
        error,
      },
    };
    expect(actions.updateWorkflowsFailure(error)).toEqual(expectedAction);
  });
});
