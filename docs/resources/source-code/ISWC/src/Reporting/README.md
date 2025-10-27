# Development environment setup

## 1. Anaconda

Install Anaconda Python 3.7: https://www.anaconda.com/distribution/

## 2. Databricks Connect

Setup guide: https://docs.microsoft.com/en-us/azure/databricks/dev-tools/databricks-connect#client-setup

Summary:

    > conda create --name reporting python=3.9
    > conda activate reporting
    > pip uninstall pyspark
    > pip install -U databricks-connect==11.3.*
    > databricks-connect configure
    > databricks-connect test

    Note: Dev Databricks workspace can be found under ISWCDev resource group in CISAC Azure Directory.
    In order to generate token, go to Workspace > Shared > Generate API token. Attach to cluster and run cell.

## 3. Visual Studio Code

Setup guide: https://docs.microsoft.com/en-us/azure/databricks/dev-tools/databricks-connect#visual-studio-code

## 4. ediparser package preparation

1. Connect to virtual environment: `conda activate reporting`

2. Change directory to Edi directory: `cd /ISWC/src/Reporting/`
3. Install the reporting package to the virtual environment in "edit" mode (note the '.' signifying local directory) : `pip install -e .`
4. Install pip dependancies: `pip install -r requirements.txt`

## 4.1 Package preparation

1. Follow the steps in section 3 above.
2. Install pip dependancies: `pip install -r requirements.txt`
3. Some python projects will have local dependencies on our other projects. Eg, EDI is dependent on Reporting. To run these projects locally you will
   need to install the package to the virtual environment as per step 3, navigating to the project folder and using `pip install -e .`

### 4.2 Deploying your local package.

During development it may be useful to deploy your local code rather than running the full deployment pipeline. To do that:

1. Change directory to the directory containing your setup.py: `cd /ISWC/src/Reporting/`
2. Generate python wheel: `python setup.py bdist_wheel`
3. Upload wheel to `package-source` folder in the cisaciswcdatabricksdev storage account.
4. Connect to virtual environment: `conda activate dbconnect`
5. Install the package to the virtual environment in "edit" mode (note the '.' signifying local directory) : `pip install -e .`

## 5. Running Test Suite

1. Change directory to tests directory: `cd /ISWC/src/Reporting/reporting/tests`
2. Run pytest: `py.test`

# Troubleshooting

- java.io.IOException: Could not locate executable null\bin\winutils.exe in the Hadoop binaries.

      	https://stackoverflow.com/a/35652866
