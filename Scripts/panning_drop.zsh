FA_Panning_Drop(){
    #"@(bonysoil|bonysoil-.*)": [
	#			{ type: "item", code: "bone",  chance: { avg: 0.3, var: 0 }  }
	#		],
    local OUTPUT=$1
    local CHANCE=$2
    local VARIATION=$3
    local END=$4
    #echo "\t\t\t\t{ \"type\": \"item\", \"code\": \"$OUTPUT\",  \"chance\": { \"avg\": $CHANCE, \"var\": $VARIATION }  }$END"
    pan_file="${pan_file}{ \"type\": \"item\", \"code\": \"$OUTPUT\",  \"chance\": { \"avg\": $CHANCE, \"var\": $VARIATION }  }$END"
}

FA_Panning_Drop_loop(){
    local MODID=$1
    local CHANCE=$2
    local VARIATION=$3
    for D in $loop_array
    do
        FA_Panning_Drop "$MODID:powdered-ore-${D}-washed" "$CHANCE" "$VARIATION" ","
    done
}

FA_panning_file(){
    # copper=1084 bismuth=271 chromium=1907 gold=1063 ironbloom=1482 lead=327
    #echo "$(cat Scripts/helpers/ore-pan-start.txt)"
    local MODID=$1
    local lastC
    local ammount_of_item=${#material[@]}
    local lastitem="$material[$ammount_of_item]"
    pan_file=""
    for F in $material
    do
        if [ $F = $lastitem ]; then
            lastC=""
        else
            lastC=","
        fi
        #echo "\t\t\t\"$MODID:powdered-ore-sand-${F}\": ["
        pan_file="${pan_file}\"$MODID:powdered-ore-sand-${F}\": ["
        case $material_element[$F] in
            copper)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
                loop_array=( sphalerite galena magnetite limonite pyrite nativegold )
                FA_Panning_Drop_loop $MODID 0.1 0
                FA_Panning_Drop "game:crushed-quartz" "0.2" 0 ","
                FA_Panning_Drop "game:powder-sulfur" "0.3" 0
            ;;
            bismuth)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
                loop_array=( sphalerite galena chalcopyrite magnetite limonite pyrite )
                FA_Panning_Drop_loop $MODID 0.1 0
                FA_Panning_Drop "game:crushed-quartz" "0.2" 0 ","
                FA_Panning_Drop "game:powder-sulfur" "0.3" 0
            ;;
            chromium)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ""
            ;;
            gold)
                case $F in
                    *quartz*)
                        FA_Panning_Drop "$MODID:powdered-ore-nativegold-washed" "0.2" 0 ","
                        FA_Panning_Drop "game:crushed-quartz" "1" 0 ","
                    ;;
                    *)
                        FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
                    ;;
                esac
                loop_array=( sphalerite galena chalcopyrite magnetite )
                FA_Panning_Drop_loop $MODID 0.1 0
                FA_Panning_Drop "game:powder-sulfur" "0.1" 0 ","
                FA_Panning_Drop "$MODID:powdered-ore-nativesilver-washed" "0.05" 0 ","
                FA_Panning_Drop "$MODID:powdered-ore-pyrite-washed" "0.25" 0
            ;;
            silver)
                case $F in
                    *quartz*)
                        FA_Panning_Drop "$MODID:powdered-ore-nativesilver-washed" "0.2" 0 ","
                        FA_Panning_Drop "game:crushed-quartz" "1" 0 ","
                    ;;
                    *galena*)
                        FA_Panning_Drop "$MODID:powdered-ore-nativesilver-washed" "0.2" 0 ","
                        FA_Panning_Drop "$MODID:powdered-ore-galena-washed" "1" 0 ","
                    ;;
                    *)
                        FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
                        FA_Panning_Drop "$MODID:powdered-ore-galena-washed" "0.1" 0 ","
                    ;;
                esac
                loop_array=( sphalerite chalcopyrite magnetite malachite azurite tetrahedrite )
                FA_Panning_Drop_loop $MODID 0.05 0
                FA_Panning_Drop "game:powder-sulfur" "0.3" 0 ","
                FA_Panning_Drop "$MODID:powdered-ore-nativegold-washed" "0.05" 0 ","
                FA_Panning_Drop "$MODID:powdered-ore-pyrite-washed" "0.1" 0
            ;;
            ironbloom)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.2" 0 ","
                loop_array=( sphalerite galena chalcopyrite nativegold pentlandite )
                FA_Panning_Drop_loop $MODID 0.05 0
                FA_Panning_Drop "game:crushed-quartz" "0.2" 0 ","
                FA_Panning_Drop "game:powder-sulfur" "0.3" 0
            ;;
            lead)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
                loop_array=( nativesilver nativegold sphalerite chalcopyrite pyrite magnetite tetrahedrite azurite malachite )
                FA_Panning_Drop_loop $MODID 0.05 0
                FA_Panning_Drop "game:crushed-quartz" "0.1" 0 ","
                FA_Panning_Drop "game:powder-sulfur" "0.2" 0
            ;;
            platinum)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
                loop_array=( nativesilver nativegold sphalerite chalcopyrite pentlandite tetrahedrite azurite malachite )
                FA_Panning_Drop_loop $MODID 0.05 0
                FA_Panning_Drop "game:crushed-quartz" "0.1" 0 ","
                FA_Panning_Drop "game:powder-sulfur" "0.2" 0
            ;;
            nickel)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
                loop_array=( sphalerite chalcopyrite magnetite limonite hematite )
                FA_Panning_Drop_loop $MODID 0.05 0
                FA_Panning_Drop "game:crushed-quartz" "0.1" 0 ","
                FA_Panning_Drop "game:powder-sulfur" "0.2" 0
            ;;
            tin)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
                loop_array=( sphalerite chalcopyrite limonite magnetite pyrite )
                FA_Panning_Drop_loop $MODID 0.05 0
                FA_Panning_Drop "game:crushed-quartz" "0.1" 0 ","
                FA_Panning_Drop "game:powder-sulfur" "0.2" 0
            ;;
            zinc)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
                loop_array=( sphalerite chalcopyrite limonite magnetite pyrite galena )
                FA_Panning_Drop_loop $MODID 0.05 0
                FA_Panning_Drop "game:powder-sulfur" "0.2" 0
            ;;
            arsenic)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ""
            ;;
            antimony)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ""
            ;;
            uranium)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ""
            ;;
            manganese)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ""
            ;;
            *)
                echo "ERROR MISSING ELEMENT FA_panning_file: $F"
            ;;
        esac
        #echo "\t\t\t]$lastC"
        pan_file="${pan_file}]$lastC"
        #f="$(cat ore-pan-drop.txt)"
        #f="$(echo ${f//INPUT/alchemref:powdered-ore-sand-${F}})"
        #f="$(echo ${f//OUTPUT/alchemref:powdered-ore-${F}-washed})"
        #powdered-ore-sand-${F}
        #f="$(echo ${f//Variable2/${material_element[$F]}})"
        #echo $f
    done
    #echo "$(cat Scripts/helpers/ore-pan-start.txt)"
    #cat Scripts/helpers/ore-pan-end.txt
    echo "$(cat Scripts/helpers/ore-pan-start.txt) $pan_file $(cat Scripts/helpers/ore-pan-end.txt)" | jq "."
}