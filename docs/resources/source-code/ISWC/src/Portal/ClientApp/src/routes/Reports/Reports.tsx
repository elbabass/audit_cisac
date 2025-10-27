import * as React from 'react';
import styles from './Reports.module.scss';
import { IReportsProps, IReportsState } from './ReportsTypes';
import TabView from '../../components/TabComponents/TabView/TabView';
import { getStrings } from '../../configuration/Localization';
import SubHeader from '../../components/SubHeader/SubHeader';
import SubmissionAuditContainer from './SubmissionAudit/SubmissionAuditContainer';
import AgencyInterestExtractContainer from './AgencyInterestExtract/AgencyInterestExtractContainer';
import IswcFullExtractContainer from './IswcFullExtract/IswcFullExtractContainer';
import PublisherIswcTrackingContainer from './PublisherIswcTracking/PublisherIswcTrackingContainer';
import FileSubmissionAuditContainer from './FileSubmissionAudit/FileSubmissionAuditContainer';
import AgencyStatisticsContainer from './AgencyStatistics/AgencyStatisticsContainer';
import PotentialDuplicatesContainer from './PotentialDuplicates/PotentialDuplicatesContainer';
import AgencyWorkListContainer from './AgencyWorkList/AgenctWorkListContainer'
import IswcCreatorExtractContainer from './IswcCreatorExtract/IswcCreatorExtractContainer';
import { ITab } from '../../components/TabComponents/TabView/TabViewTypes';

class Reports extends React.Component<IReportsProps, IReportsState> {
  _getTabs = (): ITab[] => {
    const {
      SUBMISSION_AUDIT,
      AGENCY_INTEREST_EXTRACT,
      ISWC_FULL_EXTRACT,
      AGENCY_STATS,
      FILE_SUB_AUDIT,
      PUBLISHER_ISWC_TRACKING,
      POTENTIAL_DUPLICATES,
      AGENCY_WORK_LIST,
      ISWC_CREATOR_EXTRACT
    } = getStrings();
    const { assignedRoles } = this.props;
    return [
      {
        text: SUBMISSION_AUDIT,
        component: <SubmissionAuditContainer />,
      },
      {
        text: AGENCY_INTEREST_EXTRACT,
        component: <AgencyInterestExtractContainer />,
        disabled: !assignedRoles.reportAgencyInterest,
      },
      {
        text: ISWC_FULL_EXTRACT,
        component: <IswcFullExtractContainer />,
        disabled: !assignedRoles.reportExtracts,
      },
      {
        text: AGENCY_STATS,
        component: <AgencyStatisticsContainer />,
      },
      {
        text: FILE_SUB_AUDIT,
        component: <FileSubmissionAuditContainer />,
      },
      {
        text: PUBLISHER_ISWC_TRACKING,
        component: <PublisherIswcTrackingContainer />,
        disabled: !assignedRoles.reportExtracts,
      },
      {
        text: POTENTIAL_DUPLICATES,
        component: <PotentialDuplicatesContainer />,
        disabled: !assignedRoles.reportExtracts,
      },
      {
        text: AGENCY_WORK_LIST,
        component: <AgencyWorkListContainer />,
        disabled: !assignedRoles.reportExtracts,
      },
      {
        text: ISWC_CREATOR_EXTRACT,
        component: <IswcCreatorExtractContainer />,
        disabled: !assignedRoles.reportExtracts,
      }
    ];
  };

  render() {
    const { clearReportsError } = this.props;
    const { REPORTS } = getStrings();
    return (
      <div className={styles.container}>
        <SubHeader title={REPORTS} />
        <TabView tabs={this._getTabs()} reportsPage tabClickAdditionalAction={clearReportsError} />
      </div>
    );
  }
}

export default Reports;
