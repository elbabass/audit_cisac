import { IIswcModel } from '../../../redux/types/IswcTypes';
import { RouteComponentProps } from 'react-router-dom';

export interface IDemergeContainerProps {
  preferredIswc: IIswcModel;
  linkedIswcs: string[];
}

export interface IDemergeProps {
  preferredIswc: IIswcModel;
  linkedIswcs: string[];
  demerging?: boolean;
  demergedSuccessfully?: boolean;
  error: any;
  loading?: boolean;
  searchByIswcBatch: (iswcs: string[]) => void;
  linkedIswcData?: IIswcModel[];
  demergeIswc: (agency: string, iswcsToDemerge: IDemergeIswc[]) => void;
  clearMergeError: () => void;
  clearDemergeData: () => void;
  router: RouteComponentProps;
}

export interface IDemergeState {
  iswcsToDemerge: number[];
  demergeSuccessful?: boolean;
}

export interface ILinkedToGridProps {
  linkedIswcs: IIswcModel[];
  iswcsToDemerge: number[];
  addToIswcsToDemerge: (rowId: number) => void;
}

export interface ILinkedToGridState {}

export interface IDemergeIswc {
  agency?: string;
  workCode?: string;
}
