import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { IAlertMessageProps } from '../../../components/AlertMessage/AlertMessageTypes';
import AlertMessage from '../../../components/AlertMessage/AlertMessage';

describe('AlertMessage Component', () => {
  let props: IAlertMessageProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      message: 'message',
      type: 'error',
    };
    component = mount(<AlertMessage {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders text component', () => {
    expect(component.find('.text').exists()).toBe(true);
  });

  it('displays error message passed from props', () => {
    expect(component.find('.text').text()).toEqual(props.message);
  });
});
