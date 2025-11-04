import * as React from 'react';
import { shallow, ShallowWrapper } from 'enzyme';
import MergeGrid from '../../../../routes/Search/Merge/MergeGrid';
import { IMergeGridProps } from '../../../../routes/Search/Merge/MergeTypes';

describe('MergeGrid Component', () => {
  let props: IMergeGridProps;
  let component: ShallowWrapper;

  beforeEach(() => {
    props = { mergeList: [], updateMergeRequestData: jest.fn() };

    component = shallow(<MergeGrid {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders Grid', () => {
    expect(component.find('Grid').exists()).toBe(true);
  });
});
