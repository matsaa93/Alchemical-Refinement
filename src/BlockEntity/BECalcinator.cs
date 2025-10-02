using System;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using AlchemicalRefinement.Inventory;

namespace AlchemicalRefinement
{
    public class CalcinatableProps
    {
        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>0</jsondefault>-->
        /// If set, this is the resulting itemstack once the CalcinationPoint has been reached for the supplied duration.
        /// </summary>
        public JsonItemStack CalcinatedStack;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// If there is a melting point, the max temperature it can reach. A value of 0 implies no limit.
        /// </summary>
        public float MaxTemperature;

        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>0</jsondefault>-->
        /// How many degrees celsius it takes to Calcinate/transform this collectible into another. Required if <see cref="CalcinatedStack"/> is set.
        /// </summary>
        public float CalcinationPoint;

        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>0</jsondefault>-->
        /// For how many seconds the temperature has to be above the melting point until the item is smelted. Recommended if <see cref="CalcinatedStack"/> is set.
        /// </summary>
        public float CalcinationDuration;
    }

    public class BECalcinator : BEFirepitContainer //BlockEntityOpenableContainer, IFirePit
    {
        //public override string InventoryClassName => "calcinator";
        private ICoreClientAPI capi;
        private ICoreServerAPI sapi;
        float calcinationAccum;
        private float calcinatorTemp;

        #region Invetory
        private InventoryCalcinator _inventory;
        public override string InventoryClassName { get { return "InventoryCalcinator"; } }
        /// <summary>
        /// SlotID: 1-2 = input, 0 = fuel, 3-4 = output
        /// </summary>
        public override InventoryBase Inventory { get { return _inventory; } }
        public ItemSlot[] FuelSlots
        {
            get
            {
                return new ItemSlot[]
                {
                    _inventory[0]
                };
            }
        }
        public ItemSlot[] InputSlots
        {
            get
            {
                return new ItemSlot[]
                {
                    _inventory[1],
                    _inventory[2]
                };
            }
        }
        /// <summary>
        /// True if all input slots are empty
        /// </summary>
        public bool InputsEmpty
        {
            get
            {
                return _inventory[1].Empty && _inventory[2].Empty;
            }
        }

        public ItemSlot[] OutputSlots
        {
            get
            {
                return new ItemSlot[]
                {
                    _inventory[3],
                    _inventory[4]
                };
            }
        }
        
        public BECalcinator()
        {
            _inventory = new InventoryCalcinator(null, null);
            //_inventory.SlotModified += OnSlotModifid;
        }

        public ItemStack InputStack
        {
            get
            {
                return _inventory[0]?.Itemstack;
            }
        }
        public ItemStack OutputStack
        {
            get
            {
                return _inventory[1]?.Itemstack;
            }
        }
        #endregion
        
        public virtual float SoundLevel
        {
            get { return 0.66f; }
        }

       
        

        public void heatItem(float dt)
        {
            if (_inventory[1].Empty || _inventory[2].Empty) return;

            if (InputStackTemp < 100)
            {
                InputStackTemp += dt * 2;
            }
        }
/*        
        public override heatBlock(float dt)
        {
            //_inventory[1].StackSize
        }
*/
        /// <summary>
        /// Set and get for Calcinator Block Temperature 
        /// </summary>
        public float CalcinatorTemp
        {
            get => calcinatorTemp;
            set => calcinatorTemp = value;
        }
        
        public float InputStackTemp
        {
            get
            {
                return InputStack?.Collectible.GetTemperature(Api.World, _inventory[0].Itemstack) ?? 0;
            }
            set
            {
                InputStack.Collectible.SetTemperature(Api.World, _inventory[0].Itemstack, value);
            }
        }

        public CalcinatableProps CalcinProps
        {
            get
            {
                return InputStack?.ItemAttributes?["calcinationProps"].AsObject<CalcinatableProps>();
            }
        }

        public override bool OnPlayerRightClick(IPlayer byPlayer, BlockSelection blockSel)
        {
            return true;
        }
    }
}
