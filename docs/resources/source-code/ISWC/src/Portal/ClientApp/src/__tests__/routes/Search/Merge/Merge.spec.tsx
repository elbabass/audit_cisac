import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { IMergeProps } from '../../../../routes/Search/Merge/MergeTypes';
import Merge from '../../../../routes/Search/Merge/Merge';

describe('Merge Page', () => {
  let props: IMergeProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      mergeList: [],
      merging: false,
      mergedSuccessfully: undefined,
      error: undefined,
      removeFromMergeList: jest.fn(),
      mergeIswcs: jest.fn(),
      clearMergeError: jest.fn(),
    };
    component = mount(<Merge {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders Merge Grid', () => {
    expect(component.find('MergeGrid').exists()).toBe(true);
  });
});
