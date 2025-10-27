import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { IAssignedRolesGridProps } from '../../../../routes/UserProfile/AssignedRolesGrid/AssignedRolesGridTypes';
import AssignedRolesGrid from '../../../../routes/UserProfile/AssignedRolesGrid/AssignedRolesGrid';
import GridIconCell from '../../../../components/GridComponents/GridIconCell/GridIconCell';

describe('AssignedRolesGrid Component', () => {
  let props: IAssignedRolesGridProps;
  let component: ReactWrapper;
  GridIconCell.displayName = 'GridIconCell';

  beforeEach(() => {
    props = {
      roles: [1, 2, 4],
      user: { email: '', agencyId: '', webUserRoles: [{ role: 1, isApproved: true }] },
    };
    component = mount(<AssignedRolesGrid {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders correct amount of rows', () => {
    const rows = component.find('tr');
    expect(rows.length).toEqual(props.roles.length);
  });

  it('renders correct amount of GridIconCells', () => {
    const rows = component.find('GridIconCell');
    expect(rows.length).toEqual(1);
  });
});
