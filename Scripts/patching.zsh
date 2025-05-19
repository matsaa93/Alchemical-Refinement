
PATCH_OP_ADD(){
    echo "\t{ \"op\": \"add\", \"path\": \"$1\", \"value\": \"$2\", \"file\": \"game:$3.json\" },"
}

PATCH_OP_ADD_AV(){
    #echo "\t{ \"op\": \"add\", \"path\": \"/allowedVariants/-\", \"value\": \"$1\", \"file\": \"game:$2.json\" },"
    PATCH_OP_ADD "/allowedVariants/-" $1 $2
}

PATCH_allowedvariants_ore(){
    #{ "op": "add", "path": "/allowedVariants/-", "value": "looseores-azurite-dolostone-free", "file": "game:blocktypes/stone/looseores.json" },
    #{ "op": "add", "path": "/allowedVariants/-", "value": "ore-poor-azurite-dolostone", "file": "game:itemtypes/resource/ore-graded.json" },
    #{ "op": "add", "path": "/allowedVariants/-", "value": "ore-bountiful-azurite-limestone", "file": "game:blocktypes/stone/ore-graded.json" },
    #{ "op": "add", "path": "/allowedVariants/-", "value": "crystalizedore-poor-azurite-dolostone", "file": "game:itemtypes/resource/crystalizedore-graded.json" },
    PATCH_OP_ADD_AV ore-$1 itemtypes/resource/ore-graded >> /tmp/item-oregraded.txt
    PATCH_OP_ADD_AV ore-$1 blocktypes/stone/ore-graded >> /tmp/block-oregraded.txt
    PATCH_OP_ADD_AV crystalizedore-$1 itemtypes/resource/crystalizedore-graded >> /tmp/item-crystalloregraded.txt
}
FA_Create_oregen_patch(){
    #jq -r ".[].attributes.inblock.allowedVariants[]" assets/game/worldgen/deposits/metalore/azurite.json
    #jq -r ".[].attributes.placeblock.allowedVariantsByInBlock.\"rock-limestone\"[]" assets/game/worldgen/deposits/metalore/azurite.json
    local ore=$1
    local file=assets/game/worldgen/deposits/metalore/$ore.json
    for rock in $(jq -r ".[].attributes.inblock.allowedVariants[]" $file); do
        ##echo "rock: $rock"
        PATCH_OP_ADD_AV looseores-$ore-$rock-free blocktypes/stone/looseores >> /tmp/item-looseore.txt
        for abondace in $(jq -r ".[].attributes.placeblock.allowedVariantsByInBlock.\"rock-$rock\"[]" $file); do
            #print -r "ore-$abondace-$ore-$rock"
            PATCH_allowedvariants_ore $abondace-$ore-$rock
        done
    done
    cat /tmp/item-looseore.txt
    echo ""
    cat /tmp/item-oregraded.txt
    echo ""
    cat /tmp/item-crystalloregraded.txt
    echo ""
    cat /tmp/block-oregraded.txt
    rm -r /tmp/*txt
}

#FA_Create_oregen_patch azurite > tmp/azurite.json
#FA_Create_oregen_patch cerussite > tmp/cerussite.json
#FA_Create_oregen_patch smithsonite > tmp/smithsonite.json
