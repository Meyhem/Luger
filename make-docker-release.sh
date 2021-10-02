#!/bin/sh

set -e

NSTAGE=1
stage() {
  echo "============= Stage $NSTAGE - $1 ============="
  NSTAGE=$((NSTAGE+1))
}

die() {
  echo "Error: $1"
  exit 1
}

stage "Checks"

release_version=$1
if [ -z "$release_version" ]; then
    die "Missing release version"
fi

current_branch=$(git rev-parse --abbrev-ref HEAD)
if [ "$current_branch" != "master" ]; then
    die "Not on master branch"
fi

existing=$(git tag | { grep -P "^$release_version$" || true; } )
if [ -z "$existing" ]; then
    die "Release tag $release_version does not exist"
fi

stage "Build image"

docker build -t luger:$release_version .
docker tag luger:$release_version luger:latest
docker tag luger:$release_version meyhem/luger:$release_version
docker tag luger:latest meyhem/luger:latest

docker push meyhem/luger:$release_version
docker push meyhem/luger:latest
