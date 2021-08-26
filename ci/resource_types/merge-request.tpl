resource_types:
  - name: merge-request
    type: docker-image
    source:
      repository: harbor.finbourne.com/tools/concourse-gitlab-mr-resource
      username: robot$concourse
      password: ((harbor.tools_token))