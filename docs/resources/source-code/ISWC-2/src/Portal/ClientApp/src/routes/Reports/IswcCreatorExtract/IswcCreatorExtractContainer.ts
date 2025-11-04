import { connect } from 'react-redux';
import { ApplicationState } from '../../../redux/store/portal';
import { Dispatch } from 'redux';
import {
  extractToFtpThunk,
  getDateOfCachedIswcCreatorExtractReportThunk,
} from '../../../redux/thunks/ReportsThunks';
import IswcCreatorExtract from './IswcCreatorExtract';
import { IReportsParamFields } from '../ReportsTypes';

export default connect(
  (state: ApplicationState) => ({
    loading: state.reportsReducer.loading,
    error: state.reportsReducer.error,
    getCreatorExtractCachedError: state.reportsReducer.getCreatorExtractCachedError,
    creatorExtractCachedVersion: state.reportsReducer.creatorExtractCachedVersion,
    email: state.appReducer.email,
    extractToFtpSuccess: state.reportsReducer.extractToFtpSuccess,
    assignedRoles: state.appReducer.assignedRoles,
  }),
  (dispatch: Dispatch) => ({
    extractToFtp: (params: IReportsParamFields) => dispatch<any>(extractToFtpThunk(params)),
    getDateOfCachedReport: () => dispatch<any>(getDateOfCachedIswcCreatorExtractReportThunk()),
  }),
)(IswcCreatorExtract);
