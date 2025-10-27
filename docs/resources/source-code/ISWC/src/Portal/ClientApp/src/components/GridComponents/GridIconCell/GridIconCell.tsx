import React, { memo } from 'react';
import { IGridIconCellProps } from './GridIconCellTypes';
import styles from './GridIconCell.module.scss';
import { ADD_NEW_ICON_RED } from '../../../consts';

const GridIconCell: React.FunctionComponent<IGridIconCellProps> = ({
  text,
  icon,
  alt,
  hoverText,
  clickable,
  largerIcon,
  smallerIcon,
  id,
}) => {
  const getIconStyle = () => {
    if (smallerIcon) return text ? styles.smallerIcon : `${styles.smallerIcon} ${styles.noMargin}`;
    if (largerIcon) return text ? styles.largerIcon : `${styles.largerIcon} ${styles.noMargin}`;
    return text ? styles.icon : `${styles.icon} ${styles.noMargin}`;
  };

  const getTextStyle = () => {
    if (icon === ADD_NEW_ICON_RED) return styles.textContainer;
  };

  return (
    <div
      className={styles.container}
      title={hoverText && hoverText}
      style={{ cursor: clickable ? 'pointer' : 'normal' }}
      id={id}
    >
      <img src={icon} className={getIconStyle()} alt={alt} id={id} />
      <div className={getTextStyle()} id={id}>
        {text}
      </div>
    </div>
  );
};

export default memo(GridIconCell);
