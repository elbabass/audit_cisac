import { reducer } from '../../../redux/reducers/SubmissionReducer';
import { IVerifiedSubmission, IPotentialMatch } from '../../../routes/Submission/SubmissionTypes';
import { SUBMISSION_MAIN_DETAILS_STEP, SUBMISSION_ADDITIONAL_DETAILS_STEP } from '../../../consts';
import {
  SubmissionActionTypes,
  NEW_SUBMISSION_REQUEST,
  NEW_SUBMISSION_FAILURE,
  NEW_SUBMISSION_SUCCESS,
  SET_SUBMISSION_STEP,
  UPDATE_SUBMISSION_REQUEST,
  UPDATE_SUBMISSION_SUCCESS,
  UPDATE_SUBMISSION_FAILURE,
  SET_POTENTIAL_MATCHES,
  SET_VERIFIED_SUBMISSION_PREVIEW,
  DELETE_SUBMISSION_REQUEST,
  DELETE_SUBMISSION_SUCCESS,
  DELETE_SUBMISSION_FAILURE,
} from '../../../redux/actions/SubmissionActionTypes';

describe('Submission Reducer', () => {
  it('should return the initial state', () => {
    expect(reducer(undefined, {} as SubmissionActionTypes)).toEqual({
      verifiedSubmission: {} as IVerifiedSubmission,
      potentialMatches: [],
      loading: false,
      error: undefined,
      step: SUBMISSION_MAIN_DETAILS_STEP,
    });
  });

  it('should handle NEW_SUBMISSION_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: NEW_SUBMISSION_REQUEST,
      }),
    ).toEqual({
      verifiedSubmission: {} as IVerifiedSubmission,
      potentialMatches: [],
      loading: true,
      error: undefined,
      step: SUBMISSION_MAIN_DETAILS_STEP,
    });
  });

  it('should handle NEW_SUBMISSION_SUCCESS', () => {
    expect(
      reducer(undefined, {
        type: NEW_SUBMISSION_SUCCESS,
        payload: {
          verifiedSubmission: {} as IVerifiedSubmission,
        },
      }),
    ).toEqual({
      verifiedSubmission: {} as IVerifiedSubmission,
      potentialMatches: [],
      loading: false,
      error: undefined,
      step: SUBMISSION_MAIN_DETAILS_STEP,
    });
  });

  it('should handle NEW_SUBMISSION_FAILURE', () => {
    expect(
      reducer(undefined, {
        type: NEW_SUBMISSION_FAILURE,
        payload: {
          error: 'error',
        },
      }),
    ).toEqual({
      verifiedSubmission: {} as IVerifiedSubmission,
      potentialMatches: [],
      loading: false,
      error: 'error',
      step: SUBMISSION_MAIN_DETAILS_STEP,
    });
  });

  it('should handle UPDATE_SUBMISSION_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: UPDATE_SUBMISSION_REQUEST,
      }),
    ).toEqual({
      verifiedSubmission: {} as IVerifiedSubmission,
      potentialMatches: [],
      loading: true,
      error: undefined,
      step: SUBMISSION_MAIN_DETAILS_STEP,
    });
  });

  it('should handle UPDATE_SUBMISSION_SUCCESS', () => {
    expect(
      reducer(undefined, {
        type: UPDATE_SUBMISSION_SUCCESS,
        payload: {
          verifiedSubmission: {} as IVerifiedSubmission,
        },
      }),
    ).toEqual({
      verifiedSubmission: {} as IVerifiedSubmission,
      potentialMatches: [],
      loading: false,
      error: undefined,
      step: SUBMISSION_MAIN_DETAILS_STEP,
    });
  });

  it('should handle UPDATE_SUBMISSION_FAILURE', () => {
    expect(
      reducer(undefined, {
        type: UPDATE_SUBMISSION_FAILURE,
        payload: {
          error: 'error',
        },
      }),
    ).toEqual({
      verifiedSubmission: {} as IVerifiedSubmission,
      potentialMatches: [],
      loading: false,
      error: 'error',
      step: SUBMISSION_MAIN_DETAILS_STEP,
    });
  });

  it('should handle SET_SUBMISSION_STEP', () => {
    expect(
      reducer(undefined, {
        type: SET_SUBMISSION_STEP,
        payload: {
          step: SUBMISSION_ADDITIONAL_DETAILS_STEP,
        },
      }),
    ).toEqual({
      verifiedSubmission: {} as IVerifiedSubmission,
      potentialMatches: [],
      loading: false,
      error: undefined,
      step: SUBMISSION_ADDITIONAL_DETAILS_STEP,
    });
  });

  it('should handle SET_POTENTIAL_MATCHES', () => {
    expect(
      reducer(undefined, {
        type: SET_POTENTIAL_MATCHES,
        payload: {
          potentialMatches: {} as IPotentialMatch[],
        },
      }),
    ).toEqual({
      verifiedSubmission: {} as IVerifiedSubmission,
      potentialMatches: {} as IPotentialMatch[],
      loading: false,
      error: undefined,
      step: SUBMISSION_MAIN_DETAILS_STEP,
    });
  });

  it('should handle SET_VERIFIED_SUBMISSION_PREVIEW', () => {
    expect(
      reducer(undefined, {
        type: SET_VERIFIED_SUBMISSION_PREVIEW,
        payload: {
          verifiedSubmission: {} as IVerifiedSubmission,
        },
      }),
    ).toEqual({
      verifiedSubmission: {} as IVerifiedSubmission,
      potentialMatches: [],
      loading: false,
      error: undefined,
      step: SUBMISSION_MAIN_DETAILS_STEP,
    });
  });

  it('should handle UPDATE_SUBMISSION_REQUEST', () => {
    expect(
      reducer(undefined, {
        type: DELETE_SUBMISSION_REQUEST,
      }),
    ).toEqual({
      verifiedSubmission: {} as IVerifiedSubmission,
      potentialMatches: [],
      loading: true,
      error: undefined,
      step: SUBMISSION_MAIN_DETAILS_STEP,
    });
  });

  it('should handle DELETE_SUBMISSION_SUCCESS', () => {
    expect(
      reducer(undefined, {
        type: DELETE_SUBMISSION_SUCCESS,
        payload: {
          verifiedSubmission: {} as IVerifiedSubmission,
        },
      }),
    ).toEqual({
      verifiedSubmission: {} as IVerifiedSubmission,
      potentialMatches: [],
      loading: false,
      error: undefined,
      step: SUBMISSION_MAIN_DETAILS_STEP,
    });
  });

  it('should handle DELETE_SUBMISSION_FAILURE', () => {
    expect(
      reducer(undefined, {
        type: DELETE_SUBMISSION_FAILURE,
        payload: {
          error: 'error',
        },
      }),
    ).toEqual({
      verifiedSubmission: {} as IVerifiedSubmission,
      potentialMatches: [],
      loading: false,
      error: 'error',
      step: SUBMISSION_MAIN_DETAILS_STEP,
    });
  });
});
