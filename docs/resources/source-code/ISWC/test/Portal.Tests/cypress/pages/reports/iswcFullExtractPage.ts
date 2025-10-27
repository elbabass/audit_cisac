import ReportsPage from './reportsPage';

class IswcFullExtractPage extends ReportsPage {
   static extractToExcel = () => cy.get("[class*='IswcFullExtract_actionButton']");
   static cachedSlider = () => cy.get("[class*='Switch_switchInput']").find('input');
}

export default IswcFullExtractPage;
