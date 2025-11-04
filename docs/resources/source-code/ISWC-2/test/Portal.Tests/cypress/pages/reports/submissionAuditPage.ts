import ReportsPage from './reportsPage';

class SubmissionAuditPage extends ReportsPage {
   static workcode = () => cy.get('#agencyWorkCode');
   static displayList = () => cy.contains('Display List View');
   static typeSortIcon = () => cy.get("[class*='GridHeaderCell_icon']").eq(1);

   static listRows = () => cy.get('tr');
   static rowType = () => cy.get('td').eq(1);
   static rowAgency = () => cy.get("[id='Submitting Agency:']");
   static rowAgencyWorkcode = () => cy.get("[id='Agency Work Code:']");
   static rowCreators = () => cy.get("[id='Creator Name(s):']");
   static rowIpNameNumbers = () => cy.get("[id='IP Name Number:']");
   static extractToExcel = () => cy.get("[class*='SubmissionAudit_actionButton']").eq(2);
   static fromDate = () => cy.get('#fromDate').first();
   static toDate = () => cy.get('#toDate').first();
}

export default SubmissionAuditPage;
