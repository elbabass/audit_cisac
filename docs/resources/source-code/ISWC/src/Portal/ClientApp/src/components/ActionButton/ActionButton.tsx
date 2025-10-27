import React, { PureComponent } from 'react';
import styles from './ActionButton.module.scss';
import { IActionButtonProps, IActionButtonState } from './ActionbuttonTypes';

export default class ActionButton extends PureComponent<IActionButtonProps, IActionButtonState> {
  constructor(props: IActionButtonProps) {
    super(props);

    this.state = {
      buttonStyle: props.theme === 'secondary' ? styles.secondaryActionButton : styles.actionButton,
      buttonStyleIsDisabled: false,
    };
  }

  componentDidMount() {
    const { isDisabled } = this.props;
    const { buttonStyleIsDisabled } = this.state;

    if (isDisabled && !buttonStyleIsDisabled) {
      this.setState({
        buttonStyle: styles.actionButtonDisabled,
        buttonStyleIsDisabled: true,
      });
    }
  }

  componentDidUpdate() {
    const { isDisabled, theme } = this.props;
    const { buttonStyleIsDisabled } = this.state;

    if (isDisabled && !buttonStyleIsDisabled) {
      this.setState({
        buttonStyle: styles.actionButtonDisabled,
        buttonStyleIsDisabled: true,
      });
    } else if (!isDisabled && buttonStyleIsDisabled) {
      this.setState({
        buttonStyle: theme === 'secondary' ? styles.secondaryActionButton : styles.actionButton,
        buttonStyleIsDisabled: false,
      });
    }
  }

  onClickButton = () => {
    const { buttonAction } = this.props;

    this.setState(
      {
        buttonStyle: `${styles.actionButton} ${styles.actionButtonClicked}`,
      },
      () => {
        buttonAction && buttonAction();
      },
    );
  };

  render() {
    const { buttonText, isDisabled, submitType } = this.props;
    const { buttonStyle } = this.state;

    return (
      <button
        onClick={this.onClickButton}
        className={buttonStyle}
        disabled={isDisabled}
        type={submitType ? 'submit' : 'button'}
      >
        {buttonText}
      </button>
    );
  }
}
