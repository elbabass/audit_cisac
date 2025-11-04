## How to run smoke test

-  Install required packages by **npm i**.

-  Add environment variable values to cypress.env.json.

-  If running the production smoke test only the following variables are needed: AzureKeyVaultSecret-ISWC-BaseAddress-IswcPortal, AzureKeyVaultSecret-ISWC-BaseAddress-IswcPublic, AzureKeyVaultSecret-ISWC-Uri-LoginRedirect, AgencyPortalLoginPassword, AgencyPortalLoginEmail.

-  To run the tests headless in Electron use **npm run test**. Electron is installed as a dependency of Cypress so this will work out of the box without any browser being installed on the machine running the tests. It is also a Chromium based browser.

-  If you wish to run in Chrome then simply call cypress directly with the browser arg set **npx cypress run -b chrome --headless**

-  Cypress also supports the canary channel of Chrome, as well as various release channels of Firefox and Edge Chromium. The only prerequisite to use any of these browsers is that they must be installed on the machine running the tests.

-  To open the test runner user **npm run open** or **npm run open:uat**. The test runner will enable you to step through tests using DOM snapshots as well as monitoring API requests made on the page. This is a very useful debugger. Clicking on any of the API requests will output the full request and response to the console. Clicking on any Cypress command will output debug info on that too.

-  The test runner also has a dropdown for selecting which browser to use populated with all the available browsers found on the machine.

-  Fixtures for any agency can be added to the fixtures folder. Copy the required fields from an existing fixture file.

-  To add an agency to the smoke test spec, add a new fixture and add it to the fixtures array in smoke.spec.ts.

-  The prod.json fixture contains the existing prod data for the prod smoke test and should not be used in other tests.
