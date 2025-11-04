import React, { PureComponent } from 'react';
import { NavItem } from 'reactstrap';
import { Link } from 'react-router-dom';
import styles from './Header.module.scss';
import { IHeaderState } from './HeaderTypes';
import { logOutUser } from '../../App/Portal/LogOut';
import PortalHeader from './PortalHeader/PortalHeader';
import { PUBLIC_MODE } from '../../consts';
import PublicPortalHeader from './PublicPortalHeader/PublicPortalHeader';
import { IAssignedRoles } from '../../redux/types/RoleTypes';

export default class Header extends PureComponent<
  { assignedRoles?: IAssignedRoles },
  IHeaderState
> {
  constructor(props: { assignedRoles: IAssignedRoles }) {
    super(props);
    this.state = {
      isOpen: false,
      showSettings: false,
    };
  }

  _toggle = () => {
    const { isOpen } = this.state;
    this.setState({ isOpen: !isOpen });
  };

  _toggleSettings = () => {
    const { showSettings } = this.state;
    this.setState({ showSettings: !showSettings });
  };

  _logout = () => {
    logOutUser();
  };

  renderHeaderItem = (link: string, icon: string, heading: string, newTab?: boolean) => (
    <NavItem className={styles.headerItem}>
      <Link to={link} target={newTab ? '_blank' : ''} className={`${styles.headerLink} nav-link`}>
        <img src={icon} className={styles.icon} alt={icon} height="16px" width="16px" />
        {heading}
      </Link>
    </NavItem>
  );

  render() {
    const { isOpen, showSettings } = this.state;
    const { assignedRoles } = this.props;
    const Header =
      process.env.REACT_APP_MODE === PUBLIC_MODE ? (
        <PublicPortalHeader
          renderHeaderItem={this.renderHeaderItem}
          isOpen={isOpen}
          toggle={this._toggle}
        />
      ) : (
        <PortalHeader
          renderHeaderItem={this.renderHeaderItem}
          toggle={this._toggle}
          isOpen={isOpen}
          showSettings={showSettings}
          logout={this._logout}
          toggleSettings={this._toggleSettings}
          assignedRoles={assignedRoles}
        />
      );

    return Header;
  }
}
