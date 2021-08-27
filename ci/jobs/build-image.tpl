merge:
  - template: resources/git/github/source-code-findataex.tpl
  - template: resources/git/github/findataex-version.tpl
  - template: resources/docker/findataex-image.tpl
  - template: resources/s3/nuget-config.tpl

jobs:
  - name: build-image
    plan:
      - get: findataex-version
        params:
          bump: patch
      - get: source-code-findataex
        params:
          doppler_blameable: true
        trigger: true
        passed:
          - integration-tests
      - get: nuget-config
        resource: s3-nuget-config
      - task: build-binary
        config:
          {{ include "build-binary.task.tpl" | indentSub 10 }}
      - put: findataex-image
        params:
          tag_file: findataex-version/version
          tag_as_latest: true
          build: source-code-findataex/docker
          build_args:
            FLY_VERSION: "7.1.0"
      - put: findataex-version
        params:
          file: findataex-version/version
