import axios from 'axios';
import { config } from '../../configuration/Configuration';

export const   getLookupData = (): Promise<any> => {
  return new Promise<any>((resolve, reject) => {
    axios({
      headers: {
        'Content-Type': 'application/json',
      },
      method: 'get',
      url: 'Lookup/GetLookupData',
    })
      .then((result) => {
        localStorage.setItem('lookupData', JSON.stringify(result.data));
        resolve(result);
      })
      .catch((err) => {
        if (err.response?.status === 503) {
          reject(err.response.status);
        } else {
          reject(err);
        }
      });
  });
};

export const checkApiStatus = (): Promise<any> => {
  return new Promise<any>((resolve, reject) => {
    axios({
      headers: {
        'Cache-Control': 'no-cache',
        Pragma: 'no-cache',
        'Content-Type': 'application/json',
      },
        method: 'get',
        url: config().iswcApiManagementUri,
    })
      .then((result) => {
        resolve(result);
      })
      .catch((err) => {
        if (err.response?.status === 503) {
          reject(err.response.status);
        } else {
          reject(err);
        }
      });
  });
};
