# Development environment setup

## 1. Anaconda

Install Anaconda Python 3.7: https://www.anaconda.com/distribution/

## 2. Databricks Connect

Setup guide: https://docs.microsoft.com/en-us/azure/databricks/dev-tools/databricks-connect#client-setup

Summary:

    NOTE: This setup guide is exclusive to the IntegrationCluster in the Dev Databricks workspace which can be
    found under the ISWCDev resource group in the CISAC Azure Directory.

    > conda create --name dbconnect python=3.9
    > conda activate dbconnect
    > pip uninstall pyspark
    > pip install -U databricks-connect==11.3.*
    > databricks-connect configure
        > For host enter the start of your databricks url eg. https://adb-{number}.{number}.azuredatabricks.net
        > In order to generate token, go to the Dev Database Workspace then : Workspace > Shared > Generate API token. Attach to cluster and run cell.
        > Cluster ID can be taken by navigating to the cluster in the web browser and copying from the url
        > Organisation ID is the number that follows ?o= in the cluster url
    > databricks-connect test

## 3. Visual Studio Code

Setup guide: https://docs.microsoft.com/en-us/azure/databricks/dev-tools/databricks-connect#visual-studio-code

## 4. ediparser package preparation

1. Connect to virtual environment: `conda activate dbconnect`

2. Change directory to Edi directory: `cd ISWC/src/Integration/Edi/`
3. Install the ediparser package to the virtual environment in "edit" mode (note the '.' signifying local directory) : `pip install -e .`
4. Install pip dependancies: `pip install -r requirements.txt`

## 4.1 Package preparation

1. Follow the steps in section 3 above.
2. Install pip dependancies: `pip install -r requirements.txt`
3. Some python projects will have local dependencies on our other projects. Eg, EDI is dependent on Reporting. To run these projects locally you will
   need to install the package to the virtual environment as per step 3, navigating to the project folder and using `pip install -e .`

### 4.2 Deploying your local package.

During development it may be useful to deploy your local code rather than running the full deployment pipeline. To do that:

1. Change directory to the directory containing your setup.py: `cd ISWC/src/Integration/Edi/`
2. Generate python wheel: `python setup.py bdist_wheel`
3. Upload wheel to `package-source` folder in the cisaciswcdatabricksdev storage account.
4. Connect to virtual environment: `conda activate dbconnect`
5. Install the package to the virtual environment in "edit" mode (note the '.' signifying local directory) : `pip install -e .`

## 5. Running Test Suite

1. Install pytest: `pip install pytest`
2. Change directory to tests directory: `cd ISWC/src/Integration/Edi/ediparser/tests`
3. Run pytest: `py.test`

# Troubleshooting

- java.io.IOException: Could not locate executable null\bin\winutils.exe in the Hadoop binaries.

      	https://stackoverflow.com/a/35652866
