groups:
  - name: all
    jobs:
      - merge-request
      - unit-tests
      - integration-tests
      - build-image

merge:
  - template: jobs/merge-request.tpl
  - template: jobs/unit-tests.tpl
  - template: jobs/integration-tests.tpl
  - template: jobs/build-image.tpl