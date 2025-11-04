import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { IGridIconCellProps } from '../../../components/GridComponents/GridIconCell/GridIconCellTypes';
import GridIconCell from '../../../components/GridComponents/GridIconCell/GridIconCell';
import { VIEW_MORE_ICON } from '../../../consts';

describe('GridIconCell Component', () => {
  let props: IGridIconCellProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      text: 'Icon',
      icon: VIEW_MORE_ICON,
      alt: 'alt',
    };

    component = mount(<GridIconCell {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders text', () => {
    expect(component.text()).toBe('Icon');
  });

  it('renders icon', () => {
    expect(component.find('img').exists()).toBe(true);
  });
});
