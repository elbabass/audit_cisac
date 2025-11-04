import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { IGridTextCellProps } from '../../../components/GridComponents/GridTextCell/GridTextCellTypes';
import GridTextCell from '../../../components/GridComponents/GridTextCell/GridTextCell';

describe('GridTextCell Component', () => {
  let props: IGridTextCellProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      text: 'Text',
    };

    component = mount(<GridTextCell {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders text', () => {
    expect(component.text()).toBe('Text');
  });
});
