import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { ISwitchProps } from '../../../components/Switch/SwitchTypes';
import Switch from '../../../components/Switch/Switch';

describe('Switch Component', () => {
  let props: ISwitchProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      active: false,
      toggleSwitch: jest.fn(),
      text: 'Abcd',
    };
    component = mount(<Switch {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders text', () => {
    expect(component.find('.labelText').text()).toBe('Abcd');
  });

  it('toggleSwitch called when input updated', () => {
    component.find('input').simulate('change');
    expect(props.toggleSwitch).toHaveBeenCalled();
  });
});
