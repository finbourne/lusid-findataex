# lusid-findataex

Retrieves financial data from BBG DL for consumption by LUSID

## Example Usage

Arguments required are the type of financial data required, the insturment unique ids (figis only currently supported), the output directory and the filesystem for output storage (local or Lusid Drive).

Financial Data Types:
* GetData - Retrieve general financial data by field headers (currently fields default to retrieve last price only for testing phase)
* GetActions - Corporate Action Data For Equities

Instrument Ids : 
* Passed in as a "|" separated list

Output Storage : 
* "lusid" to use Lusid Drive or leave blank to use file system
* If using Lusid Drive ensure you have a "secrets.json" file in the working directory that contains an entry for `driveUrl`, or have your Lusid secrets environment variables set.

### Price Data

`FinDataEx.dll "GetData" ""EQ0010174300001000|EQ0021695200001000" "/home/prices_dir/2020-01-01/"`

`FinDataEx.dll "GetData" ""EQ0010174300001000|EQ0021695200001000" "/Prices/2020-01-01/" "lusid"`


## DL Setup

### Default Settings

By default DLWS connection goes to "https://dlws.bloomberg.com/dlps". There is the option to override if necessary.

### Authetication

Set the following environment variables for your DL certificate and password:

* `BBG_DL_CERT_PATH` : Path to you DL certificate (e.g. /tmp/DLWSCert.p12)
* `BBG_DL_CERT_DATA` : Base64 encoded binary certificate
* `BBG_DL_CERT_PASS` : DL certificate password

Finbourne has a developer account with credentials stored in AWS Parameter Store at:
* /Scheduler/ClientConfiguration/datalicense-test/DLWSCert/BbgDlCertBinStr
* /Scheduler/ClientConfiguration/datalicense-test/DLWSCert/BbgDlPass

### DL Support

First port of call should be the BBG DL [support docs.](https://service.bloomberg.com/portal/docs/dl). Access to logs requires a BBG Web Portal Support account. Speak to BBG account for manager for access.


## CI/CD

Coming soon