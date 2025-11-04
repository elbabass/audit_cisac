import React, { PureComponent } from 'react';
import { ITabViewProps, ITabViewState, ITab } from './TabViewTypes';
import styles from './TabView.module.scss';
import TabHeader from '../TabHeader/TabHeader';

export default class TabView extends PureComponent<ITabViewProps, ITabViewState> {
  constructor(props: ITabViewProps) {
    super(props);
    this.state = {
      activeTab: 0,
    };
  }

  _setActiveTab = (tab: number) => {
    this.setState({
      activeTab: tab,
    });
  };

  renderView = () => {
    const { tabs } = this.props;
    const { activeTab } = this.state;

    return tabs.map((tab: ITab, index: number) => {
      if (!tab.disabled) {
        return (
          <div
            className={styles.tabViewDiv}
            style={activeTab !== index ? { display: 'none' } : { display: 'block' }}
            key={index}
          >
            {tab.component}
          </div>
        );
      } else return null;
    });
  };

  render() {
    const { tabs, headerColor, tabClickAdditionalAction, reportsPage } = this.props;
    const { activeTab } = this.state;

    return (
      <div className={styles.container}>
        <div className={styles.tabHeader} style={headerColor ? { background: headerColor } : {}}>
          <TabHeader
            tabs={tabs}
            activeTab={activeTab}
            changeTab={(tab: number) => {
              this._setActiveTab(tab);
              tabClickAdditionalAction && tabClickAdditionalAction();
            }}
            reportsPage={reportsPage}
          />
        </div>

        <div>{this.renderView()}</div>
      </div>
    );
  }
}
