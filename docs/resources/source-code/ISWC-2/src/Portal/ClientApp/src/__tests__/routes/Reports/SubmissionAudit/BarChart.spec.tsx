import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { IBarChartProps } from '../../../../routes/Reports/SubmissionAudit/BarChart/BarChartTypes';
import BarChart from '../../../../routes/Reports/SubmissionAudit/BarChart/BarChart';

jest.mock('../../../../consts', () => {
  return {
    ...jest.requireActual('../../../../consts'),
    getAgencies: jest.fn(() => [{ name: 'abc', agencyId: '213' }]),
  };
});

describe('BarChart Page', () => {
  let props: IBarChartProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = { agencyStatisticsSearchResults: [], numberOfDaysInMonth: 0 };
    component = mount(<BarChart {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders Bar Chart', () => {
    expect(component.find('Bar').exists()).toBe(true);
  });
});
