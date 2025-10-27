import React, { FunctionComponent, memo } from 'react';
import styles from './Loader.module.scss';

const Loader: FunctionComponent = () => (
  <div className={styles.loaderContainer}>
    <div className={styles.loader}></div>
  </div>
);

export default memo(Loader);
