import React, { memo } from 'react';
import styles from '../Header.module.scss';
import { Navbar, NavbarBrand, NavItem, NavbarToggler, Collapse, Nav } from 'reactstrap';
import { Link } from 'react-router-dom';
import { IHeaderProps } from '../HeaderTypes';
import {
  LOGO_ICON,
  SEARCH_PATH,
  SUBMISSION_PATH,
  WORKFLOWS_PATH,
  SETTINGS_ICON,
  SEARCH_ICON_GREY,
  ADD_NEW_ICON,
  WORK_FLOW_ICON,
  REPORTS_PATH,
  REPORT_ICON,
  EXTERNAL_LINK_ICON_GREY,
  AGENCY_PORTAL_USER_GUIDE,
  MANAGE_USER_ROLES_PATH,
  USER_PROFILE_PATH,
} from '../../../consts';
import { getStrings } from '../../../configuration/Localization';

const PortalHeader: React.FunctionComponent<IHeaderProps> = ({
  renderHeaderItem,
  isOpen,
  showSettings,
  toggle,
  toggleSettings,
  logout,
  assignedRoles,
}) => {
  const {
    SEARCH,
    NEW_SUBMISSION,
    WORKFLOWS,
    REPORTS,
    SETTINGS,
    LOGOUT,
    USER_GUIDE,
    MY_PROFILE,
    MANAGE_USER_ROLES,
  } = getStrings();

  return (
    <div>
      <Navbar dark expand="sm" className={styles.header}>
        <NavbarBrand href="/">
          <img
            src={LOGO_ICON}
            className={styles.headerLogo}
            alt={'ISWC Network logo'}
            width="150px"
            height="43px"
          />
        </NavbarBrand>
        <NavbarToggler onClick={toggle} className={styles.toggler} />
        <Collapse isOpen={isOpen} navbar>
          <Nav className="ml-auto" navbar>
            {assignedRoles?.search && renderHeaderItem(SEARCH_PATH, SEARCH_ICON_GREY, SEARCH)}
            {assignedRoles?.update &&
              renderHeaderItem(SUBMISSION_PATH, ADD_NEW_ICON, NEW_SUBMISSION)}
            {assignedRoles?.update && renderHeaderItem(WORKFLOWS_PATH, WORK_FLOW_ICON, WORKFLOWS)}
            {assignedRoles?.reportBasics && renderHeaderItem(REPORTS_PATH, REPORT_ICON, REPORTS)}
            {renderHeaderItem(AGENCY_PORTAL_USER_GUIDE, EXTERNAL_LINK_ICON_GREY, USER_GUIDE, true)}
            <NavItem className={styles.settingsItem}>
              <div className={`${styles.settingsLink} nav-link`} onClick={toggleSettings}>
                <img
                  src={SETTINGS_ICON}
                  className={styles.settingsIcon}
                  alt={'settings icon'}
                  height="16px"
                  width="16px"
                />
                <div className={styles.settingsText}>{SETTINGS}</div>
                {showSettings && (
                  <div className={styles.settingsDropdown}>
                    <Link className={`${styles.dropdownButton} nav-link`} to={USER_PROFILE_PATH}>
                      {MY_PROFILE}
                    </Link>
                    {assignedRoles?.manageRoles && (
                      <Link
                        className={`${styles.dropdownButton} nav-link`}
                        to={MANAGE_USER_ROLES_PATH}
                      >
                        {MANAGE_USER_ROLES}
                      </Link>
                    )}
                    <div className={styles.dropdownButton} onClick={logout}>
                      {LOGOUT}
                    </div>
                  </div>
                )}
              </div>
            </NavItem>
          </Nav>
        </Collapse>
      </Navbar>
    </div>
  );
};

export default memo(PortalHeader);
