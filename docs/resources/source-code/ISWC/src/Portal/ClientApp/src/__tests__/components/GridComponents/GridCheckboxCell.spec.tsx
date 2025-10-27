import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { IGridCheckboxCellProps } from '../../../components/GridComponents/GridCheckboxCell/GridCheckboxCellTypes';
import GridCheckboxCell from '../../../components/GridComponents/GridCheckboxCell/GridCheckboxCell';

describe('GridCheckboxCell Component', () => {
  let props: IGridCheckboxCellProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      text: 'Demerge',
      onClickCheckbox: jest.fn(),
    };

    component = mount(<GridCheckboxCell {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders text', () => {
    expect(component.text()).toBe('Demerge');
  });

  it('calls onClickCheckbox when clicked', () => {
    component.find('input').simulate('change');
    expect(props.onClickCheckbox).toHaveBeenCalledTimes(1);
  });

  it('checkbox is checked if prop is true', () => {
    component = mount(<GridCheckboxCell {...props} checked />);
    expect(component.find('input').props().checked).toBe(true);
  });
});
