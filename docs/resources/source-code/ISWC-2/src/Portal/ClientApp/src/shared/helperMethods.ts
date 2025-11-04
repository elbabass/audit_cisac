import axios from 'axios';
import Cookies from 'universal-cookie';
import { getAgencies, UserRoles } from '../consts';
import { IInterestedParty, IPerformer, IRecording } from '../redux/types/IswcTypes';
import { IVerifiedSubmission } from '../routes/Submission/SubmissionTypes';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { ReactPlugin } from '@microsoft/applicationinsights-react-js';
import { config } from '../configuration/Configuration';
import { IAssignedRoles, IWebUserRole } from '../redux/types/RoleTypes';
import { getStrings } from '../configuration/Localization';

export const _parseTitles = (titles: { name: string; type: string }[]) => {
  let titleStrings: string[] = [];

  if (titles !== undefined && titles !== null && titles.length > 0) {
    titles.forEach((t) => {
      titleStrings.push(` ${t.name} (${t.type})`);
    });
  }

  return titleStrings.join();
};

export const _filterCreators = (interestedParties: IInterestedParty[]) => {
  return interestedParties.filter((ip) => ip.role !== 'E' && ip.role !== 'AM');
};

export const _getPerformers = (works: IVerifiedSubmission[]) => {
  const performersArray: IPerformer[] = [];
  works?.forEach((x) =>
    x.performers?.forEach((p) => {
      if (x.iswcEligible) {
        if (!performersArray.some((e) => e.firstName === p.firstName && e.lastName === p.lastName))
          performersArray.push(p);
      }
    }),
  );

  return performersArray;
};

export const _getRecordings = (works: IVerifiedSubmission[]) => {
  const recordingsArray: IRecording[] = [];
  works?.forEach((x) => {
    if (x.additionalIdentifiers != null && x.additionalIdentifiers.recordings != null && x.additionalIdentifiers.recordings.length > 0) {

      let submitter: string, submitterType : string, submitterWorkNumber : string; 
      if(x.additionalIdentifiers.labelIdentifiers != null && x.additionalIdentifiers.labelIdentifiers.length > 0) {
        submitter = x.additionalIdentifiers.labelIdentifiers[0].submitterDPID;
        submitterType = "Label";
        submitterWorkNumber = x.additionalIdentifiers.labelIdentifiers[0].workCode[0];
      }
      else if(x.additionalIdentifiers.publisherIdentifiers != null && x.additionalIdentifiers.publisherIdentifiers.length > 0) {
        submitter = x.additionalIdentifiers.publisherIdentifiers[0].submitterCode;
        submitterType = "Publisher";
        submitterWorkNumber = x.additionalIdentifiers.publisherIdentifiers[0].workCode[0];
      }
      else if(x.additionalIdentifiers.agencyWorkCodes != null && x.additionalIdentifiers.agencyWorkCodes.length > 0) {
        submitter = x.additionalIdentifiers.agencyWorkCodes[0].agency;
        submitterType = "Society";
        submitterWorkNumber = x.additionalIdentifiers.agencyWorkCodes[0].workCode;
      }

      x.additionalIdentifiers.recordings.forEach((r) => {
        if (!recordingsArray.some((e) => e.isrc === r.isrc))
          r.submitter = submitter;
          r.submitterType = submitterType;
          r.submitterWorkNumber = submitterWorkNumber;
          
          recordingsArray.push(r);
      })
    }
  });

  return recordingsArray;
};

export const _getContributorNames = (interestedParties: IInterestedParty[]) => {
  let creatorNames: string[] = [];
  const creators = _filterCreators(interestedParties);

  if (creators !== undefined) {
    creators.forEach((ip) => ip.name && creatorNames.push(' ' + ip.name));
  }
  return creatorNames.join();
};

export const _getNameNumbers = (interestedParties: IInterestedParty[]) => {
  let nameNumbers: string[] = [];
  const creators = _filterCreators(interestedParties);

  if (creators !== undefined) {
    creators.forEach(
      (ip) => ip.nameNumber && nameNumbers.push(' ' + padIpNameNumber(ip.nameNumber.toString())),
    );
  }
  return nameNumbers.join();
};

export const _getAgency = (id: string) => {
  let agency = getAgencies().find((x) => x.agencyId === id);
  return agency !== undefined ? agency.name : id;
};

export const _getAgenciesString = (works: IVerifiedSubmission[]) => {
  let agencies: string[] = [];
  works.forEach((x) => {
    let agencyName = _getAgency(x.agency);
    if (!agencies.find((y) => y === ' ' + agencyName)) agencies.push(' ' + agencyName);
  });

  return agencies.join();
};

export const zeroPad = (num: number) => String(num).padStart(3, '0');

export const _findGetParameter = (parameterName: string) => {
  var result = null,
    tmp = [];
  window.location.search
    .substr(1)
    .split('&')
    .forEach(function (item) {
      tmp = item.split('=');
      if (tmp[0] === parameterName) result = decodeURIComponent(tmp[1]);
    });
  return result;
};

export const _parseJwt = (token: string) => {
  if (!token) return;
  var base64Url = token.split('.')[1];
  var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
  var jsonPayload = decodeURIComponent(
    atob(base64)
      .split('')
      .map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      })
      .join(''),
  );

  return JSON.parse(jsonPayload);
};

export const formatDateString = (date: string) => {
  if (date === null || date === '') return '';

  const newDate = new Date(date);
  let minutes = ('0' + newDate.getMinutes()).slice(-2);
  return `${newDate.toDateString()} ${newDate.getHours()}:${minutes}`;
};

let user_agent_id = '';

export const getLoggedInAgencyId = () => {
  if (user_agent_id) {
    return user_agent_id;
  } else if (new Cookies().get('authToken')) {
    user_agent_id = _parseJwt(new Cookies().get('authToken'))['agent_id'];
    return user_agent_id;
  } else {
    return '';
  }
};

export const formatIswc = (iswc: string) => {
  const format = 'X-XXX.XXX.XXX-X';
  let string = '';

  for (let i = 0, x = 0; i < format.length && x < iswc.length; i++) {
    string += format.charAt(i) === 'X' ? iswc.charAt(x++) : format.charAt(i);
  }
  return string;
};

export const validateIswcAndFormat = (iswc: string | undefined | null) => {
  if (iswc === undefined || iswc === null) return '';
  iswc = iswc.replace(/[^T0-9]/g, '');

  if (iswc.length === 11 && iswc[0] === 'T') {
    const iswcSubstring = iswc.replace('T', '');
    if (iswcSubstring.length === 10 && !isNaN(parseInt(iswcSubstring))) {
      return formatIswc(iswc);
    }
  }

  return iswc;
};

export const validateIswcAndFormatArray = (iswcs: string[]) => {
  if (iswcs === undefined || iswcs === null) return [];
  const formattedArray: string[] = [];

  for (let x = 0; x < iswcs.length; x++) {
    iswcs[x] = iswcs[x].replace(/[^T0-9]/g, '');

    if (iswcs[x].length === 11 && iswcs[x][0] === 'T') {
      const iswcSubstring = iswcs[x].replace('T', '');
      if (iswcSubstring.length === 10 && !isNaN(parseInt(iswcSubstring))) {
        formattedArray.push(formatIswc(iswcs[x]));
        continue;
      }

      formattedArray.push(iswcs[x]);
      continue;
    }

    formattedArray.push(iswcs[x]);
  }

  return formattedArray;
};

export const padIpNameNumber = (ipNameNumber: string | undefined): string => {
  if (!ipNameNumber) return '';
  return ipNameNumber.length < 11 ? padIpNameNumber('0' + ipNameNumber) : ipNameNumber;
};

export const addAuthTokenToCookies = () => {
  let token = axios.defaults.headers.common['Authorization'];
  const cookies = new Cookies();

  if (token && !cookies.get('authToken')) {
    const tokenExpirationDate = new Date(_parseJwt(token)['exp'] * 1000);
    cookies.set('authToken', token, { expires: tokenExpirationDate });
  } else if (!token && cookies.get('authToken')) {
    axios.defaults.headers.common['Authorization'] = cookies.get('authToken');
  }
};

export const getUserEmailFromToken = () => {
  let token = axios.defaults.headers.common['Authorization'];
  let tokenData = _parseJwt(token);

  return tokenData?.email;
};

export const getUserAgencyFromToken = () => {
  let token = axios.defaults.headers.common['Authorization'];
  let tokenData = _parseJwt(token);

  return _getAgency(tokenData?.agent_id);
};

export const getAssignedRolesFromToken = (): IAssignedRoles => {
  let token = axios.defaults.headers.common['Authorization'];
  let roles = _parseJwt(token)?.roles;

  if (!roles) {
    return {
      search: false,
      update: false,
      reportBasics: false,
      reportExtracts: false,
      reportAgencyInterest: false,
      reportIswcFullExtract: false,
      manageRoles: false,
    };
  }

  var roleArray = roles.split(';').map((role: string) => {
    let n = Number(role);
    return n === 0 ? n : n || role;
  });

  return {
    search: roleArray.includes(UserRoles.SEARCH_ROLE),
    update: roleArray.includes(UserRoles.UPDATE_ROLE),
    reportBasics: roleArray.includes(UserRoles.REPORT_BASICS_ROLE),
    reportExtracts: roleArray.includes(UserRoles.REPORT_EXTRACT_ROLE),
    reportAgencyInterest: roleArray.includes(UserRoles.REPORT_AGENCY_INTEREST_ROLE),
    reportIswcFullExtract: roleArray.includes(UserRoles.REPORT_ISWC_FULL_EXTRACT_ROLE),
    manageRoles: roleArray.includes(UserRoles.MANAGE_ROLES_ROLE),
  };
};

export const addApplicationInsights = (history: any) => {
  var reactPlugin = new ReactPlugin();
  var appInsights = new ApplicationInsights({
    config: {
      instrumentationKey: config().applicationInsightsKey,
      extensions: [reactPlugin],
      extensionConfig: {
        [reactPlugin.identifier]: { history: history },
      },
    },
  });
  appInsights.loadAppInsights();
};

export function pad(n: any, width: number, z: string = '0') {
  n = n + '';
  return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
}

export const addSpaceToString = (string?: string) => {
  if (!string) return '';
  return string.replace(/([A-Z])/g, ' $1').trim();
};

export const getDate = (dateParam: Date, daysPrevious?: number) => {
  let date = dateParam || new Date();

  if (daysPrevious) {
    date.setDate(date.getDate() - daysPrevious);
  }

  const day = String(date.getDate()).padStart(2, '0');
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const year = date.getFullYear();

  return `${year}-${month}-${day}`;
};

export const UserLoggedIn = () => {
  const cookies = new Cookies();
  return cookies.get('authToken') || axios.defaults.headers.common['Authorization'];
};

export const checkRoles = (requiredRole: number, roles: IWebUserRole[]) => {
  for (let x = 0; x < roles.length; x++) {
    if (requiredRole === roles[x].role && roles[x].isApproved) {
      return true;
    }
  }

  return false;
};

export const getRoleText = (role: number) => {
  const {
    SEARCH,
    UPDATE,
    REPORT_BASICS,
    REPORT_EXTRACTS,
    REPORT_AGENCY_INTEREST,
    REPORT_ISWC_FULL,
    MANAGE_ROLES,
  } = getStrings();

  switch (role) {
    case UserRoles.SEARCH_ROLE:
      return SEARCH;
    case UserRoles.UPDATE_ROLE:
      return UPDATE;
    case UserRoles.REPORT_BASICS_ROLE:
      return REPORT_BASICS;
    case UserRoles.REPORT_EXTRACT_ROLE:
      return REPORT_EXTRACTS;
    case UserRoles.REPORT_AGENCY_INTEREST_ROLE:
      return REPORT_AGENCY_INTEREST;
    case UserRoles.REPORT_ISWC_FULL_EXTRACT_ROLE:
      return REPORT_ISWC_FULL;
    case UserRoles.MANAGE_ROLES_ROLE:
      return MANAGE_ROLES;
    default:
      return '';
  }
};

export const createMergeDemergeMessage = (iswcs: string[]) => {
  let message = '';

  for (let i = 0; i < iswcs.length; i++) {
    message += iswcs[i];

    if (i !== iswcs.length - 1) {
      message += ', ';
    }
  }

  return message;
};

export const getRoleType = (role?: string) => {
  switch (role) {
    case 'C':
      return 'C (C,A,CA)';
    case 'MA':
      return 'MA (AR, SR)';
    case 'TA':
      return 'TA (AD, SA, TR)';
    default:
      return '';
  }
};

export const legalEntityMessage = (legalEntity?: string, isCreator?: boolean) => {
  const { LEGAL_ENTITY, NATURAL_PERSON } = getStrings();
  if (isCreator) {
    if (legalEntity === 'L') return LEGAL_ENTITY;
  } else {
    if (legalEntity === 'N') return NATURAL_PERSON;
  }
};
