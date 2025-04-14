#!/usr/bin/env zsh
SCRIPT_path=$(/bin/readlink -f ${0%/*})
get_Json_info(){
    local ref=$(jq ".$1"  modinfo.json)
    echo ${ref//\"/}
}
version=$(get_Json_info "version")
modid=$(get_Json_info "modid")
VintagestoryDir="$HOME/.config/VintagestoryData/Mods"
Filename="${modid}-${version}.zip"

[[ ! -d build ]] && mkdir build
[ -f build/$Filename ] && rm -r build/${modid}-* 
zip -r build/$Filename assets modinfo.json modicon.png
echo $Filename
#cp build/$Filename $VintagestoryDir/$Filename
