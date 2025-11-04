import React, { memo } from 'react';
import styles from './SearchBy.module.scss';
import BasicFormInput from '../../../components/FormInput/BasicFormInput';
import SearchButton from './SearchButton';
import { ISearchByCreatorProps } from './SearchByTypes';
import { getStrings } from '../../../configuration/Localization';
import { PUBLIC_MODE } from '../../../consts';

const SearchByCreator: React.FunctionComponent<ISearchByCreatorProps> = (
  props: ISearchByCreatorProps,
) => {
  const {
    CREATOR_IP_NAME_NUMBER_FIELD,
    CREATOR_BASE_NUMBERS,
    ENTER_CREATOR_DATA_NO_SURNAME_PUBLIC,
    ENTER_CREATOR_DATA_NO_SURNAME,
    OR,
  } = getStrings();

  return (
    <form
      autoComplete="off"
      onSubmit={(e) => {
        e.preventDefault();
      }}
    >
      <div className={styles.descriptionText}>
        {process.env.REACT_APP_MODE === PUBLIC_MODE
          ? ENTER_CREATOR_DATA_NO_SURNAME_PUBLIC
          : ENTER_CREATOR_DATA_NO_SURNAME}
      </div>
      <div className={styles.formRow}>
        <div className={styles.formInput}>
          <BasicFormInput
            label={CREATOR_IP_NAME_NUMBER_FIELD}
            name={'creatorNameNumbers'}
            onChange={props.onChange}
            disabled={props.creatorBaseNumbers.length > 0}
            value={props.creatorNameNumbers}
          />
        </div>
        {process.env.REACT_APP_MODE !== PUBLIC_MODE && <div className={styles.orDiv}>{OR}</div>}
        {process.env.REACT_APP_MODE !== PUBLIC_MODE && (
          <div className={styles.formInput}>
            <BasicFormInput
              label={CREATOR_BASE_NUMBERS}
              name={'creatorBaseNumbers'}
              onChange={props.onChange}
              disabled={props.creatorNameNumbers.length > 0}
              value={props.creatorBaseNumbers}
            />
          </div>
        )}
      </div>
      <SearchButton search={props.search} />
    </form>
  );
};

export default memo(SearchByCreator);
