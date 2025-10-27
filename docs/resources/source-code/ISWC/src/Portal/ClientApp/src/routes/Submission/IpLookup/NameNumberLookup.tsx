import * as React from 'react';
import { IIpLookupProps, IIpLookupState } from './IpLookupTypes';
import styles from './IpLookup.module.scss';
import GridIconCell from '../../../components/GridComponents/GridIconCell/GridIconCell';
import {
  CLOSE_ICON,
  IP_NAME_NUMBER_FIELD,
  NAME_FIELD,
  IP_BASE_NUMBER_FIELD,
  ADD_FIELD,
  PLUS_ICON,
  ADD,
} from '../../../consts';
import { searchByIp } from '../../../redux/services/SubmissionService';
import Grid from '../../../components/GridComponents/Grid/Grid';
import { IGridRow } from '../../../components/GridComponents/Grid/GridTypes';
import GridTextCell from '../../../components/GridComponents/GridTextCell/GridTextCell';
import { IIpLookupResponse, ISubmissionStateKeys } from '../SubmissionTypes';
import Loader from '../../../components/Loader/Loader';
import IconActionButton from '../../../components/ActionButton/IconActionButton';
import { getStrings } from '../../../configuration/Localization';
import { padIpNameNumber } from '../../../shared/helperMethods';
import AlertMessage from '../../../components/AlertMessage/AlertMessage';

export default class NameNumberLookup extends React.PureComponent<IIpLookupProps, IIpLookupState> {
  constructor(props: IIpLookupProps) {
    super(props);

    this.state = {
      loading: false,
      error: undefined,
      results: [],
    };
  }

  componentDidMount() {
    this._searchByIp();
  }

  _searchByIp = async () => {
    const { ipType, nameNumber } = this.props;

    this.setState({
      error: undefined,
      results: [],
      loading: true,
    });

    searchByIp(Number(nameNumber), ipType)
      .then((res: any) => {
        this.setState({
          results: res.data,
          error: res.data.length === 0 ? 'IP not found' : null,
          loading: false,
        });
      })
      .catch((err: any) => {
        this.setState({
          error: 'IP not found',
          loading: false,
        });
      });
  };

  _getHeaderCells = () => {
    const strings = getStrings();
    return [
      { text: strings[NAME_FIELD], field: NAME_FIELD },
      { text: strings[IP_NAME_NUMBER_FIELD], field: IP_NAME_NUMBER_FIELD },
      { text: strings[IP_BASE_NUMBER_FIELD], field: IP_BASE_NUMBER_FIELD },
      { field: ADD_FIELD },
    ];
  };

  _addIp = (result: IIpLookupResponse) => {
    const { addElementToSubmissionDataArray, rowId, close, ipType } = this.props;
    const key: ISubmissionStateKeys = ipType === 2 ? 'creators' : 'publishers';

    addElementToSubmissionDataArray(
      {
        name: result.name,
        nameNumber: result.ipNameNumber.toString(),
        baseNumber: result.ipBaseNumber,
        role: ipType === 2 ? 'CA' : 'E',
        legalEntityType: result.legalEntityType,
      },
      rowId,
      key,
    );
    close && close();
  };

  _getGridRows = () => {
    const { results } = this.state;
    const strings = getStrings();
    let gridRows: IGridRow[] = [];

    results.forEach((result, index) => {
      gridRows.push({
        rowId: index,
        cells: [
          {
            element: <GridTextCell text={result.name} />,
            field: NAME_FIELD,
          },
          {
            element: <GridTextCell text={padIpNameNumber(result.ipNameNumber.toString())} />,
            field: IP_NAME_NUMBER_FIELD,
          },
          {
            element: <GridTextCell text={result.ipBaseNumber} />,
            field: IP_BASE_NUMBER_FIELD,
          },
          {
            element: (
              <IconActionButton
                icon={PLUS_ICON}
                buttonText={strings[ADD]}
                buttonAction={() => this._addIp(result)}
              />
            ),
            field: ADD_FIELD,
          },
        ],
      });
    });

    return gridRows;
  };

  render() {
    const { close } = this.props;
    const { error, results, loading } = this.state;
    const { REFINE_SEARCH } = getStrings();

    return (
      <div className={styles.container}>
        <div className={styles.closeButton} onClick={() => close && close()}>
          <GridIconCell icon={CLOSE_ICON} alt={'Close Icon'} smallerIcon />
        </div>
        {loading && <Loader />}
        {!loading && error && (
          <div>
            <div className={styles.error}>{error}</div>
            <div className={styles.tryAgain} onClick={this._searchByIp}>
              Try Again
            </div>
          </div>
        )}
        {!loading && !error && results.length > 0 && (
          <div className={styles.gridDiv}>
            {this._getGridRows().length >= 50 && (
              <div className={styles.alertDiv}>
                <AlertMessage type={'info'} message={REFINE_SEARCH} />
              </div>
            )}
            <Grid headerCells={this._getHeaderCells()} gridRows={this._getGridRows()} />
          </div>
        )}
      </div>
    );
  }
}
