import { connect } from 'react-redux';
import { ApplicationState } from '../../../redux/store/portal';
import { Dispatch } from 'redux';
import PotentialDuplicates from './PotentialDuplicates';
import {
  extractToFtpThunk,
  getDateOfCachedPotentialDuplicatesReportThunk,
} from '../../../redux/thunks/ReportsThunks';
import { IReportsParamFields } from '../ReportsTypes';

export default connect(
  (state: ApplicationState) => ({
    loading: state.reportsReducer.loading,
    error: state.reportsReducer.error,
    extractToFtpSuccess: state.reportsReducer.extractToFtpSuccess,
    getPotentialDuplicatesCachedError: state.reportsReducer.getPotentialDuplicatesCachedError,
    potentialDuplicatesCachedVersion: state.reportsReducer.potentialDuplicatesCachedVersion,
    email: state.appReducer.email,
    assignedRoles: state.appReducer.assignedRoles,
  }),
  (dispatch: Dispatch) => ({
    extractToFtp: (params: IReportsParamFields) => dispatch<any>(extractToFtpThunk(params)),
    getDateOfCachedReport: () => dispatch<any>(getDateOfCachedPotentialDuplicatesReportThunk()),
  }),
)(PotentialDuplicates);
