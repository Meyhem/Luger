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

frontenddir="Luger.React"
backenddir="Luger"
packagedir="luger_package"

stage "Tools check"
dotnet --version || die "No dotnet"
yarn --version || die "No yarn"

stage "Prepare dirs"
rm -rf $packagedir
mkdir -p $packagedir

stage "Build backend"
dotnet publish --output $packagedir $backenddir --configuration Release

stage "Build frontend"
yarn --cwd $frontenddir build
cp -R $frontenddir/build/* $packagedir/wwwroot

stage "Done"
echo $packagedir
