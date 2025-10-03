using System;
using AlchemicalRefinement.GUI;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using AlchemicalRefinement.Inventory;
using Vintagestory.API.Config;

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

        private GUICalcinator _clientDialog;
        float calcinationAccum;
        private float calcinatorTemp;
        private float calcinatoinProgress = 0;

        #region Invetory
        private InventoryCalcinator _inventory;
        public override string InventoryClassName { get { return "InventoryCalcinator"; } }
        /// <summary>
        /// SlotID: 0 = fuel, 1-3 = input, 4-5 = output
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
                    _inventory[2],
                    _inventory[3]
                };
            }
        }
        
        public ItemSlot[] OutputSlots
        {
            get
            {
                return new ItemSlot[]
                {
                    _inventory[4],
                    _inventory[5]
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
                return _inventory[1].Empty && _inventory[2].Empty && _inventory[3].Empty;
            }
        }

        public bool FuelSlotsEmpty
        {
            get { return _inventory[1].Empty; }
        }

        
        
        public BECalcinator()
        {
            _inventory = new InventoryCalcinator(null, null);
            _inventory.SlotModified += OnSlotModifid;
        }

        private void OnSlotModifid(int slotid)
        {
            if (slotid >= 0 || slotid < 5) // input or fuel slot update
            {
                //FindMatchingRecipe();
                MarkDirty(true);
                if (_clientDialog != null)
                {
                    //_clientDialog.Update(RecipeProgress, CurrentTemp, CurrentRecipe);
                }
            }
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

        public string DialogTitle => Lang.Get("alchemref:gui-title-calcinator");

        #region Blockinteractions

        public override void OnBlockRemoved()
        {
            base.OnBlockRemoved();
            if (_clientDialog != null)
            {
                _clientDialog.TryClose();
                GUICalcinator gUIlog = _clientDialog;
                if (gUIlog != null) { gUIlog.Dispose(); }
                _clientDialog = null;
            }
        }

        public override bool OnPlayerRightClick(IPlayer byPlayer, BlockSelection blockSel)
        {
            if (Api.Side == EnumAppSide.Client)
            {
                toggleInventoryDialogClient(byPlayer, () => {
                    _clientDialog = new GUICalcinator(DialogTitle, Inventory, Pos, Api as ICoreClientAPI);
                    _clientDialog.Update(BlockTemperature, FuelHours, calcinatoinProgress);
                    return _clientDialog;
                });
            }
            
            return true;
        }
        #endregion
        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            RegisterGameTickListener(OnCraftTick, 500);

            _inventory.Pos = this.Pos;
            _inventory.LateInitialize($"{InventoryClassName}-{this.Pos.X}/{this.Pos.Y}/{this.Pos.Z}", api);
        }

        public void OnCraftTick(float dt)
        {
            if (IsBurning || IsSmoldering)
            {
                if (Api.Side == EnumAppSide.Client)
                {
                    _clientDialog.Update(BlockTemperature, FuelHours, calcinatoinProgress);
                    //MarkDirty();
                }
            }

        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            ITreeAttribute invtree = new TreeAttribute();
            this._inventory.ToTreeAttributes(invtree);
            tree["inventory"] = invtree;
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessorForResolve)
        {
            base.FromTreeAttributes(tree, worldAccessorForResolve);
            _inventory.FromTreeAttributes(tree.GetTreeAttribute("inventory"));
            if (Api?.Side == EnumAppSide.Client && _clientDialog != null)
            {
                _clientDialog.Update(BlockTemperature, FuelHours, calcinatoinProgress);
            }
        }
        
    }
}
