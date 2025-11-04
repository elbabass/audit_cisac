import { connect } from 'react-redux';
import SubmissionAudit from './SubmissionAudit';
import { ApplicationState } from '../../../redux/store/portal';
import { Dispatch } from 'redux';
import {
  submissionAuditSearchThunk,
  extractToFtpThunk,
  agencyStatisticsSearchThunk,
} from '../../../redux/thunks/ReportsThunks';
import { IReportsParamFields, IAgencyStatisticsParamFields } from '../ReportsTypes';

export default connect(
  (state: ApplicationState) => ({
    submissionAuditSearchResults: state.reportsReducer.submissionAuditSearchResults,
    agencyStatisticsSearchResultsSubAudit:
      state.reportsReducer.agencyStatisticsSearchResultsSubAudit,
    loading: state.reportsReducer.loading,
    error: state.reportsReducer.error,
    email: state.appReducer.email,
    extractToFtpSuccess: state.reportsReducer.extractToFtpSuccess,
    assignedRoles: state.appReducer.assignedRoles,
  }),
  (dispatch: Dispatch) => ({
    submissionAuditSearch: (params: IReportsParamFields) =>
      dispatch<any>(submissionAuditSearchThunk(params)),
    reportsAgencyStatisticsSearch: (params: IAgencyStatisticsParamFields) =>
      dispatch<any>(agencyStatisticsSearchThunk(params, true)),
    extractToFtp: (params: IReportsParamFields) => dispatch<any>(extractToFtpThunk(params)),
  }),
)(SubmissionAudit);
