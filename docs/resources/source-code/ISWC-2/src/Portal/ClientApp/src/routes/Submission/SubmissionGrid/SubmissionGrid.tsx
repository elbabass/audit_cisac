import * as React from 'react';
import { ISubmissionGridProps, ISubmissionGridState } from './SubmissionGridTypes';
import Grid from '../../../components/GridComponents/Grid/Grid';
import styles from './SubmissionGrid.module.scss';
import IconActionButton from '../../../components/ActionButton/IconActionButton';
import { PLUS_ICON } from '../../../consts';

export default class SubmissionGrid extends React.PureComponent<
  ISubmissionGridProps,
  ISubmissionGridState
> {
  render() {
    const { actionButtonText, addRowToGrid, headerCells, gridRows, showActionButton } = this.props;

    return (
      <div className={styles.container}>
        <Grid
          headerCells={headerCells}
          gridRows={gridRows}
          headerColor={'#f7f7f7'}
          cellPadding={'25px 25px 25px 10px'}
          restrictRowHeightOnMobile
        />
        {showActionButton && (
          <div className={styles.actionButtonDiv}>
            <IconActionButton
              icon={PLUS_ICON}
              buttonText={actionButtonText && actionButtonText}
              buttonAction={addRowToGrid && addRowToGrid}
            />
          </div>
        )}
      </div>
    );
  }
}
