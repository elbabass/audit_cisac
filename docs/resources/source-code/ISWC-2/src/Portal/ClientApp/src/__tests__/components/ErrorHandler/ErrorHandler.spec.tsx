import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import ErrorHandler from '../../../components/AlertMessage/ErrorHandler/ErrorHandler';
import { IErrorHandlerProps } from '../../../components/AlertMessage/ErrorHandler/ErrorHandlerTypes';

describe('ErrorHandler Component', () => {
  let props: IErrorHandlerProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      error: {
        response: {
          data: {
            message: 'Test Error',
          },
        },
      },
    };
    component = mount(<ErrorHandler {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });
});
