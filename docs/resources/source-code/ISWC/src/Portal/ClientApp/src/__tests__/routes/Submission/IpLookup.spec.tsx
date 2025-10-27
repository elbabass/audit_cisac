import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { IIpLookupProps } from '../../../routes/Submission/IpLookup/IpLookupTypes';
import IpLookup from '../../../routes/Submission/IpLookup/IpLookup';

describe('IpLookup Component', () => {
  let props: IIpLookupProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      close: jest.fn(),
      ipType: 1,
      addElementToSubmissionDataArray: jest.fn(),
      rowId: 0,
    };
    component = mount(<IpLookup {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('clicking close icon calls close() prop', () => {
    component.find('.closeButton').simulate('click');
    expect(props.close).toHaveBeenCalledTimes(1);
  });

  it('clicking cancel button calls close() prop', () => {
    const buttons = component.find('ActionButton');
    buttons.first().simulate('click');
    expect(props.close).toHaveBeenCalledTimes(1);
  });
});
