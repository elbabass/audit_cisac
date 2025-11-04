import * as React from 'react';
import styles from './AgencyInterestExtract.module.scss';
import {
  IAgencyInterestExtractProps,
  IAgencyInterestExtractState,
} from './AgencyInterestExtractTypes';
import IconActionButton from '../../../components/ActionButton/IconActionButton';
import {
  DOWNLOAD_ICON,
  getAgencies,
  TS_ALL,
  TS_AGENCY,
  TS_PUBLISHER,
  REPORT_TYPE_AGENCY_INTEREST_EXTRACT,
} from '../../../consts';
import { getStrings } from '../../../configuration/Localization';
import DateRangeInput from '../../../components/FormInput/DateRangeInput';
import DropdownInput from '../../../components/FormInput/DropdownInput';
import { mapAgenciesToDropDownType } from '../../../shared/MappingObjects';
import GridCheckboxCell from '../../../components/GridComponents/GridCheckboxCell/GridCheckboxCell';
import { getLoggedInAgencyId, getDate } from '../../../shared/helperMethods';
import Loader from '../../../components/Loader/Loader';
import ExtractToFTP from '../ExtractToFTP/ExtractToFTP';
import ErrorHandler from '../../../components/AlertMessage/ErrorHandler/ErrorHandler';

class AgencyInterestExtract extends React.PureComponent<
  IAgencyInterestExtractProps,
  IAgencyInterestExtractState
> {
  transactionSourceArray: number[];
  constructor(props: IAgencyInterestExtractProps) {
    super(props);

    this.state = {
      showExtractToFtpDiv: false,
      paramFields: {
        fromDate: getDate(new Date(), 7),
        toDate: getDate(new Date()),
        agencyName: getLoggedInAgencyId(),
        transactionSource: TS_ALL,
        agreementFromDate: getDate(new Date(), 7),
        agreementToDate: getDate(new Date()),
        reportType: REPORT_TYPE_AGENCY_INTEREST_EXTRACT,
        email: props.email,
      },
    };

    this.transactionSourceArray = [TS_AGENCY, TS_PUBLISHER];
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

  _extractToFtp = () => {
    const { extractToFtp } = this.props;
    const { paramFields } = this.state;

    this.setState({ showExtractToFtpDiv: true });
    extractToFtp(paramFields);
  };

  renderExtractToFtpDiv = () => {
    const { loading, error, extractToFtpSuccess } = this.props;
    const { showExtractToFtpDiv } = this.state;

    if (loading) return <Loader />;
    if (error) return <ErrorHandler error={error} />;
    if (showExtractToFtpDiv && extractToFtpSuccess) return <ExtractToFTP />;

    return <div />;
  };

  render() {
    const {
      EXTRACT_TO_FTP,
      AGENCY_NAME_FIELD,
      TRANSACTION_SOURCE,
      PUBLISHER,
      AGENCY,
      IP_AGREEMENT,
    } = getStrings();
    const { paramFields } = this.state;

    return (
      <div className={styles.container}>
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
                  options={mapAgenciesToDropDownType(getAgencies())}
                  defaultToLoggedInAgency={true}
                />
              </div>
              <div className={styles.textfieldDiv}>
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
          <div className={styles.bottomFilterDiv}>
            <div className={styles.label}>{IP_AGREEMENT}</div>
            <div className={styles.dateFilter}>
              <DateRangeInput
                changeFromDate={(event) => this._updateParameter(event)}
                changeToDate={(event) => this._updateParameter(event)}
                fromDate={paramFields.agreementFromDate}
                toDate={paramFields.agreementToDate}
                fromDateName={'agreementFromDate'}
                toDateName={'agreementToDate'}
              />
            </div>
          </div>
        </div>
        <div className={styles.reportTypeSelectDiv}>
          <div className={styles.actionButton}>
            <IconActionButton
              icon={DOWNLOAD_ICON}
              buttonText={EXTRACT_TO_FTP}
              theme={'secondary'}
              buttonAction={this._extractToFtp}
            />
          </div>
        </div>
        {this.renderExtractToFtpDiv()}
      </div>
    );
  }
}

export default AgencyInterestExtract;
