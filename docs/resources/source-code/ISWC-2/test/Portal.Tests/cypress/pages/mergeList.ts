class MergeList {
   static iswc = () => cy.get("[id='ISWC:']");
   static submit = () => cy.get("button[class*='ActionButton']");
   static remove = () => cy.get("[id='Remove']");
   static checkbox = () => cy.get("input[type='checkbox']");
   static listItems = () => cy.get('tr');
}

export default MergeList;
