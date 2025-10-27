import React, { memo } from 'react';
import styles from './SearchBy.module.scss';
import BasicFormInput from '../../../components/FormInput/BasicFormInput';
import SearchButton from './SearchButton';
import { ISearchByProps } from './SearchByTypes';
import { getStrings } from '../../../configuration/Localization';
import DropdownInput from '../../../components/FormInput/DropdownInput';
import { PUBLIC_MODE, getAgencies } from '../../../consts';
import { mapAgenciesToDropDownType } from '../../../shared/MappingObjects';

const SearchByWorkCode: React.FunctionComponent<ISearchByProps> = (props: ISearchByProps) => {
  const { AGENCY_WORK_CODE_FIELD, AGENCY, DATABASE } = getStrings();

  return (
    <form
      autoComplete="off"
      onSubmit={(e) => {
        e.preventDefault();
      }}
    >
      <div className={styles.formRow}>
        <div className={styles.formInput}>
          <BasicFormInput
            label={AGENCY_WORK_CODE_FIELD}
            name={'workCode'}
            onChange={props.onChange}
          />
        </div>
        <div className={styles.formInput}>
          <DropdownInput
            label={AGENCY}
            name={'agency'}
            onChange={props.onChange}
            options={mapAgenciesToDropDownType(getAgencies())}
            defaultToLoggedInAgency
          />
        </div>
        {process.env.REACT_APP_MODE !== PUBLIC_MODE && (
          <div className={styles.formInput}>
            <BasicFormInput label={DATABASE} name={'sourceDb'} onChange={props.onChange} />
          </div>
        )}
      </div>
      <SearchButton search={props.search} />
    </form>
  );
};

export default memo(SearchByWorkCode);
