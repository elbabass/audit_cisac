import ReportsPage from './reportsPage';

class AgencyWorkListPage extends ReportsPage {
    static extractToExcel = () => cy.get("[class*='AgencyWorkList_actionButton']");
    static cachedSlider = () => cy.get("[class*='Switch_switchInput']").find('input');
}

export default AgencyWorkListPage;