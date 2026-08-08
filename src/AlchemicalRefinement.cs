using System;
using Vintagestory.API.Common;
using Vintagestory.Common;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using Vintagestory.API.Config;


[assembly: ModInfo("AlchemicalRefinement",
    Authors = new string[] { "Matsaa93", "UnknownFutureGuy" },
    Description = "Extension to, Alchemy, Refining, and mining.",
    Version = "0.4.1")]
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
            //capi.ShowChatMessage("gayyy");
            //capi.Logger.StoryEvent("it is alchemical-refinement time story-event");
            //capi.Logger.Event("it is alchemical-refinement time event");

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
