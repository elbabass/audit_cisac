import './commands';

beforeEach(() => {
  cy.intercept({
    pathname: '/api/Lookup/**',
    method: 'GET',
  }).as('lookup');

  cy.intercept({
    pathname: '/configuration/**',
    method: 'GET',
  }).as('configuration');

  const injectOrigin = (req) => {
    req.headers['origin'] =  Cypress.env('AzureKeyVaultSecret-ISWC-BaseAddress-IswcPortal');
    req.continue();
  };

  cy.intercept(
    {
      pathname: '/submission',
      method: 'POST',
    },
    injectOrigin
  ).as('addSubmission');

  cy.intercept(
    {
      pathname: '/iswc/searchByIswc',
      method: 'GET',
    },
    injectOrigin
  ).as('searchIswc');

  cy.intercept(
    {
      pathname: '/iswc/searchByIswc/batch',
      method: 'POST',
    },
    injectOrigin
  ).as('searchIswcBatch');

  cy.intercept(
    {
      pathname: '/iswc/searchByAgencyWorkCode**',
      method: 'GET',
    },
    injectOrigin
  ).as('searchAgencyWorkcode');

  cy.intercept(
    {
      pathname: '/iswc/searchByTitleAndContributor',
      method: 'POST',
    },
    injectOrigin
  ).as('searchByTitle');

  cy.intercept(
    {
      pathname: '/submission**',
      method: 'PUT',
    },
    injectOrigin
  ).as('updateSubmission');

  cy.intercept(
    {
      pathname: '/submission**',
      method: 'DELETE',
    },
    injectOrigin
  ).as('deleteWork');

  cy.intercept(
    {
      pathname: '/iswc/merge**',
      method: 'POST',
    },
    injectOrigin
  ).as('mergeIswcs');

  cy.intercept(
    {
      pathname: '/iswc/merge**',
      method: 'DELETE',
    },
    injectOrigin
  ).as('demergeIswcs');

  cy.intercept(
    {
      pathname: '/Lookup/GetIps',
      method: 'POST',
    },
    injectOrigin
  ).as('getIps');
});

before(() => {
  cy.intercept({
    pathname: '/Lookup/**',
    method: 'GET',
  }).as('lookup');
  cy.intercept({
    pathname: '/configuration/**',
    method: 'GET',
  }).as('configuration');
})

Cypress.on('uncaught:exception', (err, runnable) => {
  return false
})