import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import PortalHeader from '../../../components/Header/PortalHeader/PortalHeader';
import { IHeaderProps } from '../../../components/Header/HeaderTypes';
import { MemoryRouter } from 'react-router-dom';

describe('PortalHeader Component', () => {
  let component: ReactWrapper;
  let props: IHeaderProps;

  beforeEach(() => {
    props = {
      renderHeaderItem: jest.fn(),
      isOpen: false,
      showSettings: false,
      toggle: jest.fn(),
      toggleSettings: jest.fn(),
      logout: jest.fn(),
      assignedRoles: {
        search: true,
        update: true,
        reportBasics: true,
        reportExtracts: true,
        reportAgencyInterest: true,
        reportIswcFullExtract: true,
        manageRoles: false,
      },
    };

    component = mount(<PortalHeader {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders correct number of header items', () => {
    expect(props.renderHeaderItem).toHaveBeenCalledTimes(5);
  });

  it('renders settingsDropdown if showSettings is true', () => {
    component = mount(
      <MemoryRouter>
        <PortalHeader {...props} showSettings={true} />
      </MemoryRouter>,
    );
    expect(component.find('.settingsDropdown').exists()).toBe(true);
  });

  it('hides header item if assignedRole is false', () => {
    props = {
      renderHeaderItem: jest.fn(),
      isOpen: false,
      showSettings: false,
      toggle: jest.fn(),
      toggleSettings: jest.fn(),
      logout: jest.fn(),
      assignedRoles: {
        search: true,
        update: false,
        reportBasics: false,
        reportExtracts: false,
        reportAgencyInterest: false,
        reportIswcFullExtract: false,
        manageRoles: false,
      },
    };

    component = mount(
      <MemoryRouter>
        <PortalHeader {...props} />
      </MemoryRouter>,
    );
    expect(props.renderHeaderItem).toHaveBeenCalledTimes(2);
  });
});
