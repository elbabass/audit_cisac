import React from 'react';
import { IGridHeaderCell, IGridRow } from '../../../components/GridComponents/Grid/GridTypes';
import {
  TYPE_FIELD,
  TITLE_FIELD,
  REMOVE_FIELD,
  REMOVE_ICON,
  IP_NAME_NUMBER_FIELD,
  IP_BASE_NUMBER_FIELD,
  ROLE_FIELD,
  NAME_FIELD,
  AGENCY_NAME_FIELD,
  IP_LOOKUP_ACTION,
  SEARCH_ICON,
  DERIVED_WORK_ISWC_FIELD,
  DERIVED_WORK_TITLE_FIELD,
  LOOKUP_ICON,
  AGENCY_WORK_CODE_FIELD,
  OR,
  REMOVE,
  getDropdownLookupData,
  LOOKUP,
  LAST_NAME_FIELD,
  FIRST_NAME_FIELD,
  SURNAME_BAND_NAME,
  FIRST_NAME,
  SEARCH,
} from '../../../consts';
import GridInputCell from '../../../components/GridComponents/GridInputCell/GridInputCell';
import GridDropdownCell from '../../../components/GridComponents/GridDropdownCell/GridDropdownCell';
import GridTextCell from '../../../components/GridComponents/GridTextCell/GridTextCell';
import GridIconCell from '../../../components/GridComponents/GridIconCell/GridIconCell';
import styles from './SubmissionMainDetails.module.scss';
import IpLookup from '../IpLookup/IpLookup';
import {
  ISubmissionStateKeys,
  ITitleSubmissionRow,
  ISubmissionMainDetailsStateObjectKeys,
  ICreatorPublisherSubmissionRow,
  IAgencyWorkCodesSubmissionRow,
  IDerivedFromWorksSubmissionRow,
  IPerformerSubmissionRow,
} from '../SubmissionTypes';
import { getStrings } from '../../../configuration/Localization';
import NameNumberLookup from '../IpLookup/NameNumberLookup';
import {
  padIpNameNumber,
  validateIswcAndFormat,
  _getAgency,
  legalEntityMessage,
} from '../../../shared/helperMethods';

const strings = getStrings();

// Titles Grid Data
export const submissionTitlesHeaderCells: IGridHeaderCell[] = [
  { text: strings[TYPE_FIELD], field: TYPE_FIELD },
  { text: strings[TITLE_FIELD], field: TITLE_FIELD },
  { field: REMOVE_FIELD },
];

export const submissionTitlesGridRows = (
  titles: ITitleSubmissionRow[],
  updateSubmissionDataArray: (
    value: string,
    rowId: number,
    key: ISubmissionStateKeys,
    objectKey: ISubmissionMainDetailsStateObjectKeys,
  ) => void,
  removeElementFromArray: (key: ISubmissionStateKeys, rowId: number) => void,
): IGridRow[] => {
  let titlesGridRows: IGridRow[] = [];

  titlesGridRows.push({
    rowId: 0,
    cells: [
      {
        element: <GridTextCell text={titles[0].type} />,
        field: TYPE_FIELD,
      },
      {
        element: (
          <GridInputCell
            value={titles[0].title}
            onChange={(text: string) => updateSubmissionDataArray(text, 0, 'titles', 'title')}
          />
        ),
        field: TITLE_FIELD,
      },
      {
        element: <div style={{ visibility: 'hidden' }} />,
        field: REMOVE_FIELD,
      },
    ],
  });

  titles.forEach((title, index) => {
    if (index === 0) return;

    titlesGridRows.push({
      rowId: index,
      cells: [
        {
          element: (
            <GridDropdownCell
              options={getDropdownLookupData('TitleType')}
              value={title.type}
              selectNewOption={(text: string) =>
                updateSubmissionDataArray(text, index, 'titles', 'type')
              }
            />
          ),
          field: TYPE_FIELD,
        },
        {
          element: (
            <GridInputCell
              value={title.title}
              onChange={(text: string) => updateSubmissionDataArray(text, index, 'titles', 'title')}
            />
          ),
          field: TITLE_FIELD,
        },
        {
          element: (
            <div onClick={() => removeElementFromArray('titles', index)}>
              <GridIconCell
                icon={REMOVE_ICON}
                text={strings[REMOVE]}
                alt={'Remove Icon'}
                clickable
                id={strings[REMOVE]}
              />
            </div>
          ),
          field: REMOVE_FIELD,
        },
      ],
    });
  });

  return titlesGridRows;
};

// Creators Grid Data
export const submissionCreatorsHeaderCells: IGridHeaderCell[] = [
  { text: strings[NAME_FIELD], field: NAME_FIELD },
  { text: strings[IP_NAME_NUMBER_FIELD], field: IP_NAME_NUMBER_FIELD },
  { text: strings[IP_BASE_NUMBER_FIELD], field: IP_BASE_NUMBER_FIELD },
  { text: strings[ROLE_FIELD], field: ROLE_FIELD },
  { field: REMOVE_FIELD },
];

export const submissionCreatorsGridRows = (
  creators: ICreatorPublisherSubmissionRow[],
  updateSubmissionDataArray: (
    value: string,
    rowId: number,
    key: ISubmissionStateKeys,
    objectKey: ISubmissionMainDetailsStateObjectKeys,
  ) => void,
  removeElementFromArray: (key: ISubmissionStateKeys, rowId: number) => void,
  addElementToSubmissionDataArray: (element: any, rowId: number, key: ISubmissionStateKeys) => void,
): IGridRow[] => {
  let creatorsGridRows: IGridRow[] = [];

  creators.forEach((creator, index) => {
    creatorsGridRows.push({
      rowId: index,
      cells: [
        {
          element: <GridTextCell text={creator.name} />,
          field: NAME_FIELD,
        },
        {
          element: creator.baseNumber ? (
            <GridTextCell text={padIpNameNumber(creator.nameNumber)} />
          ) : (
            <div className={styles.lookupGridCell}>
              <div className={styles.nameNumberContainer}>
                <GridInputCell
                  value={creator.nameNumber}
                  onChange={(text: string) =>
                    updateSubmissionDataArray(
                      text.replace(/[^0-9]/g, ''),
                      index,
                      'creators',
                      'nameNumber',
                    )
                  }
                  noRightBorder
                />
                <div className={styles.nameNumberSearch}>
                  <GridIconCell icon={SEARCH_ICON} alt={'Search icon'} clickable id={SEARCH} />
                </div>
              </div>
              <div className={styles.lookupButtonDiv}>
                {strings[OR]}
                <div className={styles.gridIconCellDiv}>
                  <GridIconCell
                    icon={LOOKUP_ICON}
                    text={strings[LOOKUP]}
                    id={strings[LOOKUP]}
                    alt={'Lookup Icon'}
                    clickable
                    largerIcon
                  />
                </div>
              </div>
            </div>
          ),
          field: IP_NAME_NUMBER_FIELD,
          // Handled in GridRow.tsx*
          action: !creator.baseNumber ? IP_LOOKUP_ACTION : '',
        },
        {
          element: <GridTextCell text={creator.baseNumber} />,
          field: IP_BASE_NUMBER_FIELD,
        },
        {
          element: (
            <GridDropdownCell
              options={[
                { value: 'CA', name: 'CA' },
                { value: 'C', name: 'C' },
                { value: 'A', name: 'A' },
                { value: 'SA', name: 'SA' },
                { value: 'AD', name: 'AD' },
                { value: 'TR', name: 'TR' },
                { value: 'SR', name: 'SR' },
                { value: 'AR', name: 'AR' },
              ]}
              value={creator.role}
              selectNewOption={(text: string) =>
                updateSubmissionDataArray(text, index, 'creators', 'role')
              }
            />
          ),
          field: ROLE_FIELD,
        },
        {
          element: (
            <div onClick={() => removeElementFromArray('creators', index)}>
              <GridIconCell
                icon={REMOVE_ICON}
                text={strings[REMOVE]}
                alt={'Remove Icon'}
                clickable
                id={strings[REMOVE]}
              />
            </div>
          ),
          field: REMOVE_FIELD,
        },
      ],
      viewMore: [
        <IpLookup
          ipType={2}
          nameNumber={creator.nameNumber}
          addElementToSubmissionDataArray={addElementToSubmissionDataArray}
          rowId={index}
        />,
        <NameNumberLookup
          ipType={2}
          nameNumber={creator.nameNumber}
          addElementToSubmissionDataArray={addElementToSubmissionDataArray}
          rowId={index}
        />,
      ],
      message: legalEntityMessage(creator.legalEntityType, true),
    });
  });

  return creatorsGridRows;
};

// Publishers Grid Data
export const submissionPublishersHeaderCells: IGridHeaderCell[] = [
  { text: strings[NAME_FIELD], field: NAME_FIELD },
  { text: strings[IP_NAME_NUMBER_FIELD], field: IP_NAME_NUMBER_FIELD },
  { text: strings[IP_BASE_NUMBER_FIELD], field: IP_BASE_NUMBER_FIELD },
  { text: strings[ROLE_FIELD], field: ROLE_FIELD },
  { field: REMOVE_FIELD },
];

export const submissionPublishersGridRows = (
  publishers: ICreatorPublisherSubmissionRow[],
  updateSubmissionDataArray: (
    value: string,
    rowId: number,
    key: ISubmissionStateKeys,
    objectKey: ISubmissionMainDetailsStateObjectKeys,
  ) => void,
  removeElementFromArray: (key: ISubmissionStateKeys, rowId: number) => void,
  addElementToSubmissionDataArray: (element: any, rowId: number, key: ISubmissionStateKeys) => void,
): IGridRow[] => {
  let publishersGridRows: IGridRow[] = [];

  publishers.forEach((publisher, index) => {
    publishersGridRows.push({
      rowId: index,
      cells: [
        {
          element: <GridTextCell text={publisher.name} />,
          field: NAME_FIELD,
        },
        {
          element: publisher.baseNumber ? (
            <GridTextCell text={padIpNameNumber(publisher.nameNumber)} />
          ) : (
            <div className={styles.lookupGridCell}>
              <div className={styles.nameNumberContainer}>
                <GridInputCell
                  value={publisher.nameNumber}
                  onChange={(text: string) =>
                    updateSubmissionDataArray(
                      text.replace(/[^0-9]/g, ''),
                      index,
                      'publishers',
                      'nameNumber',
                    )
                  }
                  noRightBorder
                />
                <div className={styles.nameNumberSearch}>
                  <GridIconCell icon={SEARCH_ICON} alt={'Search icon'} clickable id={SEARCH} />
                </div>
              </div>
              <div className={styles.lookupButtonDiv}>
                {strings[OR]}
                <div className={styles.gridIconCellDiv}>
                  <GridIconCell
                    icon={LOOKUP_ICON}
                    text={strings[LOOKUP]}
                    alt={'Lookup Icon'}
                    id={strings[LOOKUP]}
                    clickable
                    largerIcon
                  />
                </div>
              </div>
            </div>
          ),
          field: IP_NAME_NUMBER_FIELD,
          // Handled in GridRow.tsx*
          action: !publisher.baseNumber ? IP_LOOKUP_ACTION : '',
        },
        {
          element: <GridTextCell text={publisher.baseNumber} />,
          field: IP_BASE_NUMBER_FIELD,
        },
        {
          element: (
            <GridDropdownCell
              options={[
                { value: 'E', name: 'E' },
                { value: 'AM', name: 'AM' },
              ]}
              value={publisher.role}
              selectNewOption={(text: string) =>
                updateSubmissionDataArray(text, index, 'publishers', 'role')
              }
            />
          ),
          field: ROLE_FIELD,
        },
        {
          element: (
            <div onClick={() => removeElementFromArray('publishers', index)}>
              <GridIconCell
                icon={REMOVE_ICON}
                text={strings[REMOVE]}
                alt={'Remove Icon'}
                clickable
                id={strings[REMOVE]}
              />
            </div>
          ),
          field: REMOVE_FIELD,
        },
      ],
      viewMore: [
        <IpLookup
          ipType={1}
          nameNumber={publisher.nameNumber}
          addElementToSubmissionDataArray={addElementToSubmissionDataArray}
          rowId={index}
        />,
        <NameNumberLookup
          ipType={1}
          nameNumber={publisher.nameNumber}
          addElementToSubmissionDataArray={addElementToSubmissionDataArray}
          rowId={index}
        />,
      ],
      message: legalEntityMessage(publisher.legalEntityType),
    });
  });

  return publishersGridRows;
};

// Agency Work Codes Grid Data
export const submissionAgencyWorkCodesHeaderCells: IGridHeaderCell[] = [
  { text: strings[AGENCY_WORK_CODE_FIELD], field: AGENCY_WORK_CODE_FIELD },
  { text: strings[AGENCY_NAME_FIELD], field: AGENCY_NAME_FIELD },
  { field: REMOVE_FIELD },
];

export const submissionAgencyWorkCodesGridRows = (
  agencyWorkCodes: IAgencyWorkCodesSubmissionRow[],
  updateSubmissionDataArray: (
    value: string,
    rowId: number,
    key: ISubmissionStateKeys,
    objectKey: ISubmissionMainDetailsStateObjectKeys,
  ) => void,
  removeElementFromArray: (key: ISubmissionStateKeys, rowId: number) => void,
  updateInstance?: boolean,
): IGridRow[] => {
  let agencyWorkCodesGridRows: IGridRow[] = [];

  agencyWorkCodes.forEach((agencyWorkCode, index) => {
    agencyWorkCodesGridRows.push({
      rowId: index,
      cells: [
        {
          element: updateInstance ? (
            <GridTextCell text={agencyWorkCode.agencyWorkCode} />
          ) : (
            <GridInputCell
              value={agencyWorkCode.agencyWorkCode}
              onChange={(text: string) =>
                updateSubmissionDataArray(text, index, 'agencyWorkCode', 'agencyWorkCode')
              }
            />
          ),
          field: AGENCY_WORK_CODE_FIELD,
        },
        {
          element: <GridTextCell text={_getAgency(agencyWorkCode.agencyName)} />,
          field: AGENCY_NAME_FIELD,
        },
        {
          element: (
            <div
              style={{ visibility: 'hidden' }}
              onClick={() => removeElementFromArray('agencyWorkCode', index)}
            >
              <GridIconCell
                icon={REMOVE_ICON}
                text={strings[REMOVE]}
                alt={'Remove Icon'}
                clickable
                id={strings[REMOVE]}
              />
            </div>
          ),
          field: REMOVE_FIELD,
        },
      ],
    });
  });

  return agencyWorkCodesGridRows;
};

// Derived From Work Grid Data
export const submissionDerivedFromHeaderCells: IGridHeaderCell[] = [
  { text: strings[DERIVED_WORK_ISWC_FIELD], field: DERIVED_WORK_ISWC_FIELD },
  { text: strings[DERIVED_WORK_TITLE_FIELD], field: DERIVED_WORK_TITLE_FIELD },
  { field: REMOVE_FIELD },
];

export const submissionDerivedFromGridRows = (
  derivedFromWorks: IDerivedFromWorksSubmissionRow[],
  updateSubmissionDataArray: (
    value: string,
    rowId: number,
    key: ISubmissionStateKeys,
    objectKey: ISubmissionMainDetailsStateObjectKeys,
  ) => void,
  removeElementFromArray: (key: ISubmissionStateKeys, rowId: number) => void,
): IGridRow[] => {
  let derivedFromGridRows: IGridRow[] = [];

  derivedFromWorks.forEach((derivedFromWork, index) => {
    derivedFromGridRows.push({
      rowId: index,
      cells: [
        {
          element: (
            <GridInputCell
              value={validateIswcAndFormat(derivedFromWork.iswc)}
              onChange={(text: string) =>
                updateSubmissionDataArray(text, index, 'derivedFromWorks', 'iswc')
              }
            />
          ),
          field: DERIVED_WORK_ISWC_FIELD,
        },
        {
          element: (
            <GridInputCell
              value={derivedFromWork.title}
              onChange={(text: string) =>
                updateSubmissionDataArray(text, index, 'derivedFromWorks', 'title')
              }
            />
          ),
          field: DERIVED_WORK_TITLE_FIELD,
        },
        {
          element: (
            <div onClick={() => removeElementFromArray('derivedFromWorks', index)}>
              <GridIconCell
                icon={REMOVE_ICON}
                text={strings[REMOVE]}
                alt={'Remove Icon'}
                clickable
                id={strings[REMOVE]}
              />
            </div>
          ),
          field: REMOVE_FIELD,
        },
      ],
    });
  });

  return derivedFromGridRows;
};

// Performers Grid data
export const performersHeaderCells = [
  { text: strings[SURNAME_BAND_NAME], field: LAST_NAME_FIELD },
  { text: strings[FIRST_NAME], field: FIRST_NAME },
  { field: REMOVE_FIELD },
];

export const submissionPerformersGridRows = (
  performers: IPerformerSubmissionRow[],
  updateSubmissionDataArray: (
    value: string,
    rowId: number,
    key: ISubmissionStateKeys,
    objectKey: ISubmissionMainDetailsStateObjectKeys,
  ) => void,
  removeElementFromArray: (key: ISubmissionStateKeys, rowId: number) => void,
): IGridRow[] => {
  let performersGridRows: IGridRow[] = [];
  const strings = getStrings();
  performers?.forEach((performer, index) => {
    performersGridRows.push({
      rowId: index,
      cells: [
        {
          element: (
            <GridInputCell
              value={performer.lastName}
              onChange={(text: string) =>
                updateSubmissionDataArray(text, index, 'performers', 'lastName')
              }
            />
          ),
          field: LAST_NAME_FIELD,
        },
        {
          element: (
            <GridInputCell
              value={performer.firstName}
              onChange={(text: string) =>
                updateSubmissionDataArray(text, index, 'performers', 'firstName')
              }
            />
          ),
          field: FIRST_NAME_FIELD,
        },
        {
          element: (
            <div onClick={() => removeElementFromArray('performers', index)}>
              <GridIconCell
                icon={REMOVE_ICON}
                text={strings[REMOVE]}
                alt={'Remove Icon'}
                clickable
                id={strings[REMOVE]}
              />
            </div>
          ),
          field: REMOVE_FIELD,
        },
      ],
    });
  });

  return performersGridRows;
};
