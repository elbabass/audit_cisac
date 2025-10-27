import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import PublisherIswcTracking from '../../../../routes/Reports/PublisherIswcTracking/PublisherIswcTracking';
import { IPublisherIswcTrackingProps } from '../../../../routes/Reports/PublisherIswcTracking/PublisherIswcTrackingTypes';

jest.mock('../../../../consts', () => {
  return {
    ...jest.requireActual('../../../../consts'),
    getAgencies: jest.fn(() => [{ name: 'abc', agencyId: '213' }]),
  };
});

describe('PublisherIswcTracking Page', () => {
  let props: IPublisherIswcTrackingProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      loading: false,
      extractToFtp: jest.fn(),
      email: '',
      extractToFtpSuccess: false,
    };
    component = mount(<PublisherIswcTracking {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('IconButton calls extractToFtp when clicked', () => {
    component.find('IconActionButton').simulate('click');
    expect(props.extractToFtp).toHaveBeenCalledTimes(1);
  });
});
