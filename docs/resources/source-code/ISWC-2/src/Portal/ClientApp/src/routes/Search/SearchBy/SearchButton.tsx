import React, { memo } from 'react';
import ActionButton from '../../../components/ActionButton/ActionButton';
import styles from './SearchBy.module.scss';
import { getStrings } from '../../../configuration/Localization';
import { SEARCH } from '../../../consts';

const SearchButton: React.FunctionComponent<{ search: () => void }> = (props) => (
  <div>
    <div className={styles.formButton}>
      <ActionButton buttonText={getStrings()[SEARCH]} buttonAction={props.search} submitType />
    </div>
  </div>
);

export default memo(SearchButton);
