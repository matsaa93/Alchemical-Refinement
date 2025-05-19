get_Json_info(){
    local ref=$(jq ".$1"  $working_dir/modinfo.json)
    echo ${ref//\"/}
}
zcalc(){ echo $(($@)) }
zcalc_goldenratio_int(){ zcalc "$1 * 1.618" |awk '{print int($1+0.5)}' }
FA_SB(){ cat <<< "[$@]" }
FA_CB(){ cat <<< "{$@}" }
FA_C(){ cat <<< "\"$@\"" }
FA_V(){ cat <<< "$(FA_C $1): $2" }
FA_VD(){ cat <<< "${1}$(FA_V $2 $(FA_C $3))$4" }
FA_VC(){ cat <<< "${1}$(FA_V $2 $(FA_CB "$3"))$4" }
FA_VN(){ cat <<< "${1}$(FA_V "$2" "$3")$4" }

declare -a loop_array
declare -A loop_array_msg

working_dir="$HOME/Documents/GitHub/Alchemical-Refinement"
version=$(get_Json_info "version")
modid=$(get_Json_info "modid")
vintagestory_dir=/usr/share/vintagestory
vintagestory_mods_dir="$HOME/.config/VintagestoryData/Mods"
## common directories
assets_mod_dir=$working_dir/assets/$modid
Texture_mod_dir=$assets_mod_dir/textures
Shape_mod_dir=$assets_mod_dir/shapes
lang_mod_dir=$assets_mod_dir/lang
recipe_mod_dir=$assets_mod_dir/recipes
worldproperties_mod_dir=$assets_mod_dir/worldproperties
itemtypes_mod_dir=$assets_mod_dir/itemtypes
blocktypes_mod_dir=$assets_mod_dir/blocktypes
game_patches=$working_dir/assets/game/patches
game_textures=$working_dir/assets/game/textures
VS_texture_dir=$vintagestory_dir/assets/survival/textures
scripts_dir=$working_dir/Scripts
scripts_helper_dir=$scripts_dir/helpers
get_Json_output(){
    local ref=$(jq ".$1" $2)
    echo ${ref//\"/}
}
compare_variant_files(){
    for A in $(get_Json_output "variants[].Code" $1 ); do
        same=false
        for B in $(get_Json_output "variants[].Code" $2 ); do
            if [ $A = $B ]; then
                same=true
                break
            fi
        done
        [ $same = false ] && echo $A
    done
}
#compare_variant_files assets/alchemref/worldproperties/block/ore-mineral.json Referance/ore-graded.json > tmp/compared.txt
