using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlchemicalRefinement.API.Common;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

#nullable disable

namespace AlchemicalRefinement.Inventory
{
    /// <summary>
    /// Inventory with Two Input Slots and Two Output Slots and A Fuel Slot
    /// </summary>
    public class InventoryCalcinator : InventoryBase, ISlotProvider
    {
        ICoreClientAPI capi;
        ICoreServerAPI sapi;
        
        private ItemSlot[] _slots;
        public ItemSlot[] Slots { get { return _slots; } }
        ItemSlot[] calcinationSlots;
        ItemSlot[] calcinatedOutSlots;
        public override int Count => _slots.Length;
        
        public ItemSlot[] CalcinationSlots { get { return calcinationSlots ?? Array.Empty<ItemSlot>(); } }

        public ItemSlot[] CalcinatedOutSlots  { get { return calcinatedOutSlots ?? Array.Empty<ItemSlot>(); } }

        /// <summary>
        /// SlotID: FuelStack = 0 input = 1-3 , output = 4-5
        /// </summary>
        public InventoryCalcinator(string inventoryID, ICoreAPI api) : base(inventoryID, api)
        {
            
            _slots = GenEmptySlots(6);
            calcinationSlots = new []{ _slots[1], _slots[2], _slots[3] };
            calcinatedOutSlots = new []{ _slots[4], _slots[5] };
        }

        public InventoryCalcinator(string className, string instanceID, ICoreAPI api) : base(className, instanceID, api)
        {
            _slots = GenEmptySlots(6);
            calcinationSlots = new []{ _slots[1], _slots[2], _slots[3] };
            calcinatedOutSlots = new []{ _slots[4], _slots[5] };
        }
        
        public override void LateInitialize(string inventoryID, ICoreAPI api)
        {
            base.LateInitialize(inventoryID, api);
            if (api.Side == EnumAppSide.Server)
            {
                sapi = api as ICoreServerAPI;
            }
            else
            {
                capi = api as ICoreClientAPI;
            }

        }
        
        public override ItemSlot this[int slotId]
        {
            get
            {
                if (slotId < 0 || slotId >= Count) return null;
                return _slots[slotId];
            }
            set
            {
                if (slotId < 0 || slotId >= Count) throw new ArgumentOutOfRangeException(nameof(slotId));
                if (value == null) throw new ArgumentNullException(nameof(value));
                _slots[slotId] = value;
            }
        }

        public override void DidModifyItemSlot(ItemSlot slot, ItemStack extractedStack = null)
        {
            base.DidModifyItemSlot(slot, extractedStack);
        }
        public override void OnItemSlotModified(ItemSlot slot)
        {
            base.OnItemSlotModified(slot);
        }

        public override void FromTreeAttributes(ITreeAttribute tree)
        {
            List<ItemSlot> modifiedSlots = new List<ItemSlot>();
            _slots = SlotsFromTreeAttributes(tree, _slots,  modifiedSlots);
            for (int i = 0; i < modifiedSlots.Count; i++) DidModifyItemSlot(modifiedSlots[i]);
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            SlotsToTreeAttributes(_slots, tree);
        }

        protected override ItemSlot NewSlot(int i)
        {
            return new ItemSlotSurvival(this);
        }

        public override float GetSuitability(ItemSlot sourceSlot, ItemSlot targetSlot, bool isMerge)
        {
            CalcinationProperties props = sourceSlot.Itemstack?.Collectible.Attributes?["calcinationProps"].AsObject<CalcinationProperties>();
            if (targetSlot == _slots[0] && props != null) return 4f;

            return base.GetSuitability(sourceSlot, targetSlot, isMerge);
        }

        public override ItemSlot GetAutoPushIntoSlot(BlockFacing atBlockFace, ItemSlot fromSlot)
        {
            return _slots[1];
        }

    }
}
