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
      cp ../nuget-config/nuget.config .
      nuget_path=./nuget.config

      export VERSION=$(cat oktahttpcachingproxy-version/version)

      echo "Starting testing of v${VERSION}"

      echo "Starting Build"
      #dotnet build $project_file

      # set the working directory to be the output folder
      #cd Integrations/IntegrationTests/bin/Debug/netcoreapp3.1
      cd source-code-findataex
      
      ls -la

      dotnet test ./src/Lusid.FinDataEx.sln --filter 'TestCategory!=Unsafe'
