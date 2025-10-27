import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { IModalProps } from '../../../components/Modal/ModalTypes';
import { Modal } from 'reactstrap';

describe('Modal Component', () => {
  let props: IModalProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      isModalOpen: true,
      toggleModal: jest.fn(),
      headerText: 'Header',
      bodyText: 'Body',
      leftButtonText: 'left button',
      leftButtonAction: jest.fn(),
      rightButtonText: 'right button',
      rightButtonAction: jest.fn(),
      type: 'confirm',
    };
    component = mount(<Modal {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });
});
