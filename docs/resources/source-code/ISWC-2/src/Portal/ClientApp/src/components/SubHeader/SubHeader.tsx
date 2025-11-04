import React, { memo } from 'react';
import { ISubHeaderProps } from './SubHeaderTypes';
import styles from './SubHeader.module.scss';

const SubHeader: React.FunctionComponent<ISubHeaderProps> = ({ title }) => {
  return (
    <div className={styles.container}>
      <div className={styles.textDiv}>{title}</div>
    </div>
  );
};

export default memo(SubHeader);
