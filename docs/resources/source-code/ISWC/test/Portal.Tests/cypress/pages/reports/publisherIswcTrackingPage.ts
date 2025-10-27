import ReportsPage from './reportsPage';

class PublisherIswcTrackingPage extends ReportsPage {
   static extractToExcel = () => cy.get("[class*='PublisherIswcTracking_actionButton']");
}

export default PublisherIswcTrackingPage;
