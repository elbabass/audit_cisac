import {
  IAgencyWorkCodesSubmissionRow,
  ITitleSubmissionRow,
  ICreatorPublisherSubmissionRow,
  IDerivedFromWorksSubmissionRow,
  ISubmissionStateKeys,
  ISubmissionMainDetailsStateObjectKeys,
  IPerformerSubmissionRow,
} from '../SubmissionTypes';

export interface ISubmissionMainDetailsProps {
  agencyWorkCodes: IAgencyWorkCodesSubmissionRow[];
  titles: ITitleSubmissionRow[];
  creators: ICreatorPublisherSubmissionRow[];
  publishers: ICreatorPublisherSubmissionRow[];
  derivedWorkType: string;
  derivedFromWorks: IDerivedFromWorksSubmissionRow[];
  updateSubmissionDataString: (value: string, key: ISubmissionStateKeys) => void;
  updateSubmissionDataArray: (
    value: string,
    rowId: number,
    key: ISubmissionStateKeys,
    objectKey: ISubmissionMainDetailsStateObjectKeys,
  ) => void;
  addElementToSubmissionDataArray: (element: any, rowId: number, key: ISubmissionStateKeys) => void;
  addElementToArray: (key: ISubmissionStateKeys, row: any) => void;
  removeElementFromArray: (key: ISubmissionStateKeys, rowId: number) => void;
  updateInstance?: boolean;
  performers: IPerformerSubmissionRow[];
}

export interface ISubmissionMainDetailsState {}
