import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import GridInputCell from '../../../components/GridComponents/GridInputCell/GridInputCell';
import { IGridInputCellProps } from '../../../components/GridComponents/GridInputCell/GridInputCellTypes';

describe('GridInputCell Component', () => {
  let props: IGridInputCellProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      onChange: jest.fn(),
      value: 'Hi',
    };

    component = mount(<GridInputCell {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('calls onChange when input is changed', () => {
    component.find('input').simulate('change', { target: { value: 'Hii' } });
    expect(props.onChange).toHaveBeenCalledTimes(1);
  });
});
