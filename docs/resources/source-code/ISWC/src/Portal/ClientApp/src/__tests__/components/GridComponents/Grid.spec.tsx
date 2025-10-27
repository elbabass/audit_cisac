import * as React from 'react';
import { shallow, ShallowWrapper } from 'enzyme';
import Grid from '../../../components/GridComponents/Grid/Grid';
import { IGridProps } from '../../../components/GridComponents/Grid/GridTypes';
import {
  MOCK_GRID_HEADERS,
  PREFERRED_ISWC_FIELD,
  ORIGINAL_TITLE_FIELD,
  CREATOR_NAMES_FIELD,
} from '../../../consts';
import GridHeaderCell from '../../../components/GridComponents/GridHeaderCell/GridHeaderCell';
import Pagination from '../../../components/GridComponents/Pagination/Pagination';

describe('Grid Component', () => {
  let props: IGridProps;
  let component: ShallowWrapper;
  GridHeaderCell.displayName = 'GridHeaderCell';
  Pagination.displayName = 'Pagination';

  beforeEach(() => {
    props = {
      headerCells: MOCK_GRID_HEADERS,
      gridRows: [
        {
          rowId: 0,
          cells: [
            { element: <div>T000000004</div>, field: PREFERRED_ISWC_FIELD },
            {
              element: <div>WHY DOES IT ALWAYS RAIN ON ME</div>,
              field: ORIGINAL_TITLE_FIELD,
            },
            {
              element: <div>John, Dylan, Clince</div>,
              field: CREATOR_NAMES_FIELD,
            },
          ],
        },
      ],
    };

    component = shallow(<Grid {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('grid renders correct amount of columns', () => {
    const cells = component.find('GridHeaderCell');
    expect(cells.length).toEqual(props.headerCells.length);
  });

  it('grid renders correct amount of rows', () => {
    const rows = component.find('GridRow');
    expect(rows.length).toEqual(props.gridRows.length);
  });

  it('displays Pagination if pagination prop is true', () => {
    component = shallow(<Grid {...props} pagination />);
    expect(component.find('Pagination').exists()).toBe(true);
  });
});
