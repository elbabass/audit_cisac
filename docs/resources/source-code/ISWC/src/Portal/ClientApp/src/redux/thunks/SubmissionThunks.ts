import { Action, Dispatch } from 'redux';
import {
  newSubmissionRequest,
  newSubmissionSuccess,
  newSubmissionFailure,
  setSubmissionStep,
  setPotentialMatches,
  updateSubmissionRequest,
  updateSubmissionFailure,
  updateSubmissionSuccess,
  setVerifiedSubmissionPreview,
  deleteSubmissionRequest,
  deleteSubmissionSuccess,
  deleteSubmissionFailure,
} from '../actions/SubmissionActions';
import { newSubmission, updateSubmission, deleteSubmission } from '../services/SubmissionService';
import {
  IAgencyWorkCodesSubmissionRow,
  ITitleSubmissionRow,
  ICreatorPublisherSubmissionRow,
  IDerivedFromWorksSubmissionRow,
  ISubmissionResponse,
  IPerformerSubmissionRow,
} from '../../routes/Submission/SubmissionTypes';
import { ApplicationState } from '../store/portal';
import { ThunkAction } from 'redux-thunk';
import {
  SUBMISSION_SUCCESS_STEP,
  SUBMISSION_ADDITIONAL_DETAILS_STEP,
  SUBMISSION_MAIN_DETAILS_STEP,
} from '../../consts';
import { searchByIswc } from '../services/SearchService';
import { turnOnMaintenanceMode } from '../actions/AppActions';

export const newSubmissionThunk = (
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
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(
      newSubmissionRequest(
        agencyWorkCode,
        titles,
        creators,
        disambiguation,
        publishers,
        derivedWorkType,
        derivedFromWorks,
        disambiguationIswcs,
        disambiguationReason,
        bvltr,
        performers,
        standardInstrumentation,
      ),
    );
    return newSubmission(
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
    )
      .then((res: ISubmissionResponse) => {
        if (res.verifiedSubmission.iswc) {
          dispatch(newSubmissionSuccess(res.verifiedSubmission));
          dispatch(setPotentialMatches(res.potentialMatches));
          dispatch(setSubmissionStep(SUBMISSION_SUCCESS_STEP));
        } else {
          dispatch(setVerifiedSubmissionPreview(res.verifiedSubmission));
          dispatch(setPotentialMatches(res.potentialMatches));
          dispatch(setSubmissionStep(SUBMISSION_ADDITIONAL_DETAILS_STEP));
        }
      })
      .catch((error: any) => {
        if (error.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(newSubmissionFailure(error));
      });
  };
};

export const updateSubmissionThunk = (
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
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch, getState: () => ApplicationState) => {
    dispatch(updateSubmissionRequest());
    return updateSubmission(
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
    )
      .then((res: ISubmissionResponse) => {
        const { step } = getState().submissionReducer;
        if (step === SUBMISSION_MAIN_DETAILS_STEP) {
          dispatch(setVerifiedSubmissionPreview(res.verifiedSubmission));
          dispatch(setPotentialMatches(res.potentialMatches));
          dispatch(setSubmissionStep(SUBMISSION_ADDITIONAL_DETAILS_STEP));
        } else if (step === SUBMISSION_ADDITIONAL_DETAILS_STEP) {
          dispatch(updateSubmissionSuccess(res.verifiedSubmission));
          dispatch(setSubmissionStep(SUBMISSION_SUCCESS_STEP));
        }
      })
      .catch((error: any) => {
        if (error.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(updateSubmissionFailure(error));
      });
  };
};

export const deleteSubmissionThunk = (
  preferredIswc: string,
  agency: string,
  workcode: string,
  reasonCode: string,
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    dispatch(deleteSubmissionRequest());
    return deleteSubmission(preferredIswc, agency, workcode, reasonCode)
      .then(() => {
        dispatch(deleteSubmissionSuccess());
      })
      .catch((error: any) => {
        if (error.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        dispatch(deleteSubmissionFailure(error));
      });
  };
};

export const searchByIswcSubmissionThunk = (
  iswc?: string,
): ThunkAction<void, ApplicationState, null, Action<string>> => {
  return (dispatch: Dispatch) => {
    return searchByIswc(iswc)
      .then((res) => {
        return res;
      })
      .catch((error: any) => {
        if (error.response?.status === 503) {
          dispatch(turnOnMaintenanceMode());
        }
        return error;
      });
  };
};
