import * as React from 'react';
import { IGridHeaderCell, IGridRow } from '../../../../components/GridComponents/Grid/GridTypes';
import { getStrings } from '../../../../configuration/Localization';
import {
  DATE_FIELD,
  DATE_TYPE,
  TYPE_FIELD,
  SUBMITTING_AGENCY_FIELD,
  AGENCY_WORK_CODE_FIELD,
  PUBLISHER_IP_NAME_NUMBER_FIELD,
  PUBLISHER_WORK_NUMBER_FIELD,
  CREATOR_NAMES_FIELD,
  IP_NAME_NUMBER_FIELD,
  STATUS_FIELD,
  STATUS_APPROVED_ICON,
  STATUS_REJECTED_ICON,
  ISWC_FIELD,
} from '../../../../consts';
import GridTextCell from '../../../../components/GridComponents/GridTextCell/GridTextCell';
import Media from 'react-media';
import GridTableCell from '../../../../components/GridComponents/GridTableCell/GridTableCell';
import {
  _getAgency,
  formatDateString,
  padIpNameNumber,
} from '../../../../shared/helperMethods';
import Grid from '../../../../components/GridComponents/Grid/Grid';
import GridIconCell from '../../../../components/GridComponents/GridIconCell/GridIconCell';
import { ISubmissionAuditGridState, ISubmissionAuditGridProps } from './SubmissionAuditGridTypes';
import styles from '../SubmissionAudit.module.scss';

class SubmissionAuditGrid extends React.PureComponent<
  ISubmissionAuditGridProps,
  ISubmissionAuditGridState
> {
  _getHeaderCells = (): IGridHeaderCell[] => {
    const strings = getStrings();
    return [
      { text: strings[DATE_FIELD], field: DATE_FIELD, sortable: true, type: DATE_TYPE },
      { text: strings[TYPE_FIELD], field: TYPE_FIELD, sortable: true },
      {
        text: strings[SUBMITTING_AGENCY_FIELD],
        field: SUBMITTING_AGENCY_FIELD,
      },
      { text: strings[AGENCY_WORK_CODE_FIELD], field: AGENCY_WORK_CODE_FIELD },
      {
        text: strings[PUBLISHER_IP_NAME_NUMBER_FIELD],
        field: PUBLISHER_IP_NAME_NUMBER_FIELD,
      },
      {
        text: strings[PUBLISHER_WORK_NUMBER_FIELD],
        field: PUBLISHER_WORK_NUMBER_FIELD,
      },
      {
        text: strings[ISWC_FIELD],
        field: ISWC_FIELD,
      },
      { text: strings[CREATOR_NAMES_FIELD], field: CREATOR_NAMES_FIELD },
      {
        text: strings[IP_NAME_NUMBER_FIELD],
        field: IP_NAME_NUMBER_FIELD,
      },
      {
        text: strings[STATUS_FIELD],
        field: STATUS_FIELD,
      },
    ];
  };

  _getGridData = (): IGridRow[] => {
    const { submissionAuditSearchResults } = this.props;
    let gridRows: IGridRow[] = [];

    submissionAuditSearchResults?.forEach((searchResult, index) => {
      const paddedCreatorNameNumbers: string[] = [];
      searchResult.creatorNameNumbers
        ?.split(';')
        .forEach((nameNumber) => paddedCreatorNameNumbers.push(padIpNameNumber(nameNumber)));

      gridRows.push({
        rowId: index,
        cells: [
          {
            element: <GridTextCell text={formatDateString(searchResult.createdDate)} />,
            field: DATE_FIELD,
          },
          {
            element: (
              <GridTextCell
                text={searchResult.transactionType}
              />
            ),
            field: TYPE_FIELD,
          },
          {
            element: <GridTextCell text={_getAgency(searchResult.agencyCode)} />,
            field: SUBMITTING_AGENCY_FIELD,
          },
          {
            element: <GridTextCell text={searchResult.agencyWorkCode} />,
            field: AGENCY_WORK_CODE_FIELD,
          },
          {
            element: (
              <GridTextCell text={padIpNameNumber(searchResult.publisherNameNumber?.toString())} />
            ),
            field: PUBLISHER_IP_NAME_NUMBER_FIELD,
          },
          {
            element: <GridTextCell text={searchResult.publisherWorkNumber} />,
            field: PUBLISHER_WORK_NUMBER_FIELD,
          },
          {
            element: <GridTextCell text={searchResult.preferredIswc} />,
            field: ISWC_FIELD,
          },
          {
            element: (
              <Media queries={{ small: { maxWidth: 1024 } }}>
                {(matches) =>
                  matches.small ? (
                    <GridTextCell text={searchResult.creatorNames} />
                  ) : (
                    <GridTableCell textArray={searchResult.creatorNames?.split(';')} />
                  )
                }
              </Media>
            ),
            field: CREATOR_NAMES_FIELD,
          },
          {
            element: (
              <Media queries={{ small: { maxWidth: 1024 } }}>
                {(matches) =>
                  matches.small ? (
                    <GridTextCell text={paddedCreatorNameNumbers.join(', ')} />
                  ) : (
                    <GridTableCell textArray={paddedCreatorNameNumbers} />
                  )
                }
              </Media>
            ),
            field: IP_NAME_NUMBER_FIELD,
          },
          {
            element: this.renderStatusIcon(
              searchResult.isProcessingFinished,
              searchResult.isProcessingError,
              searchResult.code,
              searchResult.message,
            ),
            field: STATUS_FIELD,
          },
        ],
      });
    });

    return gridRows;
  };

  renderStatusIcon = (finished: boolean, error: boolean, code: string, message: string) => {
    const { SUCCESSFUL } = getStrings();
    if (!finished) {
      return <div />;
    }

    if (error) {
      return (
        <div className={styles.errorIconDiv}>
          <GridIconCell icon={STATUS_REJECTED_ICON} alt={''} hoverText={message} />
          <div className={styles.codeDiv}>({code})</div>
        </div>
      );
    }

    return <GridIconCell icon={STATUS_APPROVED_ICON} alt={''} hoverText={SUCCESSFUL} />;
  };

  render() {
    const { LIST_VIEW } = getStrings();
    return (
      <div>
        <div className={styles.graphTitle}>{LIST_VIEW}</div>
        <Grid headerCells={this._getHeaderCells()} gridRows={this._getGridData()} pagination />
      </div>
    );
  }
}

export default SubmissionAuditGrid;
