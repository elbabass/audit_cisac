import React, { memo, FunctionComponent } from 'react';
import styles from './Workflows.module.scss';
import GridCheckboxCell from '../../components/GridComponents/GridCheckboxCell/GridCheckboxCell';
import { getStrings } from '../../configuration/Localization';
import DateInput from '../../components/FormInput/DateInput';
import { IWorkflowsHeaderProps } from './WorkflowsTypes';

const WorkflowsHeader: FunctionComponent<IWorkflowsHeaderProps> = props => {
  const {
    ASSIGNED_TO_ME,
    CREATED_BY_ME,
    FILTER_BY_STATUS,
    SHOW_WORKFLOWS,
    FROM_DATE,
    TO_DATE,
    PENDING_APPROVAL,
    APPROVED,
    REJECTED,
  } = getStrings();

  const {
    statusFilters: { pending, approved, rejected },
    showAssigned,
    fromDate,
    toDate,
    updateStatusFilters,
    changeWorkFlowsDisplayed,
    updateDates,
    loading,
  } = props;

  return (
    <div className={styles.headerContainer}>
      <div className={`${styles.headerItem}`}>
        <div className={`${styles.headerTextContainer} ${styles.tabletTitle}`}>
          {SHOW_WORKFLOWS}
        </div>
        <div className={styles.headerTextContainer}>
          <div className={styles.sectionTitle}>{SHOW_WORKFLOWS}</div>
          <div className={styles.labelText}>
            <GridCheckboxCell
              text={ASSIGNED_TO_ME}
              onClickCheckbox={changeWorkFlowsDisplayed}
              checked={showAssigned}
              disabled={loading}
            />
          </div>
        </div>
        <div className={styles.headerTextContainer}>
          <div className={styles.sectionTitle}></div>
          <div className={styles.labelText}>
            <GridCheckboxCell
              text={CREATED_BY_ME}
              onClickCheckbox={changeWorkFlowsDisplayed}
              checked={!showAssigned}
              disabled={loading}
            />
          </div>
        </div>
      </div>
      <div className={`${styles.statusFilter} ${styles.divider}`}>
        <div className={`${styles.headerTextContainer} ${styles.tabletTitle}`}>
          {FILTER_BY_STATUS}
        </div>
        <div className={styles.headerTextContainer}>
          <div className={styles.sectionTitle}>{FILTER_BY_STATUS}</div>
          <div className={styles.labelText}>
            <GridCheckboxCell
              text={PENDING_APPROVAL}
              onClickCheckbox={() => updateStatusFilters('pending')}
              checked={pending}
              disabled={loading}
            />
          </div>
        </div>
        <div className={styles.headerTextContainer}>
          <div className={styles.sectionTitle}></div>
          <div className={styles.labelText}>
            <GridCheckboxCell
              text={APPROVED}
              onClickCheckbox={() => updateStatusFilters('approved')}
              checked={approved}
              disabled={loading}
            />
          </div>
        </div>
        <div className={styles.headerTextContainer}>
          <div className={styles.sectionTitle}></div>
          <div className={styles.labelText}>
            <GridCheckboxCell
              text={REJECTED}
              onClickCheckbox={() => updateStatusFilters('rejected')}
              checked={rejected}
              disabled={loading}
            />
          </div>
        </div>
      </div>
      <div className={`${styles.headerItem} ${styles.divider}`}>
        <div className={styles.headerTextContainer}>
          <DateInput
            label={FROM_DATE}
            value={fromDate}
            name={'fromDate'}
            changeData={updateDates}
          />
        </div>
        <div className={styles.headerTextContainer}>
          <DateInput label={TO_DATE} value={toDate} name={'toDate'} changeData={updateDates} />
        </div>
      </div>
    </div>
  );
};

export default memo(WorkflowsHeader);
