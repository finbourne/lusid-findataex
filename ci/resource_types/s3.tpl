resource_types:
- name: s3
  type: docker-image
  source:
    repository: harbor.finbourne.com/tools/s3-resource-simple
    username: robot$concourse
    password: ((harbor.tools_token))