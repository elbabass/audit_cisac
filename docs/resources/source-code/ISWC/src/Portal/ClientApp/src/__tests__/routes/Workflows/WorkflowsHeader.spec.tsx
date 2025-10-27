import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import WorkflowsHeader from '../../../routes/Workflows/WorkflowsHeader';
import { IWorkflowsHeaderProps } from '../../../routes/Workflows/WorkflowsTypes';

describe('WorkflowsGrid Component', () => {
  let props: IWorkflowsHeaderProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      statusFilters: {
        pending: true,
        approved: false,
        rejected: false,
      },
      showAssigned: true,
      changeWorkFlowsDisplayed: jest.fn(),
      updateStatusFilters: jest.fn(),
      loading: false,
    };
    component = mount(<WorkflowsHeader {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders 2 header items', () => {
    const headerItems = component.find('.headerItem');
    expect(headerItems.length).toEqual(2);
  });

  it('renders 1 status filters item', () => {
    expect(component.find('.statusFilter').length).toEqual(1);
  });

  it('renders 5 checkbox inputs', () => {
    expect(component.find('.labelText').length).toEqual(5);
  });

  it('renders 2 date inputs', () => {
    expect(component.find('DateInput').length).toEqual(2);
  });
});
