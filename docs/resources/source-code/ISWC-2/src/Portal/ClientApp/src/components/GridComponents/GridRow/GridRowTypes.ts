import { IGridRow } from '../Grid/GridTypes';

export interface IGridRowProps {
  gridRow: IGridRow;
  getColumnHeader: (field: string) => string;
  cellPadding?: string;
  addToSelectedRows?: (rowId: number) => void;
  numColumns: number;
  restrictRowHeightOnMobile?: boolean;
  lastRow?: boolean;
  message?: string;
}

export interface IGridRowState {
  displayViewMoreDiv?: boolean;
  viewMoreIndex: number;
}
