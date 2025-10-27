import { connect } from 'react-redux';
import App from './App';
import { Dispatch } from 'redux';
import { ApplicationState } from '../../redux/store/portal';
import {
  turnOnMaintenanceMode,
  turnOffMaintenanceMode,
  setUserEmail,
  setUserAgency,
  setAssignedRoles,
} from '../../redux/actions/AppActions';
import { IAssignedRoles } from '../../redux/types/RoleTypes';

export default connect(
  (state: ApplicationState) => ({
    maintenanceMode: state.appReducer.maintenanceMode,
    assignedRoles: state.appReducer.assignedRoles,
  }),
  (dispatch: Dispatch) => ({
    turnOnMaintenanceMode: () => dispatch(turnOnMaintenanceMode()),
    turnOffMaintenanceMode: () => dispatch(turnOffMaintenanceMode()),
    setUserEmail: (email: string) => dispatch(setUserEmail(email)),
    setUserAgency: (agency: string) => dispatch(setUserAgency(agency)),
    setAssignedRoles: (assignedRoles: IAssignedRoles) => dispatch(setAssignedRoles(assignedRoles)),
  }),
)(App);
