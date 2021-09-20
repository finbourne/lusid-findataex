# Lusid-FinDataEx

Retrieves financial data from BBG DL for consumption by LUSID


## Usage

Arguments required include the type of operation required, the input and output targets for the data, and details of the instruments to query.

For detailed, up to date help, invoke the argument `--help`. The following are the main options:

**Operation Types** : [What action is being taken]
* BloombergRequest - Request live data from Bloomberg for the listed instruments
* ParseExisting - Read bloomberg data from a static file such as a .csv

**Financial Data Types** : [What types of financial data to handle]
* GetData - Retrieve general financial data by field headers
* GetActions - Corporate Action Data For Equities

**Input Source/Output Target** : [What location to take data from/to]
* CLI - Take data directly from the command line
* LUSID - Transfer data directly to a LUSID Portfolio
* Drive - Read or write files in LUSID Drive
* Local - Read or write files in the local filesystem

**Instrument Source Args** : [When making a request to Bloomberg, what instruments should be queried]
* When input source is CLI - A comma separated list of instrument names
* When input source is LUSID - A comma separate list of `[SCOPE]|[PORTFOLIO]` pairs describing locations in LUSID
* When input source is Drive - A comma separated list of `[PATH],[DELIMETER (optional)],[COLUMN INDEX (optional)]`
* When input source is Local - A comma separated list of `[PATH],[DELIMETER (optional)],[COLUMN INDEX (optional)]`

### Price Data Examples

`>FinDataEx.exe getdata -d CCY -t BloombergRequest -i Lusid production|global-equity -o Drive --output-path /home/dl_results/MySubmission_{REQUEST_ID}_{AS_AT}.csv --enable-live-request`

`>FinDataEx.exe getdata -d BBG_ID -t ParseExisting -i Local EQ0010174300001000,EQ0021695200001000 -o Lusid --output-path production|global-equity`

### Corporate Actions Examples

`>FinDataEx getactions -c DVD_CASH -t BloombergRequest -i Lusid production:global-equity -o Lusid --output-path ibor-test|bbg-corp-action-loader --enable-live-request`

`>FinDataEx getactions -c DVD_CASH -t ParseExisting -i Local --input-path=./corporateActionsFile.csv -o Lusid --output-path ibor-test|bbg-corp-action-loader`

### Running from the Image

To run from the Docker Image manually, run `docker run -it --entrypoint /bin/sh harbor.finbourne.com/tools/findataex`.

## LUSID Connection

### Authentication

LUSID authentication should be through a standard `secrets.json` file, or via the matching environment variables.

For more details, please see the instructions at: https://support.lusid.com/knowledgebase/article/KA-01663

Note that using LUSID Drive for Input or Output will require an environment variable `DRIVE_URL` or `secrets.json` entry `driveUrl`.


## Bloomberg Data License Connection

### Default Settings

By default DLWS connection goes to: https://dlws.bloomberg.com/dlps.

There is the option to override if necessary.

### Authentication

Set the following environment variables for your DL certificate and password:

* `BBG_DL_CERT_PATH` : Path to you DL certificate (e.g. /tmp/DLWSCert.p12)
* `BBG_DL_CERT_DATA` : Base64 encoded binary certificate
* `BBG_DL_CERT_PASS` : DL certificate password

### DL Support

First port of call should be the BBG DL [support docs](https://service.bloomberg.com/portal/docs/dl). Access to logs requires a BBG Web Portal Support account. Speak to BBG account for manager for access.


## CI/CD

There is a Concourse pipeline that has been established: https://concourse.finbourne.com/teams/sales-engineering/pipelines/findataex.

This pipeline runs a suite of Unit tests, as well as the capability of running Integration testing. Please see the dedicated README in the Integration test folder.

Docker images are created and stored in Harbor at: harbor.finbourne.com/tools/findataex

A sandbox environment exists at https://datalicense-test.lusid.com.

## Ongoing Work

There are several areas of the application which still require work. Some examples are:

* Completing and running the integration tests
* Refactor to clean up the InstrumentSourceArgs option, it is currently a little overloaded
* Instrument Data supporting output to a LUSID portfolio
* Handling more Corporate Action Types
* Port the repo to GitLab, as it would be easier to test from within our internal network