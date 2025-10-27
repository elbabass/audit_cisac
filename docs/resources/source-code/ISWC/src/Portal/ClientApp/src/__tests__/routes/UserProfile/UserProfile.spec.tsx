import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import UserProfile from '../../../routes/UserProfile/UserProfile';
import { IUserProfileProps } from '../../../routes/UserProfile/UserProfileTypes';
import AssignedRolesGrid from '../../../routes/UserProfile/AssignedRolesGrid/AssignedRolesGrid';
import SubHeader from '../../../components/SubHeader/SubHeader';
import GoBack from '../../../components/GoBack/GoBack';
import { MemoryRouter } from 'react-router';

jest.mock('../../../consts', () => {
  return {
    ...jest.requireActual('../../../consts'),
    getAgencies: jest.fn(() => [{ name: 'abc', agencyId: '213' }]),
  };
});

describe('UserProfile Component', () => {
  let props: IUserProfileProps;
  let component: ReactWrapper;
  AssignedRolesGrid.displayName = 'AssignedRolesGrid';
  SubHeader.displayName = 'SubHeader';
  GoBack.displayName = 'GoBack';

  beforeEach(() => {
    props = {
      header: 'Header',
      user: { email: '', agencyId: '', webUserRoles: [{ role: 1 }] },
      roles: [1, 2, 4],
      loggedInUserProfileRoles: [{ role: 1, isApproved: true }],
    };
    component = mount(
      <MemoryRouter>
        <UserProfile {...props} />
      </MemoryRouter>,
    );
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders AssignedRolesGrid', () => {
    expect(component.find('AssignedRolesGrid').exists()).toBe(true);
  });

  it('renders SubHeader', () => {
    expect(component.find('SubHeader').exists()).toBe(true);
  });

  it('renders GoBack if manageMode is true', () => {
    component = mount(
      <MemoryRouter>
        <UserProfile {...props} manageMode />
      </MemoryRouter>,
    );
    expect(component.find('GoBack').exists()).toBe(true);
  });

  it('Modal is displayed when isModalOpen is true', () => {
    component.setState({
      isModalOpen: true,
    });
    expect(component.find('Modal').exists()).toBe(true);
  });
});
