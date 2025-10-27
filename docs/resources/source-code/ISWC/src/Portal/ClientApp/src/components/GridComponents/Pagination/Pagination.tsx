import React, { memo } from 'react';
import { IPaginationProps } from './PaginationTypes';
import styles from './Pagination.module.scss';
import PaginationButton from './PaginationButton/PaginationButton';
import { getStrings } from '../../../configuration/Localization';
import {
  PAGINATION_SHOW,
  PAGINATION_PER_PAGE,
  OF,
  PAGINATION_NEXT,
  PAGINATION_PREVIOUS,
  RESULTS,
} from '../../../consts';

const Pagination: React.FunctionComponent<IPaginationProps> = ({
  gridRowCount,
  numberOfPages,
  currentPage,
  currentPageValue,
  startIndex,
  rowsPerPage,
  setCurrentPage,
  changeRowsPerPage,
  onClickNext,
  onClickPrevious,
  numberOfLastPage,
  lastPage,
  setNumberOfLastPage,
  setLastPage
}) => {
  const strings = getStrings();

  const _changeRowsPerPage = (event: React.ChangeEvent<HTMLSelectElement>) => {
      changeRowsPerPage(Number(event.target.value));

      if (setLastPage !== undefined) {
        setLastPage(false);
      }
  };

  const _increasePageNumber = () => {
    if (currentPage !== numberOfPages) {
      setCurrentPage(currentPage + 1);
    }
  };

  const _decreasePageNumber = () => {
    if (currentPage !== 1) {
      setCurrentPage(currentPage - 1);
    }
  };

  const _handleOnBlur = () => {
    setCurrentPage(currentPage);
  };

  const _changePageNumber = (event: React.ChangeEvent<HTMLInputElement>) => {
    const { value } = event.target;
    const valueNumber = Number(value);

    if (valueNumber >= numberOfPages) {
      setCurrentPage(numberOfPages);
    } else if (valueNumber! < numberOfPages) {
      setCurrentPage(valueNumber);
    }
  };

  const renderOptions = () => {
    const options = [
      <option key={0}>20</option>,
      <option key={1}>30</option>,
      <option key={2}>40</option>,
      <option key={3}>50</option>,
      <option key={4}>60</option>,
      <option key={5}>70</option>,
      <option key={6}>80</option>,
      <option key={7}>90</option>,
      <option key={8}>100</option>,
    ];

    return options;
  };

  const renderDropDownDiv = () => {
    return (
      <div className={styles.showPerDiv}>
        {strings[PAGINATION_SHOW]}
        <select
          onChange={(event) => _changeRowsPerPage(event)}
          className={styles.resultsPerPageSelect}
          value={rowsPerPage}
        >
          {renderOptions()}
        </select>
        {strings[PAGINATION_PER_PAGE]}
      </div>
    );
  };

  const renderPageNumbers = () => {
    return (
      <div className={styles.pageNumberDiv}>
        <PaginationButton
          onClick={onClickPrevious ?? _decreasePageNumber}
          text={strings[PAGINATION_PREVIOUS]}
          iconLeft
          disabled={(onClickPrevious === undefined || startIndex === 0) && currentPage === 1}
        />
        <div className={styles.inputDiv}>
          <input
            className={styles.pageNumberInput}
            type={'text'}
            value={startIndex === undefined ? currentPageValue : startIndex / rowsPerPage + 1}
            onChange={_changePageNumber}
            onBlur={_handleOnBlur}
          />
          {strings[OF]} {gridRowCount === rowsPerPage && !lastPage ? 'multiple' : (numberOfPages > 1 ? numberOfPages : numberOfLastPage)}
        </div>
        <PaginationButton
          onClick={onClickNext ?? _increasePageNumber}
          text={strings[PAGINATION_NEXT]}
          disabled={
            gridRowCount !== rowsPerPage && (currentPage === numberOfPages || 0 === numberOfPages)
          }
        />
      </div>
    );
  };

  return (
    <div className={styles.paginationDiv}>
      <div className={styles.showPerPageDiv}>
        <div className={styles.resultsNumberDiv}>
          {`${gridRowCount}${gridRowCount === rowsPerPage ? '+' : ''}`} {strings[RESULTS]}
        </div>
        {renderOptions() !== null && renderDropDownDiv()}
      </div>
      {renderPageNumbers()}
    </div>
  );
};

export default memo(Pagination);
