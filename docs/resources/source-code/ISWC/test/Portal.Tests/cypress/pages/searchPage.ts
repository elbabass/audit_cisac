class SearchPage {
   static addSubmission = () => cy.contains('New Submission');
   static reports = () => cy.get("[href='/reports']");
   static homeLogo = () => cy.get("[alt='ISWC Network logo']");
   static iswcField = () => cy.get('#iswc');
   static iswcSearchButton = () => cy.get("div[class*='SearchBy_formButton'] button").first();
   static searchResults = () => cy.get('[class*=Search_resultsContainer] tbody tr');
   static searchResultIswc = () => SearchPage.searchResults().find("[id='ISWC:']");
   static searchResultTitle = () => SearchPage.searchResults().find("[id='Original Title:']");
   static searchAgencies = () => SearchPage.searchResults().find("[id='Submitting Agencies:']");
   static searchByAgencyWorkcode = () => cy.contains('by Agency Work Code');
   static workcodeField = () => cy.get('#workCode');
   static agencySelect = () => cy.get('#agency');
   static agencySearchButton = () => cy.get("div[class*='SearchBy_formButton'] button").eq(1);
   static searchByTitleTab = () => cy.contains('by Title');
   static titleSearchButton = () => cy.get("div[class*='SearchBy_formButton'] button").eq(2);
   static titleField = () => cy.get('#title');
   static agencyName = () => cy.get('[id="Agency Name:"]');
   static submittingAgencies = () => cy.get('[id="Submitting Agencies:"]');
}

export default SearchPage;
