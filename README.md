# lusid-findataex
Retrieves financial data from third-party vendors for consumption by LUSID


## Scheduler Testing (Temp Only - WILL REMOVE)

### Intro
Aim of this test is to replicate the workflow of a data request to an external data vendor (DL) through the findataex tool. Mock out the following workflow :

- Execute a data extraction request using findataex. The request configuratoin file is loaded from LUSID drive.
- Findataex will mock the DL vendor response by loading a vendor DL response which is again stored in LUSID drive. Mocks responses live in a specific directory (that should not ever need to be amended).
- The mock vendor response is then processed in Findataex using the same logic as would be done in a real response. The processing results in an output file (e.g. price file) that is written to LUSID drive.


### Prerequisite files to prepare in LUSID Drive

- Need to upload a set of mock vendor responses from DL. In fbn-ci drive please see existing directory "FinDataExCITests_BBG_DL_Responses" that contains an example. Make a note of the lusid drive file id of the mock response you'd like to be returned from your request.

- Setup a sample fde request json. These files contain the parameters for a request. See (https://github.com/finbourne/lusid-findataex/tree/feature/SE-286-LusidDriveOutput/Lusid.FinDataEx.Tests/Vendor/Dl/TestData) for examples used in integration testing. For a fbn-ci example looks in the drive folder "FinDataExCITests_UserFDERequests" for the sample "fde_request_dl_prices_lusid_drive.json". Take a look in the json file and note first the output which is "lusiddrive". Other output options exist in the codebase that for example write to local filesystems if required. Also note the connectorConfig.url entry "lusiddrive://52ff9d39-ace8-4873-95dc-d288feda3011". This is a fabricated url we use to mock the vendor response. It instructs findataex to look for a vendor response to our request with the lusid file drive id "52ff9d39-ace8-4873-95dc-d288feda3011". That is the file id of the DL response in the previous step.  Using an actual DL response ensures we still run through the parsing logic for DL responses in this test (i.e. keeping it as realisitc as we can without making an external vendor call). Make a note of the lusid drive file id for the request you have created.

### Run

- Findata ex takes the following arguments: Lusid.FinDataEx.dll <requestSource> <requestIdOrPath> <outputDir>. 

- For this test as we're loading requests from drive an example call would be : Lusid.FinDataEx.dll "LusidDrive", "f9bc7674-cedd-442e-9302-2855d455897c", "/test-scheduler"

- Note the ouputDir when using LusidDrive must be from the root. i.e. don't forget to suffix with '/'

- An image is currently loaded into Harbour under findataex-dl-ci-test with a corresponing "Job" setup that can be used in CI. 

- Run with requestSource="LusidDrive", requestPath="f9bc7674-cedd-442e-9302-2855d455897c" outputDir="findataex-dl-ci-test"

- Note if you need to change the mock DL response file the lusid drive id of the file will likely also change. This means you must update the fde request file url to point to that new id. Saving the amended request file will also generate a new file id for the request file this time. So ensure your passing in the latest file id into the job args.


### Trouble Shooting

Any issues in the current workflow then take a look at https://github.com/finbourne/lusid-findataex/blob/feature/SE-286-LusidDriveOutput/Lusid.FinDataEx.Tests/Vendor/FinDataExRuntimeToLusidDriveTests.cs and debug through. That is an end to end test that follows the same processas described above but done from a local instance. You'll need a LUSID secrets file with your own credentials. 

If your private LUSID instance doesn't have drive access then add it following the instructions at https://wiki.finbourne.com/en/iam/licences#assigning-system-licences

Also beware of using the wrong api URL in your secrets. For drive it is https://your-tenant.lusid.com/drive and not https://your-tenant.lusid.com/api