import * as React from 'react';
import { IPotentialDuplicatesProps, IPotentialDuplicatesState } from './PotentialDuplicatesTypes';
import styles from './PotentialDuplicates.module.scss';
import { DOWNLOAD_ICON, getAgencies, REPORT_TYPE_POTENTIAL_DUPLICATES } from '../../../consts';
import { formatDateString, getLoggedInAgencyId } from '../../../shared/helperMethods';
import { getStrings } from '../../../configuration/Localization';
import Switch from '../../../components/Switch/Switch';
import DropdownInput from '../../../components/FormInput/DropdownInput';
import { mapAgenciesToDropDownType } from '../../../shared/MappingObjects';
import DateRangeInput from '../../../components/FormInput/DateRangeInput';
import ExtractToFTP from '../ExtractToFTP/ExtractToFTP';
import IconActionButton from '../../../components/ActionButton/IconActionButton';
import Loader from '../../../components/Loader/Loader';
import ErrorHandler from '../../../components/AlertMessage/ErrorHandler/ErrorHandler';
import AlertMessage from '../../../components/AlertMessage/AlertMessage';

class PotentialDuplicates extends React.PureComponent<
  IPotentialDuplicatesProps,
  IPotentialDuplicatesState
> {
  constructor(props: IPotentialDuplicatesProps) {
    super(props);
    this.state = {
      showExtractToFtpDiv: false,
      paramFields: {
        reportType: REPORT_TYPE_POTENTIAL_DUPLICATES,
        agencyName: getLoggedInAgencyId(),
        fromDate: undefined,
        toDate: undefined,
        mostRecentVersion: true,
        considerOriginalTitlesOnly: true,
        email: props.email,
      },
    };
  }

  componentDidMount = () => {
    const { getDateOfCachedReport } = this.props;
    getDateOfCachedReport();
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

  _toggleSwitch = (considerOriginalTitlesOnly: boolean) => {
    const { paramFields } = this.state;
    if (considerOriginalTitlesOnly) {
      this.setState({
        paramFields: {
          ...paramFields,
          considerOriginalTitlesOnly: !paramFields.considerOriginalTitlesOnly,
        },
      });
    } else {
      this.setState({
        paramFields: { ...paramFields, mostRecentVersion: !paramFields.mostRecentVersion },
      });
    }
  };

  _extractToFtp = () => {
    const { extractToFtp } = this.props;
    const { paramFields } = this.state;

    this.setState({ showExtractToFtpDiv: true });
    extractToFtp(paramFields);
  };

  renderSwitchDetails = () => {
    const { paramFields } = this.state;
    const { potentialDuplicatesCachedVersion, getPotentialDuplicatesCachedError } = this.props;
    const { ISWC_FULL_EXTRACT_WARNING, LAST_VERSION_AVAILABLE } = getStrings();
    if (paramFields.mostRecentVersion)
      return (
        <div className={styles.alertMessageDiv}>
          {`${LAST_VERSION_AVAILABLE} ${formatDateString(potentialDuplicatesCachedVersion)}`}
          {getPotentialDuplicatesCachedError && (
            <ErrorHandler error={getPotentialDuplicatesCachedError} />
          )}
        </div>
      );
    else
      return (
        <div className={styles.alertMessageDiv}>
          <AlertMessage message={ISWC_FULL_EXTRACT_WARNING} type={'warn'} />
        </div>
      );
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
    const { paramFields } = this.state;
    const { assignedRoles } = this.props;
    const {
      USE_MOST_RECENT_VERSION,
      AGENCY_NAME_FIELD,
      CONSIDER_OT_ONLY,
      PD_DATE_RANGE,
      PD_LEAVE,
      EXTRACT_TO_FTP,
    } = getStrings();

    return (
      <div className={styles.container}>
        <div className={styles.dropdownSwitchDiv}>
          <div className={styles.dropdownDiv}>
            <DropdownInput
              label={AGENCY_NAME_FIELD}
              name={'agencyName'}
              onChange={(event) => this._updateParameter(event)}
              options={mapAgenciesToDropDownType(getAgencies())}
              defaultToLoggedInAgency={true}
            />
          </div>
          <Switch
            text={CONSIDER_OT_ONLY}
            active={paramFields.considerOriginalTitlesOnly}
            toggleSwitch={() => this._toggleSwitch(true)}
            textTopPosition
          />
        </div>
        <div className={styles.textDiv}>
          <b>{PD_DATE_RANGE}</b>
          <div>{PD_LEAVE}</div>
        </div>
        <div className={styles.dateDiv}>
          <DateRangeInput
            changeFromDate={(event) => this._updateParameter(event)}
            changeToDate={(event) => this._updateParameter(event)}
            fromDate={paramFields.fromDate}
            toDate={paramFields.toDate}
          />
        </div>
        {assignedRoles.reportIswcFullExtract && (
          <Switch
            text={USE_MOST_RECENT_VERSION}
            active={paramFields.mostRecentVersion}
            toggleSwitch={() => this._toggleSwitch(false)}
          />
        )}
        <div>{this.renderSwitchDetails()}</div>
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

export default PotentialDuplicates;
