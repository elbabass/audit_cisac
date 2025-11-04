import { Reducer } from 'redux';
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
  DELETE_SUBMISSION_SUCCESS,
  DELETE_SUBMISSION_FAILURE,
} from '../actions/SubmissionActionTypes';
import { IVerifiedSubmission, IPotentialMatch } from '../../routes/Submission/SubmissionTypes';
import { SUBMISSION_MAIN_DETAILS_STEP } from '../../consts';

export interface ISubmissionReducerState {
  verifiedSubmission: IVerifiedSubmission;
  potentialMatches: IPotentialMatch[];
  loading: boolean;
  error: any;
  step: number;
}

const initialState: ISubmissionReducerState = {
  verifiedSubmission: {} as IVerifiedSubmission,
  potentialMatches: [],
  loading: false,
  error: undefined,
  step: SUBMISSION_MAIN_DETAILS_STEP,
};

export const reducer: Reducer<ISubmissionReducerState> = (
  state = initialState,
  action: SubmissionActionTypes,
): ISubmissionReducerState => {
  if (state === undefined) {
    return initialState;
  }
  switch (action.type) {
    case NEW_SUBMISSION_REQUEST: {
      return {
        ...state,
        loading: true,
        error: undefined,
      };
    }
    case NEW_SUBMISSION_SUCCESS: {
      const { verifiedSubmission } = action.payload;

      return {
        ...state,
        verifiedSubmission,
        loading: false,
        error: undefined,
      };
    }
    case NEW_SUBMISSION_FAILURE: {
      const { error } = action.payload;
      return {
        ...state,
        error,
        loading: false,
      };
    }
    case UPDATE_SUBMISSION_REQUEST: {
      return {
        ...state,
        loading: true,
        error: undefined,
      };
    }
    case UPDATE_SUBMISSION_SUCCESS: {
      const { verifiedSubmission } = action.payload;

      return {
        ...state,
        verifiedSubmission,
        loading: false,
        error: undefined,
      };
    }
    case UPDATE_SUBMISSION_FAILURE: {
      const { error } = action.payload;
      return {
        ...state,
        error,
        loading: false,
      };
    }
    case DELETE_SUBMISSION_REQUEST: {
      return {
        ...state,
        loading: true,
      };
    }
    case DELETE_SUBMISSION_SUCCESS: {
      return {
        ...state,
        loading: false,
      };
    }
    case DELETE_SUBMISSION_FAILURE: {
      const { error } = action.payload;
      return {
        ...state,
        error,
        loading: false,
      };
    }
    case SET_SUBMISSION_STEP: {
      const { step } = action.payload;
      return {
        ...state,
        step,
      };
    }
    case SET_POTENTIAL_MATCHES: {
      const { potentialMatches } = action.payload;
      return {
        ...state,
        potentialMatches,
      };
    }
    case SET_VERIFIED_SUBMISSION_PREVIEW: {
      const { verifiedSubmission } = action.payload;

      return {
        ...state,
        verifiedSubmission,
        loading: false,
        error: undefined,
      };
    }
    case CLEAR_SUBMISSION_ERROR: {
      return {
        ...state,
        error: undefined,
      };
    }
    default:
      return state;
  }
};
