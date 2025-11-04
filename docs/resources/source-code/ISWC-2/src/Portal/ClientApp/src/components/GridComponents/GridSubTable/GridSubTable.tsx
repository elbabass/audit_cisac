import React, { memo } from 'react';
import Grid from "../Grid/Grid";
import styles from "./GridSubTable.module.scss";
import { IGridSubTableProps } from "../Grid/GridTypes"

const GridSubTable: React.FunctionComponent<IGridSubTableProps> = ({
    title,
    headerCells,
    rows,
    }) => {
    return (
      <div className={styles.subTableContainer}>
        <div className={styles.subTableTitle}>{`${title}:`}</div>
        <div className={styles.subTable}>
          <Grid headerCells={headerCells} headerColor={'#e6e6e6'} gridRows={rows} />
        </div>
      </div>
    );
}

export default memo(GridSubTable);
