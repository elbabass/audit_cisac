import SearchPage from '../../pages/searchPage';
import SubmissionPage from '../../pages/submission';
import { randomString, formatIswc, unformatIswc, getNewTitle } from '../../utils/utils';
import ViewMoreGrid from '../../pages/grids/viewMore';
import { Submission } from 'cypress/models/submission';

const fixture = require('../../fixtures/agencies/bmi.json');

describe(`Add Submission Success Tests`, function () {
   before(() => {
      // Add API check since this is first spec executed after deployment.
      cy.wait(10000);
      cy.waitUntil(
         () =>
            cy
               .request({
                  url: `${Cypress.env('AzureKeyVaultSecret-ISWC-BaseAddress-IswcApi')}/submission`,
                  failOnStatusCode: false,
                  followRedirect: true,
                  method: 'POST',
               })
               .then((res) => {
                  return res.status == 401;
               }),
         {
            errorMsg: 'API not ready within 2m',
            timeout: 120000,
            interval: 5000,
         }
      );
   });

   beforeEach(function () {
      cy.login(`${fixture.agencyName.toLowerCase()}@cis.net`, Cypress.env('AgencyPortalLoginPassword'));
      cy.visit('/search');
   });

   it(`Can create a new submission`, function () {
      SearchPage.addSubmission().click();
      SubmissionPage.originalTitle().type(getNewTitle());
      SubmissionPage.creatorNameNumberField(1).type(fixture.ips[0].nameNumber);
      SubmissionPage.lookupCreatorNameNumber(1).click();
      cy.wait('@getIps', { timeout: 20000 }).its('response.statusCode').should('eq', 200);
      SubmissionPage.addIp().click();
      SubmissionPage.workcodeField().type(randomString(15));
      SubmissionPage.next().click();
      cy.wait('@addSubmission').its('response.statusCode').should('eq', 201);
      cy.contains('Preferred ISWC Assigned').should('be.visible');
   });

   it(`Can add a derived from work`, function () {
      const sub = new Submission(fixture);
      cy.addSubmission(sub).then(function (resp) {
         cy.waitForSubmission(sub.agency, sub.workcode);
         const iswc = resp.body['verifiedSubmission']['iswc'];
         SearchPage.addSubmission().trigger('mouseover').click();
         SubmissionPage.originalTitle().type(getNewTitle());
         SubmissionPage.creatorNameNumberField(1).type(fixture.ips[0].nameNumber);
         SubmissionPage.lookupCreatorNameNumber(1).click();
         cy.wait('@getIps', { timeout: 20000 }).its('response.statusCode').should('eq', 200);
         SubmissionPage.addIp().click();
         SubmissionPage.workcodeField().type(randomString(18));
         SubmissionPage.derivedType().select('Modified Version');
         SubmissionPage.derivedIswc().type(iswc);
         SubmissionPage.derivedTitle().type(sub.originalTitle);
         SubmissionPage.next().click();
         cy.wait('@addSubmission').its('response.statusCode').should('eq', 201);
         SubmissionPage.assignedIswc().then(($el) => {
            const derivedIswc = $el.text();
            cy.visit('/search');
            SearchPage.iswcField().type(derivedIswc);
            SearchPage.iswcSearchButton().click();
            cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
            ViewMoreGrid.viewMore().click();
            ViewMoreGrid.derivedFromTitle().should('have.text', sub.originalTitle);
            ViewMoreGrid.derivedFromIswc().should('have.text', formatIswc(iswc));
            ViewMoreGrid.derivedFromType().should('have.text', 'Modified Version');
         });
      });
   });

   it(`Can copy work as new submission`, function () {
      const sub = new Submission(fixture);
      cy.addSubmission(sub).then(function (resp) {
         cy.waitForSubmission(sub.agency, sub.workcode);
         const iswc = resp.body['verifiedSubmission']['iswc'];
         SearchPage.iswcField().type(iswc);
         SearchPage.iswcSearchButton().click();
         cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
         ViewMoreGrid.viewMore().click();
         ViewMoreGrid.newSubmission().click();
         SubmissionPage.workcodeField().type(randomString(18));
         SubmissionPage.next().click();
         cy.wait('@addSubmission').its('response.statusCode').should('eq', 201);
         SubmissionPage.submit().click;
         cy.contains('Matches Found').should('be.visible');
         SubmissionPage.disambiguateAllTab().click();
         SubmissionPage.disambiguateReason().select('DIT');
         SubmissionPage.submit().click();
         cy.wait('@addSubmission').its('response.statusCode').should('eq', 201);
         cy.contains('Preferred ISWC Assigned').should('be.visible');
         SubmissionPage.assignedIswc().then(function ($el) {
            const disambiguatedIswc = unformatIswc($el.text());
            cy.visit('/search');
            SearchPage.iswcField().type(disambiguatedIswc);
            SearchPage.iswcSearchButton().click();
            cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
            SearchPage.searchResultTitle().should('have.text', sub.originalTitle);
            ViewMoreGrid.viewMore().click();
            ViewMoreGrid.performersSurname().should('have.text', sub.performers[0].lastName);
            ViewMoreGrid.ipNameNumber().first().should('contain.text', sub.interestedParties[0].nameNumber);
            ViewMoreGrid.ipNameNumber().eq(1).should('contain.text', sub.interestedParties[1].nameNumber);
            ViewMoreGrid.performersSurname().should('have.text', sub.performers[0].lastName);
            ViewMoreGrid.performerFirstName(0).should('have.text', sub.performers[0].firstName);
         });
      });
   });

   it(`Can create a new disambiguated submission`, function () {
      const sub = new Submission(fixture);
      cy.addSubmission(sub).then(function (resp) {
         cy.waitForSubmission(sub.agency, sub.workcode);
         const iswc = resp.body['verifiedSubmission']['iswc'];
         SearchPage.addSubmission().click();
         SubmissionPage.originalTitle().type(sub.originalTitle);
         SubmissionPage.creatorNameNumberField(1).type(sub.interestedParties[0].nameNumber.toString());
         SubmissionPage.lookupCreatorNameNumber(1).click();
         cy.wait('@getIps', { timeout: 20000 }).its('response.statusCode').should('eq', 200);
         SubmissionPage.addIp().click();
         SubmissionPage.addNewCreator().click();
         SubmissionPage.creatorNameNumberField(1).type(sub.interestedParties[1].nameNumber.toString());
         SubmissionPage.lookupCreatorNameNumber(1).click();
         SubmissionPage.addIp().click();
         SubmissionPage.workcodeField().type(randomString(15));
         SubmissionPage.next().click();
         cy.wait('@addSubmission');
         cy.contains('Matches Found').should('be.visible');
         SubmissionPage.disambiguateAllTab().click();
         SubmissionPage.disambiguateReason().select('DIT');
         SubmissionPage.submit().click();
         cy.wait('@addSubmission').its('response.statusCode').should('eq', 201);
         cy.contains('Preferred ISWC Assigned').should('be.visible');
         SubmissionPage.assignedIswc().then(function ($el) {
            const disambiguatedIswc = unformatIswc($el.text());
            cy.visit('/search');
            SearchPage.iswcField().type(disambiguatedIswc);
            SearchPage.iswcSearchButton().click();
            cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
            SearchPage.submittingAgencies()
               .find(
                  '[title=\'This work submission has been disambiguated from other works. Click "View More" for details.\']'
               )
               .should('exist');
            ViewMoreGrid.viewMore().click();
            ViewMoreGrid.disambiguationReason().should('have.text', 'DIT (Different Work)');
            ViewMoreGrid.disambiguatedFrom().should(function (this: Mocha.Context, $from) {
               const disambiguatedFromIswc = unformatIswc($from.text());
               expect(disambiguatedFromIswc.trim()).to.be.equal(iswc);
            });
         });
      });
   });

   it(`Can create a new submission with performers`, function () {
      SearchPage.addSubmission().click();
      SubmissionPage.originalTitle().type(getNewTitle());
      SubmissionPage.creatorNameNumberField(1).type(fixture.ips[0].nameNumber);
      SubmissionPage.lookupCreatorNameNumber(1).click();
      cy.wait('@getIps', { timeout: 20000 }).its('response.statusCode').should('eq', 200);
      SubmissionPage.addIp().click();
      SubmissionPage.workcodeField().type(randomString(15));
      const performerLastName = randomString(10);
      const performerFirstName = randomString(10);
      SubmissionPage.performerLastName().type(performerLastName);
      SubmissionPage.performerFirstName(0).type(performerFirstName);
      SubmissionPage.next().click();
      cy.wait('@addSubmission').its('response.statusCode').should('eq', 201);
      SubmissionPage.assignedIswc().then(($el) => {
         const iswc = $el.text();
         cy.visit('/search');
         SearchPage.iswcField().type(iswc);
         SearchPage.iswcSearchButton().click();
         cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
         ViewMoreGrid.viewMore().click();
         ViewMoreGrid.performersSurname().should('have.text', performerLastName);
         ViewMoreGrid.performerFirstName(0).should('have.text', performerFirstName);
      });
   });
});
