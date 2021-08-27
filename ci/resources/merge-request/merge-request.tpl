merge:
  - template: resource_types/merge-request.tpl

resources:
  - name: merge-request
    type: merge-request
    icon: source-pull
    source:
      uri: git@github.com:finbourne/lusid-findataex.git
      private_key: ((github.id_rsa))
      # private_token: ((github.access_token))
