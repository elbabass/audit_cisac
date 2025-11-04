import React, { FunctionComponent, memo } from 'react';
import styles from './ExtractToFTP.module.scss';
import { getStrings } from '../../../configuration/Localization';
const { EXTRACT_TO_FTP, EXTRACT_TO_SFTP_SUBMITTED, EXTRACT_TO_FTP_JSON, EXTRACT_TO_SFTP_SUBMITTED_JSON } = getStrings();

interface ExtractToFTPProps {
  fileType?: 'csv' | 'json';
}

const ExtractToFTP: FunctionComponent<ExtractToFTPProps> = ({ fileType = 'csv' }) => {
  let title: string = EXTRACT_TO_FTP;
  let description: string = EXTRACT_TO_SFTP_SUBMITTED;

  if (fileType === 'json') {
    title = EXTRACT_TO_FTP_JSON;
    description = EXTRACT_TO_SFTP_SUBMITTED_JSON;
  }

  return (
    <div className={styles.container}>
      <div className={styles.title}>{title}</div>
      <div>{description}</div>
    </div>
  );
};

export default memo(ExtractToFTP);
