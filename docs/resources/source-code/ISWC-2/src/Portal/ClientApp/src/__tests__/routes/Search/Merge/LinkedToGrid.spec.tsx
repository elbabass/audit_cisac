import * as React from 'react';
import { shallow, ShallowWrapper } from 'enzyme';
import LinkedToGrid from '../../../../routes/Search/Demerge/LinkedToGrid';
import { ILinkedToGridProps } from '../../../../routes/Search/Demerge/DemergeTypes';

describe('LinkedToGrid Component', () => {
  let props: ILinkedToGridProps;
  let component: ShallowWrapper;

  beforeEach(() => {
    props = {
      linkedIswcs: [],
      iswcsToDemerge: [],
      addToIswcsToDemerge: jest.fn(),
    };

    component = shallow(<LinkedToGrid {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders Grid', () => {
    expect(component.find('Grid').exists()).toBe(true);
  });
});
