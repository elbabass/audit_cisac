export interface IFormInputProps {
  onChange: (event: any) => void;
  name: string;
  label?: string;
  placeholder?: string;
  size?: string;
  value?: string;
  disabled?: boolean;
}

export interface IDateInputProps {
  label?: string;
  value?: string;
  changeData?: (event: any) => void;
  name?: string;
}

export interface IDateInputState {
  date?: string;
}

export interface IDropdownProps {
  onChange: (event: any) => void;
  name: string;
  label: string;
  size?: string;
  options: IDropdownOption[];
  value?: string;
  disabled?: boolean;
  defaultToLoggedInAgency?: boolean;
}

export interface IDropdownOption {
  value: string;
  name: string;
}

export interface IDateRangeInputProps {
  changeFromDate: (event: any) => void;
  changeToDate: (event: any) => void;
  fromDate?: string;
  toDate?: string;
  fromDateName?: string;
  toDateName?: string;
}
