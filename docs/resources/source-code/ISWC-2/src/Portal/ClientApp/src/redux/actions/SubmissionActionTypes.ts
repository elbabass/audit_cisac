import {
  IVerifiedSubmission,
  IPotentialMatch,
  IAgencyWorkCodesSubmissionRow,
  ITitleSubmissionRow,
  ICreatorPublisherSubmissionRow,
  IDerivedFromWorksSubmissionRow,
  IPerformerSubmissionRow,
} from '../../routes/Submission/SubmissionTypes';

export const NEW_SUBMISSION_REQUEST = 'NEW_SUBMISSION_REQUEST';
export const NEW_SUBMISSION_SUCCESS = 'NEW_SUBMISSION_SUCCESS';
export const NEW_SUBMISSION_FAILURE = 'NEW_SUBMISSION_FAILURE';
export const UPDATE_SUBMISSION_REQUEST = 'UPDATE_SUBMISSION_REQUEST';
export const UPDATE_SUBMISSION_SUCCESS = 'UPDATE_SUBMISSION_SUCCESS';
export const UPDATE_SUBMISSION_FAILURE = 'UPDATE_SUBMISSION_FAILURE';
export const DELETE_SUBMISSION_REQUEST = 'DELETE_SUBMISSION_REQUEST';
export const DELETE_SUBMISSION_SUCCESS = 'DELETE_SUBMISSION_SUCCESS';
export const DELETE_SUBMISSION_FAILURE = 'DELETE_SUBMISSION_FAILURE';
export const SET_SUBMISSION_STEP = 'SET_SUBMISSION_STEP';
export const SET_POTENTIAL_MATCHES = 'SET_POTENTIAL_MATCHES';
export const SET_VERIFIED_SUBMISSION_PREVIEW = 'SET_VERIFIED_SUBMISSION_PREVIEW';
export const CLEAR_SUBMISSION_ERROR = 'CLEAR_SUBMISSION_ERROR';

export interface NewSubmissionRequest {
  type: typeof NEW_SUBMISSION_REQUEST;
  payload: {
    agencyWorkCode: IAgencyWorkCodesSubmissionRow[];
    titles: ITitleSubmissionRow[];
    creators: ICreatorPublisherSubmissionRow[];
    disambiguation: boolean;
    publishers?: ICreatorPublisherSubmissionRow[];
    derivedWorkType?: string;
    derivedFromWorks?: IDerivedFromWorksSubmissionRow[];
    disambiguationIswcs?: string[];
    disambiguationReason?: string;
    bvltr?: string;
    performers?: IPerformerSubmissionRow[];
    standardInstrumentation?: string;
  };
}

export interface NewSubmissionSuccess {
  type: typeof NEW_SUBMISSION_SUCCESS;
  payload: {
    verifiedSubmission: IVerifiedSubmission;
  };
}

export interface NewSubmissionFailure {
  type: typeof NEW_SUBMISSION_FAILURE;
  payload: {
    error: any;
  };
}

export interface UpdateSubmissionRequest {
  type: typeof UPDATE_SUBMISSION_REQUEST;
}

export interface UpdateSubmissionSuccess {
  type: typeof UPDATE_SUBMISSION_SUCCESS;
  payload: {
    verifiedSubmission: IVerifiedSubmission;
  };
}

export interface UpdateSubmissionFailure {
  type: typeof UPDATE_SUBMISSION_FAILURE;
  payload: {
    error: any;
  };
}

export interface DeleteSubmissionRequest {
  type: typeof DELETE_SUBMISSION_REQUEST;
}

export interface DeleteSubmissionSuccess {
  type: typeof DELETE_SUBMISSION_SUCCESS;
}

export interface DeleteSubmissionFailure {
  type: typeof DELETE_SUBMISSION_FAILURE;
  payload: {
    error: any;
  };
}

export interface SetSubmissionStep {
  type: typeof SET_SUBMISSION_STEP;
  payload: {
    step: number;
  };
}

export interface SetPotentialMatches {
  type: typeof SET_POTENTIAL_MATCHES;
  payload: {
    potentialMatches: IPotentialMatch[];
  };
}

export interface SetVerifiedSubmissionPreview {
  type: typeof SET_VERIFIED_SUBMISSION_PREVIEW;
  payload: {
    verifiedSubmission: IVerifiedSubmission;
  };
}

export interface ClearSubmissionError {
  type: typeof CLEAR_SUBMISSION_ERROR;
}

export type SubmissionActionTypes =
  | NewSubmissionRequest
  | NewSubmissionSuccess
  | NewSubmissionFailure
  | UpdateSubmissionRequest
  | UpdateSubmissionSuccess
  | UpdateSubmissionFailure
  | DeleteSubmissionRequest
  | DeleteSubmissionSuccess
  | DeleteSubmissionFailure
  | SetSubmissionStep
  | SetPotentialMatches
  | SetVerifiedSubmissionPreview
  | ClearSubmissionError;
