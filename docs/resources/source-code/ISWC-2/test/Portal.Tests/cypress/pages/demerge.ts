class Demerge {
   static iswc = () => cy.get("[id='ISWC:']");
   static prefferedIswc = () => cy.get("[id='Preferred ISWC:']");
   static demergeCheckbox = () => cy.get("label[class*='GridCheckboxCell']");
   static submit = () => cy.contains('Submit Demerge');
}

export default Demerge;
