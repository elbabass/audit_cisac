import React, { FunctionComponent, memo } from 'react';
import styles from './Maintenance.module.scss';
import maintenance from '../../assets/maintenance.jpg';
import { getStrings } from '../../configuration/Localization';
const { SCHEDULED_MAINTENANCE, BE_BACK_UP, SORRY_FOR_INCON } = getStrings();

const Maintenance: FunctionComponent = () => (
  <div className={styles.container}>
    <div className={styles.subContainer}>
      <div className={styles.title}>{SCHEDULED_MAINTENANCE}</div>
      <div>{BE_BACK_UP}</div>
      <div>
        <img src={maintenance} className={styles.image} alt={'Maintenance'} />
      </div>
      <div>{SORRY_FOR_INCON}</div>
    </div>
  </div>
);

export default memo(Maintenance);
