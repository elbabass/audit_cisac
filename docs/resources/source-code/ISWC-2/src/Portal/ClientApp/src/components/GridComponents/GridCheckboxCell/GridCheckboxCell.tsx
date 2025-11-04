import React, { memo } from 'react';
import { IGridCheckboxCellProps } from './GridCheckboxCellTypes';
import styles from './GridCheckboxCell.module.scss';

const GridCheckboxCell: React.FunctionComponent<IGridCheckboxCellProps> = ({
  text,
  onClickCheckbox,
  checked,
  disabled,
}) => {
  return (
    <div className={styles.container}>
      <label className={styles.subContainer}>
        <input
          type={'checkbox'}
          onChange={() => onClickCheckbox && onClickCheckbox()}
          checked={checked}
          disabled={disabled}
        />
        <span className={styles.checkbox} />
        {text ? <div className={styles.checkboxLabel}>{text}</div> : null}
      </label>
    </div>
  );
};

export default memo(GridCheckboxCell);
