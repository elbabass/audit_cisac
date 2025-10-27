import React, { memo } from 'react';
import styles from '../Header.module.scss';
import { Navbar, NavbarBrand, NavbarToggler, Collapse, Nav, NavItem } from 'reactstrap';
import { IHeaderProps } from '../HeaderTypes';
import {
  LOGO_ICON,
  SEARCH_PATH,
  SEARCH_ICON_GREY,
  EXTERNAL_LINK_ICON_GREY,
  CISAC_USER_GUIDE,
} from '../../../consts';
import { getStrings } from '../../../configuration/Localization';

const PortalHeader: React.FunctionComponent<IHeaderProps> = ({
  renderHeaderItem,
  isOpen,
  toggle,
}) => {
  const { SEARCH, USER_GUIDE } = getStrings();
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
        <NavbarToggler onClick={toggle} />
        <Collapse isOpen={isOpen} navbar>
          <Nav className="ml-auto" navbar>
            {renderHeaderItem(SEARCH_PATH, SEARCH_ICON_GREY, SEARCH)}
            <NavItem className={styles.headerItem}>
              <a
                target="_blank"
                rel="noopener noreferrer"
                href={CISAC_USER_GUIDE}
                className={`${styles.headerLink} nav-link`}
              >
                <img
                  src={EXTERNAL_LINK_ICON_GREY}
                  className={styles.icon}
                  alt={EXTERNAL_LINK_ICON_GREY}
                  width="16px"
                  height="16px"
                />
                {USER_GUIDE}
              </a>
            </NavItem>
          </Nav>
        </Collapse>
      </Navbar>
    </div>
  );
};

export default memo(PortalHeader);
