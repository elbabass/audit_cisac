import React, { FunctionComponent, memo } from 'react';
import ActionButton from '../../components/ActionButton/ActionButton';
import styles from './LandingPage.module.scss';
import { PUBLIC_SEARCH_PATH } from '../../consts';
import { getStrings } from '../../configuration/Localization';
import { ILanguageSelectionProps } from './LandingPageTypes';
import { Link } from 'react-router-dom';

const LanguageSelection: FunctionComponent<ILanguageSelectionProps> = (props) => {
    const { LANG_ENGLISH, LANG_FRENCH, LANG_SPANISH, SELECT_PREFERRED_LANGUAGE } = getStrings();
    const { selectLanguage } = props;

    const getLanguageString = (culture: string) => {
        let language = LANG_ENGLISH;
        switch (culture) {
            case 'fr':
                language = LANG_FRENCH;
                break;
            case 'es':
                language = LANG_SPANISH;
                break;
            case 'en':
            default:
                language = LANG_ENGLISH;
        }
        return language;
    };

    const renderButtons = (cultures: string[]) => {
        return cultures.map((culture) => (
            <Link
                key={culture}
                to={{
                    pathname: PUBLIC_SEARCH_PATH,
                    state: {
                        culture: culture,
                    },
                }}
                className={styles.languageButton}
                onClick={() => selectLanguage(culture)}
            >
                <ActionButton buttonText={getLanguageString(culture)} />
            </Link>
        ));
    };

    return (
        <div className={styles.contentContainer}>
            <div className={styles.selectLanguageText}>{SELECT_PREFERRED_LANGUAGE}</div>
            {renderButtons(['en', 'fr', 'es'])}
        </div>
    );
};

export default memo(LanguageSelection);
