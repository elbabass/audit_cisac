import * as React from 'react';
import styles from './SubmissionMainDetails.module.scss';
import { ISubmissionMainDetailsProps } from './SubmissionMainDetailsTypes';
import SubmissionGrid from '../SubmissionGrid/SubmissionGrid';
import { IGridHeaderCell, IGridRow } from '../../../components/GridComponents/Grid/GridTypes';
import {
  submissionTitlesHeaderCells,
  submissionTitlesGridRows,
  submissionCreatorsHeaderCells,
  submissionCreatorsGridRows,
  submissionPublishersHeaderCells,
  submissionPublishersGridRows,
  submissionAgencyWorkCodesHeaderCells,
  submissionAgencyWorkCodesGridRows,
  submissionDerivedFromHeaderCells,
  submissionDerivedFromGridRows,
  performersHeaderCells,
  submissionPerformersGridRows,
} from './SubmissionMainDetailsGrids';
import {
  CREATORS_SUBMISSION_ROW,
  PUBLISHERS_SUBMISSION_ROW,
  TITLES_SUBMISSION_ROW,
  AGENCY_WORK_CODES_SUBMISSION_ROW,
  DERIVED_FROM_WORKS_SUBMISSION_ROW,
  TITLES_FIELD,
  CREATORS_FIELD,
  PUBLISHERS_FIELD,
  ADD_NEW_CREATOR,
  ADD_NEW_TITLE,
  AGENCY_WORK_CODES,
  DERIVED_FROM_WORK,
  ADD_NEW_DERIVED_WORK,
  ADD_NEW_PUBLISHER,
  IF_DERIVED_WORK,
  SELECT_TYPE,
  BLANK_OPTION,
  PERFORMERS,
  PERFORMERS_SUBMISSION_ROW,
  ADD_NEW_PERFORMER,
} from '../../../consts';
import { ISubmissionStateKeys } from '../SubmissionTypes';
import { getStrings } from '../../../configuration/Localization';
import GridDropdownCell from '../../../components/GridComponents/GridDropdownCell/GridDropdownCell';

const SubmissionMainDetails: React.FunctionComponent<ISubmissionMainDetailsProps> = ({
  titles,
  creators,
  publishers,
  agencyWorkCodes,
  derivedWorkType,
  derivedFromWorks,
  updateSubmissionDataString,
  updateSubmissionDataArray,
  addElementToSubmissionDataArray,
  removeElementFromArray,
  addElementToArray,
  updateInstance,
  performers,
}) => {
  const strings = getStrings();

  const renderSubmissionGrid = (
    title: string,
    headerCells: IGridHeaderCell[],
    rows: IGridRow[],
    key: ISubmissionStateKeys,
    row: any,
    showActionButton: boolean,
    actionButtonText?: string,
  ) => {
    return (
      <div className={styles.gridDiv}>
        <div className={styles.gridTitles}>{title}</div>
        <SubmissionGrid
          headerCells={headerCells}
          gridRows={rows}
          actionButtonText={actionButtonText}
          addRowToGrid={() => addElementToArray(key, row)}
          showActionButton={showActionButton}
        />
      </div>
    );
  };

  return (
    <div className={styles.container}>
      {renderSubmissionGrid(
        `${strings[TITLES_FIELD]}:`,
        submissionTitlesHeaderCells,
        submissionTitlesGridRows(titles, updateSubmissionDataArray, removeElementFromArray),
        'titles',
        TITLES_SUBMISSION_ROW,
        true,
        strings[ADD_NEW_TITLE],
      )}
      {renderSubmissionGrid(
        `${strings[CREATORS_FIELD]}:`,
        submissionCreatorsHeaderCells,
        submissionCreatorsGridRows(
          creators,
          updateSubmissionDataArray,
          removeElementFromArray,
          addElementToSubmissionDataArray,
        ),
        'creators',
        CREATORS_SUBMISSION_ROW,
        true,
        strings[ADD_NEW_CREATOR],
      )}
      {renderSubmissionGrid(
        `${strings[PUBLISHERS_FIELD]}:`,
        submissionPublishersHeaderCells,
        submissionPublishersGridRows(
          publishers,
          updateSubmissionDataArray,
          removeElementFromArray,
          addElementToSubmissionDataArray,
        ),
        'publishers',
        PUBLISHERS_SUBMISSION_ROW,
        true,
        strings[ADD_NEW_PUBLISHER],
      )}
      {renderSubmissionGrid(
        `${strings[AGENCY_WORK_CODES]}:`,
        submissionAgencyWorkCodesHeaderCells,
        submissionAgencyWorkCodesGridRows(
          agencyWorkCodes,
          updateSubmissionDataArray,
          removeElementFromArray,
          updateInstance,
        ),
        'agencyWorkCode',
        AGENCY_WORK_CODES_SUBMISSION_ROW,
        false,
        undefined,
      )}
      <div className={styles.gridTitles}>{strings[DERIVED_FROM_WORK]}:</div>
      <div className={styles.descriptionText}>{strings[IF_DERIVED_WORK]}</div>
      <div className={styles.descriptionTextBold}>{strings[SELECT_TYPE]}:</div>
      <div className={styles.dropdownDiv}>
        <GridDropdownCell
          options={[
            BLANK_OPTION,
            { value: 'ModifiedVersion', name: 'Modified Version' },
            { value: 'Composite', name: 'Composite' },
            { value: 'Excerpt', name: 'Excerpt' },
          ]}
          value={derivedWorkType}
          selectNewOption={(text: string) => updateSubmissionDataString(text, 'derivedWorkType')}
        />
      </div>
      {renderSubmissionGrid(
        '',
        submissionDerivedFromHeaderCells,
        submissionDerivedFromGridRows(
          derivedFromWorks,
          updateSubmissionDataArray,
          removeElementFromArray,
        ),
        'derivedFromWorks',
        DERIVED_FROM_WORKS_SUBMISSION_ROW,
        true,
        strings[ADD_NEW_DERIVED_WORK],
      )}
      {renderSubmissionGrid(
        `${strings[PERFORMERS]}:`,
        performersHeaderCells,
        submissionPerformersGridRows(performers, updateSubmissionDataArray, removeElementFromArray),
        'performers',
        PERFORMERS_SUBMISSION_ROW,
        true,
        strings[ADD_NEW_PERFORMER],
      )}
    </div>
  );
};

export default React.memo(SubmissionMainDetails);
