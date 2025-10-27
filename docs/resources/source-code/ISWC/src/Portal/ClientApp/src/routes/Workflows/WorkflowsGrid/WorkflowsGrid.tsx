import * as React from 'react';
import {
  IWorkflowsGridProps,
  IWorkflowsGridState,
  IWorkflowUpdateRequestBody,
} from './WorkflowGridTypes';
import { IGridHeaderCell, IGridRow } from '../../../components/GridComponents/Grid/GridTypes';
import {
  ISWC_FIELD,
  WORKFLOW_ID_FIELD,
  WORKFLOW_TYPE_FIELD,
  DATE_FIELD,
  ASSIGNED_TO_FIELD,
  VIEW_MORE_FIELD,
  STATUS_FIELD,
  SELECT_ALL_FIELD,
  VIEW_MORE_ICON,
  VIEW_MORE_ACTION,
  STATUS_PENDING_ICON,
  CHECKBOX_FIELD,
  STATUS_APPROVED_ICON,
  ORIGINATING_AGENCY_FIELD,
  STATUS_REJECTED_ICON,
  CHECKMARK_ICON,
  CLOSE_ICON_WHITE,
  CANCEL,
  WF_UPDATED_SUCCESSFULLY,
  ERROR_OCCURED,
  UPDATE_WF_TASKS,
  OKAY,
  APPROVE_ALL,
  ITEMS_SELECTED,
  REJECT_ALL,
  WF_TASKS_WILL_BE,
  ARE_YOU_SURE,
  DATE_TYPE,
  YES,
  COMMENTS,
} from '../../../consts';
import Grid from '../../../components/GridComponents/Grid/Grid';
import GridTextCell from '../../../components/GridComponents/GridTextCell/GridTextCell';
import GridIconCell from '../../../components/GridComponents/GridIconCell/GridIconCell';
import styles from './WorkflowGrid.module.scss';
import { getStrings } from '../../../configuration/Localization';
import ViewMore from '../../Search/ViewMore/ViewMore';
import GridCheckboxCell from '../../../components/GridComponents/GridCheckboxCell/GridCheckboxCell';
import { IWorkflow } from '../../../redux/types/IswcTypes';
import { formatDateString, validateIswcAndFormat, _getAgency } from '../../../shared/helperMethods';
import IconActionButton from '../../../components/ActionButton/IconActionButton';
import Modal from '../../../components/Modal/Modal';

export default class WorkflowsGrid extends React.PureComponent<
  IWorkflowsGridProps,
  IWorkflowsGridState
> {
  constructor(props: IWorkflowsGridProps) {
    super(props);

    this.state = {
      selectedRows: [],
      allSelected: false,
      workflowsToUpdate: [],
      isModalOpen: false,
      responseModalOpen: false,
      statusToUpdateTo: 'Approved',
      updateSuccessMessage: '',
    };
  }

  componentDidUpdate(prevProps: IWorkflowsGridProps, prevState: IWorkflowsGridState) {
    const { updating, search } = this.props;
    const { isModalOpen, responseModalOpen } = this.state;
    if (prevProps.updating && !updating && prevState.isModalOpen && isModalOpen) {
      this.setState({ workflowsToUpdate: [], isModalOpen: !isModalOpen, responseModalOpen: true });
    }

    if (prevState.responseModalOpen && !responseModalOpen) {
      search();
      this.setState({ workflowsToUpdate: [] });
    }
  }

  _addToSelectedRows = (rowId: number, workflow: IWorkflow) => {
    const { selectedRows, workflowsToUpdate } = this.state;
    var rowExistsInArray = selectedRows.some(function (row) {
      return row === rowId;
    });

    if (rowExistsInArray) {
      this.setState({
        selectedRows: selectedRows.filter((iswc) => {
          return iswc !== rowId;
        }),
        allSelected: false,
      });

      let wfIndex = workflowsToUpdate.findIndex((x) => x.taskId === workflow.workflowTaskId);
      let wfsArr = workflowsToUpdate
        .slice(0, wfIndex)
        .concat(workflowsToUpdate.slice(wfIndex + 1, workflowsToUpdate.length));

      this.setState({ workflowsToUpdate: wfsArr });
    } else {
      this.setState({
        selectedRows: selectedRows.concat(rowId),
      });
      let requestBody: IWorkflowUpdateRequestBody = {
        taskId: workflow.workflowTaskId,
        workflowType: workflow.workflowType,
        status: '',
      };
      this.setState({ workflowsToUpdate: [...workflowsToUpdate, requestBody] });
    }
  };

  _selectAllRows = (number: number) => {
    const { allSelected } = this.state;

    if (allSelected) {
      this.setState({
        selectedRows: [],
        workflowsToUpdate: [],
        allSelected: false,
      });
    } else {
      const newArray = Array.from(Array(number).keys());
      this.setState({
        selectedRows: newArray,
        allSelected: true,
      });
      this._setAllWorkflowTasksToBeUpdated();
    }
  };

  _setAllWorkflowTasksToBeUpdated = () => {
    const { workflows } = this.props;

    let workflowsToUpdate: IWorkflowUpdateRequestBody[] = [];

    workflows.forEach((wf) => {
      if (wf.status === 'Outstanding') {
        let requestBody: IWorkflowUpdateRequestBody = {
          taskId: wf.workflowTaskId,
          workflowType: wf.workflowType,
          status: '',
        };
        workflowsToUpdate.push(requestBody);
      }

      this.setState({ workflowsToUpdate });
    });
  };

  _updateAllWorkflowTasks = () => {
    const { workflowsToUpdate, statusToUpdateTo, workflowMessage } = this.state;
    const { updateWorkflows, workflows } = this.props;
    let workflowsToUpdateCopy = [...workflowsToUpdate];

    if (workflowMessage) {
      for (let x = 0; x < workflowsToUpdateCopy.length; x++) {
        workflowsToUpdateCopy[x].workflowMessage = workflowMessage;
      }
    }

    workflowsToUpdate.forEach((wf) => (wf.status = statusToUpdateTo));
    updateWorkflows(workflows[0]?.assignedSociety, workflowsToUpdateCopy);
  };

  _updateWorkflowTask = (workflow: IWorkflow, status: string, workflowIndex: number) => {
    let requestBody: IWorkflowUpdateRequestBody[] = [
      {
        taskId: workflow.workflowTaskId,
        workflowType: workflow.workflowType,
        status,
      },
    ];
    this.setState({
      workflowsToUpdate: requestBody,
      statusToUpdateTo: status,
      selectedRows: [workflowIndex],
    });

    this._toggleModal();
  };

  _toggleModal = () => {
    const { isModalOpen } = this.state;
    this.setState({ isModalOpen: !isModalOpen }, () => {
      if (!isModalOpen) {
        this.setState({
          workflowMessage: undefined,
        });
      }
    });
  };

  _toggleResponseModal = () => {
    const { responseModalOpen } = this.state;
    this.setState({ responseModalOpen: !responseModalOpen });
  };

  _updateComment = (workflowMessage: string) => {
    this.setState({
      workflowMessage: workflowMessage,
    });
  };

  _renderStatusIcon = (workflow: IWorkflow, workflowIndex: number) => {
    const { APPROVE, REJECT, APPROVED, REJECTED } = getStrings();
    const { showAssigned } = this.props;
    let statusIcon = STATUS_PENDING_ICON;
    switch (workflow.status) {
      case 'Outstanding':
        statusIcon = STATUS_PENDING_ICON;
        break;
      case 'Approved':
        statusIcon = STATUS_APPROVED_ICON;
        break;
      case 'Rejected':
        statusIcon = STATUS_REJECTED_ICON;
    }

    return (
      <div className={styles.statusCell}>
        {workflow.status === 'Rejected' && workflow.workflowMessage !== undefined ? (
          <GridIconCell
            icon={statusIcon}
            alt={workflow.status}
            hoverText={workflow.workflowMessage}
          />
        ) : (
          <GridIconCell icon={statusIcon} alt={workflow.status} hoverText={workflow.status} />
        )}
        {workflow.status === 'Outstanding' && showAssigned && (
          <div className={styles.statusCellTextDiv}>
            <div
              className={styles.statusCellText}
              onClick={() => this._updateWorkflowTask(workflow, APPROVED, workflowIndex)}
            >
              {APPROVE}
            </div>
            <div
              className={styles.statusCellText}
              onClick={() => this._updateWorkflowTask(workflow, REJECTED, workflowIndex)}
            >
              {REJECT}
            </div>
          </div>
        )}
      </div>
    );
  };

  _renderSelectCell = (workflow: IWorkflow, index: number) => {
    const { selectedRows } = this.state;
    const { showAssigned } = this.props;
    if (workflow.status === 'Outstanding' && showAssigned)
      return (
        <GridCheckboxCell
          onClickCheckbox={() => this._addToSelectedRows(index, workflow)}
          checked={selectedRows.indexOf(index) > -1}
        />
      );
    return <div></div>;
  };

  _renderPromptModal = () => {
    const { isModalOpen, workflowsToUpdate, statusToUpdateTo } = this.state;
    const { updating } = this.props;
    const strings = getStrings();

    return (
      <Modal
        headerText={strings[UPDATE_WF_TASKS]}
        subHeaderText={statusToUpdateTo === 'Rejected' ? strings[COMMENTS] : undefined}
        onChangeInput={this._updateComment}
        bodyText={`${workflowsToUpdate.length} ${strings[WF_TASKS_WILL_BE]} ${statusToUpdateTo}. ${strings[ARE_YOU_SURE]}`}
        isModalOpen={isModalOpen}
        toggleModal={this._toggleModal}
        loading={updating || false}
        leftButtonText={strings[CANCEL]}
        leftButtonAction={this._toggleModal}
        rightButtonText={strings[YES]}
        rightButtonAction={this._updateAllWorkflowTasks}
        type={statusToUpdateTo === 'Rejected' ? 'input' : 'confirm'}
      />
    );
  };

  _renderResponseModal = () => {
    const { responseModalOpen } = this.state;
    const { updateSuccessful } = this.props;
    const strings = getStrings();

    return (
      <Modal
        headerText={strings[UPDATE_WF_TASKS]}
        bodyText={updateSuccessful ? strings[WF_UPDATED_SUCCESSFULLY] : strings[ERROR_OCCURED]}
        isModalOpen={responseModalOpen}
        toggleModal={this._toggleModal}
        rightButtonText={strings[OKAY]}
        rightButtonAction={this._toggleResponseModal}
        type={'confirm'}
      />
    );
  };

  _toggleAcceptModal = () => {
    this.setState({ statusToUpdateTo: 'Approved' });
    this._toggleModal();
  };

  _toggleRejectModal = () => {
    this.setState({ statusToUpdateTo: 'Rejected' });
    this._toggleModal();
  };

  render() {
    const { allSelected, workflowsToUpdate } = this.state;
    const { workflows, showAssigned, hideSelectButtons } = this.props;
    const strings = getStrings();

    let headerCells: IGridHeaderCell[] = [
      { text: strings[WORKFLOW_ID_FIELD], field: WORKFLOW_ID_FIELD, sortable: true },
      { text: strings[ISWC_FIELD], field: ISWC_FIELD, sortable: true },
      { text: strings[WORKFLOW_TYPE_FIELD], field: WORKFLOW_TYPE_FIELD, sortable: true },
      { text: strings[DATE_FIELD], field: DATE_FIELD, sortable: true, type: DATE_TYPE },
      {
        text: strings[ORIGINATING_AGENCY_FIELD],
        field: ORIGINATING_AGENCY_FIELD,
        sortable: true,
      },
      { text: strings[ASSIGNED_TO_FIELD], field: ASSIGNED_TO_FIELD, sortable: true },
      { field: VIEW_MORE_FIELD },
      { text: strings[STATUS_FIELD], field: STATUS_FIELD },
      {
        text: strings[SELECT_ALL_FIELD],
        field: SELECT_ALL_FIELD,
        onClickHeaderCell: () => showAssigned && this._selectAllRows(getGridRows().length),
        checked: allSelected,
      },
    ];
    const getGridRows = (): IGridRow[] => {
      const workflowsGridRows: IGridRow[] = [];

      workflows.forEach((workflow, index) => {
        workflowsGridRows.push({
          rowId: index,
          cells: [
            {
              element: <GridTextCell text={workflow.workflowTaskId.toString()} />,
              field: WORKFLOW_ID_FIELD,
            },
            {
              element: <GridTextCell text={validateIswcAndFormat(workflow.iswcMetadata?.iswc)} />,
              field: ISWC_FIELD,
            },
            { element: <GridTextCell text={workflow.workflowType} />, field: WORKFLOW_TYPE_FIELD },
            {
              element: <GridTextCell text={formatDateString(workflow.createdDate)} />,
              field: DATE_FIELD,
            },
            {
              element: <GridTextCell text={_getAgency(workflow.originatingSociety)} />,
              field: ORIGINATING_AGENCY_FIELD,
            },
            {
              element: <GridTextCell text={_getAgency(workflow.assignedSociety)} />,
              field: ASSIGNED_TO_FIELD,
            },
            {
              element: (
                <GridIconCell
                  text={strings[VIEW_MORE_FIELD]}
                  icon={VIEW_MORE_ICON}
                  alt={'View More Icon'}
                  clickable
                  id={strings[VIEW_MORE_FIELD]}
                />
              ),
              field: VIEW_MORE_FIELD,
              // Handled in GridRow.tsx
              action: VIEW_MORE_ACTION,
            },
            {
              element: this._renderStatusIcon(workflow, index),
              field: STATUS_FIELD,
            },
            {
              element: this._renderSelectCell(workflow, index),
              field: CHECKBOX_FIELD,
            },
          ],
          viewMore: [<ViewMore iswcModel={workflow.iswcMetadata} isSubmissionGrid />],
        });
      });

      return workflowsGridRows;
    };

    if (hideSelectButtons) headerCells.pop();

    return (
      <div className={styles.container}>
        {this._renderPromptModal()}
        {this._renderResponseModal()}
        {!hideSelectButtons && (
          <div className={styles.selectedRowsDiv}>
            <div className={styles.selectedRowsText}>
              {workflowsToUpdate.length} {strings[ITEMS_SELECTED]}
            </div>
            <div className={`${styles.actionButton} ${styles.marginRight}`}>
              <IconActionButton
                icon={CHECKMARK_ICON}
                buttonText={strings[APPROVE_ALL]}
                buttonAction={this._toggleAcceptModal}
                isDisabled={!showAssigned}
                theme={'primary'}
              />
            </div>
            <div className={styles.actionButton}>
              <IconActionButton
                icon={CLOSE_ICON_WHITE}
                buttonText={strings[REJECT_ALL]}
                buttonAction={this._toggleRejectModal}
                isDisabled={!showAssigned}
                theme={'primary'}
              />
            </div>
          </div>
        )}
        <Grid
          headerCells={headerCells}
          gridRows={getGridRows()}
          pagination
          paginationPositionLeft
          onClickNext={this.props.onClickNext}
          onClickPrevious={this.props.onClickPrevious}
          changeRowsPerPage={this.props.changeRowsPerPage}
          startIndex={this.props.startIndex}
          rowsPerPage={this.props.rowsPerPage}
          numberOfLastPage={this.props.numberOfLastPage}
          lastPage={this.props.lastPage}
          setNumberOfLastPage={this.props.setNumberOfLastPage}
          setLastPage={this.props.setLastPage}
        />
      </div>
    );
  }
}
