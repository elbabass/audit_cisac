import * as React from 'react';
import { shallow, ShallowWrapper } from 'enzyme';
import Header from '../../../components/Header/Header';
import PortalHeader from '../../../components/Header/PortalHeader/PortalHeader';

describe('Header Component', () => {
  let component: ShallowWrapper;
  PortalHeader.displayName = 'PortalHeader';
  beforeEach(() => {
    component = shallow(<Header />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders PortalHeader', () => {
    expect(component.find('PortalHeader').exists()).toBe(true);
  });
});
