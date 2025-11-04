import { connect } from 'react-redux';
import { ApplicationState } from '../../../redux/store/portal';
import { Dispatch } from 'redux';
import { extractToFtpThunk } from '../../../redux/thunks/ReportsThunks';
import PublisherIswcTracking from './PublisherIswcTracking';
import { IReportsParamFields } from '../ReportsTypes';

export default connect(
  (state: ApplicationState) => ({
    loading: state.reportsReducer.loading,
    error: state.reportsReducer.error,
    email: state.appReducer.email,
    extractToFtpSuccess: state.reportsReducer.extractToFtpSuccess,
  }),
  (dispatch: Dispatch) => ({
    extractToFtp: (params: IReportsParamFields) => dispatch<any>(extractToFtpThunk(params)),
  }),
)(PublisherIswcTracking);
