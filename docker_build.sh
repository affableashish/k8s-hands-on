#!/bin/sh
set -eu

# in case we are being run from outside this directory
cd "$(dirname "$0")"

if [ -z "$*" ]; then 
    echo "You must provide a tag number"
    exit 1;
fi

docker build -f TestApp.Cli.Dockerfile . -t andrewlock/my-test-cli:$1
docker build -f TestApp.Cli-Exec-Host.Dockerfile . -t andrewlock/my-test-cli-exec-host:$1
docker build -f TestApp.Api.Dockerfile . -t andrewlock/my-test-api:$1
docker build -f TestApp.Service.Dockerfile . -t andrewlock/my-test-service:$1