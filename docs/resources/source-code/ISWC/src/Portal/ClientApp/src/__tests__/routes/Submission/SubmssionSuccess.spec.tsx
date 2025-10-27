import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import SubmissionSuccess from '../../../routes/Submission/SubmissionSuccess/SubmissionSuccess';
import { ISubmissionSuccessProps } from '../../../routes/Submission/SubmissionSuccess/SubmissionSuccessTypes';
import { IVerifiedSubmission } from '../../../routes/Submission/SubmissionTypes';

describe('SubmissionAdditionalDetails Component', () => {
  let props: ISubmissionSuccessProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      verifiedSubmission: {} as IVerifiedSubmission,
    };
    component = mount(<SubmissionSuccess {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders Grid', () => {
    expect(component.find('Grid').exists()).toBe(true);
  });
});
