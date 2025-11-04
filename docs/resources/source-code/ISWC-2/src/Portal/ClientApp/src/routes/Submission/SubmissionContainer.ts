import { connect } from 'react-redux';
import Submission from './Submission';
import { ApplicationState } from '../../redux/store/portal';
import { Dispatch } from 'redux';
import { setSubmissionStep, clearSubmissionError } from '../../redux/actions/SubmissionActions';
import {
  newSubmissionThunk,
  searchByIswcSubmissionThunk,
  updateSubmissionThunk,
} from '../../redux/thunks/SubmissionThunks';
import {
  IAgencyWorkCodesSubmissionRow,
  ITitleSubmissionRow,
  ICreatorPublisherSubmissionRow,
  IDerivedFromWorksSubmissionRow,
  IPerformerSubmissionRow,
} from './SubmissionTypes';
import { RouteComponentProps } from 'react-router-dom';
import { clearSearch } from '../../redux/actions/SearchActions';

export default connect(
  (state: ApplicationState, routerState: RouteComponentProps) => ({
    step: state.submissionReducer.step,
    verifiedSubmission: state.submissionReducer.verifiedSubmission,
    potentialMatches: state.submissionReducer.potentialMatches,
    loading: state.submissionReducer.loading,
    error: state.submissionReducer.error,
    agencyWorkCode: routerState.location.state && routerState.location.state.agencyWorkCode,
    titles: routerState.location.state && routerState.location.state.titles,
    creators: routerState.location.state && routerState.location.state.creators,
    disambiguation: routerState.location.state && routerState.location.state.disambiguation,
    preferredIswc: routerState.location.state && routerState.location.state.preferredIswc,
    publishers: routerState.location.state && routerState.location.state.publishers,
    derivedWorkType: routerState.location.state && routerState.location.state.derivedWorkType,
    derivedFromWorks: routerState.location.state && routerState.location.state.derivedFromWorks,
    disambiguationIswcs:
      routerState.location.state && routerState.location.state.disambiguationIswcs,
    disambiguationReason:
      routerState.location.state && routerState.location.state.disambiguationReason,
    bvltr: routerState.location.state && routerState.location.state.bvltr,
    performers: routerState.location.state && routerState.location.state.performers,
    standardInstrumentation:
      routerState.location.state && routerState.location.state.standardInstrumentation,
    updateInstance: routerState.location.state && routerState.location.state.updateInstance,
    router: routerState,
  }),
  (dispatch: Dispatch) => ({
    setSubmissionStep: (step: number) => dispatch(setSubmissionStep(step)),
    newSubmission: (
      previewDisambiguation: boolean,
      agencyWorkCode: IAgencyWorkCodesSubmissionRow[],
      titles: ITitleSubmissionRow[],
      creators: ICreatorPublisherSubmissionRow[],
      disambiguation: boolean,
      preferredIswc?: string,
      publishers?: ICreatorPublisherSubmissionRow[],
      derivedWorkType?: string,
      derivedFromWorks?: IDerivedFromWorksSubmissionRow[],
      disambiguationIswcs?: string[],
      disambiguationReason?: string,
      bvltr?: string,
      performers?: IPerformerSubmissionRow[],
      standardInstrumentation?: string,
    ) =>
      dispatch<any>(
        newSubmissionThunk(
          previewDisambiguation,
          agencyWorkCode,
          titles,
          creators,
          disambiguation,
          preferredIswc,
          publishers,
          derivedWorkType,
          derivedFromWorks,
          disambiguationIswcs,
          disambiguationReason,
          bvltr,
          performers,
          standardInstrumentation,
        ),
      ),
    updateSubmission: (
      previewDisambiguation: boolean,
      agencyWorkCode: IAgencyWorkCodesSubmissionRow[],
      titles: ITitleSubmissionRow[],
      creators: ICreatorPublisherSubmissionRow[],
      disambiguation: boolean,
      disambiguationIswcs?: string[],
      preferredIswc?: string,
      publishers?: ICreatorPublisherSubmissionRow[],
      derivedWorkType?: string,
      derivedFromWorks?: IDerivedFromWorksSubmissionRow[],
      performers?: IPerformerSubmissionRow[],
    ) =>
      dispatch<any>(
        updateSubmissionThunk(
          previewDisambiguation,
          agencyWorkCode,
          titles,
          creators,
          disambiguation,
          disambiguationIswcs,
          preferredIswc,
          publishers,
          derivedWorkType,
          derivedFromWorks,
          performers,
        ),
      ),
    searchByIswc: (iswc?: string) => dispatch<any>(searchByIswcSubmissionThunk(iswc)),
    clearSearch: () => dispatch(clearSearch()),
    clearSubmissionError: () => dispatch(clearSubmissionError()),
  }),
)(Submission);
