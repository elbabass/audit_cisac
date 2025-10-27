import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { IViewMoreHeaderProps } from '../../../../routes/Search/ViewMore/ViewMoreTypes';
import { IIswcModel } from '../../../../redux/types/IswcTypes';
import ViewMoreHeader from '../../../../routes/Search/ViewMore/ViewMoreHeader';
import { MemoryRouter } from 'react-router-dom';
import GridIconCell from '../../../../components/GridComponents/GridIconCell/GridIconCell';

describe('ViewMore', () => {
  let props: IViewMoreHeaderProps;
  let component: ReactWrapper;
  GridIconCell.displayName = 'GridIconCell';

  beforeEach(() => {
    props = {
      assignedRoles: {
        search: true,
        update: true,
        reportBasics: true,
        reportExtracts: true,
        reportAgencyInterest: true,
        reportIswcFullExtract: true,
        manageRoles: false,
      },
      iswc: {} as IIswcModel,
      updateMergeList: jest.fn(),
    };
    component = mount(
      <MemoryRouter>
        <ViewMoreHeader {...props} />
      </MemoryRouter>,
    );
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders Update Submission if update is true', () => {
    const updateButton = component.findWhere(
      (n) => n.name() === 'GridIconCell' && n.prop('alt') === 'update icon',
    );
    expect(updateButton.length).toBe(1);
  });

  it('hides Update Submission if update is false', () => {
    component = mount(
      <MemoryRouter>
        <ViewMoreHeader
          {...props}
          assignedRoles={{
            search: true,
            update: false,
            reportBasics: true,
            reportExtracts: true,
            reportAgencyInterest: true,
            reportIswcFullExtract: true,
            manageRoles: false,
          }}
        />
      </MemoryRouter>,
    );

    const updateButton = component.findWhere(
      (n) => n.name() === 'GridIconCell' && n.prop('alt') === 'update icon',
    );
    expect(updateButton.length).toBe(0);
  });
});
