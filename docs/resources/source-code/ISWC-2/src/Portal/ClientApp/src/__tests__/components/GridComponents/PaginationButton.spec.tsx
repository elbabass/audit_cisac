import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import PaginationButton from '../../../components/GridComponents/Pagination/PaginationButton/PaginationButton';
import { IPaginationButtonProps } from '../../../components/GridComponents/Pagination/PaginationButton/PaginationButtonTypes';

describe('Pagination Component', () => {
  let props: IPaginationButtonProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      onClick: jest.fn(),
      text: '',
    };

    component = mount(<PaginationButton {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('calls onClick when pageNumberButton div is clicked', () => {
    component.find('.pageNumberButton').simulate('click');
    expect(props.onClick).toHaveBeenCalledTimes(1);
  });

  it('does not call onClick when pageNumberButton div is clicked when disabled is true', () => {
    component = mount(<PaginationButton {...props} disabled={true} />);
    component.find('.pageNumberButtonDisabled').simulate('click');
    expect(props.onClick).toHaveBeenCalledTimes(0);
  });
});
