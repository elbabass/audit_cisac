import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import SearchByWorkCode from '../../../../routes/Search/SearchBy/SearchByWorkCode';
import { ISearchByProps } from '../../../../routes/Search/SearchBy/SearchByTypes';

jest.mock('../../../../consts', () => {
  return {
    ...jest.requireActual('../../../../consts'),
    getAgencies: jest.fn(() => [{ name: 'abc', agencyId: '213' }]),
  };
});

describe('SearchByWorkCode Component', () => {
  let props: ISearchByProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      search: jest.fn(),
      onChange: jest.fn(),
    };
    component = mount(<SearchByWorkCode {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders work code input', () => {
    expect(component.find('#workCode').exists()).toBe(true);
  });

  it('renders agency input', () => {
    expect(component.find('#agency').exists()).toBe(true);
  });

  it('renders database input', () => {
    expect(component.find('#sourceDb').exists()).toBe(true);
  });

  it('button calls search when clicked', () => {
    component.find('button').simulate('click');
    expect(props.search).toHaveBeenCalled();
  });
});
