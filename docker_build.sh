#!/bin/sh
set -eu

# in case we are being run from outside this directory
cd "$(dirname "$0")"

if [ -z "$*" ]; then 
    echo "You must provide a tag number"
    exit 1;
fi

docker build -f TestApp.Cli.Dockerfile . -t akhanal/my-test-cli:$1
docker build -f TestApp.Cli-Exec-Host.Dockerfile . -t akhanal/my-test-cli-exec-host:$1
docker build -f TestApp.Api.Dockerfile . -t akhanal/my-test-api:$1
docker build -f TestApp.Service.Dockerfile . -t akhanal/my-test-service:$1