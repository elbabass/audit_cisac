import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import WorkflowsGrid from '../../../routes/Workflows/WorkflowsGrid/WorkflowsGrid';
import { IWorkflowsGridProps } from '../../../routes/Workflows/WorkflowsGrid/WorkflowGridTypes';
import { IStatusFiltersType } from '../../../routes/Workflows/WorkflowsTypes';
import GridCheckboxCell from '../../../components/GridComponents/GridCheckboxCell/GridCheckboxCell';

describe('WorkflowsGrid Component', () => {
  let props: IWorkflowsGridProps;
  let component: ReactWrapper;
  GridCheckboxCell.displayName = 'GridCheckboxCell';

  beforeEach(() => {
    props = {
      workflows: [],
      updateWorkflows: jest.fn(),
      showAssigned: false,
      updating: false,
      updateSuccessful: undefined,
      error: undefined,
      filters: {} as IStatusFiltersType,
      hideSelectButtons: false,
      search: jest.fn(),
    };
    component = mount(<WorkflowsGrid {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders grid', () => {
    expect(component.find('Grid').exists()).toBe(true);
  });

  it('shows select all header cell if hideSelectButtons is false', () => {
    expect(component.find('GridCheckboxCell').length).toEqual(1);
  });

  it('hides select all header cell if hideSelectButtons is true', () => {
    component = mount(<WorkflowsGrid {...props} hideSelectButtons={true} />);
    expect(component.find('GridCheckboxCell').length).toEqual(0);
  });

  it('shows select all buttons if hideSelectButtons is false', () => {
    expect(component.find('.selectedRowsDiv').length).toBeTruthy();
  });

  it('hides select all buttons if hideSelectButtons is true', () => {
    component = mount(<WorkflowsGrid {...props} hideSelectButtons={true} />);
    expect(component.find('.selectedRowsDiv').length).toBeFalsy();
  });
});
