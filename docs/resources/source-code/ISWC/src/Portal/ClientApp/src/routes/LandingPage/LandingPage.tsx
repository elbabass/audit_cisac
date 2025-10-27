import React, { PureComponent } from 'react';
import TermsAndConditions from './TermsAndConditions';
import LangugeSelection from './LangugeSelection';
import { ILandingPageState, ILandingPageProps } from './LandingPageTypes';
import styles from './LandingPage.module.scss';
import { LOGO_ICON } from '../../consts';

export default class LandingPage extends PureComponent<ILandingPageProps, ILandingPageState> {
  constructor(props: ILandingPageProps) {
    super(props);
    this.state = {
      showTermsAndConditions: true,
      error: false,
    };
  }

  _changeStep = () => {
    const { showTermsAndConditions } = this.state;
    this.setState(() => ({ showTermsAndConditions: !showTermsAndConditions }));
  };

  _getLanguage = (culture: string) => {
    const { setCulture } = this.props;
    sessionStorage.setItem('culture', culture);
    setCulture(culture);
  };

  renderLandingPage = () => {
    const { showTermsAndConditions } = this.state;
    if (showTermsAndConditions) return <TermsAndConditions changeStep={this._changeStep} />;
    else return <LangugeSelection selectLanguage={this._getLanguage} />;
  };

  render() {
    return (
      <div className={styles.background}>
        <div className={styles.logoContainer}>
          <img
            src={LOGO_ICON}
            className={styles.logo}
            alt={'ISWC Network logo'}
            width="250px"
            height="72px"
          />
        </div>
        {this.renderLandingPage()}
      </div>
    );
  }
}
