import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import SearchByTitleContributors from '../../../../routes/Search/SearchBy/SearchByTitleContributors';
import { ISearchByTitleProps } from '../../../../routes/Search/SearchBy/SearchByTypes';

jest.mock('../../../../consts', () => {
  return {
    ...jest.requireActual('../../../../consts'),
    getAgencies: jest.fn(() => [{ name: 'abc', agencyId: '213' }]),
  };
});
describe('SearchByWorkCode Component', () => {
  let props: ISearchByTitleProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      search: jest.fn(),
      onChange: jest.fn(),
      surnames: '',
      nameNumbers: '',
      baseNumbers: '',
    };
    component = mount(<SearchByTitleContributors {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders title input', () => {
    expect(component.find('#title').exists()).toBe(true);
  });

  it('renders surname input', () => {
    expect(component.find('#surnames').exists()).toBe(true);
  });

  it('renders name number input', () => {
    expect(component.find('#nameNumbers').exists()).toBe(true);
  });

  it('renders base number input input', () => {
    expect(component.find('#baseNumbers').exists()).toBe(true);
  });

  it('button calls search when clicked', () => {
    component.find('button').simulate('click');
    expect(props.search).toHaveBeenCalled();
  });
});
