import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { RouteComponentProps } from 'react-router-dom';
import { ApplicationState } from '../../../redux/store/portal';
import { searchByIswcBatchThunk } from '../../../redux/thunks/SearchThunks';
import { demergeIswcThunk } from '../../../redux/thunks/MergeThunks';
import Demerge from './Demerge';
import { IBatchSearchIswc } from '../../../redux/types/IswcTypes';
import { IDemergeIswc } from './DemergeTypes';
import { clearMergeError } from '../../../redux/actions/MergeActions';
import { clearSearch } from '../../../redux/actions/SearchActions';

export default connect(
  (state: ApplicationState, routerState: RouteComponentProps) => ({
    preferredIswc: routerState.location.state?.preferredIswc,
    linkedIswcs: routerState.location.state?.linkedIswcs,
    demerging: state.mergeReducer.processing,
    demergedSuccessfully: state.mergeReducer.demergedSuccessfully,
    error: state.mergeReducer.error,
    linkedIswcData: state.searchReducer.batchResults,
    loading: state.searchReducer.isSearching,
    router: routerState,
  }),
  (dispatch: Dispatch) => ({
    searchByIswcBatch: (iswcs?: string[]) => {
      let iswcBatch: IBatchSearchIswc[] = [];
      iswcs &&
        iswcs.forEach((iswc) => {
          iswcBatch.push({ iswc });
        });
      dispatch<any>(searchByIswcBatchThunk(iswcBatch));
    },
    demergeIswc: (preferredIswc: string, iswcsToDemerge: IDemergeIswc[]) =>
      dispatch<any>(demergeIswcThunk(preferredIswc, iswcsToDemerge)),
    clearMergeError: () => dispatch(clearMergeError()),
    clearDemergeData: () => dispatch(clearSearch()),
  }),
)(Demerge);
