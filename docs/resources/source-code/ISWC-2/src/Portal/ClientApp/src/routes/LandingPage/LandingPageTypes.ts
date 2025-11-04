export interface ITermsAndConditionsProps {
  changeStep: () => void;
}
export interface ITermsAndConditionsState {
  captcha: any;
  captchaReady: boolean;
  captchaVerified: boolean;
  recaptchaPublicKey: string;
}

export interface ILandingPageState {
  showTermsAndConditions: boolean;
  error: any;
}

export interface ILandingPageProps {
  setCulture: (culture: string) => void;
}

export interface ILanguageSelectionProps {
  selectLanguage: (culture: string) => void;
}
