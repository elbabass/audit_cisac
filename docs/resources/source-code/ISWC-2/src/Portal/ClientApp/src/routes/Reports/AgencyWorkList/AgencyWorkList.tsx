import * as React from 'react';
import { IAgencyWorkListProps, IAgencyWorkListState } from './AgencyWorkListTypes';
import styles from './AgencyWorkList.module.scss';
import IconActionButton from '../../../components/ActionButton/IconActionButton';
import { DOWNLOAD_ICON, REPORT_TYPE_AGENCY_WORK_LIST } from '../../../consts';
import { getStrings } from '../../../configuration/Localization';
import AlertMessage from '../../../components/AlertMessage/AlertMessage';
import ErrorHandler from '../../../components/AlertMessage/ErrorHandler/ErrorHandler';
import Loader from '../../../components/Loader/Loader';
import ExtractToFTP from '../ExtractToFTP/ExtractToFTP';
import { formatDateString } from '../../../shared/helperMethods';
import Switch from '../../../components/Switch/Switch';

class AgencyWorkList extends React.PureComponent<IAgencyWorkListProps, IAgencyWorkListState> {
  constructor(props: IAgencyWorkListProps) {
    super(props);
    this.state = {
      showExtractToFtpDiv: false,
      paramFields: {
        reportType: REPORT_TYPE_AGENCY_WORK_LIST,
        email: props.email,
        mostRecentVersion: !(props.agencyWorkListCachedVersion === ''),
      },
    };
  }

  componentDidMount = () => {
    const { getDateOfCachedReport } = this.props;
    getDateOfCachedReport();
  };

  componentDidUpdate = (prevProps: IAgencyWorkListProps, prevState: IAgencyWorkListState) => {
      const { agencyWorkListCachedVersion } = this.props;
      const { paramFields } = this.state;

      if (agencyWorkListCachedVersion && !prevProps.agencyWorkListCachedVersion) {
          this.setState({
              paramFields: { ...paramFields, mostRecentVersion: !(agencyWorkListCachedVersion === '') },
          });
      }
    }

  _toggleSwitch = () => {
    const { paramFields } = this.state;
    this.setState({
      paramFields: { ...paramFields, mostRecentVersion: !paramFields.mostRecentVersion },
    });
  };

  _extractToFtp = () => {
    const { extractToFtp } = this.props;
    const { paramFields } = this.state;

    this.setState({ showExtractToFtpDiv: true });
    extractToFtp(paramFields);
  };

  renderSwitchDetails = () => {
    const { paramFields } = this.state;
    const { agencyWorkListCachedVersion, getAgencyWorkListCachedError } = this.props;
    const { AGENCY_WORK_LIST_WARNING, LAST_VERSION_AVAILABLE } = getStrings();
    if (paramFields.mostRecentVersion)
      return (
        <div className={styles.alertMessageDiv}>
          {`${LAST_VERSION_AVAILABLE} ${formatDateString(agencyWorkListCachedVersion)}`}
          {getAgencyWorkListCachedError && <ErrorHandler error={getAgencyWorkListCachedError} />}
        </div>
      );
    else
      return (
        <div className={styles.alertMessageDiv}>
          <AlertMessage message={AGENCY_WORK_LIST_WARNING} type={'warn'} />
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
    const { EXTRACT_TO_FTP, USE_MOST_RECENT_VERSION } = getStrings();
    
    return (
      <div className={styles.container}>
            {assignedRoles.reportIswcFullExtract && (
          <Switch
            text={USE_MOST_RECENT_VERSION}
            active={paramFields.mostRecentVersion}
            toggleSwitch={this._toggleSwitch}
          />
        )}
        <div>{this.renderSwitchDetails()}</div>
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

export default AgencyWorkList;
