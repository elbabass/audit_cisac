import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { IHeaderProps } from '../../../components/Header/HeaderTypes';
import PublicPortalHeader from '../../../components/Header/PublicPortalHeader/PublicPortalHeader';

describe('PortalHeader Component', () => {
  let component: ReactWrapper;
  let props: IHeaderProps;

  beforeEach(() => {
    props = {
      renderHeaderItem: jest.fn(),
      isOpen: false,
      toggle: jest.fn(),
    };

    component = mount(<PublicPortalHeader {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders correct number of header items', () => {
    expect(props.renderHeaderItem).toHaveBeenCalledTimes(1);
  });
});
