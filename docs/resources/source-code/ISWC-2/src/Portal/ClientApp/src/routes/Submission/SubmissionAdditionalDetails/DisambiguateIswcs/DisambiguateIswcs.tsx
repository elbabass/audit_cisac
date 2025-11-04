import * as React from 'react';
import { IDisambiguateIswcsProps, IDisambiguateIswcsState } from './DisambiguateIswcsTypes';
import GridInputCell from '../../../../components/GridComponents/GridInputCell/GridInputCell';
import GridDropdownCell from '../../../../components/GridComponents/GridDropdownCell/GridDropdownCell';
import Grid from '../../../../components/GridComponents/Grid/Grid';
import SubmissionGrid from '../../SubmissionGrid/SubmissionGrid';
import {
  ISWC_FIELD,
  TITLE_FIELD,
  VIEW_MORE_FIELD,
  VIEW_MORE_ICON,
  VIEW_MORE_ACTION,
  PERFORMERS_SUBMISSION_ROW,
  ISWCS_TO_DISAMIGUATE,
  ADDITIONAL_ISWCS_TO_DISAMBIGUATE,
  REASONS_FOR_DISAMBIGUATION,
  STANDARD_INSTRUMENTATION_FIELD,
  PERFORMERS,
  ADD_NEW_PERFORMER,
  getDropdownLookupData,
  BVLTR,
  DISAMBIGUATION_REASONS,
  BLANK_OPTION,
  CLOSE_ICON,
} from '../../../../consts';
import GridTextCell from '../../../../components/GridComponents/GridTextCell/GridTextCell';
import GridIconCell from '../../../../components/GridComponents/GridIconCell/GridIconCell';
import styles from './DisambiguateIswcs.module.scss';
import { IGridHeaderCell, IGridRow } from '../../../../components/GridComponents/Grid/GridTypes';
import { ISubmissionStateKeys } from '../../SubmissionTypes';
import {
  performersHeaderCells,
  submissionPerformersGridRows,
} from '../../SubmissionMainDetails/SubmissionMainDetailsGrids';
import { IIswcModel } from '../../../../redux/types/IswcTypes';
import ViewMore from '../../../Search/ViewMore/ViewMore';
import { getStrings } from '../../../../configuration/Localization';
import {
  validateIswcAndFormat,
  validateIswcAndFormatArray,
} from '../../../../shared/helperMethods';

export default class DisambiguateIswcs extends React.PureComponent<
  IDisambiguateIswcsProps,
  IDisambiguateIswcsState
> {
  constructor(props: IDisambiguateIswcsProps) {
    super(props);

    this.state = {
      loading: false,
      potentialMatches: props.potentialMatches.map((x) => {
        return { potentialMatch: x, iswcModel: undefined };
      }),
      openAccordion: false,
    };
  }

  _toggleAccordion = () => {
    const { openAccordion } = this.state;
    this.setState({
      openAccordion: !openAccordion,
    });
  };

  _getIswcModel = async (rowId: number, iswc: string) => {
    const { searchByIswc } = this.props;
    const iswcModel: IIswcModel[] = await searchByIswc(iswc);
    const potentialMatchesStateCopy = JSON.parse(JSON.stringify(this.state.potentialMatches));

    potentialMatchesStateCopy[rowId].iswcModel = iswcModel[0];

    this.setState({
      potentialMatches: potentialMatchesStateCopy,
    });
  };

  _getDisambiguationGridHeaders = () => {
    const strings = getStrings();
    return [
      { text: strings[ISWC_FIELD], field: ISWC_FIELD },
      { text: strings[TITLE_FIELD], field: TITLE_FIELD },
      { field: VIEW_MORE_FIELD },
    ];
  };

  _getDisambiguationGridRows = () => {
    const { potentialMatches } = this.state;
    let gridRows: IGridRow[] = [];
    const strings = getStrings();

    potentialMatches.forEach((match, index) => {
      gridRows.push({
        rowId: index,
        cells: [
          {
            element: <GridTextCell text={validateIswcAndFormat(match.potentialMatch.iswc)} />,
            field: ISWC_FIELD,
          },
          {
            element: <GridTextCell text={match.potentialMatch.originalTitle} />,
            field: TITLE_FIELD,
          },
          {
            element: (
              <div onClick={() => this._getIswcModel(index, match.potentialMatch.iswc)}>
                <GridIconCell
                  text={strings[VIEW_MORE_FIELD]}
                  icon={VIEW_MORE_ICON}
                  alt={'View More Icon'}
                  clickable
                  id={strings[VIEW_MORE_FIELD]}
                />
              </div>
            ),
            field: VIEW_MORE_FIELD,
            // Handled in GridRow.tsx*
            action: VIEW_MORE_ACTION,
          },
        ],
        viewMore: match.iswcModel && [<ViewMore iswcModel={match.iswcModel} isSubmissionGrid />],
      });
    });

    return gridRows;
  };

  renderDisambiguationReasonsAccordion = () => {
    const { openAccordion } = this.state;
    const {
      FIND_OUT_MORE_DISAMBIG,
      DISAMBIG_REASON_CODES,
      DIT,
      DIT_DESCRIPTION,
      FOR_EXAMPLE,
      DIT_DESCRIPTION_2,
      DIT_DESCRIPTION_3,
      DIA,
      DIA_DESCRIPTION,
      DIA_DESCRIPTION_2,
      DIA_DESCRIPTION_3,
      DIE,
      DIE_DESCRIPTION,
      DIC,
      DIC_DESCRIPTION,
      DIC_DESCRIPTION_2,
      DIV,
      DIV_DESCRIPTION,
    } = getStrings();
    return (
      <div className={styles.accordDiv}>
        <div className={styles.accordText} onClick={this._toggleAccordion}>
          {FIND_OUT_MORE_DISAMBIG}
        </div>
        <div
          className={styles.accordSubDiv}
          style={!openAccordion ? { display: 'none' } : { display: 'block' }}
        >
          <div className={styles.closeIconDiv}>
            <div className={styles.closeIcon} onClick={this._toggleAccordion}>
              <GridIconCell icon={CLOSE_ICON} alt={'Close'} smallerIcon />
            </div>
          </div>
          <div className={styles.textDiv}>
            <div className={styles.textDivTitle}>{DISAMBIG_REASON_CODES}</div>
            <ul>
              <li>
                <b>{DIT}</b> - {DIT_DESCRIPTION}
                <br />
                {FOR_EXAMPLE}:
                <br />
                {DIT_DESCRIPTION_2}
                <br />
                {DIT_DESCRIPTION_3}
              </li>
              <li>
                <b>{DIA}</b> - {DIA_DESCRIPTION}
                <br />
                {FOR_EXAMPLE}:
                <br />
                {DIA_DESCRIPTION_2}
                <br /> {DIA_DESCRIPTION_3}
              </li>
              <li>
                <b>{DIE}</b> - {DIE_DESCRIPTION}
              </li>
              <li>
                <b>{DIC}</b> - {DIC_DESCRIPTION}
                <br />
                {DIC_DESCRIPTION_2}
              </li>
              <li>
                <b>{DIV}</b> - {DIV_DESCRIPTION}
              </li>
            </ul>
          </div>
        </div>
      </div>
    );
  };

  renderSubmissionGrid = (
    title: string,
    headerCells: IGridHeaderCell[],
    rows: IGridRow[],
    key: ISubmissionStateKeys,
    row: any,
    showActionButton: boolean,
    actionButtonText?: string,
  ) => {
    const { addElementToArray } = this.props;

    return (
      <div className={styles.gridDiv}>
        <div className={styles.subHeader}>{title}</div>
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

  render() {
    const {
      disambiguationIswcs,
      disambiguationReason,
      bvltr,
      performers,
      standardInstrumentation,
      updateSubmissionDataArray,
      removeElementFromArray,
      updateSubmissionDataString,
    } = this.props;
    const strings = getStrings();

    return (
      <div className={styles.container}>
        <div className={styles.subHeader}>{strings[ISWCS_TO_DISAMIGUATE]}</div>
        <Grid
          headerCells={this._getDisambiguationGridHeaders()}
          gridRows={this._getDisambiguationGridRows()}
          cellPadding={'25px 25px 25px 10px'}
          headerColor={'#f7f7f7'}
        />
        <div className={styles.margin}>
          <div className={styles.subHeader}>{strings[ADDITIONAL_ISWCS_TO_DISAMBIGUATE]}</div>
          <div className={styles.inputDiv}>
            <GridInputCell
              onChange={(text: string) =>
                updateSubmissionDataString(text.replace(/[^T0-9;]/g, ''), 'disambiguationIswcs')
              }
              value={validateIswcAndFormatArray(disambiguationIswcs).join(';')}
            />
          </div>
        </div>
        <div className={styles.reasonBvltrDiv}>
          <div className={styles.leftDiv}>
            <div className={styles.subHeader}>{strings[REASONS_FOR_DISAMBIGUATION]}</div>
            <div className={styles.dropdownDiv}>
              <GridDropdownCell
                options={[BLANK_OPTION, ...DISAMBIGUATION_REASONS]}
                selectNewOption={(text: string) =>
                  updateSubmissionDataString(text, 'disambiguationReason')
                }
                value={disambiguationReason}
              />
            </div>
          </div>
          <div className={styles.rightDiv}>
            <div className={styles.subHeader}>BVLTR:</div>
            <div className={styles.dropdownDiv}>
              <GridDropdownCell
                options={[BLANK_OPTION, ...BVLTR]}
                selectNewOption={(text: string) => updateSubmissionDataString(text, 'bvltr')}
                value={bvltr}
              />
            </div>
          </div>
        </div>
        {this.renderDisambiguationReasonsAccordion()}
        <div className={styles.reasonBvltrDiv}>
          <div className={styles.leftDiv}>
            {this.renderSubmissionGrid(
              `${strings[PERFORMERS]}:`,
              performersHeaderCells,
              submissionPerformersGridRows(
                performers,
                updateSubmissionDataArray,
                removeElementFromArray,
              ),
              'performers',
              PERFORMERS_SUBMISSION_ROW,
              true,
              strings[ADD_NEW_PERFORMER],
            )}
          </div>
          <div className={styles.rightDiv}>
            <div className={styles.subHeader}>{strings[STANDARD_INSTRUMENTATION_FIELD]}:</div>
            <div className={styles.dropdownDiv}>
              <GridDropdownCell
                options={[BLANK_OPTION, ...getDropdownLookupData('Instrumentation')]}
                selectNewOption={(text: string) =>
                  updateSubmissionDataString(text, 'standardInstrumentation')
                }
                value={standardInstrumentation}
              />
            </div>
          </div>
        </div>
      </div>
    );
  }
}
