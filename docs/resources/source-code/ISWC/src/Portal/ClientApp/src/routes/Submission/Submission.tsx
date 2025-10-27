import * as React from 'react';
import styles from './Submission.module.scss';
import {
  ISubmissionProps,
  ISubmissionState,
  ISubmissionStateKeys,
  ISubmissionMainDetailsStateObjectKeys,
} from './SubmissionTypes';
import SubmissionMainDetails from './SubmissionMainDetails/SubmissionMainDetails';
import {
  CREATORS_SUBMISSION_ROW,
  PUBLISHERS_SUBMISSION_ROW,
  DERIVED_FROM_WORKS_SUBMISSION_ROW,
  PERFORMERS_SUBMISSION_ROW,
  SUBMISSION_MAIN_DETAILS_STEP,
  SUBMISSION_ADDITIONAL_DETAILS_STEP,
  SUBMISSION_SUCCESS_STEP,
  NEW_SUBMISSION,
  UPDATE_SUBMISSION,
  MAKE_ANOTHER_SUBMISSION,
  COPY_AS_NEW_SUBMISSION,
  GO_BACK,
  SUBMIT,
  NEXT,
  BLANK_OPTION,
  CANCEL,
} from '../../consts';
import SubmissionAdditionalDetails from './SubmissionAdditionalDetails/SubmissionAdditionalDetails';
import SubHeader from '../../components/SubHeader/SubHeader';
import ActionButton from '../../components/ActionButton/ActionButton';
import SubmissionSuccess from './SubmissionSuccess/SubmissionSuccess';
import Loader from '../../components/Loader/Loader';
import ErrorHandler from '../../components/AlertMessage/ErrorHandler/ErrorHandler';
import { getStrings } from '../../configuration/Localization';
import { getLoggedInAgencyId } from '../../shared/helperMethods';

export default class Submission extends React.Component<ISubmissionProps, ISubmissionState> {
  updateInstance: boolean | undefined;
  agencyId: string;

  constructor(props: ISubmissionProps) {
    super(props);
    this.state = {
      agencyWorkCode: props.agencyWorkCode || [
        { agencyWorkCode: '', agencyName: getLoggedInAgencyId() },
      ],
      titles: props.titles || [{ title: '', type: 'OT' }],
      creators: props.creators || [CREATORS_SUBMISSION_ROW],
      disambiguation: props.disambiguation || false,
      preferredIswc: props.preferredIswc || '',
      publishers: props.publishers || [PUBLISHERS_SUBMISSION_ROW],
      derivedWorkType: props.derivedWorkType || BLANK_OPTION.value,
      derivedFromWorks: props.derivedFromWorks || [DERIVED_FROM_WORKS_SUBMISSION_ROW],
      disambiguationIswcs: props.disambiguationIswcs || [],
      disambiguationReason: props.disambiguationReason || BLANK_OPTION.value,
      bvltr: props.bvltr || BLANK_OPTION.value,
      performers: props.performers || [PERFORMERS_SUBMISSION_ROW],
      standardInstrumentation: props.standardInstrumentation || BLANK_OPTION.value,
    };

    this.updateInstance = props.updateInstance;
    this.agencyId = getLoggedInAgencyId();
  }

  componentDidMount = () => {
    const strings = getStrings();
    document.title = this.updateInstance ? strings[UPDATE_SUBMISSION] : strings[NEW_SUBMISSION];

    const { clearSearch } = this.props;
    if (this.updateInstance) clearSearch();
  };

  componentDidUpdate = (prevProps: ISubmissionProps) => {
    const { verifiedSubmission, router } = this.props;
    if (verifiedSubmission && !prevProps.verifiedSubmission.iswc && verifiedSubmission.iswc) {
      this._updateSubmissionDataString(verifiedSubmission.iswc, 'preferredIswc');
    }
    if (
      prevProps.router.location &&
      router.location &&
      prevProps.router.location.pathname === router.location.pathname &&
      prevProps.router.location.key !== router.location.key
    ) {
      window.location.reload();
    }
  };

  componentWillUnmount = () => {
    const { clearSubmissionError } = this.props;
    clearSubmissionError();
    this._resetToFirstStep();
  };

  _resetToFirstStep = () => {
    const { setSubmissionStep } = this.props;

    this._clearStateSubmission();
    setSubmissionStep(SUBMISSION_MAIN_DETAILS_STEP);
    this.updateInstance = false;
  };

  _goBack = () => {
    const { setSubmissionStep } = this.props;

    this.setState({
      disambiguation: false,
      disambiguationIswcs: [],
      disambiguationReason: BLANK_OPTION.value,
      bvltr: BLANK_OPTION.value,
      standardInstrumentation: BLANK_OPTION.value,
    });
    setSubmissionStep(SUBMISSION_MAIN_DETAILS_STEP);
  };

  _copyAsNewSubmission = () => {
    const { setSubmissionStep } = this.props;

    this.setState({
      preferredIswc: '',
      agencyWorkCode: [{ agencyWorkCode: '', agencyName: this.agencyId }],
      disambiguation: false,
      derivedWorkType: BLANK_OPTION.value,
      derivedFromWorks: [DERIVED_FROM_WORKS_SUBMISSION_ROW],
      disambiguationIswcs: [],
      disambiguationReason: BLANK_OPTION.value,
      bvltr: BLANK_OPTION.value,
      standardInstrumentation: BLANK_OPTION.value,
    });
    setSubmissionStep(SUBMISSION_MAIN_DETAILS_STEP);
    this.updateInstance = false;
  };

  _clearStateSubmission = () => {
    this.setState({
      agencyWorkCode: [{ agencyWorkCode: '', agencyName: this.agencyId }],
      titles: [{ title: '', type: 'OT' }],
      creators: [CREATORS_SUBMISSION_ROW],
      disambiguation: false,
      preferredIswc: '',
      publishers: [PUBLISHERS_SUBMISSION_ROW],
      derivedWorkType: BLANK_OPTION.value,
      derivedFromWorks: [DERIVED_FROM_WORKS_SUBMISSION_ROW],
      disambiguationIswcs: [],
      disambiguationReason: BLANK_OPTION.value,
      bvltr: BLANK_OPTION.value,
      performers: [PERFORMERS_SUBMISSION_ROW],
      standardInstrumentation: BLANK_OPTION.value,
    });
  };

  _updateSubmissionDataArray = (
    value: string,
    rowId: number,
    key: ISubmissionStateKeys,
    objectKey: ISubmissionMainDetailsStateObjectKeys,
  ) => {
    const stateArrayCopy = JSON.parse(JSON.stringify(this.state[key]));
    stateArrayCopy[rowId][objectKey] = value;

    this.setState({
      [key]: stateArrayCopy,
    } as Pick<ISubmissionState, keyof ISubmissionState>);
  };

  _updateSubmissionDataString = (value: string, key: ISubmissionStateKeys) => {
    let arrayValue = key === 'disambiguationIswcs' ? value.split(';') : undefined;

    const stringCopy = arrayValue
      ? JSON.parse(JSON.stringify(arrayValue))
      : JSON.parse(JSON.stringify(value || ''));

    this.setState({
      [key]: stringCopy,
    } as Pick<ISubmissionState, keyof ISubmissionState>);
  };

  _addElementToSubmissionDataArray = (element: any, rowId: number, key: ISubmissionStateKeys) => {
    const stateArrayCopy = JSON.parse(JSON.stringify(this.state[key]));
    stateArrayCopy[rowId] = element;

    this.setState({
      [key]: stateArrayCopy,
    } as Pick<ISubmissionState, keyof ISubmissionState>);
  };

  _setDisambiguation = () => {
    const { disambiguation } = this.state;
    this.setState({
      disambiguation: !disambiguation,
    });
  };

  _addElementToArray = (key: ISubmissionStateKeys, row: any) => {
    const stateArrayCopy = JSON.parse(JSON.stringify(this.state[key]));
    stateArrayCopy.push(row);

    this.setState({
      [key]: stateArrayCopy,
    } as Pick<ISubmissionState, keyof ISubmissionState>);
  };

  _removeElementFromArray = (key: ISubmissionStateKeys, rowId: number) => {
    const stateArrayCopy = JSON.parse(JSON.stringify(this.state[key]));
    stateArrayCopy.splice(rowId, 1);

    this.setState({
      [key]: stateArrayCopy,
    } as Pick<ISubmissionState, keyof ISubmissionState>);
  };

  _getMainDetailsAction = () => {
    const {
      agencyWorkCode,
      titles,
      creators,
      publishers,
      derivedWorkType,
      derivedFromWorks,
      disambiguation,
      disambiguationIswcs,
      performers,
    } = this.state;
    const { updateSubmission, newSubmission } = this.props;
    const derivedWorkTypeParam =
      derivedWorkType !== BLANK_OPTION.value ? derivedWorkType : undefined;

    if (this.updateInstance) {
      return updateSubmission(
        true,
        agencyWorkCode,
        titles.filter((title) => title.title.length !== 0),
        creators.filter((creator) => creator.nameNumber?.length !== 0),
        disambiguation,
        disambiguationIswcs,
        undefined,
        publishers.filter((publisher) => publisher.nameNumber?.length !== 0),
        derivedWorkTypeParam,
        derivedFromWorks.filter(
          (derivedFromWork) =>
            JSON.stringify(derivedFromWork) !== JSON.stringify(DERIVED_FROM_WORKS_SUBMISSION_ROW),
        ),
        performers.filter(
          (performer) => JSON.stringify(performer) !== JSON.stringify(PERFORMERS_SUBMISSION_ROW),
        ),
      );
    } else {
      return newSubmission(
        true,
        agencyWorkCode,
        titles.filter((title) => title.title.length !== 0),
        creators.filter((creator) => creator.nameNumber?.length !== 0),
        disambiguation,
        undefined,
        publishers.filter((publisher) => publisher.nameNumber?.length !== 0),
        derivedWorkTypeParam,
        derivedFromWorks.filter(
          (derivedFromWork) =>
            JSON.stringify(derivedFromWork) !== JSON.stringify(DERIVED_FROM_WORKS_SUBMISSION_ROW),
        ),
        undefined,
        undefined,
        undefined,
        performers.filter(
          (performer) => JSON.stringify(performer) !== JSON.stringify(PERFORMERS_SUBMISSION_ROW),
        ),
      );
    }
  };

  _getAdditionalDetailsAction = () => {
    const {
      agencyWorkCode,
      titles,
      creators,
      publishers,
      derivedWorkType,
      derivedFromWorks,
      disambiguation,
      disambiguationIswcs,
      disambiguationReason,
      bvltr,
      performers,
      standardInstrumentation,
      preferredIswc,
    } = this.state;
    const { newSubmission, potentialMatches, updateSubmission } = this.props;
    const derivedWorkTypeParam =
      derivedWorkType !== BLANK_OPTION.value ? derivedWorkType : undefined;
    const bvltrParam = bvltr !== BLANK_OPTION.value ? bvltr : undefined;
    const standardInstrumentationParam =
      standardInstrumentation !== BLANK_OPTION.value ? standardInstrumentation : undefined;
    const disambiguationReasonParam =
      disambiguationReason !== BLANK_OPTION.value ? disambiguationReason : undefined;

    const potentialMatchesIswcs: string[] = [];

    potentialMatches.forEach((x) => potentialMatchesIswcs.push(x.iswc));

    if (this.updateInstance) {
      return updateSubmission(
        false,
        agencyWorkCode,
        titles.filter((title) => title.title.length !== 0),
        creators.filter((creator) => creator.nameNumber?.length !== 0),
        disambiguation,
        disambiguationIswcs,
        undefined,
        publishers.filter((publisher) => publisher.nameNumber?.length !== 0),
        derivedWorkTypeParam,
        derivedFromWorks.filter(
          (derivedFromWork) =>
            JSON.stringify(derivedFromWork) !== JSON.stringify(DERIVED_FROM_WORKS_SUBMISSION_ROW),
        ),
        performers.filter(
          (performer) => JSON.stringify(performer) !== JSON.stringify(PERFORMERS_SUBMISSION_ROW),
        ),
      );
    } else {
      if (disambiguation) {
        return newSubmission(
          false,
          agencyWorkCode,
          titles.filter((title) => title.title.length !== 0),
          creators.filter((creator) => creator.nameNumber?.length !== 0),
          disambiguation,
          undefined,
          publishers.filter((publisher) => publisher.nameNumber?.length !== 0),
          derivedWorkTypeParam,
          derivedFromWorks.filter(
            (derivedFromWork) =>
              JSON.stringify(derivedFromWork) !== JSON.stringify(DERIVED_FROM_WORKS_SUBMISSION_ROW),
          ),
          potentialMatchesIswcs.concat(disambiguationIswcs.filter((iswc) => iswc !== '')),
          disambiguationReasonParam,
          bvltrParam,
          performers.filter(
            (performer) => JSON.stringify(performer) !== JSON.stringify(PERFORMERS_SUBMISSION_ROW),
          ),
          standardInstrumentationParam,
        );
      } else {
        return newSubmission(
          false,
          agencyWorkCode,
          titles.filter((title) => title.title.length !== 0),
          creators.filter((creator) => creator.nameNumber?.length !== 0),
          disambiguation,
          preferredIswc,
          publishers.filter((publisher) => publisher.nameNumber?.length !== 0),
          derivedWorkTypeParam,
          derivedFromWorks.filter(
            (derivedFromWork) =>
              JSON.stringify(derivedFromWork) !== JSON.stringify(DERIVED_FROM_WORKS_SUBMISSION_ROW),
          ),
          undefined,
          undefined,
          undefined,
          performers.filter(
            (performer) => JSON.stringify(performer) !== JSON.stringify(PERFORMERS_SUBMISSION_ROW),
          ),
        );
      }
    }
  };

  renderStep = () => {
    const {
      preferredIswc,
      agencyWorkCode,
      titles,
      creators,
      publishers,
      derivedWorkType,
      derivedFromWorks,
      performers,
      disambiguationReason,
      disambiguationIswcs,
      bvltr,
      standardInstrumentation,
    } = this.state;
    const { step, verifiedSubmission, potentialMatches, searchByIswc } = this.props;

    switch (step) {
      case SUBMISSION_MAIN_DETAILS_STEP:
        return (
          <div className={styles.subDiv}>
            <SubmissionMainDetails
              agencyWorkCodes={agencyWorkCode}
              titles={titles}
              creators={creators}
              publishers={publishers}
              performers={performers}
              derivedWorkType={derivedWorkType}
              derivedFromWorks={derivedFromWorks}
              updateSubmissionDataArray={this._updateSubmissionDataArray}
              updateSubmissionDataString={this._updateSubmissionDataString}
              addElementToSubmissionDataArray={this._addElementToSubmissionDataArray}
              addElementToArray={this._addElementToArray}
              removeElementFromArray={this._removeElementFromArray}
              updateInstance={this.updateInstance}
            />
          </div>
        );
      case SUBMISSION_ADDITIONAL_DETAILS_STEP:
        return (
          <SubmissionAdditionalDetails
            preferredIswc={preferredIswc}
            updateSubmissionDataArray={this._updateSubmissionDataArray}
            updateSubmissionDataString={this._updateSubmissionDataString}
            addElementToArray={this._addElementToArray}
            removeElementFromArray={this._removeElementFromArray}
            performers={performers}
            disambiguationReason={disambiguationReason}
            disambiguationIswcs={disambiguationIswcs}
            bvltr={bvltr}
            standardInstrumentation={standardInstrumentation}
            potentialMatches={potentialMatches}
            searchByIswc={searchByIswc}
            verifiedSubmission={verifiedSubmission}
            setDisambiguation={this._setDisambiguation}
            updateInstance={this.updateInstance}
          />
        );
      case SUBMISSION_SUCCESS_STEP: {
        return (
          <SubmissionSuccess
            newIswc={potentialMatches.length === 0 || verifiedSubmission.disambiguation}
            verifiedSubmission={verifiedSubmission}
            updateInstance={this.updateInstance}
          />
        );
      }
      default:
        return;
    }
  };

  renderButtons = () => {
    const { step, loading, router } = this.props;
    const strings = getStrings();

    switch (step) {
      case SUBMISSION_MAIN_DETAILS_STEP:
        return (
          <div>
            <div className={styles.actionButton} id={'nextDiv'}>
              <ActionButton
                buttonText={strings[NEXT]}
                buttonAction={() => this._getMainDetailsAction()}
                isDisabled={loading}
              />
            </div>
            {this.updateInstance && (
              <div className={styles.actionButton}>
                <ActionButton
                  buttonText={strings[CANCEL]}
                  buttonAction={() => router.history.goBack()}
                  isDisabled={loading}
                  theme={'secondary'}
                />
              </div>
            )}
          </div>
        );
      case SUBMISSION_ADDITIONAL_DETAILS_STEP:
        return (
          <div id={'submitGoDiv'}>
            <div className={styles.actionButton}>
              <ActionButton
                buttonText={strings[SUBMIT]}
                buttonAction={() => this._getAdditionalDetailsAction()}
                isDisabled={loading}
              />
            </div>
            {this.updateInstance && (
              <div className={styles.actionButton}>
                <ActionButton
                  buttonText={strings[CANCEL]}
                  buttonAction={this._resetToFirstStep}
                  isDisabled={loading}
                  theme={'secondary'}
                />
              </div>
            )}
            <div className={this.updateInstance ? styles.actionButtonLeft : styles.actionButton}>
              <ActionButton
                buttonText={strings[GO_BACK]}
                buttonAction={this._goBack}
                isDisabled={loading}
                theme={'secondary'}
              />
            </div>
          </div>
        );
      case SUBMISSION_SUCCESS_STEP:
        return (
          <div>
            <div className={`${styles.actionButton} ${styles.actionButtonBig}`}>
              <ActionButton
                buttonText={strings[COPY_AS_NEW_SUBMISSION]}
                buttonAction={this._copyAsNewSubmission}
              />
            </div>
            <div className={`${styles.actionButton} ${styles.actionButtonBig}`}>
              <ActionButton
                buttonText={strings[MAKE_ANOTHER_SUBMISSION]}
                buttonAction={this._resetToFirstStep}
              />
            </div>
          </div>
        );
      default:
        return;
    }
  };

  render() {
    const { error, step, loading } = this.props;
    const strings = getStrings();

    return (
      <div className={styles.container}>
        <SubHeader
          title={this.updateInstance ? strings[UPDATE_SUBMISSION] : strings[NEW_SUBMISSION]}
        />
        <div className={styles.mainDiv}>{this.renderStep()}</div>
        <div className={styles.bottomDiv}>
          {error !== undefined && step !== SUBMISSION_SUCCESS_STEP && (
            <div className={styles.errorDiv}>
              <ErrorHandler error={error} />
            </div>
          )}
          {this.renderButtons()}
        </div>
        <div className={styles.loaderDiv}>{loading && <Loader />}</div>
      </div>
    );
  }
}
