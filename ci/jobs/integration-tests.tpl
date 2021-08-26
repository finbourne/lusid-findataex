merge:
  - template: resources/git/github/source-code-findataex.tpl
  - template: resources/git/github/findataex-version.tpl

jobs:
  - name: integration-tests
    plan:
      - get: findataex-version
        params:
          bump: patch
      - get: source-code-findataex
        params:
          doppler_blameable: true
        trigger: true
        passed:
          - unit-tests
      - in_parallel:
        - task: run-integration-tests
          config:
            {{ include "integration-tests.task.tpl" | indentSub 12 }}
