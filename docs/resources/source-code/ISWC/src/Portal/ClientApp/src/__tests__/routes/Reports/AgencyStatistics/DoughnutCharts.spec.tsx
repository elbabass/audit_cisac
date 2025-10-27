import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import DoughnutCharts from '../../../../routes/Reports/AgencyStatistics/DoughnutCharts/DoughnutCharts';
import { IDoughnutChartsProps } from '../../../../routes/Reports/AgencyStatistics/DoughnutCharts/DoughnutChartsTypes';

jest.mock('../../../../consts', () => {
  return {
    ...jest.requireActual('../../../../consts'),
    getAgencies: jest.fn(() => [{ name: 'abc', agencyId: '213' }]),
  };
});

describe('DoughnutCharts Page', () => {
  let props: IDoughnutChartsProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {};
    component = mount(<DoughnutCharts {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders Doughnut Charts', () => {
    const charts = component.find('Doughnut');
    expect(charts.length).toBe(2);
  });
});
