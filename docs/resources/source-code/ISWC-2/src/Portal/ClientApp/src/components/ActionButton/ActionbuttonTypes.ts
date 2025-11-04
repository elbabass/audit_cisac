export interface IActionButtonProps {
  buttonText: string;
  isDisabled?: boolean;
  buttonAction?: () => void;
  theme?: 'primary' | 'secondary';
  submitType?: boolean;
}

export interface IActionButtonState {
  buttonStyle: string;
  buttonStyleIsDisabled?: boolean;
}

export interface IIconActionButtonProps {
  icon: string;
  buttonText?: string;
  isDisabled?: boolean;
  buttonAction?: () => void;
  theme?: 'primary' | 'secondary';
  bigIcon?: boolean;
}
