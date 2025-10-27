import * as React from 'react';
import { shallow, ShallowWrapper } from 'enzyme';
import ManageUserRoles from '../../../routes/ManageUserRoles/ManageUserRoles';
import { IManageUserRolesProps } from '../../../routes/ManageUserRoles/ManageUserRolesTypes';

describe('ManageUserRoles Component', () => {
  let props: IManageUserRolesProps;
  let component: ShallowWrapper;

  beforeEach(() => {
    props = {
      users: [],
      usersWithPendingRequests: [],
      updateUserRole: jest.fn(),
      getAllUsers: jest.fn(),
      getUserProfileRoles: jest.fn(),
      getAccessRequests: jest.fn(),
    };
    component = shallow(<ManageUserRoles {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders TabView  if showUserProfile is false', () => {
    component.setState({
      showUserProfile: false,
    });
    expect(component.find('TabView').exists()).toBe(true);
  });

  it('renders UserProfile if showUserProfile is true', () => {
    component.setState({
      showUserProfile: true,
    });
    expect(component.find('UserProfile').exists()).toBe(true);
  });
});
