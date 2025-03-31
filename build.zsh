#!/usr/bin/env zsh
version=$(cat modinfo.json | jq '.Version')
VintagestoryDir="$HOME/.config/VintagestoryData/Mods"
Filename="alchemical-refinement-${version//\"/}.zip"
mkdir build
rm -f buiild/$Filename
zip -r build/$Filename assets modinfo.json modicon.png
#cp build/$Filename $VintagestoryDir/$Filename
