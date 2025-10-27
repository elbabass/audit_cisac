export interface IAlertMessageProps {
  message: string;
  type: 'error' | 'success' | 'info' | 'warn';
  subMessage?: string;
  close?: () => void;
  viewMore?: string;
}

export interface IAlertMessageState {
  showSubMessage: boolean;
}
