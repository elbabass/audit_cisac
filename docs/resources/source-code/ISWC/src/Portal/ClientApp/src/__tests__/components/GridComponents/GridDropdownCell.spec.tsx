import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { IGridDropdownCellProps } from '../../../components/GridComponents/GridDropdownCell/GridDropdownCellTypes';
import GridDropdownCell from '../../../components/GridComponents/GridDropdownCell/GridDropdownCell';

describe('GridDropdownCell Component', () => {
  let props: IGridDropdownCellProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      options: [
        { name: 'A', value: 'A' },
        { name: 'B', value: 'B' },
      ],
      value: 'A',
      selectNewOption: jest.fn(),
    };

    component = mount(<GridDropdownCell {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders text', () => {
    expect(component.text()).toBe('AB');
  });

  it('dropdown value is correct', () => {
    expect(component.find('select').props().value).toBe('A');
  });

  it('dropdown onchange calls selectNewOption', () => {
    component.find('select').simulate('change', { target: { value: 'B' } });
    expect(props.selectNewOption).toHaveBeenCalledTimes(1);
  });
});
