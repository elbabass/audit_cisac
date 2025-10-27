import * as React from 'react';
import { IAccessRequestsProps, IAccessRequestsState } from './AccessRequestsTypes';
import styles from './AccessRequests.module.scss';
import Grid from '../../../components/GridComponents/Grid/Grid';
import {
  DATE_FIELD,
  MANAGE_ROLES_ACTION_FIELD,
  REASON_FIELD,
  ROLE_REQUESTED_FIELD,
  USERNAME_FIELD,
  EDIT_ICON,
  MANAGE_ROLES_ACTION,
} from '../../../consts';
import { IGridHeaderCell, IGridRow } from '../../../components/GridComponents/Grid/GridTypes';
import GridTextCell from '../../../components/GridComponents/GridTextCell/GridTextCell';
import GridIconCell from '../../../components/GridComponents/GridIconCell/GridIconCell';
import { getStrings } from '../../../configuration/Localization';
import { formatDateString, getRoleText } from '../../../shared/helperMethods';
import Loader from '../../../components/Loader/Loader';
import AlertMessage from '../../../components/AlertMessage/AlertMessage';

export default class AccessRequests extends React.PureComponent<
  IAccessRequestsProps,
  IAccessRequestsState
> {
  componentDidMount = () => {
    const { getAccessRequests } = this.props;
    getAccessRequests();
  };

  _getGridHeaderCells = (): IGridHeaderCell[] => {
    const { USERNAME, DATE_FIELD, ROLE_REQUESTED, REASON_FIELD } = getStrings();
    return [
      { text: USERNAME, field: USERNAME_FIELD },
      { text: DATE_FIELD, field: DATE_FIELD },
      { text: ROLE_REQUESTED, field: ROLE_REQUESTED_FIELD },
      { text: REASON_FIELD, field: REASON_FIELD },
      { field: MANAGE_ROLES_ACTION_FIELD },
    ];
  };

  _getGridRows = () => {
    const { MANAGE_ROLES } = getStrings();
    const { manageRolesAction, usersWithPendingRequests } = this.props;
    var rows: IGridRow[] = [];
    var rowCount = 0;

    for (let x = 0; x < usersWithPendingRequests.length; x++) {
      for (let i = 0; i < usersWithPendingRequests[x].webUserRoles.length; i++) {
        rows.push({
          rowId: rowCount,
          cells: [
            {
              element: <GridTextCell text={usersWithPendingRequests[x].email} />,
              field: USERNAME_FIELD,
            },
            {
              element: (
                <GridTextCell
                  text={
                    usersWithPendingRequests[x]?.webUserRoles[i].requestedDate &&
                    formatDateString(usersWithPendingRequests[x]?.webUserRoles[i].requestedDate!)
                  }
                />
              ),
              field: DATE_FIELD,
            },
            {
              element: (
                <GridTextCell
                  text={getRoleText(usersWithPendingRequests[x].webUserRoles[i].role)}
                />
              ),
              field: ROLE_REQUESTED_FIELD,
            },
            {
              element: (
                <GridTextCell
                  text={usersWithPendingRequests[x].webUserRoles[i].notification?.message}
                />
              ),
              field: REASON_FIELD,
            },
            {
              element: (
                <div onClick={() => manageRolesAction(usersWithPendingRequests[x])}>
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
        });
      }
    }

    return rows;
  };

  renderGrid = () => {
    const { loading, error } = this.props;
    const { ERROR_RETRIEVING } = getStrings();
    if (loading) {
      return <Loader />;
    } else if (error) {
      return <AlertMessage message={ERROR_RETRIEVING} type={'error'} />;
    }

    return <Grid headerCells={this._getGridHeaderCells()} gridRows={this._getGridRows()} />;
  };

  render() {
    const { ACCESS_REQUEST_TEXT } = getStrings();

    return (
      <div className={styles.container}>
        <div className={styles.text}>{ACCESS_REQUEST_TEXT}</div>
        {this.renderGrid()}
      </div>
    );
  }
}
