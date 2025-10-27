import 'cypress-wait-until';
import { Submission } from 'cypress/models/submission';
import General from 'cypress/pages/general';

const apiGetTokenReq = {
   method: 'POST',
   url: `${Cypress.env('AzureKeyVaultSecret-ISWC-BaseAddress-IswcApi')}/connect/token`,
   form: true,

   body: {
      grant_type: 'client_credentials',
      client_id: 'iswcapimanagement',
      client_secret: Cypress.env('AzureKeyVaultSecret-ISWC-Secret-IswcApiManagement'),
      scope: 'iswcapi',
      AgentID: '000',
   },
};

Cypress.Commands.add('login', (email: string, password: string) => {
   cy.intercept({
      pathname: '/Profile/GetUserRoles',
      method: 'GET',
   }).as('getUserRoles');
   cy.session([email, password], () => {
      const req = {
         method: 'POST',
         form: true,
         url: `${Cypress.env('AzureKeyVaultSecret-ISWC-Uri-LoginRedirect').replace('/login', '')}/connect/`,

         body: {
            email: email,
            password: password,
         },
      };
      cy.request(req)
         .then((response) => {
            const token = response.body;
            let el = document.createElement('html');
            el.innerHTML = token;
            const passport = el.getElementsByTagName('input')[1].value;
            return passport;
         })
         .then((passport) => {
            const loginRequest = {
               method: 'POST',
               authority: Cypress.config().baseUrl?.replace('https://', ''),
               form: true,
               url: '/login',
               body: {
                  userid: email,
                  passport: passport,
                  redirecturl: '/search',
               },
            };
            return cy.request(loginRequest).then((response) => {
               const redirect = response.allRequestResponses[0]['Response Headers']['location'];
               cy.visit(`${Cypress.config().baseUrl}${redirect}`).wait('@getUserRoles').waitForLookups();
            });
         });
   });
});

Cypress.Commands.add('addSubmission', (submission: Submission): Cypress.Chainable<Cypress.Response<any>> => {
   if (Cypress.env('access_token')) {
      const addRequest = {
         method: 'POST',
         headers: { authorization: `Bearer ${Cypress.env('access_token')}`, 'Content-type': 'application/json' },
         url: `${Cypress.env('AzureKeyVaultSecret-ISWC-BaseAddress-IswcApi')}/submission`,
         body: submission,
      };
      return cy.request(addRequest);
   } else {
      return cy
         .request(apiGetTokenReq)
         .then((response) => {
            const token = response.body['access_token'];
            Cypress.env({ access_token: token });
            return token;
         })
         .then((token) => {
            const addRequest = {
               method: 'POST',
               headers: { authorization: `Bearer ${token}`, 'Content-type': 'application/json' },
               url: `${Cypress.env('AzureKeyVaultSecret-ISWC-BaseAddress-IswcApi')}/submission`,
               body: submission,
            };
            return cy.request(addRequest);
         });
   }
});

Cypress.Commands.add(
   'updateSubmission',
   (preferredIswc: string, submission: Submission): Cypress.Chainable<Cypress.Response<any>> => {
      if (Cypress.env('access_token')) {
         const updateRequest = {
            method: 'PUT',
            headers: { authorization: `Bearer ${Cypress.env('access_token')}`, 'Content-type': 'application/json' },
            url: `${Cypress.env(
               'AzureKeyVaultSecret-ISWC-BaseAddress-IswcApi'
            )}/submission?preferredIswc=${preferredIswc}`,
            body: submission,
         };
         return cy.request(updateRequest);
      } else {
         return cy
            .request(apiGetTokenReq)
            .then((response) => {
               const token = response.body['access_token'];
               Cypress.env({ access_token: token });
               return token;
            })
            .then((token) => {
               const updateRequest = {
                  method: 'PUT',
                  headers: { authorization: `Bearer ${token}`, 'Content-type': 'application/json' },
                  url: `${Cypress.env(
                     'AzureKeyVaultSecret-ISWC-BaseAddress-IswcApi'
                  )}/submission?preferredIswc=${preferredIswc}`,
                  body: submission,
               };
               return cy.request(updateRequest);
            });
      }
   }
);

Cypress.Commands.add(
   'mergeIswcs',
   (agency: string, parentIswc: string, childIswcs: string[]): Cypress.Chainable<Cypress.Response<any>> => {
      if (Cypress.env('access_token')) {
         const mergeRequest = {
            method: 'POST',
            headers: { authorization: `Bearer ${Cypress.env('access_token')}`, 'Content-type': 'application/json' },
            url: `${Cypress.env(
               'AzureKeyVaultSecret-ISWC-BaseAddress-IswcApi'
            )}/iswc/merge?preferredIswc=${parentIswc}&agency=${agency}`,
            body: { iswcs: childIswcs },
         };
         return cy.request(mergeRequest);
      } else {
         return cy
            .request(apiGetTokenReq)
            .then((response) => {
               const token = response.body['access_token'];
               Cypress.env({ access_token: token });
               return token;
            })
            .then((token) => {
               const mergeRequest = {
                  method: 'POST',
                  headers: { authorization: `Bearer ${token}`, 'Content-type': 'application/json' },
                  url: `${Cypress.env(
                     'AzureKeyVaultSecret-ISWC-BaseAddress-IswcApi'
                  )}/iswc/merge?preferredIswc=${parentIswc}&agency=${agency}`,
                  body: { iswcs: childIswcs },
               };
               return cy.request(mergeRequest);
            });
      }
   }
);

Cypress.Commands.add(
   'demergeIswcs',
   (agency: string, parentIswc: string, workcode: string): Cypress.Chainable<Cypress.Response<any>> => {
      if (Cypress.env('access_token')) {
         const demergeRequest = {
            method: 'DELETE',
            headers: { authorization: `Bearer ${Cypress.env('access_token')}`, 'Content-type': 'application/json' },
            url: `${Cypress.env(
               'AzureKeyVaultSecret-ISWC-BaseAddress-IswcApi'
            )}/iswc/merge?preferredIswc=${parentIswc}&agency=${agency}&workcode=${workcode}`,
         };
         return cy.request(demergeRequest);
      } else {
         return cy
            .request(apiGetTokenReq)
            .then((response) => {
               const token = response.body['access_token'];
               Cypress.env({ access_token: token });
               return token;
            })
            .then((token) => {
               const demergeRequest = {
                  method: 'DELETE',
                  headers: { authorization: `Bearer ${token}`, 'Content-type': 'application/json' },
                  url: `${Cypress.env(
                     'AzureKeyVaultSecret-ISWC-BaseAddress-IswcApi'
                  )}/iswc/merge?preferredIswc=${parentIswc}&agency=${agency}&workcode=${workcode}`,
               };
               return cy.request(demergeRequest);
            });
      }
   }
);

Cypress.Commands.add('getLatestDatabricksReportJob', () => {
   const req = {
      method: 'GET',
      url: `${Cypress.env('AzureKeyVaultSecret-ISWC-Databricks-ReportsJobUri')}runs/list?job_id=${Cypress.env(
         'AzureKeyVaultSecret-ISWC-Databricks-ReportsJobId'
      )}&active_only=true&limit=1`,
      headers: {
         Authorization: `Bearer ${Cypress.env('AzureKeyVaultSecret-ISWC-Databricks-ReportsJobBearerToken')}`,
      },
   };
   return cy.request(req).then((response) => {
      return response.body['runs'][0];
   });
});

Cypress.Commands.add('waitForDatabricksJobToFinish', (runId: string) => {
   const req = {
      method: 'GET',
      url: `${Cypress.env('AzureKeyVaultSecret-ISWC-Databricks-ReportsJobUri')}runs/get?run_id=${runId}`,
      headers: {
         Authorization: `Bearer ${Cypress.env('AzureKeyVaultSecret-ISWC-Databricks-ReportsJobBearerToken')}`,
      },
   };
   return cy.waitUntil(
      () =>
         cy.request(req).then((resp) => {
            if (resp.body['state']['life_cycle_state'] == 'TERMINATED') return resp.body['state'];
            return false;
         }),
      {
         errorMsg: 'Databricks job did not finish within 12 mins',
         timeout: 60000 * 12,
         interval: 20000,
      }
   );
});

Cypress.Commands.add('waitForLatestPipelineToFinish', (date: Date) => {
   const params = {
      date: date,
      clientId: Cypress.env('AzureKeyVaultSecret-EDI-AAD-ClientID'),
      secret: Cypress.env('AzureKeyVaultSecret-EDI-AAD-Secret'),
      tenantId: Cypress.env('AzureKeyVaultSecret-EDI-AAD-Tenant'),
      resourceName: Cypress.env('AzureKeyVaultSecret-EDI-ResourceGroupName'),
      factoryName: Cypress.env('AzureKeyVaultSecret-EDI-DataFactoryName'),
   };
   return cy.waitUntil(
      () =>
         cy.task('getLatestPipelineRunStatus', params).then((status) => {
            if (status == 'Succeeded' || status == 'Failed') return status;
            return false;
         }),
      {
         errorMsg: 'Pipeline did not finish within 10 mins',
         timeout: 60000 * 10,
         interval: 20000,
      }
   );
});

Cypress.Commands.add('waitForSubmission', (agency: string, workcode: string) => {
   const searchRequest = {
      method: 'GET',
      headers: { authorization: `Bearer ${Cypress.env('access_token')}`, 'Content-type': 'application/json' },
      failOnStatusCode: false,
      url: `${Cypress.env(
         'AzureKeyVaultSecret-ISWC-BaseAddress-IswcApi'
      )}/iswc/searchByAgencyWorkCode?agency=${agency}&workcode=${workcode}&detailLevel=Minimal`,
   };
   return cy.waitUntil(
      () =>
         cy.request(searchRequest).then((res) => {
            return res.status != 404;
         }),
      {
         errorMsg: 'Submission not found in index after CAR transaction',
         timeout: 60000,
         interval: 1500,
      }
   );
});

Cypress.Commands.add('waitForLookups', () => {
   cy.wait(['@configuration', '@configuration', '@lookup']);
});


Cypress.Commands.add('logOut', () => {
   if(Cypress.env('access_token')) {
      General.settingsCog().click();
      General.logoutButton().click();
   }
});