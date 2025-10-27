import React, { FunctionComponent, memo } from 'react';
import { getStrings } from '../../../configuration/Localization';
import styles from './WorkflowsAdvancedSearch.module.scss';
import { IWorkflowsAdvancedSearchProps } from './WorkflowsAdvancedSearchTypes';
import DropdownInput from '../../../components/FormInput/DropdownInput';
import BasicFormInput from '../../../components/FormInput/BasicFormInput';
import { mapAgenciesToDropDownType } from '../../../shared/MappingObjects';
import { getAgencies, WORKFLOW_TYPE } from '../../../consts';

const WorkflowsAdvancedSearch: FunctionComponent<IWorkflowsAdvancedSearchProps> = (props) => {
  const {
    ADVANCED_SEARCH,
    ADVANCED_SEARCH_DESC,
    ISWC_FIELD,
    AGENCY_WORK_CODES,
    ORIGINATING_AGENCY_FIELD,
    WORKFLOW_TYPE_FIELD,
    OR,
  } = getStrings();

  const _fieldIsEmpty = (field?: string) => {
    if (!field) {
      return true;
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.labelText}>{ADVANCED_SEARCH}</div>
      <div>
        <label className={styles.switchInput}>
          <input type="checkbox" onClick={props.toggleAdvancedSearch} />
          <span className={`${styles.slider} ${styles.round}`}></span>
        </label>
      </div>
      {props.showAdvancedSearch && (
        <div>
          <div className={styles.descriptionText}>{ADVANCED_SEARCH_DESC}</div>
          <div className={styles.searchContainer}>
            <div className={styles.formInput}>
              <BasicFormInput
                label={ISWC_FIELD}
                name={'iswc'}
                onChange={props.updateSearchFilters}
                value={props.searchFilters.iswc}
                disabled={
                  !_fieldIsEmpty(props.searchFilters.workCodes) ||
                  (props.searchFilters.agency !== '--' &&
                    !_fieldIsEmpty(props.searchFilters.agency)) ||
                  (props.searchFilters.workflowType !== '--' &&
                    !_fieldIsEmpty(props.searchFilters.workflowType))
                }
              />
            </div>
            <div className={styles.orDiv}>{OR}</div>
            <div className={styles.formInput}>
              <BasicFormInput
                label={AGENCY_WORK_CODES}
                name={'workCodes'}
                onChange={props.updateSearchFilters}
                value={props.searchFilters.workCodes}
                disabled={
                  !_fieldIsEmpty(props.searchFilters.iswc) ||
                  (props.searchFilters.agency !== '--' &&
                    !_fieldIsEmpty(props.searchFilters.agency)) ||
                  (props.searchFilters.workflowType !== '--' &&
                    !_fieldIsEmpty(props.searchFilters.workflowType))
                }
              />
            </div>
            <div className={styles.orDiv}>{OR}</div>
            <div className={styles.formInput}>
              <DropdownInput
                label={ORIGINATING_AGENCY_FIELD}
                name={'agency'}
                options={[{ value: '--', name: '--' }, ...mapAgenciesToDropDownType(getAgencies())]}
                onChange={props.updateSearchFilters}
                disabled={
                  !_fieldIsEmpty(props.searchFilters.iswc) ||
                  !_fieldIsEmpty(props.searchFilters.workCodes) ||
                  (props.searchFilters.workflowType !== '--' &&
                    !_fieldIsEmpty(props.searchFilters.workflowType))
                }
                value={props.searchFilters.agency}
              />
            </div>
            <div className={styles.orDiv}>{OR}</div>
            <div className={styles.formInput}>
              <DropdownInput
                label={WORKFLOW_TYPE_FIELD}
                name={'workflowType'}
                options={WORKFLOW_TYPE}
                onChange={props.updateSearchFilters}
                disabled={
                  !_fieldIsEmpty(props.searchFilters.iswc) ||
                  !_fieldIsEmpty(props.searchFilters.workCodes) ||
                  (props.searchFilters.agency !== '--' &&
                    !_fieldIsEmpty(props.searchFilters.agency))
                }
                value={props.searchFilters.workflowType}
              />
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default memo(WorkflowsAdvancedSearch);
