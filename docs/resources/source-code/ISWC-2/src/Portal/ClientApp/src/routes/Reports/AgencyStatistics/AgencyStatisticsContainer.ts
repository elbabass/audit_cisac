import { connect } from 'react-redux';
import { ApplicationState } from '../../../redux/store/portal';
import { Dispatch } from 'redux';
import AgencyStatistics from './AgencyStatistics';
import { IAgencyStatisticsParamFields } from '../ReportsTypes';
import { agencyStatisticsSearchThunk } from '../../../redux/thunks/ReportsThunks';

export default connect(
  (state: ApplicationState) => ({
    loading: state.reportsReducer.loading,
    error: state.reportsReducer.error,
    agencyStatisticsSearchResults: state.reportsReducer.agencyStatisticsSearchResults,
  }),
  (dispatch: Dispatch) => ({
    reportsAgencyStatisticsSearch: (params: IAgencyStatisticsParamFields) =>
      dispatch<any>(agencyStatisticsSearchThunk(params)),
  }),
)(AgencyStatistics);
