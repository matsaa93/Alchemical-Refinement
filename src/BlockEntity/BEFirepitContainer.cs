using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace AlchemicalRefinement
{
    public class BEFirepitContainer : BlockEntityOpenableContainer, IFirePit
    {
        private ICoreClientAPI capi;
        private ICoreServerAPI sapi;
        public float FuelHours;
        public int FirepitStage;
        MeshData firepitMesh;
        double lastTickTotalHours;
        private float blockMaxTemperature = 750;
        public float BlockTemperature;
    
        public override InventoryBase Inventory { get; }
        public override string InventoryClassName { get; }
        
        
        public override bool OnPlayerRightClick(IPlayer byPlayer, BlockSelection blockSel)
        {
            return true;
        }

        /// <summary>
        /// If CanIgnite is changed this needs to be overidden to make it fit with the new firepitStage in that methode. 
        /// </summary>
        public virtual bool IsBurning => FirepitStage == 6 && FuelHours > 0;
        public virtual bool IsSmoldering => FirepitStage == 7 && FuelHours > -3 && BlockTemperature >= 10;
        
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
        
        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            if (api.Side == EnumAppSide.Server)
            {
                sapi = api as ICoreServerAPI;
                RegisterGameTickListener(OnFirepitBurnTick, 100);
                //_heatPerSecondBase = base.Block.Attributes["heatpersecond"].AsInt(0);
            }
            else
            {
                capi = api as ICoreClientAPI;
                RegisterGameTickListener(OnFirepitBurnTick, 100);
                LoadFirepitMesh();
                if (FirepitStage == 6 && IsBurning)
                {
                    GetBehavior<BEBehaviorFirepitAmbient>()?.ToggleAmbientSounds(true);
                }
            }

            //RegisterGameTickListener(onBurnTick, 100);
            //loadFirepitMesh();

            //if (firepitStage == 6 && IsBurning)
            //{
            //    GetBehavior<BEBehaviorFirepitAmbient>()?.ToggleAmbientSounds(true);
            //}
        }
        
        /// <summary>
        /// The Ticking function for the Firepit.
        /// It sets the Temperature with the heatBlock methode,
        /// to change how this function Override the heatBlock function.
        /// </summary>
        /// <param name="dt"></param>
        private void OnFirepitBurnTick(float dt)
        {
            if (FirepitStage == 6 && !IsBurning)
            {
                GetBehavior<BEBehaviorFirepitAmbient>()?.ToggleAmbientSounds(false);
                FirepitStage++;
                MarkDirty(true);
            }

            if (IsBurning)
            {
                HeatBlock(dt);
                //blockTemperature += 3 * 1;
            } else if (IsSmoldering)
            {
                CoolBlock(dt);
            }

            double dh = Api.World.Calendar.TotalHours - lastTickTotalHours;
            if (dh > 0.1f)
            {
                if (IsBurning) FuelHours -= (float)dh;
                lastTickTotalHours = Api.World.Calendar.TotalHours;
            }
            // to do add rest of function/methode See BEBoiler onBurnTick for more
        }

        /// <summary>
        /// Calculation Function for Temperature of the block used for making the block hotter.
        /// </summary>
        /// <param name="dt"></param>
        public virtual void HeatBlock(float dt)
        {
            if (BlockTemperature <= blockMaxTemperature)
            {
                BlockTemperature += dt * 8;
            }
        }

        public virtual void CoolBlock(float dt)
        {
            if(BlockTemperature >= 0) BlockTemperature -= dt * 8;
        }
        
        
        
        
        /// <summary>
        /// If override remember to call this after your interactions are done
        /// base.OnInteract(byPlayer, blockSel);
        /// this adds Interactions for adding Firewood and Tinder and Igniting the Block.
        /// </summary>
        /// <param name="byPlayer"></param>
        /// <param name="blockSel"></param>
        /// <returns>True if interaction was valid</returns>
        public virtual bool OnInteract(IPlayer byPlayer, BlockSelection blockSel)
        {
            ItemSlot hotbarSlot = byPlayer.InventoryManager.ActiveHotbarSlot;

            bool addGrass = hotbarSlot.Itemstack?.Collectible is ItemDryGrass && FirepitStage == 0;
            bool addFireWood = hotbarSlot.Itemstack?.Collectible is ItemFirewood && FirepitStage >= 1 && FirepitStage <= 4;
            bool reignite = hotbarSlot.Itemstack?.Collectible is ItemFirewood && (FirepitStage >= 5 && FuelHours <= 6f);
            bool interactgui = hotbarSlot.Empty && (FirepitStage >= 5);

            if (interactgui) OnPlayerRightClick(byPlayer, blockSel);
            if (addGrass || addFireWood || reignite)
            {
                if (!reignite) FirepitStage++;
                else if (FirepitStage == 7) FirepitStage = 5;

                MarkDirty(true);
                hotbarSlot.TakeOut(1);
                (byPlayer as IClientPlayer)?.TriggerFpAnimation(EnumHandInteract.HeldItemInteract);
                Block block = Api.World.GetBlock(FirepitShapeBlockCodes[FirepitStage]);
                if (block?.Sounds != null) Api.World.PlaySoundAt(block.Sounds.Place, Pos, 0, byPlayer);
            }

            if (addGrass) return true;
            if (addFireWood || reignite)
            {
                FuelHours = Math.Max(2, FuelHours + 2);
                return true;
            }

            return false;
        }
        
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
            GetBehavior<BEBehaviorFirepitAmbient>()?.ToggleAmbientSounds(true);

            MarkDirty(true);
            lastTickTotalHours = Api.World.Calendar.TotalHours;
        }
        
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
        
        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessorForResolve)
        {
            base.FromTreeAttributes(tree, worldAccessorForResolve);

            FirepitStage = tree.GetInt("firepitConstructionStage");
            lastTickTotalHours = tree.GetDouble("lastTickTotalHours");
            FuelHours = tree.GetFloat("fuelHours");
            BlockTemperature  = tree.GetFloat("blockTemperature");

            if (Api != null) LoadFirepitMesh();
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);

            tree.SetInt("firepitConstructionStage", FirepitStage);
            tree.SetDouble("lastTickTotalHours", lastTickTotalHours);
            tree.SetFloat("fuelHours", FuelHours);
            tree.SetFloat("blockTemperature", BlockTemperature);
        }
        public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
        {
            mesher.AddMeshData(firepitMesh);

            return base.OnTesselation(mesher, tessThreadTesselator);
        }
    }
}

