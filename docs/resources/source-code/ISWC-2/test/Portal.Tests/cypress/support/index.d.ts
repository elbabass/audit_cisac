declare namespace Cypress {
   interface Chainable {
      /**
       * Login to the portal
       */
      login(email: string, password: string): Chainable<any>;
      /**
       * Add a submission via API
       */
      addSubmission(submission: any): Cypress.Chainable<Cypress.Response<any>>;
      /**
       * Get the most recent Process Report job
       */
      getLatestDatabricksReportJob(): any;
      /**
       * Get the most recent full extract pipeline run status
       */
      waitForLatestPipelineToFinish(date: Date): any;
      /**
       * Wait for the specified Process Report job to finish
       */
      waitForDatabricksJobToFinish(runId: string): any;
      /**
       * Wait for api/lookup and /configuration requests to complete.
       */
      waitForLookups(): any;
      /**
       * Merge the given child ISWC's into the given parent ISWC.
       */
      mergeIswcs(agency: string, parentIswc: string, childIswcs: string[]): any;
      /**
       * Demerge the given workcode from the given parent ISWC.
       */
      demergeIswcs(agency: string, parentIswc: string, workcode: string): any;
      /**
       * Update a submission via API
       */
      updateSubmission(preferredIswc: string, submission: any): Cypress.Chainable<Cypress.Response<any>>;
      /**
       * Wait for a submission to be indexed.
       */
      waitForSubmission(agency: string, workcode: string): any;
      /**
       * Logout from site if logged in
       */
      logOut(): any;
   }
}
