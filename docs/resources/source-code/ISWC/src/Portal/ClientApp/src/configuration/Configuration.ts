import axios from 'axios';

export interface ClientAppConfiguration {
  loginRedirectUri: string;
  iswcApiManagementUri: string;
  recaptchaPublicKey: string;
  applicationInsightsKey: string;
}

export interface PortalMessage {
  messageHeader: string;
  messageBody: string;
}

function getClientAppConfiguration(): ClientAppConfiguration {
  const config = localStorage.getItem('clientAppConfiguration');
  if (config) return JSON.parse(config);
  else return {} as ClientAppConfiguration;
}

export function loadClientAppConfiguration() {
  return new Promise<ClientAppConfiguration>((resolve, reject) => {
    axios({
      method: 'get',
      url: '/configuration/GetClientAppConfiguration',
    })
      .then((res) => {
        localStorage.setItem('clientAppConfiguration', JSON.stringify(res.data));
        resolve(res.data);
      })
      .catch((error) => {
        reject(error);
      });
  });
}

export function getPortalMessages() {
  let strings = localStorage.getItem('messages');
  if (strings != null) return JSON.parse(strings);
  else return {};
}

export function fetchPortalMessages(culture?: string) {
  return new Promise<any>((resolve, reject) => {
    axios({
      method: 'get',
      url: '/configuration/GetPortalMessages',
      headers: {
        'Content-Type': 'application/json',
      },
      params: {
        env: process.env.REACT_APP_MODE,
        culture,
      },
    })
      .then((res) => {
        localStorage.setItem('messages', JSON.stringify(res.data));
        resolve(res);
      })
      .catch((error) => {
        reject(error);
      });
  });
}

export { getClientAppConfiguration as config };
