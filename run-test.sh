#!/bin/bash
usage(){
    echo "#################################################################################################################"
    echo "# no argumants please enter a correct argument:                                                                 #"
    echo "#     -r  .. run          -to run vintage story in that instance with the mod without building a new mod file.  #"
    echo "#     -rb .. run-build    -to run vintagestory instance and build the mod.zip file.                             #"
    echo "#     -rp .. run-pre      -to to run the pre-release of vintage story placed in the $working_dir/Referance/VS-prerelease             #"
    echo "#     -b  .. build        -to build the mod.zip file keep in mind that the modinfo.json needs                   #"
    echo "#         ..              -to have the modid and version variable set for this file to be correctly made        #"
    echo "#################################################################################################################"
}
get_Json_info(){
    #local ref=$(jq -r ".$1"  modinfo.json)
    #echo ${ref//\"/}
    jq -r ".$1"  modinfo.json
}
run_fun(){
    export LD_DEBUG=true
    #./build.zsh
    #cd $vintagestory_dir
    cd $1
    ./Vintagestory --dataPath "$VSDataDIR" --addModPath "$build_dir" --playStyle "preset-surviveandbuild" --openWorld  "modding test world" --tracelog
}
SCRIPT_path=$(/bin/readlink -f ${0%/*})
build_dir="$(dirname "$(readlink -f "$0")")/build"
vintagestory_dir_pre=$SCRIPT_path/Referance/VS-prerelease
VSDataDIR=$SCRIPT_path/Referance/VSDataDIR
vintagestory_dir=/opt/vintagestory
vintagestory_mods_dir="$HOME/.config/VintagestoryData/Mods"
version=$(get_Json_info "version")
modid=$(get_Json_info "modid")
build_fun(){
    # Build function for making/packging the mod.zip file.
    # it will look at the modinfo.json file to make the file name,
    # so this is needed to have the moidid and version variables set.
    local Filename="${modid}-${version}.zip"

    [[ ! -d $build_dir ]] && mkdir build
    [ -f $build_dir/$Filename ] && rm -r build/${modid}-*

    zip -r $build_dir/$Filename assets modinfo.json modicon.png
    echo "mod $Filename buildt in:"
    echo $build_dir
}

build_run(){
    build_fun
    run_fun $1
}
#source Scripts/common.zsh
echo "$modid $version"

arg=$1
if [[ -n "${arg}" ]]; then
    $([ $1 = "build" ] || [ $1 = "-b" ]) && build_fun
    $([ $1 = "run" ] || [ $1 = "-r" ]) && run_fun $vintagestory_dir
    $([ $1 = "run-pre" ] || [ $1 = "-rp" ]) && run_fun $vintagestory_dir_pre
    $([ $1 = "run-build" ] || [ $1 = "-rb" ]) && build_run $vintagestory_dir
else
    usage
fi
