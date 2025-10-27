import { connect } from 'react-redux';
import App from './App';
import { Dispatch } from 'redux';
import { ApplicationState } from '../../redux/store/publicPortal';
import { turnOnMaintenanceMode, turnOffMaintenanceMode } from '../../redux/actions/AppActions';

export default connect(
  (state: ApplicationState) => ({
    maintenanceMode: state.appReducer.maintenanceMode,
  }),
  (dispatch: Dispatch) => ({
    turnOnMaintenanceMode: () => dispatch(turnOnMaintenanceMode()),
    turnOffMaintenanceMode: () => dispatch(turnOffMaintenanceMode()),
  }),
)(App);
