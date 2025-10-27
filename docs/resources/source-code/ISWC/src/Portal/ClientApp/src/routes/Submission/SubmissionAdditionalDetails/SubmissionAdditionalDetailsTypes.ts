import {
  ISubmissionStateKeys,
  ISubmissionMainDetailsStateObjectKeys,
  IPerformerSubmissionRow,
  IPotentialMatch,
  IVerifiedSubmission,
} from '../SubmissionTypes';
import { IIswcModel } from '../../../redux/types/IswcTypes';

export interface ISubmissionAdditionalDetailsProps {
  preferredIswc?: string;
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
  setDisambiguation: () => void;
  performers: IPerformerSubmissionRow[];
  disambiguationReason: string;
  disambiguationIswcs: string[];
  bvltr: string;
  standardInstrumentation: string;
  potentialMatches: IPotentialMatch[];
  verifiedSubmission: IVerifiedSubmission;
  updateInstance?: boolean;
}
