import { IVerifiedSubmission } from '../SubmissionTypes';

export interface ISubmissionSuccessProps {
  verifiedSubmission: IVerifiedSubmission;
  updateInstance?: boolean;
  newIswc?: boolean;
}
