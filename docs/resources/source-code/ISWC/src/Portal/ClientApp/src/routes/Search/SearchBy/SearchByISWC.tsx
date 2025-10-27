import React, { memo } from 'react';
import styles from './SearchBy.module.scss';
import BasicFormInput from '../../../components/FormInput/BasicFormInput';
import SearchButton from './SearchButton';
import { ISearchByProps } from './SearchByTypes';
import { getStrings } from '../../../configuration/Localization';

const SearchByISWC: React.FunctionComponent<ISearchByProps> = (props: ISearchByProps) => {
  const { ISWC_FIELD } = getStrings();
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
            label={ISWC_FIELD}
            name={'iswc'}
            onChange={props.onChange}
            placeholder={'T-000.000.000-0'}
            value={props.value}
          />
        </div>
      </div>
      <SearchButton search={props.search} />
    </form>
  );
};

export default memo(SearchByISWC);
