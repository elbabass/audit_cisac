export interface ITabViewProps {
  tabs: ITab[];
  headerColor?: string;
  tabClickAdditionalAction?: () => void;
  reportsPage?: boolean;
}

export interface ITabViewState {
  activeTab: number;
}

export interface ITab {
  text: string;
  component: React.ReactElement;
  disabled?: boolean;
}
