import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import SubmissionAdditionalDetails from '../../../routes/Submission/SubmissionAdditionalDetails/SubmissionAdditionalDetails';
import { ISubmissionAdditionalDetailsProps } from '../../../routes/Submission/SubmissionAdditionalDetails/SubmissionAdditionalDetailsTypes';
import { PERFORMERS_SUBMISSION_ROW } from '../../../consts';
import { IVerifiedSubmission } from '../../../routes/Submission/SubmissionTypes';

describe('SubmissionAdditionalDetails Component', () => {
  let props: ISubmissionAdditionalDetailsProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      updateSubmissionDataArray: jest.fn(),
      updateSubmissionDataString: jest.fn(),
      addElementToArray: jest.fn(),
      removeElementFromArray: jest.fn(),
      performers: [PERFORMERS_SUBMISSION_ROW],
      disambiguationReason: '',
      disambiguationIswcs: [],
      bvltr: '',
      standardInstrumentation: '',
      verifiedSubmission: {} as IVerifiedSubmission,
      preferredIswc: '',
      searchByIswc: jest.fn(),
      potentialMatches: [],
      setDisambiguation: jest.fn(),
    };
    component = mount(<SubmissionAdditionalDetails {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders TabView', () => {
    expect(component.find('TabView').exists()).toBe(true);
  });
});
