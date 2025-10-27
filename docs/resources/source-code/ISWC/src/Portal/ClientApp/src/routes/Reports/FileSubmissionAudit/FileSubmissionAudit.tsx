import * as React from 'react';
import { IFileSubmissionAuditProps, IFileSubmissionAuditState } from './FileSubmissionAuditTypes';
import styles from './FileSubmissionAudit.module.scss';
import { getStrings } from '../../../configuration/Localization';
import { IGridHeaderCell, IGridRow } from '../../../components/GridComponents/Grid/GridTypes';
import GridTextCell from '../../../components/GridComponents/GridTextCell/GridTextCell';
import {
  DATE_FIELD,
  DATE_TYPE,
  SUBMITTING_AGENCY_FIELD,
  SUBMITTING_PUBLISHER_FIELD,
  SUBMITTED_FILE_NAME_FIELD,
  STATUS_FIELD,
  ACK_FILE_NAME_FIELD,
  ACK_GENERATED_FIELD,
  PROCESSING_DURATION_FIELD,
  STATUS_APPROVED_ICON,
  STATUS_REJECTED_ICON,
  STATUS_PENDING_ICON,
  getAgencies,
  TS_AGENCY,
  TS_PUBLISHER,
  TS_ALL,
  REPORT_TYPE_FILE_SUBMISSION_AUDIT,
  LOOKUP_ICON_BLACK,
} from '../../../consts';
import GridIconCell from '../../../components/GridComponents/GridIconCell/GridIconCell';
import Grid from '../../../components/GridComponents/Grid/Grid';
import DateRangeInput from '../../../components/FormInput/DateRangeInput';
import DropdownInput from '../../../components/FormInput/DropdownInput';
import GridCheckboxCell from '../../../components/GridComponents/GridCheckboxCell/GridCheckboxCell';
import { mapAgenciesToDropDownType } from '../../../shared/MappingObjects';
import {
  getLoggedInAgencyId,
  formatDateString,
  padIpNameNumber,
} from '../../../shared/helperMethods';
import Loader from '../../../components/Loader/Loader';
import ErrorHandler from '../../../components/AlertMessage/ErrorHandler/ErrorHandler';
import { IFileAuditReportResult } from '../../../redux/types/ReportTypes';
import IconActionButton from '../../../components/ActionButton/IconActionButton';

class FileSubmissionAudit extends React.PureComponent<
  IFileSubmissionAuditProps,
  IFileSubmissionAuditState
> {
  transactionSourceArray: number[];

  constructor(props: IFileSubmissionAuditProps) {
    super(props);

    this.state = {
      paramFields: {
        fromDate: this._getDate(new Date(), 7),
        toDate: this._getDate(new Date()),
        agencyName: getLoggedInAgencyId(),
        transactionSource: TS_ALL,
        reportType: REPORT_TYPE_FILE_SUBMISSION_AUDIT,
      },
    };

    this.transactionSourceArray = [TS_AGENCY, TS_PUBLISHER];
  }

  _searchFileAudit = () => {
    const { fileSubmissionAuditSearch } = this.props;
    const { paramFields } = this.state;

    fileSubmissionAuditSearch(paramFields);
  };

  _updateParameter = (event: any) => {
    const name = event.target.id;
    let value = event.target.value;

    this.setState({
      paramFields: {
        ...this.state.paramFields,
        [name]: value,
      },
    });
  };

  _getDate = (dateParam: Date, daysPrevious?: number) => {
    let date = dateParam || new Date();

    if (daysPrevious) {
      date.setDate(date.getDate() - daysPrevious);
    }

    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();

    return `${year}-${month}-${day}`;
  };

  _handleTransactionSourceSelect = (type: number) => {
    if (!this.transactionSourceArray.includes(type)) {
      this.transactionSourceArray = [...this.transactionSourceArray.concat(type)];
    } else {
      this.transactionSourceArray = [
        ...this.transactionSourceArray.filter((source) => source !== type),
      ];
    }

    if (this.transactionSourceArray.length === 2 || this.transactionSourceArray.length === 0) {
      this.setState({
        paramFields: {
          ...this.state.paramFields,
          transactionSource: TS_ALL,
        },
      });
    } else {
      this.setState({
        paramFields: {
          ...this.state.paramFields,
          transactionSource: this.transactionSourceArray[0],
        },
      });
    }
  };

  _getTimeDifference = (result: IFileAuditReportResult) => {
    if (result.dateAckGenerated === undefined) return '';

    let date1: any = new Date(result.dateAckGenerated);
    let date2: any = new Date(result.datePickedUp);

    let differenceMilliseconds = Math.abs(date1 - date2);

    const portions: string[] = [];

    const msInHour = 1000 * 60 * 60;
    const hours = Math.trunc(differenceMilliseconds / msInHour);
    if (hours > 0) {
      portions.push(hours + 'h');
      differenceMilliseconds = differenceMilliseconds - hours * msInHour;
    }

    const msInMinute = 1000 * 60;
    const minutes = Math.trunc(differenceMilliseconds / msInMinute);
    if (minutes > 0) {
      portions.push(minutes + 'm');
      differenceMilliseconds = differenceMilliseconds - minutes * msInMinute;
    }

    const seconds = Math.trunc(differenceMilliseconds / 1000);
    if (seconds > 0) {
      portions.push(seconds + 's');
    }

    return portions.join(' ');
  };

  _getHeaderCells = (): IGridHeaderCell[] => {
    const strings = getStrings();
    return [
      { text: strings[DATE_FIELD], field: DATE_FIELD, sortable: true, type: DATE_TYPE },
      {
        text: strings[SUBMITTING_AGENCY_FIELD],
        field: SUBMITTING_AGENCY_FIELD,
        sortable: true,
      },
      {
        text: strings[SUBMITTING_PUBLISHER_FIELD],
        field: SUBMITTING_PUBLISHER_FIELD,
        sortable: true,
      },
      {
        text: strings[SUBMITTED_FILE_NAME_FIELD],
        field: SUBMITTED_FILE_NAME_FIELD,
      },
      {
        text: strings[STATUS_FIELD],
        field: STATUS_FIELD,
      },
      { text: strings[ACK_FILE_NAME_FIELD], field: ACK_FILE_NAME_FIELD },
      {
        text: strings[ACK_GENERATED_FIELD],
        field: ACK_GENERATED_FIELD,
      },
      {
        text: strings[PROCESSING_DURATION_FIELD],
        field: PROCESSING_DURATION_FIELD,
      },
    ];
  };

  _getGridData = (): IGridRow[] => {
    const { fileSubmissionAuditSearchResults } = this.props;
    const gridRows: IGridRow[] = [];

    fileSubmissionAuditSearchResults?.forEach((result, index) => {
      gridRows.push({
        rowId: index,
        cells: [
          {
            element: <GridTextCell text={formatDateString(result.datePickedUp)} />,
            field: DATE_FIELD,
          },
          { element: <GridTextCell text={result.agencyCode} />, field: SUBMITTING_AGENCY_FIELD },
          {
            element: <GridTextCell text={padIpNameNumber(result.publisherNameNumber)} />,
            field: SUBMITTING_PUBLISHER_FIELD,
          },
          {
            element: <GridTextCell text={result.fileName} />,
            field: SUBMITTED_FILE_NAME_FIELD,
          },
          { element: this.renderStatusIcon(result.status), field: STATUS_FIELD },
          { element: <GridTextCell text={result.ackFileName} />, field: ACK_FILE_NAME_FIELD },
          {
            element: <GridTextCell text={formatDateString(result.dateAckGenerated)} />,
            field: ACK_GENERATED_FIELD,
          },
          {
            element: <GridTextCell text={this._getTimeDifference(result)} />,
            field: PROCESSING_DURATION_FIELD,
          },
        ],
      });
    });

    return gridRows;
  };

  renderStatusIcon = (status: string) => {
    let statusIcon = STATUS_APPROVED_ICON;

    if (status === 'Finished Error') {
      statusIcon = STATUS_REJECTED_ICON;
    } else if (status === 'In Progress') {
      statusIcon = STATUS_PENDING_ICON;
    }

    return <GridIconCell icon={statusIcon} alt={status} hoverText={status} />;
  };

  renderFilterDiv = () => {
    const { paramFields } = this.state;
    const { AGENCY_NAME_FIELD, TRANSACTION_SOURCE, AGENCY, PUBLISHER } = getStrings();

    return (
      <div className={styles.filterDiv}>
        <div className={styles.dateFilter}>
          <DateRangeInput
            changeFromDate={(event) => this._updateParameter(event)}
            changeToDate={(event) => this._updateParameter(event)}
            fromDate={paramFields.fromDate}
            toDate={paramFields.toDate}
          />
        </div>
        <div className={styles.dropdownCheckboxDiv}>
          <div className={styles.dropdownDiv}>
            <DropdownInput
              label={AGENCY_NAME_FIELD}
              name={'agencyName'}
              onChange={(event) => this._updateParameter(event)}
              options={[{ value: 'All', name: '<All>' }, ...mapAgenciesToDropDownType(getAgencies())]}
              defaultToLoggedInAgency={true}
            />
          </div>
          <div className={styles.checkBoxDiv}>
            <div className={styles.label}>{TRANSACTION_SOURCE}:</div>
            <div className={styles.checkBoxRow}>
              <div className={styles.checkBox}>
                <GridCheckboxCell
                  text={AGENCY}
                  onClickCheckbox={() => this._handleTransactionSourceSelect(TS_AGENCY)}
                  checked={this.transactionSourceArray.includes(TS_AGENCY)}
                />
              </div>
              <div className={styles.checkBox}>
                <GridCheckboxCell
                  text={PUBLISHER}
                  onClickCheckbox={() => this._handleTransactionSourceSelect(TS_PUBLISHER)}
                  checked={this.transactionSourceArray.includes(TS_PUBLISHER)}
                />
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  };

  renderMainView = () => {
    const { loading, fileSubmissionAuditSearchResults, error } = this.props;
    const { LIST_VIEW, NO_RECORDS } = getStrings();

    if (loading) return <Loader />;

    if (error) return <ErrorHandler error={error} />;

    if (fileSubmissionAuditSearchResults && fileSubmissionAuditSearchResults.length > 0) {
      return (
        <div>
          <div className={styles.gridTitle}>{LIST_VIEW}</div>
          <Grid headerCells={this._getHeaderCells()} gridRows={this._getGridData()} pagination />
        </div>
      );
    }

    if (fileSubmissionAuditSearchResults && fileSubmissionAuditSearchResults.length === 0)
      return <ErrorHandler error={{ response: { data: { message: NO_RECORDS } } }} />;
  };

  render() {
    const { fileSubmissionAuditSearch } = this.props;
    const { paramFields } = this.state;
    const { DISPLAY_LIST_VIEW } = getStrings();
    return (
      <div className={styles.container}>
        {this.renderFilterDiv()}
        <div className={styles.reportTypeSelectDiv}>
          <div className={styles.actionButton}>
            <IconActionButton
              icon={LOOKUP_ICON_BLACK}
              buttonText={DISPLAY_LIST_VIEW}
              theme={'secondary'}
              buttonAction={() => fileSubmissionAuditSearch(paramFields)}
              bigIcon={true}
            />
          </div>
        </div>

        {this.renderMainView()}
      </div>
    );
  }
}

export default FileSubmissionAudit;
