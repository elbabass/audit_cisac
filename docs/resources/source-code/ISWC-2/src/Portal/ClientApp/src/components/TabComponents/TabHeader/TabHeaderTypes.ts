import { ITab } from '../TabView/TabViewTypes';

export interface ITabHeaderProps {
  tabs: ITab[];
  activeTab: number;
  changeTab: (tab: number) => void;
  reportsPage?: boolean;
}
