import * as React from 'react';
import Grid from '../../../components/GridComponents/Grid/Grid';
import {
  ISWC_FIELD,
  TITLE_FIELD,
  VIEW_MORE_FIELD,
  VIEW_MORE_ICON,
  VIEW_MORE_ACTION,
  PREFERRED_ISWC_ASSIGNED,
  SUBMISSION_SUCCESSFUL,
  SUBMISSION_SUCCESSFUL_SELECTED_PREF,
  SUBMISSION_SUCCESSFUL_UPDATE,
} from '../../../consts';
import GridTextCell from '../../../components/GridComponents/GridTextCell/GridTextCell';
import GridIconCell from '../../../components/GridComponents/GridIconCell/GridIconCell';
import { ISubmissionSuccessProps } from './SubmissionSuccessTypes';
import styles from './SubmissionSuccess.module.scss';
import { IIswcModel } from '../../../redux/types/IswcTypes';
import { mapVerifiedSubmissionToIswcModel } from '../../../shared/MappingObjects';
import ViewMore from '../../Search/ViewMore/ViewMore';
import { getStrings } from '../../../configuration/Localization';
import { validateIswcAndFormat } from '../../../shared/helperMethods';

const SubmissionSuccess: React.FunctionComponent<ISubmissionSuccessProps> = ({
  verifiedSubmission,
  updateInstance,
  newIswc,
}) => {
  const strings = getStrings();

  const _getGridHeaderCells = () => {
    return [
      { text: strings[ISWC_FIELD], field: ISWC_FIELD, sortable: true },
      { text: strings[TITLE_FIELD], field: TITLE_FIELD, sortable: true },
      { field: VIEW_MORE_FIELD },
    ];
  };

  const _getGridRows = () => {
    const iswc: IIswcModel = mapVerifiedSubmissionToIswcModel(verifiedSubmission);
    return [
      {
        rowId: 0,
        cells: [
          {
            element: <GridTextCell text={validateIswcAndFormat(verifiedSubmission.iswc)} />,
            field: ISWC_FIELD,
          },
          { element: <GridTextCell text={verifiedSubmission.originalTitle} />, field: TITLE_FIELD },
          {
            element: (
              <GridIconCell
                text={strings[VIEW_MORE_FIELD]}
                icon={VIEW_MORE_ICON}
                alt={'View More Icon'}
                clickable
                id={strings[VIEW_MORE_FIELD]}
              />
            ),
            field: VIEW_MORE_FIELD,
            // Handled in GridRow.tsx*
            action: VIEW_MORE_ACTION,
          },
        ],
        viewMore: [<ViewMore iswcModel={iswc} isSubmissionGrid />],
      },
    ];
  };

  const renderSuccessMessage = () => {
    if (updateInstance) {
      return (
        strings[SUBMISSION_SUCCESSFUL_UPDATE] + ` ${validateIswcAndFormat(verifiedSubmission.iswc)}`
      );
    }
    if (newIswc) {
      return strings[SUBMISSION_SUCCESSFUL];
    }

    return (
      strings[SUBMISSION_SUCCESSFUL_SELECTED_PREF] +
      ` ${validateIswcAndFormat(verifiedSubmission.iswc)}`
    );
  };

  return (
    <div className={styles.container}>
      <div className={styles.sectionTitle}>{strings[PREFERRED_ISWC_ASSIGNED]}</div>
      <div className={styles.description}>{renderSuccessMessage()}</div>
      <div className={styles.grid}>
        <Grid
          headerCells={_getGridHeaderCells()}
          gridRows={_getGridRows()}
          cellPadding={'25px 25px 25px 10px'}
          headerColor={'#f7f7f7'}
        />
      </div>
    </div>
  );
};

export default React.memo(SubmissionSuccess);
