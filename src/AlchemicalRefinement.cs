using System;
using Vintagestory.API.Common;
using Vintagestory.Common;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace AlchemicalRefinement
{
    public class AlchemicalRefinementMod : ModSystem
    {
        ICoreClientAPI capi;
        ICoreServerAPI sapi;
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            if (api.Side == EnumAppSide.Client)
            {
                capi = api as ICoreClientAPI;
            }
            else
            {
                sapi = api as ICoreServerAPI;
            }

            RegisterBlocks(api);
            RegisterBlockEntities(api);

        }
        public void RegisterBlocks(ICoreAPI api)
        {
            api.RegisterBlockClass("ARCalcinatorBlock", typeof(BlockCalcinator));
        }
        public void RegisterBlockEntities(ICoreAPI api)
        {
            api.RegisterBlockEntityClass("ARBECalcinator", typeof(BECalcinator));
        }
    }
     
}
