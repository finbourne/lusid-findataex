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

      dotnet restore source-code-findataex/src/Lusid.FinDataEx.sln --configfile nuget-config/nuget.config
      dotnet test source-code-findataex/src/Lusid.FinDataEx.sln --filter 'TestCategory!=Unsafe'