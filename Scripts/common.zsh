get_Json_info(){
    local ref=$(jq ".$1"  modinfo.json)
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
