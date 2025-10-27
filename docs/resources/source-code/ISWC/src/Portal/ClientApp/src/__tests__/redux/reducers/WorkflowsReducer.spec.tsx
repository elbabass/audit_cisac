import { reducer } from '../../../redux/reducers/WorkflowsReducer';
import {
  WorkflowsActionTypes,
  GET_WORKFLOWS_REQUEST,
  GET_WORKFLOWS_SUCCESS,
  GET_WORKFLOWS_FAILURE,
  UPDATE_WORKFLOW_REQUEST,
  UPDATE_WORKFLOW_SUCCESS,
  UPDATE_WORKFLOW_FAILURE,
} from '../../../redux/actions/WorkflowsActionTypes';
import { IWorkflow } from '../../../redux/types/IswcTypes';

describe('User Reducer', () => {
  it('should return the initial state', () => {
    expect(reducer(undefined, {} as WorkflowsActionTypes)).toEqual({
      workflows: undefined,
      loading: false,
    });
  });

  it('should handle GET_WORKFLOWS_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: GET_WORKFLOWS_REQUEST,
      }),
    ).toEqual({
      loading: true,
      workflows: undefined,
      error: undefined,
    });
  });

  it('should handle GET_WORKFLOWS_SUCCESS', () => {
    const workflows = {} as IWorkflow[];
    expect(
      reducer(undefined, {
        type: GET_WORKFLOWS_SUCCESS,
        payload: {
          workflows,
        },
      }),
    ).toEqual({
      loading: false,
      workflows,
      error: undefined,
    });
  });

  it('should handle GET_WORKFLOWS_FAILURE', () => {
    const error = 'error';
    expect(
      reducer(undefined, {
        type: GET_WORKFLOWS_FAILURE,
        payload: {
          error,
        },
      }),
    ).toEqual({
      loading: false,
      workflows: undefined,
      error,
    });
  });

  it('should handle UPDATE_WORKFLOW_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: UPDATE_WORKFLOW_REQUEST,
      }),
    ).toEqual({
      updating: true,
      updateSuccessful: undefined,
      loading: false,
      workflows: undefined,
    });
  });

  it('should handle UPDATE_WORKFLOW_SUCCESS', () => {
    const updatedWorkflows = {} as IWorkflow[];
    expect(
      reducer(undefined, {
        type: UPDATE_WORKFLOW_SUCCESS,
        payload: {
          updatedWorkflows,
        },
      }),
    ).toEqual({
      updating: false,
      workflows: updatedWorkflows,
      updateSuccessful: true,
      loading: false,
    });
  });

  it('should handle UPDATE_WORKFLOW_FAILURE', () => {
    const error = 'error';
    expect(
      reducer(undefined, {
        type: UPDATE_WORKFLOW_FAILURE,
        payload: {
          error,
        },
      }),
    ).toEqual({
      loading: false,
      updating: false,
      updatedWorkflows: undefined,
      updateSuccessful: false,
      error,
      workflows: undefined,
    });
  });
});
