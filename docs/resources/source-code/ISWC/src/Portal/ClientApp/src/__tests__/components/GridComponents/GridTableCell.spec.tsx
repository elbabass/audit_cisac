import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { IGridTableCellProps } from '../../../components/GridComponents/GridTableCell/GridTableCellTypes';
import GridTableCell from '../../../components/GridComponents/GridTableCell/GridTableCell';

describe('GridTableCell Component', () => {
  let props: IGridTableCellProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      textArray: ['Text', 'text'],
    };

    component = mount(<GridTableCell {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders correct amount of rows', () => {
    expect(component.find('.row').length).toBe(props.textArray?.length);
  });
});
