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
      #ls -la

      #cp nuget-config/nuget.config .
      #nuget_path=./nuget.config

      export VERSION=$(cat oktahttpcachingproxy-version/version)

      echo "Starting testing of v${VERSION}"

      echo "Starting Build"
      #dotnet build $project_file

      # set the working directory to be the output folder
      #cd Integrations/IntegrationTests/bin/Debug/netcoreapp3.1
      #cd source-code-findataex

      dotnet restore source-code-findataex/src/Lusid.FinDataEx.sln --configfile nuget-config/nuget.config

      dotnet test source-code-findataex/src/Lusid.FinDataEx.sln --filter 'TestCategory!=Unsafe'
