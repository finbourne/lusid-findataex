merge:
  - template: resources/git/github/source-code-findataex.tpl
  - template: resources/git/github/findataex-version.tpl

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
      - in_parallel:
        - task: run-unit-tests
          config:
            {{ include "unit-tests.task.tpl" | indentSub 12 }}
