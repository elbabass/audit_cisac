import { IIswcModel } from '../../redux/types/IswcTypes';
import { RouteComponentProps } from 'react-router-dom';
import { IAssignedRoles } from '../../redux/types/RoleTypes';

export interface ISearchProps {
  isSearching: boolean;
  searchResults?: IIswcModel[];
  mergeList: IIswcModel[];
  searchByIswc: (iswc?: string) => void;
  searchByWorkCode: (agency?: string, workCode?: string) => void;
  searchByTitleAndContributor: (
    title?: string,
    surnames?: string,
    nameNumbers?: string,
    baseNumbers?: string,
  ) => void;
  addToMergeList: (iswc: IIswcModel) => void;
  removeFromMergeList: (iswc: number | string) => void;
  error?: any;
  clearSearchError: () => void;
  clearSubmissionError: () => void;
  router: RouteComponentProps;
  deleteSubmission?: (
    preferredIswc: string,
    agency: string,
    workcode: string,
    reasonCode: string,
  ) => void;
  deletionLoading: boolean;
  deletionError?: string;
  assignedRoles?: IAssignedRoles;
}

export interface ISearchState {
  formFields: {
    iswc: string;
    workCode: string;
    agency: string;
    title: string;
    surnames: string;
    nameNumbers: string;
    baseNumbers: string;
    creatorNameNumbers: string;
    creatorBaseNumbers: string;
  };
  showSubMessage: boolean;
}
