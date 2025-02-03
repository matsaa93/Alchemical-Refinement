#!/usr/bin/env zsh
debug=true
declare -A material_stacksizex
declare -A material_tier
declare -A material_grade
declare -A material_grade_bountiful 
declare -A material_grade_rich 
declare -A material_grade_medium 
declare -A material_grade_poor
declare -A material_element
declare -A material_temp
declare -a material
zcalc(){ echo $(($@)) }
zcalc_goldenratio_int(){ zcalc "$1 * 1.618" |awk '{print int($1+0.5)}' }
FA_SB(){ cat <<< "[$@]" }
FA_CB(){ cat <<< "{$@}" }
FA_C(){ cat <<< "\"$@\"" }
FA_V(){ cat <<< "$(FA_C $1): $2" }
FA_VD(){ cat <<< "${1}$(FA_V $2 $(FA_C $3))$4" }
FA_VC(){ cat <<< "${1}$(FA_V $2 $(FA_CB "$3"))$4" }
FA_VN(){ cat <<< "${1}$(FA_V "$2" "$3")$4" }

FA_OP(){
    mode=$2
    local p
    [[ $mode == "crush" ]] && p="crushingPropsByType"
    [[ $mode == "grind" ]] && p="grindingPropsByType"
    local section="$(FA_VD "\n\t" "op" "$1" ",")"
    section="${section}$(FA_VD "\n\t" "path" "/$p" ",")"
    section="${section}\n\t\"value\": {${3}\n\t},"
    section="${section}$(FA_VD "\n\t" "file" "${4}.json")"
    section="\n\t$(FA_CB "$section\n\t")"
    cat <<< "$section"
}
FA_IS(){
    local section="$(FA_VD "\n\t\t\t\t" "type" "item" ",")"
    section="${section}$(FA_VD "\n\t\t\t\t" "code" "$2" ",")"
    section="${section}$(FA_VN "\n\t\t\t\t" "stacksize" "$3")"
    case $mode in
        "crush")
            section="\n\t\t\t\"crushedStack\": {${section}\n\t\t\t},"
            section="${section}$(FA_VN "\n\t\t\t" "hardnessTier" "$4")"
            ;;
        "grind")
            section="\n\t\t\t\"grindedStack\": {${section}\n\t\t\t}"
            ;;
    esac
    #section="\n\t\t\"${1}\": {${section}\n\t\t},"
    [[ $last == false ]] && section="\n\t\t\"${1}\": {${section}\n\t\t},"
    [[ $last == true ]] && section="\n\t\t\"${1}\": {${section}\n\t\t}"
    cat <<< "$section"
}
FA_grind_crush(){
    local ammount_of_item=${#material[@]}
    last=false
    local lastC=false
    local sectionC=""
    mode=$1 
    for F in $material
    do
        material_grade[poor]=$material_grade_poor[$F]
        material_grade[medium]=$material_grade_medium[$F]
        material_grade[rich]=$material_grade_rich[$F]
        material_grade[bountiful]=$material_grade_bountiful[$F]
        [[ $F == $material[$ammount_of_item] ]] && lastC=true
        if [[ $F == quartz_* ]]; then
            Fi=$(echo $F | cut -c 8-)
        else
            Fi=$F
        fi
        for G in poor medium rich bountiful
        do
            $([[ $lastC == true ]] && [[ $G == "bountiful" ]]) && last=true 
            sectionC="${sectionC}$(FA_IS "ore-${G}-$F-*" "nugget-$Fi" "$material_grade[$G]" "$material_tier[$F]")"
            #printf "\n${F}_${G}: $material_grade[$G]"
        done
    done
    #local section="$(FA_VC "\n\t\t" "crushedStack" "$(FA_IS)" ",")"
    cat <<< "$sectionC"
}
#printf "$(FA_IS)"
#printf "$(FA_OP add crush "$(FA_TEST)")"


F_write_start(){
    local file=${1}.json
    printf "\t{\n\t\"op\": \"${2}\",\n\t\"path\": \"${3}\",\n\t\"value\": {"
}
F_write_value(){
    local ending
    local starting
    if [[ $5 == true ]]; then
        starting="\n\t\t\"$6\": {\n\t\t  \"$2\":"
    else
        starting="\n\t\t\"$2\":"
    fi
    if [[ $4 == true ]]; then
        ending="\n\t\t},"
    else
        ending=","
    fi
    printf "$starting {\n\t\t\t\"type\": \"item\",\n\t\t\t\"code\": \"${1}\",\n\t\t\t\"stacksize\": $3 \n\t\t\t}$ending"
}
F_write_file(){
    printf "\n\t\"file\": \"$1\"\n\t}$2"
}
F_write_hardness(){ printf "\n\t\t\"hardnessTier\": ${1}\n\t\t}${2}" }

F_test_write(){
    F_write_start cas add /grindingPropsByType
    F_write_value cas crushedStack 4 false true nugget-cas
    F_write_hardness 2 \,
    F_write_file itemtypes/resource/nugget.json \,
}
F_array_grade_set(){
    if [[ $3 == true ]]; then
        material_grade_poor[${1}]=$(zcalc_goldenratio_int $2)
        #[[ $debug == true ]] && printf "\n\t poor: $material_grade_poor[$1]"
        material_grade_medium[${1}]=$(zcalc_goldenratio_int $material_grade_poor[$1])
        #[[ $debug == true ]] && printf "\n\t medium: $material_grade_medium[$1]"
        material_grade_rich[${1}]=$(zcalc_goldenratio_int $material_grade_medium[$1])
        #[[ $debug == true ]] && printf "\n\t rich: $material_grade_rich[$1]"
        material_grade_bountiful[${1}]=$(zcalc_goldenratio_int $material_grade_rich[$1])
        #[[ $debug == true ]] && printf "\n\t bountiful: $material_grade_bountiful[$1]"
    else
        #local poor=$(zcalc "$2 + 1")
        #local medium=$(zcalc "$2 + 2")
        #local rich=$(zcalc "$2 + 3")
        #local bountiful=$(zcalc "$2 + 4")
        material_grade_poor[${1}]=$(zcalc_goldenratio_int $(zcalc "$2 + 1"))
        material_grade_medium[${1}]=$(zcalc_goldenratio_int $(zcalc "$2 + 2"))
        material_grade_rich[${1}]=$(zcalc_goldenratio_int $(zcalc "$2 + 3"))
        material_grade_bountiful[${1}]=$(zcalc_goldenratio_int $(zcalc "$2 + 4"))
    fi
    if [[ $debug == true ]]; then
        printf "\n\t poor: $material_grade_poor[$1]"
        printf "\n\t medium: $material_grade_medium[$1]"
        printf "\n\t rich: $material_grade_rich[$1]"
        printf "\n\t bountiful: $material_grade_bountiful[$1]"
    fi
}
F_array_material_set(){
    material+=$1
    material_element[${1}]="$2"
    material_tier[${1}]=$3
    material_stacksizex[${1}]=$4
    material_temp[${1}]=$5
    if [[ $debug == true ]]; then
        printf "\nMaterial: ${1}"; 
        printf "\n\tTier: $material_tier[${1}]" 
        printf "\n\tStacksize: $material_stacksizex[${1}]"
        printf "\n\tTemp: $material_temp[${1}]"
        printf "\n\tElement: $material_element[${1}]"
    fi
    F_array_grade_set $1 $4 $6
}
# copper=1084 bismuth=271 chromium=1907 gold=1063 ironbloom=1482 lead=327
F_array_test(){
    F_array_material_set bismuthinite bismuth 2 1.8 271
    F_array_material_set azurite copper 2 1.5 1084
    F_array_material_set chalcopyrite copper 2 1 1084
    F_array_material_set chalcocite copper 2 1 1084
    F_array_material_set tetrahedrite copper 2 1 1084
    F_array_material_set malachite copper 2 1 1084
    F_array_material_set nativecopper copper 2 1.5 1084
    F_array_material_set chromite chromium 3 1.7 1907
    F_array_material_set hematite ironbloom 3 1.8 1482
    F_array_material_set limonite ironbloom 2 1 1482
    F_array_material_set magnetite ironbloom 3 1.5 1482
    F_array_material_set pyrite ironbloom 2 1 1482
    F_array_material_set cerussite lead 2 2 327
    F_array_material_set galena lead 2 1.7 327
    F_array_material_set vanadinite lead 2 2 327
    F_array_material_set wulfenite lead 2 1.5 327
    F_array_material_set pentlandite nickel 3 0.5 1084
    F_array_material_set galena_nativesilver silver 2 1.7 961
    F_array_material_set quartz_nativesilver silver 2 0.6 961
    F_array_material_set nativesilver silver 2 1.6 961
    F_array_material_set freibergite silver 2 1 961
    F_array_material_set nativegold gold 2 1 1063
    F_array_material_set quartz_nativegold gold 2 0.3 1063
}
F_array_test
F_array_test_Geology_Addition(){
    F_array_material_set franckeite tin 2 1.2 232
    F_array_material_set cassiterite tin 2 0.3 232
    F_array_material_set teallite tin 2 1 232
    F_array_material_set nativeplatinum platinum 3 1 1770
    F_array_material_set sperrylite platinum 3 1 1770
    F_array_material_set hemimorphite zinc 2 1.5 419
    F_array_material_set smithsonite zinc 2 1.5 419
    F_array_material_set sphalerite zinc 2 1.5 419
}
F_array_test_Geology_Addition
F_array_test_OresAPlanty(){
    F_array_material_set stibnite arsenic 2 1.5
    F_array_material_set uranium uranium 3 1.5
    F_array_material_set rhodochrosite manganese 3 1.7
}
F_array_test_OresAPlanty
printf "$material ${#material[@]}\n"
FA_item_oregraded(){
    testing="$(FA_grind_crush crush)"
    testing="$(FA_OP add crush $testing "game:itemtypes/resource/ore-graded")"
    printf "["
    printf "$testing"
    printf "\n]"
}
FA_Grid_orechunk(){
    local SectionC=""
    sectionC="${sectionC}\n\t{"
    sectionC="${sectionC}\n\t\tingredientPattern: \"H   S\""
    sectionC="${sectionC}\n\t\tingredients: {"
    sectionC="${sectionC}\n\t\t\t \"H\": { type: \"item\", code: \"hammer-*\", isTool: true },"
    sectionC="${sectionC}\n\t\t\t \"S\": { type: \"block\", name: \"ore\", code: \"ore-${1}-*\" }"
    sectionC="${sectionC}\n\t\t},"
    sectionC="${sectionC}\n\t\twidth: 1,"
    sectionC="${sectionC}\n\t\theight: 2,"
    sectionC="${sectionC}\n\t\toutput: { type: \"item\", code: \"ore-${1}-*\", quantity: $2 }"
    if [[ $last == true ]]; then
        sectionC="${sectionC}\n\t}"
        
    else 
        sectionC="${sectionC}\n\t},"
    fi
    cat <<< "$sectionC"
}

FA_block_oregraded(){
    local section=""
    #mode="crush"
    last=false
    lastC=false
    local i
    local ammount_of_item=${#material[@]}
    for F in $material 
    do
        i=1
        [[ $F == $material[$ammount_of_item] ]] && lastC=true
        for G in poor medium rich bountiful
        do
            $([[ $lastC == true ]] && [[ $G == "bountiful" ]]) && last=true
            section="${section}$(FA_Grid_orechunk "${G}-${F}" $i)"
            i=$(zcalc "$i + 1")
        done
    done
    #lastC=true
    #section=""
    printf "["
    printf $section
    printf "\n]"
}

FA_combustion_prop(){
    #"*-malachite-calcinated": {
	#		meltingPoint: 1084,
	#		meltingDuration: 30,
	#		smeltedRatio: 16,
	#		smeltedStack: { type: "item", code: "game:ingot-copper" }
	#	},
    local file
    #local ammount_of_item=${#material[@]}
    #echo > xcumbust.txt
    for F in $material
    do
        for type in calcinated washed
        do 
            f="$(cat zcombustprop.txt)"
            f="$(echo ${f//Variable1/${F}-${type}})"
            f="$(echo ${f//Temp/${material_temp[$F]}})"
            f="$(echo ${f//Variable2/${material_element[$F]}})"
            echo $f
        done
    done
}
FA_Lang_File_powdered_ore(){
    for F in $material
    do
        material_Upcase="$(echo "$F" | sed 's/.*/\u&/')" 
        s="\n\t// ${material_Upcase} section:"
        f="\n\t\"alchemical-refinement:item-powdered-ore-${F}-raw\": \"Powdered ${material_Upcase} Raw\","
        n="\n\t\"alchemical-refinement:item-powdered-ore-${F}-calcinated\": \"Powdered ${material_Upcase} Calcinated\","
        m="\n\t\"alchemical-refinement:item-powdered-ore-${F}-washed\": \"Powdered ${material_Upcase} washed\","
        t="\n\t\"alchemical-refinement:block-powdered-ore-sand-${F}\": \"Ore Sand ${material_Upcase}\","
        g="\n\t\"alchemical-refinement:block-washed-powdered-ore-sand-${F}\": \"Washed Ore Sand ${material_Upcase}\","
        printf "${s}${f}${n}${m}${t}${g}"
    done
}
FA_Panning_Drop(){
    #"@(bonysoil|bonysoil-.*)": [
	#			{ type: "item", code: "bone",  chance: { avg: 0.3, var: 0 }  }
	#		],
    local OUTPUT=$1
    local CHANCE=$2
    local VARIATION=$3
    local END=$4
    echo "\t\t\t\t{ \"type\": \"item\", \"code\": \"$OUTPUT\",  \"chance\": { \"avg\": $CHANCE, \"var\": $VARIATION }  }$END"
}
FA_panning_file(){
    # copper=1084 bismuth=271 chromium=1907 gold=1063 ironbloom=1482 lead=327
    echo "$(cat ore-pan-start.txt)"
    local MODID=$1
    for F in $material
    do
        echo "\t\t\t\"$MODID:powdered-ore-sand-${F}\": ["
        case $material_element[$F] in
            copper)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
                for D in sphalerite galena magnetite limonite pyrite nativegold
                do
                    FA_Panning_Drop "$MODID:powdered-ore-${D}-washed" "0.1" 0 ","
                done
                FA_Panning_Drop "game:crushed-quartz" "0.2" 0 ","
                FA_Panning_Drop "game:powder-sulfur" "0.3" 0
            ;;
            bismuth)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
                for D in sphalerite galena chalcopyrite magnetite limonite pyrite
                do
                    FA_Panning_Drop "$MODID:powdered-ore-${D}-washed" "0.1" 0 ","
                done
                FA_Panning_Drop "game:crushed-quartz" "0.2" 0 ","
                FA_Panning_Drop "game:powder-sulfur" "0.3" 0
            ;;
            chromium)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
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
                for D in sphalerite galena chalcopyrite magnetite
                do
                    FA_Panning_Drop "$MODID:powdered-ore-${D}-washed" "0.1" 0 ","
                done
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
                for D in sphalerite chalcopyrite magnetite malachite azurite tetrahedrite
                do
                    FA_Panning_Drop "$MODID:powdered-ore-${D}-washed" "0.05" 0 ","
                done
                FA_Panning_Drop "game:powder-sulfur" "0.3" 0 ","
                FA_Panning_Drop "$MODID:powdered-ore-nativegold-washed" "0.05" 0 ","
                FA_Panning_Drop "$MODID:powdered-ore-pyrite-washed" "0.1" 0
            ;;
            ironbloom)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.2" 0 ","
                for D in sphalerite galena chalcopyrite nativegold pentlandite
                do
                    FA_Panning_Drop "$MODID:powdered-ore-${D}-washed" "0.05" 0 ","
                done
                FA_Panning_Drop "game:crushed-quartz" "0.2" 0 ","
                FA_Panning_Drop "game:powder-sulfur" "0.3" 0
            ;;
            lead)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
                for D in nativesilver nativegold sphalerite chalcopyrite pyrite magnetite tetrahedrite azurite malachite
                do
                    FA_Panning_Drop "$MODID:powdered-ore-${D}-washed" "0.05" 0 ","
                done
                FA_Panning_Drop "game:crushed-quartz" "0.1" 0 ","
                FA_Panning_Drop "game:powder-sulfur" "0.2" 0
            ;;
            platinum)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
                for D in nativesilver nativegold sphalerite chalcopyrite pentlandite tetrahedrite azurite malachite
                do
                    FA_Panning_Drop "$MODID:powdered-ore-${D}-washed" "0.05" 0 ","
                done
                FA_Panning_Drop "game:crushed-quartz" "0.1" 0 ","
                FA_Panning_Drop "game:powder-sulfur" "0.2" 0
            ;;
            nickel)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
                for D in sphalerite chalcopyrite magnetite limonite hematite
                do
                    FA_Panning_Drop "$MODID:powdered-ore-${D}-washed" "0.05" 0 ","
                done
                FA_Panning_Drop "game:crushed-quartz" "0.1" 0 ","
                FA_Panning_Drop "game:powder-sulfur" "0.2" 0
            ;;
            tin)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
                for D in sphalerite chalcopyrite limonite magnetite pyrite
                do
                    FA_Panning_Drop "$MODID:powdered-ore-${D}-washed" "0.05" 0 ","
                done
                FA_Panning_Drop "game:crushed-quartz" "0.1" 0 ","
                FA_Panning_Drop "game:powder-sulfur" "0.2" 0
            ;;
            zinc)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
                for D in sphalerite chalcopyrite limonite magnetite pyrite galena
                do
                    FA_Panning_Drop "$MODID:powdered-ore-${D}-washed" "0.05" 0 ","
                done
                FA_Panning_Drop "game:powder-sulfur" "0.2" 0
            ;;
            arsenic)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
            ;;
            uranium)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
            ;;
            manganese)
                FA_Panning_Drop "$MODID:powdered-ore-${F}-washed" "0.25" 0 ","
            ;;
            *)
                echo "ERROR MISSING ELEMENT FA_panning_file: $F"
            ;;
        esac
        echo "\t\t\t],"
        #f="$(cat ore-pan-drop.txt)"
        #f="$(echo ${f//INPUT/alchemical-refinement:powdered-ore-sand-${F}})"
        #f="$(echo ${f//OUTPUT/alchemical-refinement:powdered-ore-${F}-washed})"
        #powdered-ore-sand-${F}
        #f="$(echo ${f//Variable2/${material_element[$F]}})"
        #echo $f
    done
    cat ore-pan-end.txt
}
FA_Block_drop(){
    #{
	#			type: "item", 
	#			code: "nugget-nativesilver", 
	#			quantity: { avg: 1, var: 0 } 
	#		}
    echo "{ \"type\": \"item\", }"
}

#FA_block_oregraded > assets/alchemical-refinement/recipes/grid/ore-chunks.json

#FA_item_oregraded > assets/game/patches/item-ore-graded-crush.json
FA_Lang_File_powdered_ore > tmp/lang.json
#FA_combustion_prop > tmp/wash-combust.txt
#FA_combustion_prop > xcumbust.txt
#FA_combustion_prop > tmp/combust.txt
#FA_panning_file > tmp/ore-pan.json
#FA_Panning_Drop
#FA_panning_file "alchemical-refinement" > tmp/ore-pan-new.json



#FA_block_oregraded > block-ore-graded-crush.json
#printf "$(F_add_sqeer_bracket test)"
#printf "$(F_add_curly_bracket test)"
#printf "$(F_add_comment test)"
#printf "$(F_add_sqeer_bracket $(F_add_sqeer_bracket $(F_add_comment gay lord fucker)))"
#zcalc(){
    #bc <<< "$@"
    #local args="$@"
    #nu -c "= $args"
#}

#F_grade_array(){
    #material_grade[poor]=echo "${1} * 1.618" | bc #|awk '{print int($1+0.5)}')
#material_grade[medium]=echo $(( ${material_grade[poor]}*1.618 )) |awk '{print int($1+0.5)}'
#material_grade[rich]=echo $(( ${material_grade[medium]}*1.618 )) |awk '{print int($1+0.5)}'
#material_grade[bountiful]=echo $(( ${material_grade[rich]}*1.618 )) |awk '{print int($1+0.5)}'
#echo $(( 2*1.618 )) |awk '{print int($1+0.5)}'
#
#if [[ $2 == true ]]; then 
#    for f in bountiful rich medium poor; do
#        number=$(zcalc "$1 * 1.618" |awk '{print int($1+0.5)}')
#        echo $number
#        printf "\n${f}: $material_grade[${f}]"
#    done
#fi
#}
# F_grade_array 2 true

