import {
  SubmissionActionTypes,
  NEW_SUBMISSION_REQUEST,
  NEW_SUBMISSION_SUCCESS,
  NEW_SUBMISSION_FAILURE,
  SET_SUBMISSION_STEP,
  SET_POTENTIAL_MATCHES,
  UPDATE_SUBMISSION_REQUEST,
  UPDATE_SUBMISSION_SUCCESS,
  UPDATE_SUBMISSION_FAILURE,
  SET_VERIFIED_SUBMISSION_PREVIEW,
  CLEAR_SUBMISSION_ERROR,
  DELETE_SUBMISSION_REQUEST,
  DELETE_SUBMISSION_FAILURE,
  DELETE_SUBMISSION_SUCCESS,
} from './SubmissionActionTypes';
import {
  IVerifiedSubmission,
  IPotentialMatch,
  IAgencyWorkCodesSubmissionRow,
  ITitleSubmissionRow,
  ICreatorPublisherSubmissionRow,
  IDerivedFromWorksSubmissionRow,
  IPerformerSubmissionRow,
} from '../../routes/Submission/SubmissionTypes';

export function newSubmissionRequest(
  agencyWorkCode: IAgencyWorkCodesSubmissionRow[],
  titles: ITitleSubmissionRow[],
  creators: ICreatorPublisherSubmissionRow[],
  disambiguation: boolean,
  publishers?: ICreatorPublisherSubmissionRow[],
  derivedWorkType?: string,
  derivedFromWorks?: IDerivedFromWorksSubmissionRow[],
  disambiguationIswcs?: string[],
  disambiguationReason?: string,
  bvltr?: string,
  performers?: IPerformerSubmissionRow[],
  standardInstrumentation?: string,
): SubmissionActionTypes {
  return {
    type: NEW_SUBMISSION_REQUEST,
    payload: {
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
    },
  };
}

export function newSubmissionSuccess(
  verifiedSubmission: IVerifiedSubmission,
): SubmissionActionTypes {
  return {
    type: NEW_SUBMISSION_SUCCESS,
    payload: {
      verifiedSubmission,
    },
  };
}

export function newSubmissionFailure(error: any): SubmissionActionTypes {
  return {
    type: NEW_SUBMISSION_FAILURE,
    payload: {
      error,
    },
  };
}

export function updateSubmissionRequest(): SubmissionActionTypes {
  return {
    type: UPDATE_SUBMISSION_REQUEST,
  };
}

export function updateSubmissionSuccess(
  verifiedSubmission: IVerifiedSubmission,
): SubmissionActionTypes {
  return {
    type: UPDATE_SUBMISSION_SUCCESS,
    payload: {
      verifiedSubmission,
    },
  };
}

export function updateSubmissionFailure(error: any): SubmissionActionTypes {
  return {
    type: UPDATE_SUBMISSION_FAILURE,
    payload: {
      error,
    },
  };
}

export function deleteSubmissionRequest(): SubmissionActionTypes {
  return {
    type: DELETE_SUBMISSION_REQUEST,
  };
}

export function deleteSubmissionSuccess(): SubmissionActionTypes {
  return {
    type: DELETE_SUBMISSION_SUCCESS,
  };
}

export function deleteSubmissionFailure(error: any): SubmissionActionTypes {
  return {
    type: DELETE_SUBMISSION_FAILURE,
    payload: {
      error,
    },
  };
}

export function setSubmissionStep(step: number): SubmissionActionTypes {
  return {
    type: SET_SUBMISSION_STEP,
    payload: {
      step,
    },
  };
}

export function setPotentialMatches(potentialMatches: IPotentialMatch[]): SubmissionActionTypes {
  return {
    type: SET_POTENTIAL_MATCHES,
    payload: {
      potentialMatches,
    },
  };
}

export function setVerifiedSubmissionPreview(
  verifiedSubmission: IVerifiedSubmission,
): SubmissionActionTypes {
  return {
    type: SET_VERIFIED_SUBMISSION_PREVIEW,
    payload: {
      verifiedSubmission,
    },
  };
}

export function clearSubmissionError(): SubmissionActionTypes {
  return {
    type: CLEAR_SUBMISSION_ERROR,
  };
}
