import { connect } from 'react-redux';
import { ApplicationState } from '../../redux/store/portal';
import { Dispatch } from 'redux';
import ManageUserRoles from './ManageUserRoles';
import {
  getAccessRequestsThunk,
  getAllUsersThunk,
  getUserProfileRolesThunk,
  updateUserRoleThunk,
} from '../../redux/thunks/AppThunks';
import { IWebUser } from '../../redux/types/RoleTypes';

export default connect(
  (state: ApplicationState) => ({
    users: state.appReducer.users,
    userProfileError: state.appReducer.userProfileError,
    userProfileLoading: state.appReducer.userProfileLoading,
    agencyUsersError: state.appReducer.agencyUsersError,
    agencyUsersLoading: state.appReducer.agencyUsersLoading,
    userProfileRoles: state.appReducer.userProfileRoles,
    updateRoleError: state.appReducer.updateRoleError,
    usersWithPendingRequests: state.appReducer.usersWithPendingRequests,
    getAccessRequestsLoading: state.appReducer.getAccessRequestsLoading,
    getAccessRequestsError: state.appReducer.getAccessRequestsError,
  }),
  (dispatch: Dispatch) => ({
    getAllUsers: () => dispatch<any>(getAllUsersThunk()),
    getUserProfileRoles: (user: IWebUser) => dispatch<any>(getUserProfileRolesThunk(user)),
    updateUserRole: (user: IWebUser) => dispatch<any>(updateUserRoleThunk(user)),
    getAccessRequests: () => dispatch<any>(getAccessRequestsThunk()),
  }),
)(ManageUserRoles);
