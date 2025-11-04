import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import {
  VIEW_MORE_ICON,
  PREFERRED_ISWC_FIELD,
  ORIGINAL_TITLE_FIELD,
  CREATOR_NAMES_FIELD,
  VIEW_MORE_FIELD,
  VIEW_MORE_ACTION,
} from '../../../consts';
import GridRow from '../../../components/GridComponents/GridRow/GridRow';
import { IGridRowProps } from '../../../components/GridComponents/GridRow/GridRowTypes';
import GridTextCell from '../../../components/GridComponents/GridTextCell/GridTextCell';
import GridIconCell from '../../../components/GridComponents/GridIconCell/GridIconCell';

describe('GridRow Component', () => {
  let props: IGridRowProps;
  let component: ReactWrapper;
  GridIconCell.displayName = 'GridIconCell';

  beforeEach(() => {
    props = {
      gridRow: {
        rowId: 0,
        cells: [
          { element: <GridTextCell text={'T000000000'} />, field: PREFERRED_ISWC_FIELD },
          {
            element: <GridTextCell text={'WHY DOES IT ALWAYS RAIN ON ME'} />,
            field: ORIGINAL_TITLE_FIELD,
          },
          {
            element: <GridTextCell text={'John, Dylan, Clince'} />,
            field: CREATOR_NAMES_FIELD,
          },
          {
            element: (
              <GridIconCell text={'View More'} icon={VIEW_MORE_ICON} alt={'View More Icon'} />
            ),
            field: VIEW_MORE_FIELD,
            action: VIEW_MORE_ACTION,
          },
        ],
        viewMore: [<div>View More</div>],
      },
      getColumnHeader: jest.fn(),
      numColumns: 4,
    };

    component = mount(<GridRow {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('grid renders correct amount of columns', () => {
    const rowCells = component.find('td');
    expect(rowCells.length).toEqual(props.gridRow.cells.length);
  });

  it('view more div not is displayed when displayViewMoreDiv is false', () => {
    expect(component.find('.viewMoreDiv').exists()).toBe(false);
  });

  it('view more div is displayed when displayViewMoreDiv is true', () => {
    component.setState({
      displayViewMoreDiv: true,
    });
    expect(component.find('.viewMoreDiv').exists()).toBe(true);
  });
});
