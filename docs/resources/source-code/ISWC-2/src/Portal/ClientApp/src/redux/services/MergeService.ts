import axios from 'axios';
import { IMergeBody } from '../types/IswcTypes';
import { IDemergeIswc } from '../../routes/Search/Demerge/DemergeTypes';
import { config } from '../../configuration/Configuration';

export const mergeIswcs = (
  preferredIswc?: string,
  agency?: string,
  mergeBody?: IMergeBody,
): Promise<any> => {
  return new Promise<any>((resolve, reject) => {
    axios({
      headers: {
        'Cache-Control': 'no-cache',
        Pragma: 'no-cache',
        'Content-Type': 'application/json',
      },

      method: 'post',
      url: config().iswcApiManagementUri + '/iswc/merge',
      params: {
        preferredIswc: preferredIswc,
        agency: agency,
      },
      data: mergeBody,
    })
      .then((result) => {
        resolve(result);
      })
      .catch((err) => {
        reject(err);
      });
  });
};

export const demergeIswc = (
  preferredIswc: string,
  iscwsToDemerge: IDemergeIswc[],
): Promise<any> => {
  let promiseArr: Promise<any>[] = [];

  iscwsToDemerge.forEach((iswc) => {
    let p = new Promise<any>((resolve, reject) => {
      axios({
        method: 'delete',
          url: config().iswcApiManagementUri + '/iswc/merge',
        params: {
          preferredIswc: preferredIswc,
          agency: iswc.agency,
          workCode: iswc.workCode,
        },
      })
        .then((result) => {
          resolve(result);
        })
        .catch((err) => {
          reject(err);
        });
    });
    promiseArr.push(p);
  });
  return Promise.all(promiseArr);
};
