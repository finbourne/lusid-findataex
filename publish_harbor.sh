#!/bin/bash -e

usage()
{
  echo
  echo "Usage: $0 "
  echo
  echo " -u <harbor username>"
  echo " -p <harbor password>"
  echo " -d <harbor domain>"
  echo " -t <harbor team>"
  echo " -n <image name>"
  echo " -g <image tag>"
  echo
  exit 1
}

while getopts 'u:p:d:t:n:g:' opt
do
  case $opt in
    u) harbor_username=$OPTARG ;;
    p) harbor_password=$OPTARG ;;
    d) harbor_domain=$OPTARG ;;
    t) harbor_team=$OPTARG ;;
    n) image_name=$OPTARG ;;
    g) image_tag=$OPTARG ;;
  esac
done

# validate params
[ -z "$harbor_username" ] && usage
[ -z "$harbor_password" ] && usage
[ -z "$harbor_domain" ] && usage
[ -z "$harbor_team" ] && usage
[ -z "$image_name" ] && usage
[ -z "$image_tag" ] && usage

docker login $harbor_domain -u $harbor_username -p $harbor_password

docker build -t $harbor_domain/$harbor_team/$image_name:$image_tag .
docker build -t $harbor_domain/$harbor_team/$image_name:latest .

docker push $harbor_domain/$harbor_team/$image_name:$image_tag
docker push $harbor_domain/$harbor_team/$image_name:latest