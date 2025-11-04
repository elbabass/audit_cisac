import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import SearchByIswc from '../../../../routes/Search/SearchBy/SearchByISWC';
import { ISearchByProps } from '../../../../routes/Search/SearchBy/SearchByTypes';

describe('SearchByIswc Component', () => {
  let props: ISearchByProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      search: jest.fn(),
      onChange: jest.fn(),
    };
    component = mount(<SearchByIswc {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders iswc input', () => {
    expect(component.find('input').prop('id')).toBe('iswc');
  });

  it('button calls search when clicked', () => {
    component.find('button').simulate('click');
    expect(props.search).toHaveBeenCalled();
  });
});
