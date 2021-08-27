resources:
- name: s3-nuget-config
  type: s3
  source:
    bucket: fbn-prod-prod-config
    region: eu-west-1
    path: nuget    

merge:
- template: resource_types/s3.tpl     