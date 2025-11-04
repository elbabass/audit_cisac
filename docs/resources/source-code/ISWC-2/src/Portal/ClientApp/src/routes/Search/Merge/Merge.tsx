import React, { Component } from 'react';
import { IMergeProps, IMergeState } from './MergeTypes';
import MergeGrid from './MergeGrid';
import styles from './Merge.module.scss';
import { getStrings } from '../../../configuration/Localization';
import ActionButton from '../../../components/ActionButton/ActionButton';
import { IMergeBody } from '../../../redux/types/IswcTypes';
import Loader from '../../../components/Loader/Loader';
import ErrorHandler from '../../../components/AlertMessage/ErrorHandler/ErrorHandler';
import { getLoggedInAgencyId, createMergeDemergeMessage } from '../../../shared/helperMethods';
import SubHeader from '../../../components/SubHeader/SubHeader';
import { MERGE_SUCCESSFUL } from '../../../consts';
import AlertMessage from '../../../components/AlertMessage/AlertMessage';

export default class Merge extends Component<IMergeProps, IMergeState> {
  agencyId: string;
  preferredIswc: string;
  childIswcs: string[];
  constructor(props: IMergeProps) {
    super(props);

    this.state = {};

    this.agencyId = getLoggedInAgencyId();
    this.preferredIswc = '';
    this.childIswcs = [];
  }

  componentDidMount() {
    const { clearMergeError } = this.props;
    clearMergeError();
  }

  componentDidUpdate(prevProps: IMergeProps) {
    const { preferredIswc, mergeBody } = this.state;
    const { merging, mergedSuccessfully, mergeList } = this.props;

    if (prevProps.merging && !merging) {
      this.setState({ mergeSuccessful: mergedSuccessfully });
    }
    if (prevProps.mergeList !== mergeList) {
      this._updateMergeRequestData(preferredIswc);
    }
    if (!prevProps.mergedSuccessfully && mergedSuccessfully) {
      this.preferredIswc = preferredIswc ? preferredIswc : '';
      this.childIswcs = mergeBody?.iswcs ? mergeBody.iswcs : [];
      this.setState({ mergeBody: {}, preferredIswc: undefined });
    }
  }

  _updateMergeRequestData = (preferredIswc: string | undefined) => {
    const { mergeList } = this.props;
    let mergeBody: IMergeBody = {};

    if (preferredIswc === undefined) {
      return this.setState({ preferredIswc: undefined, mergeBody: undefined });
    }

    mergeList.forEach((x) => {
      if (x.iswc !== preferredIswc) {
        mergeBody.iswcs ? mergeBody.iswcs.push(x.iswc) : (mergeBody.iswcs = [x.iswc]);
      }
    });

    this.setState({ preferredIswc, mergeBody });
  };

  _mergeIswcs = () => {
    const { mergeIswcs } = this.props;
    const { preferredIswc, mergeBody } = this.state;

    if (preferredIswc !== undefined && mergeBody?.iswcs)
      mergeIswcs(preferredIswc, this.agencyId, mergeBody);
  };

  _renderMergeStatus = () => {
    const { error } = this.props;
    const { mergeSuccessful } = this.state;
    const strings = getStrings();

    if (!mergeSuccessful && error !== undefined)
      return (
        <div className={styles.unsuccessfulStatus}>
          <ErrorHandler error={error} />
        </div>
      );
    else if (mergeSuccessful) {
      let message = createMergeDemergeMessage(this.childIswcs);
      message += ` ${strings[MERGE_SUCCESSFUL]} ${this.preferredIswc}`;
      return <AlertMessage type={'success'} message={message} />;
    } else return null;
  };

  render() {
    const { mergeList, removeFromMergeList, merging } = this.props;
    const { SELECT_ISWC_TO_MERGE_INTO, MERGE_LIST, SUBMIT_MERGE } = getStrings();
    return (
      <div className={styles.container}>
        <SubHeader title={MERGE_LIST} />
        <div className={styles.textDiv}>{SELECT_ISWC_TO_MERGE_INTO}</div>
        <MergeGrid
          mergeList={mergeList}
          removeFromMergeList={removeFromMergeList}
          updateMergeRequestData={this._updateMergeRequestData}
        />
        <div className={styles.statusAndButtonDiv}>
          <div className={styles.statusContainer}>{this._renderMergeStatus()}</div>
          <div className={styles.actionButtonDiv}>
            <ActionButton buttonText={SUBMIT_MERGE} buttonAction={this._mergeIswcs} />
          </div>
        </div>
        {merging && <Loader />}
      </div>
    );
  }
}
