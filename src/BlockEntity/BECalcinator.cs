using System;
using AlchemicalRefinement.API.Common;
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

    public class BECalcinator : BEFirepitContainer //BlockEntityOpenableContainer, IFirePit
    {
        //public override string InventoryClassName => "calcinator";
        private ICoreClientAPI capi;
        private ICoreServerAPI sapi;

        private GUICalcinator _clientDialog;
        float calcinationAccum;
        private float calcinatorTemp;
        private float calcinatoinProgress = 0;
        public string AttributeInfo;

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
            get { return _inventory[0].Empty; }
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
                return _inventory[1]?.Itemstack;
            }
        }
        public ItemStack OutputStack
        {
            get
            {
                return _inventory[4]?.Itemstack;
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

        public CalcinationProperties CalcinationProps
        {
            get
            {
                return InputStack?.ItemAttributes?["calcinationProps"].AsObject<CalcinationProperties>();
            }
        }

        public virtual CalcinationProperties GetCalcinationProperties(ItemStack stack)//ItemSlot input)
        { // calcinationPropsByType
            //return input.Itemstack?.ItemAttributes?["calcinationProps"].AsObject<CalcinationProperties>(); //itemstack?.ItemAttributes?["calcinationProps"].AsObject<CalcinationProperties>();
            var props = stack?.ItemAttributes?["calcinationProps"].Exists == true ? stack.ItemAttributes["calcinationProps"].AsObject<CalcinationProperties>(null, stack.Collectible.Code.Domain) : null;
            props?.CalcinatedStack?.Resolve(Api.World, "Calcinatable Properties CalcinatedStack", stack.Collectible.Code);
            return  props;
        }
        public string getAttributeInfo(int slot)
        {
            var props = GetCalcinationProperties(InputSlots[0].Itemstack);
            //var props = CalcinationProps;
            return props.CalcinatedStack?.Code.ToString();
        }

        public bool CanCalcinate()
        {
            CalcinationProperties calcinProps = GetCalcinationProperties(InputSlots[0].Itemstack);
            return calcinProps != null;
        }

        protected void CalcinateInput()
        {
            if (GetCalcinationProperties(InputSlots[0].Itemstack).CalcinatedStack != null)
            {
                CalcinationProperties  calcinProps = GetCalcinationProperties(InputSlots[0].Itemstack);
                Api.World.Logger.Warning("CalcinatedStack Is Real not a null referance at least");
                ItemStack calcinStack = calcinProps.CalcinatedStack.ResolvedItemStack.Clone();
                if (calcinStack != null && OutputSlots[0].Empty)
                {
                    OutputSlots[0].Itemstack = calcinStack;
                    InputSlots[0].TakeOut(1);
                    InputSlots[0].MarkDirty();
                }
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
                    _clientDialog.Update(BlockTemperature, FuelHours, calcinatoinProgress, AttributeInfo);
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
                //InputSlots[1].Itemstack;
                //var calsin = GetCalcinationProperties(null, InputSlots[1].Itemstack, null);
                if (CanCalcinate())
                {
                    //AttributeInfo = getAttributeInfo(1);
                    Api.World.Logger.Warning("GetCalcinationProperties Is Real not a null referance at least");
                    CalcinateInput();
                }
                //AttributeInfo = getAttributeInfo(1);
                //CalcinateInput();
                if (Api.Side == EnumAppSide.Server)
                {
                    //_clientDialog.Update(BlockTemperature, FuelHours, calcinatoinProgress);
                    MarkDirty(true);
                }
            }

        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            ITreeAttribute invtree = new TreeAttribute();
            this._inventory.ToTreeAttributes(invtree);
            tree["inventory"] = invtree;
            //AttributeInfo = tree.GetString("attributeInfo");
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessorForResolve)
        {
            base.FromTreeAttributes(tree, worldAccessorForResolve);
            _inventory.FromTreeAttributes(tree.GetTreeAttribute("inventory"));
            //tree.SetString("attributeInfo", AttributeInfo);
            if (Api?.Side == EnumAppSide.Client && _clientDialog != null)
            {
                _clientDialog.Update(BlockTemperature, FuelHours, calcinatoinProgress, AttributeInfo);
            }
        }
        
    }
}
