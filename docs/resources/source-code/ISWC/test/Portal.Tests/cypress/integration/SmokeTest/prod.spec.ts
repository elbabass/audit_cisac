import ViewMoreGrid from '../../pages/grids/viewMore';
import SearchPage from '../../pages/searchPage';
import { getTitleForLargeResults, unformatIswc } from '../../utils/utils';

const fixture = require('../../fixtures/production/prod.json');

describe(`Prod Test - ${fixture.agencyName}`, function () {
   beforeEach(function () {
      cy.login(Cypress.env('AgencyPortalLoginEmail'), Cypress.env('AgencyPortalLoginPassword'));
      cy.visit('/search');
   });

   it(`${fixture.agencyName} - searches by agency workcode`, function () {
      SearchPage.searchByAgencyWorkcode().click();
      SearchPage.workcodeField().type(fixture.prodData.workcode);
      SearchPage.agencySelect().select(fixture.agencyName);
      SearchPage.agencySearchButton().click();
      cy.wait('@searchAgencyWorkcode').its('response.statusCode').should('eq', 200);
      SearchPage.searchResults().should('have.length', 1);
      SearchPage.searchResultIswc().should('have.text', fixture.prodData.iswc);
   });

   it(`${fixture.agencyName} - searches by title`, function () {
      SearchPage.searchByTitleTab().click();
      SearchPage.titleField().type(fixture.prodData.title);
      SearchPage.titleSearchButton().click();
      cy.wait('@searchByTitle').its('response.statusCode').should('eq', 200);
      SearchPage.searchResults().should('have.length.gte', 1);
      SearchPage.searchResultTitle().should('contain.text', fixture.prodData.title);
   });

   it(`${fixture.agencyName} - displays work info under View More`, function () {
      SearchPage.searchByAgencyWorkcode().click();
      SearchPage.workcodeField().type(fixture.prodData.workcode);
      SearchPage.agencySelect().select(fixture.agencyName);
      SearchPage.agencySearchButton().click();
      cy.wait('@searchAgencyWorkcode').its('response.statusCode').should('eq', 200);
      SearchPage.searchResults().should('have.length', 1);
      ViewMoreGrid.viewMore().click();
      ViewMoreGrid.ipNameNumber().first().should('contain.text', fixture.prodData.ip.nameNumber);
      ViewMoreGrid.ipName().first().should('contain.text', fixture.prodData.ip.name);
      ViewMoreGrid.ipAffiliation().first().should('have.text', fixture.agencyName);
      ViewMoreGrid.ipRole().first().should('have.text', 'C (C,A,CA)');
      ViewMoreGrid.agencyCode().first().should('have.text', fixture.agencyCode);
      ViewMoreGrid.ipBaseNumber().first().should('have.text', fixture.prodData.ip.baseNumber);
      ViewMoreGrid.agencyyWorkCode().first().should('have.text', fixture.prodData.workcode);
   });

   it(`${fixture.agencyName} - searches by ISWC`, function () {
      SearchPage.iswcField().type(fixture.prodData.iswc);
      SearchPage.iswcSearchButton().click();
      cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
      SearchPage.searchResults().should('have.length', 1);
      SearchPage.searchResultIswc().should('have.text', fixture.prodData.iswc);
   });

   it(`${fixture.agencyName} - searches by title with many results`, function () {
      SearchPage.searchByTitleTab().click();
      SearchPage.titleField().type(getTitleForLargeResults());
      SearchPage.titleSearchButton().click();
      cy.wait('@searchByTitle', { timeout: 15000 }).its('response.statusCode').should('eq', 200);
      SearchPage.searchResults().should('have.length.gte', 1);
   });
});
