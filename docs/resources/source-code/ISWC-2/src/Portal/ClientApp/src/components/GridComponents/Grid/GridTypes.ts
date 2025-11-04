export interface IGridProps {
  headerCells: IGridHeaderCell[];
  gridRows: IGridRow[];
  componentInstance?: string;
  borderGrid?: boolean;
  cellPadding?: string;
  pagination?: boolean;
  paginationPositionLeft?: boolean;
  headerColor?: string;
  restrictRowHeightOnMobile?: boolean;
  fixedTableLayout?: boolean;
  onClickNext?: () => void;
  onClickPrevious?: () => void;
  startIndex?: number;
  rowsPerPage?: number;
  changeRowsPerPage?: (rowsPerPage: number) => void;
  numberOfLastPage?: number;
  lastPage?: boolean;
  setNumberOfLastPage?: (numberOfLastPage: number) => void;
  setLastPage?: (lastPage: boolean) => void;
}

export interface IGridState {
  gridRows: IGridRow[];
  activeHeaderCell?: string;
  currentPage: number;
  rowsPerPage: number;
  currentPageValue: string;
}

export interface IGridHeaderCell {
  text?: string;
  sortable?: boolean;
  type?: string;
  field: string;
  onClickHeaderCell?: () => void;
  checked?: boolean;
  hideInPublicMode?: boolean;
  show?: boolean;
}

export interface IGridRow {
  rowId: number;
  cells: IGridCell[];
  viewMore?: JSX.Element[];
  message?: string;
  subTable?: JSX.Element;
}

export interface IGridSubTableProps {
  title: string;
  headerCells: IGridHeaderCell[];
  rows: IGridRow[];
}

export interface IGridCell {
  element: JSX.Element;
  field: string;
  action?: string;
  hideInPublicMode?: boolean;
}
