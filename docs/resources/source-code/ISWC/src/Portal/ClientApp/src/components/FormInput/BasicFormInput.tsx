import React, { memo } from 'react';
import { IFormInputProps } from './FormInputTypes';
import styles from './FormInput.module.scss';

const BasicFormInput: React.FunctionComponent<IFormInputProps> = (props) => {
  return (
    <div className={styles.inputContainer}>
      {props.label && (
        <label htmlFor={props.name} className={styles.formLabel}>
          {props.label + ':'}
        </label>
      )}

      <input
        type="text"
        id={props.name}
        placeholder={props.placeholder || ''}
        onChange={props.onChange}
        className={styles.formInput}
        value={props.value}
        disabled={props.disabled}
        autoComplete={'off'}
      />
    </div>
  );
};

export default memo(BasicFormInput);
