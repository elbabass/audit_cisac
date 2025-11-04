import React, { memo } from 'react';
import styles from './SearchBy.module.scss';
import BasicFormInput from '../../../components/FormInput/BasicFormInput';
import SearchButton from './SearchButton';
import { ISearchByTitleProps } from './SearchByTypes';
import { getStrings } from '../../../configuration/Localization';
import { PUBLIC_MODE } from '../../../consts';

const SearchByTitleContributors: React.FunctionComponent<ISearchByTitleProps> = (
  props: ISearchByTitleProps,
) => {
  const {
    ENTER_TITLE,
    TITLE_FIELD,
    ENTER_CREATOR_DATA,
    ENTER_CREATOR_DATA_PUBLIC,
    CREATOR_SURNAMES,
    CREATOR_IP_NAME_NUMBER_FIELD,
    CREATOR_BASE_NUMBERS,
    OR,
  } = getStrings();
  return (
    <form
      autoComplete="off"
      onSubmit={(e) => {
        e.preventDefault();
      }}
    >
      <div>
        <div className={styles.descriptionText}>{ENTER_TITLE}</div>
      </div>
      <div className={styles.formRow}>
        <div className={styles.formInputLarge}>
          <BasicFormInput
            label={TITLE_FIELD}
            name={'title'}
            onChange={props.onChange}
            size={'large'}
          />
        </div>
      </div>
      <div>
        <div className={styles.descriptionText}>
          {process.env.REACT_APP_MODE === PUBLIC_MODE
            ? `2. ${ENTER_CREATOR_DATA_PUBLIC}`
            : `2. ${ENTER_CREATOR_DATA}`}
        </div>
      </div>
      <div className={styles.formRow}>
        <div className={styles.formInput}>
          <BasicFormInput
            label={CREATOR_SURNAMES}
            name={'surnames'}
            onChange={props.onChange}
            disabled={props.baseNumbers.length > 0 || props.nameNumbers.length > 0}
          />
        </div>
        <div className={styles.orDiv}>{OR}</div>
        <div className={styles.formInput}>
          <BasicFormInput
            label={CREATOR_IP_NAME_NUMBER_FIELD}
            name={'nameNumbers'}
            onChange={props.onChange}
            disabled={props.baseNumbers.length > 0 || props.surnames.length > 0}
            value={props.nameNumbers}
          />
        </div>
        {process.env.REACT_APP_MODE !== PUBLIC_MODE && <div className={styles.orDiv}>{OR}</div>}
        {process.env.REACT_APP_MODE !== PUBLIC_MODE && (
          <div className={styles.formInput}>
            <BasicFormInput
              label={CREATOR_BASE_NUMBERS}
              name={'baseNumbers'}
              onChange={props.onChange}
              disabled={props.surnames.length > 0 || props.nameNumbers.length > 0}
            />
          </div>
        )}
      </div>
      <SearchButton search={props.search} />
    </form>
  );
};

export default memo(SearchByTitleContributors);
