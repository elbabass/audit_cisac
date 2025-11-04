import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import AssignedRolesGridActionCell from '../../../../routes/UserProfile/AssignedRolesGrid/AssignedRolesGridActionCell/AssignedRolesGridActionCell';
import { IAssignedRolesGridActionCellProps } from '../../../../routes/UserProfile/AssignedRolesGrid/AssignedRolesGridActionCell/AssignedRolesGridActionCellTypes';

describe('AssignedRolesGridActionCell Component', () => {
  let props: IAssignedRolesGridActionCellProps;
  let component: ReactWrapper;

  it('renders without crashing', () => {
    props = {
      topText: 'topText',
      topAction: jest.fn(),
      bottomText: 'bottomText',
      bottomAction: jest.fn(),
      smallText: true,
    };
    component = mount(<AssignedRolesGridActionCell {...props} />);
    expect(component.exists()).toBe(true);
  });

  it('topText props', () => {
    props = {
      topText: 'topText',
    };
    component = mount(<AssignedRolesGridActionCell {...props} />);
    const div = component.find('.greyText');
    expect(div.exists()).toBe(true);
    expect(div.text().includes(props.topText)).toBe(true);
  });

  it('topText and topAction props', () => {
    props = {
      topText: 'topText',
      topAction: jest.fn(),
    };
    component = mount(<AssignedRolesGridActionCell {...props} />);

    const div = component.find('.actionText');
    expect(div.exists()).toBe(true);
    expect(div.text().includes(props.topText)).toBe(true);
    div.simulate('click');
    expect(props.topAction).toHaveBeenCalledTimes(1);
  });

  it('topText, topAction, bottomText and bottomAction props', () => {
    props = {
      topText: 'topText',
      topAction: jest.fn(),
      bottomText: 'bottomText',
      bottomAction: jest.fn(),
    };
    component = mount(<AssignedRolesGridActionCell {...props} />);

    const divs = component.find('.actionText');
    const topDiv = divs.first();
    const bottomDiv = divs.last();

    expect(topDiv.exists()).toBe(true);
    expect(topDiv.text().includes(props.topText)).toBe(true);
    topDiv.simulate('click');
    expect(props.topAction).toHaveBeenCalledTimes(1);

    expect(bottomDiv.exists()).toBe(true);
    expect(bottomDiv.text().includes(props.bottomText ? props.bottomText : '')).toBe(true);
    bottomDiv.simulate('click');
    expect(props.bottomAction).toHaveBeenCalledTimes(1);
  });
});
