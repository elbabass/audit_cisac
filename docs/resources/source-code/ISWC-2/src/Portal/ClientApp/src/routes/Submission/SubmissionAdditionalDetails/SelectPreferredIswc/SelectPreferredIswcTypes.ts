import { IPotentialMatch, ISubmissionStateKeys } from '../../SubmissionTypes';
import { IIswcModel } from '../../../../redux/types/IswcTypes';

export interface ISelectPreferredIswcsProps {
  preferredIswc?: string;
  updateSubmissionDataString: (value: string, key: ISubmissionStateKeys) => void;
  searchByIswc: (iswc?: string) => IIswcModel[];
  potentialMatches: IPotentialMatch[];
}

export interface ISelectPreferredIswcsState {
  loading: boolean;
  potentialMatches: IPotentialMatchWithIswcModel[];
}

export interface IPotentialMatchWithIswcModel {
  potentialMatch: IPotentialMatch;
  iswcModel?: IIswcModel;
}
