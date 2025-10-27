import * as React from 'react';
import { ISelectPreferredIswcsProps, ISelectPreferredIswcsState } from './SelectPreferredIswcTypes';
import Grid from '../../../../components/GridComponents/Grid/Grid';
import {
  SELECT_PREFERRED_FIELD,
  ISWC_FIELD,
  TITLE_FIELD,
  VIEW_MORE_FIELD,
  VIEW_MORE_ICON,
  VIEW_MORE_ACTION,
  PREFERRED,
} from '../../../../consts';
import GridCheckboxCell from '../../../../components/GridComponents/GridCheckboxCell/GridCheckboxCell';
import GridTextCell from '../../../../components/GridComponents/GridTextCell/GridTextCell';
import GridIconCell from '../../../../components/GridComponents/GridIconCell/GridIconCell';
import { IGridRow } from '../../../../components/GridComponents/Grid/GridTypes';
import ViewMore from '../../../Search/ViewMore/ViewMore';
import { IIswcModel } from '../../../../redux/types/IswcTypes';
import styles from './SelectPreferredIswc.module.scss';
import { getStrings } from '../../../../configuration/Localization';
import { validateIswcAndFormat } from '../../../../shared/helperMethods';

export default class SelectPreferredIswc extends React.PureComponent<
  ISelectPreferredIswcsProps,
  ISelectPreferredIswcsState
> {
  constructor(props: ISelectPreferredIswcsProps) {
    super(props);

    this.state = {
      loading: false,
      potentialMatches: props.potentialMatches.map((x) => {
        return { potentialMatch: x, iswcModel: undefined };
      }),
    };
  }

  componentDidMount = () => {
    const { updateSubmissionDataString } = this.props;
    const { potentialMatches } = this.props;

    updateSubmissionDataString(potentialMatches[0]?.iswc, 'preferredIswc');
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

  _getGridHeaderCells = () => {
    const strings = getStrings();
    return [
      { text: strings[SELECT_PREFERRED_FIELD], field: SELECT_PREFERRED_FIELD },
      { text: strings[ISWC_FIELD], field: ISWC_FIELD },
      { text: strings[TITLE_FIELD], field: TITLE_FIELD },
      { field: VIEW_MORE_FIELD },
    ];
  };

  _getGridRows = () => {
    const { preferredIswc, updateSubmissionDataString } = this.props;
    const { potentialMatches } = this.state;
    let gridRows: IGridRow[] = [];
    const strings = getStrings();

    potentialMatches.forEach((match, index) => {
      gridRows.push({
        rowId: index,
        cells: [
          {
            element: (
              <GridCheckboxCell
                onClickCheckbox={() =>
                  updateSubmissionDataString(match.potentialMatch.iswc, 'preferredIswc')
                }
                checked={preferredIswc === match.potentialMatch.iswc}
                text={strings[PREFERRED]}
              />
            ),
            field: SELECT_PREFERRED_FIELD,
          },
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

  render() {
    return (
      <div className={styles.container}>
        <Grid
          headerCells={this._getGridHeaderCells()}
          gridRows={this._getGridRows()}
          cellPadding={'25px 25px 25px 10px'}
          headerColor={'#f7f7f7'}
        />
      </div>
    );
  }
}
