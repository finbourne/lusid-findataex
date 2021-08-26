resources:

  - name: findataex-version
    type: semver
    icon: exponent
    source:
      driver: git
      branch: master
      uri: git@gitlab.finbourne.com:cicd/versions.git
      file: findataex.version
      private_key: ((gitlab.id_rsa))
      initial_version: 1.0.0