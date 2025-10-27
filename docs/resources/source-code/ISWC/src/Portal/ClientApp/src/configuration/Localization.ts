import axios from 'axios';
import { normalizeArray } from '../redux/services/HelperMethods';

export interface stringsObj {
  [key: string]: string;
}

export function getStrings(): stringsObj {
  let strings = localStorage.getItem('strings');
  if (strings != null) return JSON.parse(strings);
  else return {};
}

export function fetchStringsFromDb(culture?: string) {
  return new Promise<any>((resolve, reject) => {
    axios({
      method: 'get',
      url: '/configuration/GetLocalizedStrings',
      params: { culture },
    })
      .then((res) => {
        let result = normalizeArray(res.data, 'name', 'value');
        localStorage.setItem('strings', JSON.stringify(result));
        resolve(result);
      })
      .catch((error) => {
        reject(error);
      });
  });
}
