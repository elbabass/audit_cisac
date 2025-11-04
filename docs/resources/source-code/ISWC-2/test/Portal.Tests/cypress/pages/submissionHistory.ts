class SubmissionHistory {
   static type = () => cy.get("[id='Type:']");
   static agency = () => cy.get("[id='Submitting Agency:']");
   static method = () => cy.get("[id='Method:']");
   static workcode = () => cy.get("[id='Submitting Agency Work No.:']");
   static creators = () => cy.get("[id='Creators:']");
   static title = () => cy.get("[id='Titles:']");
}

export default SubmissionHistory;
