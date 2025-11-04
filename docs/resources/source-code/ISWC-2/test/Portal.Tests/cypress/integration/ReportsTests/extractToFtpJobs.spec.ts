import SearchPage from '../../pages/searchPage';
import SubmissionAuditPage from 'cypress/pages/reports/submissionAuditPage';
import AgencyInterestExtractPage from 'cypress/pages/reports/agencyInterestExtractPage';
import IswcFullExtractPage from 'cypress/pages/reports/iswcFullExtractPage';
import PublisherIswcTrackingPage from 'cypress/pages/reports/publisherIswcTrackingPage';
import { AgencyFixture } from 'cypress/models/agencyFixture';
import AgencyWorkListPage from 'cypress/pages/reports/agencyWorkListPage';
import ReportsPage from 'cypress/pages/reports/reportsPage';

const fixture = require('../../fixtures/agencies/bmi.json') as AgencyFixture;

describe(`Extract to FTP jobs tests`, function () {
   beforeEach(function () {
      cy.login(`${fixture.agencyName.toLowerCase()}@cis.net`, Cypress.env('AgencyPortalLoginPassword'));
      cy.visit('/reports');
   });

   it(`Submission audit extract job succeeds`, function () {
      SubmissionAuditPage.extractToExcel().click();
      cy.wait(6000);
      cy.getLatestDatabricksReportJob().then(($run) => {
         const params = $run.overriding_parameters.notebook_params;
         expect(params.submitting_agency_code).equal(fixture.agencyCode);
         expect(params.report_type).equal('SubmissionAudit');
         expect(params.status).equal('All');
         expect(params.transaction_source).equal('All');
         expect(params.submitting_agency_code).equal('021');
         const runId = $run.run_id;
         cy.waitForDatabricksJobToFinish(runId).then(($state) => {
            expect($state.result_state).equal('SUCCESS');
         });
      });
   });

   it(`Agency Interest Extract job succeeds`, function () {
      AgencyInterestExtractPage.agencyInterestExtractTab().click();
      AgencyInterestExtractPage.extractToExcel().click();
      cy.wait(6000);
      cy.getLatestDatabricksReportJob().then(($run) => {
         const params = $run.overriding_parameters.notebook_params;
         expect(params.report_type).equal('AgencyInterestExtract');
         const runId = $run.run_id;
         cy.waitForDatabricksJobToFinish(runId).then(($state) => {
            expect($state.result_state).equal('SUCCESS');
         });
      });
   });

   it(`ISWC Full Extract job succeeds - cache`, function () {
      let date = new Date();
      IswcFullExtractPage.iswcFullExtractTab().click();
      IswcFullExtractPage.extractToExcel().click();
      cy.wait(6000);
      cy.getLatestDatabricksReportJob().then(($run) => {
         const params = $run.overriding_parameters.notebook_params;
         expect(params.submitting_agency_code).equal(fixture.agencyCode);
         expect(params.report_type).equal('IswcFullExtract');
         expect(params.most_recent_version).equal('True');
         const runId = $run.run_id;
         cy.waitForDatabricksJobToFinish(runId).then(($state) => {
            expect($state.result_state).equal('SUCCESS');
            cy.waitForLatestPipelineToFinish(date).then(function ($result) {
               expect($result).equal('Succeeded');
            });
         });
      });
   });

   it(`ISWC Full Extract job succeeds - no cache`, function () {
      let date = new Date();
      IswcFullExtractPage.iswcFullExtractTab().click();
      IswcFullExtractPage.cachedSlider().uncheck({ force: true });
      IswcFullExtractPage.extractToExcel().click();
      cy.wait(6000);
      cy.getLatestDatabricksReportJob().then(($run) => {
         const params = $run.overriding_parameters.notebook_params;
         expect(params.submitting_agency_code).equal(fixture.agencyCode);
         expect(params.report_type).equal('IswcFullExtract');
         expect(params.most_recent_version).equal('False');
         const runId = $run.run_id;
         cy.waitForDatabricksJobToFinish(runId).then(($state) => {
            expect($state.result_state).equal('SUCCESS');
            cy.waitForLatestPipelineToFinish(date).then(function ($result) {
               expect($result).equal('Succeeded');
            });
         });
      });
   });

   it(`Publisher ISWC Tracking extract job succeeds`, function () {
      PublisherIswcTrackingPage.publisherIswcTrackingTab().click();
      PublisherIswcTrackingPage.extractToExcel().click();
      cy.wait(6000);
      cy.getLatestDatabricksReportJob().then(($run) => {
         const params = $run.overriding_parameters.notebook_params;
         expect(params.submitting_agency_code).equal(fixture.agencyCode);
         expect(params.report_type).equal('PublisherIswcTracking');
         const runId = $run.run_id;
         cy.waitForDatabricksJobToFinish(runId).then(($state) => {
            expect($state.result_state).equal('SUCCESS');
         });
      });
   });

   it('Agency Work List extract job succeeds', function () {
      let date = new Date();
      ReportsPage.agencyWorkListTab().click();
      AgencyWorkListPage.cachedSlider().uncheck({force:true} )
      AgencyWorkListPage.extractToExcel().click();
      cy.wait(6000);
      cy.getLatestDatabricksReportJob().then(($run) => {
         const params = $run.overriding_parameters.notebook_params;
         expect(params.submitting_agency_code).equal(fixture.agencyCode);
         expect(params.report_type).equal('AgencyWorkList');
         expect(params.most_recent_version).equal('False');
         const runId = $run.run_id;
         cy.waitForDatabricksJobToFinish(runId).then(($state) => {
            expect($state.result_state).equal('SUCCESS');
            cy.waitForLatestPipelineToFinish(date).then(function ($result) {
               expect($result).equal('Succeeded');
            });
         });
      });
   })
});
