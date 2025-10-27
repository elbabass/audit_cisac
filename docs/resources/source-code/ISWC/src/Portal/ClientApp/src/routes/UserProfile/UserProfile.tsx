import React from 'react';
import AlertMessage from '../../components/AlertMessage/AlertMessage';
import GoBack from '../../components/GoBack/GoBack';
import Loader from '../../components/Loader/Loader';
import Modal from '../../components/Modal/Modal';
import SubHeader from '../../components/SubHeader/SubHeader';
import { getStrings } from '../../configuration/Localization';
import { IWebUserRole } from '../../redux/types/RoleTypes';
import { _getAgency } from '../../shared/helperMethods';
import AssignedRolesGrid from './AssignedRolesGrid/AssignedRolesGrid';
import styles from './UserProfile.module.scss';
import { IUserProfileProps, IUserProfileState } from './UserProfileTypes';

export default class UserProfile extends React.PureComponent<IUserProfileProps, IUserProfileState> {
  constructor(props: IUserProfileProps) {
    super(props);
    this.state = {
      isModalOpen: false,
    };
  }

  componentDidMount = () => {
    const { manageMode, getLoggedInUserProfileRoles, getUserProfileRoles, user } = this.props;
    if (manageMode) {
      getUserProfileRoles && getUserProfileRoles(user);
    } else {
      if (getLoggedInUserProfileRoles) getLoggedInUserProfileRoles();
    }
  };

  _openModalAndSetRequestedRole = (role: IWebUserRole) => {
    this.setState({
      isModalOpen: true,
      requestedRole: role,
    });
  };

  _closeModal = () => {
    this.setState({
      isModalOpen: false,
    });
  };

  _requestAccess = () => {
    const { requestedRole } = this.state;
    const { requestAccess } = this.props;
    if (requestedRole && requestAccess) {
      requestAccess(requestedRole);
      this._closeModal();
    }
  };

  _updateReason = (text: string) => {
    this.setState((prevState) => ({
      requestedRole: {
        ...prevState.requestedRole!,
        notification: { message: text },
      },
    }));
  };

  renderRequestAccessModal = () => {
    const { isModalOpen } = this.state;
    const { accessRequestLoading } = this.props;
    const { SUBMIT_REQUEST, CANCEL, REQUEST_REASON } = getStrings();

    return (
      <Modal
        isModalOpen={isModalOpen}
        toggleModal={this._closeModal}
        headerText={REQUEST_REASON + ':'}
        rightButtonText={SUBMIT_REQUEST}
        leftButtonText={CANCEL}
        rightButtonAction={this._requestAccess}
        leftButtonAction={this._closeModal}
        type={'input'}
        onChangeInput={this._updateReason}
        loading={accessRequestLoading}
      />
    );
  };

  renderGrid = () => {
    const {
      ASSIGNED_ROLES,
      ERROR_RETRIEVING,
      ERROR_UPDATING,
      ERROR_REQUESTING,
      NO_ROLES_FOUND,
    } = getStrings();
    const {
      roles,
      manageMode,
      userProfileLoading,
      userProfileError,
      user,
      updateRoleError,
      accessRequestError,
      getLoggedInUserProfileRoles,
      updateUserRole,
    } = this.props;

    if (userProfileLoading) {
      return <Loader />;
    } else if (userProfileError) {
      return <AlertMessage message={ERROR_RETRIEVING} type={'error'} />;
    } else if (updateRoleError) {
      return <AlertMessage message={ERROR_UPDATING} type={'error'} />;
    } else if (accessRequestError) {
      return <AlertMessage message={ERROR_REQUESTING} type={'error'} />;
    }

    if (user.webUserRoles.length > 0) {
      return (
        <div>
          <div className={styles.label}>{ASSIGNED_ROLES}:</div>
          <AssignedRolesGrid
            roles={roles}
            manageMode={manageMode}
            requestAccess={this._openModalAndSetRequestedRole}
            getLoggedInUserProfileRoles={getLoggedInUserProfileRoles}
            updateUserRole={updateUserRole}
            user={user}
          />
        </div>
      );
    } else {
      return <AlertMessage message={NO_ROLES_FOUND} type={'error'} />;
    }
  };

  render() {
    const {
      USERNAME,
      NAME_FIELD,
      SOCIETY_CODE,
      GO_BACK_TO_MANAGE_USER_ROLES,
      ACCESS_DENIED,
    } = getStrings();
    const { user, manageMode, header, goBack, router } = this.props;
    const roleAccessMessage = router?.location?.state?.roleAccessMessage;

    return (
      <div className={styles.container}>
        {this.renderRequestAccessModal()}
        <SubHeader title={header} />
        <div className={styles.subDiv}>
          {manageMode && (
            <GoBack text={GO_BACK_TO_MANAGE_USER_ROLES} action={() => goBack && goBack()} />
          )}
          {roleAccessMessage && (
            <div className={styles.alertMessage}>
              <AlertMessage message={ACCESS_DENIED} type={'info'} />
            </div>
          )}
          <div className={styles.topDiv}>
            <div className={styles.topLeftDiv}>
              <div className={styles.label}>{USERNAME}:</div>
              <div>{user.email}</div>
            </div>
            <div className={styles.topRightDiv}>
              <div className={styles.label}>
                {SOCIETY_CODE} / {NAME_FIELD}:
              </div>
              <div>{manageMode ? _getAgency(user.agencyId) : user.agencyId}</div>
            </div>
          </div>
          {this.renderGrid()}
        </div>
      </div>
    );
  }
}
