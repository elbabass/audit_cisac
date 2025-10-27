import React, { Component } from 'react';
import Media from 'react-media';
import TabView from '../../components/TabComponents/TabView/TabView';
import styles from './Search.module.scss';
import SearchByISWC from './SearchBy/SearchByISWC';
import SearchByWorkCode from './SearchBy/SearchByWorkCode';
import SearchByTitleContributors from './SearchBy/SearchByTitleContributors';
import { ISearchProps, ISearchState } from './SearchTypes';
import GridTextCell from '../../components/GridComponents/GridTextCell/GridTextCell';
import GridIconCell from '../../components/GridComponents/GridIconCell/GridIconCell';
import {
  VIEW_MORE_ICON,
  VIEW_MORE_ACTION,
  ISWC_FIELD,
  ORIGINAL_TITLE_FIELD,
  CREATOR_NAMES_FIELD,
  IP_NAME_NUMBER_FIELD,
  SUBMITTING_AGENCIES_FIELD,
  ORIGINAL_SUBMISSION_FIELD,
  LAST_UPDATE_FIELD,
  VIEW_MORE_FIELD,
  SEARCH_RESULTS,
  RANK_FIELD,
  NUMBER_TYPE,
  DATE_TYPE,
  DISAMBIGUATION_MESSAGE,
  MERGED_ICON,
  WORK_HAS_BEEN_MERGED,
  PUBLIC_MODE,
  PROVISIONAL,
  SEARCH,
} from '../../consts';
import Grid from '../../components/GridComponents/Grid/Grid';
import ViewMore from './ViewMore/ViewMore';
import { IGridRow, IGridHeaderCell } from '../../components/GridComponents/Grid/GridTypes';
import { getStrings } from '../../configuration/Localization';
import Loader from '../../components/Loader/Loader';
import {
  _getContributorNames,
  _getNameNumbers,
  _getAgenciesString,
  formatDateString,
  validateIswcAndFormat,
  getLoggedInAgencyId,
} from '../../shared/helperMethods';
import SubHeader from '../../components/SubHeader/SubHeader';
import ErrorHandler from '../../components/AlertMessage/ErrorHandler/ErrorHandler';
import { IIswcModel } from '../../redux/types/IswcTypes';
import SearchByCreator from './SearchBy/SearchByCreator';
import GridTableCell from '../../components/GridComponents/GridTableCell/GridTableCell';
import { IVerifiedSubmission } from '../Submission/SubmissionTypes';
import AlertMessage from '../../components/AlertMessage/AlertMessage';
import { getPortalMessages, PortalMessage } from '../../configuration/Configuration';
const queryString = require('query-string');

export default class Search extends Component<ISearchProps, ISearchState> {
  constructor(props: ISearchProps) {
    super(props);
    this.state = {
      formFields: {
        iswc: '',
        workCode: '',
        agency: getLoggedInAgencyId(),
        title: '',
        surnames: '',
        nameNumbers: '',
        baseNumbers: '',
        creatorNameNumbers: '',
        creatorBaseNumbers: '',
      },
      showSubMessage: true,
    };
  }

  componentDidMount = () => {
    const { SEARCH } = getStrings();
    document.title = SEARCH;

    const { search } = this.props.router.location;
    const { searchByIswc } = this.props;

    if (search) {
      const parsedSearchQuery = queryString.parse(search);
      if (parsedSearchQuery?.iswc) {
        searchByIswc(parsedSearchQuery.iswc);
      }
    }
  };

  componentWillUnmount = () => {
    const { clearSearchError } = this.props;
    clearSearchError();
  };

  _onFormChange = (event: any) => {
    const name = event.target.id;
    let value = event.target.value;

    if (name === 'iswc') {
      value = validateIswcAndFormat(value);
    } else if (name === 'nameNumbers' || name === 'creatorNameNumbers') {
      value = value.replace(/[^;0-9]/g, '');
    }

    this.setState({
      formFields: {
        ...this.state.formFields,
        [name]: value,
      },
    });
  };

  _searchByIswc = () => {
    const {
      formFields: { iswc },
    } = this.state;
    const { searchByIswc } = this.props;

    searchByIswc(iswc?.replace(/[^T0-9]/g, ''));
  };

  _searchByAgencyWorkCode = () => {
    const {
      formFields: { workCode, agency },
    } = this.state;
    const { searchByWorkCode } = this.props;

    searchByWorkCode(agency, workCode);
  };

  _searchByTitlesAndContributors = () => {
    const {
      formFields: { title, surnames, nameNumbers, baseNumbers },
    } = this.state;
    const { searchByTitleAndContributor } = this.props;

    searchByTitleAndContributor(title, surnames, nameNumbers, baseNumbers);
  };

  _searchByCreators = () => {
    const {
      formFields: { creatorNameNumbers, creatorBaseNumbers },
    } = this.state;
    const { searchByTitleAndContributor } = this.props;

    searchByTitleAndContributor(undefined, undefined, creatorNameNumbers, creatorBaseNumbers);
  };

  _getIswcLastUpdatedDate = (iswc: IIswcModel) => {
    const sortedWorks = iswc.works?.sort(
      (a, b) => new Date(b.lastModifiedDate).getTime() - new Date(a.lastModifiedDate).getTime(),
    );

    if (sortedWorks) return sortedWorks[0]?.lastModifiedDate;
  };

  _checkIfIswcWasDisambiguated = (works: IVerifiedSubmission[]) => {
    for (let x = 0; x < works.length; x++) {
      if (works[x].disambiguation) {
        return true;
      }
    }

    return false;
  };

  _toggleSubMessage = () => {
    const { showSubMessage } = this.state;

    this.setState({
      showSubMessage: !showSubMessage,
    });
  };

  _getTabs = () => {
    const {
      iswc,
      surnames,
      baseNumbers,
      nameNumbers,
      creatorBaseNumbers,
      creatorNameNumbers,
    } = this.state.formFields;
    const {
      SEARCH_BY_ISWC,
      SEARCH_BY_AGENCY_WORK_CODE,
      SEARCH_BY_TITLE,
      SEARCH_BY_CREATOR,
    } = getStrings();
    return [
      {
        text: SEARCH_BY_ISWC,
        component: (
          <SearchByISWC search={this._searchByIswc} onChange={this._onFormChange} value={iswc} />
        ),
      },
      {
        text: SEARCH_BY_AGENCY_WORK_CODE,
        component: (
          <SearchByWorkCode search={this._searchByAgencyWorkCode} onChange={this._onFormChange} />
        ),
      },
      {
        text: SEARCH_BY_TITLE,
        component: (
          <SearchByTitleContributors
            search={this._searchByTitlesAndContributors}
            onChange={this._onFormChange}
            surnames={surnames}
            baseNumbers={baseNumbers}
            nameNumbers={nameNumbers}
          />
        ),
      },
      {
        text: SEARCH_BY_CREATOR,
        component: (
          <SearchByCreator
            search={this._searchByCreators}
            onChange={this._onFormChange}
            creatorBaseNumbers={creatorBaseNumbers}
            creatorNameNumbers={creatorNameNumbers}
          />
        ),
      },
    ];
  };

  renderSearchResults = () => {
    const {
      searchResults,
      isSearching,
      mergeList,
      addToMergeList,
      removeFromMergeList,
      error,
      deleteSubmission,
      deletionLoading,
      deletionError,
      clearSubmissionError,
      assignedRoles,
      router,
    } = this.props;
    const strings = getStrings();

    if (isSearching) {
      return (
        <div className={styles.resultsContainer}>
          <Loader />
        </div>
      );
    } else if (searchResults !== undefined) {
      const searchResultsHeaderCells: IGridHeaderCell[] = [
        { text: strings[ISWC_FIELD], field: ISWC_FIELD, sortable: true },
        { text: strings[ORIGINAL_TITLE_FIELD], field: ORIGINAL_TITLE_FIELD, sortable: true },
        { text: strings[RANK_FIELD], field: RANK_FIELD, sortable: true, type: NUMBER_TYPE },
        { text: strings[CREATOR_NAMES_FIELD], field: CREATOR_NAMES_FIELD },
        { text: strings[IP_NAME_NUMBER_FIELD], field: IP_NAME_NUMBER_FIELD },
        {
          text: strings[SUBMITTING_AGENCIES_FIELD],
          field: SUBMITTING_AGENCIES_FIELD,
          hideInPublicMode: true,
        },
        {
          text: strings[ORIGINAL_SUBMISSION_FIELD],
          field: ORIGINAL_SUBMISSION_FIELD,
          sortable: true,
          type: DATE_TYPE,
          hideInPublicMode: true,
        },
        {
          text: strings[LAST_UPDATE_FIELD],
          field: LAST_UPDATE_FIELD,
          sortable: true,
          type: DATE_TYPE,
          hideInPublicMode: true,
        },
        { field: VIEW_MORE_FIELD },
      ];
      let searchResultsGridRows: IGridRow[] = [];

      searchResults.forEach((result, index) => {
        const iswcLastUpdatedDate = this._getIswcLastUpdatedDate(result);
        searchResultsGridRows.push({
          rowId: index,
          cells: [
            {
              element: (
                <div className={styles.cellDiv}>
                  <GridTextCell 
                    text={validateIswcAndFormat(result.iswc)} 
                    indicator={result.iswcStatus.toUpperCase() === PROVISIONAL}
                    delayWrap />
                  {result.parentISWC && (
                    <div className={styles.iconDiv}>
                      <GridIconCell
                        icon={MERGED_ICON}
                        alt={'Merged'}
                        hoverText={strings[WORK_HAS_BEEN_MERGED]}
                        largerIcon
                      />
                    </div>
                  )}
                </div>
              ),
              field: ISWC_FIELD,
            },
            {
              element: <GridTextCell text={result.originalTitle} />,
              field: ORIGINAL_TITLE_FIELD,
            },
            {
              element: <GridTextCell text={index + 1 + ''} />,
              field: RANK_FIELD,
            },
            {
              element: (
                <Media queries={{ small: { maxWidth: 1024 } }}>
                  {(matches) =>
                    matches.small ? (
                      <GridTextCell text={_getContributorNames(result.interestedParties)} />
                    ) : (
                      <GridTableCell
                        textArray={_getContributorNames(result.interestedParties).split(', ')}
                      />
                    )
                  }
                </Media>
              ),
              field: CREATOR_NAMES_FIELD,
            },
            {
              element: (
                <Media queries={{ small: { maxWidth: 1024 } }}>
                  {(matches) =>
                    matches.small ? (
                      <GridTextCell text={_getNameNumbers(result.interestedParties)} />
                    ) : (
                      <GridTableCell
                        textArray={_getNameNumbers(result.interestedParties).split(', ')}
                      />
                    )
                  }
                </Media>
              ),
              field: IP_NAME_NUMBER_FIELD,
            },
            {
              element: (
                <GridTextCell
                  text={_getAgenciesString(result.works!)}
                  displayIcon={result.works && this._checkIfIswcWasDisambiguated(result.works)}
                  displayIconPopUpText={strings[DISAMBIGUATION_MESSAGE]}
                />
              ),
              field: SUBMITTING_AGENCIES_FIELD,
              hideInPublicMode: true,
            },
            {
              element: <GridTextCell text={formatDateString(result.createdDate)} />,
              field: ORIGINAL_SUBMISSION_FIELD,
              hideInPublicMode: true,
            },
            {
              element: (
                <GridTextCell
                  text={iswcLastUpdatedDate ? formatDateString(iswcLastUpdatedDate) : ''}
                />
              ),
              field: LAST_UPDATE_FIELD,
              hideInPublicMode: true,
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
          viewMore: [
            <ViewMore
              iswcModel={result}
              mergeList={mergeList}
              addToMergeList={addToMergeList}
              removeFromMergeList={removeFromMergeList}
              deleteSubmission={deleteSubmission}
              deletionLoading={deletionLoading}
              deletionError={deletionError}
              clearSubmissionError={clearSubmissionError}
              assignedRoles={assignedRoles}
              router={router}
            />,
          ],
        });
      });
      return (
        <div>
          <div className={styles.sectionTitle}>{strings[SEARCH_RESULTS]}</div>
          <Grid
            headerCells={searchResultsHeaderCells}
            gridRows={searchResultsGridRows}
            pagination
            cellPadding={'20px 10px 20px 10px'}
            componentInstance={SEARCH}
          />
        </div>
      );
    } else if (error !== undefined) {
      return (
        <div className={styles.errorContainer}>
          <ErrorHandler error={error} />
        </div>
      );
    }
  };

  renderPortalMessage = () => {
    const messages = getPortalMessages();
    if (messages !== null && messages.length > 0) {
      let message: PortalMessage = messages[0];
      return (
        <div className={styles.portalMessage}>
          <AlertMessage
            message={message.messageHeader}
            type={'info'}
            subMessage={message.messageBody}
            close={this._toggleSubMessage}
          />
        </div>
      );
    }
  };

  render() {
    const { SEARCH } = getStrings();
    const { clearSearchError } = this.props;
    const { showSubMessage } = this.state;
    return (
      <div className={styles.container}>
        {process.env.REACT_APP_MODE === PUBLIC_MODE && showSubMessage && this.renderPortalMessage()}
        <SubHeader title={SEARCH} />
        <TabView tabs={this._getTabs()} tabClickAdditionalAction={clearSearchError} />
        <div className={styles.resultsContainer}>{this.renderSearchResults()}</div>
      </div>
    );
  }
}
