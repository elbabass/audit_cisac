import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import DisambiguateIswcs from '../../../routes/Submission/SubmissionAdditionalDetails/DisambiguateIswcs/DisambiguateIswcs';
import { IDisambiguateIswcsProps } from '../../../routes/Submission/SubmissionAdditionalDetails/DisambiguateIswcs/DisambiguateIswcsTypes';
import GridDropdownCell from '../../../components/GridComponents/GridDropdownCell/GridDropdownCell';

jest.mock('../../../consts', () => {
  return {
    ...jest.requireActual('../../../consts'),
    getDropdownLookupData: jest.fn(() => [{ name: 'abc', value: '213' }]),
  };
});

describe('DisambiguateIswcs Component', () => {
  let props: IDisambiguateIswcsProps;
  let component: ReactWrapper;
  GridDropdownCell.displayName = 'GridDropdownCells';

  beforeEach(() => {
    props = {
      updateSubmissionDataArray: jest.fn(),
      updateSubmissionDataString: jest.fn(),
      addElementToArray: jest.fn(),
      removeElementFromArray: jest.fn(),
      performers: [],
      disambiguationReason: '',
      disambiguationIswcs: [],
      bvltr: '',
      standardInstrumentation: '',
      potentialMatches: [],
      searchByIswc: jest.fn(),
    };
    component = mount(<DisambiguateIswcs {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('renders two Grids', () => {
    const grids = component.find('Grid');
    expect(grids.length).toBe(2);
  });

  it('renders three GridDropdownCells', () => {
    const grids = component.find('GridDropdownCells');
    expect(grids.length).toBe(3);
  });
});
