merge:
  - template: resources/git/github/findataex-version.tpl
  - template: resources/merge-request/merge-request.tpl
  - template: resources/git/github/source-code-findataex.tpl

jobs:
  - name: merge-request
    plan:
      - get: source-code-findataex
        resource: merge-request
        version: every
        trigger: true
      - get: findataex-version
      - put: merge-request
        params:
          repository: source-code-findataex
          status: running
      - in_parallel:
        - task: run-unit-tests
          config:
            {{ include "unit-tests.task.tpl" | indentSub 12 }}
    on_failure:
      put: merge-request
      params:
        repository: source-code-findataex
        status: failed
    on_success:
      put: merge-request
      params:
        repository: source-code-findataex
        status: success