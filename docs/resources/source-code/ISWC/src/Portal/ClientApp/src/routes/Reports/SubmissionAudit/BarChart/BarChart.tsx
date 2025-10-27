import * as React from 'react';
import styles from '../SubmissionAudit.module.scss';
import { IBarChartProps, IBarChartState } from './BarChartTypes';
import Media from 'react-media';
import { Bar } from 'react-chartjs-2';
import { getStrings } from '../../../../configuration/Localization';

class BarChart extends React.PureComponent<IBarChartProps, IBarChartState> {
  _getLabels = () => {
    const { numberOfDaysInMonth } = this.props;
    const labels: string[] = [];

    for (var x = 1; x < numberOfDaysInMonth + 1; x++) {
      labels.push(x.toString());
    }

    return labels;
  };

  _getData = (type: 'successful' | 'failure') => {
    const { agencyStatisticsSearchResults, numberOfDaysInMonth } = this.props;
    const data: number[] = [];
    for (let x = 0; x < numberOfDaysInMonth; x++) data.push(0);

    for (let x = 0; x < data.length; x++) {
      agencyStatisticsSearchResults.forEach((result) => {
        if (result.day === x + 1) {
          let res = type === 'successful' ? result.validSubmissions : result.invalidSubmissions;
            data[x] = data[x] + res;
        }
      });
    }

    return data;
  };

  _getColour = (colour: string) => {
    const { numberOfDaysInMonth } = this.props;
    const colourArray: string[] = [];
    for (let x = 0; x < numberOfDaysInMonth; x++) {
      colourArray.push(colour);
    }

    return colourArray;
  };

  _getChartData = () => {
    const { SUCCESSFUL, FAILED } = getStrings();
    return {
      labels: this._getLabels(),
      datasets: [
        {
          label: SUCCESSFUL,
          data: this._getData('successful'),
          backgroundColor: this._getColour('#84afff'),
        },
        {
          label: FAILED,
          data: this._getData('failure'),
          backgroundColor: this._getColour('#3B71D8'),
        },
      ],
    };
  };

  _getTotalForMonth = (type: 'successful' | 'failure') => {
    const { agencyStatisticsSearchResults } = this.props;
    let total = 0;

    for (let x = 0; x < agencyStatisticsSearchResults.length; x++) {
      total =
        type === 'successful'
          ? total + agencyStatisticsSearchResults[x].validSubmissions
          : total + agencyStatisticsSearchResults[x].invalidSubmissions;
    }
    return total;
  };

  renderTotalsRow = () => {
    const { TOTAL_SUCCEEDED_FOR_MONTH, TOTAL_ERROR_FOR_MONTH, ERROR_RATE_PER_MONTH } = getStrings();

    return (
      <div className={styles.belowGridRow}>
        <div className={styles.section}>
          {TOTAL_SUCCEEDED_FOR_MONTH}: <b>{this._getTotalForMonth('successful')}</b>
        </div>
        <div className={styles.sectionBorder}>
          {TOTAL_ERROR_FOR_MONTH}: <b>{this._getTotalForMonth('failure')}</b>
        </div>
        <div className={styles.section}>
          {ERROR_RATE_PER_MONTH}:{' '}
          <b>
            {(
              (this._getTotalForMonth('failure') /
                (this._getTotalForMonth('failure') + this._getTotalForMonth('successful'))) *
              100
            ).toFixed(2)}
            %
          </b>
        </div>
      </div>
    );
  };

  render() {
    const { ERROR_SUCCESS_GRAPH, DAYS, NO_OF_SUBS } = getStrings();

    const chartOptions: Chart.ChartOptions = {
      scales: {
        xAxes: [
          {
            stacked: true,
            gridLines: {
              drawOnChartArea: false,
            },
            scaleLabel: {
              labelString: DAYS,
              display: true,
            },
          },
        ],
        yAxes: [
          {
            stacked: true,
            gridLines: {
              drawOnChartArea: false,
            },
            scaleLabel: {
              labelString: NO_OF_SUBS,
              display: true,
            },
          },
        ],
      },
    };

    return (
      <div>
        <div className={styles.graphTitle}>{ERROR_SUCCESS_GRAPH}</div>
        <Media queries={{ small: { maxWidth: 576 } }}>
          {(matches) =>
            matches.small ? (
              <Bar data={this._getChartData()} options={chartOptions} height={200} />
            ) : (
              <Bar data={this._getChartData()} options={chartOptions} height={110} />
            )
          }
        </Media>
        {this.renderTotalsRow()}
      </div>
    );
  }
}

export default BarChart;
