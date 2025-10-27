import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';

import SelectPreferredIswc from '../../../routes/Submission/SubmissionAdditionalDetails/SelectPreferredIswc/SelectPreferredIswc';
import { ISelectPreferredIswcsProps } from '../../../routes/Submission/SubmissionAdditionalDetails/SelectPreferredIswc/SelectPreferredIswcTypes';

describe('SelectPreferredIswc Component', () => {
  let props: ISelectPreferredIswcsProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      preferredIswc: 'preferred',
      updateSubmissionDataString: jest.fn(),
      searchByIswc: jest.fn(),
      potentialMatches: [],
    };
    component = mount(<SelectPreferredIswc {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders Grid', () => {
    expect(component.find('Grid').exists()).toBe(true);
  });
});
