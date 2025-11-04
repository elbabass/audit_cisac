import { randomString } from '../../utils/utils';
import SubmissionAuditPage from '../../pages/reports/submissionAuditPage';
import { Submission } from 'cypress/models/submission';
import { AgencyFixture } from 'cypress/models/agencyFixture';

const fixture = require('../../fixtures/agencies/sacem.json') as AgencyFixture;

describe(`Portal reports tests`, function () {
   beforeEach(function () {
      cy.login(`${fixture.agencyName.toLowerCase()}@cis.net`, Cypress.env('AgencyPortalLoginPassword'));
      cy.visit('/search');
   });

   it(`Transactions display in Submission Audit report`, function () {
      const subOne = new Submission(fixture);
      cy.addSubmission(subOne).then(function (resp) {
         subOne.preferredIswc = resp.body['verifiedSubmission']['iswc'];
         const subTwo = new Submission(fixture);
         cy.addSubmission(subTwo).then(function (res) {
            subTwo.preferredIswc = res.body['verifiedSubmission']['iswc'];
            subOne.originalTitle = `UI TEST${randomString(25)} ${randomString(25)}`;
            cy.updateSubmission(subOne.preferredIswc, subOne).then(function () {
               cy.mergeIswcs(fixture.agencyCode, subOne.preferredIswc, [subTwo.preferredIswc]).then(function () {
                  cy.wait(2000);
                  cy.demergeIswcs(fixture.agencyCode, subOne.preferredIswc, subTwo.workcode).then(function () {
                     cy.visit('/reports');
                     SubmissionAuditPage.workcode().type(subOne.workcode);
                     SubmissionAuditPage.displayList().click();
                     SubmissionAuditPage.typeSortIcon().click();
                     SubmissionAuditPage.typeSortIcon().should('have.attr', 'src', './assets/icon_arrowDown-red.svg');
                     SubmissionAuditPage.listRows()
                        .eq(1)
                        .within(function (this: Mocha.Context) {
                           SubmissionAuditPage.rowType().should('have.text', 'CAR');
                           SubmissionAuditPage.rowAgency().should('have.text', fixture.agencyName);
                           SubmissionAuditPage.rowAgencyWorkcode().should('have.text', subOne.workcode);
                           SubmissionAuditPage.rowCreators().should('contain.text', fixture.ips[0].name);
                           SubmissionAuditPage.rowIpNameNumbers().should('contain.text', fixture.ips[0].nameNumber);
                        });
                     SubmissionAuditPage.listRows()
                        .eq(2)
                        .within(function (this: Mocha.Context) {
                           SubmissionAuditPage.rowType().should('have.text', 'CUR');
                           SubmissionAuditPage.rowAgency().should('have.text', fixture.agencyName);
                           SubmissionAuditPage.rowAgencyWorkcode().should('have.text', subOne.workcode);
                           SubmissionAuditPage.rowCreators().should('contain.text', fixture.ips[0].name);
                           SubmissionAuditPage.rowIpNameNumbers().should('contain.text', fixture.ips[0].nameNumber);
                        });
                     SubmissionAuditPage.listRows()
                        .eq(3)
                        .within(function (this: Mocha.Context) {
                           SubmissionAuditPage.rowType().should('have.text', 'MER');
                           SubmissionAuditPage.rowAgency().should('have.text', fixture.agencyName);
                           SubmissionAuditPage.rowAgencyWorkcode().should('have.text', subOne.workcode);
                           SubmissionAuditPage.rowCreators().should('contain.text', fixture.ips[0].name);
                           SubmissionAuditPage.rowIpNameNumbers().should('contain.text', fixture.ips[0].nameNumber);
                        });
                  });
               });
            });
         });
      });
   });
});
