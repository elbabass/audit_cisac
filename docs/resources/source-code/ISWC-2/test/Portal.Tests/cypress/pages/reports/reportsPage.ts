class ReportsPage {
   static submissionAuditTab = () => cy.contains('Submission Audit');
   static agencyInterestExtractTab = () => cy.contains('Agency Interest Extract');
   static iswcFullExtractTab = () => cy.contains('ISWC Full Extract');
   static agencyStatisticsTab = () => cy.contains('Agency Statistics');
   static fileSubmissionAuditTab = () => cy.contains('File Submission Audit');
   static publisherIswcTrackingTab = () => cy.contains('Publisher ISWC Tracking');
   static agencyWorkListTab = () => cy.contains('Agency Work List');
}

export default ReportsPage;
