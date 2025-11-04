import React, { memo } from 'react';
import styles from './PaginationButton.module.scss';
import { IPaginationButtonProps } from './PaginationButtonTypes';
import {
  RIGHT_ARROW_ICON,
  LEFT_ARROW_ICON,
  LEFT_ARROW_DISABLED_ICON,
  RIGHT_ARROW_DISABLED_ICON,
} from '../../../../consts';

const GridInputCell: React.FunctionComponent<IPaginationButtonProps> = ({
  onClick,
  text,
  iconLeft,
  disabled,
}) => {
  return (
    <div
      className={!disabled ? styles.pageNumberButton : styles.pageNumberButtonDisabled}
      onClick={!disabled ? onClick : () => null}
    >
      {iconLeft && (
        <img
          src={!disabled ? LEFT_ARROW_ICON : LEFT_ARROW_DISABLED_ICON}
          className={`${styles.arrowIcon} ${styles.left}`}
          alt=""
        />
      )}
      {text}
      {!iconLeft && (
        <img
          src={!disabled ? RIGHT_ARROW_ICON : RIGHT_ARROW_DISABLED_ICON}
          className={`${styles.arrowIcon} ${styles.right}`}
          alt=""
        />
      )}
    </div>
  );
};

export default memo(GridInputCell);
