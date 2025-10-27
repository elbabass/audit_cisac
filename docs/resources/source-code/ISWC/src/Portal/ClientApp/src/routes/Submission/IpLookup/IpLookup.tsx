import * as React from 'react';
import { IIpLookupProps, IIpLookupState, IIpLookupStateKeys } from './IpLookupTypes';
import styles from './IpLookup.module.scss';
import GridInputCell from '../../../components/GridComponents/GridInputCell/GridInputCell';
import ActionButton from '../../../components/ActionButton/ActionButton';
import GridIconCell from '../../../components/GridComponents/GridIconCell/GridIconCell';
import {
  CLOSE_ICON,
  IP_NAME_NUMBER_FIELD,
  NAME_FIELD,
  IP_BASE_NUMBER_FIELD,
  ADD_FIELD,
  PLUS_ICON,
  CANCEL,
  SEARCH,
  ADD,
  IP_NOT_FOUND,
  LOOKUP,
  SURNAME_FIELD,
  PUBLISHER_IP_NAME,
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

export default class IpLookup extends React.PureComponent<IIpLookupProps, IIpLookupState> {
  constructor(props: IIpLookupProps) {
    super(props);

    this.state = {
      name: '',
      basenumber: '',
      error: undefined,
      results: [],
      loading: false,
    };
  }

  _updateSubmissionDataString = (value: string, key: IIpLookupStateKeys) => {
    const stringCopy = JSON.parse(JSON.stringify(value));

    this.setState({
      [key]: stringCopy,
    } as Pick<IIpLookupState, keyof IIpLookupState>);
  };

  _searchByIp = async () => {
    const { ipType, nameNumber } = this.props;
    const { name, basenumber } = this.state;
    const strings = getStrings();

    this.setState({
      error: undefined,
      results: [],
      loading: true,
    });

    searchByIp(Number(nameNumber), ipType, name, basenumber)
      .then((res: any) => {
        this.setState({
          results: res.data,
          error: res.data.length === 0 ? 'IP not found' : null,
          loading: false,
        });
      })
      .catch((err: any) => {
        this.setState({
          error: strings[IP_NOT_FOUND],
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
    let gridRows: IGridRow[] = [];
    const strings = getStrings();

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
    const { close, ipType } = this.props;
    const { error, results, loading } = this.state;
    const strings = getStrings();

    return (
      <div className={styles.container}>
        <div className={styles.closeButton} onClick={() => close && close()}>
          <GridIconCell icon={CLOSE_ICON} alt={'Close Icon'} smallerIcon />
        </div>
        <div className={styles.title}>{strings[LOOKUP]}</div>
        <form
          autoComplete="off"
          onSubmit={(e) => {
            e.preventDefault();
          }}
        >
          <div className={styles.inputRow}>
            <div>
              <div className={styles.inputLabel}>
                {ipType === 1 ? strings[PUBLISHER_IP_NAME] : strings[SURNAME_FIELD]}
              </div>
              <div className={styles.input}>
                <GridInputCell
                  onChange={(text: string) => this._updateSubmissionDataString(text, 'name')}
                />
              </div>
            </div>
            <div>
              <div className={styles.inputLabel}>{strings[IP_BASE_NUMBER_FIELD]}</div>
              <div className={styles.input}>
                <GridInputCell
                  onChange={(text: string) => this._updateSubmissionDataString(text, 'basenumber')}
                />
              </div>
            </div>
          </div>
          <div className={styles.actionButtonsDiv}>
            <div className={styles.actionButtonDiv}>
              <ActionButton buttonText={strings[CANCEL]} buttonAction={() => close && close()} />
            </div>
            <div className={styles.actionButtonDiv}>
              <ActionButton
                buttonText={strings[SEARCH]}
                buttonAction={() => this._searchByIp()}
                submitType
              />
            </div>
          </div>
        </form>
        {loading && <Loader />}
        {!loading && error && <div>{error}</div>}
        {!loading && !error && results.length > 0 && (
          <div className={styles.gridDiv}>
            {this._getGridRows().length >= 50 && (
              <div className={styles.alertDiv}>
                <AlertMessage
                  type={'info'}
                  message={
                    'More than 50 results matching your search criteria are available. Please refine your search by using the IP Number or more precise IP Name information.'
                  }
                />
              </div>
            )}
            <Grid headerCells={this._getHeaderCells()} gridRows={this._getGridRows()} />
          </div>
        )}
      </div>
    );
  }
}
