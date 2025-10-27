import { IGridHeaderCell, IGridRow } from '../../../components/GridComponents/Grid/GridTypes';

export interface ISubmissionGridProps {
  headerCells: IGridHeaderCell[];
  gridRows: IGridRow[];
  actionButtonText?: string;
  addRowToGrid?: () => void;
  showActionButton?: boolean;
}

export interface ISubmissionGridState {
  headerCells: IGridHeaderCell[];
  gridRows: IGridRow[];
}
