platform: linux 
image_resource:
  type: docker-image
  source: 
    repository: harbor.finbourne.com/build/dotnetcoresdk-3-1-awscli
    username: robot$concourse
    password: ((harbor.build_token))
    tag: "0.0.33"
inputs:
  - name: source-code-findataex
  - name: findataex-version
  - name: nuget-config
run:
  path: /bin/bash
  args:
    - -cel
    - |
      export VERSION=$(cat oktahttpcachingproxy-version/version)
      echo "Starting testing of v${VERSION}"

      tokenUrl='((findataex-ci.token_url))'
      username='((findataex-ci.username))'
      password='((findataex-ci.password))'
      clientId='((findataex-ci.client_id))'
      clientSecret='((findataex-ci.client_secret))'
      lusidApi='((findataex-ci.lusid_api))'
      driveApi='((findataex-ci.drive_api))'
      appName='((findataex-ci.app_name))'
      bbgCert='((findataex-ci.bbg_cert))'
      bbgPass='((findataex-ci.bbg_pass))'

      export FBN_TOKEN_URL="$tokenUrl"
      export FBN_USERNAME="$username"
      export FBN_PASSWORD="$password"
      export FBN_CLIENT_ID="$clientId"
      export FBN_CLIENT_SECRET="$clientSecret"
      export FBN_LUSID_API_URL="$lusidApi"
      export FBN_DRIVE_API_URL="$driveApi"
      export FBN_APP_NAME="$appName"
      export BBG_DL_CERT_DATA="$bbgCert"
      export BBG_DL_CERT_PASS="$bbgPass"

      dotnet restore source-code-findataex/src/Lusid.FinDataEx.sln --configfile nuget-config/nuget.config
      dotnet test source-code-findataex/src/Lusid.FinDataEx.sln --filter 'TestCategory=Unsafe'