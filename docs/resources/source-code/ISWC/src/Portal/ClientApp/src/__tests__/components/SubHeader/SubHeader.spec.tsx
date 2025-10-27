import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import SubHeader from '../../../components/SubHeader/SubHeader';
import { ISubHeaderProps } from '../../../components/SubHeader/SubHeaderTypes';

describe('TabHeader Component', () => {
  let props: ISubHeaderProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      title: 'Title',
    };
    component = mount(<SubHeader {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders title', () => {
    expect(component.find('.textDiv').text()).toBe('Title');
  });
});
