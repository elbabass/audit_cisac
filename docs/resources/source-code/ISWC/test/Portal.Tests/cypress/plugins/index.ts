import * as msRestNodeAuth from '@azure/ms-rest-nodeauth';
import { DataFactoryManagementClient } from '@azure/arm-datafactory';

module.exports = (on: any, config: any) => {
   const baseUrl = config.env['AzureKeyVaultSecret-ISWC-BaseAddress-IswcPortal'] || null;

   on('before:browser:launch', (browser: any, launchOptions: any) => {
      if (browser.name === 'chrome' && browser.isHeadless) {
         launchOptions.args.push('--disable-gpu');
         return launchOptions;
      }
   });
   
   on('task', {
      getLatestPipelineRunStatus(params: any) {
         return msRestNodeAuth
            .loginWithServicePrincipalSecretWithAuthResponse(params.clientId, params.secret, params.tenantId)
            .then((creds) => {
               const client = new DataFactoryManagementClient(
                  creds.credentials,
                  config.env['AzureKeyVaultSecret-EDI-SubscriptionID']
               );
               return client.pipelineRuns
                  .queryByFactory(params.resourceName, params.factoryName, {
                     orderBy: [{ order: 'DESC', orderBy: 'RunStart' }],
                     lastUpdatedBefore: new Date(),
                     lastUpdatedAfter: params.date,
                  })
                  .then(($run) => {
                     return $run.value[0].status;
                  });
            });
      },
   });
   
   if (baseUrl) {
      config.baseUrl = baseUrl;
   }
   return config;
};
