import React, { memo } from 'react';
import { LEFT_ARROW_RED_ICON } from '../../consts';
import styles from './GoBack.module.scss';
import { IGoBackProps } from './GoBackTypes';

const GoBack: React.FunctionComponent<IGoBackProps> = ({ text, action }) => {
  return (
    <div className={styles.goBackDiv} onClick={action}>
      <img src={LEFT_ARROW_RED_ICON} className={styles.arrowIcon} alt="" />
      {text}
    </div>
  );
};

export default memo(GoBack);
