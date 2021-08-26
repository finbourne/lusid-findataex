resources:
  - name: findataex-image
    type: docker-image
    icon: docker
    source:
      repository: harbor.finbourne.com/tools/findataex
      username: robot$concourse
      password: ((harbor.tools_token))