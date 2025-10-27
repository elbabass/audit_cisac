import React, { PureComponent } from 'react';
import { ILinkedToGridProps, ILinkedToGridState } from './DemergeTypes';
import Grid from '../../../components/GridComponents/Grid/Grid';
import styles from './Demerge.module.scss';
import { IGridRow, IGridHeaderCell } from '../../../components/GridComponents/Grid/GridTypes';
import GridTextCell from '../../../components/GridComponents/GridTextCell/GridTextCell';
import {
  ORIGINAL_TITLE_FIELD,
  CREATOR_NAMES_FIELD,
  CREATION_DATE_FIELD,
  VIEW_MORE_ICON,
  VIEW_MORE_FIELD,
  VIEW_MORE_ACTION,
  CHECKBOX_FIELD,
  ISWC_FIELD,
  DEMERGE,
  DATE_TYPE,
} from '../../../consts';
import GridIconCell from '../../../components/GridComponents/GridIconCell/GridIconCell';
import GridCheckboxCell from '../../../components/GridComponents/GridCheckboxCell/GridCheckboxCell';
import { getStrings } from '../../../configuration/Localization';
import {
  _getContributorNames,
  formatDateString,
  validateIswcAndFormat,
} from '../../../shared/helperMethods';
import ViewMore from '../ViewMore/ViewMore';

export default class LinkedToGrid extends PureComponent<ILinkedToGridProps, ILinkedToGridState> {
  linkedToHeaderCells = (): IGridHeaderCell[] => {
    const strings = getStrings();
    return [
      { text: strings[ISWC_FIELD], field: ISWC_FIELD, sortable: true },
      { text: strings[ORIGINAL_TITLE_FIELD], field: ORIGINAL_TITLE_FIELD, sortable: true },
      { text: strings[CREATOR_NAMES_FIELD], field: CREATOR_NAMES_FIELD },
      {
        text: strings[CREATION_DATE_FIELD],
        field: CREATION_DATE_FIELD,
        sortable: true,
        type: DATE_TYPE,
      },
      { field: VIEW_MORE_FIELD },
      { field: CHECKBOX_FIELD },
    ];
  };

  linkedToGridRows = (): IGridRow[] => {
    const { linkedIswcs, iswcsToDemerge, addToIswcsToDemerge } = this.props;
    const strings = getStrings();

    let gridRows: IGridRow[] = [];
    linkedIswcs.forEach((iswc, index) => {
      gridRows.push({
        rowId: index,
        cells: [
          { element: <GridTextCell text={validateIswcAndFormat(iswc.iswc)} />, field: ISWC_FIELD },
          {
            element: <GridTextCell text={iswc.originalTitle} />,
            field: ORIGINAL_TITLE_FIELD,
          },
          {
            element: <GridTextCell text={_getContributorNames(iswc.interestedParties)} />,
            field: CREATOR_NAMES_FIELD,
          },
          {
            element: <GridTextCell text={formatDateString(iswc.createdDate)} />,
            field: CREATION_DATE_FIELD,
          },
          {
            element: (
              <GridIconCell
                text={strings[VIEW_MORE_FIELD]}
                icon={VIEW_MORE_ICON}
                alt={'View More Icon'}
                clickable
                id={strings[VIEW_MORE_FIELD]}
              />
            ),
            field: VIEW_MORE_FIELD,
            // Handled in GridRow.tsx*
            action: VIEW_MORE_ACTION,
          },
          {
            element: (
              <GridCheckboxCell
                text={strings[DEMERGE]}
                onClickCheckbox={() => addToIswcsToDemerge(index)}
                checked={iswcsToDemerge.indexOf(index) > -1}
              />
            ),
            field: CHECKBOX_FIELD,
          },
        ],
        viewMore: [<ViewMore iswcModel={iswc} />],
      });
    });
    return gridRows;
  };

  render() {
    return (
      <div className={styles.container}>
        <Grid headerCells={this.linkedToHeaderCells()} gridRows={this.linkedToGridRows()} />
      </div>
    );
  }
}
