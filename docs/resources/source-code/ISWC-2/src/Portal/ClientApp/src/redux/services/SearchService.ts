import axios from 'axios';
import { ITitle, IInterestedParty, IBatchSearchIswc, IIswcModel } from '../types/IswcTypes';
import { parseInterestedParties, ensureResultIsArray } from './HelperMethods';
import { config } from '../../configuration/Configuration';

export const searchByIswc = (iswc?: string): Promise<IIswcModel[]> => {
  return new Promise<IIswcModel[]>((resolve, reject) => {
    axios({
      headers: { 'Cache-Control': 'no-cache', Pragma: 'no-cache' },
      method: 'get',
      url: config().iswcApiManagementUri + '/iswc/searchByIswc',
      params: {
        iswc: iswc,
      },
    })
      .then((res) => {
        let result = ensureResultIsArray(res.data);
        resolve(result);
      })
      .catch((err) => {
        reject(err);
      });
  });
};

export const searchByWorkCode = (agency?: string, workCode?: string): Promise<IIswcModel[]> => {
  return new Promise<IIswcModel[]>((resolve, reject) => {
    axios({
      headers: { 'Cache-Control': 'no-cache', Pragma: 'no-cache' },
      method: 'get',
      url: config().iswcApiManagementUri + '/iswc/searchByAgencyWorkCode',
      params: {
        agency,
        workCode,
        detailLevel: 0,
      },
    })
      .then((res) => {
        let result = ensureResultIsArray(res.data);
        resolve(result);
      })
      .catch((err) => {
        reject(err);
      });
  });
};

export const searchByTitleAndContributors = (
  title?: string,
  surnames?: string,
  nameNumbers?: string,
  baseNumbers?: string,
): Promise<IIswcModel[]> => {
  return new Promise<IIswcModel[]>((resolve, reject) => {
    let titleData = undefined;
    if (title && title?.length > 0) {
      titleData = [
        {
          title: title,
          type: 'OT',
        },
      ] as ITitle[];
    }

    axios({
      headers: {
        'Cache-Control': 'no-cache',
        Pragma: 'no-cache',
        'Content-Type': 'application/json',
      },
      method: 'post',
        url: config().iswcApiManagementUri + '/iswc/searchByTitleAndContributor',
      data: {
        titles: titleData,
        interestedParties: parseInterestedParties(
          surnames,
          nameNumbers,
          baseNumbers,
        ) as IInterestedParty[],
      },
    })
      .then((res) => {
        let result = ensureResultIsArray(res.data);
        resolve(result);
      })
      .catch((err) => {
        reject(err);
      });
  });
};

export const searchByIswcBatch = (iswcs: IBatchSearchIswc[]): Promise<IIswcModel[]> => {
  return new Promise<IIswcModel[]>((resolve, reject) => {
    axios({
      headers: { 'Cache-Control': 'no-cache', Pragma: 'no-cache' },
      method: 'post',
      url: config().iswcApiManagementUri + '/iswc/searchByIswc/batch',
      data: iswcs,
    })
      .then((res) => {
        let result = ensureResultIsArray(res.data);
        resolve(result);
      })
      .catch((err) => {
        reject(err);
      });
  });
};
