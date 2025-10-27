import React, { memo } from 'react';
import { ISwitchProps } from './SwitchTypes';
import styles from './Switch.module.scss';

const Switch: React.FunctionComponent<ISwitchProps> = ({
  text,
  active,
  toggleSwitch,
  textTopPosition,
  disabled,
}) => {
  return (
    <div
      className={
        textTopPosition
          ? `${styles.switchContainerTextTop} ${styles.switchContainer}`
          : styles.switchContainer
      }
    >
      <div className={styles.labelText}>{text}</div>
      <div>
        <label
          className={
            textTopPosition ? `${styles.switchInputLeft} ${styles.switchInput}` : styles.switchInput
          }
        >
          <input type="checkbox" onChange={toggleSwitch} checked={active} disabled={disabled} />
          <span className={`${styles.slider} ${styles.round}`}></span>
        </label>
      </div>
    </div>
  );
};

export default memo(Switch);
