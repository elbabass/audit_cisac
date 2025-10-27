import { InterestedParty } from './submission';

export class IneligibleAgency {
   agencyCode: string;
   sourceDb: number;
   ips: InterestedParty[];
}

export class AgencyFixture {
   agencyCode: string;
   agencyName: string;
   sourceDb: number;
   ips: InterestedParty[];
   ineligibleAgency: IneligibleAgency;
}
