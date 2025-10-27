class General {
    static settingsCog = () => cy.get('[class*=Header_settingsIcon__Qgwhs]');
    static logoutButton = () => cy.contains('Log Out');
}

export default General;