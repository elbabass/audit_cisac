import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import AccessRequests from '../../../routes/ManageUserRoles/AccessRequests/AccessRequests';
import { IAccessRequestsProps } from '../../../routes/ManageUserRoles/AccessRequests/AccessRequestsTypes';
import Loader from '../../../components/Loader/Loader';
import AlertMessage from '../../../components/AlertMessage/AlertMessage';

describe('AccessRequests Component', () => {
  let props: IAccessRequestsProps;
  let component: ReactWrapper;
  Loader.displayName = 'Loader';

  beforeEach(() => {
    props = {
      manageRolesAction: jest.fn(),
      getAccessRequests: jest.fn(),
      usersWithPendingRequests: [],
    };
    component = mount(<AccessRequests {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders Grid', () => {
    expect(component.find('Grid').exists()).toBe(true);
  });

  it('renders Loader if loading is true', () => {
    component = mount(<AccessRequests {...props} loading />);
    expect(component.find('Loader').exists()).toBe(true);
  });

  it('renders AlertMessage if error exists', () => {
    component = mount(<AccessRequests {...props} error />);
    expect(component.find('AlertMessage').exists()).toBe(true);
  });

  it('getAccessRequests called once', () => {
    expect(props.getAccessRequests).toHaveBeenCalledTimes(1);
  });
});
