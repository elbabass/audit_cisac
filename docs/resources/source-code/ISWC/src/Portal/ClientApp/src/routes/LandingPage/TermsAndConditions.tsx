import axios from 'axios';
import React, { PureComponent } from 'react';
import Reaptcha from 'reaptcha';
import Cookies from 'universal-cookie';
import ActionButton from '../../components/ActionButton/ActionButton';
import { getStrings } from '../../configuration/Localization';
import { _parseJwt } from '../../shared/helperMethods';
import styles from './LandingPage.module.scss';
import { ITermsAndConditionsProps, ITermsAndConditionsState } from './LandingPageTypes';
import { validateReCaptchaResponse } from './ReCaptchaValidationService';
import { config } from '../../configuration/Configuration';

export default class TermsAndConditions extends PureComponent<
  ITermsAndConditionsProps,
  ITermsAndConditionsState
> {
  constructor(props: ITermsAndConditionsProps) {
    super(props);
    this.state = {
      captcha: null,
      captchaReady: false,
      captchaVerified: false,
      recaptchaPublicKey: config().recaptchaPublicKey || ' ',
    };
  }

  verifyCallback = (recaptchaToken: any) => {
    const cookies = new Cookies();

    validateReCaptchaResponse(recaptchaToken)
      .then((data: any) => {
        const tokenExpirationDate = new Date(_parseJwt(data.token_id)['exp'] * 1000);
        cookies.set('authToken', 'Bearer ' + data.token_id, { expires: tokenExpirationDate });
        axios.defaults.headers.common['Authorization'] = 'Bearer ' + data.token_id;
        cookies.set('recaptchaToken', recaptchaToken, { sameSite: true });
        this.setState({ captchaVerified: data.success });
      })
      .catch((err: any) => {
        console.log(err);
        this.setState({ captchaVerified: false });
      });
  };

  checkIfVerified() {
    return this.state.captchaVerified ? false : true;
  }

  render() {
    const { TERMS_AND_CONDITIONS_TEXT, AGREE_TO_TERMS } = getStrings();
    const { changeStep } = this.props;
    const { recaptchaPublicKey } = this.state;
    return (
      <div className={styles.background}>
        <div className={styles.contentContainer}>
          <div className={styles.textContainer} style={{ whiteSpace: 'pre-line' }}>
            {TERMS_AND_CONDITIONS_TEXT}
          </div>
          <div className={styles.captchaContainer}>
            <Reaptcha
              ref={(element: any) => {
                this.setState({ captcha: element });
              }}
              size="normal"
              data-theme="dark"
              sitekey={recaptchaPublicKey}
              onVerify={this.verifyCallback}
              onExpire={() => this.setState({ captchaVerified: false })}
            />
          </div>
        </div>
        <div className={styles.buttonContainer}>
          <div className={styles.button}>
            <ActionButton
              buttonText={AGREE_TO_TERMS}
              isDisabled={this.checkIfVerified()}
              buttonAction={changeStep}
            />
          </div>
        </div>
      </div>
    );
  }
}
