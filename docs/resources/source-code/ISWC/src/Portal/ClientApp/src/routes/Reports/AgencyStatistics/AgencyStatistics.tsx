import * as React from 'react';
import styles from './AgencyStatistics.module.scss';
import { IAgencyStatisticsProps, IAgencyStatisticsState } from './AgencyStatisticsTypes';
import DropdownInput from '../../../components/FormInput/DropdownInput';
import { getStrings } from '../../../configuration/Localization';
import GridCheckboxCell from '../../../components/GridComponents/GridCheckboxCell/GridCheckboxCell';
import { getLoggedInAgencyId } from '../../../shared/helperMethods';
import {
  PER_MONTH_PERIOD,
  PER_YEAR_PERIOD,
  getAgencies,
  MONTHS,
  REPORT_ICON_BLACK,
} from '../../../consts';
import { mapAgenciesToDropDownType } from '../../../shared/MappingObjects';
import IconActionButton from '../../../components/ActionButton/IconActionButton';
import DoughnutCharts from './DoughnutCharts/DoughnutCharts';
import { IDropdownOption } from '../../../components/FormInput/FormInputTypes';
import Loader from '../../../components/Loader/Loader';
import ErrorHandler from '../../../components/AlertMessage/ErrorHandler/ErrorHandler';

class AgencyStatistics extends React.PureComponent<IAgencyStatisticsProps, IAgencyStatisticsState> {
  constructor(props: IAgencyStatisticsProps) {
    super(props);
    this.state = {
      paramFields: {
        agencyName: getLoggedInAgencyId(),
        year: new Date().getFullYear(),
        month: new Date().getMonth() + 1,
        timePeriod: PER_MONTH_PERIOD,
      },
    };
  }

  _setTimePeriod = (period: number) => {
    this.setState({
      paramFields: { ...this.state.paramFields, timePeriod: period },
    });
  };

  _updateParameter = (event: any) => {
    const name = event.target.id;
    let value = event.target.value;

    this.setState({
      paramFields: {
        ...this.state.paramFields,
        [name]: name === 'year' || name === 'month' ? parseInt(value) : value,
      },
    });
  };

  _getYearDropdownOptions = (): IDropdownOption[] => {
    const currentYear = new Date().getFullYear();
    const yearsArray = [];
    const yearsDiff = currentYear - 2020;

    for (let x = 0; x <= yearsDiff; x++) {
      yearsArray.push({
        name: (2020 + x).toString(),
        value: (2020 + x).toString(),
      });
    }
    return yearsArray.sort((a, b) => parseInt(b.value) - parseInt(a.value));
  };

  _search = () => {
    const { reportsAgencyStatisticsSearch } = this.props;
    const { paramFields } = this.state;

    reportsAgencyStatisticsSearch(paramFields);
  };

  renderFiltersDiv = () => {
    const { paramFields } = this.state;
    const { PER_MONTH, TIME_PERIOD, PER_YEAR, AGENCY_NAME_FIELD } = getStrings();

    return (
      <div className={styles.filtersDiv}>
        <div className={styles.dropdownDiv}>
          <DropdownInput
            label={AGENCY_NAME_FIELD}
            onChange={(event) => this._updateParameter(event)}
            name={'agencyName'}
            options={[{ value: 'All', name: '<All>' }, ...mapAgenciesToDropDownType(getAgencies())]}
            defaultToLoggedInAgency={true}
          />
        </div>
        <div>
          <div className={styles.label}>{TIME_PERIOD}:</div>
          <div className={styles.checkBoxRow}>
            <div className={styles.checkBox}>
              <GridCheckboxCell
                text={PER_MONTH}
                onClickCheckbox={() => this._setTimePeriod(PER_MONTH_PERIOD)}
                checked={paramFields.timePeriod === PER_MONTH_PERIOD}
              />
            </div>
            <div className={styles.checkBox}>
              <GridCheckboxCell
                text={PER_YEAR}
                onClickCheckbox={() => this._setTimePeriod(PER_YEAR_PERIOD)}
                checked={paramFields.timePeriod === PER_YEAR_PERIOD}
              />
            </div>
          </div>
        </div>
      </div>
    );
  };

  renderTimePeriodDiv = () => {
    const { paramFields } = this.state;
    const {
      PER_MONTH,
      PER_YEAR,
      SELECT_PREF_MONTH_YEAR,
      SELECT_PREF_YEAR,
      YEAR,
      MONTH,
    } = getStrings();

    return (
      <div>
        <div className={styles.title}>
          {paramFields.timePeriod === PER_MONTH_PERIOD ? PER_MONTH : PER_YEAR}
        </div>
        <div>
          {paramFields.timePeriod === PER_MONTH_PERIOD ? SELECT_PREF_MONTH_YEAR : SELECT_PREF_YEAR}
        </div>
        <div className={styles.dropdownRow}>
          <div className={styles.dropdownDiv}>
            <DropdownInput
              label={YEAR}
              options={this._getYearDropdownOptions()}
              onChange={(event) => this._updateParameter(event)}
              name={'year'}
            />
          </div>
          {paramFields.timePeriod === PER_MONTH_PERIOD && (
            <div className={styles.dropdownDiv}>
              <DropdownInput
                label={MONTH}
                options={MONTHS}
                onChange={(event) => this._updateParameter(event)}
                name={'month'}
              />
            </div>
          )}
        </div>
      </div>
    );
  };

  renderMainView = () => {
    const { loading, error, agencyStatisticsSearchResults } = this.props;
    const { NO_RECORDS } = getStrings();

    if (loading) return <Loader />;

    if (error) return <ErrorHandler error={error} />;

    if (agencyStatisticsSearchResults && agencyStatisticsSearchResults?.length > 0) {
      return <DoughnutCharts agencyStatisticsSearchResults={agencyStatisticsSearchResults} />;
    } else if (agencyStatisticsSearchResults && agencyStatisticsSearchResults?.length === 0) {
      return <ErrorHandler error={{ response: { data: { message: NO_RECORDS } } }} />;
    }
  };

  render() {
    const { SHOW_GRAPHS } = getStrings();
    return (
      <div className={styles.container}>
        {this.renderFiltersDiv()}
        <div className={styles.mainDiv}>{this.renderTimePeriodDiv()}</div>
        <div className={styles.actionButton}>
          <IconActionButton
            icon={REPORT_ICON_BLACK}
            buttonText={SHOW_GRAPHS}
            theme={'secondary'}
            buttonAction={() => this._search()}
            bigIcon={true}
          />
        </div>
        <div className={styles.graphView}>{this.renderMainView()}</div>
      </div>
    );
  }
}

export default AgencyStatistics;
