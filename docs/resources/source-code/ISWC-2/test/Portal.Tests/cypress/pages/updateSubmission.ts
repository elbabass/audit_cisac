class UpdateSubmissionPage {
   static iswc = () => cy.get("[id='ISWC:']");
   static preferredIswcCheckbox = () => cy.get("input[type='checkbox']");
   static submit = () => cy.contains('Submit');
}

export default UpdateSubmissionPage;
