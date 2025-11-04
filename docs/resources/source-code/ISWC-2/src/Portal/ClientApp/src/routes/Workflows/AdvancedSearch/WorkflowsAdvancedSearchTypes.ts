export interface IWorkflowsAdvancedSearchProps {
  toggleAdvancedSearch: () => void;
  showAdvancedSearch: boolean;
  searchFilters: {
    iswc?: string;
    workCodes?: string;
    agency?: string;
    workflowType?: string;
  };
  updateSearchFilters: (event: any) => void;
}
