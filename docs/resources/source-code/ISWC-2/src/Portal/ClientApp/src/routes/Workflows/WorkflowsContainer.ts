import { connect } from 'react-redux';
import Workflows from './Workflows';
import { ApplicationState } from '../../redux/store/portal';
import { Dispatch } from 'redux';
import { getWorkflowsThunk, updateWorkflowThunk } from '../../redux/thunks/WorkflowsThunk';
import { clearWorkflowError } from '../../redux/actions/WorkflowsActions';
import { IWorkflowSearchModel } from '../../redux/types/IswcTypes';
import { IWorkflowUpdateRequestBody } from './WorkflowsGrid/WorkflowGridTypes';

export default connect(
  (state: ApplicationState) => ({
    loading: state.workflowsReducer.loading,
    workflows: state.workflowsReducer.workflows,
    error: state.workflowsReducer.error,
    updating: state.workflowsReducer.updating,
    updateSuccessful: state.workflowsReducer.updateSuccessful,
  }),
  (dispatch: Dispatch) => ({
    getWorkflows: (filters: IWorkflowSearchModel) => {
      dispatch<any>(getWorkflowsThunk(filters));
    },
    updateWorkflows: (agency?: string, body?: IWorkflowUpdateRequestBody[]) => {
      dispatch<any>(updateWorkflowThunk(agency, body));
    },
    clearWorkflowError: () => dispatch(clearWorkflowError()),
  }),
)(Workflows);
