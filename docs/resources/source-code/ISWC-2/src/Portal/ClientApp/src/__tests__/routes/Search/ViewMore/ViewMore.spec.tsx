import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import ViewMore from '../../../../routes/Search/ViewMore/ViewMore';
import { IViewMoreProps } from '../../../../routes/Search/ViewMore/ViewMoreTypes';
import { IIswcModel } from '../../../../redux/types/IswcTypes';
import { MemoryRouter } from 'react-router-dom';

describe('ViewMore', () => {
  let props: IViewMoreProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      iswcModel: { interestedParties: [{}] } as IIswcModel,
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
    component = mount(
      <MemoryRouter>
        <ViewMore {...props} />
      </MemoryRouter>,
    );
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders ViewMoreHeader', () => {
    expect(component.find('ViewMoreHeader')).toBeTruthy();
  });
});
