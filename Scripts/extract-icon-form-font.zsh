#!/usr/bin/env zsh
source common.zsh
font_dir=../tmp/icon/fonts
#ls $font_dir
svg_dir=../tmp/icon/SVG
#ls $svg_dir
svg_unicode=helpers/svg-unicode.txt
ttf_file=""
declare -A unicode_arr
var_ucode(){
    unicode_arr[1F70A]=vinegar-1
    unicode_arr[1F70B]=vinegar-2
    unicode_arr[1F70C]=vinegar-3
    unicode_arr[1F70D]=sulphur-1
    unicode_arr[1F70E]=philosophers-sulphur-1
    unicode_arr[1F70F]=black-sulphur-1
    unicode_arr[1F71A]=gold-1
    unicode_arr[1F71B]=silver-1
    unicode_arr[1F71C]=iron-ore-1
    unicode_arr[1F71D]=iron-ore-2
    unicode_arr[1F71E]=crocus-of-iron-1
    unicode_arr[1F71F]=regulus-of-iron-1
    unicode_arr[1F72A]=lead-ore-1
    unicode_arr[1F72B]=antimony-ore-1
    unicode_arr[1F72C]=sublimate-of-antimony-1
    unicode_arr[1F72D]=salt-of-antimony-1
    unicode_arr[1F72E]=sublimate-of-salt-of-antimony-1
    unicode_arr[1F72F]=vinegar-of-antimony-1
    unicode_arr[1F73A]=arsenic-1
    unicode_arr[1F73B]=realgar-1
    unicode_arr[1F73C]=realgar-2
    unicode_arr[1F73D]=aurpigment-1
    unicode_arr[1F73E]=bismuth-ore-1
    unicode_arr[1F73F]=tartar-1
    unicode_arr[1F74A]=wax-1
    unicode_arr[1F74B]=powder-1
    unicode_arr[1F74C]=calx-1
    unicode_arr[1F74D]=tutty-1
    unicode_arr[1F74E]=caput-mortuum-1
    unicode_arr[1F74F]=scepter-of-jove-1
    unicode_arr[1F75A]=powdered-brick-1
    unicode_arr[1F75B]=amalgam-1
    unicode_arr[1F75C]=stratum-super-stratum-1
    unicode_arr[1F75D]=stratum-super-stratum-2
    unicode_arr[1F75E]=sublimation-1
    unicode_arr[1F75F]=precipitate-1
    unicode_arr[1F76A]=alembic-1
    unicode_arr[1F76B]=bath-of-mary-1
    unicode_arr[1F76C]=bath-of-vapors-1
    unicode_arr[1F76D]=retort-1
    unicode_arr[1F76E]=hour-1
    unicode_arr[1F76F]=night
    unicode_arr[1F700]=quintessence-1
    unicode_arr[1F701]=air-1
    unicode_arr[1F702]=fire-1
    unicode_arr[1F703]=earth-1
    unicode_arr[1F704]=water-1
    unicode_arr[1F705]=aqua-fortis
    unicode_arr[1F706]=aqua-regia-1
    unicode_arr[1F707]=aqua-regia-2
    unicode_arr[1F708]=aqua-vitae-1
    unicode_arr[1F709]=aqua-vitae-2
    unicode_arr[1F710]=mercury-sublimate-1
    unicode_arr[1F711]=mercury-sublimate-2
    unicode_arr[1F712]=mercury-sublimate-3
    unicode_arr[1F713]=cinnabar-1
    unicode_arr[1F714]=salt-1
    unicode_arr[1F715]=nitre-1
    unicode_arr[1F716]=vitriol-1
    unicode_arr[1F717]=vitriol-2
    unicode_arr[1F718]=rock-salt-1
    unicode_arr[1F719]=rock-salt-2
    unicode_arr[1F720]=copper-ore-1
    unicode_arr[1F721]=iron-copper-ore-1
    unicode_arr[1F722]=sublimate-of-copper-1
    unicode_arr[1F723]=crocus-of-copper-1
    unicode_arr[1F724]=crocus-of-copper-2
    unicode_arr[1F725]=copper-antimoniate-1
    unicode_arr[1F726]=salt-of-copper-antimoniate-1
    unicode_arr[1F727]=sublimate-of-copper-1
    unicode_arr[1F728]=verdigris-1
    unicode_arr[1F729]=tin-ore-1
    unicode_arr[1F730]=regulus-of-antimony-1
    unicode_arr[1F731]=regulus-of-antimony-2
    unicode_arr[1F732]=regulus-1
    unicode_arr[1F733]=regulus-2
    unicode_arr[1F734]=regulus-3
    unicode_arr[1F735]=regulus-4
    unicode_arr[1F736]=alkali-1
    unicode_arr[1F737]=alkali-2
    unicode_arr[1F738]=marcasite-1
    unicode_arr[1F739]=sal-ammoniac-1
    unicode_arr[1F740]=tartar-2
    unicode_arr[1F741]=quicklime-1
    unicode_arr[1F742]=borax-1
    unicode_arr[1F743]=borax-2
    unicode_arr[1F744]=borax-3
    unicode_arr[1F745]=alum-1
    unicode_arr[1F746]=oil-1
    unicode_arr[1F747]=spirit-1
    unicode_arr[1F748]=tincture-1
    unicode_arr[1F749]=gum-1
    unicode_arr[1F750]=caduceus-1
    unicode_arr[1F751]=trident-1
    unicode_arr[1F752]=starred-trident-1
    unicode_arr[1F753]=lodestone-1
    unicode_arr[1F754]=soap-1
    unicode_arr[1F755]=urine-1
    unicode_arr[1F756]=horse-dung-1
    unicode_arr[1F757]=ashes-1
    unicode_arr[1F758]=pot-ashes
    unicode_arr[1F759]=brick-1
    unicode_arr[1F760]=distill-1
    unicode_arr[1F761]=dissolve-1
    unicode_arr[1F762]=dissolve-2
    unicode_arr[1F763]=purify-1
    unicode_arr[1F764]=putrefaction-1
    unicode_arr[1F765]=crucible-1
    unicode_arr[1F766]=crucible-2
    unicode_arr[1F767]=crucible-3
    unicode_arr[1F768]=crucible-4
    unicode_arr[1F769]=crucible-5
    unicode_arr[1F770]=day-night-1
    unicode_arr[1F771]=month-1
    unicode_arr[1F773]=half-dram-1
    unicode_arr[1F773]=half-ounce-1
}
select_loop(){
    local ref=../tmp/fonts.txt
    ls $font_dir > $ref
    for line ($(sed -n '=' ${ref})); do; echo "$line: $(sed -n "${line}p" ${ref})"; done
    echo -n "Enter a number: "
    read number
    ttf_file=$(sed -n "${number}p" ${ref})
    rm $ref
}
move_unicode(){
    for line ($(sed -n '=' ${svg_unicode})); do
        unicode=$(sed -n "${line}p" ${svg_unicode})
        #echo "$line: $unicode"
        unicode_file="$(ls /tmp/svg/${unicode}*.svg)"
        [ -f $unicode_file ] && mv $unicode_file $svg_dir_ttf/
        #if [ -f $unicode_file ]; then 
        #    mv $unicode_file $svg_dir_ttf/
        #fi
    done
}
icon_extract_main(){
    select_loop
    echo "## you took: $ttf_file"
    echo "extracting the Files from the ttf tile to svg vector file."
    ./Svg-extract.ff $font_dir/$ttf_file
    svg_dir_ttf=$svg_dir/$ttf_file
    mkdir -p $svg_dir_ttf
    echo "do you wish to move all files or only ones defined in $svg_unicode"
    echo "enter (y)es/(n)o:"
    read -q yesno
    case $yesno in
        y)
            mv /tmp/svg/* $svg_dir_ttf/
        ;;
        n)
            move_unicode
            rm -rf /tmp/svg/*
        ;;
    esac
}
rename_icons(){
    var_ucode
    local unicode_file=helpers/ucodes.txt; local dir=$svg_dir/$1
    local unicode; local unicode_svg
    local unicode_u; local unicode_u_svg
    mkdir -p $dir/{u,uni}
    for line ($(sed -n '=' ${unicode_file})); do 
        unicode=$(sed -n "${line}p" ${unicode_file})
        unicode_u="u$unicode"
        unicode_uni="uni$unicode"
        echo "${unicode_u}: $unicode_arr[$unicode]"
        unicode_u_svg="$(ls $dir/$unicode_u*)"
        #unicode_uni_svg="$(ls $dir/$unicode_uni*)"
        #echo $unicode_uni_svg
        [ -f $unicode_u_svg ] && cp $unicode_u_svg "$dir/u/$unicode_arr[$unicode].svg" #echo $unicode_u_svg
        #[ -f $unicode_uni_svg ] && cp $unicode_uni_svg "$dir/uni/$unicode_arr[$unicode].svg"
    done
}
# rename_icons ariel
# icon_extract_main
extract_rename_icons(){
    icon_extract_main
    rename_icons "$ttf_file"
}
usage(){
    echo "# 1) - extract_icons          --> to extract icons from Font"
    echo "# 2) - rename_icons           --> to rename icons extracted to the names refenanced in var_ucode function"
    echo "# 3) - extract_rename_icons   --> to chain both the above functions"
    echo "# 4) - exit                   --> to exit the script"

}
usage
select opt in extract_icons rename_icons extract_rename_icons exit; do
    case $opt in
        *icons)
            echo "mode: $opt"
            $opt
        ;;
        exit)
            break
        ;;
    esac
    usage
done