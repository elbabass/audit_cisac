import SearchPage from '../../pages/searchPage';
import SubmissionPage from '../../pages/submission';

const fixture = require('../../fixtures/agencies/bmi.json');

describe('Legal Entity Warning Tests', function () {
   beforeEach(function () {
      cy.login(`${fixture.agencyName.toLowerCase()}@cis.net`, Cypress.env('AgencyPortalLoginPassword'));
      cy.visit('/search');
   });

   it(`Warning displays when adding creator as publisher`, function () {
      SearchPage.addSubmission().click();
      SubmissionPage.publisherNameNumber().type(fixture.ips[0].nameNumber);
      SubmissionPage.lookupPublisherNameNumber().click();
      SubmissionPage.addIp().click();
      cy.contains('This IP is a Natural Person, check that this is the correct Publisher IP').should('be.visible');
   });

   it(`Warning displays when adding publisher as creator`, function () {
      SearchPage.addSubmission().click();
      SubmissionPage.creatorNameNumberField(1).type('269137346');
      SubmissionPage.lookupCreatorNameNumber(1).click();
      SubmissionPage.addIp().click();
      cy.contains('This IP is a Legal Entity, check that this is the correct Creator').should('be.visible');
   });
});
