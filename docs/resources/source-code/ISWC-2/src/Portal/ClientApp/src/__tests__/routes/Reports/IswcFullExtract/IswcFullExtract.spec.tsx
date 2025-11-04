import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import IswcFullExtract from '../../../../routes/Reports/IswcFullExtract/IswcFullExtract';
import { IIswcFullExtractProps } from '../../../../routes/Reports/IswcFullExtract/IswcFullExtractTypes';
import Switch from '../../../../components/Switch/Switch';

describe('IswcFullExtract Page', () => {
  let props: IIswcFullExtractProps;
  let component: ReactWrapper;
  Switch.displayName = 'Switch';

  beforeEach(() => {
    props = {
      loading: false,
      extractToFtp: jest.fn(),
      fullExtractCachedVersion: '',
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
    component = mount(<IswcFullExtract {...props} />);
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
    expect(component.find('Switch').exists()).toBe(true);
  });

  it('Switch is hidden if assignedRoles.reportIswcFullExtract is false', () => {
    component = mount(
      <IswcFullExtract
        {...props}
        assignedRoles={{ ...props.assignedRoles, reportIswcFullExtract: false }}
      />,
    );
    expect(component.find('Switch').exists()).toBe(false);
  });
});
