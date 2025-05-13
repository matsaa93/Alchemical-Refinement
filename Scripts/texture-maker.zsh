image_helper=$working_dir/image-helpers
#item_mask=$working_dir/image-helpers/item-mask
#ore_mask=$working_dir/image-helpers/ore-mask
refFile=/tmp/files.txt
refMask=/tmp/mask.txt


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