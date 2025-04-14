FA_Lang_File_powdered_ore(){
    local MODID=$1
    for F in $material
    do
        material_Upcase="$(echo "$F" | sed 's/.*/\u&/')" 
        s="\n\t// ${material_Upcase} section:"
        f="\n\t\"$MODID:item-powdered-ore-${F}-raw\": \"Powdered ${material_Upcase} Raw\","
        n="\n\t\"$MODID:item-powdered-ore-${F}-calcinated\": \"Powdered ${material_Upcase} Calcinated\","
        m="\n\t\"$MODID:item-powdered-ore-${F}-washed\": \"Powdered ${material_Upcase} washed\","
        t="\n\t\"$MODID:block-powdered-ore-sand-${F}\": \"Ore Sand ${material_Upcase}\","
        g="\n\t\"$MODID:block-washed-powdered-ore-sand-${F}\": \"Washed Ore Sand ${material_Upcase}\","
        printf "${s}${f}${n}${m}${t}${g}"
    done
}

FA_LANG_entry(){
    local MODID=$1; local type=$2; local code=$3; local variantA=$4; local variantB=$5
    local variantUpCase="$(echo "$variantB" | sed 's/.*/\u&/') $(echo "${code//_/ }" | sed 's/.*/\u&/') $(echo "$variantA" | sed 's/.*/\u&/')"
    echo "\t\"$MODID:$type-$code-$variantA-$variantB\": \"$variantUpCase\","
}

FA_Lang_File_two_variants(){
    local MODID=$2; local type=$3; local code=$4
    for F in $(jq ".variants[].Code" $worldproperties_mod_dir/block/$1)
    do
        echo ""
        for D in $loop_array
        do 
            FA_LANG_entry $MODID $type $code "${F//\"/}" $D
        done
        FA_LANG_entry $MODID "${type}desc" $code "${F//\"/}" "*"
    done
}
FA_LANG_HANDBOOK_entry(){
    local F="$(cat Scripts/helpers/handbook.json)"
    echo ${F//VAR/$1}
}
FA_LANG_HANDBOOK_variants(){
    for F in $(jq ".variants[].Code" $worldproperties_mod_dir/block/$1.json); do
        FA_LANG_HANDBOOK_entry "${F//\"/}_$1" > $assets_mod_dir/config/handbook/${F//\"/}.json
    done
}
FA_LANG_HANDBOOK_variants vitriol

#FA_LANG_HANDBOOK_entry blue_vitriol

#loop_array=( "rough" "chunk" "tall" "cluster" )
#FA_Lang_File_two_variants vitriol.json $modid item "crystal_vitriol" > tmp/lang_crystal_vitriol.json
#
#loop_array=( "raw" "washed" "purified" )
#FA_Lang_File_two_variants vitriol.json $modid item "powdered_vitriol" > tmp/lang_powdered_vitriol.json

