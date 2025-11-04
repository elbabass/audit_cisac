import * as React from 'react';
import { IAssignedRoles } from '../../redux/types/RoleTypes';
import Header from '../Header/Header';
import styles from './Layout.module.scss';

export default (props: { children?: React.ReactNode; assignedRoles?: IAssignedRoles }) => (
  <React.Fragment>
    <div className={styles.container}>
      <Header assignedRoles={props.assignedRoles} />
      <div className={styles.subContainer}>{props.children}</div>
    </div>
  </React.Fragment>
);
