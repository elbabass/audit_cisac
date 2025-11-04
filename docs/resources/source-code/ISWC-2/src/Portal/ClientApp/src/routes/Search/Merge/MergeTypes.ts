import { IIswcModel, IMergeBody } from '../../../redux/types/IswcTypes';

export interface IMergeProps {
  mergeList: IIswcModel[];
  merging: boolean | undefined;
  mergedSuccessfully: boolean | undefined;
  error: any;
  removeFromMergeList: (iswcToRemove: number | string) => void;
  mergeIswcs: (preferredIswc?: string, agency?: string, mergeBody?: IMergeBody) => void;
  clearMergeError: () => void;
}

export interface IMergeState {
  preferredIswc?: string;
  mergeBody?: IMergeBody;
  mergeSuccessful?: boolean;
}

export interface IMergeGridProps {
  mergeList: IIswcModel[];
  removeFromMergeList?: (iswcToRemove: number | string) => void;
  updateMergeRequestData: (preferredIswc: string) => void;
}

export interface IMergeGridState {
  selectedRowId: number;
}
