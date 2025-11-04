import { Action, Dispatch } from 'redux';
import { ThunkAction } from 'redux-thunk';
import { ApplicationState } from '../store/portal';
import {
  getWorkflowsRequest,
  getWorkflowsSuccess,
  getWorkflowsFailure,
  updateWorkflowRequest,
  updateWorkflowSucess,
  updateWorkflowsFailure,
} from '../actions/WorkflowsActions';
import { getWorkflows, updateWorkflows } from '../services/WorkflowsService';
import { IWorkflowUpdateRequestBody } from '../../routes/Workflows/WorkflowsGrid/WorkflowGridTypes';
import { turnOnMaintenanceMode } from '../actions/AppActions';
import { IWorkflowSearchModel } from '../types/IswcTypes';

export const getWorkflowsThunk = (
  filters: IWorkflowSearchModel,
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(getWorkflowsRequest());
    return getWorkflows(filters)
      .then((res) => {
        dispatch(getWorkflowsSuccess(res));
      })
      .catch((error) => {
        if (error.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(getWorkflowsFailure(error));
      });
  };
};

export const updateWorkflowThunk = (
  agency?: string,
  body?: IWorkflowUpdateRequestBody[],
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(updateWorkflowRequest());
    return updateWorkflows(agency, body)
      .then((res) => {
        dispatch(updateWorkflowSucess(res));
      })
      .catch((error) => {
        if (error.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(updateWorkflowsFailure(error));
      });
  };
};
