import { connect } from 'react-redux';
import Search from './Search';
import { ApplicationState } from '../../redux/store/publicPortal';
import { Dispatch } from 'redux';
import {
  searchByIswcThunk,
  searchByWorkCodeThunk,
  searchByTitleAndContributorThunk,
} from '../../redux/thunks/SearchThunks';
import { clearSearchError } from '../../redux/actions/SearchActions';
import { RouteComponentProps } from 'react-router-dom';

export default connect(
  (state: ApplicationState, routerState: RouteComponentProps) => ({
    isSearching: state.searchReducer.isSearching,
    searchResults: state.searchReducer.searchResults,
    error: state.searchReducer.error,
    router: routerState,
  }),
  (dispatch: Dispatch) => ({
    searchByIswc: (iswc?: string) => dispatch<any>(searchByIswcThunk(iswc)),
    searchByWorkCode: (agency?: string, workCode?: string) =>
      dispatch<any>(searchByWorkCodeThunk(agency, workCode)),
    searchByTitleAndContributor: (title?: string, surnames?: string, nameNumbers?: string) =>
      dispatch<any>(searchByTitleAndContributorThunk(title, surnames, nameNumbers)),
    clearSearchError: () => dispatch(clearSearchError()),
  }),
)(Search);
