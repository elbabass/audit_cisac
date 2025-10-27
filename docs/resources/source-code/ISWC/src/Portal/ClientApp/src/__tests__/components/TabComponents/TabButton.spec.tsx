import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { ITabButtonProps } from '../../../components/TabComponents/TabButton/TabButtonTypes';
import TabButton from '../../../components/TabComponents/TabButton/TabButton';

describe('TabButton Component', () => {
  let props: ITabButtonProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      text: 'Text',
      onClickButton: jest.fn(),
    };
    component = mount(<TabButton {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders text', () => {
    expect(component.text()).toBe('Text');
  });

  it('calls onClickButton when clicked', () => {
    component.find('div').simulate('click');
    expect(props.onClickButton).toHaveBeenCalledTimes(1);
  });

  it('calls onClickButton when clicked', () => {
    component.find('div').simulate('click');
    expect(props.onClickButton).toHaveBeenCalledTimes(1);
  });
});
