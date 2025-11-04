import * as React from 'react';
import SubHeader from '../../components/SubHeader/SubHeader';
import TabView from '../../components/TabComponents/TabView/TabView';
import { getStrings } from '../../configuration/Localization';
import { IWebUser } from '../../redux/types/RoleTypes';
import UserProfile from '../UserProfile/UserProfile';
import AccessRequests from './AccessRequests/AccessRequests';
import AgencyUsers from './AgencyUsers/AgencyUsers';
import styles from './ManageUserRoles.module.scss';
import { IManageUserRolesProps, IManageUserRolesState } from './ManageUserRolesTypes';
import { UserRoles } from './../../consts';
class ManageUserRoles extends React.PureComponent<IManageUserRolesProps, IManageUserRolesState> {
  constructor(props: IManageUserRolesProps) {
    super(props);
    this.state = {
      showUserProfile: false,
      user: { email: '', agencyId: '', webUserRoles: [] },
    };
  }

  _setUserAndShowProfile = (user: IWebUser) => {
    this.setState(
      {
        user: user,
      },
      () => this._toggleUserProfile(),
    );
  };

  _toggleUserProfile = () => {
    const { showUserProfile } = this.state;

    this.setState({
      showUserProfile: !showUserProfile,
    });
  };

  _getTabs = () => {
    const { OUTSTANDING_ACCESS_REQUESTS, USERS_IN_AGENCY } = getStrings();
    const {
      users,
      getAllUsers,
      agencyUsersError,
      agencyUsersLoading,
      getAccessRequests,
      usersWithPendingRequests,
      getAccessRequestsError,
      getAccessRequestsLoading,
    } = this.props;

    return [
      {
        text: OUTSTANDING_ACCESS_REQUESTS,
        component: (
          <AccessRequests
            manageRolesAction={this._setUserAndShowProfile}
            getAccessRequests={getAccessRequests}
            usersWithPendingRequests={usersWithPendingRequests}
            error={getAccessRequestsError}
            loading={getAccessRequestsLoading}
          />
        ),
      },
      {
        text: USERS_IN_AGENCY,
        component: (
          <AgencyUsers
            users={users}
            getAllUsers={getAllUsers}
            error={agencyUsersError}
            loading={agencyUsersLoading}
            manageRolesAction={this._setUserAndShowProfile}
          />
        ),
      },
    ];
  };

  render() {
    const { MANAGE_USER_ROLES, USER_PROFILE } = getStrings();
    const { showUserProfile, user } = this.state;
    const {
      getUserProfileRoles,
      updateUserRole,
      userProfileLoading,
      userProfileError,
      userProfileRoles,
      updateRoleError,
    } = this.props;

    return (
      <div className={styles.container}>
        {showUserProfile ? (
          <UserProfile
            header={USER_PROFILE}
            roles={[
              UserRoles.SEARCH_ROLE,
              UserRoles.UPDATE_ROLE,
              UserRoles.REPORT_BASICS_ROLE,
              UserRoles.REPORT_EXTRACT_ROLE,
              UserRoles.REPORT_AGENCY_INTEREST_ROLE,
              UserRoles.REPORT_ISWC_FULL_EXTRACT_ROLE,
              UserRoles.MANAGE_ROLES_ROLE,
            ]}
            user={{
              email: user.email,
              agencyId: user.agencyId,
              webUserRoles: userProfileRoles ? userProfileRoles : [],
            }}
            manageMode
            goBack={this._toggleUserProfile}
            getUserProfileRoles={getUserProfileRoles}
            userProfileLoading={userProfileLoading}
            userProfileError={userProfileError}
            updateUserRole={updateUserRole}
            updateRoleError={updateRoleError}
          />
        ) : (
          <div>
            <SubHeader title={MANAGE_USER_ROLES} />
            <TabView tabs={this._getTabs()} />
          </div>
        )}
      </div>
    );
  }
}

export default ManageUserRoles;
