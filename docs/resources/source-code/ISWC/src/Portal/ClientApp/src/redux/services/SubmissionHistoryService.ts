import axios from 'axios';
import { IAuditHistoryResult } from '../types/IswcTypes';

export const getSubmissionHistory = (preferredIswc?: string): Promise<IAuditHistoryResult[]> => {
  return new Promise<IAuditHistoryResult[]>((resolve, reject) => {
    axios({
      headers: { 'Cache-Control': 'no-cache', Pragma: 'no-cache' },
      method: 'get',
      url: '/Audit/Search',
      params: {
        preferredIswc: preferredIswc,
      },
    })
      .then(res => {
        resolve(res.data);
      })
      .catch(err => {
        reject(err);
      });
  });
};
