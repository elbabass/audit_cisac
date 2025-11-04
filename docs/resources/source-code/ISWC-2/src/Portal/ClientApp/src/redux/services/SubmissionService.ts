import axios from 'axios';
import {
  IAgencyWorkCodesSubmissionRow,
  ITitleSubmissionRow,
  ICreatorPublisherSubmissionRow,
  IDerivedFromWorksSubmissionRow,
  ISubmissionResponse,
  IPerformerSubmissionRow,
  IDisambiguateFromSubmissionRow,
} from '../../routes/Submission/SubmissionTypes';
import { config } from '../../configuration/Configuration';
import { SUBMISSION_PATH } from '../../consts';

export const newSubmission = (
  previewDisambiguation: boolean,
  agencyWorkCode: IAgencyWorkCodesSubmissionRow[],
  titles: ITitleSubmissionRow[],
  creators: ICreatorPublisherSubmissionRow[],
  disambiguation: boolean,
  preferredIswc?: string,
  publishers?: ICreatorPublisherSubmissionRow[],
  derivedWorkType?: string,
  derivedFromWorks?: IDerivedFromWorksSubmissionRow[],
  disambiguationIswcs?: string[],
  disambiguationReason?: string,
  bvltr?: string,
  performers?: IPerformerSubmissionRow[],
  standardInstrumentation?: string,
): Promise<ISubmissionResponse> => {
  return new Promise<ISubmissionResponse>((resolve, reject) => {
    const originalTitle = titles.filter((x) => {
      return x.type === 'OT';
    });
    const otherTitles = titles.filter((x) => {
      return x.type !== 'OT';
    });
    const disambiguateFrom: IDisambiguateFromSubmissionRow[] = [];
    disambiguationIswcs?.forEach((dis) => {
      disambiguateFrom.push({ iswc: dis });
    });
    derivedFromWorks?.forEach((derivedFromWork) => {
      if (derivedFromWork.iswc && derivedFromWork.iswc.length > 0) {
        derivedFromWork.iswc = derivedFromWork.iswc.replace(/[^T0-9]/g, '');
      } else {
        derivedFromWork.iswc = undefined;
      }
      if (derivedFromWork.title?.length === 0) {
        derivedFromWork.title = undefined;
      }
    });
    const otTitle = originalTitle[0];

    axios({
      headers: { 'Cache-Control': 'no-cache', Pragma: 'no-cache' },
        method: 'post',
        url: config().iswcApiManagementUri + SUBMISSION_PATH,
      data: {
        preferredIswc: preferredIswc || undefined,
        agency: agencyWorkCode[0].agencyName,
        sourcedb: Number(agencyWorkCode[0].agencyName),
        workcode: agencyWorkCode[0].agencyWorkCode,
        category: 'DOM',
        disambiguation,
        disambiguationReason: disambiguationReason || undefined,
        disambiguateFrom,
        bvltr: bvltr || undefined,
        derivedWorkType: derivedWorkType || undefined,
        derivedFromIswcs: derivedFromWorks,
        performers,
        instrumentation: standardInstrumentation && [{ code: standardInstrumentation }],
        originalTitle: otTitle?.title || '',
        otherTitles,
        interestedParties: creators.concat(publishers || []),
        previewDisambiguation,
      },
    })
      .then((res) => {
        resolve(res.data);
      })
      .catch((err) => {
        reject(err);
      });
  });
};

export const updateSubmission = (
  previewDisambiguation: boolean,
  agencyWorkCode: IAgencyWorkCodesSubmissionRow[],
  titles: ITitleSubmissionRow[],
  creators: ICreatorPublisherSubmissionRow[],
  disambiguation: boolean,
  disambiguationIswcs?: string[],
  preferredIswc?: string,
  publishers?: ICreatorPublisherSubmissionRow[],
  derivedWorkType?: string,
  derivedFromWorks?: IDerivedFromWorksSubmissionRow[],
  performers?: IPerformerSubmissionRow[],
): Promise<ISubmissionResponse> => {
  return new Promise<ISubmissionResponse>((resolve, reject) => {
    const originalTitle = titles.filter((x) => {
      return x.type === 'OT';
    });
    const otherTitles = titles.filter((x) => {
      return x.type !== 'OT';
    });
    const disambiguateFrom: IDisambiguateFromSubmissionRow[] = [];
    disambiguationIswcs?.forEach((dis) => {
      disambiguateFrom.push({ iswc: dis });
    });

    axios({
      headers: { 'Cache-Control': 'no-cache', Pragma: 'no-cache' },
        method: 'put',
        url: config().iswcApiManagementUri + SUBMISSION_PATH,
      data: {
        preferredIswc,
        agency: agencyWorkCode[0].agencyName,
        sourcedb: Number(agencyWorkCode[0].agencyName),
        workcode: agencyWorkCode[0].agencyWorkCode,
        category: 'DOM',
        disambiguation: disambiguation,
        disambiguateFrom: disambiguateFrom,
        derivedWorkType: derivedWorkType || undefined,
        derivedFromIswcs: derivedFromWorks,
        originalTitle: originalTitle[0] ? originalTitle[0].title : undefined,
        otherTitles,
        interestedParties: creators.concat(publishers || []),
        previewDisambiguation,
        performers,
      },
      params: {
        preferredIswc,
      },
    })
      .then((res) => {
        resolve(res.data);
      })
      .catch((err) => {
        reject(err);
      });
  });
};

export const deleteSubmission = (
  preferredIswc: string,
  agency: string,
  workcode: string,
  reasonCode: string,
) => {
  return new Promise<any>((resolve, reject) => {
    axios({
      headers: {
        'Cache-Control': 'no-cache',
        Pragma: 'no-cache',
        'Content-Type': 'application/json',
      },
        method: 'delete',
        url: config().iswcApiManagementUri + SUBMISSION_PATH,
      params: {
        preferredIswc,
        agency,
        workcode,
        sourceDb: Number(agency),
        reasonCode,
      },
    })
      .then((res) => {
        resolve(res);
      })
      .catch((err) => {
        reject(err);
      });
  });
};

export const searchByIp = (
  nameNumber: number,
  ipType: number,
  name?: string,
  baseNumber?: string,
): Promise<any> => {
  return new Promise<any>((resolve, reject) => {
    axios({
      headers: {
        'Cache-Control': 'no-cache',
        Pragma: 'no-cache',
        'Content-Type': 'application/json',
      },
        method: 'post',
        url: '/Lookup/GetIps',
      data: {
        ipBaseNumber: baseNumber,
        type: undefined,
        isAuthoritative: false,
        affiliation: undefined,
        ipNameNumber: nameNumber,
        name,
        contributorType: ipType,
      },
    })
      .then((res) => {
        resolve(res);
      })
      .catch((err) => {
        reject(err);
      });
  });
};
