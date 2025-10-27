import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import Switch from '../../../../components/Switch/Switch';
import { IPotentialDuplicatesProps } from '../../../../routes/Reports/PotentialDuplicates/PotentialDuplicatesTypes';
import PotentialDuplicates from '../../../../routes/Reports/PotentialDuplicates/PotentialDuplicates';

jest.mock('../../../../consts', () => {
  return {
    ...jest.requireActual('../../../../consts'),
    getAgencies: jest.fn(() => [{ name: 'abc', agencyId: '213' }]),
  };
});

describe('PotentialDuplicates Page', () => {
  let props: IPotentialDuplicatesProps;
  let component: ReactWrapper;
  Switch.displayName = 'Switch';

  beforeEach(() => {
    props = {
      loading: false,
      extractToFtp: jest.fn(),
      potentialDuplicatesCachedVersion: '',
      getDateOfCachedReport: jest.fn(),
      email: 'string',
      extractToFtpSuccess: false,
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
    component = mount(<PotentialDuplicates {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('IconButton calls extractToFtp when clicked', () => {
    component.find('IconActionButton').simulate('click');
    expect(props.extractToFtp).toHaveBeenCalledTimes(1);
  });

  it('IconButton calls extractToFtp when clicked', () => {
    component.find('IconActionButton').simulate('click');
    expect(props.extractToFtp).toHaveBeenCalledTimes(1);
  });

  it('Switch is displayed if assignedRoles.reportIswcFullExtract is true', () => {
    expect(component.find('Switch').length).toBe(2);
  });

  it('Switch is hidden if assignedRoles.reportIswcFullExtract is false', () => {
    component = mount(
      <PotentialDuplicates
        {...props}
        assignedRoles={{ ...props.assignedRoles, reportIswcFullExtract: false }}
      />,
    );
    expect(component.find('Switch').length).toBe(1);
  });
});
