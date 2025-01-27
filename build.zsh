#!/usr/bin/env zsh
version=$(cat modinfo.json | jq '.Version')
VintagestoryDir="$HOME/.config/VintagestoryData/Mods"
Filename="alchemical-refinement-${version//\"/}.zip"

zip -r build/$Filename assets modinfo.json
cp build/$Filename $VintagestoryDir/$Filename