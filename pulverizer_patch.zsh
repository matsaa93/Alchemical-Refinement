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
    if [[ $debug == true ]]; then; printf "\nmaterial: ${1}"; printf "\n\ttier: $material_tier[${1}]"; printf "\n\tstacksize: $material_stacksizex[${1}]"; fi
    F_array_grade_set $1 $4 $5
}

F_array_test(){
    F_array_material_set cassiterite tin 2 0.3
    F_array_material_set chromite chromium 3 1.7
    F_array_material_set hematite ironbloom 3 1.8
    F_array_material_set limonite ironbloom 2 1
    F_array_material_set quartz_nativegold gold 2 0.3
    F_array_material_set magnetite ironbloom 3 1.5
    F_array_material_set malachite copper 2 1
    F_array_material_set nativecopper copper 2 1.5
    F_array_material_set pentlandite nickel 3 0.5
    F_array_material_set galena lead 2 1.7
    F_array_material_set quartz_nativesilver silver 2 0.6
    F_array_material_set sphalerite zinc 2 1.5
    F_array_material_set uranium uranium 3 1.5
    F_array_material_set rhodochrosite manganese 3 1.7
    F_array_material_set bismuthinite bismuth 2 1.8
    F_array_material_set stibnite zinc 2 1.5
}
F_array_test
F_array_test_Geology_Addition(){
    F_array_material_set azurite copper 2 1.5
    F_array_material_set chalcopyrite copper 2 1
    F_array_material_set cerussite lead 2 2
    F_array_material_set chalcocite copper 2 1
    F_array_material_set franckeite tin 2 1.2
    F_array_material_set freibergite silver 2 1
    F_array_material_set hemimorphite zinc 2 1.5
    F_array_material_set nativeplatinum platinum 3 1
    F_array_material_set smithsonite zinc 2 1.5
    F_array_material_set sperrylite platinum 3 1
    F_array_material_set teallite tin 2 1
    F_array_material_set tetrahedrite copper 2 1
    F_array_material_set vanadinite lead 2 2
    F_array_material_set wulfenite lead 2 1.5
    F_array_material_set pyrite ironbloom 2 1
}
F_array_test_Geology_Addition
F_array_test_OresAPlanty(){
    F_array_material_set nativegold gold 2 1
    F_array_material_set nativesilver silver 2 1.6
}
F_array_test_OresAPlanty
printf "$material ${#material[@]}"
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
    local ammount_of_item=${#material[@]}
    #echo > xcumbust.txt
    for F in $material
    do
        f="$(cat zcombustprop.txt)"
        f="$(echo ${f//Variable1/${F}})"
        f="$(echo ${f//Variable2/${material_element[$F]}})"
        echo $f
    done
}
#FA_block_oregraded > assets/alchemical-refinement/recipes/grid/ore-chunks.json

FA_item_oregraded > assets/game/patches/item-ore-graded-crush.json
#FA_combustion_prop > xcumbust.txt

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

