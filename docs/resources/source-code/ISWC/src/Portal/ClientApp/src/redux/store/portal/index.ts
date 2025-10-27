import * as SearchReducer from '../../reducers/SearchReducer';
import * as MergeReducer from '../../reducers/MergeReducer';
import * as SubmissionReducer from '../../reducers/SubmissionReducer';
import * as WorkflowsReducer from '../../reducers/WorkflowsReducer';
import * as SubmissionHistoryReducer from '../../reducers/SubmissionHistoryReducer';
import * as AppReducer from '../../reducers/AppReducer';
import * as ReportsReducer from '../../reducers/ReportsReducer';

export interface ApplicationState {
  searchReducer: SearchReducer.ISearchReducerState;
  mergeReducer: MergeReducer.IMergeReducerState;
  submissionReducer: SubmissionReducer.ISubmissionReducerState;
  workflowsReducer: WorkflowsReducer.IWorkflowsReducerState;
  submissionHistoryReducer: SubmissionHistoryReducer.ISubmissionHistoryReducerState;
  appReducer: AppReducer.IAppReducerState;
  reportsReducer: ReportsReducer.IReportsReducerState;
}

export const reducers = {
  searchReducer: SearchReducer.reducer,
  mergeReducer: MergeReducer.reducer,
  submissionReducer: SubmissionReducer.reducer,
  workflowsReducer: WorkflowsReducer.reducer,
  submissionHistoryReducer: SubmissionHistoryReducer.reducer,
  appReducer: AppReducer.reducer,
  reportsReducer: ReportsReducer.reducer,
};

export interface AppThunkAction<TAction> {
  (dispatch: (action: TAction) => void, getState: () => ApplicationState): void;
}
