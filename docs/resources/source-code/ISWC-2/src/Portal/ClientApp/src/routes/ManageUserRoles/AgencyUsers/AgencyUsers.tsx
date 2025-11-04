import * as React from 'react';
import styles from './AgencyUsers.module.scss';
import Grid from '../../../components/GridComponents/Grid/Grid';
import {
  MANAGE_ROLES_ACTION_FIELD,
  UPDATE_FIELD,
  REPORT_BASICS_FIELD,
  USERNAME_FIELD,
  EDIT_ICON,
  MANAGE_ROLES_ACTION,
  SEARCH_FIELD,
  REPORT_AGENCY_INTEREST_FIELD,
  REPORT_ISWC_FULL_FIELD,
  MANAGE_ROLES_FIELD,
  CHECKMARK_ICON_BLACK,
  REPORT_EXTRACT_FIELD,
  UserRoles,
} from '../../../consts';
import { IGridHeaderCell } from '../../../components/GridComponents/Grid/GridTypes';
import GridTextCell from '../../../components/GridComponents/GridTextCell/GridTextCell';
import GridIconCell from '../../../components/GridComponents/GridIconCell/GridIconCell';
import { IAgencyUsersProps, IAgencyUsersState } from './AgencyUsersTypes';
import { getStrings } from '../../../configuration/Localization';
import BasicFormInput from '../../../components/FormInput/BasicFormInput';
import { checkRoles } from '../../../shared/helperMethods';
import { IWebUserRole } from '../../../redux/types/RoleTypes';
import AlertMessage from '../../../components/AlertMessage/AlertMessage';
import Loader from '../../../components/Loader/Loader';

class AgencyUsers extends React.PureComponent<IAgencyUsersProps, IAgencyUsersState> {
  constructor(props: IAgencyUsersProps) {
    super(props);
    this.state = {
      username: '',
      users: props.users,
    };
  }

  componentDidMount = () => {
    const { getAllUsers } = this.props;
    getAllUsers();
  };

  componentDidUpdate = (prevProps: IAgencyUsersProps) => {
    if (prevProps.users.length === 0 && this.props.users.length > 0) {
      this.setState({
        users: this.props.users,
      });
    }
  };

  _updateUsername = (event: any) => {
    let value = event.target.value;
    this.setState(
      {
        username: value,
      },
      () => this._filterRows(),
    );
  };

  _filterRows = () => {
    const { username } = this.state;
    const { users } = this.props;
    let filteredUsers = [...users];
    filteredUsers = filteredUsers.filter((user) => user.email.includes(username));

    this.setState({
      users: filteredUsers,
    });
  };

  _getGridHeaderCells = (): IGridHeaderCell[] => {
    const {
      USERNAME,
      SEARCH,
      UPDATE,
      REPORT_BASICS,
      REPORT_EXTRACTS,
      REPORT_AGENCY_INTEREST,
      REPORT_ISWC_FULL,
      MANAGE_ROLES,
    } = getStrings();

    return [
      { text: USERNAME, field: USERNAME_FIELD },
      { text: SEARCH, field: SEARCH_FIELD },
      { text: UPDATE, field: UPDATE_FIELD },
      { text: REPORT_BASICS, field: REPORT_BASICS_FIELD },
      { text: REPORT_EXTRACTS, field: REPORT_EXTRACT_FIELD },
      { text: REPORT_AGENCY_INTEREST, field: REPORT_AGENCY_INTEREST_FIELD },
      { text: REPORT_ISWC_FULL, field: REPORT_ISWC_FULL_FIELD },
      { text: MANAGE_ROLES, field: MANAGE_ROLES_FIELD },
      { field: MANAGE_ROLES_ACTION_FIELD },
    ];
  };

  _getCell = (role: number, userRoles: IWebUserRole[]) => {
    if (checkRoles(role, userRoles)) {
      return <GridIconCell icon={CHECKMARK_ICON_BLACK} alt={'Checkmark Icon'} />;
    }

    return <GridTextCell text={'-'} />;
  };

  _getGridRows = () => {
    const { MANAGE_ROLES } = getStrings();
    const { users } = this.state;
    const { manageRolesAction } = this.props;

    return users.map((user, index) => {
      return {
        rowId: index,
        cells: [
          {
            element: <GridTextCell text={user.email} />,
            field: USERNAME_FIELD,
          },
          {
            element: this._getCell(UserRoles.SEARCH_ROLE, user.webUserRoles),
            field: SEARCH_FIELD,
          },
          {
            element: this._getCell(UserRoles.UPDATE_ROLE, user.webUserRoles),
            field: UPDATE_FIELD,
          },
          {
            element: this._getCell(UserRoles.REPORT_BASICS_ROLE, user.webUserRoles),
            field: REPORT_BASICS_FIELD,
          },
          {
            element: this._getCell(UserRoles.REPORT_EXTRACT_ROLE, user.webUserRoles),
            field: REPORT_EXTRACT_FIELD,
          },
          {
            element: this._getCell(UserRoles.REPORT_AGENCY_INTEREST_ROLE, user.webUserRoles),
            field: REPORT_AGENCY_INTEREST_FIELD,
          },
          {
            element: this._getCell(UserRoles.REPORT_ISWC_FULL_EXTRACT_ROLE, user.webUserRoles),
            field: REPORT_ISWC_FULL_FIELD,
          },
          {
            element: this._getCell(UserRoles.MANAGE_ROLES_ROLE, user.webUserRoles),
            field: MANAGE_ROLES_FIELD,
          },
          {
            element: (
              <div onClick={() => manageRolesAction(user)}>
                <GridIconCell
                  text={MANAGE_ROLES}
                  icon={EDIT_ICON}
                  alt={'Edit Icon'}
                  clickable
                  id={MANAGE_ROLES}
                />
              </div>
            ),
            field: MANAGE_ROLES_ACTION_FIELD,
            action: MANAGE_ROLES_ACTION,
          },
        ],
      };
    });
  };

  renderUsers = () => {
    const { error, loading } = this.props;
    const { ERROR_RETRIEVING } = getStrings();
    if (loading) {
      return <Loader />;
    } else if (error) {
      return <AlertMessage message={ERROR_RETRIEVING} type={'error'} />;
    }

    return (
      <Grid headerCells={this._getGridHeaderCells()} gridRows={this._getGridRows()} pagination />
    );
  };

  render() {
    const { AGENCY_USERS_TEXT, SEARCH_BY_USERNAME } = getStrings();
    const { username } = this.state;

    return (
      <div className={styles.container}>
        <div className={styles.text}>{AGENCY_USERS_TEXT}</div>
        <div className={styles.input}>
          <BasicFormInput
            name={'username'}
            onChange={this._updateUsername}
            placeholder={SEARCH_BY_USERNAME}
            value={username}
          />
        </div>
        {this.renderUsers()}
      </div>
    );
  }
}

export default AgencyUsers;
