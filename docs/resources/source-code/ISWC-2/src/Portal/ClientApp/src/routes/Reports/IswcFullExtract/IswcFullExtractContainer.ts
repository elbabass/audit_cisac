import { connect } from 'react-redux';
import { ApplicationState } from '../../../redux/store/portal';
import { Dispatch } from 'redux';
import {
  extractToFtpThunk,
  getDateOfCachedIswcFullExtractReportThunk,
} from '../../../redux/thunks/ReportsThunks';
import IswcFullExtract from './IswcFullExtract';
import { IReportsParamFields } from '../ReportsTypes';

export default connect(
  (state: ApplicationState) => ({
    loading: state.reportsReducer.loading,
    error: state.reportsReducer.error,
    getFullExtractCachedError: state.reportsReducer.getFullExtractCachedError,
    fullExtractCachedVersion: state.reportsReducer.fullExtractCachedVersion,
    email: state.appReducer.email,
    extractToFtpSuccess: state.reportsReducer.extractToFtpSuccess,
    assignedRoles: state.appReducer.assignedRoles,
  }),
  (dispatch: Dispatch) => ({
    extractToFtp: (params: IReportsParamFields) => dispatch<any>(extractToFtpThunk(params)),
    getDateOfCachedReport: () => dispatch<any>(getDateOfCachedIswcFullExtractReportThunk()),
  }),
)(IswcFullExtract);
