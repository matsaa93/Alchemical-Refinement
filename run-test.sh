#!/bin/bash
build_dir="$(dirname "$(readlink -f "$0")")/build"
vintagestorydir=/usr/share/vintagestory
export LD_DEBUG=true
./build.zsh
echo "mod buildt in:"
echo $build_dir
#cp build/alchemical-ref* /home/matsaa93/.var/app/at.vintagestory.VintageStory/config/VintagestoryData/Mods/
#flatpak run at.vintagestory.VintageStory --addModPath "$build_dir"
cd $vintagestorydir
./Vintagestory --addModPath "$build_dir" --playStyle "preset-surviveandbuild" --openWorld  "modding test world" --tracelog
#popd


