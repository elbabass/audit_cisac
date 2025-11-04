import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import AgencyWorkList from '../../../../routes/Reports/AgencyWorkList/AgencyWorkList';
import { IAgencyWorkListProps } from '../../../../routes/Reports/AgencyWorkList/AgencyWorkListTypes';
import Switch from '../../../../components/Switch/Switch';

describe('AgencyWorkList Page', () => {
  let props: IAgencyWorkListProps;
  let component: ReactWrapper;
  Switch.displayName = 'Switch';

  beforeEach(() => {
    props = {
      loading: false,
      extractToFtp: jest.fn(),
      agencyWorkListCachedVersion: '',
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
    component = mount(<AgencyWorkList {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('IconButton calls extractToFtp when clicked', () => {
    component.find('IconActionButton').simulate('click');
    expect(props.extractToFtp).toHaveBeenCalledTimes(1);
  });

  it('Switch is displayed if assignedRoles.reportIswcFullExtract is true', () => {
    expect(component.find('Switch').exists()).toBe(true);
  });

  it('Switch is hidden if assignedRoles.reportIswcFullExtract is false', () => {
    component = mount(
      <AgencyWorkList
        {...props}
        assignedRoles={{ ...props.assignedRoles, reportIswcFullExtract: false }}
      />,
    );
    expect(component.find('Switch').exists()).toBe(false);
  });
});
