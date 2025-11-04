import React, { PureComponent } from 'react';
import { IGridRowProps, IGridRowState } from './GridRowTypes';
import styles from './GridRow.module.scss';
import { IGridCell } from '../Grid/GridTypes';
import GridIconCell from '../GridIconCell/GridIconCell';
import {
  VIEW_MORE_ACTION,
  VIEW_MORE_FIELD,
  VIEW_LESS_FIELD,
  VIEW_MORE_ICON,
  PUBLIC_MODE,
  IP_LOOKUP_ACTION,
  SEARCH,
} from '../../../consts';
import { getStrings } from '../../../configuration/Localization';
import AlertMessage from '../../AlertMessage/AlertMessage';

export default class GridRow extends PureComponent<IGridRowProps, IGridRowState> {
  constructor(props: IGridRowProps) {
    super(props);
    this.state = {
      displayViewMoreDiv: false,
      viewMoreIndex: 0,
    };
  }

  _toggleViewMore = (index?: number) => {
    const { displayViewMoreDiv } = this.state;

    this.setState({
      displayViewMoreDiv: !displayViewMoreDiv,
      viewMoreIndex: index !== undefined ? index : 0,
    });
  };

  _onClickCell = (cell: IGridCell, event: React.MouseEvent<HTMLElement>) => {
    const { LOOKUP } = getStrings();
    const { displayViewMoreDiv } = this.state;
    switch (cell.action) {
      case VIEW_MORE_ACTION:
        this._toggleViewMore();
        break;
      case IP_LOOKUP_ACTION: {
        const target = event.target as HTMLElement;
        if (target.id === LOOKUP) this._toggleViewMore();
        // if SEARCH render NameNumberLookup in ViewMore
        if (target.id === SEARCH) {
          if (displayViewMoreDiv) {
            this.setState({ displayViewMoreDiv: false }, () =>
              this.setState({ displayViewMoreDiv: true, viewMoreIndex: 1 }),
            );
          } else this.setState({ displayViewMoreDiv: true, viewMoreIndex: 1 });
        }
        break;
      }
      default:
        return;
    }
  };

  displayCell = (cell: IGridCell) => {
    if (cell.field === VIEW_MORE_FIELD) return this.renderViewMoreCell(cell);
    else return cell.element;
  };

  renderViewMoreCell = (cell: IGridCell) => {
    const strings = getStrings();
    const { displayViewMoreDiv } = this.state;
    if (!displayViewMoreDiv) return cell.element;
    return (
      <GridIconCell
        text={strings[VIEW_LESS_FIELD]}
        icon={VIEW_MORE_ICON}
        alt={'View More Icon'}
        clickable
        id={strings[VIEW_LESS_FIELD]}
      />
    );
  };

  renderViewMore = () => {
    const { viewMoreIndex } = this.state;
    if (this.props.gridRow.viewMore && Array.isArray(this.props.gridRow.viewMore)) {
      const ViewMore = this.props.gridRow.viewMore[viewMoreIndex];
      const props = { close: this._toggleViewMore };
      return React.cloneElement(ViewMore, props);
    }
  };

  renderRowCells = () => {
    const {
      gridRow,
      getColumnHeader,
      cellPadding,
      restrictRowHeightOnMobile,
      message,
    } = this.props;
    let cellPaddingStyle = cellPadding ? { padding: cellPadding } : {};
    if (message) cellPaddingStyle = { padding: '25px 25px 5px 10px' };
    return gridRow.cells.map((cell: IGridCell, index: number) => {
      if (!(process.env.REACT_APP_MODE === PUBLIC_MODE && cell.hideInPublicMode)) {
        return (
          <td
            className={styles.gridCell}
            style={cellPaddingStyle}
            onClick={(event) => (cell.action ? this._onClickCell(cell, event) : null)}
            id={getColumnHeader(cell.field)}
            key={`${index}.1`}
          >
            <div
              className={
                restrictRowHeightOnMobile
                  ? `${styles.gridCellRightTabletMax}${styles.gridCellRightTablet}`
                  : styles.gridCellRightTablet
              }
            >
              {this.displayCell(cell)}
            </div>
          </td>
        );
      } else return null;
    });
  };

  _getContainerStyle = () => {
    const { message, lastRow } = this.props;

    if (message) {
      return styles.messageContainer;
    } else {
      if (lastRow) return styles.container;
      else return `${styles.container} ${styles.containerMargin}`;
    }
  };

  render() {
    const { numColumns, message, gridRow } = this.props;
    const { displayViewMoreDiv } = this.state;

    return (
      <tbody className={styles.rowDiv}>
        <tr 
          className={`${this._getContainerStyle()} ${styles.block}`} 
          style={gridRow.subTable ? { border : 'none' } : {}}
        >
          {this.renderRowCells()}
        </tr>
        {gridRow.subTable && (
          <tr className={`${this._getContainerStyle()} ${styles.block}`}>
            <td className={styles.block} colSpan={numColumns}>
              {gridRow.subTable}
            </td>
          </tr>
        )}
        {message && (
          <tr className={styles.messageRow}>
            <td className={styles.messsageCell} colSpan={numColumns}>
              <AlertMessage message={message} type={`warn`} />
            </td>
          </tr>
        )}
        {displayViewMoreDiv ? (
          <tr className={`${styles.block} ${styles.viewMoreDiv}`}>
            <td className={styles.block} colSpan={numColumns}>
              {this.renderViewMore()}
            </td>
          </tr>
        ) : null}
      </tbody>
    );
  }
}
