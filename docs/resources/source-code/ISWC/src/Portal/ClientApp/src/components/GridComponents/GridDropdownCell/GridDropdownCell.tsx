import React, { memo } from 'react';
import { IGridDropdownCellProps } from './GridDropdownCellTypes';
import styles from './GridDropdownCell.module.scss';

const GridDropdownCell: React.FunctionComponent<IGridDropdownCellProps> = ({
  options,
  value,
  selectNewOption,
}) => {
  const renderOptions = () => {
    var arr = [];
    for (let i = 0; i < options.length; i++) {
      arr.push(
        <option key={i} value={options[i].value}>
          {options[i].name}
        </option>,
      );
    }
    return arr;
  };

  return (
    <select
      className={styles.input}
      value={value}
      onChange={(event) => selectNewOption(event.target.value)}
    >
      {renderOptions()}
    </select>
  );
};

export default memo(GridDropdownCell);
