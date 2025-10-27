import { connect } from 'react-redux';
import FileSubmissionAudit from './FileSubmissionAudit';
import { ApplicationState } from '../../../redux/store/portal';
import { Dispatch } from 'redux';
import { fileSubmissionAuditSearchThunk } from '../../../redux/thunks/ReportsThunks';
import { IReportsParamFields } from '../ReportsTypes';

export default connect(
  (state: ApplicationState) => ({
    fileSubmissionAuditSearchResults: state.reportsReducer.fileSubmissionAuditSearchResults,
    loading: state.reportsReducer.loading,
    error: state.reportsReducer.error,
  }),
  (dispatch: Dispatch) => ({
    fileSubmissionAuditSearch: (params: IReportsParamFields) =>
      dispatch<any>(fileSubmissionAuditSearchThunk(params)),
  }),
)(FileSubmissionAudit);
