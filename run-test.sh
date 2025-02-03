#!/bin/bash
build_dir="$(dirname "$(readlink -f "$0")")/build"
vintagestorydir=/usr/share/vintagestory
./build.zsh
echo "mod buildt in:"
echo $build_dir
cd $vintagestorydir
./Vintagestory --addModPath "$build_dir" --playStyle "preset-surviveandbuild" --openWorld  "modding test world" --tracelog
#popd


