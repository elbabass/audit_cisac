import { connect } from 'react-redux';
import { ApplicationState } from '../../redux/store/portal';
import { Dispatch } from 'redux';
import Reports from './Reports';
import { clearReportsError } from '../../redux/actions/ReportsActions';

export default connect(
  (state: ApplicationState) => ({
    assignedRoles: state.appReducer.assignedRoles,
  }),
  (dispatch: Dispatch) => ({
    clearReportsError: () => dispatch<any>(clearReportsError()),
  }),
)(Reports);
