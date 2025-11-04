import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import ActionButton from '../../components/ActionButton/ActionButton';
import { IActionButtonProps } from '../../components/ActionButton/ActionbuttonTypes';

describe('ActionButton Component', () => {
  let props: IActionButtonProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      buttonText: 'Click here',
      buttonAction: jest.fn(),
    };

    component = mount(<ActionButton {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders text', () => {
    expect(component.text()).toBe('Click here');
  });

  it('calls buttonAction when clicked', () => {
    component.find('button').simulate('click');
    expect(props.buttonAction).toHaveBeenCalledTimes(1);
  });

  it('style changed when clicked', () => {
    component.find('button').simulate('click');
    expect(component.find('button').prop('className')).toBe('actionButton actionButtonClicked');
  });
});
