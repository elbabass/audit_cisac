import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { IAgencyStatisticsProps } from '../../../../routes/Reports/AgencyStatistics/AgencyStatisticsTypes';
import AgencyStatistics from '../../../../routes/Reports/AgencyStatistics/AgencyStatistics';
import { DoughnutCharts } from '../../../../routes/Reports/AgencyStatistics/DoughnutCharts/DoughnutCharts';

jest.mock('../../../../consts', () => {
  return {
    ...jest.requireActual('../../../../consts'),
    getAgencies: jest.fn(() => [{ name: 'abc', agencyId: '213' }]),
  };
});

describe('AgencyStatistics Page', () => {
  let props: IAgencyStatisticsProps;
  let component: ReactWrapper;
  DoughnutCharts.displayName = 'DoughnutCharts';

  beforeEach(() => {
    props = {
      loading: false,
      reportsAgencyStatisticsSearch: jest.fn(),
    };
    component = mount(<AgencyStatistics {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('does not render DoughnutCharts if agencyStatisticsSearchResults are not defined', () => {
    expect(component.find('DoughnutCharts').exists()).toBe(false);
  });

  it('IconButton calls reportsAgencyStatisticsSearch when clicked', () => {
    component.find('IconActionButton').simulate('click');
    expect(props.reportsAgencyStatisticsSearch).toHaveBeenCalledTimes(1);
  });
});
