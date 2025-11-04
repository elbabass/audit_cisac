import React, { memo } from 'react';
import { ITabHeaderProps } from './TabHeaderTypes';
import styles from './TabHeader.module.scss';
import TabButton from '../TabButton/TabButton';
import { ITab } from '../TabView/TabViewTypes';

const TabHeader: React.FunctionComponent<ITabHeaderProps> = ({
  tabs,
  activeTab,
  changeTab,
  reportsPage,
}) => {
  const renderTabButtons = () => {
    return tabs.map((tab: ITab, index: number) => {
      if (!tab.disabled) {
        return (
          <TabButton
            text={tab.text}
            active={activeTab === index}
            onClickButton={() => changeTab(index)}
            key={index}
            reportsPage={reportsPage}
          />
        );
      } else return null;
    });
  };

  return (
    <div className={styles.container}>
      <div className={styles.tabButtonsDiv}>
        <div
          className={
            reportsPage ? `${styles.reportsButtonDiv} ${styles.buttonDiv}` : styles.buttonDiv
          }
        >
          {renderTabButtons()}
        </div>
      </div>
    </div>
  );
};

export default memo(TabHeader);
