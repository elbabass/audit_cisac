import React, { PureComponent } from 'react';
import { IMergeGridProps, IMergeGridState } from './MergeTypes';
import Grid from '../../../components/GridComponents/Grid/Grid';
import styles from './Merge.module.scss';
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
  REMOVE_ICON,
  REMOVE_FIELD,
  REMOVE,
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

export default class MergeGrid extends PureComponent<IMergeGridProps, IMergeGridState> {
  constructor(props: IMergeGridProps) {
    super(props);

    this.state = {
      selectedRowId: 0,
    };
  }

  componentDidMount() {
    const { updateMergeRequestData, mergeList } = this.props;
    if (mergeList.length > 0) updateMergeRequestData(mergeList[0].iswc);
  }

  selectRow = (rowId: number) => {
    const { mergeList, updateMergeRequestData } = this.props;
    this.setState({
      selectedRowId: rowId,
    });
    updateMergeRequestData(mergeList[rowId].iswc);
  };

  _removeRow = (index: number) => {
    const { removeFromMergeList } = this.props;
    removeFromMergeList && removeFromMergeList(index);
  };

  mergeHeaderCells = (): IGridHeaderCell[] => {
    const strings = getStrings();
    return [
      { field: CHECKBOX_FIELD },
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
      { field: REMOVE_FIELD },
    ];
  };

  mergeGridRows = (): IGridRow[] => {
    const { selectedRowId } = this.state;
    const { mergeList } = this.props;
    const mergeRows: IGridRow[] = [];
    const strings = getStrings();

    mergeList.forEach((iswc, index) => {
      mergeRows.push({
        rowId: index,
        cells: [
          {
            element: (
              <GridCheckboxCell
                onClickCheckbox={() => this.selectRow(index)}
                checked={selectedRowId === index}
              />
            ),
            field: CHECKBOX_FIELD,
          },
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
              <div onClick={() => this._removeRow(index)}>
                <GridIconCell
                  icon={REMOVE_ICON}
                  text={strings[REMOVE]}
                  alt={'Remove Icon'}
                  clickable
                  id={strings[REMOVE]}
                />
              </div>
            ),
            field: REMOVE_FIELD,
          },
        ],
        viewMore: [<ViewMore iswcModel={iswc} isSubmissionGrid />],
      });
    });

    return mergeRows;
  };

  render() {
    return (
      <div className={styles.gridContainer}>
        <Grid headerCells={this.mergeHeaderCells()} gridRows={this.mergeGridRows()} />
      </div>
    );
  }
}
