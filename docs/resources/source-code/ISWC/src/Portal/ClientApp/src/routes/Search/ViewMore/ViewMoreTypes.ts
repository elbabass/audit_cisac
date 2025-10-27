import { IIswcModel } from '../../../redux/types/IswcTypes';
import { IGridRow } from '../../../components/GridComponents/Grid/GridTypes';
import { IAgencyWorkCodesSubmissionRow, ISubmissionState } from '../../Submission/SubmissionTypes';
import { IAssignedRoles } from '../../../redux/types/RoleTypes';
import { RouteComponentProps } from 'react-router-dom';

export interface IViewMoreProps {
  iswcModel: IIswcModel;
  mergeList?: IIswcModel[];
  addToMergeList?: (iswc: IIswcModel) => void;
  removeFromMergeList?: (iswcToRemove: number | string) => void;
  close?: () => void;
  isSubmissionGrid?: boolean;
  deleteSubmission?: (
    preferredIswc: string,
    agency: string,
    workcode: string,
    reasonCode: string,
  ) => void;
  deletionLoading?: boolean;
  deletionError?: any;
  clearSubmissionError?: () => void;
  assignedRoles?: IAssignedRoles;
  router?: RouteComponentProps;
}

export interface IViewMoreState {
  rows: {
    titles?: IGridRow[];
    creators?: IGridRow[];
    works?: IGridRow[];
    derivedWorks?: IGridRow[];
    disambiguation?: IGridRow[];
    performers?: IGridRow[];
    recordings?: IGridRow[];
  };
  archivedIswcs: string[];
  isModalOpen: boolean;
  deletionMessage?: string;
  isResubmit?: boolean;
  mostRecentSubmissionFromAgency?: ISubmissionState;
  deletedWorkCode?: IAgencyWorkCodesSubmissionRow;
}

export interface IViewMoreHeaderProps {
  iswc: IIswcModel;
  updateMergeList: () => void;
  mergeList?: IIswcModel[];
  isSubmissionGrid?: boolean;
  assignedRoles?: IAssignedRoles;
}

export interface IViewMoreHeaderState {
  mostRecentSubmissionFromAgency?: ISubmissionState;
}

export interface IDeletionData {
  preferredIswc: string;
  agency: string;
  workcode: string;
  reasonCode: string;
  rowId: number;
}
