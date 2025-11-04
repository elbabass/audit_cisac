import axios from 'axios';
import { getLoggedInAgencyId } from '../../shared/helperMethods';
import {
  IReportsParamFields,
  IAgencyStatisticsParamFields,
} from '../../routes/Reports/ReportsTypes';
import { PER_MONTH_PERIOD } from '../../consts';

export const reportsSearch = (params: IReportsParamFields, url: string): Promise<any> => {
  return new Promise<any>((resolve, reject) => {
    axios({
      headers: { 'Cache-Control': 'no-cache', Pragma: 'no-cache' },
      method: 'post',
      url: url,
      data: {
        FromDate: params.fromDate,
        ToDate: params.toDate,
        Status: params.status,
        AgencyWorkCode: params.agencyWorkCode?.trim(),
        AgencyName: params.agencyName,
        TransactionSource: params.transactionSource,
        Report: params.reportType,
        MostRecentVersion: params.mostRecentVersion,
        SubmittingAgency: getLoggedInAgencyId(),
        AgreementFromDate: params.agreementFromDate,
        AgreementToDate: params.agreementToDate,
        Email: params.email,
        ConsiderOriginalTitlesOnly: params.considerOriginalTitlesOnly,
        CreatorNameNumber: params.creatorNameNumber,
        CreatorBaseNumber: params.creatorBaseNumber,
      },
    })
      .then((result) => resolve(result))
      .catch((error) => reject(error));
  });
};

export const reportsAgencyStatisticsSearch = (
  params: IAgencyStatisticsParamFields,
  url: string,
): Promise<any> => {
  return new Promise<any>((resolve, reject) => {
    axios({
      headers: { 'Cache-Control': 'no-cache', Pragma: 'no-cache' },
      method: 'post',
      url: url,
      data: {
        Year: params.year,
        Month: params.timePeriod === PER_MONTH_PERIOD ? params.month : undefined,
        TimePeriod: params.timePeriod,
        AgencyName: params.agencyName,
        TransactionSource: params.transactionSource,
      },
    })
      .then((result) => resolve(result))
      .catch((error) => reject(error));
  });
};

export const getDateOfCachedReport = (folder: string) => {
  return new Promise<any>((resolve, reject) => {
    axios({
      headers: { 'Cache-Control': 'no-cache', Pragma: 'no-cache' },
      method: 'get',
      url: 'Report/GetDateOfCachedReport',
      params: { folder: folder },
    })
      .then((result) => resolve(result))
      .catch((error) => reject(error));
  });
};
