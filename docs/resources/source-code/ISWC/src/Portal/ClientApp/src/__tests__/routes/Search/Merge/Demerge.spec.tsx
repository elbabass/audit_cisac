import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { IDemergeProps } from '../../../../routes/Search/Demerge/DemergeTypes';
import Demerge from '../../../../routes/Search/Demerge/Demerge';

describe('Demerge Page', () => {
  let props: IDemergeProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      preferredIswc: {
        iswc: 'T0000000000',
        iswcStatus: 'Preferred',
        agency: '3',
        createdDate: '29 Aug, 2019 - 12:00 hrs',
        lastModifiedDate: '29 Aug, 2019 - 12:00 hrs',
        lastModifiedBy: 'WEB',
        interestedParties: [],
        originalTitle: 'Title',
        works: [],
      },
      linkedIswcs: [],
      error: undefined,
      searchByIswcBatch: jest.fn(),
      demergeIswc: jest.fn(),
      clearMergeError: jest.fn(),
      clearDemergeData: jest.fn(),
      router: {
        history: {} as any,
        location: {} as any,
        match: {} as any,
      },
    };
    component = mount(<Demerge {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });
});
