import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import AgencyInterestExtract from '../../../../routes/Reports/AgencyInterestExtract/AgencyInterestExtract';
import { IAgencyInterestExtractProps } from '../../../../routes/Reports/AgencyInterestExtract/AgencyInterestExtractTypes';

jest.mock('../../../../consts', () => {
  return {
    ...jest.requireActual('../../../../consts'),
    getAgencies: jest.fn(() => [{ name: 'abc', agencyId: '213' }]),
  };
});

describe('AgencyInterestExtract Page', () => {
  let props: IAgencyInterestExtractProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      extractToFtp: jest.fn(),
      loading: false,
      email: 'email',
      extractToFtpSuccess: false,
    };
    component = mount(<AgencyInterestExtract {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('IconButton calls extractToFtp when clicked', () => {
    component.find('IconActionButton').simulate('click');
    expect(props.extractToFtp).toHaveBeenCalledTimes(1);
  });
});
