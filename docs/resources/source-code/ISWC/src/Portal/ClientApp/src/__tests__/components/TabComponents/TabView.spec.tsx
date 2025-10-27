import * as React from 'react';
import { shallow, ShallowWrapper } from 'enzyme';
import TabView from '../../../components/TabComponents/TabView/TabView';
import { ITabViewProps } from '../../../components/TabComponents/TabView/TabViewTypes';
import TabHeader from '../../../components/TabComponents/TabHeader/TabHeader';

describe('TabView Component', () => {
  let props: ITabViewProps;
  let component: ShallowWrapper;
  TabHeader.displayName = 'TabHeader';

  beforeEach(() => {
    props = {
      tabs: [
        { text: 'by ISWC', component: <div>by ISWC</div> },
        { text: 'by Agency Work Code', component: <div>by Agency Work Code</div> },
        { text: 'by Title', component: <div>by Title</div> },
      ],
    };
    component = shallow(<TabView {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders TabHeader component', () => {
    expect(component.find('TabHeader').exists()).toBe(true);
  });

  it('renders correct number of tab views', () => {
    const tabViews = component.find('.tabViewDiv');
    expect(tabViews.length).toEqual(props.tabs.length);
  });
});
