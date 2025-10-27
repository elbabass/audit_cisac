import { ISubmissionStateKeys, IIpLookupResponse } from '../SubmissionTypes';

export interface IIpLookupProps {
  close?: () => void;
  ipType: number;
  nameNumber?: string;
  addElementToSubmissionDataArray: (element: any, rowId: number, key: ISubmissionStateKeys) => void;
  rowId: number;
}

export interface IIpLookupState {
  name?: string;
  basenumber?: string;
  error: any;
  results: IIpLookupResponse[];
  loading: boolean;
}

export type IIpLookupStateKeys = keyof IIpLookupState;
