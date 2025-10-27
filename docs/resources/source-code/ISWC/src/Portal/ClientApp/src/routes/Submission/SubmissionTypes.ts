import {
  IDerivedFrom,
  IDisambiguateFrom,
  IPerformer,
  IInstrumentation,
  ITitle,
  IInterestedParty,
  IIswcModel,
  IAdditionalIdentifiers
} from '../../redux/types/IswcTypes';
import { RouteComponentProps } from 'react-router-dom';

export interface ISubmissionProps {
  step: number;
  setSubmissionStep: (step: number) => void;
  newSubmission: (
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
  ) => void;
  updateSubmission: (
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
  ) => void;
  searchByIswc: (iswc?: string) => IIswcModel[];
  verifiedSubmission: IVerifiedSubmission;
  loading: boolean;
  potentialMatches: IPotentialMatch[];
  error: any;
  agencyWorkCode?: IAgencyWorkCodesSubmissionRow[];
  titles?: ITitleSubmissionRow[];
  creators?: ICreatorPublisherSubmissionRow[];
  disambiguation?: boolean;
  preferredIswc?: string;
  publishers?: ICreatorPublisherSubmissionRow[];
  derivedWorkType?: string;
  derivedFromWorks?: IDerivedFromWorksSubmissionRow[];
  disambiguationIswcs?: string[];
  disambiguationReason?: string;
  bvltr?: string;
  performers?: IPerformerSubmissionRow[];
  standardInstrumentation?: string;
  updateInstance?: boolean;
  clearSearch: () => void;
  router: RouteComponentProps;
  clearSubmissionError: () => void;
}

export interface ISubmissionState {
  agencyWorkCode: IAgencyWorkCodesSubmissionRow[];
  titles: ITitleSubmissionRow[];
  creators: ICreatorPublisherSubmissionRow[];
  disambiguation: boolean;
  preferredIswc?: string;
  publishers: ICreatorPublisherSubmissionRow[];
  derivedWorkType: string;
  derivedFromWorks: IDerivedFromWorksSubmissionRow[];
  disambiguationIswcs: string[];
  disambiguationReason: string;
  bvltr: string;
  performers: IPerformerSubmissionRow[];
  standardInstrumentation: string;
}

export interface ITitleSubmissionRow {
  title: string;
  type: string;
}

export interface ICreatorPublisherSubmissionRow {
  name: string;
  nameNumber?: string;
  baseNumber: string;
  role: string;
  legalEntityType?: string;
}

export interface IAgencyWorkCodesSubmissionRow {
  agencyWorkCode: string;
  agencyName: string;
}

export interface IDerivedFromWorksSubmissionRow {
  iswc?: string;
  title?: string;
}

export interface IPerformerSubmissionRow {
  lastName: string;
  firstName: string;
}

export interface IDisambiguateFromSubmissionRow {
  iswc: string;
}

export interface IPerformerSubmissionRow {
  firstName: string;
  lastName: string;
}

export interface IRejection {
  code: string;
  message: string;
}

export type ISubmissionStateKeys = keyof ISubmissionState;

export type ISubmissionMainDetailsStateObjectKeys =
  | keyof ITitleSubmissionRow
  | keyof ICreatorPublisherSubmissionRow
  | keyof IAgencyWorkCodesSubmissionRow
  | keyof IDerivedFromWorksSubmissionRow
  | keyof IPerformerSubmissionRow;

export interface ISubmission {
  preferredIswc?: string;
  agency: string;
  sourcedb: number;
  workcode: string;
  category: string;
  disambiguation: boolean;
  createdDate: string;
  lastModifiedDate: string;
  disambiguationReason?: string;
  disambiguateFrom?: IDisambiguateFrom[];
  bvltr?: string;
  derivedWorkType?: string;
  derivedFromIswcs?: IDerivedFrom[];
  performers?: IPerformer[];
  instrumentation?: IInstrumentation[];
  cisnetCreatedDate?: string;
  cisnetLastModifiedDate?: string;
  iswc?: string;
  originalTitle: string;
  otherTitles?: ITitle[];
  interestedParties: IInterestedParty[];
  archivedIswc?: string;
  PreviewDisambiguation?: boolean;
}

export interface IVerifiedSubmission {
  id: number;
  iswc?: any;
  iswcStatus?: any;
  deleted: boolean;
  linkedFrom?: string;
  linkedTo?: string;
  rejection?: IRejection;
  createdDate?: string;
  lastModifiedDate: string;
  lastModifiedBy?: string;
  agency: string;
  sourcedb: number;
  workcode: string;
  category: string;
  disambiguation: boolean;
  disambiguationReason?: string;
  disambiguateFrom?: IDisambiguateFrom[];
  bvltr?: string;
  derivedWorkType?: string;
  derivedFromIswcs?: IDerivedFrom[];
  performers?: IPerformer[];
  instrumentation?: IInstrumentation[];
  cisnetCreatedDate?: string;
  cisnetLastModifiedDate?: string;
  originalTitle: string;
  otherTitles?: ITitle[];
  interestedParties: IInterestedParty[];
  archivedIswc?: string;
  iswcEligible: boolean;
  additionalIdentifiers?: IAdditionalIdentifiers
}

export interface IPotentialMatch {
  iswc: string;
  interestedParties: IInterestedParty[];
  originalTitle: string;
}

export interface ISubmissionResponse {
  verifiedSubmission: IVerifiedSubmission;
  potentialMatches: IPotentialMatch[];
  linkedIswcs: any[];
}

export interface IIpLookupResponse {
  ipBaseNumber: string;
  type: number;
  isAuthoritative: boolean;
  affiliatiion: string;
  ipNameNumber: number;
  name: string;
  contributorType: number;
  legalEntityType: string;
}
