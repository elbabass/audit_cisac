import React, { Component } from 'react';
import { getStrings } from '../../../configuration/Localization';
import styles from './Demerge.module.scss';
import ActionButton from '../../../components/ActionButton/ActionButton';
import LinkedToGrid from './LinkedToGrid';
import { IGridHeaderCell, IGridRow } from '../../../components/GridComponents/Grid/GridTypes';
import {
  PREFERRED_ISWC_FIELD,
  ORIGINAL_TITLE_FIELD,
  CREATOR_NAMES_FIELD,
  CREATION_DATE_FIELD,
  VIEW_MORE_FIELD,
  VIEW_MORE_ICON,
  VIEW_MORE_ACTION,
  DEMERGE_SUCCESSFUL,
  DATE_TYPE,
  DEMERGE_PATH,
} from '../../../consts';
import GridTextCell from '../../../components/GridComponents/GridTextCell/GridTextCell';
import GridIconCell from '../../../components/GridComponents/GridIconCell/GridIconCell';
import Grid from '../../../components/GridComponents/Grid/Grid';
import { IDemergeProps, IDemergeState, IDemergeIswc } from './DemergeTypes';
import {
  _getContributorNames,
  formatDateString,
  getLoggedInAgencyId,
  validateIswcAndFormat,
  createMergeDemergeMessage,
} from '../../../shared/helperMethods';
import ViewMore from '../ViewMore/ViewMore';
import Loader from '../../../components/Loader/Loader';
import ErrorHandler from '../../../components/AlertMessage/ErrorHandler/ErrorHandler';
import AlertMessage from '../../../components/AlertMessage/AlertMessage';

export default class Demerge extends Component<IDemergeProps, IDemergeState> {
  agencyId: string;
  preferredIswc: string;
  childIswcs: string[];

  constructor(props: IDemergeProps) {
    super(props);
    this.state = {
      demergeSuccessful: false,
      iswcsToDemerge: [],
    };

    this.agencyId = getLoggedInAgencyId();
    this.preferredIswc = '';
    this.childIswcs = [];
  }

  componentDidMount() {
    const { linkedIswcs, searchByIswcBatch, clearMergeError } = this.props;
    clearMergeError();
    searchByIswcBatch(linkedIswcs);
  }

  componentDidUpdate(prevProps: IDemergeProps) {
    const { demerging, demergedSuccessfully, clearDemergeData, router } = this.props;
    if (prevProps.demerging && !demerging && demergedSuccessfully) {
      router.history.replace({
        pathname: DEMERGE_PATH,
        state: { linkedIswcs: [], preferredIswc: router.location.state.preferredIswc },
      });
      clearDemergeData();
      this.setState({
        demergeSuccessful: demergedSuccessfully,
        iswcsToDemerge: [],
      });
    }
  }

  _addToIswcsToDemerge = (rowId: number) => {
    const { iswcsToDemerge } = this.state;
    var rowExistsInArray = iswcsToDemerge.some(function (row) {
      return row === rowId;
    });

    if (rowExistsInArray) {
      this.setState({
        iswcsToDemerge: iswcsToDemerge.filter((iswc) => {
          return iswc !== rowId;
        }),
      });
    } else {
      this.setState({
        iswcsToDemerge: iswcsToDemerge.concat(rowId),
      });
    }
  };

  _demergeIswcs = () => {
    const { linkedIswcData, demergeIswc, preferredIswc } = this.props;
      const { iswcsToDemerge } = this.state;
      let demergeIswcs: IDemergeIswc[] = [];

    if (linkedIswcData !== undefined) {
      this.preferredIswc = preferredIswc.iswc;
      linkedIswcData.forEach((linkedIswc, index) => {
        let filteredWorks = linkedIswc.works?.filter((x) => x.agency === this.agencyId);
        let parentWorks = preferredIswc.works?.filter((x) => x.agency === this.agencyId);
        if (filteredWorks && filteredWorks.length === 0 && parentWorks && parentWorks.length > 0)
          filteredWorks = linkedIswc.works;
          
        if (iswcsToDemerge.includes(index)) {
          filteredWorks && this.childIswcs.push(filteredWorks[0]?.iswc);
          demergeIswcs.push({
            agency: filteredWorks && filteredWorks[0]?.agency,
            workCode: filteredWorks && filteredWorks[0]?.workcode,
          });
        }
      });

      if (demergeIswcs.length > 0) {
        demergeIswc(preferredIswc.iswc, demergeIswcs);
      }
    }
  };

  // Preferred ISWC Grid Data
  preferredIswcsHeaderCells = (): IGridHeaderCell[] => {
    const strings = getStrings();
    return [
      { text: strings[PREFERRED_ISWC_FIELD], field: PREFERRED_ISWC_FIELD, sortable: true },
      { text: strings[ORIGINAL_TITLE_FIELD], field: ORIGINAL_TITLE_FIELD, sortable: true },
      { text: strings[CREATOR_NAMES_FIELD], field: CREATOR_NAMES_FIELD },
      {
        text: strings[CREATION_DATE_FIELD],
        field: CREATION_DATE_FIELD,
        sortable: true,
        type: DATE_TYPE,
      },
      { field: VIEW_MORE_FIELD },
    ];
  };

  preferredIswcsGridRows = (): IGridRow[] => {
    const { preferredIswc } = this.props;
    const strings = getStrings();
    return [
      {
        rowId: 0,
        cells: [
          {
            element: <GridTextCell text={validateIswcAndFormat(preferredIswc.iswc)} />,
            field: PREFERRED_ISWC_FIELD,
          },
          {
            element: <GridTextCell text={preferredIswc.originalTitle} />,
            field: ORIGINAL_TITLE_FIELD,
          },
          {
            element: <GridTextCell text={_getContributorNames(preferredIswc.interestedParties)} />,
            field: CREATOR_NAMES_FIELD,
          },
          {
            element: <GridTextCell text={formatDateString(preferredIswc.createdDate)} />,
            field: CREATION_DATE_FIELD,
          },
          {
            element: (
              <GridIconCell
                text={strings[VIEW_MORE_FIELD]}
                icon={VIEW_MORE_ICON}
                alt={'View More Icon'}
                clickable
                id={strings[VIEW_MORE_FIELD]}
              />
            ),
            field: VIEW_MORE_FIELD,
            // Handled in GridRow.tsx*
            action: VIEW_MORE_ACTION,
          },
        ],
        viewMore: [<ViewMore iswcModel={preferredIswc} isSubmissionGrid />],
      },
    ];
  };

  _renderDemergeResponseStatus = () => {
    const { error } = this.props;
    const { demergeSuccessful } = this.state;
    const strings = getStrings();

    if (!demergeSuccessful && error !== undefined)
      return (
        <div className={styles.unsuccessfulStatus}>
          <ErrorHandler error={error} />
        </div>
      );
    else if (demergeSuccessful) {
      let message = createMergeDemergeMessage(this.childIswcs);
      message += ` ${strings[DEMERGE_SUCCESSFUL]} ${this.preferredIswc}`;
      return <AlertMessage type={'success'} message={message} />;
    }
  };

  render() {
    const { DEMERGE, SUBMIT_DEMERGE, PREFERRED_ISWC_FIELD, FOLLOWING_ISWCS_MERGED } = getStrings();
    const { preferredIswc, linkedIswcData, loading, demerging } = this.props;
    const { iswcsToDemerge } = this.state;

    return (
      <div className={styles.container}>
        <div className={styles.headerDiv}>
          <div className={styles.titleDiv}>{DEMERGE}</div>
        </div>
        <div className={styles.gridContainer}>
          <div className={styles.gridTitle}>{`${PREFERRED_ISWC_FIELD}:`}</div>
          {preferredIswc && (
            <div className={styles.grid}>
              <Grid
                headerCells={this.preferredIswcsHeaderCells()}
                gridRows={this.preferredIswcsGridRows()}
              />
            </div>
          )}
        </div>
        {loading && <Loader />}
        {!loading && linkedIswcData && (
          <div className={styles.gridContainer}>
            <div className={styles.gridText}>
              {FOLLOWING_ISWCS_MERGED &&
                FOLLOWING_ISWCS_MERGED.format(validateIswcAndFormat(preferredIswc?.iswc))}
            </div>
            <div className={styles.grid}>
              <LinkedToGrid
                linkedIswcs={linkedIswcData}
                iswcsToDemerge={iswcsToDemerge}
                addToIswcsToDemerge={this._addToIswcsToDemerge}
              />
            </div>
          </div>
        )}
        <div className={styles.statusAndButtonDiv}>
          <div className={styles.statusContainer}>{this._renderDemergeResponseStatus()}</div>
          <div className={styles.actionButtonDiv}>
            <ActionButton
              buttonText={SUBMIT_DEMERGE}
              buttonAction={this._demergeIswcs}
              isDisabled={iswcsToDemerge.length === 0}
            />
          </div>
        </div>
        {demerging && <Loader />}
      </div>
    );
  }
}
