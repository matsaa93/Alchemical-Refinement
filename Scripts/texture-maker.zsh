image_helper=$working_dir/image-helpers
#item_mask=$working_dir/image-helpers/item-mask
#ore_mask=$working_dir/image-helpers/ore-mask
refFile=/tmp/files.txt
refMask=/tmp/mask.txt
nugget_mask=$image_helper/nugget-mask
powder_block_mask=$image_helper/powdered-block-mask
blackout_mask=$image_helper/blackout-mask
powder_mask=$image_helper/powder-mask
F_TEXTURE_CRUSHED_ORE_auto(){
    ls nugget > $refFile
    for line ($(sed -n '=' $refFile)); do
        file=$(sed -n "${line}p" $refFile)
    	file_noext="$(echo "${file}" | cut -b ${cutlength}-$(($(echo "${file}" | awk '{print length}') - 4 )))"
    	F_TEXTURE_CRUSHED_ORE $file_noext $1
    done
    rm $refFile
}

F_TEXTURE_GRAYSCALE(){
    magick $1 -colorspace Gray $2
}

F_TEXTURE_GENERATE_COLOR(){
    magick -size 32x32 xc:"rgb($1,$2,$3)" color-${1}-${2}-${3}.png
}

F_TEXTURE_GENERATE_ALL_COLORS(){
    for r in 1 16 32 48 64 80 96 112 128 144 160 176 192 208 224 240 255; do
        for g in 1 16 32 48 64 80 96 112 128 144 160 176 192 208 224 240 255; do
            for b in 1 16 32 48 64 80 96 112 128 144 160 176 192 208 224 240 255; do 
                magick -size 32x32 xc:"rgb($r,$g,$b)" colors/color_RGB-$r-$g-$b.png
            done
        done
    done 
}

F_TEXTURE_MASK(){
    local input=$1
    local mask=$2
    local output=$3
    magick $input $mask -alpha off -compose CopyOpacity -composite +compose $output 
}
F_TEXTURE_CRUSHED_ORE(){
	local ore=$1; local dir=$2; local mode=$3; local maskdir=$4
    mkdir -p $dir
	magick composite $InDir/${ore}.png $maskdir/borax.gif $maskdir/saltpeter.gif $dir/${ore}.png
	magick composite $InDir/${ore}.png $maskdir/saltpeter.gif $maskdir/borax.gif $dir/${ore}2.png
	#magick borax.gif nugget/${ore}.png -compose $mode -composite ${ore}.png
	#magick nugget/${ore}.png borax.gif -compose $mode -composite ${ore}2.png
	magick $dir/${ore}.png $dir/${ore}2.png -compose $mode -composite $dir/${ore}3.png
	magick $dir/${ore}2.png $dir/${ore}.png -compose $mode -composite $dir/${ore}4.png
	magick $dir/${ore}3.png $dir/${ore}4.png -compose Multiply -composite $dir/${ore}5.png
	magick $dir/${ore}.png $dir/${ore}3.png -compose $mode -composite $dir/${ore}6.png
	magick $dir/${ore}.png $dir/${ore}4.png -compose $mode -composite $dir/${ore}7.png
}
F_TEXTURE_ORE(){
	local ore=$1; local dir=$2; local mode=$3; local maskdir=$4
    mkdir -p $dir
	magick composite $InDir/${ore}.png $maskdir/platinum1.gif $maskdir/quartz1.gif $dir/${ore}.png
	magick composite $InDir/${ore}.png $maskdir/platinum2.gif $maskdir/quartz2.gif $dir/${ore}2.png
    magick composite $InDir/${ore}.png $maskdir/platinum3.gif $maskdir/quartz3.gif $dir/${ore}3.png
	#magick borax.gif nugget/${ore}.png -compose $mode -composite ${ore}.png
	#magick nugget/${ore}.png borax.gif -compose $mode -composite ${ore}2.png
	#magick $dir/${ore}.png $dir/${ore}2.png -compose $mode -composite $dir/${ore}3.png
	magick $dir/${ore}2.png $dir/${ore}.png -compose $mode -composite $dir/${ore}4.png
	magick $dir/${ore}3.png $dir/${ore}4.png -compose Multiply -composite $dir/${ore}5.png
	magick $dir/${ore}.png $dir/${ore}3.png -compose $mode -composite $dir/${ore}6.png
	magick $dir/${ore}.png $dir/${ore}4.png -compose $mode -composite $dir/${ore}7.png
    magick composite $InDir/${ore}.png $dir/${ore}.png $dir/${ore}4.png $dir/${ore}8.png
    magick composite $InDir/${ore}.png $dir/${ore}2.png $dir/${ore}5.png $dir/${ore}9.png
    magick composite $InDir/${ore}.png $dir/${ore}3.png $dir/${ore}6.png $dir/${ore}10.png
}



F_TEXTURE_MASK_COMPOSE_BLOCK(){
     local ore=$1; local dir=$2; local mode=$3
     mkdir -p $dir
     maskdir="$image_helper/block-mask"
     magick composite $InDir/${ore}.png $maskdir/quartz.gif $maskdir/saltpeter.gif $dir/${ore}.png
     magick composite $InDir/${ore}.png $maskdir/saltpeter.gif $maskdir/quartz.gif $dir/${ore}2.png
     #magick borax.gif nugget/${ore}.png -compose $2 -composite ${ore}.png
     #magick nugget/${ore}.png borax.gif -compose $2 -composite ${ore}2.png
     F_TEXTURE_MASK $InDir/${ore}.png $maskdir/quartz.png $dir/${ore}3.png
     F_TEXTURE_MASK $InDir/${ore}.png $maskdir/clearquartz.png $dir/${ore}4.png
     F_TEXTURE_MASK $InDir/${ore}.png $maskdir/fractal_gray.png $dir/${ore}5.png
     F_TEXTURE_MASK $InDir/${ore}.png $maskdir/fluorite_gray.png $dir/${ore}6.png
     F_TEXTURE_MASK $InDir/${ore}.png $maskdir/fractald.png $dir/${ore}7.png
     #magick $dir/${ore}.png $dir/${ore}2.png -compose $mode -composite $dir/${ore}3.png
     #magick $dir/${ore}2.png $dir/${ore}.png -compose $mode -composite $dir/${ore}4.png
     magick $dir/${ore}3.png $dir/${ore}4.png -compose Multiply -composite $dir/${ore}8.png
     magick $dir/${ore}6.png $dir/${ore}5.png -compose Multiply -composite $dir/${ore}11.png
     magick $dir/${ore}.png $dir/${ore}3.png -compose $mode -composite $dir/${ore}9.png
     magick $dir/${ore}.png $dir/${ore}4.png -compose $mode -composite $dir/${ore}10.png
}

F_MASK_TEXTURE_LOOP(){
    maskdir=$1
    echo $maskdir
    ls $maskdir > $refMask
    for lineb ($(sed -n '=' $refMask)); do
	    mask=$(sed -n "${lineb}p" $refMask)
	    echo "$file_noext-$lineb.png  line $lineb"
	    F_TEXTURE_MASK $InDir/$file $maskdir/$mask $OutDir/$file_noext-$lineb.png
    done
}

F_MASK_TEXTURE_DIR(){
    file_noext="$(echo "${file}" | cut -b ${cutlength}-$(($(echo "${file}" | awk '{print length}') - 4 )))"
    for d in "block-mask" "item-mask" "ore-mask"; do
        OutDir="$working_dir/tmp/textures/$d"
        mkdir -p $OutDir
        F_MASK_TEXTURE_LOOP "$image_helper/$d"
        if [ $d != "ore-mask" ]; then
            F_TEXTURE_CRUSHED_ORE "$file_noext" "$working_dir/tmp/textures/$d/Hcompose" Hardlight "$image_helper/$d"
            F_TEXTURE_CRUSHED_ORE "$file_noext" "$working_dir/tmp/textures/$d/Scompose" Softlight "$image_helper/$d"
            echo "not ore-mask"
        else
            F_TEXTURE_ORE "$file_noext" "$working_dir/tmp/textures/$d/Hcompose" Hardlight "$image_helper/$d"
            F_TEXTURE_ORE "$file_noext" "$working_dir/tmp/textures/$d/Scompose" Softlight "$image_helper/$d"
            echo "ore-mask"
        fi
    done
    #F_TEXTURE_MASK_COMPOSE_BLOCK "$file_noext" "$working_dir/tmp/textures/block-mask/compose" Hardlight
}

F_MASK_TEXTURE_FILES_ORE(){
    InDir=assets/game/textures/item/resource/nugget
    echo $InDir
	ls $InDir > $refFile
	for line ($(sed -n '=' $refFile)); do
		file=$(sed -n "${line}p" $refFile)
        F_MASK_TEXTURE_DIR
	done
    rm $refMask; rm $refFile
}

F_TEXTURE_GENERATE_FILES(){
    ls nugget > $refFile
    for line ($(sed -n '=' $refFile)); do
          file=$(sed -n "${line}p" $refFile)
          file_noext="$(echo "${file}" | cut -b ${cutlength}-$(($(echo "${file}" | awk '{print length}') - 4 )))"
          F_TEXTURE_MASK_COMPOSE $file_noext $1 $2
          F_TEXTURE_MASK nugget/$file borax.gif powder/$file
      done
      #ls new > $refFile
 #      for line ($(sed -n '=' $refFile)); do
 #     	file=$(sed -n "${line}p" $refFile)
  #      file_noext="$(echo "${file}" | cut -b ${cutlength}-$(($(echo "${file}" | awk '{print length}') - 4 )))"
   #   	F_TEXTURE_MASK $1/$file borax1.gif powder/$file_noext-borax.png
    #  	F_TEXTURE_MASK $1/$file saltpeter1.gif powder/$file_noext-saltpeter.png 
     # 	F_TEXTURE_MASK $1/$file quartz1.gif powder/$file_noext-quatz.png
      #done
      rm $refFile
}
#  textures/block/stone/{
#  cobbleskull/chalk* 
#  cobblestone/chalk*
#  polishedrock-old/full/chalk* 
#  polishedrock-old/cobbled/chalk*
#  agedbrick/chalk*
#  crackedbrick/chalk*
#  drystone/chalk*
#  rock/chalk*
#  termitemound/chalk*
#  cobblestonestairs/chalk*
#  polishedrock/chalk*
#  polishedrock-old/inside/chalk*
#  polishedrockslab/chalk*
#  brick/chalk*
#  drystonefence/chalk*
#  gravel/chalk*
#  sand/chalk*
#  }
F_TEXTURE_MASK_MAKE(){
    rock_mask=$image_helper/rock-mask
    local stone_textures=$VS_texture_dir/block/stone
    cd $stone_textures
    for DIR in cobblestone cobbleskull "polishedrock-old/full" "polishedrock-old/cobbled" agedbrick crackedbrick termitemound rock sand gravel drystone drystonefence brick polishedrockslab "polishedrock-old/inside" polishedrock cobblestonestairs; do
        #ls $stone_textures/$DIR/chalk*
        for File in $(ls $DIR/chalk*); do
            echo "Input File: $File Output File: ${File//\//_}"
            Fileb=${File//\//_}
            F_TEXTURE_GRAYSCALE $File "$rock_mask/${Fileb//png/gif}"
        done
    done
    popd
}
#F_TEXTURE_MASK_MAKE
F_TEXTURE_ADD_COLOR(){
    # use alpha: copy or deactivate
    # use blend: seamless-blend or saliency-blend
    local mask=$1; local RGB=$2; local alphamode=$3; local blendmode=$4; local output=$5
    magick -size 32x32 xc:"rgb($RGB)" $mask -alpha $alphamode -compose ${blendmode}-blend -composite $output
}
declare -A colorRGB
declare -A lineWidt
F_TEXTURE_ADD_MULTI_COLOR(){
    local mask=$1; local output=$2
    magick -size 32x32 xc:"rgb(${colorRGB[main]})" \
        -stroke xc:"rgb(${colorRGB[A]})" -strokewidth $lineWidt[A] -draw 'line 2,22 29,22' \
        -stroke xc:"rgb(${colorRGB[B]})" -strokewidth $lineWidt[B] -draw 'line 2,22 29,22' \
        -channel RGBA -${blur}-blur $blurA -swirl $swirl \
        $mask -alpha deactivate -compose saliency-blend -composite $output
}
# -rotational-blur 180
# -swirl 180
# -motion-blur 0x12+45 -rotational-blur
# magick -size 32x32 xc:"rgb(235,220,220)" tmp/crystal/saltpeter.gif -alpha copy -compose seamless-blend -composite tmp/test3.png
# magick -size 32x32 xc:"rgb(255,240,240)" tmp/crystal/saltpeter.gif -alpha deactivate -compose seamless-blend -composite tmp/test2.png
# magick -size 32x32 xc:"rgb(235,220,220)" tmp/crystal/saltpeter.gif -alpha deactivate -compose seamless-blend -composite tmp/test2.png
# magick -size 32x32 xc:"rgb(205,190,190)" tmp/crystal/saltpeter.gif -alpha deactivate -compose seamless-blend -composite tmp/test2.png
F_TEXTURE_ROCK(){
    rock_mask=$image_helper/rock-mask
    local stone_textures=$game_textures/block/stone
    local COLOR=$2
    cd $rock_mask
    for m in $(ls); do
        File=${m//_/\/}
        File=${File//chalk/$1}
        File=$stone_textures/${File//gif/png}
        echo $File
        F_TEXTURE_ADD_COLOR $m $COLOR deactivate seamless $File
    done
}

V_set_number_input(){
    echo -n "Enter $1 number:"
    read number
}
V_Select_blur_modes(){
    #blur_swirl_ARG=""
    echo "what mode do you wish to use?"
    select opt in rotational-blur motion-blur ; do
        case $opt in
            rotational-blur) V_set_number_input "angle (0-360)"; blur=rotational; blurA="$number"; break;;
            motion-blur) V_set_number_input "angle (0-360)"; blur=motion; blurA="0x12+$number"; break;;
            *) echo "not an option";;
        esac
    done
    echo "do you wish to add swirl to the filter?"
    echo "enter (y)es/(n)o:"
    select yesno in yes no; do 
        case $yesno in
            yes) V_set_number_input "angle 0-360"; swirl=$number; break;;
            no) echo "no swirl then"; swirl=0; break;;
            *) echo "not an option";;
        esac
    done 
}
V_set_line_width(){
    for line in A B; do
        V_set_number_input "line width for line $line (1-20)"
        lineWidt[$line]=$number
    done
}

F_MASK_INTERPOLATE_COLORS(){
    local mode=$3
    local mask=$1; local Output=$2
    magick -size 1x1 xc:"rgb(${colorRGB[main]})" xc:"rgb(${colorRGB[A]})" xc:"rgb(${colorRGB[B]})" +append /tmp/duotone_clut.gif
    
    magick -size 20x256 gradient: -rotate 90   /tmp/duotone_clut.gif -interpolate $mode -clut /tmp/duotone_gradient.gif
    #magick $mask   duotone_clut.gif -interpolate $mode -clut ${Output}gradient.png
    magick $mask   /tmp/duotone_clut.gif -clut ${Output}-clut.png
}

F_MASK_blackout(){
    local input=$1; local mask=$2; local output=$3
    magick $input \( $mask -negate \) -alpha off -compose copy_opacity -composite $output
}

F_TEXTURE_NUGGET(){
    # -rotational-blur 180
    # -swirl 180
    # -motion-blur 0x12+45
    #blur_swirl_ARG="-rotational-blur 180 -swirl 180"
    #V_Select_blur_modes
    #V_set_line_width
    blur=rotational;blurA=180; swirl=90
    lineWidt[A]=15; lineWidt[B]=9;
    echo "blur mode: $blur_swirl_ARG"
    echo "Line width A: $lineWidt[A]"
    echo "Line width B: $lineWidt[B]"
    DIR=tmp/nugget
    mkdir -p $DIR/block
    colorRGB[main]=$1; colorRGB[A]=$2; colorRGB[B]=$3 
    for m in $(ls $nugget_mask); do
        outputbase=$4-${m//.gif/}
        echo "F_TEXTURE_ADD_MULTI_COLOR $nugget_mask/$m $DIR/$4-${m//.gif/}.png"
        F_TEXTURE_ADD_MULTI_COLOR $nugget_mask/$m $DIR/$outputbase.png
        F_MASK_INTERPOLATE_COLORS $nugget_mask/$m $DIR/$outputbase mesh
        magick $DIR/$outputbase.png $DIR/$outputbase-clut.png -compose Hardlight -composite $DIR/$outputbase-compined.png
    done
    for m in $(ls $powder_block_mask); do
        outputbase=$4-${m//.gif/}
        F_TEXTURE_ADD_MULTI_COLOR $powder_block_mask/$m $DIR/block/$outputbase.png
        F_MASK_INTERPOLATE_COLORS $powder_block_mask/$m $DIR/block/$outputbase mesh
        magick $DIR/block/$outputbase.png $DIR/block/$outputbase-clut.png -compose Hardlight -composite $DIR/block/$outputbase-compined.png
    done
    local tPowder=/tmp/powder
    mkdir $tPowder
    for m in $(ls $powder_mask); do
        outputbase=$4-${m//.gif/}
        F_TEXTURE_ADD_MULTI_COLOR $powder_mask/$m $tPowder/$outputbase.png
        F_MASK_INTERPOLATE_COLORS $powder_mask/$m $tPowder/$outputbase mesh
        magick $tPowder/$outputbase.png $tPowder/$outputbase-clut.png -compose Hardlight -composite $tPowder/$outputbase-combined.png
        # $DIR/powder/$outputbase.png
        F_MASK_blackout $tPowder/$outputbase.png $blackout_mask/powder-mask.gif $DIR/powder/$outputbase.png
        F_MASK_blackout $tPowder/$outputbase-clut.png $blackout_mask/powder-mask.gif $DIR/powder/$outputbase-clut.png
        F_MASK_blackout $tPowder/$outputbase-combined.png $blackout_mask/powder-mask.gif $DIR/powder/$outputbase-combined.png
    done
}



F_MASK_blackout_ore(){
    InDir=assets/game/textures/item/resource/nugget
    local file; local filemask
    echo $InDir
	ls $InDir > $refFile
    ls $blackout_mask/ore* > $refMask
	for line ($(sed -n '=' $refFile)); do
		file=$(sed -n "${line}p" $refFile)
        file_noext="$(echo "${file}" | cut -b ${cutlength}-$(($(echo "${file}" | awk '{print length}') - 4 )))"
        for lineb ($(sed -n '=' $refMask)); do
            filemask=$(sed -n "${lineb}p" $refMask)
            echo "File: $file"
            echo "Filemask: $filemask"
            F_MASK_blackout $InDir/$file "$filemask" "tmp/textures/ore-mask/$file_noext-$lineb.png"
        done
        F_MASK_blackout $InDir/$file "$blackout_mask/powder-mask.gif" "tmp/textures/powder-mask/$file_noext.png"
	done
    rm $refMask; rm $refFile
}
#F_MASK_blackout assets/game/textures/item/resource/nugget/azurite.png $blackout_mask/powder-mask.gif tmp/test.png

#F_MASK_blackout_ore
#color_select=$(zenity --color-selection --show-palette --title Color\ Select)
#F_MASK_TEXTURE_FILES_ORE
#F_TEXTURE_NUGGET "10,44,143" "40,83,191" "70,136,99" azurite
#F_TEXTURE_NUGGET "4,11,83" "40,83,191" "70,136,99" azurite
#F_TEXTURE_NUGGET "0,56,168" "0,127,255" "115,169,194)" azurite
#F_TEXTURE_NUGGET "155,221,255" "166,231,255" "102,205,170" copper
#F_TEXTURE_NUGGET "5,17,47" "26,95,180" "5,143,126" copper
# F_TEXTURE_NUGGET "56,28,15" "199,153,129" "174,138,112" cerussite
#F_TEXTURE_NUGGET "0,95,114" "90,136,162" "42,139,155" smithsonite
#F_TEXTURE_NUGGET "37,42,22" "42,139,155" "90,136,162" smithsonite

F_MASK_TEXTURE_FILES_ORE
F_MASK_blackout_ore
# F_TEXTURE_ROCK dolomite "205,185,185"
# F_TEXTURE_ROCK calcite "205,205,165"
#F_TEXTURE_ROCK aragonite "193,154,107"

# magick -size 32x32 xc:"rgb(193,154,107)" \
#           -stroke xc:"rgb(193,154,0)"    -strokewidth 15 -draw 'line 5,50 65,50' \
#           -stroke xc:"rgb(193,0,107)" -strokewidth  9 -draw 'line 5,50 65,50' \
#           -channel RGBA  -motion-blur 0x12+45 -swirl 90 radial_swirl.png
# i=1
# for d in $(ls old); do                                                    ─╯
# F_TEXTURE_GRAYSCALE old/$d ${i}.png
# i=$(($i + 1))
# done
