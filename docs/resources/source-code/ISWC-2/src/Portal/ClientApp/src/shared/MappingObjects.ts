import {
  IVerifiedSubmission,
  ISubmissionState,
  ITitleSubmissionRow,
  ICreatorPublisherSubmissionRow,
  IDerivedFromWorksSubmissionRow,
  IPerformerSubmissionRow,
} from '../routes/Submission/SubmissionTypes';
import { IIswcModel } from '../redux/types/IswcTypes';
import {
  DERIVED_FROM_WORKS_SUBMISSION_ROW,
  PERFORMERS_SUBMISSION_ROW,
  PUBLISHERS_SUBMISSION_ROW,
  CREATORS_SUBMISSION_ROW,
  TITLES_SUBMISSION_ROW,
} from '../consts';
import { IAgency } from '../redux/types/LookupTypes';
import { IDropdownOption } from '../components/FormInput/FormInputTypes';

export function mapVerifiedSubmissionToIswcModel(
  verifiedSubmission: IVerifiedSubmission,
): IIswcModel {
  return {
    agency: verifiedSubmission.agency,
    createdDate: verifiedSubmission.createdDate ? verifiedSubmission.createdDate : '',
    interestedParties: verifiedSubmission.interestedParties,
    iswc: verifiedSubmission.iswc,
    iswcStatus: verifiedSubmission.iswcStatus,
    lastModifiedBy: verifiedSubmission.lastModifiedBy ? verifiedSubmission.lastModifiedBy : '',
    lastModifiedDate: verifiedSubmission.lastModifiedDate
      ? verifiedSubmission.lastModifiedDate
      : '',
    originalTitle: verifiedSubmission.originalTitle,
    otherTitles: verifiedSubmission.otherTitles,
  };
}

export function mapSubmissionToSubmissionState(submission: IVerifiedSubmission): ISubmissionState {
  const titles: ITitleSubmissionRow[] = [];
  titles.push({ title: submission.originalTitle, type: 'OT' });
  submission.otherTitles?.forEach((otherTitle) =>
    titles.push({ title: otherTitle.title, type: otherTitle.type }),
  );

  const creators: ICreatorPublisherSubmissionRow[] = [];
  submission.interestedParties.forEach((ip) => {
    if (ip.role && isCreator(ip.role)) {
      creators.push({
        name: ip.name || '',
        nameNumber: ip.nameNumber?.toString() && ip.nameNumber.toString(),
        baseNumber: ip.baseNumber,
        role: mapRolledUpRoleToRoleType(ip.role),
      });
    }
  });

  const publishers: ICreatorPublisherSubmissionRow[] = [];
  submission.interestedParties.forEach((ip) => {
    if (ip.role && !isCreator(ip.role)) {
      publishers.push({
        name: ip.name || '',
        nameNumber: ip.nameNumber?.toString() && ip.nameNumber.toString(),
        baseNumber: ip.baseNumber,
        role: mapRolledUpRoleToRoleType(ip.role),
      });
    }
  });

  const derivedFromWorks: IDerivedFromWorksSubmissionRow[] = [];
  submission.derivedFromIswcs?.forEach((der) =>
    derivedFromWorks.push({ iswc: der.iswc, title: der.title }),
  );

  const disambiguationIswcs: string[] = [];
  submission.disambiguateFrom?.forEach((dis) => disambiguationIswcs.push(dis.iswc));

  const performers: IPerformerSubmissionRow[] = [];
  submission.performers?.forEach((performer) =>
    performers.push({ firstName: performer.firstName, lastName: performer.lastName }),
  );

  return {
    agencyWorkCode: [{ agencyWorkCode: submission.workcode, agencyName: submission.agency }],
    titles:
      titles && titles.length > 0 ? titles : [{ title: '', type: 'OT' }, TITLES_SUBMISSION_ROW],
    creators: creators && creators.length > 0 ? creators : [CREATORS_SUBMISSION_ROW],
    disambiguation: submission.disambiguation,
    preferredIswc: submission.iswc,
    publishers: publishers && publishers.length > 0 ? publishers : [PUBLISHERS_SUBMISSION_ROW],
    derivedWorkType: submission.derivedWorkType ? submission.derivedWorkType : '',
    derivedFromWorks:
      derivedFromWorks && derivedFromWorks.length > 0
        ? derivedFromWorks
        : [DERIVED_FROM_WORKS_SUBMISSION_ROW],
    disambiguationIswcs:
      disambiguationIswcs && disambiguationIswcs.length > 0 ? disambiguationIswcs : [],
    disambiguationReason: submission.disambiguationReason ? submission.disambiguationReason : '',
    bvltr: submission.bvltr ? submission.bvltr : '',
    performers: performers && performers.length > 0 ? performers : [PERFORMERS_SUBMISSION_ROW],
    standardInstrumentation: '',
  };
}

function isCreator(role: string): boolean {
  if (role === 'C' || role === 'TA' || role === 'MA') return true;
  return false;
}

function mapRolledUpRoleToRoleType(role: string): string {
  if (isCreator(role)) {
    if (role === 'MA') return "AR";
    else if (role === "TA") return "AD";
    else return "C";
  }
  else return "E";
}

export const mapAgenciesToDropDownType = (agencies: IAgency[]) => {
  var agencyDropDowns: IDropdownOption[] = [];
  agencies.sort((a: IAgency, b: IAgency) => {
    return a.name.localeCompare(b.name);
  });
  agencies.forEach((agency) => {
    agencyDropDowns.push({
      value: agency.agencyId,
      name: agency.name,
    });
  });

  return agencyDropDowns;
};
