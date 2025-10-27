import { IDropdownOption } from '../../FormInput/FormInputTypes';

export interface IGridDropdownCellProps {
  options: IDropdownOption[];
  selectNewOption: (option: string) => void;
  value?: string;
}
