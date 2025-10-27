import React, { memo } from 'react';
import { IGridTextCellProps } from './GridTextCellTypes';
import styles from './GridTextCell.module.scss';

const GridTextCell: React.FunctionComponent<IGridTextCellProps> = ({
  text,
  delayWrap,
  displayIcon,
  displayIconPopUpText,
  indicator,
}) => {
  const style = delayWrap ? `${styles.container} ${styles.delayWrap}` : styles.container;
  return (
    <div className={style}>
      {text && (
        <div
          title={displayIcon ? displayIconPopUpText : undefined}
          className={displayIcon ? styles.text : undefined}
        >
          <span className={indicator ? styles.indicator : undefined}>{text}</span>
        </div>
      )}
    </div>
  );
};

export default memo(GridTextCell);
