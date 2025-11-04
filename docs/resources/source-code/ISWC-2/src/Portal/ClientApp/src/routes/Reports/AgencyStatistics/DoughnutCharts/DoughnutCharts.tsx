import React, { FunctionComponent, memo } from 'react';
import styles from '../AgencyStatistics.module.scss';
import { IDoughnutChartsProps } from './DoughnutChartsTypes';
import { Doughnut } from 'react-chartjs-2';
import { getStrings } from '../../../../configuration/Localization';

export const DoughnutCharts: FunctionComponent<IDoughnutChartsProps> = ({
  agencyStatisticsSearchResults,
}) => {
    const { TOTAL_WORKFLOWS_ASSIGNED, TOTAL_SUCCESSFUL_SUBMISSIONS } = getStrings();

  const chartOptions = (title: string, total?: number): Chart.ChartOptions => ({
    animation: {
      animateRotate: true,
    },
    title: {
      text: title + ': ' + total,
      display: true,
      position: 'top',
    },
    legend: {
      position: 'bottom',
      labels: {
        boxWidth: 25,
        usePointStyle: true,
      },
    },
  });

  const _getSubChartData = () => {
    const {
      ISWC_ELIGIBLE,
      ISWC_ELIGIBLE_DISAMBIGUATION,
      NON_ISWC_ELIGIBLE,
      NON_ISWC_ELIGIBLE_DISAMBIGUATION,
    } = getStrings();

    let totalEligibleIswcSubmissions = 0;
    let totalEligibleIswcSubmissionsWithDisambiguation = 0;
    let totalIneligibleIswcSubmissions = 0;
    let totalIneligibleIswcSubmissionsWithDisambiguation = 0;

    agencyStatisticsSearchResults?.forEach((x) => {
      totalEligibleIswcSubmissions = totalEligibleIswcSubmissions + x.eligibleIswcSubmissions;
      totalEligibleIswcSubmissionsWithDisambiguation =
        totalEligibleIswcSubmissionsWithDisambiguation +
        x.eligibleIswcSubmissionsWithDisambiguation;
      totalIneligibleIswcSubmissions = totalIneligibleIswcSubmissions + x.inEligibleIswcSubmissions;
      totalIneligibleIswcSubmissionsWithDisambiguation =
        totalIneligibleIswcSubmissionsWithDisambiguation +
        x.inEligibleIswcSubmissionsWithDisambiguation;
    });

    return {
      labels: [
        ISWC_ELIGIBLE,
        ISWC_ELIGIBLE_DISAMBIGUATION,
        NON_ISWC_ELIGIBLE,
        NON_ISWC_ELIGIBLE_DISAMBIGUATION,
      ],
      datasets: [
        {
          data: [
            totalEligibleIswcSubmissions,
            totalEligibleIswcSubmissionsWithDisambiguation,
            totalIneligibleIswcSubmissions,
            totalIneligibleIswcSubmissionsWithDisambiguation,
          ],
          backgroundColor: ['#3B71D8', '#3bc6d8', '#c46e54', '#ffbb99'],
        },
      ],
    };
  };

  const _getWorkflowsChartData = () => {
    const { APPROVED, PENDING, REJECTED } = getStrings();
    let totalWorkflowTasksAssignedPending = 0;
    let totalWorkflowTasksAssignedApproved = 0;
    let totalWorkflowTasksAssignedRejected = 0;

    agencyStatisticsSearchResults?.forEach((x) => {
      totalWorkflowTasksAssignedPending =
        totalWorkflowTasksAssignedPending + x.workflowTasksAssignedPending;
      totalWorkflowTasksAssignedApproved =
        totalWorkflowTasksAssignedApproved + x.workflowTasksAssignedApproved;
      totalWorkflowTasksAssignedRejected =
        totalWorkflowTasksAssignedRejected + x.workflowTasksAssignedRejected;
    });

    return {
      labels: [PENDING, APPROVED, REJECTED],
      datasets: [
        {
          data: [
            totalWorkflowTasksAssignedPending,
            totalWorkflowTasksAssignedApproved,
            totalWorkflowTasksAssignedRejected,
          ],
          backgroundColor: ['#e46e00', '#2a994c', '#83252b'],
        },
      ],
    };
  };

  const _getTotals = (type: 'submission' | 'workflow') => {
    let total = 0;

    agencyStatisticsSearchResults?.forEach((x) => {
      total = type === 'submission' ? total + x.validSubmissions : total + x.workflowTasksAssigned;
    });

    return total;
  };

  return (
    <div className={styles.chartRowDiv}>
      <div className={styles.chartDiv}>
        <Doughnut
          data={_getSubChartData()}
                  options={chartOptions(TOTAL_SUCCESSFUL_SUBMISSIONS, _getTotals('submission'))}
          height={170}
        />
      </div>
      <div className={styles.chartDiv}>
        <Doughnut
          data={_getWorkflowsChartData()}
          options={chartOptions(TOTAL_WORKFLOWS_ASSIGNED, _getTotals('workflow'))}
          height={170}
        />
      </div>
    </div>
  );
};

export default memo(DoughnutCharts);
