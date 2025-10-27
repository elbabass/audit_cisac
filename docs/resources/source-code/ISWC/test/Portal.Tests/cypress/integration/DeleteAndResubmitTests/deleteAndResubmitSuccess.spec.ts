import SearchPage from '../../pages/searchPage';
import SubmissionPage from '../../pages/submission';
import UpdateSubmissionPage from '../../pages/updateSubmission';
import { getNewTitle } from '../../utils/utils';
import ViewMoreGrid from '../../pages/grids/viewMore';
import { Submission } from 'cypress/models/submission';
import { AgencyFixture } from 'cypress/models/agencyFixture';
import General from 'cypress/pages/general';

const bmiFixture = require('../../fixtures/agencies/bmi.json') as AgencyFixture;
const sacemFixture = require('../../fixtures/agencies/sacem.json') as AgencyFixture;

describe('Delete and Resubmit Tests', function() {
    beforeEach(function () {
        cy.login(`${bmiFixture.agencyName.toLowerCase()}@cis.net`, Cypress.env('AgencyPortalLoginPassword'));
        cy.visit('/search');
        const bmiSub = new Submission(bmiFixture);
        const sacemSub = new Submission(sacemFixture);
        sacemSub.originalTitle = bmiSub.originalTitle;
        sacemSub.interestedParties = bmiSub.interestedParties;
        cy.addSubmission(bmiSub).then(function (resp) {
            bmiSub.preferredIswc = resp.body['verifiedSubmission']['iswc'];
            sacemSub.preferredIswc = resp.body['verifiedSubmission']['iswc'];
            cy.wrap(bmiSub).as('bmiSub');
            cy.wrap(sacemSub).as('sacemSub');
            cy.waitForSubmission(bmiSub.agency, bmiSub.workcode);
        });
        
        General.settingsCog().click();
        General.logoutButton().click();

        cy.login(`${sacemFixture.agencyName.toLowerCase()}@cis.net`, Cypress.env('AgencyPortalLoginPassword'));
        cy.visit('/search');
        cy.addSubmission(sacemSub).then(function () {            
            cy.wrap(sacemSub).as('sacemSub');
            cy.waitForSubmission(sacemSub.agency, sacemSub.workcode);
        })
    });
    
    it('Delete and Resubmit into same ISWC', function () {
        const sacemSub = this.sacemSub as Submission;
        SearchPage.iswcField().type(sacemSub.preferredIswc);
        SearchPage.iswcSearchButton().click();
        cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
        ViewMoreGrid.viewMore().click();
        ViewMoreGrid.deleteAndResubmit().click();
        ViewMoreGrid.confirmDeleteAndResubmit().click();
        SubmissionPage.next().click();
        UpdateSubmissionPage.submit().click();        
        cy.visit('/search');
        SearchPage.iswcField().type(sacemSub.preferredIswc);
        SearchPage.iswcSearchButton().click();
        cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
        ViewMoreGrid.viewMore().click();
        ViewMoreGrid.agencyCode().first().should('contain.text', '058');
    })

    it('Delete and Resubmit into different ISWC', function () {
        const sacemSub = this.sacemSub as Submission;
        const bmiSub = this.bmiSub as Submission;
        SearchPage.iswcField().type(sacemSub.preferredIswc);
        SearchPage.iswcSearchButton().click();
        cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
        ViewMoreGrid.viewMore().click();
        ViewMoreGrid.deleteAndResubmit().click();
        ViewMoreGrid.confirmDeleteAndResubmit().click();
        SubmissionPage.originalTitle().clear();
        SubmissionPage.originalTitle().type(getNewTitle());
        SubmissionPage.removeIp().first().click();
        SubmissionPage.addNewCreator().click();
        SubmissionPage.creatorNameNumberField(1).type(sacemFixture.ips[0].nameNumber.toString());
        SubmissionPage.lookupCreatorNameNumber(1).click();
        SubmissionPage.addIp().click();
        SubmissionPage.next().click();
        cy.wait('@addSubmission').its('response.statusCode').should('eq', 201);
        SubmissionPage.resubmissionIswc().then(function ($el) {
            sacemSub.preferredIswc = $el.text();
            cy.visit('/search');
            SearchPage.iswcField().type(sacemSub.preferredIswc);
            SearchPage.iswcSearchButton().click();
            cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
            ViewMoreGrid.viewMore().click();
            ViewMoreGrid.agencyCode().should('contain.text', '058');

            cy.visit('/search');
            SearchPage.iswcField().type(bmiSub.preferredIswc);
            SearchPage.iswcSearchButton().click();
            cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
            ViewMoreGrid.viewMore().click();
            ViewMoreGrid.agencyCode().should('contain.text', '021');
        });
    })
});