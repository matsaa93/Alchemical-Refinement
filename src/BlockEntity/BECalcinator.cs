using System;
using AlchemicalRefinement.API.Common;
using AlchemicalRefinement.API.Common.BlockEntity;
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

    public class BECalcinator : BlockEntityFirepitContainer
    {
        //public override string InventoryClassName => "calcinator";
        private ICoreClientAPI capi;
        private ICoreServerAPI sapi;

        private GUICalcinator _clientDialog;
        private float calcinationAccum;
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
        public ItemSlot FuelSlot => _inventory[0];
        public bool FuelSlotEmpty => FuelSlot.Empty;
        
        public ItemSlot CalcinationSlot => _inventory[1];
        public ItemStack CalcinationStack => _inventory[1]?.Itemstack;
        public bool CalcinationSlotEmpty => CalcinationSlot.Empty;

        public ItemSlot OxidationSlot => _inventory[2];
        public ItemStack OxidationStack => _inventory[2]?.Itemstack;
        public bool OxidationSlotEmpty => OxidationSlot.Empty;
        
        public ItemSlot CatalysationSlot => _inventory[3];
        public ItemStack CatalysationStack => _inventory[3]?.Itemstack;
        public bool CatalysationSlotEmpty => CatalysationSlot.Empty;
        
        /// <summary>
        /// True if all input slots are empty
        /// </summary>
        public bool InputsEmpty => CalcinationSlotEmpty && OxidationSlotEmpty && CatalysationSlotEmpty;

        public ItemSlot OutputPrimarySlot => _inventory[4];
        public ItemStack OutputPrimaryStack
        {
            get => _inventory[4]?.Itemstack;
            set => _inventory[4].Itemstack = value;
        }
        public bool OutputPrimarySlotEmpty => OutputPrimarySlot.Empty;

        public  ItemSlot OutputSecondarySlot => _inventory[5];

        public ItemStack OutputSecondaryStack
        {
            get => _inventory[5]?.Itemstack;
            set => _inventory[5].Itemstack = value;
        }
        public bool OutputSecondarySlotEmpty => OutputSecondarySlot.Empty;
        
        /// <summary>
        /// True if all output slots are empty
        /// </summary>
        public bool OutputSlotsEmpty => OutputPrimarySlotEmpty && OutputSecondarySlotEmpty;
        
        public BECalcinator()
        {
            _inventory = new InventoryCalcinator(null, null);
            _inventory.SlotModified += OnSlotModifid;
        }

        private void OnSlotModifid(int slotid)
        {
            if (slotid >= 0 || slotid < 5) // input or fuel slot update
            {
                //TODO:
                //  ADD FindMatchingRecipe(); to check for calcination is possible
                //Api.World.Logger.Warning($"Slotid {slotid} is out of range");
                MarkDirty(true);
                if (_clientDialog != null)
                {
                    //_clientDialog.Update(RecipeProgress, CurrentTemp, CurrentRecipe);
                }
            }
        }
        
        #endregion
        
        public virtual float SoundLevel
        {
            get { return 0.66f; }
        }

       
        

        public void HeatItem(float dt)
        {
            if (_inventory[1].Empty || _inventory[2].Empty) return;

            if (InputStackTemp < 100)
            {
                InputStackTemp += dt * 2;
            }
        }

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
            get => CalcinationStack?.Collectible.GetTemperature(Api.World, _inventory[0].Itemstack) ?? 0;
            set => CalcinationStack.Collectible.SetTemperature(Api.World, _inventory[0].Itemstack, value);
        }

        public CalcinationProperties CalcinationProps
        {
            get
            {
                return CalcinationStack?.ItemAttributes?["calcinationProps"].AsObject<CalcinationProperties>();
            }
        }

        public virtual CalcinationProperties GetCalcinationProperties(ItemStack stack)//ItemSlot input)
        { // calcinationPropsByType
            //return input.Itemstack?.ItemAttributes?["calcinationProps"].AsObject<CalcinationProperties>(); //itemstack?.ItemAttributes?["calcinationProps"].AsObject<CalcinationProperties>();
            var props = stack?.ItemAttributes?["calcinationProps"].Exists == true ? stack.ItemAttributes["calcinationProps"].AsObject<CalcinationProperties>(null, stack.Collectible.Code.Domain) : null;
            props?.CalcinatedStack?.Resolve(Api.World, "Calcinatable Properties CalcinatedStack", stack.Collectible.Code);
            return  props;
        }
        

        public bool CanCalcinate()
        {
            CalcinationProperties calcinProps = GetCalcinationProperties(CalcinationStack);
            return calcinProps != null;
        }

        protected void CalcinateInput()
        {
            if (GetCalcinationProperties(CalcinationStack).CalcinatedStack != null)
            {
                CalcinationProperties  calcinProps = GetCalcinationProperties(CalcinationStack);
                Api.World.Logger.Warning("CalcinatedStack Is Real not a null referance at least");
                ItemStack calcinStack = calcinProps.CalcinatedStack.ResolvedItemStack.Clone();
                if (calcinStack != null && OutputPrimarySlot.Empty)
                {
                    OutputPrimaryStack = calcinStack;
                    CalcinationSlot.TakeOut(1);
                    CalcinationSlot.MarkDirty();
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
                    _clientDialog.Update(BlockTemperature, fuelBurnTime, calcinatoinProgress, AttributeInfo);
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
                if (!FuelSlotEmpty)
                {
                    ThermalProperties thermalProps = GetThermalProperties(FuelSlot.Itemstack);
                    CombustibleProperties combustibleProps = GetCombustibleProperties(Api.World, FuelSlot.Itemstack, null);
                    AddFuel(FuelSlot.Itemstack, FuelSlot);

                    /*if (thermalProps != null){
                        // float SpecificHeatCapacity = thermalProps.SpecificHeatCapacity;
                        // Api.World.Logger.Warning("ThermalProperties SpecificHeatCapacity: {0}kJ/kg·K",
                        //     SpecificHeatCapacity);
                        // float Weight = thermalProps.WeightInKg;
                        // Api.World.Logger.Warning("ThermalProperties Weight: {0}kg", Weight);
                        // SpecificBurnValueMin = thermalProps.SpecificBurnValueMin;
                        // Api.World.Logger.Warning("ThermalProperties SpecificBurnValueMin: {0}kJ/kg",
                        //     SpecificBurnValueMin);
                        // SpecificBurnValueMax = thermalProps.SpecificBurnValueMax;
                        // Api.World.Logger.Warning("ThermalProperties SpecificBurnValueMax: {0}kJ/kg",
                        //     SpecificBurnValueMax);
                        // float burnDuration = combustibleProps.BurnDuration;
                        // Api.World.Logger.Warning("CombustibleProperties BurnDuration: {0}Sec", burnDuration);
                        // float burnvalue = CalculateBurnValue(SpecificBurnValueMin, SpecificBurnValueMax, BlockTemperature, combustibleProps.BurnTemperature);
                        // Api.World.Logger.Warning("CombustibleProperties currentBurnValue: {0}kJ/kg", burnvalue);
                        // float energy = CalculateFuelEnergyOutput(burnDuration, Weight, burnvalue);
                        // Api.World.Logger.Warning("Calculated Fuel Energy Output: {0}kJ/s or kW", energy);
                    }*/
                }
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
                _clientDialog.Update(BlockTemperature, fuelBurnTime, calcinatoinProgress, AttributeInfo);
            }
        }
        
    }
}
