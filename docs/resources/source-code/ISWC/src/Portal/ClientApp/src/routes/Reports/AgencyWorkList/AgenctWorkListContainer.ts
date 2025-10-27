import { connect } from 'react-redux';
import { ApplicationState } from '../../../redux/store/portal';
import { Dispatch } from 'redux';
import {
  extractToFtpThunk,
  getDateOfCachedAgencyWorkListReportThunk,
} from '../../../redux/thunks/ReportsThunks';
import AgencyWorkList from './AgencyWorkList';
import { IReportsParamFields } from '../ReportsTypes';

export default connect(
  (state: ApplicationState) => ({
    loading: state.reportsReducer.loading,
    error: state.reportsReducer.error,
    getAgencyWorkListCachedError: state.reportsReducer.getAgencyWorkListCachedError,
    agencyWorkListCachedVersion: state.reportsReducer.agencyWorkListCachedVersion,
    email: state.appReducer.email,
    extractToFtpSuccess: state.reportsReducer.extractToFtpSuccess,
    assignedRoles: state.appReducer.assignedRoles,
  }),
  (dispatch: Dispatch) => ({
    extractToFtp: (params: IReportsParamFields) => dispatch<any>(extractToFtpThunk(params)),
    getDateOfCachedReport: () => dispatch<any>(getDateOfCachedAgencyWorkListReportThunk()),
  }),
)(AgencyWorkList);
