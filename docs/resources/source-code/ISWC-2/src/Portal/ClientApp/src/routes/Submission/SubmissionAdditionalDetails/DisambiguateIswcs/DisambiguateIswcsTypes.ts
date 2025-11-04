import {
  ISubmissionStateKeys,
  ISubmissionMainDetailsStateObjectKeys,
  IPerformerSubmissionRow,
  IPotentialMatch,
} from '../../SubmissionTypes';
import { IIswcModel } from '../../../../redux/types/IswcTypes';
import { IPotentialMatchWithIswcModel } from '../SelectPreferredIswc/SelectPreferredIswcTypes';

export interface IDisambiguateIswcsProps {
  updateSubmissionDataArray: (
    value: string,
    rowId: number,
    key: ISubmissionStateKeys,
    objectKey: ISubmissionMainDetailsStateObjectKeys,
  ) => void;
  updateSubmissionDataString: (value: string, key: ISubmissionStateKeys) => void;
  addElementToArray: (key: ISubmissionStateKeys, row: any) => void;
  removeElementFromArray: (key: ISubmissionStateKeys, rowId: number) => void;
  searchByIswc: (iswc?: string) => IIswcModel[];
  performers: IPerformerSubmissionRow[];
  disambiguationReason: string;
  disambiguationIswcs: string[];
  bvltr: string;
  standardInstrumentation: string;
  potentialMatches: IPotentialMatch[];
}

export interface IDisambiguateIswcsState {
  loading: boolean;
  potentialMatches: IPotentialMatchWithIswcModel[];
  openAccordion: boolean;
}
