import * as React from 'react';
import styles from './SubmissionAdditionalDetails.module.scss';
import { ISubmissionAdditionalDetailsProps } from './SubmissionAdditionalDetailsTypes';
import TabView from '../../../components/TabComponents/TabView/TabView';
import SelectPreferredIswc from './SelectPreferredIswc/SelectPreferredIswc';
import DisambiguateIswcs from './DisambiguateIswcs/DisambiguateIswcs';
import { ITab } from '../../../components/TabComponents/TabView/TabViewTypes';
import { getStrings } from '../../../configuration/Localization';
import {
  SELECT_PREFERRED_ISWC,
  DISAMBIGUATE_ALL_ISWCS,
  MATCHES_FOUND,
  SELECT_ISWC_OR_DISAMBIGUATE,
  SELECT_ISWC,
  SELECT_ISWC_INELIGIBLE,
} from '../../../consts';

const SubmissionAdditionalDetails: React.FunctionComponent<ISubmissionAdditionalDetailsProps> = ({
  preferredIswc,
  updateSubmissionDataString,
  updateSubmissionDataArray,
  addElementToArray,
  removeElementFromArray,
  performers,
  disambiguationReason,
  disambiguationIswcs,
  bvltr,
  standardInstrumentation,
  potentialMatches,
  searchByIswc,
  verifiedSubmission,
  setDisambiguation,
  updateInstance,
}) => {
  const strings = getStrings();

  const _getTabs = () => {
    const selectPreferredIswcTab: ITab = {
      text: strings[SELECT_PREFERRED_ISWC],
      component: (
        <SelectPreferredIswc
          preferredIswc={preferredIswc}
          updateSubmissionDataString={updateSubmissionDataString}
          potentialMatches={potentialMatches}
          searchByIswc={searchByIswc}
        />
      ),
    };
    const disambiguateAllIswcsTab: ITab = {
      text: strings[DISAMBIGUATE_ALL_ISWCS],
      component: (
        <DisambiguateIswcs
          updateSubmissionDataArray={updateSubmissionDataArray}
          updateSubmissionDataString={updateSubmissionDataString}
          addElementToArray={addElementToArray}
          removeElementFromArray={removeElementFromArray}
          performers={performers}
          disambiguationReason={disambiguationReason}
          disambiguationIswcs={disambiguationIswcs}
          bvltr={bvltr}
          standardInstrumentation={standardInstrumentation}
          potentialMatches={potentialMatches}
          searchByIswc={searchByIswc}
        />
      ),
    };

    if (verifiedSubmission.iswcEligible) {
      if (updateInstance) {
        return [selectPreferredIswcTab];
      }
      return [selectPreferredIswcTab, disambiguateAllIswcsTab];
    }

    return [selectPreferredIswcTab];
  };

  const getDescriptionString = () => {
    if (verifiedSubmission.iswcEligible) {
      if (updateInstance) {
        return strings[SELECT_ISWC];
      }
      return strings[SELECT_ISWC_OR_DISAMBIGUATE];
    }

    return strings[SELECT_ISWC_INELIGIBLE];
  };

  return (
    <div>
      <div className={styles.container}>
        <div className={styles.sectionTitle}>{strings[MATCHES_FOUND]}</div>
        <div className={styles.description}>{getDescriptionString()}</div>
      </div>
      <TabView
        tabs={_getTabs()}
        headerColor={'transparent'}
        tabClickAdditionalAction={setDisambiguation}
      />
    </div>
  );
};

export default React.memo(SubmissionAdditionalDetails);
