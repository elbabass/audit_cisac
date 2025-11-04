describe(`Smoke Test - Public Portal`, function () {
   it('loads public portal', function () {
      cy.visit(Cypress.env('AzureKeyVaultSecret-ISWC-BaseAddress-IswcPublic'));
      cy.get("[class*='LandingPage_contentContainer", { timeout: 10000 }).should('be.visible');
      cy.get("[alt='ISWC Network logo']").should('be.visible');
   });
});
