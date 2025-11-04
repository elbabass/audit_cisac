import { IVerifiedSubmission } from '../../routes/Submission/SubmissionTypes';

export interface IIswcModel {
  agency: string;
  createdDate: string;
  interestedParties: IInterestedParty[];
  iswc: string;
  iswcStatus: string;
  linkedISWC?: string[];
  parentISWC?: string;
  lastModifiedBy: string;
  lastModifiedDate: string;
  originalTitle: string;
  otherTitles?: ITitle[];
  works?: IVerifiedSubmission[];
}

export interface IInterestedParty {
  baseNumber: string;
  name?: string;
  lastName?: string;
  nameNumber?: number;
  role?: string;
  type?: string;
  affiliation?: string;
}

export interface ITitle {
  title: string;
  type: string;
}

export interface IDisambiguateFrom {
  iswc: string;
  title: string;
}

export interface IDerivedFrom {
  iswc: string;
  title: string;
}

export interface IInstrumentation {
  code: string;
}

export interface IPerformer {
  isni?: string;
  ipn?: number;
  firstName: string;
  lastName: string;
  designation?: string;
}

export interface IRecording {
  isrc?: string;
  recordingTitle?: string;
  subTitle?: string;
  labelName?: string;
  submitter?: string;
  submitterType?: string;
  submitterWorkNumber?: string;
  performers?: IPerformer[];
}

export interface IWorkNumber {
  agencyCode: string;
  agencyWorkCode: string;
}

export interface IMergeBody {
  iswcs?: string[];
  agencyWorks?: IWorkNumber[];
}

export interface IBatchSearchIswc {
  iswc: string;
}

export interface IWorkflow {
  workflowTaskId: number;
  workflowType: string;
  status: string;
  originatingSociety: string;
  assignedSociety: string;
  createdDate: string;
  iswcMetadata: IIswcModel;
  workflowMessage?: string;
}

export interface IAuditHistoryResult {
  submittedDate: string;
  transactionType: string;
  submittingAgency: string;
  workNumber: string;
  lastModifiedUser: string;
  creators?: IInterestedParty[];
  titles: any;
  status: string;
  workCode: number;
}

export interface IWorkflowSearchModel {
  agency: string;
  showWorkflows: number;
  statuses: number[];
  startIndex: number;
  pageLength: number;
  fromDate?: string;
  toDate?: string;
  iswc?: string;
  agencyWorkCodes?: string;
  originatingAgency?: string;
  workflowType?: string;
}

export interface IAgencyWorkCode {
  agency: string;
  workCode: string;
}

export interface IPublisherIdentifier {
  nameNumber: number;
  submitterCode: string;
  workCode: string[];
}

export interface ILabelIdentifier {
  submitterDPID: string;
  workCode: string[];
}

export interface IAdditionalIdentifiers {
  agencyWorkCodes?: IAgencyWorkCode[];
  publisherIdentifiers?: IPublisherIdentifier[];
  labelIdentifiers?: ILabelIdentifier[];
  recordings?: IRecording[];
}
