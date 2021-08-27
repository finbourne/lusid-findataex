merge:
  - template: resources/git/github/source-code-findataex.tpl
  - template: resources/git/github/findataex-version.tpl
  - template: resources/s3/nuget-config.tpl

jobs:
  - name: unit-tests
    plan:
      - get: findataex-version
        params:
          bump: patch
      - get: source-code-findataex
        params:
          doppler_blameable: true
        trigger: true
      - get: nuget-config
        resource: s3-nuget-config
      - task: run-unit-tests
        config:
          {{ include "unit-tests.task.tpl" | indentSub 10 }}