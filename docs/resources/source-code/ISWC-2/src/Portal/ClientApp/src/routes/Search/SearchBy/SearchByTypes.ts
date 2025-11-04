export interface ISearchByProps {
  value?: string;
  search: () => void;
  onChange: (event: any) => void;
}

export interface ISearchByTitleProps {
  value?: string;
  search: () => void;
  onChange: (event: any) => void;
  surnames: string;
  nameNumbers: string;
  baseNumbers: string;
}

export interface ISearchByCreatorProps {
  value?: string;
  search: () => void;
  onChange: (event: any) => void;
  creatorNameNumbers: string;
  creatorBaseNumbers: string;
}
