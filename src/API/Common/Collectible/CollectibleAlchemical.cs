using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace AlchemicalRefinement.API.Common
{
    public abstract class CollectibleAlchemicalObject : Item  //: CollectibleObject
    {
        /// <summary>
        /// Information about the burnable states
        /// </summary>
        public CalcinationProperties CalcinationProps = null;

        /// <summary>
        /// Should return the burnable properties of the item/block
        /// </summary>
        /// <param name="world">May be null</param>
        /// <param name="itemstack">Set if its an itemstack for which to get properties</param>
        /// <param name="pos">May be null</param>
        /// <returns></returns>
        public virtual CalcinationProperties GetCalcinationProperties(IWorldAccessor world, ItemStack itemstack,
            BlockPos pos)
        {
            return itemstack?.ItemAttributes?["calcinationProps"].AsObject<CalcinationProperties>();
        }
        //TODO: maybe add GetHeldItemInfo see if that is needed.

        /// <summary>
        /// If the Highest Calcination Temperature is reached it will stopp.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="calcinationSlotsProvider"></param>
        /// <param name="inputSlot"></param>
        /// <returns></returns>
        public virtual float GetCalcinationMaxTemperature(IWorldAccessor world, ISlotProvider calcinationSlotsProvider,
            ItemSlot inputSlot)
        {
            CalcinationProperties calcinationProps = GetCalcinationProperties(world, inputSlot?.Itemstack, null);
            return calcinationProps == null ? 0 : calcinationProps.CalcinationMaxTemperature;
        }
        
        /// <summary>
        /// If the Calcination Duration is reached it will complete.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="calcinationSlotsProvider"></param>
        /// <param name="inputSlot"></param>
        /// <returns></returns>
        public virtual float GetCalcinationDuration(IWorldAccessor world, ISlotProvider calcinationSlotsProvider,
            ItemSlot inputSlot)
        {
            CalcinationProperties calcinationProps = GetCalcinationProperties(world, inputSlot?.Itemstack, null);
            return calcinationProps == null ? 0 : calcinationProps.CalcinationDuration;
        }
        
        /// <summary>
        /// If the Calcination Start Temperature Point is reached it will start.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="calcinationSlotsProvider"></param>
        /// <param name="inputSlot"></param>
        /// <returns></returns>
        public virtual float GetCalcinationPoint(IWorldAccessor world, ISlotProvider calcinationSlotsProvider,
            ItemSlot inputSlot)
        {
            CalcinationProperties calcinationProps = GetCalcinationProperties(world, inputSlot?.Itemstack, null);
            return calcinationProps == null ? 0 : calcinationProps.CalcinationPoint;
        }
        
        //TODO: ADD CanCalsinate and DoCalcination Look at CanSmelt And DoSmelt in Collectible.cs In API
    }
}