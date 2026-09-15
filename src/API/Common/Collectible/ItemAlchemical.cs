using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace AlchemicalRefinement.API.Common
{
    public class ItemAlchemical : CollectibleAlchemicalObject 
    {
        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            CombustibleProperties combustibleProps = GetCombustibleProperties(world, inSlot.Itemstack, null);
            CalcinationProperties calcinationProps = GetCalcinationProperties(world, inSlot.Itemstack, null);
            
            if (combustibleProps?.SmeltedStack == null && calcinationProps?.CalcinatedStack == null)
            {
                base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
                return;
            }
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
            if (combustibleProps?.SmeltedStack != null)
            {
                string smeltType = combustibleProps.SmeltingType.ToString().ToLowerInvariant();
                int inStackSize = combustibleProps.SmeltedRatio;
                if (combustibleProps.SmeltedStack.ResolvedItemstack != null)
                {
                    int outStackSize = combustibleProps.SmeltedStack.ResolvedItemstack.StackSize;
                    float units = outStackSize * 100f / inStackSize;

                    string metal = combustibleProps.SmeltedStack.ResolvedItemstack.Collectible?.Variant?["metal"];
                    string metalName = Lang.Get("material-" + metal);
                    if (metal == null) metalName = combustibleProps.SmeltedStack.ResolvedItemstack.GetName();

                    string str = Lang.Get("game:smeltdesc-" + smeltType + "ore-plural", units.ToString("0.#"), metalName);
                    dsc.AppendLine(str);
                }
            }

            if (calcinationProps?.CalcinatedStack != null)
            {
                // api.World.Logger.Warning("It is at least not null referance");
                string calcinationType = calcinationProps.CalcinationType.ToString().ToLowerInvariant();
                // api.World.Logger.Warning("Calcination type: {0}", calcinationType);
                int incalcsize = calcinationProps.CalcinationRatio;
                //int outcalsize = calcinationProps.CalcinatedStack.ResolvedItemstack;
                string oresname = calcinationProps.CalcinatedStack.Code.ToString();
                float orepoint = calcinationProps.CalcinationPoint;
                //float oreenergy = calcinationProps.CalcinationEnergy;
                string oretype = calcinationProps.CalcinationType.ToString().ToLowerInvariant();
                /*api.World.Logger.Warning("CalcinatedStack Code: {0}", oresname);
                api.World.Logger.Warning("CalcinationType: {0}", oretype);
                api.World.Logger.Warning("Calcination Energy: {0}", oreenergy);
                api.World.Logger.Warning("CalcinatedStack Ratio: {0}", incalcsize);
                api.World.Logger.Warning("Calcination Temperature: {0}", orepoint);*/
                if (calcinationProps.OxidizerStack != null)
                {
                    string oxidisername = calcinationProps.OxidizerStack.Code.ToString();
                    api.World.Logger.Warning("OxidizerStack Code: {0}", oxidisername);
                }

                if ( calcinationProps.CatalystStack != null)
                {
                    string catalystname = calcinationProps.CatalystStack.Code.ToString();
                    api.World.Logger.Warning("CatalystStack Code: {0}", catalystname);
                }
                
            }
            
        }
    }
}