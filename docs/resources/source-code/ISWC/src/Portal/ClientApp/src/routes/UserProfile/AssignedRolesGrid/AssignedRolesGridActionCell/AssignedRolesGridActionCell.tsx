import React, { memo } from 'react';
import { IAssignedRolesGridActionCellProps } from './AssignedRolesGridActionCellTypes';
import styles from './AssignedRolesGridActionCell.module.scss';

const AssignedRolesGridActionCell: React.FunctionComponent<IAssignedRolesGridActionCellProps> = ({
  topText,
  topAction,
  bottomText,
  bottomAction,
  smallText,
}) => {
  const actionText = smallText ? `${styles.actionText} ${styles.smallText}` : styles.actionText;
  const greyText = smallText ? `${styles.greyText} ${styles.smallText}` : styles.greyText;

  return (
    <div>
      <div className={!topAction ? greyText : actionText} onClick={() => topAction && topAction()}>
        {topText}
      </div>
      {bottomText && (
        <div
          className={!bottomAction ? greyText : `${actionText} ${styles.marginTop}`}
          onClick={() => bottomAction && bottomAction()}
        >
          {bottomText}
        </div>
      )}
    </div>
  );
};

export default memo(AssignedRolesGridActionCell);
