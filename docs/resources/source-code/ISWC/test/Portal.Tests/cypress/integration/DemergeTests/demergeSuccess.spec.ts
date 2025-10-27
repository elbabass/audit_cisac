import SearchPage from '../../pages/searchPage';
import { formatIswc } from '../../utils/utils';
import ViewMoreGrid from '../../pages/grids/viewMore';
import Demerge from 'cypress/pages/demerge';
import { Submission } from 'cypress/models/submission';
import { AgencyFixture } from 'cypress/models/agencyFixture';

const bmiFixture = require('../../fixtures/agencies/bmi.json') as AgencyFixture;
const sacemFixture = require('../../fixtures/agencies/sacem.json') as AgencyFixture;

describe(`Demerge Success Tests`, function () {
   beforeEach(function () {
      cy.login(`${bmiFixture.agencyName.toLowerCase()}@cis.net`, Cypress.env('AgencyPortalLoginPassword'));
      cy.visit('/search');
   });

   it(`Can demerge two Iswcs`, function () {
      const subOne = new Submission(bmiFixture);
      const subTwo = new Submission(bmiFixture);
      cy.addSubmission(subOne).then(function (resp) {
         subOne.preferredIswc = resp.body['verifiedSubmission']['iswc'];
         cy.wrap(subOne).as('subOne');
         cy.waitForSubmission(subOne.agency, subOne.workcode);
         cy.addSubmission(subTwo).then(function (res) {
            subTwo.preferredIswc = res.body['verifiedSubmission']['iswc'];
            cy.wrap(subTwo).as('subTwo');
            cy.waitForSubmission(subTwo.agency, subTwo.workcode);
            cy.mergeIswcs(bmiFixture.agencyCode, subOne.preferredIswc, [subTwo.preferredIswc]).then(function () {
               cy.wait(3000);
               SearchPage.iswcField().type(subOne.preferredIswc);
               SearchPage.iswcSearchButton().click();
               cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
               ViewMoreGrid.viewMore().click();
               ViewMoreGrid.demerge().click();
               Demerge.iswc().should('have.text', formatIswc(subTwo.preferredIswc));
               Demerge.prefferedIswc().should('have.text', formatIswc(subOne.preferredIswc));
               cy.wait('@searchIswcBatch').its('response.statusCode').should('eq', 207);
               Demerge.demergeCheckbox().click();
               Demerge.submit().click();
               cy.wait('@demergeIswcs').its('response.statusCode').should('eq', 204);
               cy.contains(`${subTwo.preferredIswc} successfully demerged from ${subOne.preferredIswc}`).should(
                  'be.visible'
               );
            });
         });
      });
   });

   it(`Can demerge as parent agency with no submission on child Iswc`, function () {
      const subOne = new Submission(bmiFixture);
      const subTwo = new Submission(sacemFixture);
      cy.addSubmission(subOne).then(function (resp) {
         subOne.preferredIswc = resp.body['verifiedSubmission']['iswc'];
         cy.wrap(subOne).as('subOne');
         cy.waitForSubmission(subOne.agency, subOne.workcode);
         cy.addSubmission(subTwo).then(function (resp) {
            subTwo.preferredIswc = resp.body['verifiedSubmission']['iswc'];
            cy.wrap(subTwo).as('subTwo');
            cy.waitForSubmission(subTwo.agency, subTwo.workcode);
            cy.mergeIswcs(bmiFixture.agencyCode, subOne.preferredIswc, [subTwo.preferredIswc]).then(function () {
               cy.wait(3000);
               SearchPage.iswcField().type(subOne.preferredIswc);
               SearchPage.iswcSearchButton().click();
               cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
               ViewMoreGrid.viewMore().click();
               ViewMoreGrid.demerge().click();
               Demerge.iswc().should('have.text', formatIswc(subTwo.preferredIswc));
               Demerge.prefferedIswc().should('have.text', formatIswc(subOne.preferredIswc));
               cy.wait('@searchIswcBatch').its('response.statusCode').should('eq', 207);
               Demerge.demergeCheckbox().click();
               Demerge.submit().click();
               cy.wait('@demergeIswcs').its('response.statusCode').should('eq', 204);
               cy.contains(`${subTwo.preferredIswc} successfully demerged from ${subOne.preferredIswc}`).should(
                  'be.visible'
               );
            });
         });
      });
   });
});
