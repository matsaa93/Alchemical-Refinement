#!/usr/bin/env zsh
debug=false
working_dir="$HOME/Documents/GitHub/Alchemical-Refinement"
source Scripts/common.zsh
source Scripts/material-arrays.zsh
source Scripts/panning_drop.zsh
source Scripts/metal_ores.zsh
source Scripts/localization_variables.zsh
source Scripts/localization.zsh


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

FA_Block_drop(){
    #{
	#			type: "item", 
	#			code: "nugget-nativesilver", 
	#			quantity: { avg: 1, var: 0 } 
	#		}
    echo "{ \"type\": \"item\", },"
}

FA_Variants(){
    #fileVar="$fileVar\n\t\t{\n\t\t\"Code\": \"$1\"\n\t\t},"
    fileVar="$fileVar { \"code\": \"$1\" }$2"
}

FA_Variants_file(){
    fileVar=""
    declare -a arg
    arg=($@)
    local ammount_of_item=${#arg[@]}
    local lastitem="$arg[$ammount_of_item]"
    [ $debug = true ] && echo "$ammount_of_item $lastitem"
    for F in $@
    do
        if [ $F = $lastitem ]; then
            FA_Variants $F
        else
            FA_Variants $F ","
        fi
    done
    #fileVar="{\n\t\"Code\": \"$vargruop\",\n\t\"variants\": [$fileVar\n\t]\n}"
    fileVar="{ \"code\": \"$vargruop\", \"variants\": [$fileVar ] }"
    echo $fileVar | jq "."
}
#vargruop="vitriol"
#FA_Variants_file "red" "green" "blue" "white" "sweet" "glauber" "argentum" "dutch_white" "gypsum" "turpeth" "epsomite" "alunogen" "celestine" "angelesite"

# FA_combustion_prop_file


#FA_block_oregraded > assets/alchemref/recipes/grid/ore-chunks.json

#FA_item_oregraded > assets/game/patches/item-ore-graded-crush.json
#FA_Lang_File_powdered_ore $modid > tmp/lang.json
#FA_combustion_prop > tmp/wash-combust.txt
#FA_combustion_prop > xcumbust.txt
#FA_combustion_prop > tmp/combust.txt
#FA_panning_file > tmp/ore-pan.json
#FA_Panning_Drop
#FA_panning_file "alchemref" > tmp/ore-pan-new.json
#FA_combustion_prop
#FA_panning_file $modid > tmp/test.json #| jq "."
#FA_panning_file $modid > tmp/test.json

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

