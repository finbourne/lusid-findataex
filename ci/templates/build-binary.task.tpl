platform: linux 
image_resource:
  type: docker-image
  source: 
    repository: harbor.finbourne.com/build/dotnetcoresdk-3-1-awscli
    username: robot$concourse
    password: ((harbor.build_token))
    tag: "0.0.33"
inputs:
  - name: source-code-findataex
outputs:
  - name: source-code-findataex
run:
  path: /bin/bash
  args:
  - 
    solution_file=src/Lusid.FinDataEx.sln

    echo "[INFO] Building Debug $solution_file"
    dotnet build -c Debug /warnaserror $solution_file

    echo "[INFO] Building Release $solution_file"
    dotnet build -c Release /warnaserror $solution_file
