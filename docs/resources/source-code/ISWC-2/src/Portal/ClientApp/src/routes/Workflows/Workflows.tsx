import React, { PureComponent } from 'react';
import styles from './Workflows.module.scss';
import WorkflowsGrid from './WorkflowsGrid/WorkflowsGrid';
import { IWorkflowsProps, IWorkflowsState } from './WorkflowsTypes';
import WorkflowsHeader from './WorkflowsHeader';
import Loader from '../../components/Loader/Loader';
import ErrorHandler from '../../components/AlertMessage/ErrorHandler/ErrorHandler';
import WorkflowsAdvancedSearch from './AdvancedSearch/WorkflowsAdvancedSearch';
import { getStrings } from '../../configuration/Localization';
import SubHeader from '../../components/SubHeader/SubHeader';
import {
  SHOW_WORKFLOWS_ASSIGNED,
  SHOW_WORKFLOWS_CREATED,
  PENDING_WORKFLOWS,
  APPROVED_WORKFLOWS,
  REJECTED_WORKFLOWS,
} from '../../consts';
import axios from 'axios';
import Cookies from 'universal-cookie';
import { getLoggedInAgencyId, validateIswcAndFormatArray } from '../../shared/helperMethods';
import ActionButton from '../../components/ActionButton/ActionButton';

export default class Workflows extends PureComponent<IWorkflowsProps, IWorkflowsState> {
  agencyId: string;

  constructor(props: IWorkflowsProps) {
    super(props);

    this.state = {
      statusFilters: {
        pending: true,
        approved: false,
        rejected: false,
      },
      advancedSearchFilters: {
        iswc: undefined,
        workCodes: undefined,
        agency: undefined,
        workflowType: undefined,
      },
      dates: {
        fromDate: undefined,
        toDate: undefined,
      },
      showAssigned: true,
      showAdvancedSearch: false,
      workflowsToDisplay: undefined,
      pageLength: 20,
      startIndex: 0,
      numberOfLastPage: 1,
      lastPage: false,
    };

    this.agencyId = getLoggedInAgencyId();
  }

  componentDidMount() {
    const { WORKFLOWS } = getStrings();
    document.title = WORKFLOWS;

    const { clearWorkflowError } = this.props;
    const token = axios.defaults.headers.common['Authorization'];
    const cookies = new Cookies();

    if (!token && cookies.get('authToken')) {
      axios.defaults.headers.common['Authorization'] = cookies.get('authToken');
    }
    clearWorkflowError();

    this._search(0);
  }

  _updateSearchFilters = (event: any) => {
    const name = event.target.id;
    let value = event.target.value;
    if (name === 'iswc') {
      let array = value.split(';');
      array = validateIswcAndFormatArray(array);
      value = array.join(';');
    }

    this.setState({
      advancedSearchFilters: {
        ...this.state.advancedSearchFilters,
        [name]: value,
      },
    });
  };

  _updateDate = (event: any) => {
    const name = event.target.id;
    const value = event.target.value;
    this.setState({
      dates: {
        ...this.state.dates,
        [name]: value,
      },
    });
  };

  _toggleFilters = (filter: string) => {
    const {
      statusFilters: { pending, approved, rejected },
    } = this.state;

    let filters = { pending, approved, rejected };

    switch (filter) {
      case 'pending':
        filters = { ...filters, pending: !pending };
        break;
      case 'approved':
        filters = { ...filters, approved: !approved };
        break;
      case 'rejected':
        filters = { ...filters, rejected: !rejected };
        break;
    }

    this.setState({ statusFilters: filters });
  };

  _setPageLength = (rowsPerPage: number) => {
    this._search(0, rowsPerPage);
  };

  _toggleWorkflowsDisplayed = () => {
    const { showAssigned } = this.state;
    this.setState({ showAssigned: !showAssigned });
  };

  _clickNext = () => {
    if (this.state.numberOfLastPage !== undefined) {
      this._setNumberOfLastPage(this.state.numberOfLastPage + 1);
    }

    this._refreshGrid(true);
  };

  _clickPrevious = () => {
    if (this.state.numberOfLastPage !== undefined) {
      this._setNumberOfLastPage(this.state.numberOfLastPage - 1);
    }

    this._refreshGrid(false);
  };

  _setNumberOfLastPage = (numberOfLastPage: number) => {
    if (!this.state.lastPage) {
      this.setState({ numberOfLastPage: numberOfLastPage });
    }
  };

  _setLastPage = (lastPage: boolean) => {
    this.setState({ lastPage: lastPage });

    if (!lastPage) this.setState({ numberOfLastPage: 1 });
  };

  _refreshGrid = (isForward: boolean) => {
    let { startIndex, pageLength } = this.state;

    startIndex = isForward ? startIndex + pageLength : startIndex - pageLength;
    startIndex = startIndex < 0 ? 0 : startIndex;

    this._search(startIndex);
  };

  _searchOnClick = () => {
    const startIndex = 0;

    if (this._setLastPage !== undefined) {
      this._setLastPage(false);
    }

    this._search(startIndex);
  };

  _search = (startIndex: number, rowsPerPage?: number) => {
    const { getWorkflows } = this.props;
    const {
      showAssigned,
      pageLength,
      statusFilters: { pending, approved, rejected },
      dates: { fromDate, toDate },
      advancedSearchFilters: { iswc, workCodes, agency, workflowType },
    } = this.state;

    let statuses: number[] = [];
    statuses = pending ? (statuses = [...statuses, PENDING_WORKFLOWS]) : statuses;
    statuses = approved ? (statuses = [...statuses, APPROVED_WORKFLOWS]) : statuses;
    statuses = rejected ? (statuses = [...statuses, REJECTED_WORKFLOWS]) : statuses;

    rowsPerPage = rowsPerPage ?? pageLength;

    getWorkflows({
      agency: this.agencyId,
      showWorkflows: showAssigned ? SHOW_WORKFLOWS_ASSIGNED : SHOW_WORKFLOWS_CREATED,
      statuses: statuses,
      startIndex: startIndex,
      pageLength: rowsPerPage,
      fromDate: fromDate,
      toDate: toDate,
      iswc: iswc?.replace(/[^T0-9]/g, '') || '',
      agencyWorkCodes: workCodes || '',
      originatingAgency: agency?.replace('--', '') || '',
      workflowType: workflowType?.replace('--', '') || '',
    });

    this.setState({ startIndex: startIndex, pageLength: rowsPerPage });
  };

  _toggleAdvancedSearch = () => {
    const { showAdvancedSearch } = this.state;
    this.setState({
      showAdvancedSearch: !showAdvancedSearch,
    });
    this._resetAdvancedSearch();
  };

  _resetAdvancedSearch = () => {
    this.setState(
      {
        advancedSearchFilters: {
          agency: '--',
          iswc: '',
          workCodes: '',
          workflowType: '--',
        },
      },
      () => this._searchOnClick(),
    );
  };

  _renderWorkflowsGrid = () => {
    const { loading, error, updateWorkflows, updating, updateSuccessful, workflows } = this.props;
    const {
      showAssigned,
      statusFilters,
      pageLength,
      startIndex,
      numberOfLastPage,
      lastPage,
    } = this.state;
    if (loading) return <Loader />;
    else if (!loading && workflows !== undefined)
      return (
        <WorkflowsGrid
          workflows={workflows}
          updateWorkflows={updateWorkflows}
          showAssigned={showAssigned}
          updating={updating}
          updateSuccessful={updateSuccessful}
          search={this._searchOnClick}
          error={error}
          filters={statusFilters}
          hideSelectButtons={!showAssigned || !statusFilters.pending}
          onClickNext={this._clickNext}
          onClickPrevious={this._clickPrevious}
          changeRowsPerPage={this._setPageLength}
          startIndex={startIndex}
          rowsPerPage={pageLength}
          numberOfLastPage={numberOfLastPage}
          lastPage={lastPage}
          setNumberOfLastPage={this._setNumberOfLastPage}
          setLastPage={this._setLastPage}
        />
      );
    else if (!loading && error !== undefined) return <ErrorHandler error={error} />;
  };

  render() {
    const { statusFilters, showAssigned, showAdvancedSearch, advancedSearchFilters } = this.state;
    const { loading } = this.props;
    const { WORKFLOWS, SEARCH, RESET } = getStrings();
    return (
      <div>
        <SubHeader title={WORKFLOWS} />
        <div className={styles.container}>
          <WorkflowsHeader
            statusFilters={statusFilters}
            showAssigned={showAssigned}
            updateStatusFilters={this._toggleFilters}
            changeWorkFlowsDisplayed={this._toggleWorkflowsDisplayed}
            updateDates={this._updateDate}
            loading={loading}
          />

          <WorkflowsAdvancedSearch
            showAdvancedSearch={showAdvancedSearch}
            toggleAdvancedSearch={this._toggleAdvancedSearch}
            searchFilters={advancedSearchFilters}
            updateSearchFilters={this._updateSearchFilters}
          />
          <div className={styles.buttonsContainer}>
            <div className={styles.searchButton}>
              <ActionButton buttonText={SEARCH} buttonAction={this._searchOnClick} />
            </div>
            {showAdvancedSearch && (
              <div className={styles.resetText} onClick={() => this._resetAdvancedSearch()}>
                {RESET}
              </div>
            )}
          </div>
          {this._renderWorkflowsGrid()}
        </div>
      </div>
    );
  }
}
