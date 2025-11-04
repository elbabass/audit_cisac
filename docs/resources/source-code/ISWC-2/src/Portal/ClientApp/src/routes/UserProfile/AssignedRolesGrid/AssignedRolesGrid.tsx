import React, { memo } from 'react';
import GridIconCell from '../../../components/GridComponents/GridIconCell/GridIconCell';
import GridTextCell from '../../../components/GridComponents/GridTextCell/GridTextCell';
import { CHECKMARK_ICON_BLACK } from '../../../consts';
import { IAssignedRolesGridProps } from './AssignedRolesGridTypes';
import styles from './AssignedRolesGrid.module.scss';
import AssignedRolesGridActionCell from './AssignedRolesGridActionCell/AssignedRolesGridActionCell';
import { getStrings } from '../../../configuration/Localization';
import { getRoleText } from '../../../shared/helperMethods';

const AssignedRolesGrid: React.FunctionComponent<IAssignedRolesGridProps> = ({
  roles,
  manageMode,
  user,
  requestAccess,
  updateUserRole,
}) => {
  const {
    PENDING_REQUEST,
    REQUEST_ACCESS,
    REMOVE_ROLE,
    ACCESS_REQUESTED,
    TO_ASSIGN_THIS_ROLE,
    ASSIGN_ROLE,
    APPROVE,
    DECLINE,
  } = getStrings();

  const _checkIfRoleIsApproved = (role: number): boolean => {
    for (let x = 0; x < user.webUserRoles.length; x++) {
      if (role === user.webUserRoles[x].role && user.webUserRoles[x].isApproved) {
        return true;
      }
    }

    return false;
  };

  const _getIconAndActionCell = (role: number) => {
    let elements: JSX.Element[] = [];
    user.webUserRoles.forEach((userRole) => {
      if (userRole.role === role) {
        if (userRole.isApproved) {
          elements = [<GridIconCell icon={CHECKMARK_ICON_BLACK} alt={'Checkmark Icon'} />];
        } else
          elements = [
            <GridTextCell text={'-'} />,
            <AssignedRolesGridActionCell topText={`(${PENDING_REQUEST})`} />,
          ];
      }
    });

    if (elements.length > 0) return elements;

    if (!_checkIfRoleIsApproved(3) && [4, 5, 6].includes(role)) {
      return [
        <GridTextCell text={'-'} />,
        <AssignedRolesGridActionCell topText={`(${TO_ASSIGN_THIS_ROLE})`} />,
      ];
    } else {
      return [
        <GridTextCell text={'-'} />,
        <AssignedRolesGridActionCell
          topText={REQUEST_ACCESS}
          topAction={() => requestAccess && requestAccess({ role: role })}
        />,
      ];
    }
  };

  const _getIconAndActionCellManageMode = (role: number) => {
    let elements: JSX.Element[] = [];
    user.webUserRoles.forEach((userRole) => {
      if (userRole.role === role) {
        if (userRole.isApproved) {
          elements = [<GridIconCell icon={CHECKMARK_ICON_BLACK} alt={'Checkmark Icon'} />];
          if (role !== 1)
            elements.push(
              <AssignedRolesGridActionCell
                topText={REMOVE_ROLE}
                topAction={() =>
                  updateUserRole &&
                  updateUserRole({
                    email: user.email,
                    agencyId: user.agencyId,
                    webUserRoles: [{ role: role, status: false, isApproved: false }],
                  })
                }
              />,
            );
        } else
          elements = [
            <GridTextCell text={ACCESS_REQUESTED} />,
            <AssignedRolesGridActionCell
              topText={APPROVE}
              topAction={() =>
                updateUserRole &&
                updateUserRole({
                  email: user.email,
                  agencyId: user.agencyId,
                  webUserRoles: [{ role: role, status: true, isApproved: true }],
                })
              }
              bottomText={DECLINE}
              bottomAction={() =>
                updateUserRole &&
                updateUserRole({
                  email: user.email,
                  agencyId: user.agencyId,
                  webUserRoles: [{ role: role, status: false, isApproved: false }],
                })
              }
            />,
          ];
      }
    });

    if (elements.length > 0) return elements;

    if (!_checkIfRoleIsApproved(3) && [4, 5, 6].includes(role)) {
      return [
        <GridTextCell text={'-'} />,
        <AssignedRolesGridActionCell topText={`(${TO_ASSIGN_THIS_ROLE})`} />,
      ];
    } else {
      return [
        <GridTextCell text={'-'} />,
        <AssignedRolesGridActionCell
          topText={ASSIGN_ROLE}
          topAction={() =>
            updateUserRole &&
            updateUserRole({
              email: user.email,
              agencyId: user.agencyId,
              webUserRoles: [{ role: role, status: true, isApproved: true }],
            })
          }
        />,
      ];
    }
  };

  const renderRows = () => {
    return (
      <tbody>
        {roles.map((role) => (
          <tr className={styles.row} key={role}>
            <td>
              <GridTextCell text={getRoleText(role)} />
            </td>
            <td>
              {manageMode
                ? _getIconAndActionCellManageMode(role)[0]
                : _getIconAndActionCell(role)[0]}
            </td>
            <td>
              {manageMode
                ? _getIconAndActionCellManageMode(role)[1]
                : _getIconAndActionCell(role)[1]}
            </td>
          </tr>
        ))}
      </tbody>
    );
  };

  return <table>{renderRows()}</table>;
};

export default memo(AssignedRolesGrid);
