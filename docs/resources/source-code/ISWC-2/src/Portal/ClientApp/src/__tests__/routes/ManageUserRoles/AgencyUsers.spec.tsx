import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import AgencyUsers from '../../../routes/ManageUserRoles/AgencyUsers/AgencyUsers';
import { IAgencyUsersProps } from '../../../routes/ManageUserRoles/AgencyUsers/AgencyUsersTypes';
import AlertMessage from '../../../components/AlertMessage/AlertMessage';
import Loader from '../../../components/Loader/Loader';

describe('AgencyUsers Component', () => {
  let props: IAgencyUsersProps;
  let component: ReactWrapper;
  Loader.displayName = 'Loader';

  beforeEach(() => {
    props = { users: [], getAllUsers: jest.fn(), manageRolesAction: jest.fn() };
    component = mount(<AgencyUsers {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders Grid', () => {
    expect(component.find('Grid').exists()).toBe(true);
  });

  it('renders Loader if loading is true', () => {
    component = mount(<AgencyUsers {...props} loading />);
    expect(component.find('Loader').exists()).toBe(true);
  });

  it('renders AlertMessage if error exists', () => {
    component = mount(<AgencyUsers {...props} error />);
    expect(component.find('AlertMessage').exists()).toBe(true);
  });

  it('getAccessRequests called once', () => {
    expect(props.getAllUsers).toHaveBeenCalledTimes(1);
  });
});
