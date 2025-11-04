import React, { memo } from 'react';
import styles from './GridHeaderCell.module.scss';
import { IGridHeaderCellProps } from './GridHeaderCellTypes';
import { ARROW_DOWN_ICON, ARROW_DOWN_ACTIVE_ICON, ARROW_UP_ICON } from '../../../consts';

const GridHeaderCell: React.FunctionComponent<IGridHeaderCellProps> = ({
  text,
  isActive,
  onClickHeaderCell,
  isSortable,
  ascending,
}) => {
  const _getArrowIcon = () => {
    if (isActive) {
      if (ascending) {
        return ARROW_DOWN_ACTIVE_ICON;
      }
      return ARROW_UP_ICON;
    }

    return ARROW_DOWN_ICON;
  };

  return (
    <td
      className={styles.container}
      style={
        isSortable || onClickHeaderCell !== null ? { cursor: 'pointer' } : { cursor: 'normal' }
      }
      onClick={(isSortable || onClickHeaderCell) && onClickHeaderCell}
    >
      <div className={styles.subContainer}>
        {text}
        <img
          src={_getArrowIcon()}
          className={styles.icon}
          style={isSortable ? { visibility: 'visible' } : { visibility: 'hidden' }}
          alt={''}
        />
      </div>
    </td>
  );
};

export default memo(GridHeaderCell);
