import * as React from 'react';
import {
  IPublisherIswcTrackingProps,
  IPublisherIswcTrackingState,
} from './PublisherIswcTrackingTypes';
import styles from './PublisherIswcTracking.module.scss';
import DateRangeInput from '../../../components/FormInput/DateRangeInput';
import DropdownInput from '../../../components/FormInput/DropdownInput';
import { getStrings } from '../../../configuration/Localization';
import { mapAgenciesToDropDownType } from '../../../shared/MappingObjects';
import { getAgencies, REPORT_TYPE_PUBLISHER_ISWC_TRACKING, DOWNLOAD_ICON } from '../../../consts';
import { getDate, getLoggedInAgencyId } from '../../../shared/helperMethods';
import IconActionButton from '../../../components/ActionButton/IconActionButton';
import ExtractToFTP from '../ExtractToFTP/ExtractToFTP';
import Loader from '../../../components/Loader/Loader';
import ErrorHandler from '../../../components/AlertMessage/ErrorHandler/ErrorHandler';

class PublisherIswcTracking extends React.PureComponent<
  IPublisherIswcTrackingProps,
  IPublisherIswcTrackingState
> {
  constructor(props: IPublisherIswcTrackingProps) {
    super(props);
    this.state = {
      showExtractToFtpDiv: false,
      paramFields: {
        fromDate: getDate(new Date(), 7),
        toDate: getDate(new Date()),
        agencyName: getLoggedInAgencyId(),
        reportType: REPORT_TYPE_PUBLISHER_ISWC_TRACKING,
        agencyWorkCode: undefined,
        transactionSource: undefined,
        status: undefined,
        email: props.email,
      },
    };
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

  _extractToFtp = () => {
    const { extractToFtp } = this.props;
    const { paramFields } = this.state;

    this.setState({ showExtractToFtpDiv: true });
    extractToFtp(paramFields);
  };

  renderFilterDiv() {
    const { paramFields } = this.state;
    const { AGENCY_NAME_FIELD } = getStrings();

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
        <div className={styles.dropdownDiv}>
          <div className={styles.dropdownInput}>
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
        </div>
      </div>
    );
  }

  renderExtractToFtpDiv = () => {
    const { loading, error, extractToFtpSuccess } = this.props;
    const { showExtractToFtpDiv } = this.state;

    if (loading) return <Loader />;
    if (error) return <ErrorHandler error={error} />;
    if (showExtractToFtpDiv && extractToFtpSuccess) return <ExtractToFTP />;

    return <div />;
  };

  render() {
    const { EXTRACT_TO_FTP } = getStrings();
    return (
      <div className={styles.container}>
        {this.renderFilterDiv()}
        <div className={styles.actionButton}>
          <IconActionButton
            icon={DOWNLOAD_ICON}
            buttonText={EXTRACT_TO_FTP}
            theme={'secondary'}
            buttonAction={this._extractToFtp}
          />
        </div>
        {this.renderExtractToFtpDiv()}
      </div>
    );
  }
}

export default PublisherIswcTracking;
