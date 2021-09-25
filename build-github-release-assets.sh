#!/bin/bash
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
asset_dir="github_release_assets"
packageprefix="luger"

build_rids=("win-x64" "linux-x64")
declare -A build_types=(["runtime"]="--self-contained true -p:PublishSingleFile=true" ["no_runtime"]="")

stage "Tools check"
dotnet --version || die "No dotnet"
yarn --version || die "No yarn"

stage "Prepare dirs"
rm -rf $asset_dir
mkdir -p $asset_dir
rm -rf $frontenddir/build

stage "Build frontend"
yarn --cwd $frontenddir build

for rid in ${build_rids[@]}; do
    for build_type in ${!build_types[@]}; do
        packagename="$packageprefix"_"$build_type"_"$rid"
        packagepath="$asset_dir"/"$packagename"

        stage "Build backend $packagepath"
        dotnet publish --output $packagepath --configuration Release -r $rid ${build_types[$build_type]} $backenddir
        cp -R $frontenddir/build/* $packagepath/wwwroot
        pushd $asset_dir
        tar -cavf $packagename.tar.gz $packagename
        rm -rf $packagename
        popd
    done
done

stage "Done"
echo $asset_dir
