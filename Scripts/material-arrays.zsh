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
    F_array_material_set stibnite antimony 2 1.5 615
    F_array_material_set uranium uranium 3 1.5 1132
    F_array_material_set rhodochrosite manganese 3 1.7 1246
}
F_array_test_OresAPlanty
[ $debug = true ] && printf "$material ${#material[@]}\n"