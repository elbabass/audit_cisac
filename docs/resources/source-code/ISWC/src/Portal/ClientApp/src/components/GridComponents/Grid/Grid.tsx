import React, { PureComponent } from 'react';
import { IGridProps, IGridHeaderCell, IGridRow, IGridState } from './GridTypes';
import styles from './Grid.module.scss';
import GridHeaderCell from '../GridHeaderCell/GridHeaderCell';
import GridRow from '../GridRow/GridRow';
import Pagination from '../Pagination/Pagination';
import {
  SELECT_ALL_FIELD,
  PUBLIC_MODE,
  DATE_TYPE,
  NUMBER_TYPE,
  ISWC_FIELD,
  SEARCH,
} from '../../../consts';
import GridCheckboxCell from '../GridCheckboxCell/GridCheckboxCell';
import { getStrings } from '../../../configuration/Localization';

export default class Grid extends PureComponent<IGridProps, IGridState> {
  ascending: boolean;
  constructor(props: IGridProps) {
    super(props);

    this.state = {
      gridRows: props.gridRows,
      activeHeaderCell: '',
      rowsPerPage: this.props.rowsPerPage ?? 20,
      currentPage: 1,
      currentPageValue: '1',
    };

    this.ascending = false;

    if (this.props.setLastPage !== undefined && props.gridRows.length < this.state.rowsPerPage)
      this.props.setLastPage(true);
  }

  componentDidUpdate = (prevProps: IGridProps) => {
    const { gridRows } = this.props;
    const { activeHeaderCell } = this.state;

    if (prevProps.gridRows !== gridRows) {
      this.setState({ gridRows });
      if (activeHeaderCell) {
        let headerCell = this._getHeaderCell(activeHeaderCell);
        if (headerCell !== undefined)
          this._sortTableByColumn(headerCell.field, headerCell.type, true);
      }
    }
  };

  _changeRowsPerPage = (rowsPerPage: number) => {
    const { currentPage } = this.state;

    if (this.props.changeRowsPerPage) this.props.changeRowsPerPage(rowsPerPage);

    this.setState(
      {
        rowsPerPage,
      },
      () => {
        if (currentPage > this._getNumberOfPages()) {
          this.setState({
            currentPage: 1,
            currentPageValue: '1',
          });
        }
      },
    );
  };

  _setCurrentPage = (pageNumber: number) => {
    if (!pageNumber) {
      return this.setState({
        currentPageValue: '',
      });
    }

    this.setState({
      currentPage: pageNumber,
      currentPageValue: pageNumber.toString(),
    });
  };

  _getNumberOfPages = () => {
    const { gridRows, rowsPerPage } = this.state;
    return Math.ceil(gridRows.length / rowsPerPage);
  };

  _getColumnHeader = (field: string) => {
    const { headerCells } = this.props;

    for (let x = 0; x < headerCells.length; x++) {
      if (headerCells[x].field === field) {
        return headerCells[x].text ? headerCells[x].text + ':' : '';
      }
    }

    return '';
  };

  _sortTableByColumn = (field: string, type?: string, revertAscending = false) => {
    const { activeHeaderCell } = this.state;
    const { gridRows, componentInstance } = this.props;
    const tableContentsCopy = this._createCopyOfTableContents(gridRows);

    if (revertAscending) this.ascending = !this.ascending;

    if (activeHeaderCell !== field) this.ascending = false;

    this.setState({
      activeHeaderCell: field,
    });

    tableContentsCopy.sort((a: IGridRow, b: IGridRow) => {
      const cellA = a.cells.filter(function (cell) {
        return cell.field === field;
      });

      const cellB = b.cells.filter(function (cell) {
        return cell.field === field;
      });

      let newA =
        field === ISWC_FIELD && componentInstance === SEARCH
          ? cellA[0].element.props.children[0].props.text
          : cellA[0].element.props.text;
      let newB =
        field === ISWC_FIELD && componentInstance === SEARCH
          ? cellB[0].element.props.children[0].props.text
          : cellB[0].element.props.text;

      if (type === DATE_TYPE) {
        newA = new Date(newA);
        newB = new Date(newB);

        if (!this.ascending) return newA - newB;

        return newB - newA;
      }

      if (type === NUMBER_TYPE) {
        newA = parseInt(newA);
        newB = parseInt(newB);

        if (!this.ascending) return newA - newB;

        return newB - newA;
      }

      if (!this.ascending) return String(newA).localeCompare(String(newB));

      return String(newB).localeCompare(String(newA));
    });

    this.setState({
      gridRows: tableContentsCopy,
      activeHeaderCell: field,
    });

    this.ascending = !this.ascending;
  };

  _createCopyOfTableContents(gridRows: IGridRow[]) {
    return [...gridRows];
  }

  _onClickHeaderCell = (headerCell: IGridHeaderCell) => {
    if (headerCell.onClickHeaderCell) {
      return headerCell.onClickHeaderCell();
    } else if (headerCell.sortable) {
      return this._sortTableByColumn(headerCell.field, headerCell.type);
    }
  };

  _getNumberOfColumns = () => {
    const { headerCells } = this.props;
    if (process.env.REACT_APP_MODE === PUBLIC_MODE) {
      return headerCells.filter((x) => !x.hideInPublicMode).length;
    }

    return headerCells.length;
  };

  _getHeaderCell = (field: string) => {
    const { headerCells } = this.props;
    for (let i = 0; i < headerCells.length; i++) {
      if (headerCells[i].field === field) return headerCells[i];
    }
  };

  renderHeaderCells = () => {
    const { headerCells } = this.props;
    const { activeHeaderCell } = this.state;
    const { SELECT_ALL } = getStrings();

    return headerCells.map((headerCell: IGridHeaderCell, index: number) => {
      if (!(process.env.REACT_APP_MODE === PUBLIC_MODE && headerCell.hideInPublicMode)) {
        if (headerCell.field === SELECT_ALL_FIELD) {
          return (
            <td className={styles.selectAllContainer} key={index}>
              <GridCheckboxCell
                text={SELECT_ALL}
                onClickCheckbox={
                  headerCell.sortable || headerCell.onClickHeaderCell
                    ? () => this._onClickHeaderCell(headerCell)
                    : undefined
                }
                checked={headerCell.checked}
              />
            </td>
          );
        }

        return (
          <GridHeaderCell
            onClickHeaderCell={
              headerCell.sortable || headerCell.onClickHeaderCell
                ? () => this._onClickHeaderCell(headerCell)
                : undefined
            }
            text={headerCell.text}
            key={index}
            isActive={activeHeaderCell === headerCell.field}
            isSortable={headerCell.sortable}
            ascending={this.ascending}
          />
        );
      } else return null;
    });
  };

  renderTableBody = () => {
    const { gridRows, rowsPerPage, currentPage } = this.state;
    const { cellPadding, pagination, restrictRowHeightOnMobile } = this.props;
    let indexOfLastRow, indexOfFirstRow, paginationRows: IGridRow[];

    if (pagination) {
      indexOfLastRow = currentPage * rowsPerPage;
      indexOfFirstRow = indexOfLastRow - rowsPerPage;
      paginationRows = gridRows.slice(indexOfFirstRow, indexOfLastRow);

      return paginationRows.map((gridRow: IGridRow, index: number) => (
        <GridRow
          gridRow={gridRow}
          getColumnHeader={this._getColumnHeader}
          key={gridRow.rowId}
          cellPadding={cellPadding}
          numColumns={this._getNumberOfColumns()}
          restrictRowHeightOnMobile={restrictRowHeightOnMobile}
          lastRow={paginationRows.length === index + 1}
          message={gridRow.message}
        />
      ));
    }

    return gridRows.map((gridRow: IGridRow, index: number) => (
      <GridRow
        gridRow={gridRow}
        getColumnHeader={this._getColumnHeader}
        key={index}
        cellPadding={cellPadding}
        numColumns={this._getNumberOfColumns()}
        lastRow={gridRows.length === index + 1}
        message={gridRow.message}
      />
    ));
  };

  render() {
    const {
      borderGrid,
      gridRows,
      pagination,
      paginationPositionLeft,
      headerColor,
      fixedTableLayout,
      componentInstance,
    } = this.props;
    const { currentPageValue, currentPage, rowsPerPage } = this.state;

    return (
      <div className={styles.container}>
        <div className={styles.tableContainer}>
          {pagination ? (
            <div
              style={
                paginationPositionLeft
                  ? { marginRight: 'auto', marginBottom: '1.5rem' }
                  : { marginLeft: 'auto', marginBottom: '1.5rem' }
              }
            >
              <Pagination
                gridRowCount={gridRows.length}
                changeRowsPerPage={this._changeRowsPerPage}
                numberOfPages={this._getNumberOfPages()}
                setCurrentPage={this._setCurrentPage}
                currentPageValue={currentPageValue}
                currentPage={currentPage}
                startIndex={this.props.startIndex}
                rowsPerPage={rowsPerPage}
                onClickNext={this.props.onClickNext}
                onClickPrevious={this.props.onClickPrevious}
                numberOfLastPage={this.props.numberOfLastPage}
                lastPage={this.props.lastPage}
                setNumberOfLastPage={this.props.setNumberOfLastPage}
                setLastPage={this.props.setLastPage}
              />
            </div>
          ) : null}

          {/* NK180924 TODO: Update tooltip when business definition is provided */}
          {componentInstance == SEARCH && process.env.REACT_APP_MODE !== PUBLIC_MODE &&
            <div className={styles.tableBanner} title='ISWC with Provisional Status'>
              <span className={styles.indicator}>ISWC with Provisional Status</span>
            </div>
          }

          <table
            className={borderGrid ? `${styles.table} ${styles.tableBorder}` : styles.table}
            style={{ tableLayout: fixedTableLayout ? 'fixed' : 'auto' }}
          >
            <thead
              className={
                borderGrid ? `${styles.tableHeader} ${styles.tableBorder}` : styles.tableHeader
              }
              style={headerColor ? { background: headerColor } : {}}
            >
              <tr>{this.renderHeaderCells()}</tr>
            </thead>
            {this.renderTableBody()}
          </table>
          {pagination ? (
            <div
              className={styles.bottomPagination}
              style={
                paginationPositionLeft
                  ? { marginRight: 'auto', marginTop: '1.5rem' }
                  : { marginLeft: 'auto', marginTop: '1.5rem' }
              }
            >
              <Pagination
                gridRowCount={gridRows.length}
                changeRowsPerPage={this._changeRowsPerPage}
                numberOfPages={this._getNumberOfPages()}
                setCurrentPage={this._setCurrentPage}
                currentPageValue={currentPageValue}
                currentPage={currentPage}
                startIndex={this.props.startIndex}
                rowsPerPage={rowsPerPage}
                onClickNext={this.props.onClickNext}
                onClickPrevious={this.props.onClickPrevious}
                numberOfLastPage={this.props.numberOfLastPage}
                lastPage={this.props.lastPage}
                setNumberOfLastPage={this.props.setNumberOfLastPage}
                setLastPage={this.props.setLastPage}
              />
            </div>
          ) : null}
        </div>
      </div>
    );
  }
}
