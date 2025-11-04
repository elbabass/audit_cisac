import * as React from 'react';
import { IIswcFullExtractProps, IIswcFullExtractState } from './IswcFullExtractTypes';
import styles from './IswcFullExtract.module.scss';
import IconActionButton from '../../../components/ActionButton/IconActionButton';
import { DOWNLOAD_ICON, REPORT_TYPE_ISWC_FULL_EXTRACT } from '../../../consts';
import { getStrings } from '../../../configuration/Localization';
import AlertMessage from '../../../components/AlertMessage/AlertMessage';
import ErrorHandler from '../../../components/AlertMessage/ErrorHandler/ErrorHandler';
import Loader from '../../../components/Loader/Loader';
import ExtractToFTP from '../ExtractToFTP/ExtractToFTP';
import { formatDateString } from '../../../shared/helperMethods';
import Switch from '../../../components/Switch/Switch';

class IswcFullExtract extends React.PureComponent<IIswcFullExtractProps, IIswcFullExtractState> {
  constructor(props: IIswcFullExtractProps) {
    super(props);
    this.state = {
      showExtractToFtpDiv: false,
      paramFields: {
        mostRecentVersion: true,
        reportType: REPORT_TYPE_ISWC_FULL_EXTRACT,
        email: props.email,
      },
    };
  }

  componentDidMount = () => {
    const { getDateOfCachedReport } = this.props;
    getDateOfCachedReport();
  };

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
    const { fullExtractCachedVersion, getFullExtractCachedError } = this.props;
    const { ISWC_FULL_EXTRACT_WARNING, LAST_VERSION_AVAILABLE } = getStrings();
    if (paramFields.mostRecentVersion)
      return (
        <div className={styles.alertMessageDiv}>
          {`${LAST_VERSION_AVAILABLE} ${formatDateString(fullExtractCachedVersion)}`}
          {getFullExtractCachedError && <ErrorHandler error={getFullExtractCachedError} />}
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

export default IswcFullExtract;
