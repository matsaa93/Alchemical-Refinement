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
using HarmonyLib;
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
        public float calcinatoinProgress;
        public float CraftEnergy;
        private float energyTempIncrease;
        private bool activeRecipe = false;
        public string AttributeInfo;
        private CalcinationFunctions CalFun = new CalcinationFunctions();

        #region Invetory
        private InventoryCalcinator _inventory;
        public override string InventoryClassName { get { return "InventoryCalcinator"; } }
        /// <summary>
        /// SlotID: 0 = fuel, 1-3 = input, 4-5 = output
        /// </summary>
        public override InventoryBase Inventory => _inventory;

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

        public ItemStack activeResolveStack;

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

        private bool[] _recipeSlots = { false, false, false, false};
        
        public BECalcinator()
        {
            _inventory = new InventoryCalcinator(null, null);
            _inventory.SlotModified += OnSlotModifid;
        }
        #endregion

        #region OnSlotModifid
        protected void OnSlotModifid(int slotid)
        {
            if (Api is ICoreClientAPI)
            {
                _clientDialog.Update(BlockTemperature, FuelBurnTime, calcinatoinProgress, CraftEnergy);
            }
            if (slotid >= 0 && slotid <= 5) 
            {
                //Api.World.Logger.Warning($"Slotid {slotid} is out of range");
                if (Api.Side == EnumAppSide.Server)
                {
                    if (!InputsEmpty && (slotid >= 1 && slotid <= 3))
                    {
                        activeRecipe = FindMatchingRecipe(slotid);
                        
                        if (activeRecipe)
                        {
                            Api.World.Logger.Warning("amount of active Calcination stacks");
                            SetCraftThermalProperties();
                        }
                    }else if (InputsEmpty)
                    {
                        _recipeSlots[slotid] = false;
                        activeRecipe = false;
                        CraftEnergy = 0f;
                        energyTempIncrease = 0f;
                    }
                }
                MarkDirty(true);
                if (_clientDialog != null && _clientDialog.IsOpened())
                {
                    _clientDialog.SingleComposer.ReCompose();
                }
            }
        }

        private bool FindMatchingRecipe(int slotId)
        {
            ItemStack stack = null;
            if (_inventory[slotId].Empty)
            {
                _recipeSlots[slotId] = false;
                return _recipeSlots[1] ||  _recipeSlots[2] || _recipeSlots[3];
            }

            stack = _inventory[slotId].Itemstack;
            /*switch (slotId)
            {
                case 1:
                    stack = CalcinationStack;
                    //Api.World.Logger.Warning("CalcinationStack has the Properties needed for craft {0}.", stack.StackSize);
                    break;
                case 2:
                    stack = OxidationStack;
                    //Api.World.Logger.Warning("OxidationStack has the Properties needed for craft {0}.", stack.StackSize);
                    break;
                case 3:
                    stack = CatalysationStack;
                    //Api.World.Logger.Warning("CatalysationStack has the Properties needed for craft {0} {1}.", stack.StackSize, stack.GetName());
                    break;
            }*/

            if (HasCalcinationProp(stack))
            {
                _recipeSlots[slotId] = true;
            }
            else
            {
                _recipeSlots[slotId] = false;
            }
            return _recipeSlots[1] ||  _recipeSlots[2] || _recipeSlots[3];
        }

        private void SetCraftThermalProperties()
        {
            // TODO: Impliment this Methode Fully. use for loop to loop the _recipeSlots and
            //      and check if the bool is true for the slot.
            float energy = 0f;
            float  craftEnergy = 0f;
            ItemStack stack = null;
            ThermalProperties  props = null;
            CalcinationProperties CProps = null;
            for (int i = 1; i <= 3; i++)
            {
                if (!_recipeSlots[i]) continue;
                stack = i switch
                {
                    1 => CalcinationStack,
                    2 => OxidationStack,
                    3 => CatalysationStack,
                    _ => stack
                };
                props = TFun.GetThermalProperties(stack);
                CProps = CalFun.GetCalcinationProperties(Api.World, stack);
                energy += TFun.EnergyForTemperatureIncrease(props.WeightInKg, stack.StackSize, props.SpecificHeatCapacity);
                craftEnergy += TFun.CraftEnergyNeeded(CProps.CalcinationEnergy, stack.StackSize);
                Api.World.Logger.Chat("Energy for Temperature Increase in loop {0} slot{1}",energy, i);
                Api.World.Logger.Chat("Craft Energy in loop {0} slot{1}",craftEnergy, i);
            }
            energyTempIncrease = energy;
            CraftEnergy = craftEnergy;
            Api.World.Logger.Warning("Energy for Temperature Increase in total {0}",energyTempIncrease);
            Api.World.Logger.Warning("Craft Energy in total {0}",CraftEnergy);
        }

        private bool HasCalcinationProp(ItemStack stack)
        {
            if (stack == null) return false;
            CalcinationProperties calcinProps = GetCalcinationProperties(stack);
            return calcinProps != null;
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
                //Api.World.Logger.Warning("CalcinatedStack Is Real not a null referance at least");
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
                    _clientDialog.Update(BlockTemperature, FuelBurnTime, calcinatoinProgress, CraftEnergy);
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
            //_inventory.LateInitialize("InventoryCalcinator-" + Pos.X + "/" + Pos.Y + "/" + Pos.Z, api);
            _inventory.LateInitialize($"{InventoryClassName}-{this.Pos.X}/{this.Pos.Y}/{this.Pos.Z}", api);
        }

        

        public void OnCraftTick(float dt)
        {
            if (IsBurning || IsSmoldering)
            {
                if (CanCalcinate())
                {
                    CalcinateInput();
                }
                
                
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
            tree.SetFloat("calcinatoinProgress", calcinatoinProgress);
            tree.SetFloat("craftEnergy", CraftEnergy);
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessorForResolve)
        {
            base.FromTreeAttributes(tree, worldAccessorForResolve);
            _inventory.FromTreeAttributes(tree.GetTreeAttribute("inventory"));
            calcinatoinProgress = tree.GetFloat("calcinatoinProgress");
            CraftEnergy = tree.GetFloat("craftEnergy");
            if (Api?.Side == EnumAppSide.Client && _clientDialog != null)
            {
                _clientDialog.Update(BlockTemperature, FuelBurnTime, calcinatoinProgress, CraftEnergy);
            }
        }
        
    }
}
