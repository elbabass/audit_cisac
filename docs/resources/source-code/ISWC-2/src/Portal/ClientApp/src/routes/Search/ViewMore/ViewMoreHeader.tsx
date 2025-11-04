import React from 'react';
import styles from './ViewMore.module.scss';
import GridIconCell from '../../../components/GridComponents/GridIconCell/GridIconCell';
import {
  EXTERNAL_LINK_ICON,
  EDIT_ICON,
  MERGE_PATH,
  SUBMISSION_PATH,
  SUBMISSION_HISTORY_PATH,
  ADD_NEW_ICON_RED,
} from '../../../consts';
import GridCheckboxCell from '../../../components/GridComponents/GridCheckboxCell/GridCheckboxCell';
import { IViewMoreHeaderProps, IViewMoreHeaderState } from './ViewMoreTypes';
import { getStrings } from '../../../configuration/Localization';
import { Link } from 'react-router-dom';
import { mapSubmissionToSubmissionState } from '../../../shared/MappingObjects';
import { formatDateString, getLoggedInAgencyId } from '../../../shared/helperMethods';
import { IIswcModel } from '../../../redux/types/IswcTypes';

export default class ViewMoreHeader extends React.PureComponent<
  IViewMoreHeaderProps,
  IViewMoreHeaderState
> {
  agencyId: string;

  constructor(props: IViewMoreHeaderProps) {
    super(props);

    this.state = {
      mostRecentSubmissionFromAgency: undefined,
    };

    this.agencyId = getLoggedInAgencyId();
  }

  componentDidMount = () => {
    this._setMostRecentSubmissionFromAgency();
  };

  _setMostRecentSubmissionFromAgency = () => {
    const { iswc } = this.props;
    const works = iswc.works?.filter((work) => work.agency === this.agencyId);

    if (works && works.length > 0) {
      this.setState({
        mostRecentSubmissionFromAgency: mapSubmissionToSubmissionState(works[0]),
      });
    }
  };

  _getIswcLastUpdatedDate = (iswc: IIswcModel) => {
    const sortedWorks = iswc.works?.sort(
      (a, b) => new Date(b.lastModifiedDate).getTime() - new Date(a.lastModifiedDate).getTime(),
    );

    if (sortedWorks) return sortedWorks[0]?.lastModifiedDate;
  };

  render() {
    const {
      ORIGINAL_SUBMISSION_FIELD,
      LAST_UPDATE_FIELD,
      VIEW_SUBMISSION_HISTORY,
      UPDATE_SUBMISSION,
      ADD_TO_MERGE_LIST,
      VIEW_MERGE_LIST,
      NEW_SUBMISSION,
    } = getStrings();
    const { updateMergeList, iswc, mergeList, isSubmissionGrid, assignedRoles } = this.props;
    const { mostRecentSubmissionFromAgency } = this.state;
    const iswcLastUpdatedDate = this._getIswcLastUpdatedDate(iswc);

    return (
      <div className={styles.headerContainer}>
        <div className={`${styles.headerItem}`}>
          <div className={styles.headerTextContainer}>
            <div>
              <div className={styles.labelText}>{ORIGINAL_SUBMISSION_FIELD}</div>
              <div className={styles.regularText}>{formatDateString(iswc.createdDate)}</div>
            </div>
          </div>
          <div className={styles.headerTextContainer}>
            <div>
              <div className={styles.labelText}>{LAST_UPDATE_FIELD}</div>
              <div className={styles.regularText}>
                {iswcLastUpdatedDate && formatDateString(iswcLastUpdatedDate)}
              </div>
            </div>
          </div>
        </div>
        <div className={`${styles.headerItem} ${styles.divider}`}>
          {!isSubmissionGrid && (
            <div className={styles.headerTextContainer}>
              <div className={styles.actionText}>
                <Link
                  to={{
                    pathname: SUBMISSION_HISTORY_PATH,
                    state: {
                      preferredIswc: iswc,
                    },
                  }}
                >
                  <GridIconCell
                    icon={EXTERNAL_LINK_ICON}
                    text={VIEW_SUBMISSION_HISTORY}
                    alt={'external link icon'}
                    id={VIEW_SUBMISSION_HISTORY}
                  />
                </Link>
              </div>
            </div>
          )}
          {!isSubmissionGrid && assignedRoles?.update && (
            <div>
              <div className={styles.headerTextContainer}>
                <div className={styles.actionText}>
                  {mostRecentSubmissionFromAgency ? (
                    <Link
                      to={{
                        pathname: SUBMISSION_PATH,
                        state: {
                          ...mostRecentSubmissionFromAgency,
                          updateInstance: true,
                        },
                      }}
                    >
                      <GridIconCell
                        icon={EDIT_ICON}
                        text={UPDATE_SUBMISSION}
                        alt={'update icon'}
                        id={UPDATE_SUBMISSION}
                      />
                    </Link>
                  ) : (
                    <GridIconCell
                      icon={EDIT_ICON}
                      text={UPDATE_SUBMISSION}
                      alt={'update icon'}
                      id={UPDATE_SUBMISSION}
                    />
                  )}
                </div>
              </div>
              <div className={styles.headerTextContainer}>
                <div className={styles.actionText}>
                  {mostRecentSubmissionFromAgency ? (
                    <Link
                      to={{
                        pathname: SUBMISSION_PATH,
                        state: {
                          ...mostRecentSubmissionFromAgency,
                          agencyWorkCode: [
                            { agencyWorkCode: '', agencyName: getLoggedInAgencyId() },
                          ],
                          updateInstance: false,
                        },
                      }}
                    >
                      <GridIconCell
                        icon={ADD_NEW_ICON_RED}
                        text={NEW_SUBMISSION}
                        alt={'add new icon'}
                        id={NEW_SUBMISSION}
                      />
                    </Link>
                  ) : (
                    <GridIconCell
                      icon={ADD_NEW_ICON_RED}
                      text={NEW_SUBMISSION}
                      alt={'add new icon'}
                      id={NEW_SUBMISSION}
                    />
                  )}
                </div>
              </div>
            </div>
          )}
        </div>
        {mergeList && assignedRoles?.update && (
          <div className={`${styles.headerItem} ${styles.divider}`}>
            <div className={styles.headerTextContainer}>
              <div className={styles.actionText}>
                <GridCheckboxCell
                  text={ADD_TO_MERGE_LIST}
                  onClickCheckbox={updateMergeList}
                  checked={mergeList.find((x) => x.iswc === iswc.iswc) !== undefined}
                />
              </div>
            </div>
            {!isSubmissionGrid && (
              <div className={styles.headerTextContainer}>
                <div className={styles.actionText}>
                  <Link to={MERGE_PATH}>
                    <GridIconCell
                      icon={EXTERNAL_LINK_ICON}
                      text={VIEW_MERGE_LIST}
                      alt={'external link icon'}
                      id={VIEW_MERGE_LIST}
                    />
                  </Link>
                </div>
              </div>
            )}
          </div>
        )}
      </div>
    );
  }
}
