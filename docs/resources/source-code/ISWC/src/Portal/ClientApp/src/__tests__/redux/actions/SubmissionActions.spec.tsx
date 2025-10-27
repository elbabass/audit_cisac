import {
  NEW_SUBMISSION_REQUEST,
  NEW_SUBMISSION_SUCCESS,
  NEW_SUBMISSION_FAILURE,
  UPDATE_SUBMISSION_REQUEST,
  UPDATE_SUBMISSION_SUCCESS,
  UPDATE_SUBMISSION_FAILURE,
  SET_SUBMISSION_STEP,
  SET_POTENTIAL_MATCHES,
  SET_VERIFIED_SUBMISSION_PREVIEW,
  DELETE_SUBMISSION_REQUEST,
  DELETE_SUBMISSION_SUCCESS,
  DELETE_SUBMISSION_FAILURE,
} from '../../../redux/actions/SubmissionActionTypes';
import * as actions from '../../../redux/actions/SubmissionActions';
import {
  IVerifiedSubmission,
  IAgencyWorkCodesSubmissionRow,
  ITitleSubmissionRow,
  ICreatorPublisherSubmissionRow,
  IPotentialMatch,
} from '../../../routes/Submission/SubmissionTypes';

describe('Submission Actions', () => {
  it('newSubmissionRequest calls NEW_SUBMISSION_REQUEST', () => {
    const expectedAction = {
      type: NEW_SUBMISSION_REQUEST,
      payload: {
        agencyWorkCode: {} as IAgencyWorkCodesSubmissionRow[],
        titles: {} as ITitleSubmissionRow,
        creators: {} as ICreatorPublisherSubmissionRow[],
        disambiguation: false,
      },
    };
    expect(
      actions.newSubmissionRequest(
        {} as IAgencyWorkCodesSubmissionRow[],
        {} as ITitleSubmissionRow[],
        {} as ICreatorPublisherSubmissionRow[],
        false,
      ),
    ).toEqual(expectedAction);
  });

  it('newSubmissionSuccess calls NEW_SUBMISSION_SUCCESS', () => {
    const verifiedSubmission = {} as IVerifiedSubmission;
    const expectedAction = {
      type: NEW_SUBMISSION_SUCCESS,
      payload: { verifiedSubmission },
    };
    expect(actions.newSubmissionSuccess(verifiedSubmission)).toEqual(expectedAction);
  });

  it('newSubmissionFailure calls NEW_SUBMISSION_FAILURE', () => {
    const error = 'error';
    const expectedAction = {
      type: NEW_SUBMISSION_FAILURE,
      payload: { error },
    };
    expect(actions.newSubmissionFailure(error)).toEqual(expectedAction);
  });

  it('updateSubmissionRequest calls UPDATE_SUBMISSION_REQUEST', () => {
    const expectedAction = {
      type: UPDATE_SUBMISSION_REQUEST,
    };
    expect(actions.updateSubmissionRequest()).toEqual(expectedAction);
  });

  it('updateSubmissionSuccess calls UPDATE_SUBMISSION_SUCCESS', () => {
    const verifiedSubmission = {} as IVerifiedSubmission;
    const expectedAction = {
      type: UPDATE_SUBMISSION_SUCCESS,
      payload: { verifiedSubmission },
    };
    expect(actions.updateSubmissionSuccess(verifiedSubmission)).toEqual(expectedAction);
  });

  it('updateSubmissionFailure calls UPDATE_SUBMISSION_FAILURE', () => {
    const error = 'error';
    const expectedAction = {
      type: UPDATE_SUBMISSION_FAILURE,
      payload: { error },
    };
    expect(actions.updateSubmissionFailure(error)).toEqual(expectedAction);
  });

  it('setSubmissionStep calls SET_SUBMISSION_STEP', () => {
    const step = 2;
    const expectedAction = {
      type: SET_SUBMISSION_STEP,
      payload: { step },
    };
    expect(actions.setSubmissionStep(step)).toEqual(expectedAction);
  });

  it('setPotentialMatches calls SET_POTENTIAL_MATCHES', () => {
    const potentialMatches = {} as IPotentialMatch[];
    const expectedAction = {
      type: SET_POTENTIAL_MATCHES,
      payload: { potentialMatches },
    };
    expect(actions.setPotentialMatches(potentialMatches)).toEqual(expectedAction);
  });

  it('setVerifiedSubmissionPreview calls SET_VERIFIED_SUBMISSION_PREVIEW', () => {
    const verifiedSubmission = {} as IVerifiedSubmission;
    const expectedAction = {
      type: SET_VERIFIED_SUBMISSION_PREVIEW,
      payload: { verifiedSubmission },
    };
    expect(actions.setVerifiedSubmissionPreview(verifiedSubmission)).toEqual(expectedAction);
  });

  it('deleteSubmissionRequest calls DELETE_SUBMISSION_REQUEST', () => {
    const expectedAction = {
      type: DELETE_SUBMISSION_REQUEST,
    };
    expect(actions.deleteSubmissionRequest()).toEqual(expectedAction);
  });

  it('deleteSubmissionSuccess calls DELETE_SUBMISSION_SUCCESS', () => {
    const expectedAction = {
      type: DELETE_SUBMISSION_SUCCESS,
    };
    expect(actions.deleteSubmissionSuccess()).toEqual(expectedAction);
  });

  it('deleteSubmissionFailure calls DELETE_SUBMISSION_FAILURE', () => {
    const error = 'error';
    const expectedAction = {
      type: DELETE_SUBMISSION_FAILURE,
      payload: { error },
    };
    expect(actions.deleteSubmissionFailure(error)).toEqual(expectedAction);
  });
});
