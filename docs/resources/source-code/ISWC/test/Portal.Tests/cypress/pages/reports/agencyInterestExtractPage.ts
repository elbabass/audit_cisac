import ReportsPage from './reportsPage';

class AgencyInterestExtractPage extends ReportsPage {
   static extractToExcel = () => cy.get("[class*='AgencyInterestExtract_actionButton']");
   static fromDate = () => cy.get("input[type='date']").eq(2);
   static toDate = () => cy.get("input[type='date']").eq(3);
   static agreementFromDate = () => cy.get("input[type='date']").eq(4);
   static agreementToDate = () => cy.get("input[type='date']").eq(5);
}

export default AgencyInterestExtractPage;
