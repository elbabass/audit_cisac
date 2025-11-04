import SearchPage from '../../pages/searchPage';
import { getTitleForLargeResults, formatIswc } from '../../utils/utils';
import SubmissionHistory from '../../pages/submissionHistory';
import ViewMoreGrid from '../../pages/grids/viewMore';
import { Submission } from 'cypress/models/submission';
import { AgencyFixture } from 'cypress/models/agencyFixture';

const fixture = require('../../fixtures/agencies/bmi.json') as AgencyFixture;
const sub = new Submission(fixture);

describe('Search Tests', function () {
   before(function () {
      cy.addSubmission(sub).then((resp) => {
         sub.preferredIswc = resp.body['verifiedSubmission']['iswc'];
         cy.waitForSubmission(sub.agency, sub.workcode);
      });
   });

   beforeEach(function () {
      cy.login(`${fixture.agencyName.toLowerCase()}@cis.net`, Cypress.env('AgencyPortalLoginPassword'));
      cy.visit('/search');
   });

   it(`Can search by title`, function () {
      SearchPage.searchByTitleTab().click();
      SearchPage.titleField().type(sub.originalTitle);
      SearchPage.titleSearchButton().click();
      cy.wait('@searchByTitle').its('response.statusCode').should('eq', 200);
      SearchPage.searchResults().should('have.length', 1);
      SearchPage.searchResultTitle().should('have.text', sub.originalTitle);
   });

   it(`Displays work info under View More`, function () {
      SearchPage.searchByAgencyWorkcode().click();
      SearchPage.workcodeField().type(sub.workcode);
      SearchPage.agencySelect().select(sub.agency);
      SearchPage.agencySearchButton().click();
      cy.wait('@searchAgencyWorkcode').its('response.statusCode').should('eq', 200);
      SearchPage.searchResults().should('have.length', 1);
      SearchPage.submittingAgencies().should('contain.text', fixture.agencyName);
      ViewMoreGrid.viewMore().click();
      ViewMoreGrid.ipNameNumber().first().should('contain.text', sub.interestedParties[0].nameNumber);
      ViewMoreGrid.ipName().first().should('contain.text', sub.interestedParties[0].name);
      ViewMoreGrid.ipName().eq(1).should('contain.text', sub.interestedParties[1].name);
      ViewMoreGrid.ipAffiliation().first().should('have.text', fixture.agencyName);
      ViewMoreGrid.ipRole().first().should('have.text', 'C (C,A,CA)');
      ViewMoreGrid.ipNameNumber().eq(1).should('contain.text', sub.interestedParties[1].nameNumber);
      ViewMoreGrid.agencyCode().first().should('have.text', sub.agency);
      ViewMoreGrid.ipBaseNumber().first().should('have.text', sub.interestedParties[0].baseNumber);
      ViewMoreGrid.agencyyWorkCode().first().should('have.text', sub.workcode);
      ViewMoreGrid.performersSurname().first().should('have.text', sub.performers[0].lastName);
   });

   it(`Can search by ISWC`, function () {
      SearchPage.iswcField().type(sub.preferredIswc);
      SearchPage.iswcSearchButton().click();
      cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
      SearchPage.searchResultIswc().should('contain.text', formatIswc(sub.preferredIswc));
   });

   it(`Displays submission history`, function () {
      SearchPage.iswcField().type(sub.preferredIswc);
      SearchPage.iswcSearchButton().click();
      cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
      ViewMoreGrid.viewMore().click();
      ViewMoreGrid.viewSubmssionHistory().first().click();
      SubmissionHistory.type().first().should('have.text', 'CAR');
      SubmissionHistory.workcode().first().should('have.text', sub.workcode);
      SubmissionHistory.method().first().should('have.text', 'REST');
      SubmissionHistory.title().first().should('contain.text', sub.originalTitle);
      SubmissionHistory.creators().first().should('contains.text', sub.interestedParties[0].name);
   });

   it(`Can search by title with many results`, function () {
      SearchPage.searchByTitleTab().click();
      SearchPage.titleField().type(getTitleForLargeResults());
      SearchPage.titleSearchButton().click();
      cy.wait('@searchByTitle', { timeout: 15000 }).its('response.statusCode').should('eq', 200);
      SearchPage.searchResults().should('have.length.greaterThan', 1);
   });
});
