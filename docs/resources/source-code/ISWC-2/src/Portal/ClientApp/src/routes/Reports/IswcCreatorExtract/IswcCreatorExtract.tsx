import * as React from 'react';
import { IIswcCreatorExtractProps, IIswcCreatorExtractState } from './IswcCreatorExtractTypes';
import styles from './IswcCreatorExtract.module.scss';
import IconActionButton from '../../../components/ActionButton/IconActionButton';
import { DOWNLOAD_ICON, REPORT_TYPE_ISWC_CREATOR_EXTRACT } from '../../../consts';
import { getStrings } from '../../../configuration/Localization';
import AlertMessage from '../../../components/AlertMessage/AlertMessage';
import ErrorHandler from '../../../components/AlertMessage/ErrorHandler/ErrorHandler';
import Loader from '../../../components/Loader/Loader';
import ExtractToFTP from '../ExtractToFTP/ExtractToFTP';
import { formatDateString } from '../../../shared/helperMethods';
import Switch from '../../../components/Switch/Switch';
import BasicFormInput from '../../../components/FormInput/BasicFormInput';

class IswcCreatorExtract extends React.PureComponent<
  IIswcCreatorExtractProps,
  IIswcCreatorExtractState
> {
  constructor(props: IIswcCreatorExtractProps) {
    super(props);
    this.state = {
      showExtractToFtpDiv: false,
      paramFields: {
        mostRecentVersion: true,
        reportType: REPORT_TYPE_ISWC_CREATOR_EXTRACT,
        email: props.email,
        creatorNameNumber: '',
        creatorBaseNumber: '',
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

    if (name === 'creatorNameNumber') {
      value = value.replace(/[^0-9]/g, '');
    }

    this.setState({
      paramFields: {
        ...this.state.paramFields,
        [name]: value,
      },
    });
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
    const { creatorExtractCachedVersion, getCreatorExtractCachedError } = this.props;
    const { ISWC_CREATOR_EXTRACT_WARNING, LAST_VERSION_AVAILABLE } = getStrings();
    if (paramFields.mostRecentVersion)
      return (
        <div className={styles.alertMessageDiv}>
          {`${LAST_VERSION_AVAILABLE} ${formatDateString(creatorExtractCachedVersion)}`}
          {getCreatorExtractCachedError && <ErrorHandler error={getCreatorExtractCachedError} />}
        </div>
      );
    else
      return (
        <div className={styles.alertMessageDiv}>
          <AlertMessage message={ISWC_CREATOR_EXTRACT_WARNING} type={'warn'} />
        </div>
      );
  };

  renderExtractToFtpDiv = () => {
    const { loading, error, extractToFtpSuccess } = this.props;
    const { showExtractToFtpDiv } = this.state;

    if (loading) return <Loader />;
    if (error) return <ErrorHandler error={error} />;
    if (showExtractToFtpDiv && extractToFtpSuccess) return <ExtractToFTP fileType="json" />;

    return <div />;
  };

  render() {
    const { paramFields } = this.state;
    const { assignedRoles } = this.props;
    const {
      EXTRACT_TO_FTP_JSON,
      USE_MOST_RECENT_VERSION,
      ENTER_CREATOR_DATA_REPORT,
      IP_NAME_NUMBER_FIELD,
      IP_BASE_NUMBER_FIELD,
      OR,
    } = getStrings();
    return (
      <div className={styles.container}>
        <form
          autoComplete="off"
          onSubmit={(e) => {
            e.preventDefault();
          }}
        >
          <div className={styles.descriptionText}>{ENTER_CREATOR_DATA_REPORT}</div>
          <div className={styles.formRow}>
            <div className={styles.formInput}>
              <BasicFormInput
                label={IP_NAME_NUMBER_FIELD}
                name={'creatorNameNumber'}
                onChange={(event) => this._updateParameter(event)}
                disabled={
                  paramFields.creatorBaseNumber != undefined &&
                  paramFields.creatorBaseNumber.length > 0
                }
                value={paramFields.creatorNameNumber}
              />
            </div>
            <div className={styles.orDiv}>{OR}</div>
            <div className={styles.formInput}>
              <BasicFormInput
                label={IP_BASE_NUMBER_FIELD}
                name={'creatorBaseNumber'}
                onChange={(event) => this._updateParameter(event)}
                disabled={
                  paramFields.creatorNameNumber != undefined &&
                  paramFields.creatorNameNumber.length > 0
                }
                value={paramFields.creatorBaseNumber}
              />
            </div>
          </div>
        </form>
        {assignedRoles.reportIswcFullExtract && (
          <Switch
            text={USE_MOST_RECENT_VERSION}
            active={paramFields.mostRecentVersion}
            toggleSwitch={() => this._toggleSwitch()}
          />
        )}
        <div>{this.renderSwitchDetails()}</div>
        <div className={styles.reportTypeSelectDiv}>
          <div className={styles.actionButton}>
            <IconActionButton
              icon={DOWNLOAD_ICON}
              buttonText={EXTRACT_TO_FTP_JSON}
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

export default IswcCreatorExtract;
