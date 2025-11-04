import React, { PureComponent } from 'react';
import styles from './ViewMore.module.scss';
import ViewMoreHeader from './ViewMoreHeader';
import {
  titleHeaderCells,
  creatorHeaderCells,
  societyWorkCodesHeaderCells,
  derivedIswcsHeaderCells,
  getTitleGridRows,
  getCreatorGridRows,
  getSocietyWorkCodesGridRows,
  getDerivedIswcsGridRows,
  getDisambiguationGridRows,
  disambiguateFromHeaderCells,
  performerHeaderCells,
  getPerformerGridRows,
  getRecordingGridRows,
  recordingHeaderCells
} from './ViewMoreGrids';
import Grid from '../../../components/GridComponents/Grid/Grid';
import { IViewMoreProps, IViewMoreState, IDeletionData } from './ViewMoreTypes';
import { IGridHeaderCell, IGridRow } from '../../../components/GridComponents/Grid/GridTypes';
import GridIconCell from '../../../components/GridComponents/GridIconCell/GridIconCell';
import {
  EXTERNAL_LINK_ICON,
  VIEW_MORE_ICON,
  DEMERGE_PATH,
  PUBLIC_MODE,
  CANCEL,
  SUB_WITH_AGENCY_WORKCODE,
  SUCCESSFULLY_DELETED,
  DELETE_SUBMISSION,
  DELETE_CONFIRM,
  CONFIRM_DELETION,
  MERGED_ICON,
  MERGE_INFO,
  ISWC_HAS_BEEN_MERGED,
  SEARCH_FOR_ISWC,
  NEW_SUBMISSION,
} from '../../../consts';
import { SUBMISSION_PATH } from '../../../consts';
import { getStrings } from '../../../configuration/Localization';
import { Link } from 'react-router-dom';
import {
  _filterCreators,
  validateIswcAndFormatArray,
  _getPerformers,
  _getRecordings,
  validateIswcAndFormat,
} from '../../../shared/helperMethods';
import Modal from '../../../components/Modal/Modal';
import AlertMessage from '../../../components/AlertMessage/AlertMessage';
import { getLoggedInAgencyId } from '../../../shared/helperMethods';
import { mapSubmissionToSubmissionState } from '../../../shared/MappingObjects';

export default class ViewMore extends PureComponent<IViewMoreProps, IViewMoreState> {
  deletionData: IDeletionData;
  agencyId: string;
  constructor(props: IViewMoreProps) {
    super(props);

    this.state = {
      rows: {
        titles: [],
        creators: [],
        works: [],
        derivedWorks: [],
        disambiguation: [],
      },
      archivedIswcs: [],
      isModalOpen: false,
      isResubmit: false,
      mostRecentSubmissionFromAgency: undefined,
      deletedWorkCode: undefined,
    };

    this.agencyId = getLoggedInAgencyId();

    this.deletionData = {
      preferredIswc: '',
      agency: '',
      workcode: '',
      reasonCode: '',
      rowId: 0,
    };
  }

  componentDidMount() {
    this._setMostRecentSubmissionFromAgency();
    const {
      iswcModel: { originalTitle, otherTitles, interestedParties, works },
      isSubmissionGrid,
      assignedRoles,
    } = this.props;

    const creators = _filterCreators(interestedParties);
    const performers = works && _getPerformers(works);
    const recordings = works && _getRecordings(works);

    const titleRows = getTitleGridRows(originalTitle, otherTitles);
    const creatorRows = creators && getCreatorGridRows(creators);
    const workCodeRows =
      works &&
      getSocietyWorkCodesGridRows(
        works,
        isSubmissionGrid || !assignedRoles?.update,
        this._toggleDeleteModal,
        this._setDeletionData,
      );
    const derivedWorkRows = works && getDerivedIswcsGridRows(works);
    const disambiguationRows = works && getDisambiguationGridRows(works);
    const performerRows = performers && getPerformerGridRows(performers);
    const recordingsRows = recordings && getRecordingGridRows(recordings);
    const archivedIswcs: string[] = [];
    works?.forEach((x) => {
      if (x.archivedIswc && !archivedIswcs.includes(x.archivedIswc)) {
        archivedIswcs.push(x.archivedIswc);
      }
    });

    this.setState({
      rows: {
        titles: titleRows,
        creators: creatorRows,
        works: workCodeRows,
        derivedWorks: derivedWorkRows,
        disambiguation: disambiguationRows,
        performers: performerRows,
        recordings: recordingsRows,
      },
      archivedIswcs,
    });
  }

  _setMostRecentSubmissionFromAgency = () => {
    const { iswcModel } = this.props;
    const works = iswcModel.works?.filter((work) => work.agency === this.agencyId);

    if (works && works.length > 0) {
      this.setState({
        mostRecentSubmissionFromAgency: mapSubmissionToSubmissionState(works[0]),
      });
    }
  };

  componentDidUpdate(prevProps: IViewMoreProps) {
    const { deletionLoading, deletionError } = this.props;
    const { isModalOpen } = this.state;

    if (
      prevProps.deletionLoading === true &&
      deletionLoading === false &&
      !deletionError &&
      isModalOpen
    ) {
      this._setDeletionMessageAndRemoveWorkCode();
      this._toggleDeleteModal();
    }
  }

  _setDeletionMessageAndRemoveWorkCode = () => {
    const { works, creators, titles, disambiguation, derivedWorks } = this.state.rows;
    let worksCopy = works ? [...works] : [];
    let index = undefined;
    const strings = getStrings();

    for (let x = 0; x < worksCopy.length; x++) {
      if (worksCopy[x].rowId === this.deletionData.rowId) {
        index = x;
        break;
      }
    }
    if (index !== undefined) worksCopy?.splice(index, 1);

    this.setState({
      deletionMessage: `${strings[SUB_WITH_AGENCY_WORKCODE]} ${this.deletionData.workcode} ${strings[SUCCESSFULLY_DELETED]}`,
      rows: {
        works: worksCopy,
        creators,
        titles,
        derivedWorks,
        disambiguation,
      },
    });
  };

  _toggleDeleteModal = () => {
    const { clearSubmissionError } = this.props;
    const { isModalOpen } = this.state;

    if (isModalOpen && clearSubmissionError) clearSubmissionError();
    this.setState({
      isModalOpen: !isModalOpen,
    });
  };

  _setDeletionData = (
    preferredIswc: string,
    agency: string,
    workcode: string,
    reasonCode: string,
    rowId: number,
    isResubmit?: boolean,
  ) => {
    this.deletionData = { preferredIswc, agency, workcode, reasonCode, rowId };
    this.setState({
      deletedWorkCode: {
        agencyName: agency,
        agencyWorkCode: workcode,
      },
      isResubmit: isResubmit,
    });
  };

  _deleteSubmission = () => {
    const { deleteSubmission } = this.props;
    const { isResubmit } = this.state;

    if (deleteSubmission) {
      deleteSubmission(
        this.deletionData.preferredIswc,
        this.deletionData.agency,
        this.deletionData.workcode,
        this.deletionData.reasonCode,
      );
    }

    if (isResubmit) this._redirectToSubmission();
  };

  _redirectToSubmission = () => {
    const { mostRecentSubmissionFromAgency, deletedWorkCode } = this.state;
    const { router } = this.props;

    if (mostRecentSubmissionFromAgency && deletedWorkCode) {
      mostRecentSubmissionFromAgency.agencyWorkCode = [];
      mostRecentSubmissionFromAgency.agencyWorkCode.push(deletedWorkCode);
    }

    if (router != null)
      router.history.replace({
        pathname: SUBMISSION_PATH,
        state: {
          ...mostRecentSubmissionFromAgency,
          updateInstance: false,
        },
      });
  };

  _updateMergeList = () => {
    const { mergeList, addToMergeList, removeFromMergeList, iswcModel } = this.props;
    if (mergeList && mergeList.find((x) => x.iswc === iswcModel.iswc) === undefined)
      addToMergeList && addToMergeList(iswcModel);
    else removeFromMergeList && removeFromMergeList(iswcModel.iswc);
  };

  _renderViewMoreGrid(
    title: string,
    headerCells: IGridHeaderCell[],
    rows: IGridRow[],
    message?: string,
  ) {
    return (
      <div className={styles.gridContainer}>
        <div className={styles.gridTitle}>{`${title}:`}</div>
        {message && (
          <div className={styles.alertDiv}>
            <AlertMessage message={message} type={'success'} />
          </div>
        )}
        <div className={styles.grid}>
          <Grid headerCells={headerCells} gridRows={rows} />
        </div>
      </div>
    );
  }

  _checkLinkedIswcs = () => {
    const { iswcModel } = this.props;
    if (Array.isArray(iswcModel.linkedISWC) && iswcModel.linkedISWC.length > 0) return true;
  };

  renderMergeMessageDiv = () => {
    const { iswcModel } = this.props;
    const strings = getStrings();
    return (
      <div className={styles.gridContainer}>
        <div className={styles.mergeMessageDiv}>
          <div className={styles.mergeMessageDivHeader}>
            <img src={MERGED_ICON} alt={'Merged'} className={styles.mergeMessageIcon} />
            <div className={styles.mergeMessageHeader}>{strings[MERGE_INFO]}</div>
          </div>
          <div className={styles.mergeMessageTextTop}>
            {strings[ISWC_HAS_BEEN_MERGED]} {<b>{validateIswcAndFormat(iswcModel.parentISWC)}</b>}
          </div>
          <div className={styles.mergeMessageTextTop}>{strings[SEARCH_FOR_ISWC]}</div>
        </div>
      </div>
    );
  };

  renderViewMoreDetails() {
    const {
      rows: { titles, creators, works, derivedWorks, disambiguation, performers, recordings },
      archivedIswcs,
      deletionMessage,
    } = this.state;
    const { iswcModel, close, isSubmissionGrid, assignedRoles } = this.props;
    const {
      TITLES_FIELD,
      CREATORS_FIELD,
      ARCHIVED_ISWCS,
      LISTED_ISWCS_MERGED,
      DEMERGE_HERE,
      AGENCY_WORK_CODES,
      DISAMBIGUATION,
      DERIVED_WORKS,
      VIEW_LESS_FIELD,
      PERFORMERS,
      RECORDINGS
    } = getStrings();

    return (
      <div>
        {iswcModel.parentISWC && this.renderMergeMessageDiv()}
        {titles &&
          titles.length > 0 &&
          this._renderViewMoreGrid(TITLES_FIELD, titleHeaderCells(), titles)}
        {creators &&
          creators.length > 0 &&
          this._renderViewMoreGrid(CREATORS_FIELD, creatorHeaderCells(), creators)}
        {(this._checkLinkedIswcs() || archivedIswcs.length > 0) && (
          <div className={styles.gridContainer}>
            <div className={styles.gridTitle}>{ARCHIVED_ISWCS}</div>
            <div>
              {archivedIswcs.length > 0 && validateIswcAndFormatArray(archivedIswcs).join(', ')}
              {this._checkLinkedIswcs() && archivedIswcs.length > 0 && ', '}
              {Array.isArray(iswcModel.linkedISWC) &&
                iswcModel.linkedISWC.length > 0 &&
                validateIswcAndFormatArray(iswcModel.linkedISWC).join(', ')}
              {this._checkLinkedIswcs() && assignedRoles?.update && (
                <div className={styles.textWithLink}>
                  {LISTED_ISWCS_MERGED}
                  {!isSubmissionGrid && process.env.REACT_APP_MODE !== PUBLIC_MODE && (
                    <div className={`${styles.actionText} ${styles.textPadding}`}>
                      <Link
                        to={{
                          pathname: DEMERGE_PATH,
                          state: {
                            linkedIswcs: iswcModel.linkedISWC,
                            preferredIswc: iswcModel,
                          },
                        }}
                      >
                        <GridIconCell
                          icon={EXTERNAL_LINK_ICON}
                          text={DEMERGE_HERE}
                          alt={'external link icon'}
                          id={DEMERGE_HERE}
                        />
                      </Link>
                    </div>
                  )}
                </div>
              )}
            </div>
          </div>
        )}
        {performers &&
          performers.length > 0 &&
          this._renderViewMoreGrid(PERFORMERS, performerHeaderCells(), performers)}
        {works &&
          works.length > 0 &&
          this._renderViewMoreGrid(
            AGENCY_WORK_CODES,
            societyWorkCodesHeaderCells(iswcModel.works, isSubmissionGrid),
            works,
            deletionMessage,
          )}
        {recordings &&
          recordings.length > 0 &&
          this._renderViewMoreGrid(RECORDINGS, recordingHeaderCells(), recordings)}
        {works?.length === 0 && deletionMessage && (
          <div className={styles.gridContainer}>
            <div className={styles.alertDiv}>
              <AlertMessage message={deletionMessage} type={'success'} />
            </div>
          </div>
        )}
        {disambiguation &&
          disambiguation.length > 0 &&
          this._renderViewMoreGrid(DISAMBIGUATION, disambiguateFromHeaderCells(), disambiguation)}
        {derivedWorks &&
          derivedWorks.length > 0 &&
          this._renderViewMoreGrid(DERIVED_WORKS, derivedIswcsHeaderCells(), derivedWorks)}
        <div onClick={() => close && close()} className={styles.viewLess}>
          <GridIconCell
            text={VIEW_LESS_FIELD}
            icon={VIEW_MORE_ICON}
            alt={'view less icon'}
            clickable
            id={VIEW_LESS_FIELD}
          />
        </div>
      </div>
    );
  }

  renderDeleteModal = () => {
    const { isModalOpen } = this.state;
    const { deletionLoading, deletionError } = this.props;
    const strings = getStrings();

    return (
      <Modal
        headerText={strings[DELETE_SUBMISSION]}
        bodyText={strings[DELETE_CONFIRM]}
        isModalOpen={isModalOpen}
        toggleModal={this._toggleDeleteModal}
        loading={deletionLoading}
        error={deletionError}
        leftButtonText={strings[CANCEL]}
        leftButtonAction={this._toggleDeleteModal}
        rightButtonText={strings[CONFIRM_DELETION]}
        rightButtonAction={this._deleteSubmission}
        type={'confirm'}
      />
    );
  };

  render() {
    const { iswcModel, mergeList, isSubmissionGrid, assignedRoles } = this.props;
    return (
      <div className={styles.viewMoreContainer}>
        {process.env.REACT_APP_MODE !== PUBLIC_MODE && this.renderDeleteModal()}
        {process.env.REACT_APP_MODE !== PUBLIC_MODE && (
          <ViewMoreHeader
            updateMergeList={this._updateMergeList}
            iswc={iswcModel}
            mergeList={mergeList}
            isSubmissionGrid={isSubmissionGrid}
            assignedRoles={assignedRoles}
          />
        )}
        <div>{this.renderViewMoreDetails()}</div>
      </div>
    );
  }
}
