import React from 'react';
import styles from './AlertMessage.module.scss';
import { IAlertMessageProps, IAlertMessageState } from './AlertMessageTypes';
import {
  STATUS_REJECTED_ICON,
  STATUS_APPROVED_ICON,
  STATUS_INFO_ICON,
  STATUS_PENDING_ICON,
  CLOSE_ICON,
} from '../../consts';
import { getStrings } from '../../configuration/Localization';
import GridIconCell from '../GridComponents/GridIconCell/GridIconCell';

export default class AlertMessage extends React.PureComponent<
  IAlertMessageProps,
  IAlertMessageState
> {
  constructor(props: IAlertMessageProps) {
    super(props);

    this.state = {
      showSubMessage: false,
    };
  }

  _getIconAndStyle = () => {
    const { type } = this.props;
    switch (type) {
      case 'error': {
        return [STATUS_REJECTED_ICON, styles.error];
      }
      case 'success': {
        return [STATUS_APPROVED_ICON, styles.success];
      }
      case 'info': {
        return [STATUS_INFO_ICON, styles.info];
      }
      case 'warn':
        return [STATUS_PENDING_ICON, styles.warn];
    }
  };

  _toggleSubMessage = () => {
    const { showSubMessage } = this.state;

    this.setState({
      showSubMessage: !showSubMessage,
    });
  };

  render() {
    const { message, subMessage, close } = this.props;
    const { showSubMessage } = this.state;
    const { VIEW_MORE_FIELD, VIEW_LESS_FIELD } = getStrings();

    return (
      <div className={`${styles.mainContainer} ${this._getIconAndStyle()[1]}`}>
        <div className={styles.container}>
          <div className={styles.iconDiv}>
            <img
              src={this._getIconAndStyle()[0]}
              className={styles.icon}
              alt={'Alert Message Icon'}
            />
          </div>
          <div className={styles.text}>{message}</div>
          {subMessage && (
            <div className={styles.viewMore} onClick={this._toggleSubMessage}>
              {showSubMessage ? VIEW_LESS_FIELD : VIEW_MORE_FIELD}
            </div>
          )}
          {close && (
            <div className={styles.closeButton} onClick={() => close()}>
              <GridIconCell icon={CLOSE_ICON} alt={'Close Icon'} smallerIcon />
            </div>
          )}
        </div>
        {showSubMessage && <div className={styles.portalSubMessage}>{subMessage}</div>}
      </div>
    );
  }
}
