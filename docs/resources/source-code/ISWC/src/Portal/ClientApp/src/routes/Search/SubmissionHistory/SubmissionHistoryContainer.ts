import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import SubmissionHistory from './SubmissionHistory';
import { RouteComponentProps } from 'react-router-dom';
import { ApplicationState } from '../../../redux/store/portal';
import { getSubmissionHistoryThunk } from '../../../redux/thunks/SubmissionHistoryThunks';
import { clearSubmissionHistoryError } from '../../../redux/actions/SubmissionHistoryActions';

export default connect(
  (state: ApplicationState, routerState: RouteComponentProps) => ({
    preferredIswc: routerState.location.state?.preferredIswc,
    submissionHistory: state.submissionHistoryReducer.submissionHistory,
    loading: state.submissionHistoryReducer.loading,
    error: state.submissionHistoryReducer.error,
    router: routerState,
  }),
  (dispatch: Dispatch) => ({
    getSubmissionHistory: (iswc: string) => dispatch<any>(getSubmissionHistoryThunk(iswc)),
    clearSubmissionHistoryError: () => dispatch(clearSubmissionHistoryError()),
  }),
)(SubmissionHistory);
