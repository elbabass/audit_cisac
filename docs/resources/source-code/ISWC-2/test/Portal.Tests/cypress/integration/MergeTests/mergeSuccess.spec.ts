import SearchPage from '../../pages/searchPage';
import { formatIswc } from '../../utils/utils';
import ViewMoreGrid from '../../pages/grids/viewMore';
import MergeList from '../../pages/mergeList';
import { Submission } from 'cypress/models/submission';
import { AgencyFixture } from 'cypress/models/agencyFixture';

const fixture = require('../../fixtures/agencies/sacem.json') as AgencyFixture;

describe(`Merge Success Tests`, function () {
   beforeEach(function () {
      cy.login(`${fixture.agencyName.toLowerCase()}@cis.net`, Cypress.env('AgencyPortalLoginPassword'));
      cy.visit('/search');
      const subOne = new Submission(fixture);
      const subTwo = new Submission(fixture);
      cy.addSubmission(subOne).then(function (resp) {
         subOne.preferredIswc = resp.body['verifiedSubmission']['iswc'];
         cy.wrap(subOne).as('subOne');
      });
      cy.addSubmission(subTwo).then(function (resp) {
         subTwo.preferredIswc = resp.body['verifiedSubmission']['iswc'];
         cy.wrap(subTwo).as('subTwo');
         cy.waitForSubmission(subTwo.agency, subTwo.workcode);
      });
   });

   it(`Can merge two Iswcs`, function () {
      const subOne = this.subOne as Submission;
      const subTwo = this.subTwo as Submission;
      SearchPage.iswcField().type(subOne.preferredIswc);
      SearchPage.iswcSearchButton().click();
      cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
      ViewMoreGrid.viewMore().click();
      ViewMoreGrid.addToMergeList().click({ force: true });

      SearchPage.iswcField().clear().type(subTwo.preferredIswc);
      SearchPage.iswcSearchButton().click();
      cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
      ViewMoreGrid.viewMore().click();
      ViewMoreGrid.addToMergeList().click({ force: true });
      ViewMoreGrid.viewMergeList().first().click();
      MergeList.iswc().first().should('have.text', formatIswc(subOne.preferredIswc));
      MergeList.iswc().eq(1).should('have.text', formatIswc(subTwo.preferredIswc));
      MergeList.submit().click();
      cy.wait('@mergeIswcs').its('response.statusCode').should('eq', 204);
      cy.contains(`${subTwo.preferredIswc} successfully merged into ${subOne.preferredIswc}`).should('be.visible');
      cy.visit('/search');
      SearchPage.iswcField().type(subTwo.preferredIswc);
      SearchPage.iswcSearchButton().click();
      cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
      SearchPage.searchResultIswc().first().find("img[src='./assets/icon_merged.svg']").should('be.visible');
      SearchPage.searchResultIswc()
         .first()
         .find('[title=\'This work submission has been merged into another. Click "View More" for details.\']')
         .should('exist');
      ViewMoreGrid.viewMore().click();
      cy.contains(
         `This ISWC has been merged into the following Preferred ISWC: ${formatIswc(subOne.preferredIswc)}`
      ).should('be.visible');
   });

   it(`Merge list only merges currently selected works`, function () {
      const subOne = this.subOne as Submission;
      const subTwo = this.subTwo as Submission;
      const subThree = new Submission(fixture);
      cy.addSubmission(subThree).then(function (resp) {
         subThree.preferredIswc = resp.body['verifiedSubmission']['iswc'];
         SearchPage.iswcField().type(subOne.preferredIswc);
         SearchPage.iswcSearchButton().click();
         cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
         ViewMoreGrid.viewMore().click();
         ViewMoreGrid.addToMergeList().check({ force: true });
         SearchPage.iswcField().clear().type(subTwo.preferredIswc);
         SearchPage.iswcSearchButton().click();
         cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
         ViewMoreGrid.viewMore().click();
         ViewMoreGrid.addToMergeList().check({ force: true });
         SearchPage.iswcField().clear().type(subThree.preferredIswc);
         SearchPage.iswcSearchButton().click();
         cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
         ViewMoreGrid.viewMore().click();
         ViewMoreGrid.addToMergeList().check({ force: true });
         ViewMoreGrid.viewMergeList().first().click();
         MergeList.listItems().each(function ($el, index, $list) {
            $el.text().includes(formatIswc(subThree.preferredIswc)) &&
               cy.wrap($el).find("[id='Remove']").first().click();
         });
         MergeList.submit().click();
         cy.wait('@mergeIswcs').its('response.statusCode').should('eq', 204);
         cy.contains(`${subTwo.preferredIswc} successfully merged into ${subOne.preferredIswc}`).should('be.visible');
      });
   });

   it(`Merge mutliple ISWC's`, function () {
      const subOne = this.subOne as Submission;
      const subTwo = this.subTwo as Submission;
      const subThree = new Submission(fixture);
      cy.addSubmission(subThree).then(function (resp) {
         cy.waitForSubmission(subThree.agency, subThree.workcode);
         subThree.preferredIswc = resp.body['verifiedSubmission']['iswc'];
         SearchPage.iswcField().type(subOne.preferredIswc);
         SearchPage.iswcSearchButton().click();
         cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
         ViewMoreGrid.viewMore().click();
         ViewMoreGrid.addToMergeList().check({ force: true });
         SearchPage.iswcField().clear().type(subTwo.preferredIswc);
         SearchPage.iswcSearchButton().click();
         cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
         ViewMoreGrid.viewMore().click();
         ViewMoreGrid.addToMergeList().check({ force: true });
         SearchPage.iswcField().clear().type(subThree.preferredIswc);
         SearchPage.iswcSearchButton().click();
         cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
         ViewMoreGrid.viewMore().click();
         ViewMoreGrid.addToMergeList().check({ force: true });
         ViewMoreGrid.viewMergeList().first().click();
         MergeList.submit().click();
         cy.wait('@mergeIswcs').its('response.statusCode').should('eq', 204);
         cy.contains(
            `${subTwo.preferredIswc}, ${subThree.preferredIswc} successfully merged into ${subOne.preferredIswc}`
         ).should('be.visible');
      });
   });

   it(`can choose parent ISWC`, function () {
      const subOne = this.subOne as Submission;
      const subTwo = this.subTwo as Submission;
      SearchPage.iswcField().type(subOne.preferredIswc);
      SearchPage.iswcSearchButton().click();
      cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
      ViewMoreGrid.viewMore().click();
      ViewMoreGrid.addToMergeList().check({ force: true });
      SearchPage.iswcField().clear().type(subTwo.preferredIswc);
      SearchPage.iswcSearchButton().click();
      cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
      ViewMoreGrid.viewMore().click();
      ViewMoreGrid.addToMergeList().check({ force: true });
      ViewMoreGrid.viewMergeList().first().click();
      MergeList.checkbox().eq(1).check({ force: true });
      MergeList.submit().click();
      cy.wait('@mergeIswcs').its('response.statusCode').should('eq', 204);
      cy.contains(`${subOne.preferredIswc} successfully merged into ${subTwo.preferredIswc}`).should('be.visible');
      cy.visit('/search');
      SearchPage.iswcField().type(subTwo.preferredIswc);
      SearchPage.iswcSearchButton().click();
      cy.wait('@searchIswc').its('response.statusCode').should('eq', 200);
      ViewMoreGrid.viewMore().click();
      ViewMoreGrid.archivedIswcs().should('contain.text', formatIswc(subOne.preferredIswc));
   });
});
