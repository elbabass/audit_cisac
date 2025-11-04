import SearchPage from '../../pages/searchPage';
import SubmissionPage from '../../pages/submission';
import UpdateSubmissionPage from '../../pages/updateSubmission';
import { formatIswc, randomString, unformatIswc } from '../../utils/utils';
import SubmissionHistory from '../../pages/submissionHistory';
import ViewMoreGrid from '../../pages/grids/viewMore';
import { Submission } from 'cypress/models/submission';
import { AgencyFixture } from 'cypress/models/agencyFixture';

const fixture = require('../../fixtures/agencies/bmi.json') as AgencyFixture;
const agencyTwoFixture = require('../../fixtures/agencies/sacem.json') as AgencyFixture;

describe(`Update Success Tests`, function () {
   beforeEach(function () {
      cy.login(`${fixture.agencyName.toLowerCase()}@cis.net`, Cypress.env('AgencyPortalLoginPassword'));
      cy.visit('/search');
      const sub = new Submission(fixture);
      cy.addSubmission(sub).then(function (resp) {
         sub.preferredIswc = resp.body['verifiedSubmission']['iswc'];
         cy.wrap(sub).as('sub');
         cy.waitForSubmission(sub.agency, sub.workcode);
      });
   });

   it(`Can update an existing submission`, function () {
      const sub = this.sub as Submission;
      SearchPage.iswcField().type(sub.preferredIswc);
      SearchPage.iswcSearchButton().click();
      cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
      ViewMoreGrid.viewMore().click();
      ViewMoreGrid.updateSubmission().click();
      SubmissionPage.originalTitle().clear().type(randomString(25));
      SubmissionPage.next().click();
      cy.wait('@updateSubmission').its('response.statusCode').should('eq', 200);
      UpdateSubmissionPage.iswc()
         .first()
         .should(($el) => {
            const text = unformatIswc($el.text());
            expect(text).to.be.equal(sub.preferredIswc);
         });
      UpdateSubmissionPage.submit().click();
      cy.wait('@updateSubmission').its('response.statusCode').should('eq', 200);
   });

   it(`Can update an existing submission to a split copyright work`, function () {
      const sub = this.sub as Submission;
      SearchPage.iswcField().type(sub.preferredIswc);
      SearchPage.iswcSearchButton().click();
      cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
      ViewMoreGrid.viewMore().click();
      ViewMoreGrid.updateSubmission().click();
      SubmissionPage.addNewCreator().click();
      SubmissionPage.creatorNameNumberField(3).type(fixture.ineligibleAgency.ips[0].nameNumber.toString());
      SubmissionPage.lookupCreatorNameNumber(3).click();
      SubmissionPage.addIp().click();
      SubmissionPage.next().click();
      cy.wait('@updateSubmission').its('response.statusCode').should('eq', 200);
      UpdateSubmissionPage.submit().click();
      cy.wait('@updateSubmission').its('response.statusCode').should('eq', 200);
      cy.visit('/search');
      SearchPage.iswcField().type(sub.preferredIswc);
      SearchPage.iswcSearchButton().click();
      cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
      ViewMoreGrid.viewMore().click();
      ViewMoreGrid.ipName().eq(2).should('contain.text', fixture.ineligibleAgency.ips[0].name.toString());
   });

   it(`Can remove affiliated IP`, function () {
      const sub = this.sub as Submission;
      SearchPage.iswcField().type(sub.preferredIswc);
      SearchPage.iswcSearchButton().click();
      cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
      ViewMoreGrid.viewMore().click();
      ViewMoreGrid.updateSubmission().click();
      SubmissionPage.removeIp().first().click();
      SubmissionPage.next().click();
      cy.wait('@updateSubmission').its('response.statusCode').should('eq', 200);
      UpdateSubmissionPage.iswc().should('have.text', formatIswc(sub.preferredIswc));
      UpdateSubmissionPage.submit().click();
      cy.wait('@updateSubmission').its('response.statusCode').should('eq', 200);
      ViewMoreGrid.viewMore().first().click();
      ViewMoreGrid.ipNameNumber().should('have.length', 1);
   });

   it(`Returns matches for updated metadata and automatically merges after update`, function () {
      const sub = this.sub as Submission;
      const subTwo = new Submission(fixture);
      cy.addSubmission(subTwo).then(function (resp) {
         cy.waitForSubmission(subTwo.agency, subTwo.workcode);
         subTwo.preferredIswc = resp.body['verifiedSubmission']['iswc'];
         SearchPage.iswcField().type(sub.preferredIswc);
         SearchPage.iswcSearchButton().click();
         cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
         ViewMoreGrid.viewMore().click();
         ViewMoreGrid.updateSubmission().click();
         SubmissionPage.originalTitle().clear();
         SubmissionPage.originalTitle().type(subTwo.originalTitle);
         SubmissionPage.next().click();
         cy.wait('@updateSubmission').its('response.statusCode').should('eq', 200);
         UpdateSubmissionPage.iswc().should('have.length', 2);
         UpdateSubmissionPage.submit().click();
         cy.wait('@updateSubmission').its('response.statusCode').should('eq', 200);
         cy.visit('/search');
         SearchPage.iswcField().type(sub.preferredIswc);
         SearchPage.iswcSearchButton().click();
         cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
         ViewMoreGrid.viewMore().click();
         cy.contains(
            `This ISWC has been merged into the following Preferred ISWC: ${formatIswc(subTwo.preferredIswc)}`
         ).should('be.visible');
      });
   });

   it(`Displays CUR in sub history`, function () {
      const sub = this.sub as Submission;
      SearchPage.iswcField().type(sub.preferredIswc);
      SearchPage.iswcSearchButton().click();
      cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
      ViewMoreGrid.viewMore().click();
      ViewMoreGrid.updateSubmission().click();
      SubmissionPage.originalTitle().type('a');
      SubmissionPage.next().click();
      cy.wait('@updateSubmission').its('response.statusCode').should('eq', 200);
      UpdateSubmissionPage.submit().click();
      cy.wait('@updateSubmission').its('response.statusCode').should('eq', 200);
      cy.visit('/search');
      SearchPage.iswcField().type(sub.preferredIswc);
      SearchPage.iswcSearchButton().click();
      cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
      ViewMoreGrid.viewMore().click();
      ViewMoreGrid.viewSubmssionHistory().click();
      SubmissionHistory.type().first().should('have.text', 'CUR');
      SubmissionHistory.workcode().first().should('have.text', sub.workcode);
      SubmissionHistory.title().first().should('contain.text', `${sub.originalTitle}a (OT)`);
   });

   it(`Moves inelligible work to matching ineligible ISWC`, function () {
      const ineligibleSubOne = new Submission(agencyTwoFixture);
      cy.addSubmission(ineligibleSubOne).then(function (resp) {
         cy.waitForSubmission(ineligibleSubOne.agency, ineligibleSubOne.workcode);
         ineligibleSubOne.preferredIswc = resp.body['verifiedSubmission']['iswc'];
         cy.wait(4000);
         ineligibleSubOne.agency = fixture.agencyCode;
         ineligibleSubOne.sourcedb = fixture.sourceDb;
         cy.addSubmission(ineligibleSubOne).then(function (res) {
            const destinationIneligibleSub = new Submission(agencyTwoFixture);
            cy.addSubmission(destinationIneligibleSub).then(function (re) {
               cy.waitForSubmission(destinationIneligibleSub.agency, destinationIneligibleSub.workcode);
               destinationIneligibleSub.preferredIswc = re.body['verifiedSubmission']['iswc'];
               SearchPage.iswcField().type(ineligibleSubOne.preferredIswc);
               SearchPage.iswcSearchButton().click();
               cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
               ViewMoreGrid.viewMore().click();
               ViewMoreGrid.updateSubmission().click();
               SubmissionPage.originalTitle().clear();
               SubmissionPage.originalTitle().type(destinationIneligibleSub.originalTitle);
               SubmissionPage.next().click();
               cy.wait('@updateSubmission').its('response.statusCode').should('eq', 200);
               UpdateSubmissionPage.iswc().should('have.length', 2);
               UpdateSubmissionPage.submit().click();
               cy.wait('@updateSubmission').its('response.statusCode').should('eq', 200);
               UpdateSubmissionPage.iswc().should('have.text', formatIswc(destinationIneligibleSub.preferredIswc));
            });
         });
      });
   });

   it(`Can update disambiguated works`, function () {
      const sub = this.sub as Submission;
      SearchPage.addSubmission().click();
      SubmissionPage.originalTitle().type(sub.originalTitle);
      SubmissionPage.creatorNameNumberField(1).type(sub.interestedParties[0].nameNumber.toString());
      SubmissionPage.lookupCreatorNameNumber(1).click();
      SubmissionPage.addIp().click();
      SubmissionPage.addNewCreator().click();
      SubmissionPage.creatorNameNumberField(1).type(sub.interestedParties[1].nameNumber.toString());
      SubmissionPage.lookupCreatorNameNumber(1).click();
      SubmissionPage.addIp().click();
      SubmissionPage.workcodeField().type(randomString(18));
      SubmissionPage.next().click();
      SubmissionPage.disambiguateAllTab().click();
      SubmissionPage.disambiguateReason().select('DIT');
      SubmissionPage.submit().click();
      cy.wait('@addSubmission').its('response.statusCode').should('eq', 201);
      cy.contains('Preferred ISWC Assigned').should('be.visible');
      SubmissionPage.assignedIswc().then(function ($el) {
         const disambiguatedIswc = $el.text();
         cy.wait(2000);
         cy.visit('/search');
         SearchPage.iswcField().type(disambiguatedIswc);
         SearchPage.iswcSearchButton().click();
         cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
         ViewMoreGrid.viewMore().click();
         ViewMoreGrid.updateSubmission().click();
         SubmissionPage.originalTitle().type(randomString(3));
         SubmissionPage.next().click();
         cy.wait('@updateSubmission').its('response.statusCode').should('eq', 200);
         SubmissionPage.submit().click();
         cy.wait('@updateSubmission').its('response.statusCode').should('eq', 200);
         cy.visit('/search');
         SearchPage.iswcField().type(disambiguatedIswc);
         SearchPage.iswcSearchButton().click();
         cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
         ViewMoreGrid.viewMore().click();
         ViewMoreGrid.disambiguatedFrom().should('contain.text', formatIswc(sub.preferredIswc));
      });
   });

   it(`Can update an existing submission to add a performer`, function () {
      const sub = this.sub as Submission;
      SearchPage.iswcField().type(sub.preferredIswc);
      SearchPage.iswcSearchButton().click();
      cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
      ViewMoreGrid.viewMore().click();
      ViewMoreGrid.updateSubmission().click();
      SubmissionPage.originalTitle().clear().type(randomString(25));
      const performerLastName = randomString(10);
      const performerFirstName = randomString(10);
      SubmissionPage.addNewPerformer().click();
      SubmissionPage.performerLastName().eq(1).type(performerLastName);
      SubmissionPage.performerFirstName(1).type(performerFirstName);
      SubmissionPage.next().click();
      cy.wait('@updateSubmission').its('response.statusCode').should('eq', 200);
      UpdateSubmissionPage.iswc()
         .first()
         .should(($el) => {
            const text = unformatIswc($el.text());
            expect(text).to.be.equal(sub.preferredIswc);
         });
      UpdateSubmissionPage.submit().click();
      cy.wait('@updateSubmission').then((resp) => {
         const verifiedSub = resp.response.body.verifiedSubmission as Submission;
         expect(verifiedSub.performers.length).to.be.equal(2);
      });
   });
});
