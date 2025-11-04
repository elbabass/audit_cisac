import React, { memo } from 'react';
import { IGridTableCellProps } from './GridTableCellTypes';
import styles from './GridTableCell.module.scss';

const GridTableCell: React.FunctionComponent<IGridTableCellProps> = ({ textArray }) => {
  const renderRows = () => {
    return textArray?.map((x, i) => (
      <div className={styles.row} title={x} key={i}>
        {x}
        {i < textArray.length - 1 && ','}
      </div>
    ));
  };

  return <div className={styles.container}>{renderRows()}</div>;
};

export default memo(GridTableCell);
