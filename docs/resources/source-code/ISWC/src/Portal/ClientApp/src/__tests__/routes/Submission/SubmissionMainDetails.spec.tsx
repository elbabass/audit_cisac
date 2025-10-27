import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { ISubmissionMainDetailsProps } from '../../../routes/Submission/SubmissionMainDetails/SubmissionMainDetailsTypes';
import SubmissionMainDetails from '../../../routes/Submission/SubmissionMainDetails/SubmissionMainDetails';
import {
  AGENCY_WORK_CODES_SUBMISSION_ROW,
  PUBLISHERS_SUBMISSION_ROW,
  CREATORS_SUBMISSION_ROW,
  DERIVED_FROM_WORKS_SUBMISSION_ROW,
  PERFORMERS_SUBMISSION_ROW,
} from '../../../consts';

jest.mock('../../../consts', () => {
  return {
    ...jest.requireActual('../../../consts'),
    getAgencies: jest.fn(() => [{ name: 'abc', agencyId: '213' }]),
  };
});

describe('SubmissionMainDetails Component', () => {
  let props: ISubmissionMainDetailsProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      agencyWorkCodes: [AGENCY_WORK_CODES_SUBMISSION_ROW],
      titles: [{ type: 'Original Title', title: '' }],
      creators: [CREATORS_SUBMISSION_ROW],
      publishers: [PUBLISHERS_SUBMISSION_ROW],
      derivedWorkType: '',
      derivedFromWorks: [DERIVED_FROM_WORKS_SUBMISSION_ROW],
      updateSubmissionDataString: jest.fn(),
      updateSubmissionDataArray: jest.fn(),
      addElementToSubmissionDataArray: jest.fn(),
      addElementToArray: jest.fn(),
      removeElementFromArray: jest.fn(),
      updateInstance: false,
      performers: [PERFORMERS_SUBMISSION_ROW],
    };
    component = mount(<SubmissionMainDetails {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders six SubmissionGrids', () => {
    const grids = component.find('SubmissionGrid');
    expect(grids.length).toBe(6);
  });
});
