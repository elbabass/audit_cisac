import { IGridHeaderCell } from './components/GridComponents/Grid/GridTypes';
import {
  ITitleSubmissionRow,
  ICreatorPublisherSubmissionRow,
  IAgencyWorkCodesSubmissionRow,
  IDerivedFromWorksSubmissionRow,
  IPerformerSubmissionRow,
  IDisambiguateFromSubmissionRow,
} from './routes/Submission/SubmissionTypes';
import { IAgency } from './redux/types/LookupTypes';
import { IDropdownOption } from './components/FormInput/FormInputTypes';
import { getStrings } from './configuration/Localization';

// App Modes
export const PUBLIC_MODE = 'public';
export const PRIVATE_MODE = 'private';

// Reports
export const STATUS_ALL: number = 0;
export const STATUS_SUCCEEDED: number = 1;
export const STATUS_ERROR: number = 2;

export const TS_ALL: number = 0;
export const TS_AGENCY: number = 1;
export const TS_PUBLISHER: number = 2;

export const REPORT_TYPE_AGENCY_INTEREST_EXTRACT: number = 0;
export const REPORT_TYPE_ISWC_FULL_EXTRACT: number = 1;
export const REPORT_TYPE_SUBMISSION_AUDIT: number = 2;
export const REPORT_TYPE_FILE_SUBMISSION_AUDIT: number = 3;
export const REPORT_TYPE_AGENCY_STATISTICS: number = 4;
export const REPORT_TYPE_PUBLISHER_ISWC_TRACKING: number = 5;
export const REPORT_TYPE_POTENTIAL_DUPLICATES: number = 6;
export const REPORT_TYPE_AGENCY_WORK_LIST: number = 7;
export const REPORT_TYPE_ISWC_CREATOR_EXTRACT: number = 8;

export const PER_MONTH_PERIOD: number = 0;
export const PER_YEAR_PERIOD: number = 1;

export const SUBMISSION_AUDIT_LIST_VIEW: number = 1;
export const SUBMISSION_AUDIT_BAR_CHART: number = 2;
export const SUBMISSION_AUDIT_EXTRACT_FTP: number = 3;

//UserRoles
export enum UserRoles {
  SEARCH_ROLE = 1,
  UPDATE_ROLE,
  REPORT_BASICS_ROLE,
  REPORT_EXTRACT_ROLE,
  REPORT_AGENCY_INTEREST_ROLE,
  REPORT_ISWC_FULL_EXTRACT_ROLE,
  MANAGE_ROLES_ROLE,
}

// Modal Types
export const TEXT_MODAL: number = 0;
export const INPUT_MODAL: number = 1;

// Grid Field Types and Strings
export const PREFERRED_ISWC_FIELD: string = 'PREFERRED_ISWC_FIELD';
export const ORIGINAL_TITLE_FIELD: string = 'ORIGINAL_TITLE_FIELD';
export const CREATOR_NAMES_FIELD: string = 'CREATOR_NAMES_FIELD';
export const VIEW_MORE_FIELD: string = 'VIEW_MORE_FIELD';
export const VIEW_LESS_FIELD: string = 'VIEW_LESS_FIELD';
export const ALL_TITLES_FIELD: string = 'ALL_TITLES_FIELD';
export const TYPE_FIELD: string = 'TYPE_FIELD';
export const IP_NAME_NUMBER_FIELD: string = 'IP_NAME_NUMBER_FIELD';
export const AFFILIATION_FIELD: string = 'AFFILIATION_FIELD';
export const IP_BASE_NUMBER_FIELD: string = 'IP_BASE_NUMBER_FIELD';
export const ROLE_FIELD: string = 'ROLE_FIELD';
export const ROLLED_UP_ROLE: string = 'ROLLED_UP_ROLE';
export const AGENCY_CODE_FIELD: string = 'AGENCY_CODE_FIELD';
export const AGENCY_NAME_FIELD: string = 'AGENCY_NAME_FIELD';
export const AGENCY_WORK_CODE_FIELD: string = 'AGENCY_WORK_CODE_FIELD';
export const CREATION_DATE_FIELD: string = 'CREATION_DATE_FIELD';
export const ISWC_FIELD: string = 'ISWC_FIELD';
export const CHECKBOX_FIELD: string = 'CHECKBOX_FIELD';
export const RANK_FIELD: string = 'RANK_FIELD';
export const SUBMITTING_AGENCIES_FIELD: string = 'SUBMITTING_AGENCIES_FIELD';
export const ORIGINAL_SUBMISSION_FIELD: string = 'ORIGINAL_SUBMISSION_FIELD';
export const LAST_UPDATE_FIELD: string = 'LAST_UPDATE_FIELD';
export const REMOVE_FIELD: string = 'REMOVE_FIELD';
export const ADD_FIELD: string = 'ADD_FIELD';
export const TITLE_FIELD: string = 'TITLE_FIELD';
export const COMMENTS: string = 'COMMENTS';
export const DERIVED_ISWC_FIELD: string = 'DERIVED_ISWC_FIELD';
export const DERIVED_TITLE_FIELD: string = 'DERIVED_TITLE_FIELD';
export const DERIVED_WORK_TITLE_FIELD: string = 'DERIVED_WORK_TITLE_FIELD';
export const DERIVED_WORK_ISWC_FIELD: string = 'DERIVED_WORK_ISWC_FIELD';
export const DATE_FIELD: string = 'DATE_FIELD';
export const METHOD_FIELD: string = 'METHOD_FIELD';
export const SUBMITTING_AGENCY_FIELD: string = 'SUBMITTING_AGENCY_FIELD';
export const SUBMITTING_AGENCY_WORK_NO_FIELD: string = 'SUBMITTING_AGENCY_WORK_NO_FIELD';
export const CREATORS_FIELD: string = 'CREATORS_FIELD';
export const PUBLISHERS_FIELD: string = 'PUBLISHERS_FIELD';
export const TITLES_FIELD: string = 'TITLES_FIELD';
export const STATUS_FIELD: string = 'STATUS_FIELD';
export const SOURCE_AGENCY_FIELD: string = 'SOURCE_AGENCY_FIELD';
export const REASON_FIELD: string = 'REASON_FIELD';
export const DISAMBIGUATED_FROM_FIELD: string = 'DISAMBIGUATED_FROM_FIELD';
export const AUTHORITATIVE_FIELD: string = 'AUTHORITATIVE_FIELD';
export const NAME_FIELD: string = 'NAME_FIELD';
export const SURNAME_FIELD: string = 'SURNAME_FIELD';
export const PUBLISHER_IP_NAME: string = 'PUBLISHER_IP_NAME';
export const WORKFLOW_ID_FIELD: string = 'WORKFLOW_ID_FIELD';
export const WORKFLOW_TYPE_FIELD: string = 'WORKFLOW_TYPE_FIELD';
export const ORIGINATING_AGENCY_FIELD: string = 'ORIGINATING_AGENCY_FIELD';
export const ASSIGNED_TO_FIELD: string = 'ASSIGNED_TO_FIELD';
export const SELECT_ALL_FIELD: string = 'SELECT_ALL_FIELD';
export const SELECT_PREFERRED_FIELD: string = 'SELECT_PREFERRED_FIELD';
export const FIRST_NAME_FIELD: string = 'FIRST_NAME_FIELD';
export const LAST_NAME_FIELD: string = 'LAST_NAME_FIELD';
export const ADD_NEW_CREATOR: string = 'ADD_NEW_CREATOR';
export const ADD_NEW_TITLE: string = 'ADD_NEW_TITLE';
export const ADD_NEW_PUBLISHER: string = 'ADD_NEW_PUBLISHER';
export const AGENCY_WORK_CODES: string = 'AGENCY_WORK_CODES';
export const DERIVED_FROM_WORK: string = 'DERIVED_FROM_WORK';
export const ADD_NEW_DERIVED_WORK: string = 'ADD_NEW_DERIVED_WORK';
export const NEW_SUBMISSION: string = 'NEW_SUBMISSION';
export const UPDATE_SUBMISSION: string = 'UPDATE_SUBMISSION';
export const MAKE_ANOTHER_SUBMISSION: string = 'MAKE_ANOTHER_SUBMISSION';
export const COPY_AS_NEW_SUBMISSION: string = 'COPY_AS_NEW_SUBMISSION';
export const GO_BACK: string = 'GO_BACK';
export const SUBMIT: string = 'SUBMIT';
export const NEXT: string = 'NEXT';
export const IF_DERIVED_WORK: string = 'IF_DERIVED_WORK';
export const SELECT_PREFERRED_ISWC: string = 'SELECT_PREFERRED_ISWC';
export const DISAMBIGUATE_ALL_ISWCS: string = 'DISAMBIGUATE_ALL_ISWCS';
export const MATCHES_FOUND: string = 'MATCHES_FOUND';
export const SELECT_ISWC_OR_DISAMBIGUATE: string = 'SELECT_ISWC_OR_DISAMBIGUATE';
export const PREFERRED_ISWC_ASSIGNED: string = 'PREFERRED_ISWC_ASSIGNED';
export const SUBMISSION_SUCCESSFUL: string = 'SUBMISSION_SUCCESSFUL';
export const SUBMISSION_SUCCESSFUL_SELECTED_PREF: string = 'SUBMISSION_SUCCESSFUL_SELECTED_PREF';
export const SUBMISSION_SUCCESSFUL_UPDATE: string = 'SUBMISSION_SUCCESSFUL_UPDATE';
export const ISWCS_TO_DISAMIGUATE: string = 'ISWCS_TO_DISAMIGUATE';
export const ADDITIONAL_ISWCS_TO_DISAMBIGUATE: string = 'ADDITIONAL_ISWCS_TO_DISAMBIGUATE';
export const REASONS_FOR_DISAMBIGUATION: string = 'REASONS_FOR_DISAMBIGUATION';
export const STANDARD_INSTRUMENTATION_FIELD: string = 'STANDARD_INSTRUMENTATION_FIELD';
export const PREFERRED: string = 'PREFERRED';
export const PROVISIONAL: string = 'PROVISIONAL';
export const PERFORMERS: string = 'PERFORMERS';
export const ADD_NEW_PERFORMER: string = 'ADD_NEW_PERFORMER';
export const CANCEL: string = 'CANCEL';
export const SEARCH: string = 'SEARCH';
export const ADD: string = 'ADD';
export const IP_NOT_FOUND: string = 'IP_NOT_FOUND';
export const LOOKUP: string = 'LOOKUP';
export const LOGOUT: string = 'LOGOUT';
export const SELECT_ISWC: string = 'SELECT_ISWC';
export const SELECT_TYPE: string = 'SELECT_TYPE';
export const GO_BACK_TO_SEARCH_RESULTS: string = 'GO_BACK_TO_SEARCH_RESULTS';
export const USER_GUIDE: string = 'USER_GUIDE';
export const SELECT_ISWC_INELIGIBLE: string = 'SELECT_ISWC_INELIGIBLE';
export const DEMERGE_SUCCESSFUL: string = 'DEMERGE_SUCCESSFUL';
export const DEMERGE: string = 'DEMERGE';
export const MERGE_SUCCESSFUL: string = 'MERGE_SUCCESSFUL';
export const REMOVE: string = 'REMOVE';
export const SEARCH_RESULTS: string = 'SEARCH_RESULTS';
export const OR: string = 'OR';
export const YES: string = 'YES';
export const WF_UPDATED_SUCCESSFULLY: string = 'WF_UPDATED_SUCCESSFULLY';
export const ERROR_OCCURED: string = 'ERROR_OCCURED';
export const UPDATE_WF_TASKS: string = 'UPDATE_WF_TASKS';
export const OKAY: string = 'OKAY';
export const APPROVE_ALL: string = 'APPROVE_ALL';
export const ITEMS_SELECTED: string = 'ITEMS_SELECTED';
export const REJECT_ALL: string = 'REJECT_ALL';
export const WF_TASKS_WILL_BE: string = 'WF_TASKS_WILL_BE';
export const ARE_YOU_SURE: string = 'ARE_YOU_SURE';
export const PAGINATION_SHOW: string = 'PAGINATION_SHOW';
export const PAGINATION_NEXT: string = 'PAGINATION_NEXT';
export const PAGINATION_PREVIOUS: string = 'PAGINATION_PREVIOUS';
export const PAGINATION_PER_PAGE: string = 'PAGINATION_PER_PAGE';
export const OF: string = 'OF';
export const RESULTS: string = 'RESULTS';
export const PERFORMERS_FIELD: string = 'PERFORMERS_FIELD';
export const SURNAME_BAND_NAME: string = 'SURNAME_BAND_NAME';
export const FIRST_NAME: string = 'FIRST_NAME';
export const DISAMBIGUATION_MESSAGE: string = 'DISAMBIGUATION_MESSAGE';
export const SUB_WITH_AGENCY_WORKCODE: string = 'SUB_WITH_AGENCY_WORKCODE';
export const SUCCESSFULLY_DELETED: string = 'SUCCESSFULLY_DELETED';
export const DELETE_SUBMISSION: string = 'DELETE_SUBMISSION';
export const DELETE_CONFIRM: string = 'DELETE_CONFIRM';
export const CONFIRM_DELETION: string = 'CONFIRM_DELETION';
export const MULTIPLE: string = 'MULTIPLE';
export const NON_SOCIETY: string = 'NON_SOCIETY';
export const MERGE_INFO: string = 'MERGE_INFO';
export const ISWC_HAS_BEEN_MERGED: string = 'ISWC_HAS_BEEN_MERGED';
export const SEARCH_FOR_ISWC: string = 'SEARCH_FOR_ISWC';
export const WORK_HAS_BEEN_MERGED: string = 'WORK_HAS_BEEN_MERGED';
export const PUBLISHER_IP_NAME_NUMBER_FIELD: string = 'PUBLISHER_IP_NAME_NUMBER_FIELD';
export const PUBLISHER_WORK_NUMBER_FIELD: string = 'PUBLISHER_WORK_NUMBER_FIELD';
export const SUBMITTING_PUBLISHER_FIELD: string = 'SUBMITTING_PUBLISHER_FIELD';
export const SUBMITTED_FILE_NAME_FIELD: string = 'SUBMITTED_FILE_NAME_FIELD';
export const ACK_FILE_NAME_FIELD: string = 'ACK_FILE_NAME_FIELD';
export const ACK_GENERATED_FIELD: string = 'ACK_GENERATED_FIELD';
export const PROCESSING_DURATION_FIELD: string = 'PROCESSING_DURATION_FIELD';
export const USERNAME_FIELD: string = 'USERNAME_FIELD';
export const ROLE_REQUESTED_FIELD: string = 'ROLE_REQUESTED_FIELD';
export const MANAGE_ROLES_ACTION_FIELD: string = 'MANAGE_ROLES_ACTION_FIELD';
export const SEARCH_FIELD: string = 'SEARCH_FIELD';
export const MANAGE_ROLES_FIELD: string = 'MANAGE_ROLES_FIELD';
export const UPDATE_FIELD: string = 'UPDATE_FIELD';
export const REPORT_BASICS_FIELD: string = 'REPORT_BASICS_FIELD';
export const REPORT_EXTRACT_FIELD: string = 'REPORT_EXTRACT_FIELD';
export const REPORT_AGENCY_INTEREST_FIELD: string = 'REPORT_AGENCY_INTEREST_FIELD';
export const REPORT_ISWC_FULL_FIELD: string = 'REPORT_ISWC_FULL_FIELD';
export const ISNI_FIELD: string = 'ISNI_FIELD';
export const IPN_FIELD: string = 'IPN_FIELD';
export const DESIGNATION_FIELD: string = 'DESIGNATION_FIELD';
export const ISRC_FIELD: string = 'ISRC_FIELD';
export const RECORDING_TITLE_FIELD: string = 'RECORDING_TITLE_FIELD';
export const SUB_TITLE_FIELD: string = 'SUB_TITLE_FIELD';
export const LABEL_NAME_FIELD: string = 'LABEL_NAME_FIELD';
export const SUBMITTER_FIELD: string = 'SUBMITTER_FIELD';
export const SUBMITTER_TYPE_FIELD: string = 'SUBMITTER_TYPE_FIELD';
export const SUBMITTER_WORK_NUMBER_FIELD: string = 'SUBMITTER_WORK_NUMBER_FIELD';

// Cell Actions
export const VIEW_MORE_ACTION: string = 'VIEW_MORE_ACTION';
export const IP_LOOKUP_ACTION: string = 'IP_LOOKUP_ACTION';
export const DELETE_SUBMISSION_ACTION: string = 'DELETE_SUBMISSION_ACTION';
export const MANAGE_ROLES_ACTION: string = 'MANAGE_ROLES_ACTION';

// Icons
export const ADD_NEW_ICON: string = './assets/icon_addNew.svg';
export const ADD_NEW_ICON_RED: string = './assets/icon_addNew_red.svg';
export const ARROW_DOWN_ICON: string = './assets/icon_arrowDown.svg';
export const ARROW_DOWN_ACTIVE_ICON: string = './assets/icon_arrowDown-red.svg';
export const ARROW_UP_ICON: string = './assets/icon_arrowUp.svg';
export const EDIT_ICON: string = './assets/icon_edit.svg';
export const EXTERNAL_LINK_ICON: string = './assets/icon_externalLink.svg';
export const EXTERNAL_LINK_ICON_GREY: string = './assets/icon_externalLink-grey.svg';
export const LEFT_ARROW_ICON: string = './assets/icon_leftArrow.svg';
export const LEFT_ARROW_RED_ICON: string = './assets/icon_leftArrowRed.svg';
export const LEFT_ARROW_DISABLED_ICON: string = './assets/icon_leftArrowDisabled.svg';
export const MENU_ICON: string = './assets/icon_menu.svg';
export const PENCIL_ICON: string = './assets/icon_pencil.svg';
export const REMOVE_ICON: string = './assets/remove.svg';
export const RIGHT_ARROW_DISABLED_ICON: string = './assets/icon_rightArrowDisabled.svg';
export const RIGHT_ARROW_ICON: string = './assets/icon_rightArrow.svg';
export const SEARCH_ICON: string = './assets/icon_search.svg';
export const SEARCH_ICON_GREY: string = './assets/icon_search_grey.svg';
export const SETTINGS_ICON: string = './assets/icon_settings.svg';
export const VIEW_MORE_ICON: string = './assets/icon_viewMore.svg';
export const WORK_FLOW_ICON: string = './assets/icon_workflow.svg';
export const LOGO_ICON: string = './assets/logo.png';
export const STATUS_APPROVED_ICON: string = './assets/icon-status-approved.svg';
export const STATUS_PENDING_ICON: string = './assets/icon-status-pending.svg';
export const STATUS_REJECTED_ICON: string = './assets/icon-status-rejected.svg';
export const STATUS_INFO_ICON: string = './assets/icon-status-info.svg';
export const CLOSE_ICON: string = './assets/icon_close.svg';
export const STATUS_REJECTED_ICON_BLACK: string = './assets/icon-status-rejected-black.svg';
export const CALENDAR_ICON: string = './assets/icon_calendar.svg';
export const PLUS_ICON: string = './assets/icon_plus.svg';
export const CHECKMARK_ICON: string = './assets/icon_checkmark.svg';
export const CHECKMARK_ICON_BLACK: string = './assets/icon_checkmark_black.svg';
export const CLOSE_ICON_WHITE: string = './assets/icon_close_white.svg';
export const LOOKUP_ICON: string = './assets/icon_lookup.svg';
export const LOOKUP_ICON_BLACK: string = './assets/icon_lookup_black.svg';
export const DISAMBIGUATED_ICON: string = './assets/icon_disambiguated.svg';
export const MERGED_ICON: string = './assets/icon_merged.svg';
export const REPORT_ICON: string = './assets/icon_report.svg';
export const REPORT_ICON_BLACK: string = './assets/icon_report_black.svg';
export const DOWNLOAD_ICON: string = './assets/icon_download.svg';
export const DOWNLOAD_ICON_GREY: string = './assets/icon_download_grey.svg';

// Paths - Portal
export const SEARCH_DEV_PROD_PATH: string = '/(search|)';
export const SEARCH_PATH: string = '/search';
export const SUBMISSION_PATH: string = '/submission';
export const WORKFLOWS_PATH: string = '/workflows';
export const DEMERGE_PATH: string = '/demerge';
export const MERGE_PATH: string = '/merge';
export const SUBMISSION_HISTORY_PATH: string = '/submission-history';
export const REPORTS_PATH: string = '/reports';
export const AGENCY_PORTAL_USER_GUIDE: string = './assets/ISWC_Agency_Portal_User_Guide.pdf';
export const USER_PROFILE_PATH: string = '/my-profile';
export const MANAGE_USER_ROLES_PATH: string = '/manage-user-roles';

// Paths - Portal
export const PUBLIC_SEARCH_PATH: string = '/search';
export const PUBLIC_USER_GUIDE_PATH: string = '/user-guide';
export const PUBLIC_LANDING_PAGE = '/welcome';
export const CISAC_USER_GUIDE =
  'https://members.cisac.org/CisacPortal/consulterDocument.do?id=38766';

// Grid Cell Types
export const DATE_TYPE: string = 'DATE_TYPE';
export const NUMBER_TYPE: string = 'NUMBER_TYPE';

// Submission Form Grid Rows
export const TITLES_SUBMISSION_ROW: ITitleSubmissionRow = { title: '', type: 'AL' };
export const CREATORS_SUBMISSION_ROW: ICreatorPublisherSubmissionRow = {
  name: '',
  nameNumber: '',
  baseNumber: '',
  role: 'CA',
};
export const PUBLISHERS_SUBMISSION_ROW: ICreatorPublisherSubmissionRow = {
  name: '',
  nameNumber: '',
  baseNumber: '',
  role: 'E',
};
export const AGENCY_WORK_CODES_SUBMISSION_ROW: IAgencyWorkCodesSubmissionRow = {
  agencyWorkCode: '',
  agencyName: '',
};
export const DERIVED_FROM_WORKS_SUBMISSION_ROW: IDerivedFromWorksSubmissionRow = {
  iswc: '',
  title: '',
};
export const PERFORMERS_SUBMISSION_ROW: IPerformerSubmissionRow = {
  firstName: '',
  lastName: '',
};
export const DISAMBIGUATE_FROM_ISWCS_ROW: IDisambiguateFromSubmissionRow = {
  iswc: '',
};
export const BLANK_OPTION: IDropdownOption = { value: '--', name: '--' };

// Submission Steps
export const SUBMISSION_MAIN_DETAILS_STEP: number = 1;
export const SUBMISSION_ADDITIONAL_DETAILS_STEP: number = 2;
export const SUBMISSION_SUCCESS_STEP: number = 3;

// Mock Objects
export const MOCK_GRID_HEADERS: IGridHeaderCell[] = [
  { text: 'Preferred ISWC', field: PREFERRED_ISWC_FIELD, sortable: true },
  { text: 'Original Title', field: ORIGINAL_TITLE_FIELD, sortable: true },
  { text: 'Creator Name(s)', field: CREATOR_NAMES_FIELD, sortable: true },
  { field: VIEW_MORE_FIELD },
];

export const SHOW_WORKFLOWS_ASSIGNED = 0;
export const SHOW_WORKFLOWS_CREATED = 1;

export const PENDING_WORKFLOWS = 0;
export const APPROVED_WORKFLOWS = 1;
export const REJECTED_WORKFLOWS = 2;

export const getAgencies = (): IAgency[] => {
  return getLookupData('Agency') as IAgency[];
};

export const getDropdownLookupData = (type: string): IDropdownOption[] => {
  const data = getLookupData(type);
  const dropdownData: IDropdownOption[] = [];
  for (let x = 0; x < data.length; x++) {
    dropdownData.push({ value: data[x].code, name: data[x].code });
  }
  return dropdownData.sort((a, b) => a.name.localeCompare(b.name));
};

const getLookupData = (tableName: string) => {
  const lookupData = localStorage.getItem('lookupData');
  return lookupData && JSON.parse(lookupData).find((x: any) => x.key === tableName).values;
};

export const BVLTR: IDropdownOption[] = [
  { value: 'Background', name: 'Background' },
  { value: 'Logo', name: 'Logo' },
  { value: 'Theme', name: 'Theme' },
  { value: 'Visual', name: 'Visual' },
  { value: 'RolledUpCue', name: 'Rolled Up Cue' },
];

export const DISAMBIGUATION_REASONS: IDropdownOption[] = [
  { value: 'DIT', name: 'DIT - Different Work' },
  { value: 'DIA', name: 'DIA - Different arrangement' },
  { value: 'DIE', name: 'DIE - Excerpt of another work' },
  { value: 'DIC', name: 'DIC - Different AV Work / Cue' },
  { value: 'DIV', name: 'DIV - Different version of a work with different shares' },
];

export const DISAMBIGUATION_REASONS_VIEW_MORE: IDropdownOption[] = [
  { value: 'DIT', name: 'DIT (Different Work)' },
  { value: 'DIA', name: 'DIA (Different arrangement)' },
  { value: 'DIE', name: 'DIE (Excerpt of another work)' },
  { value: 'DIC', name: 'DIC (Different AV Work / Cue)' },
  { value: 'DIV', name: 'DIV (Different version of a work with different shares)' },
];

const {
  JANUARY,
  FEBRUARY,
  MARCH,
  APRIL,
  MAY,
  JUNE,
  JULY,
  AUGUST,
  SEPTEMBER,
  OCTOBER,
  NOVEMBER,
  DECEMBER,
} = getStrings();

export const MONTHS: IDropdownOption[] = [
  { value: '1', name: JANUARY },
  { value: '2', name: FEBRUARY },
  { value: '3', name: MARCH },
  { value: '4', name: APRIL },
  { value: '5', name: MAY },
  { value: '6', name: JUNE },
  { value: '7', name: JULY },
  { value: '8', name: AUGUST },
  { value: '9', name: SEPTEMBER },
  { value: '10', name: OCTOBER },
  { value: '11', name: NOVEMBER },
  { value: '12', name: DECEMBER },
];

export const WORKFLOW_TYPE: IDropdownOption[] = [
  { value: '--', name: '--' },
  { value: '0', name: 'UpdateApproval' },
  { value: '1', name: 'MergeApproval' },
  { value: '2', name: 'DemergeApproval' },
];
