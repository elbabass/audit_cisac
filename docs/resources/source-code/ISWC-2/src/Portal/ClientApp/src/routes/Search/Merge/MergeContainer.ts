import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { ApplicationState } from '../../../redux/store/portal';
import Merge from './Merge';
import { removeFromMergeList, clearMergeError } from '../../../redux/actions/MergeActions';
import { mergeIswcsThunk } from '../../../redux/thunks/MergeThunks';
import { IMergeBody } from '../../../redux/types/IswcTypes';

export default connect(
  (state: ApplicationState) => ({
    mergeList: state.mergeReducer.mergeList,
    merging: state.mergeReducer.processing,
    mergedSuccessfully: state.mergeReducer.mergedSuccessfully,
    error: state.mergeReducer.error,
  }),
  (dispatch: Dispatch) => ({
    removeFromMergeList: (iswcToRemove: number | string) =>
      dispatch(removeFromMergeList(iswcToRemove)),
    mergeIswcs: (preferredIswc?: string, agency?: string, mergeBody?: IMergeBody) =>
      dispatch<any>(mergeIswcsThunk(preferredIswc, agency, mergeBody)),
    clearMergeError: () => dispatch(clearMergeError()),
  }),
)(Merge);
