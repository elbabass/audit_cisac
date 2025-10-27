import React from 'react';
import { IGridHeaderCell, IGridRow } from '../../../components/GridComponents/Grid/GridTypes';
import {
  ALL_TITLES_FIELD,
  TYPE_FIELD,
  CREATOR_NAMES_FIELD,
  IP_NAME_NUMBER_FIELD,
  AFFILIATION_FIELD,
  IP_BASE_NUMBER_FIELD,
  ROLE_FIELD,
  AGENCY_CODE_FIELD,
  AGENCY_NAME_FIELD,
  AGENCY_WORK_CODE_FIELD,
  TITLE_FIELD,
  DERIVED_ISWC_FIELD,
  ISWC_FIELD,
  SOURCE_AGENCY_FIELD,
  REASON_FIELD,
  DISAMBIGUATED_FROM_FIELD,
  SURNAME_FIELD,
  NAME_FIELD,
  ISNI_FIELD,
  IPN_FIELD,
  FIRST_NAME_FIELD,
  SURNAME_BAND_NAME,
  DESIGNATION_FIELD,
  REMOVE_FIELD,
  REMOVE_ICON,
  MULTIPLE,
  NON_SOCIETY,
  DISAMBIGUATION_REASONS_VIEW_MORE,
  ROLLED_UP_ROLE,
  ISRC_FIELD,
  RECORDING_TITLE_FIELD,
  SUB_TITLE_FIELD,
  LABEL_NAME_FIELD,
  SUBMITTER_FIELD,
  SUBMITTER_TYPE_FIELD,
  SUBMITTER_WORK_NUMBER_FIELD,
  PERFORMERS,
} from '../../../consts';
import GridTextCell from '../../../components/GridComponents/GridTextCell/GridTextCell';
import {
  ITitle,
  IInterestedParty,
  IDisambiguateFrom,
  IPerformer,
  IRecording,
} from '../../../redux/types/IswcTypes';

import { getStrings } from '../../../configuration/Localization';
import {
  _getAgency,
  validateIswcAndFormat,
  padIpNameNumber,
  zeroPad,
  addSpaceToString,
  getLoggedInAgencyId,
  getRoleType,
} from '../../../shared/helperMethods';
import { IVerifiedSubmission } from '../../Submission/SubmissionTypes';
import GridIconCell from '../../../components/GridComponents/GridIconCell/GridIconCell';
import GridSubTable from '../../../components/GridComponents/GridSubTable/GridSubTable';

export const titleHeaderCells = (): IGridHeaderCell[] => {
  const strings = getStrings();
  return [
    { text: strings[ALL_TITLES_FIELD], field: ALL_TITLES_FIELD },
    { text: strings[TYPE_FIELD], field: TYPE_FIELD },
  ];
};

export const getTitleGridRows = (originalTitle: string, otherTitles?: ITitle[]): IGridRow[] => {
  let titleGridRows: IGridRow[] = [];

  titleGridRows.push({
    rowId: 0,
    cells: [
      {
        element: <GridTextCell text={originalTitle} />,
        field: ALL_TITLES_FIELD,
      },
      {
        element: <GridTextCell text={'OT'} />,
        field: TYPE_FIELD,
      },
    ],
  });

  if (otherTitles) {
    otherTitles.forEach((title, index) => {
      titleGridRows.push({
        rowId: index + 1,
        cells: [
          { element: <GridTextCell text={title.title} />, field: ALL_TITLES_FIELD },
          { element: <GridTextCell text={title.type.toString()} />, field: TYPE_FIELD },
        ],
      });
    });
  }

  return titleGridRows;
};

// Creators Grid Data
export const creatorHeaderCells = (): IGridHeaderCell[] => {
  const strings = getStrings();
  return [
    { text: strings[CREATOR_NAMES_FIELD], field: CREATOR_NAMES_FIELD },
    { text: strings[IP_NAME_NUMBER_FIELD], field: IP_NAME_NUMBER_FIELD },
    { text: strings[AFFILIATION_FIELD], field: AFFILIATION_FIELD },
    { text: strings[IP_BASE_NUMBER_FIELD], field: IP_BASE_NUMBER_FIELD, hideInPublicMode: true },
    { text: strings[ROLLED_UP_ROLE], field: ROLE_FIELD },
  ];
};

export const getCreatorGridRows = (interestedParties: IInterestedParty[]): IGridRow[] => {
  let creatorGridRows: IGridRow[] = [];
  interestedParties.forEach((ip, index) => {
    creatorGridRows.push({
      rowId: index,
      cells: [
        { element: <GridTextCell text={ip.name} />, field: CREATOR_NAMES_FIELD },
        {
          element: <GridTextCell text={padIpNameNumber(ip.nameNumber?.toString())} />,
          field: IP_NAME_NUMBER_FIELD,
        },
        {
          element: <GridTextCell text={getAffiliation(ip.affiliation)} />,
          field: AFFILIATION_FIELD,
        },
        {
          element: <GridTextCell text={ip.baseNumber} />,
          field: IP_BASE_NUMBER_FIELD,
          hideInPublicMode: true,
        },
        { element: <GridTextCell text={getRoleType(ip.role)} />, field: ROLE_FIELD },
      ],
    });
  });

  return creatorGridRows;
};

const getAffiliation = (affiliation?: string) => {
  if (!affiliation) return '';

  const strings = getStrings();
  if (affiliation === 'Multiple') return strings[MULTIPLE];
  else if (affiliation === '099') return strings[NON_SOCIETY];
  else return _getAgency(affiliation);
};

// Agency Work Codes Grid Data
export const societyWorkCodesHeaderCells = (
  works: IVerifiedSubmission[] | undefined,
  hideDeletionButton?: boolean,
): IGridHeaderCell[] => {
  if (!works) return [];
  const strings = getStrings();
  let headerCells = [
    { text: strings[AGENCY_CODE_FIELD], field: AGENCY_CODE_FIELD },
    { text: strings[AGENCY_NAME_FIELD], field: AGENCY_NAME_FIELD },
    { text: strings[AGENCY_WORK_CODE_FIELD], field: AGENCY_WORK_CODE_FIELD },
  ];

  return addDeleteSubmissionHeader(headerCells, works, hideDeletionButton);
};

const addDeleteSubmissionHeader = (
  headerCells: IGridHeaderCell[],
  works: IVerifiedSubmission[],
  hideDeletionButton?: boolean,
): IGridHeaderCell[] => {
  if (!hideDeletionButton) {
    for (let x = 0; x < works.length; x++) {
      if (works[x].agency === getLoggedInAgencyId()) {
        headerCells.push({
          field: REMOVE_FIELD,
        });
        break;
      }
    }
  }
  return headerCells;
};

export const getSocietyWorkCodesGridRows = (
  works: IVerifiedSubmission[],
  hideDeletionButton?: boolean,
  toggleDeleteModal?: () => void,
  setDeletionData?: (
    preferredIswc: string,
    agency: string,
    workcode: string,
    reasonCode: string,
    rowIndex: number,
  ) => void,
): IGridRow[] => {
  let workCodeGridRows: IGridRow[] = [];

  works.forEach((work, index) => {
    workCodeGridRows.push({
      rowId: index,
      cells: [
        {
          element: <GridTextCell text={zeroPad(parseInt(work.agency))} />,
          field: AGENCY_CODE_FIELD,
        },
        {
          element: <GridTextCell text={_getAgency(work.agency)} />,
          field: AGENCY_NAME_FIELD,
        },
        {
          element: <GridTextCell text={work.workcode} />,
          field: AGENCY_WORK_CODE_FIELD,
        },
      ],
    });

    if (!hideDeletionButton) {
      let addDeleteCell = false;
      works.forEach((work) => {
        if (work.agency === getLoggedInAgencyId()) {
          return (addDeleteCell = true);
        }
      });

      if (addDeleteCell) {
        if (work.agency === getLoggedInAgencyId()) {
          addDeleteSubmissionCell(
            workCodeGridRows[index],
            work,
            toggleDeleteModal,
            setDeletionData,
          );
          addDeleteAndReSubmissionCell(
            workCodeGridRows[index],
            work,
            toggleDeleteModal,
            setDeletionData,
          );
        } else {
          workCodeGridRows[index].cells.push({ element: <div />, field: REMOVE_FIELD });
        }
      }
    }
  });

  return workCodeGridRows;
};

const addDeleteAndReSubmissionCell = (
  row: IGridRow,
  work: IVerifiedSubmission,
  toggleDeleteModal?: () => void,
  setDeletionData?: (
    preferredIswc: string,
    agency: string,
    workcode: string,
    reasonCode: string,
    index: number,
    isResubmit : boolean,
  ) => void,
) => {
  const { DELETE_AND_RESUBMIT } = getStrings();
  row.cells.push({
    element: (
      <div
        onClick={() => {
          toggleDeleteModal && toggleDeleteModal();
          setDeletionData &&
            setDeletionData(work.iswc, work.agency, work.workcode, 'Portal Deletion', row.rowId,true);
        }}
      >
        <GridIconCell
          icon={REMOVE_ICON}
          alt={'Delete & Resubmit work button'}
          text={DELETE_AND_RESUBMIT}
          clickable
          id={DELETE_AND_RESUBMIT}
        />
      </div>
    ),
    field: REMOVE_FIELD,
  });
};

const addDeleteSubmissionCell = (
  row: IGridRow,
  work: IVerifiedSubmission,
  toggleDeleteModal?: () => void,
  setDeletionData?: (
    preferredIswc: string,
    agency: string,
    workcode: string,
    reasonCode: string,
    index: number,
    isResubmit: boolean,
  ) => void,
) => {
  const { DELETE } = getStrings();
  row.cells.push({
    element: (
      <div
        onClick={() => {
          toggleDeleteModal && toggleDeleteModal();
          setDeletionData &&
            setDeletionData(work.iswc, work.agency, work.workcode, 'Portal Deletion', row.rowId,false);
        }}
      >
        <GridIconCell
          icon={REMOVE_ICON}
          alt={'Delete work button'}
          text={DELETE}
          clickable
          id={DELETE}
        />
      </div>
    ),
    field: REMOVE_FIELD,
  });
};

// Derived Iswc grid data
export const derivedIswcsHeaderCells = (): IGridHeaderCell[] => {
  const strings = getStrings();
  return [
    { text: strings[TITLE_FIELD], field: TITLE_FIELD },
    { text: strings[DERIVED_ISWC_FIELD], field: DERIVED_ISWC_FIELD },
    { text: strings[TYPE_FIELD], field: TYPE_FIELD },
  ];
};

export const getDerivedIswcsGridRows = (works: IVerifiedSubmission[]): IGridRow[] => {
  let derivedIswcGridRows: IGridRow[] = [];
  const eligibleWorks = works.filter((x) => x.iswcEligible);

  eligibleWorks.sort((a, b) => {
    const dateA: any = new Date(a.lastModifiedDate);
    const dateB: any = new Date(b.lastModifiedDate);

    return dateB - dateA;
  });

  for (let x = 0; x < eligibleWorks.length; x++) {
    if (
      eligibleWorks[x].derivedFromIswcs !== undefined &&
      eligibleWorks[x].derivedFromIswcs!.length > 0
    ) {
      let startingRowId = 0;
      for (let i = 0; i < eligibleWorks[x].derivedFromIswcs!.length; i++) {
        derivedIswcGridRows.push({
          rowId: startingRowId,
          cells: [
            {
              element: <GridTextCell text={eligibleWorks[x].derivedFromIswcs![i].title} />,
              field: TITLE_FIELD,
            },
            {
              element: (
                <GridTextCell
                  text={validateIswcAndFormat(eligibleWorks[x].derivedFromIswcs![i].iswc)}
                />
              ),
              field: ISWC_FIELD,
            },
            {
              element: <GridTextCell text={addSpaceToString(eligibleWorks[x].derivedWorkType)} />,
              field: TYPE_FIELD,
            },
          ],
        });
        startingRowId++;
      }
      break;
    }
  }
  return derivedIswcGridRows;
};

export const disambiguateFromHeaderCells = (): IGridHeaderCell[] => {
  const strings = getStrings();
  return [
    { text: strings[SOURCE_AGENCY_FIELD], field: SOURCE_AGENCY_FIELD },
    { text: strings[REASON_FIELD], field: REASON_FIELD },
    { text: strings[DISAMBIGUATED_FROM_FIELD], field: DISAMBIGUATED_FROM_FIELD },
  ];
};

const getDisambiguatedFromIswcs = (disambiguatedFrom?: IDisambiguateFrom[]) => {
  let disambigautedFromIswcs: string[] = [];
  if (disambiguatedFrom !== undefined) {
    disambiguatedFrom.forEach((df) =>
      disambigautedFromIswcs.push(' ' + validateIswcAndFormat(df.iswc)),
    );
  }

  return disambigautedFromIswcs.join();
};

const getDisambiguationReason = (reasonCode?: string): string => {
  if (!reasonCode) return '';

  for (let x = 0; x < DISAMBIGUATION_REASONS_VIEW_MORE.length; x++) {
    if (DISAMBIGUATION_REASONS_VIEW_MORE[x].value === reasonCode)
      return DISAMBIGUATION_REASONS_VIEW_MORE[x].name;
  }

  return '';
};

export const getDisambiguationGridRows = (works: IVerifiedSubmission[]): IGridRow[] => {
  let disambiguateFromGridRows: IGridRow[] = [];

  works.forEach((work, index) => {
    if (work.disambiguation && work.disambiguateFrom !== undefined)
      disambiguateFromGridRows.push({
        rowId: index,
        cells: [
          {
            element: <GridTextCell text={_getAgency(work.agency)} />,
            field: SOURCE_AGENCY_FIELD,
          },
          {
            element: <GridTextCell text={getDisambiguationReason(work.disambiguationReason)} />,
            field: REASON_FIELD,
          },
          {
            element: <GridTextCell text={getDisambiguatedFromIswcs(work.disambiguateFrom)} />,
            field: DISAMBIGUATED_FROM_FIELD,
          },
        ],
      });
  });
  return disambiguateFromGridRows;
};

export const performerHeaderCells = (): IGridHeaderCell[] => {
  const strings = getStrings();
  return [
    { text: strings[ISNI_FIELD], field: ISNI_FIELD },
    { text: strings[IPN_FIELD], field: IPN_FIELD },
    { text: strings[SURNAME_BAND_NAME], field: SURNAME_FIELD },
    { text: strings[NAME_FIELD], field: NAME_FIELD },
    { text: strings[DESIGNATION_FIELD], field: DESIGNATION_FIELD },
  ];
};

export const recordingHeaderCells = (): IGridHeaderCell[] => {
  const strings = getStrings();
  return [
    { text: strings[ISRC_FIELD], field: ISRC_FIELD },
    { text: strings[RECORDING_TITLE_FIELD], field: RECORDING_TITLE_FIELD },
    { text: strings[SUB_TITLE_FIELD], field: SUB_TITLE_FIELD },
    { text: strings[LABEL_NAME_FIELD], field: LABEL_NAME_FIELD },
    { text: strings[SUBMITTER_FIELD], field: SUBMITTER_FIELD },
    { text: strings[SUBMITTER_TYPE_FIELD], field: SUBMITTER_TYPE_FIELD },
    { text: strings[SUBMITTER_WORK_NUMBER_FIELD], field: SUBMITTER_WORK_NUMBER_FIELD }
  ];
}

export const getPerformerGridRows = (performers: IPerformer[]): IGridRow[] => {
  let performerGridRows: IGridRow[] = [];

  performers?.forEach((performer, index) => {
    performerGridRows.push({
      rowId: index,
      cells: [
        { 
          element: <GridTextCell text={performer.isni} />, 
          field: ISNI_FIELD 
        },
        { 
          element: <GridTextCell text={performer.ipn?.toString()} />, 
          field: IPN_FIELD 
        },
        {
          element: <GridTextCell text={performer.lastName} />,
          field: SURNAME_FIELD,
        },
        { 
          element: <GridTextCell text={performer.firstName} />, 
          field: FIRST_NAME_FIELD 
        },
        {
          element: <GridTextCell text={performer.designation} />,
          field: DESIGNATION_FIELD,
        },
      ],
    });
  });

  return performerGridRows;
};

export const getRecordingGridRows = (recordings: IRecording[]): IGridRow[] => {
  const { PERFORMERS } = getStrings();
  let recordingGridRows: IGridRow[] = [];

  recordings.forEach((recording, index) => {
    recordingGridRows.push({
      rowId: index,
      cells: [
        {
          element: <GridTextCell text={recording.isrc} />,
          field: ISRC_FIELD,
        },
        {
          element: <GridTextCell text={recording.recordingTitle} />,
          field: RECORDING_TITLE_FIELD,
        },
        {
          element: <GridTextCell text={recording.subTitle} />,
          field: SUB_TITLE_FIELD,
        },
        {
          element: <GridTextCell text={recording.labelName} />,
          field: LABEL_NAME_FIELD,
        },
        {
          element: <GridTextCell text={recording.submitter} />,
          field: SUBMITTER_FIELD,
        },
        {
          element: <GridTextCell text={recording.submitterType} />,
          field: SUBMITTER_TYPE_FIELD,
        },
        {
          element: <GridTextCell text={recording.submitterWorkNumber} />,
          field: SUBMITTER_WORK_NUMBER_FIELD,
        }
      ],
      subTable: recording.performers && 
        <GridSubTable
          title={PERFORMERS}
          headerCells={performerHeaderCells()}
          rows={getPerformerGridRows(recording.performers)} />
    });
  });

  return recordingGridRows;
}
