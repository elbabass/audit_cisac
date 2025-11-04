export interface IWebUserRole {
  role: number;
  isApproved?: boolean;
  requestedDate?: string;
  status?: boolean;
  notification?: { message?: string };
}

export interface IAssignedRoles {
  search: boolean;
  update: boolean;
  reportBasics: boolean;
  reportExtracts: boolean;
  reportAgencyInterest: boolean;
  reportIswcFullExtract: boolean;
  manageRoles: boolean;
}

export interface IWebUser {
  email: string;
  agencyId: string;
  webUserRoles: IWebUserRole[];
}
