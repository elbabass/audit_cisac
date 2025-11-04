import React, { PureComponent } from 'react';
import { ISubmissionHistoryProps, ISubmissioHistoryState } from './SubmissionHistoryTypes';
import SubHeader from '../../../components/SubHeader/SubHeader';
import { getStrings } from '../../../configuration/Localization';
import styles from './SubmissionHistory.module.scss';
import Grid from '../../../components/GridComponents/Grid/Grid';
import { IGridHeaderCell, IGridRow } from '../../../components/GridComponents/Grid/GridTypes';
import {
  DATE_FIELD,
  TYPE_FIELD,
  METHOD_FIELD,
  SUBMITTING_AGENCY_FIELD,
  SUBMITTING_AGENCY_WORK_NO_FIELD,
  CREATORS_FIELD,
  TITLES_FIELD,
  STATUS_FIELD,
  STATUS_APPROVED_ICON,
  STATUS_REJECTED_ICON,
  STATUS_PENDING_ICON,
  DATE_TYPE,
} from '../../../consts';
import GridTextCell from '../../../components/GridComponents/GridTextCell/GridTextCell';
import GridIconCell from '../../../components/GridComponents/GridIconCell/GridIconCell';
import {
  _getContributorNames,
  formatDateString,
  _parseTitles,
  validateIswcAndFormat,
  _getAgency,
} from '../../../shared/helperMethods';
import Loader from '../../../components/Loader/Loader';
import ErrorHandler from '../../../components/AlertMessage/ErrorHandler/ErrorHandler';
import { IAuditHistoryResult } from '../../../redux/types/IswcTypes';
import GoBack from '../../../components/GoBack/GoBack';
const queryString = require('query-string');

export default class SubmissionHistory extends PureComponent<
  ISubmissionHistoryProps,
  ISubmissioHistoryState
> {
  constructor(props: ISubmissionHistoryProps) {
    super(props);
    this.state = {
      iswc: undefined,
    };
  }

  componentDidMount() {
    const { getSubmissionHistory, preferredIswc, clearSubmissionHistoryError } = this.props;
    clearSubmissionHistoryError();

    if (preferredIswc) {
      getSubmissionHistory(preferredIswc.iswc);
    } else {
      const { search } = this.props.router.location;

      if (search) {
        const parsedSearchQuery = queryString.parse(search);
        if (parsedSearchQuery?.iswc) {
          getSubmissionHistory(parsedSearchQuery.iswc);
          this.setState({
            iswc: parsedSearchQuery.iswc,
          });
        }
      }
    }
  }

  _renderStatusCell = (auditResult: IAuditHistoryResult) => {
    let statusIcon = STATUS_PENDING_ICON;
    switch (auditResult.status) {
      case 'Outstanding':
        statusIcon = STATUS_PENDING_ICON;
        break;
      case 'Approved':
        statusIcon = STATUS_APPROVED_ICON;
        break;
      case 'Rejected':
        statusIcon = STATUS_REJECTED_ICON;
    }

    return (
      <GridIconCell icon={statusIcon} alt={auditResult.status} hoverText={auditResult.status} />
    );
  };

  // Submission History Grid Data
  getSubmissionHistoryHeaderCells = (): IGridHeaderCell[] => {
    const strings = getStrings();
    return [
      { text: strings[DATE_FIELD], field: DATE_FIELD, sortable: true, type: DATE_TYPE },
      { text: strings[TYPE_FIELD], field: TYPE_FIELD, sortable: true },
      { text: strings[METHOD_FIELD], field: METHOD_FIELD, sortable: true },
      { text: strings[SUBMITTING_AGENCY_FIELD], field: SUBMITTING_AGENCY_FIELD, sortable: true },
      {
        text: strings[SUBMITTING_AGENCY_WORK_NO_FIELD],
        field: SUBMITTING_AGENCY_WORK_NO_FIELD,
      },
      { text: strings[CREATORS_FIELD], field: CREATORS_FIELD },
      { text: strings[TITLES_FIELD], field: TITLES_FIELD },
      { text: strings[STATUS_FIELD], field: STATUS_FIELD },
    ];
  };

  getSubmissionHistoryGridRows = (): IGridRow[] => {
    const { submissionHistory } = this.props;
    const submissionHistoryGridRows: IGridRow[] = [];

    submissionHistory !== undefined &&
      submissionHistory.forEach((auditResult, index) => {
        submissionHistoryGridRows.push({
          rowId: index,
          cells: [
            {
              element: <GridTextCell text={formatDateString(auditResult.submittedDate)} />,
              field: DATE_FIELD,
            },
            {
              element: <GridTextCell text={auditResult.transactionType} />,
              field: TYPE_FIELD,
            },
            {
              element: <GridTextCell text={auditResult.lastModifiedUser} />,
              field: METHOD_FIELD,
            },
            {
              element: <GridTextCell text={_getAgency(auditResult.submittingAgency)} />,
              field: SUBMITTING_AGENCY_FIELD,
            },
            {
              element: <GridTextCell text={auditResult.workNumber} />,
              field: SUBMITTING_AGENCY_WORK_NO_FIELD,
            },
            {
              element: <GridTextCell text={_getContributorNames(auditResult.creators || [])} />,
              field: CREATORS_FIELD,
            },
            {
              element: <GridTextCell text={_parseTitles(auditResult.titles)} />,
              field: TITLES_FIELD,
            },
            {
              element: this._renderStatusCell(auditResult),
              field: STATUS_FIELD,
            },
          ],
        });
      });
    return submissionHistoryGridRows;
  };

  renderGrid = () => {
    const { loading, error, submissionHistory } = this.props;

    if (loading) return <Loader />;
    else if (!loading && error) return <ErrorHandler error={error} />;
    else if (!loading && submissionHistory !== undefined) {
      return (
        <Grid
          headerCells={this.getSubmissionHistoryHeaderCells()}
          gridRows={this.getSubmissionHistoryGridRows()}
          pagination={true}
        />
      );
    }
  };

  render() {
    const {
      SUBMISSION_HISTORY,
      ISWC_FIELD,
      ORIGINAL_TITLE_FIELD,
      GO_BACK_TO_SEARCH_RESULTS,
    } = getStrings();
    const { preferredIswc, router } = this.props;
    const { iswc } = this.state;

    return (
      <div>
        <SubHeader title={SUBMISSION_HISTORY} />
        <div className={styles.container}>
          <GoBack text={GO_BACK_TO_SEARCH_RESULTS} action={() => router.history.goBack()} />
          <div className={styles.header}>
            <div className={styles.headerItem}>
              <div className={styles.headerText}>{ISWC_FIELD}</div>
              <div className={styles.headerText}>
                {validateIswcAndFormat(preferredIswc?.iswc) || validateIswcAndFormat(iswc)}
              </div>
            </div>
            {preferredIswc && (
              <div className={styles.headerItem}>
                <div className={styles.headerText}>{ORIGINAL_TITLE_FIELD}</div>
                <div className={styles.headerText}>{preferredIswc.originalTitle}</div>
              </div>
            )}
          </div>
          {this.renderGrid()}
        </div>
      </div>
    );
  }
}
