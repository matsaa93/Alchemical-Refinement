using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace AlchemicalRefinement.API.Common.BlockEntity
{
    public class BlockEntityFirepitContainer : BlockEntityOpenableContainer,IThermalProps , IFirePit, IHeatSource, ITemperatureSensitive
    {
        #region Variables

        private ICoreClientAPI capi;
        private ICoreServerAPI sapi;
        public ThermalFunctions TFun = new ThermalFunctions();
        public int FirepitStage;
        MeshData firepitMesh;
        private double lastTickTotalHours;
        public float BlockTemperature;
        private const float BlockSpecificHeatCapacity = 0.385f;
        private const float BlockWeight = 130;
        public float SpecificBurnValueMin;
        public float SpecificBurnValueMax;
    
        public override InventoryBase Inventory { get; }
        public override string InventoryClassName { get; }
        
        /// <summary>
        /// If CanIgnite is changed this needs to be overidden to make it fit with the new firepitStage in that methode. 
        /// </summary>
        public virtual bool IsBurning => FirepitStage == 6 && FuelBurnTime > 0;
        public virtual bool IsSmoldering => FirepitStage == 7 && FuelBurnTime > -3 && FuelTemperature >= 10;
        public virtual bool IsCold => BlockTemperature <= 10 && FuelTemperature <= 10;
        #endregion
        
        #region Initialization
        
        
        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            if (api.Side == EnumAppSide.Server)
            {
                sapi = api as ICoreServerAPI;
                //RegisterGameTickListener(OnClientUpdateTick, 1000);
                RegisterGameTickListener(OnFirepitBurnTick, 100);
                //_heatPerSecondBase = base.Block.Attributes["heatpersecond"].AsInt(0);
            }
            else
            {
                capi = api as ICoreClientAPI;
                //RegisterGameTickListener(OnFirepitBurnTick, 100);
                LoadFirepitMesh();
                if (FirepitStage == 6 && IsBurning)
                {
                    GetBehavior<BEBehaviorFirepitAmbient>()?.ToggleAmbientSounds(true);
                }
            }
        }
        #endregion

        #region Meshing
        public static AssetLocation[] FirepitShapeBlockCodes = new AssetLocation[]
        {
            null,
            new AssetLocation("firepit-construct1"),
            new AssetLocation("firepit-construct2"),
            new AssetLocation("firepit-construct3"),
            new AssetLocation("firepit-construct4"),
            new AssetLocation("firepit-cold"),
            new AssetLocation("firepit-lit"),
            new AssetLocation("firepit-extinct"),
        };
        
        /// <summary>
        /// Loads in the Mesh of the Firepit block,
        /// and assignes the mesh stage to the Mesh.
        /// </summary>
        private void LoadFirepitMesh()
        {
            if (Api.Side == EnumAppSide.Server) return;
            if (FirepitStage <= 0)
            {
                firepitMesh = null;
                return;
            }

            Block block = Api.World.GetBlock(FirepitShapeBlockCodes[FirepitStage]);
            firepitMesh = capi.TesselatorManager.GetDefaultBlockMesh(block);
        }
        
        public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
        {
            mesher.AddMeshData(firepitMesh);

            return base.OnTesselation(mesher, tessThreadTesselator);
        }
        #endregion

        #region TickingMethodes

        private void OnClientUpdateTick(float dt)
        {
            if ( Api is ICoreServerAPI && (IsBurning || IsSmoldering)) MarkDirty();
        }
        /// <summary>
        /// The Ticking function for the Firepit.
        /// It sets the Temperature with the heatBlock methode,
        /// to change how this function Override the heatBlock function.
        /// </summary>
        /// <param name="dt"> Time passed since last tick or block load</param>
        private void OnFirepitBurnTick(float dt)
        {
            if (IsCold) return;
            
            if (FirepitStage == 6 && !IsBurning)
            {
                GetBehavior<BEBehaviorFirepitAmbient>()?.ToggleAmbientSounds(false);
                FirepitStage++;
                MarkDirty(true);
            }
            
            if (IsBurning)
            {
                HeatBlock(dt);
                FuelBurnTime -= dt;
                //blockTemperature += 3 * 1;
            } else if (IsSmoldering || !IsCold)
            {
                CoolBlock(dt);
                if (IsCold) IsHot = false;
            }
            /*double dh = Api.World.Calendar.TotalHours - lastTickTotalHours;
            if (dh > 0.1f)
            {
                if (IsBurning) fuelBurnTime -= (float)dh;
                lastTickTotalHours = Api.World.Calendar.TotalHours;
            }*/
            // to do add rest of function/methode See BEBoiler onBurnTick for more
        }

        /// <summary>
        /// Calculation Function for Temperature of the block used for making the block hotter.
        /// can also be changed as it is virtual and possible to use for anything you want to happen,
        /// when the block meets the IsBurning condition in the OnFirepitBurnTick methode.
        /// </summary>
        /// <param name="dt"> Time passed since last tick or block load</param>
        public virtual void HeatBlock(float dt)
        {
            if (BlockTemperature < BlockTemperatureMax && BlockTemperature < FuelTemperature)
            {
                BlockTemperature += dt * BlockTemperatureIncrease;
            }
            else if ( BlockTemperature >= (BlockTemperatureMax - 4) &&  BlockTemperature <= (BlockTemperatureMax + 4) )
            { //if (number >= min && number <= max)
                BlockTemperature = BlockTemperatureMax;
            }
            else if (BlockTemperature > BlockTemperatureMax)
            {
                BlockTemperature -= dt * 2;
            }

            if (FuelTemperature < FuelTemperatureMax)
            {
                FuelTemperature += dt * FuelTempIncrease;
            }
            else if ( FuelTemperature >= (FuelTemperatureMax - 4) &&  FuelTemperature <= (FuelTemperatureMax + 4) )
            { //if (number >= min && number <= max)
                FuelTemperature = FuelTemperatureMax;
            }
            else if (FuelTemperature > FuelTemperatureMax)
            {
                FuelTemperature -= dt * 2;
            }
        }

        /// <summary>
        /// Calculation Function for Temperature when the block is cooling.
        /// can also be changed as it is virtual and possible to use for anything you want to happen,
        /// when the block meets the IsSmoldering condition in the OnFirepitBurnTick methode.
        /// </summary>
        /// <param name="dt"> Time passed since last tick or block load</param>
        public virtual void CoolBlock(float dt)
        {
            //Api.World.PlaySoundAt(new AssetLocation("sounds/effect/extinguish"), Pos, -0.5, null, false, 16);
            if(BlockTemperature >= 0) BlockTemperature -= dt * 8;
            if(FuelTemperature >= 0) FuelTemperature -= dt * 8;
        }
        public float GetHeatStrength(IWorldAccessor world, BlockPos heatSourcePos, BlockPos heatReceiverPos)
        {
            return IsBurning ? 10 : (IsSmoldering ? 0.25f : 0);
        }

        public void CoolNow(float amountRel, OnStackToCool onStackToCoolCallback)
        {
            Api.World.PlaySoundAt(new AssetLocation("sounds/effect/extinguish"), Pos, -0.5, null, false, 16);
            
            //TODO: impliment CoolNow fully replace or Add to Cool block.
            //fuelBurnTime -= (float)amountRel / 10f;

            /*if (Api.World.Rand.NextDouble() < amountRel / 5f || fuelBurnTime <= 0)
            {
                setBlockState("cold");
                extinguishedTotalHours = -99;
                canIgniteFuel = false;
                fuelBurnTime = 0;
                maxFuelBurnTime = 0;
            }*/

            MarkDirty(true);
        }

        public bool IsHot { get; private set; }

        #endregion

        #region blockInteraction

        public override bool OnPlayerRightClick(IPlayer byPlayer, BlockSelection blockSel)
        {
            return true;
        }
        
        /// <summary>
        /// If override remember to call this after your interactions are done
        /// base.OnInteract(byPlayer, blockSel);
        /// this adds Interactions for adding Fuel and Tinder and Igniting the Block.
        /// </summary>
        /// <param name="byPlayer"></param>
        /// <param name="blockSel"></param>
        /// <returns>True if interaction was valid</returns>
        public virtual bool OnInteract(IPlayer byPlayer, BlockSelection blockSel)
        {
            ItemSlot hotbarSlot = byPlayer.InventoryManager.ActiveHotbarSlot;

            bool isFuel = hotbarSlot.Itemstack?.Collectible is ItemFirewood || hotbarSlot.Itemstack?.Collectible is ItemCoal;
            bool addGrass = hotbarSlot.Itemstack?.Collectible is ItemDryGrass && FirepitStage == 0;
            bool addFireWood = hotbarSlot.Itemstack?.Collectible is ItemFirewood && FirepitStage >= 1 && FirepitStage <= 4;
            bool reignite = (isFuel) && (FirepitStage >= 5 && FuelBurnTime <= 10f);
            bool interactgui = (hotbarSlot.Empty || !isFuel) && (FirepitStage >= 5);

            if (interactgui) OnPlayerRightClick(byPlayer, blockSel);
            if (addGrass || addFireWood || reignite)
            {
                if (!reignite) FirepitStage++;
                else if (FirepitStage == 7 && FuelTemperature > 100) FirepitStage = 6;
                else if  (FirepitStage == 7 && FuelTemperature <= 100) FirepitStage = 5;
                
                MarkDirty(true);
                if (addFireWood || reignite)
                {
                    AddFuel(hotbarSlot.Itemstack, hotbarSlot);
                }
                else
                {
                    hotbarSlot.TakeOut(1);
                }
                
                (byPlayer as IClientPlayer)?.TriggerFpAnimation(EnumHandInteract.HeldItemInteract);
                Block block = Api.World.GetBlock(FirepitShapeBlockCodes[FirepitStage]);
                if (block?.Sounds != null) Api.World.PlaySoundAt(block.Sounds.Place, Pos, 0, byPlayer);
                return true;
            }
            
            return false;
        }

        #endregion

        #region fuelHandeling
        public CombustibleProperties GetCombustibleProperties(IWorldAccessor world, ItemStack stack, BlockPos pos)
        {
            return stack?.Collectible.GetCombustibleProperties(world, stack, null);
        }
        public ThermalProperties GetThermalProperties(ItemStack stack)
        {
            return stack?.ItemAttributes?["thermalProps"].AsObject<ThermalProperties>();
        }
        
        /// <summary>
        /// Methode for addind fuel to the Burn Time
        /// also Calculates the ThermalProperties for the Fuel
        /// </summary>
        /// <param name="stack">The Fuel ItemStack</param>
        /// <param name="slot">Inventory Slot for the Fuel Stack</param>
        public virtual void AddFuel(ItemStack stack, ItemSlot slot)
        {
            if (FuelBurnTime >= 2) return;
            CombustibleProperties fuelCopts = TFun.GetCombustibleProperties(Api.World, stack, null);
            ThermalProperties thermalProps = TFun.GetThermalProperties(stack);
            if(thermalProps != null && fuelCopts != null && (FuelTemperature >= 100 || FuelBurnTime <= 3))
            {
                float burnduration = fuelCopts.BurnDuration * BurnDurationModifier;
                CurrentBurnValue = TFun.CalculateBurnValue(thermalProps.SpecificBurnValueMin,
                    thermalProps.SpecificBurnValueMax, FuelTemperature, fuelCopts.BurnTemperature);
                FuelEnergyOutput = TFun.CalculateFuelEnergyOutput(burnduration, thermalProps.WeightInKg, CurrentBurnValue);
                FuelTempIncrease = TFun.CalculateTemperatureIncrease(FuelEnergyOutput * 0.2f,  thermalProps.WeightInKg, 1, thermalProps.SpecificHeatCapacity);
                BlockTemperatureIncrease = TFun.CalculateTemperatureIncrease(FuelEnergyOutput * 0.2f, BlockWeight, 1, BlockSpecificHeatCapacity);
                FuelBurnTime += burnduration;
                FuelTemperatureMax = fuelCopts.BurnTemperature;
                BlockTemperatureMax = FuelTemperatureMax * 0.85f;
                slot.TakeOut(1);
                if (IsSmoldering) FirepitStage = 6;
            }
            //slot.TakeOut(1);
        }
        
        public virtual float BurnDurationModifier
        {
            get { return 1f; }
        }
        public float FuelTemperature { get; set; }
        public float FuelTemperatureMax { get; set; }
        public float BlockTemperatureMax { get; set; }
        public float FuelBurnTime { get; set; }
        public float BlockTemperatureIncrease { get; set; }

        public float FuelTempIncrease { get; set; }
        //private float FuelTempIncreaseMax { get; set; }
        /// <summary>
        /// Calculated Fuel Energy Output when burning in kJ/s or kW
        /// </summary>
        public float FuelEnergyOutput { get; set; }
        
        /// <summary>
        /// the current Specific BurnValue for the item in kJ/kg 
        /// </summary>
        public float CurrentBurnValue { get; set; }
        
        /// <summary>
        /// Checks for the possibility to inginte the block
        /// by default it checks firepiStage override if to change stage
        /// </summary>
        /// <returns>true if It is at the right firepitStage</returns>
        public virtual bool CanIgnite()
        {
            return FirepitStage == 5;
        }
        
        /// <summary>
        /// Tries to ignite the block if it is CanIgnite returns true
        /// else it is skipped
        /// </summary>
        public virtual void TryIgnite()
        {
            if (!CanIgnite()) return;

            FirepitStage++;
            FuelTemperature = 100;
            IsHot = true;
            GetBehavior<BEBehaviorFirepitAmbient>()?.ToggleAmbientSounds(true);

            MarkDirty(true);
            lastTickTotalHours = Api.World.Calendar.TotalHours;
        }
        #endregion

        #region TreeAttribute

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessorForResolve)
        {
            base.FromTreeAttributes(tree, worldAccessorForResolve);

            FirepitStage = tree.GetInt("firepitConstructionStage");
            lastTickTotalHours = tree.GetDouble("lastTickTotalHours");
            FuelBurnTime = tree.GetFloat("fuelBurnTime");
            BlockTemperature  = tree.GetFloat("blockTemperature");
            BlockTemperatureMax = tree.GetFloat("blockMaxTemperature");
            BlockTemperatureIncrease = tree.GetFloat("blockTemperatureIncrease");
            FuelEnergyOutput = tree.GetFloat("fuelEnergyOutput");
            CurrentBurnValue = tree.GetFloat("currentBurnValue");
            FuelTemperature = tree.GetFloat("fuelTemperature");
            FuelTemperatureMax  = tree.GetFloat("fuelTemperatureMax");
            FuelTempIncrease = tree.GetFloat("fuelTempIncrease");
            

            if (Api != null) LoadFirepitMesh();
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);

            tree.SetInt("firepitConstructionStage", FirepitStage);
            tree.SetDouble("lastTickTotalHours", lastTickTotalHours);
            tree.SetFloat("fuelBurnTime", FuelBurnTime);
            tree.SetFloat("blockTemperature", BlockTemperature);
            tree.SetFloat("blockMaxTemperature", BlockTemperatureMax);
            tree.SetFloat("blockTemperatureIncrease", BlockTemperatureIncrease);
            tree.SetFloat("fuelEnergyOutput", FuelEnergyOutput);
            tree.SetFloat("currentBurnValue", CurrentBurnValue);
            tree.SetFloat("fuelTemperature", FuelTemperature);
            tree.SetFloat("fuelTemperatureMax", FuelTemperatureMax);
            tree.SetFloat("fuelTempIncrease", FuelTempIncrease);
        }

        #endregion
        

        
    }
}

