#!/bin/sh
set -e

NSTAGE=1
stage() {
  echo "============= Stage $NSTAGE - $1 ============="
  NSTAGE=$((NSTAGE+1))
}

die () {
  echo "Build failed: $1"
  exit 1
}

frontenddir="luger.react"
backenddir="Luger.Api"
packagedir="luger_package"

stage "Tools check"
dotnet --version || die "No dotnet"
yarn --version || die "No yarn"

stage "Prepare dirs"
rm -rf $packagedir
mkdir $packagedir

stage "Build backend"
dotnet publish --output $packagedir $backenddir

stage "Build frontend"
yarn --cwd $frontenddir build
cp -R $frontenddir/build/* $packagedir/wwwroot

stage "Done"
echo $packagedir