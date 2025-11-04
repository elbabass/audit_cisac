import React, { memo } from 'react';
import { IGridInputCellProps } from './GridInputCellTypes';
import styles from './GridInputCell.module.scss';

const GridInputCell: React.FunctionComponent<IGridInputCellProps> = ({
  onChange,
  value,
  noRightBorder,
}) => {
  return (
    <input
      className={styles.input}
      style={noRightBorder ? { borderRight: 'none' } : {}}
      type={'text'}
      onChange={(event) => onChange(event.target.value)}
      value={value}
    />
  );
};

export default memo(GridInputCell);
