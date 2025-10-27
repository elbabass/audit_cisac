import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { IGridHeaderCellProps } from '../../../components/GridComponents/GridHeaderCell/GridHeaderCellTypes';
import GridHeaderCell from '../../../components/GridComponents/GridHeaderCell/GridHeaderCell';

describe('GridHeaderCell Component', () => {
  let props: IGridHeaderCellProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      text: 'Text',
      onClickHeaderCell: jest.fn(),
    };

    component = mount(<GridHeaderCell {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders text', () => {
    expect(component.text()).toBe('Text');
  });

  it('calls onClickHeaderCell when clicked', () => {
    component.find('td').simulate('click');
    expect(props.onClickHeaderCell).toHaveBeenCalledTimes(1);
  });

  it('renders icon', () => {
    expect(component.find('.icon').exists()).toBe(true);
  });
});
