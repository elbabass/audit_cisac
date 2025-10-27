export interface IPaginationProps {
  gridRowCount: number;
  numberOfPages: number;
  currentPageValue: string;
  currentPage: number;
  startIndex?: number;
  rowsPerPage: number;
  setCurrentPage: (pageNumber: number) => void;
  changeRowsPerPage: (rowsPerPage: number) => void;
  onClickNext?: () => void;
  onClickPrevious?: () => void;
  numberOfLastPage?: number;
  lastPage?: boolean;
  setNumberOfLastPage?: (numberOfLastPage: number) => void;
  setLastPage?: (lastPage: boolean) => void;
}
