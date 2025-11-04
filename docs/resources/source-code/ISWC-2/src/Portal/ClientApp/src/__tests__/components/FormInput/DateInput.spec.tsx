import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import DateInput from '../../../components/FormInput/DateInput';
import { IDateInputProps } from '../../../components/FormInput/FormInputTypes';

describe('Form input Component', () => {
  let component: ReactWrapper;
  let props: IDateInputProps;

  beforeEach(() => {
    props = {
      label: 'From',
      changeData: jest.fn(),
    };
    component = mount(<DateInput {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders label', () => {
    expect(component.find('.formLabel').exists()).toBe(true);
  });

  it('renders input', () => {
    expect(component.find('input').exists()).toBe(true);
  });
});
