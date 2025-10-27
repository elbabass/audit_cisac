import React, { memo } from 'react';
import { IDateRangeInputProps } from './FormInputTypes';
import styles from './FormInput.module.scss';
import DateInput from './DateInput';
import { getStrings } from '../../configuration/Localization';

const { FROM_DATE, TO_DATE } = getStrings();

const DateRangeInput: React.FunctionComponent<IDateRangeInputProps> = ({
  changeFromDate,
  changeToDate,
  fromDate,
  toDate,
  fromDateName,
  toDateName,
}) => {
  return (
    <div className={styles.dateRangeInputContainer}>
      <div className={styles.dateRangeItem}>
        <DateInput
          label={`${FROM_DATE}:`}
          changeData={changeFromDate}
          name={fromDateName || 'fromDate'}
          value={fromDate}
        />
      </div>
      <div>
        <DateInput
          label={`${TO_DATE}:`}
          changeData={changeToDate}
          name={toDateName || 'toDate'}
          value={toDate}
        />
      </div>
    </div>
  );
};

export default memo(DateRangeInput);
