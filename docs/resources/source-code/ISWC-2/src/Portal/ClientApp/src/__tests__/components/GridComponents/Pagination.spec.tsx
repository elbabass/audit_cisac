import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { IPaginationProps } from '../../../components/GridComponents/Pagination/PaginationTypes';
import Pagination from '../../../components/GridComponents/Pagination/Pagination';
import PaginationButton from '../../../components/GridComponents/Pagination/PaginationButton/PaginationButton';

describe('Pagination Component', () => {
  let props: IPaginationProps;
  let component: ReactWrapper;
  PaginationButton.displayName = 'PaginationButton';

  beforeEach(() => {
    props = {
      gridRowCount: 2,
      numberOfPages: 2,
      currentPage: 1,
      currentPageValue: '1',
      rowsPerPage: 20,
      setCurrentPage: jest.fn(),
      changeRowsPerPage: jest.fn(),
    };

    component = mount(<Pagination {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('it calls changeRowsPerPage when dropdown is changed', () => {
    component.find('.resultsPerPageSelect').simulate('change', { target: { value: 2 } });
    expect(props.changeRowsPerPage).toHaveBeenCalledTimes(1);
  });

  it('it renders two PaginationButtons', () => {
    expect(component.find('PaginationButton').length).toBe(2);
  });
});
