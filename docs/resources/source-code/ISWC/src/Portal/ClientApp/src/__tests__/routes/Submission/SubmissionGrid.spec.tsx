import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { TYPE_FIELD, TITLE_FIELD } from '../../../consts';
import GridTextCell from '../../../components/GridComponents/GridTextCell/GridTextCell';
import GridInputCell from '../../../components/GridComponents/GridInputCell/GridInputCell';
import GridDropdownCell from '../../../components/GridComponents/GridDropdownCell/GridDropdownCell';
import { IGridRow } from '../../../components/GridComponents/Grid/GridTypes';
import { ISubmissionGridProps } from '../../../routes/Submission/SubmissionGrid/SubmissionGridTypes';
import SubmissionGrid from '../../../routes/Submission/SubmissionGrid/SubmissionGrid';

describe('SubmissionGrid Component', () => {
  let props: ISubmissionGridProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      headerCells: [
        { text: 'Type', field: TYPE_FIELD },
        { text: 'Title', field: TITLE_FIELD },
      ],
      gridRows: [
        {
          rowId: 0,
          cells: [
            { element: <GridTextCell text={'Original Title'} />, field: TYPE_FIELD },
            { element: <GridInputCell value={''} onChange={jest.fn()} />, field: TITLE_FIELD },
          ],
        },
        {
          rowId: 1,
          cells: [
            {
              element: (
                <GridDropdownCell
                  options={[
                    { name: 'A', value: 'A' },
                    { name: 'B', value: 'B' },
                    { name: 'C', value: 'C' },
                  ]}
                  value={'A'}
                  selectNewOption={() => null}
                />
              ),
              field: TYPE_FIELD,
            },
            { element: <GridInputCell value={''} onChange={jest.fn()} />, field: TITLE_FIELD },
          ],
        },
      ],
      actionButtonText: 'Add New Title',
      addRowToGrid: jest.fn(),
      showActionButton: true,
    };

    component = mount(<SubmissionGrid {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders Grid', () => {
    expect(component.find('Grid').exists()).toBe(true);
  });

  it('renders IconActionButton if showActionButton is true', () => {
    expect(component.find('IconActionButton').exists()).toBe(true);
  });

  it('does not render IconActionButton if showActionButton is false', () => {
    component = mount(<SubmissionGrid {...props} showActionButton={false} />);
    expect(component.find('IconActionButton').exists()).toBe(false);
  });
});
