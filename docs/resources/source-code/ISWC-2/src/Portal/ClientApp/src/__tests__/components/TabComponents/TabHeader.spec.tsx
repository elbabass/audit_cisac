import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { ITabHeaderProps } from '../../../components/TabComponents/TabHeader/TabHeaderTypes';
import TabHeader from '../../../components/TabComponents/TabHeader/TabHeader';
import TabButton from '../../../components/TabComponents/TabButton/TabButton';

describe('TabHeader Component', () => {
  let props: ITabHeaderProps;
  let component: ReactWrapper;
  TabButton.displayName = 'TabButton';

  beforeEach(() => {
    props = {
      tabs: [
        { text: 'Tab1', component: <div /> },
        { text: 'Tab2', component: <div /> },
      ],
      activeTab: 0,
      changeTab: jest.fn(),
    };
    component = mount(<TabHeader {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders correct number of TabButton components', () => {
    const buttons = component.find('TabButton');
    expect(buttons.length).toEqual(props.tabs.length);
  });
});
