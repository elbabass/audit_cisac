import * as React from 'react';
import styles from './SubmissionAudit.module.scss';
import { ISubmissionAuditProps, ISubmissionAuditState } from './SubmissionAuditTypes';
import IconActionButton from '../../../components/ActionButton/IconActionButton';
import {
  DOWNLOAD_ICON,
  REPORT_ICON_BLACK,
  LOOKUP_ICON_BLACK,
  getAgencies,
  STATUS_SUCCEEDED,
  STATUS_ERROR,
  STATUS_ALL,
  TS_ALL,
  TS_PUBLISHER,
  TS_AGENCY,
  REPORT_TYPE_SUBMISSION_AUDIT,
  SUBMISSION_AUDIT_LIST_VIEW,
  SUBMISSION_AUDIT_BAR_CHART,
  SUBMISSION_AUDIT_EXTRACT_FTP,
  PER_MONTH_PERIOD,
} from '../../../consts';
import { getStrings } from '../../../configuration/Localization';
import DateRangeInput from '../../../components/FormInput/DateRangeInput';
import DropdownInput from '../../../components/FormInput/DropdownInput';
import GridCheckboxCell from '../../../components/GridComponents/GridCheckboxCell/GridCheckboxCell';
import BasicFormInput from '../../../components/FormInput/BasicFormInput';
import SubmissionAuditGrid from './SubmissionAuditGrid/SubmissionAuditGrid';
import BarChart from './BarChart/BarChart';
import AlertMessage from '../../../components/AlertMessage/AlertMessage';
import { mapAgenciesToDropDownType } from '../../../shared/MappingObjects';
import { getLoggedInAgencyId, getDate } from '../../../shared/helperMethods';
import Loader from '../../../components/Loader/Loader';
import ErrorHandler from '../../../components/AlertMessage/ErrorHandler/ErrorHandler';
import ExtractToFTP from '../ExtractToFTP/ExtractToFTP';

class SubmissionAudit extends React.PureComponent<ISubmissionAuditProps, ISubmissionAuditState> {
  transactionSourceArray: number[];
  numberOfDaysInMonth: number;
  constructor(props: ISubmissionAuditProps) {
    super(props);
    this.state = {
      displayView: 0,
      paramFields: {
        fromDate: getDate(new Date(), 7),
        toDate: getDate(new Date()),
        agencyName: getLoggedInAgencyId(),
        agencyWorkCode: undefined,
        transactionSource: TS_ALL,
        status: STATUS_ALL,
        reportType: REPORT_TYPE_SUBMISSION_AUDIT,
        email: props.email,
      },
    };

    this.transactionSourceArray = [TS_AGENCY, TS_PUBLISHER];
    this.numberOfDaysInMonth = 0;
  }

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

  _handleStatusSelect = (type: number) => {
    this.setState({
      paramFields: {
        ...this.state.paramFields,
        status: type,
      },
    });
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

  _searchAndSetMainDiv = (displayView: number) => {
    const { submissionAuditSearch, extractToFtp, reportsAgencyStatisticsSearch } = this.props;
    const { paramFields } = this.state;

    if (displayView === SUBMISSION_AUDIT_LIST_VIEW) {
      submissionAuditSearch(paramFields);
    } else if (displayView === SUBMISSION_AUDIT_BAR_CHART) {
      const { fromDate } = this.state.paramFields;
      const date = new Date(fromDate!);
      const startOfMonth = new Date(date.getFullYear(), date.getMonth(), 1);
      const endOfMonth = new Date(
        date.getFullYear(),
        date.getMonth(),
        this._getLastDayInMonth(date.getFullYear(), date.getMonth()),
      );

      this.setState(
        {
          paramFields: {
            ...paramFields,
            fromDate: getDate(startOfMonth),
            toDate: getDate(endOfMonth),
            status: STATUS_ALL,
            agencyWorkCode: '',
          },
        },
        () =>
          reportsAgencyStatisticsSearch({
            agencyName: paramFields.agencyName ?? '',
            month: startOfMonth.getMonth() + 1,
            year: startOfMonth.getFullYear(),
            timePeriod: PER_MONTH_PERIOD,
            transactionSource: paramFields.transactionSource,
          }),
      );
    } else if (displayView === SUBMISSION_AUDIT_EXTRACT_FTP) {
      extractToFtp(paramFields);
    }

    if (displayView !== this.state.displayView) {
      this.setState({
        displayView: displayView,
      });
    }
  };

  _getLastDayInMonth = (year: number, month: number) => {
    var d = new Date(year, month + 1, 0);
    this.numberOfDaysInMonth = d.getDate();
    return d.getDate();
  };

  renderFilterDiv = () => {
    const { paramFields } = this.state;
    const {
      AGENCY_NAME_FIELD,
      OPTIONAL,
      AGENCY_WORK_CODE_FIELD,
      STATUS_FIELD,
      TRANSACTION_SOURCE,
      ALL,
      SUCCEEDED,
      ERROR,
      AGENCY,
      PUBLISHER,
    } = getStrings();

    return (
      <div className={styles.filterDiv}>
        <div className={`${styles.topFilterDiv} ${styles.topFilterDivIE}`}>
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
                options={[
                  { value: 'All', name: '<All>' },
                  ...mapAgenciesToDropDownType(getAgencies()),
                ]}
                defaultToLoggedInAgency={true}
              />
            </div>
            <div className={styles.textfieldDiv}>
              <BasicFormInput
                name={'agencyWorkCode'}
                label={`${AGENCY_WORK_CODE_FIELD} ${OPTIONAL}`}
                onChange={(event) => this._updateParameter(event)}
                value={paramFields.agencyWorkCode}
              />
            </div>
          </div>
        </div>
        <div className={styles.bottomFilterDiv}>
          <div className={styles.checkboxDivLeft}>
            <div className={styles.label}>{STATUS_FIELD}:</div>
            <div className={styles.checkBoxRow}>
              <div className={styles.checkBox}>
                <GridCheckboxCell
                  text={ALL}
                  onClickCheckbox={() => this._handleStatusSelect(STATUS_ALL)}
                  checked={paramFields.status === STATUS_ALL}
                />
              </div>
              <div className={styles.checkBox}>
                <GridCheckboxCell
                  text={ERROR}
                  onClickCheckbox={() => this._handleStatusSelect(STATUS_ERROR)}
                  checked={paramFields.status === STATUS_ERROR}
                />
              </div>
              <div className={styles.checkBox}>
                <GridCheckboxCell
                  text={SUCCEEDED}
                  onClickCheckbox={() => this._handleStatusSelect(STATUS_SUCCEEDED)}
                  checked={paramFields.status === STATUS_SUCCEEDED}
                />
              </div>
            </div>
          </div>
          <div className={styles.checkboxDivRight}>
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
    const { displayView } = this.state;
    const {
      loading,
      error,
      submissionAuditSearchResults,
      agencyStatisticsSearchResultsSubAudit,
      extractToFtpSuccess,
    } = this.props;
    const { NO_RECORDS } = getStrings();

    if (loading) return <Loader />;

    if (error) return <ErrorHandler error={error} />;

    if (displayView === 0) {
      return <div />;
    } else if (displayView === SUBMISSION_AUDIT_LIST_VIEW) {
      if (submissionAuditSearchResults && submissionAuditSearchResults.length === 0) {
        return <ErrorHandler error={{ response: { data: { message: NO_RECORDS } } }} />;
      }
      return <SubmissionAuditGrid submissionAuditSearchResults={submissionAuditSearchResults} />;
    } else if (
      displayView === SUBMISSION_AUDIT_BAR_CHART &&
      agencyStatisticsSearchResultsSubAudit
    ) {
      if (agencyStatisticsSearchResultsSubAudit.length === 0) {
        return <ErrorHandler error={{ response: { data: { message: NO_RECORDS } } }} />;
      }
      return (
        <BarChart
          agencyStatisticsSearchResults={agencyStatisticsSearchResultsSubAudit}
          numberOfDaysInMonth={this.numberOfDaysInMonth}
        />
      );
    } else if (displayView === SUBMISSION_AUDIT_EXTRACT_FTP && extractToFtpSuccess) {
      return <ExtractToFTP />;
    }
  };

  render() {
    const { submissionAuditSearchResults, assignedRoles } = this.props;
    const {
      DISPLAY_LIST_VIEW,
      DISPLAY_ERROR_SUCCESS_GRAPH,
      EXTRACT_TO_FTP,
      CHOOSE_REPORT_TYPE,
      DISPLAY_LIST_VIEW_MESSAGE,
    } = getStrings();

    return (
      <div className={styles.container}>
        {this.renderFilterDiv()}
        <div className={styles.text}>{CHOOSE_REPORT_TYPE}</div>
        {submissionAuditSearchResults && submissionAuditSearchResults.length === 1000 && (
          <div className={styles.alertMessageDiv}>
            <AlertMessage message={DISPLAY_LIST_VIEW_MESSAGE} type={'info'} />
          </div>
        )}
        <div className={styles.reportTypeSelectDiv}>
          <div className={styles.actionButton}>
            <IconActionButton
              icon={LOOKUP_ICON_BLACK}
              buttonText={DISPLAY_LIST_VIEW}
              theme={'secondary'}
              buttonAction={() => this._searchAndSetMainDiv(SUBMISSION_AUDIT_LIST_VIEW)}
              bigIcon={true}
            />
          </div>
          <div className={`${styles.actionButton} ${styles.actionButtonBig}`}>
            <IconActionButton
              icon={REPORT_ICON_BLACK}
              buttonText={DISPLAY_ERROR_SUCCESS_GRAPH}
              theme={'secondary'}
              buttonAction={() => this._searchAndSetMainDiv(SUBMISSION_AUDIT_BAR_CHART)}
            />
          </div>
          {assignedRoles.reportExtracts && (
            <div className={styles.actionButton}>
              <IconActionButton
                icon={DOWNLOAD_ICON}
                buttonText={EXTRACT_TO_FTP}
                theme={'secondary'}
                buttonAction={() => this._searchAndSetMainDiv(SUBMISSION_AUDIT_EXTRACT_FTP)}
              />
            </div>
          )}
        </div>
        <div className={styles.mainView}>{this.renderMainView()}</div>
      </div>
    );
  }
}

export default SubmissionAudit;
