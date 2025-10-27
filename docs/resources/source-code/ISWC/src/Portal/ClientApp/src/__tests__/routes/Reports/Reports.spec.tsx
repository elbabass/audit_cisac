import * as React from 'react';
import { shallow, ShallowWrapper } from 'enzyme';
import { IReportsProps } from '../../../routes/Reports/ReportsTypes';
import Reports from '../../../routes/Reports/Reports';

jest.mock('../../../consts', () => {
  return {
    ...jest.requireActual('../../../consts'),
    getAgencies: jest.fn(() => [{ name: 'abc', agencyId: '213' }]),
  };
});

describe('Reports Page', () => {
  let props: IReportsProps;
  let component: ShallowWrapper;

  beforeEach(() => {
    props = {
      clearReportsError: jest.fn(),
      assignedRoles: {
        search: true,
        update: true,
        reportBasics: true,
        reportExtracts: true,
        reportAgencyInterest: true,
        reportIswcFullExtract: true,
        manageRoles: false,
      },
    };
    component = shallow(<Reports {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders TabsView', () => {
    expect(component.find('TabView').exists()).toBe(true);
  });
});
