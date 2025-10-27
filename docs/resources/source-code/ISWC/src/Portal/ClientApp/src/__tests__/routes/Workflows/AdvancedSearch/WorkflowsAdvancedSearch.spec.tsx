import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import WorkflowsAdvancedSearch from '../../../../routes/Workflows/AdvancedSearch/WorkflowsAdvancedSearch';
import { IWorkflowsAdvancedSearchProps } from '../../../../routes/Workflows/AdvancedSearch/WorkflowsAdvancedSearchTypes';

jest.mock('../../../../consts', () => {
  return {
    ...jest.requireActual('../../../../consts'),
    getAgencies: jest.fn(() => [{ name: 'abc', agencyId: '213' }]),
  };
});

describe('WorkflowsAdvancedSearch Component', () => {
  let props: IWorkflowsAdvancedSearchProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      toggleAdvancedSearch: jest.fn(),
      showAdvancedSearch: true,
      searchFilters: {
        iswc: '',
        workCodes: '',
        agency: '',
        workflowType: '',
      },
      updateSearchFilters: jest.fn(),
    };
    component = mount(<WorkflowsAdvancedSearch {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });
});
