import React, { memo } from 'react';
import { ITabButtonProps } from './TabButtonTypes';
import styles from './TabButton.module.scss';

const TabButton: React.FunctionComponent<ITabButtonProps> = ({
  text,
  active,
  onClickButton,
  reportsPage,
  disabled,
}) => {
  const _getStyle = () => {
    if (disabled) {
      if (reportsPage) {
        return `${styles.disabled} ${styles.container} ${styles.reportsContainer}`;
      }
      return `${styles.disabled} ${styles.container}`;
    }
    if (active) {
      if (reportsPage) {
        return `${styles.active} ${styles.container} ${styles.reportsContainer}`;
      }
      return `${styles.active} ${styles.container}`;
    } else {
      if (reportsPage) {
        return `${styles.container} ${styles.reportsContainer}`;
      }
      return styles.container;
    }
  };
  return (
    <div onClick={() => !disabled && onClickButton()} className={_getStyle()}>
      {text}
    </div>
  );
};

export default memo(TabButton);
