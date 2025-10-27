import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { ISubmissionProps, IVerifiedSubmission } from '../../../routes/Submission/SubmissionTypes';
import Submission from '../../../routes/Submission/Submission';
import SubmissionAdditionalDetails from '../../../routes/Submission/SubmissionAdditionalDetails/SubmissionAdditionalDetails';
import SubmissionMainDetails from '../../../routes/Submission/SubmissionMainDetails/SubmissionMainDetails';
import { SUBMISSION_ADDITIONAL_DETAILS_STEP, SUBMISSION_MAIN_DETAILS_STEP } from '../../../consts';
import ErrorHandler from '../../../components/AlertMessage/ErrorHandler/ErrorHandler';
import { RouteComponentProps } from 'react-router-dom';

jest.mock('../../../consts', () => {
  return {
    ...jest.requireActual('../../../consts'),
    getAgencies: jest.fn(() => [{ name: 'abc', agencyId: '213' }]),
  };
});

describe('Submission Page', () => {
  let props: ISubmissionProps;
  let component: ReactWrapper;
  SubmissionAdditionalDetails.displayName = 'SubmissionAdditionalDetails';
  SubmissionMainDetails.displayName = 'SubmissionMainDetails';
  ErrorHandler.displayName = 'ErrorHandler';

  beforeEach(() => {
    props = {
      step: SUBMISSION_MAIN_DETAILS_STEP,
      setSubmissionStep: jest.fn(),
      newSubmission: jest.fn(),
      updateSubmission: jest.fn(),
      searchByIswc: jest.fn(),
      verifiedSubmission: {} as IVerifiedSubmission,
      loading: false,
      potentialMatches: [],
      error: undefined,
      clearSearch: jest.fn,
      router: {} as RouteComponentProps,
      clearSubmissionError: jest.fn(),
    };
    component = mount(<Submission {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders SubmissionMainDetails if step is SUBMISSION_MAIN_DETAILS_STEP', () => {
    expect(component.find('SubmissionMainDetails').exists()).toBe(true);
  });

  it('renders Next ActionButton if step is SUBMISSION_MAIN_DETAILS_STEP', () => {
    expect(component.find('#nextDiv').exists()).toBe(true);
  });

  it('calls newSubmission if step is SUBMISSION_MAIN_DETAILS_STEP and ActionButton is clicked', () => {
    const button = component.find('ActionButton');
    button.simulate('click');
    expect(props.newSubmission).toHaveBeenCalledTimes(1);
  });

  it('calls updateSubmission if step is SUBMISSION_MAIN_DETAILS_STEP, updateInstance is true and ActionButton is clicked', () => {
    component = mount(<Submission {...props} updateInstance={true} />);
    const button = component.find('ActionButton');
    button.first().simulate('click');
    expect(props.updateSubmission).toHaveBeenCalledTimes(1);
  });

  it('renders SubmissionAdditionalDetails if step is SUBMISSION_ADDITIONAL_DETAILS_STEP', () => {
    component = mount(<Submission {...props} step={SUBMISSION_ADDITIONAL_DETAILS_STEP} />);
    expect(component.find('SubmissionAdditionalDetails').exists()).toBe(true);
  });

  it('renders two ActionButtons if step is SUBMISSION_ADDITIONAL_DETAILS_STEP', () => {
    component = mount(<Submission {...props} step={SUBMISSION_ADDITIONAL_DETAILS_STEP} />);
    expect(component.find('#submitGoDiv').exists()).toBe(true);
  });

  it('calls newSubmission if step is SUBMISSION_ADDITIONAL_DETAILS_STEP, submit ActionButton is clicked', () => {
    component = mount(<Submission {...props} step={SUBMISSION_ADDITIONAL_DETAILS_STEP} />);
    const button = component.find('ActionButton').first();
    button.simulate('click');
    expect(props.newSubmission).toHaveBeenCalledTimes(1);
  });

  it('renders ErrorHandler if there is an error prop', () => {
    component = mount(<Submission {...props} error={'error'} />);
    expect(component.find('ErrorHandler').exists()).toBe(true);
  });
});
