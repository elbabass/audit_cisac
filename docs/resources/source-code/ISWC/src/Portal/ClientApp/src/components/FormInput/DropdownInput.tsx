import React, { memo } from 'react';
import { IDropdownProps } from './FormInputTypes';
import styles from './FormInput.module.scss';
import { getLoggedInAgencyId } from '../../shared/helperMethods';

const DropdownInput: React.FunctionComponent<IDropdownProps> = (props) => {
  const _generateOptionsList = () => {
    if (props.options)
      return props.options.map((option) => (
        <option key={option.value} value={option.value}>
          {option.name}
        </option>
      ));
  };
  return (
    <div className={styles.inputContainer}>
      <label htmlFor={props.name} className={styles.formLabel}>
        {props.label + ':'}
      </label>
      <select
        id={props.name}
        onChange={props.onChange}
        className={styles.dropdownInput}
        disabled={props.disabled}
        defaultValue={props.defaultToLoggedInAgency ? getLoggedInAgencyId() : ''}
        value={props.value}
      >
        {_generateOptionsList()}
      </select>
    </div>
  );
};

export default memo(DropdownInput);
