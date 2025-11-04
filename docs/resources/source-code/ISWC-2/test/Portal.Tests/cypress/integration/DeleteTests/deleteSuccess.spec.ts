import SearchPage from '../../pages/searchPage';
import ViewMoreGrid from '../../pages/grids/viewMore';
import { Submission } from 'cypress/models/submission';
import { AgencyFixture } from 'cypress/models/agencyFixture';

const fixture = require('../../fixtures/agencies/sacem.json') as AgencyFixture;

describe(`Delete submission success tests`, function () {
   before(function () {
      const submission = new Submission(fixture);
      cy.addSubmission(submission).then((resp) => {
         submission.preferredIswc = resp.body['verifiedSubmission']['iswc'];
         cy.wrap(submission).as('submission');
         cy.waitForSubmission(submission.agency, submission.workcode);
      });
   });

   beforeEach(function () {
      cy.login(`${fixture.agencyName.toLowerCase()}@cis.net`, Cypress.env('AgencyPortalLoginPassword'));
      cy.visit('/search');
   });

   it(`Can delete a submission when eligible`, function () {
      const submission = this.submission as Submission;
      SearchPage.iswcField().type(submission.preferredIswc);
      SearchPage.iswcSearchButton().click();
      cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
      ViewMoreGrid.viewMore().click();
      ViewMoreGrid.deleteWork().click();
      ViewMoreGrid.confirmDeletion().click();
      cy.wait('@deleteWork').its('response.statusCode').should('eq', 204);
      cy.contains(`The submission with Agency Work Code ${submission.workcode} has been successfully deleted`).should(
         'be.visible'
      );
      SearchPage.iswcField().clear().type(submission.preferredIswc);
      SearchPage.iswcSearchButton().click();
      cy.wait('@searchIswc').its('response.statusCode').should('eq', 404);
      SearchPage.searchResults().should('have.length', 0);
   });
});
