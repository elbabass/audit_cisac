import { connect } from 'react-redux';
import Search from './Search';
import { ApplicationState } from '../../redux/store/portal';
import { Dispatch } from 'redux';
import {
  searchByIswcThunk,
  searchByWorkCodeThunk,
  searchByTitleAndContributorThunk,
} from '../../redux/thunks/SearchThunks';
import { IIswcModel } from '../../redux/types/IswcTypes';
import { addToMergeList, removeFromMergeList } from '../../redux/actions/MergeActions';
import { clearSearchError } from '../../redux/actions/SearchActions';
import { RouteComponentProps } from 'react-router-dom';
import { deleteSubmissionThunk } from '../../redux/thunks/SubmissionThunks';
import { clearSubmissionError } from '../../redux/actions/SubmissionActions';

export default connect(
  (state: ApplicationState, routerState: RouteComponentProps) => ({
    isSearching: state.searchReducer.isSearching,
    searchResults: state.searchReducer.searchResults,
    error: state.searchReducer.error,
    mergeList: state.mergeReducer.mergeList,
    router: routerState,
    deletionLoading: state.submissionReducer.loading,
    deletionError: state.submissionReducer.error,
    assignedRoles: state.appReducer.assignedRoles,
  }),
  (dispatch: Dispatch) => ({
    searchByIswc: (iswc?: string) => dispatch<any>(searchByIswcThunk(iswc)),
    searchByWorkCode: (agency?: string, workCode?: string) =>
      dispatch<any>(searchByWorkCodeThunk(agency, workCode)),
    searchByTitleAndContributor: (
      title?: string,
      surnames?: string,
      nameNumbers?: string,
      baseNumbers?: string,
    ) => dispatch<any>(searchByTitleAndContributorThunk(title, surnames, nameNumbers, baseNumbers)),
    addToMergeList: (iswc: IIswcModel) => dispatch(addToMergeList(iswc)),
    removeFromMergeList: (iswc: number | string) => dispatch(removeFromMergeList(iswc)),
    clearSearchError: () => dispatch(clearSearchError()),
    clearSubmissionError: () => dispatch(clearSubmissionError()),
    deleteSubmission: (
      preferredIswc: string,
      agency: string,
      workcode: string,
      reasonCode: string,
    ) => dispatch<any>(deleteSubmissionThunk(preferredIswc, agency, workcode, reasonCode)),
  }),
)(Search);
