import { randomString } from 'cypress/utils/utils';

export class InterestedParty {
   name: string;
   nameNumber: number;
   baseNumber: string;
   role: string;

   constructor(name: string, nameNumber: number, baseNumber: string, role: string) {
      this.name = name;
      this.nameNumber = nameNumber;
      this.baseNumber = baseNumber;
      this.role = role;
   }
}

export class Performer {
   firstName: string = randomString(8);
   lastName: string = randomString(8);
}

export class Submission {
   agency: string;
   sourcedb: number;
   workcode: string;
   preferredIswc: string;
   category: string;
   originalTitle: string;
   interestedParties: InterestedParty[];
   performers: Performer[];
   constructor(fixture: any) {
      this.agency = fixture.agencyCode;
      this.sourcedb = fixture.sourceDb;
      this.interestedParties = [];
      fixture.ips.forEach((ip) => {
         this.interestedParties.push(new InterestedParty(ip.name, ip.nameNumber, ip.baseNumber, ip.role));
      });
      this.category = 'DOM';
      this.originalTitle = `UI TEST ${randomString(20)} ${randomString(20)}`;
      this.workcode = randomString(17);
      this.performers = [new Performer()];
      this.preferredIswc = '';
   }
}
