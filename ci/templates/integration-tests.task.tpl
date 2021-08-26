platform: linux 
image_resource:
  type: docker-image
  source: 
    repository: harbor.finbourne.com/build/dotnetcoresdk-3-1-awscli
    username: robot$concourse
    password: harbor.build_token
    tag: "0.0.33"
inputs:
  - name: source-code-findataex
  - name: findataex-version
run:
  path: /bin/bash
  args:
    - |
      echo "Starting Build"
      dotnet build $project_file

      # set the working directory to be the output folder
      cd Integrations/IntegrationTests/bin/Debug/netcoreapp3.1

      dotnet test $project_file --filter 'TestCategory=Unsafe'
