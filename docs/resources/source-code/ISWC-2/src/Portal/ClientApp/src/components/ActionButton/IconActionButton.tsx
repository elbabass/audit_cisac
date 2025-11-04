import React, { PureComponent } from 'react';
import styles from './ActionButton.module.scss';
import { IActionButtonState, IIconActionButtonProps } from './ActionbuttonTypes';

export default class IconActionButton extends PureComponent<
  IIconActionButtonProps,
  IActionButtonState
> {
  constructor(props: IIconActionButtonProps) {
    super(props);

    const { theme, isDisabled } = this.props;
    this.state = {
      buttonStyle:
        theme === 'primary'
          ? isDisabled
            ? styles.actionButtonDisabled
            : styles.actionButton
          : isDisabled
          ? styles.secondaryActionButtonDisabled
          : styles.secondaryActionButton,
    };
  }

  onClickButton = () => {
    const { buttonAction, theme } = this.props;
    const { buttonStyle } = this.state;

    this.setState(
      {
        buttonStyle: `${buttonStyle} ${
          theme === 'primary' ? styles.actionButtonClicked : styles.secondaryActionButtonClicked
        }`,
      },
      () => {
        buttonAction && buttonAction();
      },
    );
  };

  render() {
    const { icon, buttonText, isDisabled, bigIcon } = this.props;
    const { buttonStyle } = this.state;

    return (
      <button onClick={this.onClickButton} className={buttonStyle} disabled={isDisabled}>
        <div className={styles.iconTextDiv}>
          <img src={icon} alt={'action icon'} className={bigIcon ? styles.bigIcon : styles.icon} />
          {buttonText}
        </div>
      </button>
    );
  }
}
