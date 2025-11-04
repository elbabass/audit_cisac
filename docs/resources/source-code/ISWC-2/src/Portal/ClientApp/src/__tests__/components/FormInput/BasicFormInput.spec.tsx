import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import BasicFormInput from '../../../components/FormInput/BasicFormInput';
import { IFormInputProps } from '../../../components/FormInput/FormInputTypes';

describe('Form input Component', () => {
  let component: ReactWrapper;
  let props: IFormInputProps;

  beforeEach(() => {
    props = {
      label: 'ISWC',
      name: 'iswc',
      onChange: jest.fn(),
    };
    component = mount(<BasicFormInput {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders label', () => {
    expect(component.find('label').exists()).toBe(true);
  });

  it('renders input', () => {
    expect(component.find('input').exists()).toBe(true);
  });

  it('onChange called when input updated', () => {
    component.find('input').simulate('change');
    expect(props.onChange).toHaveBeenCalled();
  });
});
