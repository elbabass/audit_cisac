export interface IModalProps {
  isModalOpen: boolean;
  headerText: string;
  bodyText?: string;
  loading?: boolean;
  error?: any;
  leftButtonText?: string;
  rightButtonText: string;
  type: 'confirm' | 'input';
  onChangeInput?: (text: string) => void;
  toggleModal: () => void;
  rightButtonAction: () => void;
  leftButtonAction?: () => void;
  subHeaderText?: string;
}
