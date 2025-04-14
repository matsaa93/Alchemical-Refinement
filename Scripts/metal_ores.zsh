FA_combustion_prop(){
    ## FA_combustion_prop material {type/state} "domain:item"
    local combust
    combust="\n\t\t\t\"meltingPoint\": ${material_temp[$1]}, \"meltingDuration\": ${meltingDuration[$2]}, \"smeltedRatio\": $smeltedRatio[$2]"
    combust="${combust}\n\t\t\t\"smeltedStack\": { \"type\": \"item\", \"code\": \"$3\" }"
    echo "\n\t\t\"*-${1}-$2\": {${combust}\n\t\t},"
}

FA_combustion_prop_file(){
    #"*-malachite-calcinated": {
	#		meltingPoint: 1084,
	#		meltingDuration: 30,
	#		smeltedRatio: 16,
	#		smeltedStack: { type: "item", code: "game:ingot-copper" }
	#	},
    local file=""
    local ammount_of_item=${#material[@]}
    local combust
    local ingot
    #echo > xcumbust.txt
    declare -A smeltedRatio
    declare -A meltingDuration
    meltingDuration[calcinated]=25
    meltingDuration[washed]=20
    smeltedRatio[calcinated]=16
    smeltedRatio[washed]=14
    for F in $material
    do
        ingot="game:ingot-$material_element[$F]"
        [ $material_element[$F] = "ironbloom" ] && ingot="game:ironbloom"
        for type in calcinated washed
        do
            #f="$(cat zcombustprop.txt)"
            #f="$(echo ${f//Variable1/${F}-${type}})"
            #f="$(echo ${f//amount/$smeltedRatio[${type}]})"
            #f="$(echo ${f//Temp/${material_temp[$F]}})"
            #f="$(echo ${f//Variable2/${material_element[$F]}})"
            #echo $f
            #combust="\n\t\t\t\"meltingPoint\": ${material_temp[$F]}, \"meltingDuration\": ${meltingDuration[$type]}, \"smeltedRatio\": $smeltedRatio[$type]"
            #combust="${combust}\n\t\t\t\"smeltedStack\": { \"type\": \"item\", \"code\": \"game:$ingot\" }"
            #file="$file\n\t\t\"*-${F}-$type\": {${combust}\n\t\t},"
            file="${file}$(FA_combustion_prop $F $type $ingot)"
        done
    done
    echo "\n\t\"combustiblePropsByType\": {$file\n\t}"
}

