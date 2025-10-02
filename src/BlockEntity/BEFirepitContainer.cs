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
        public float fuelHours;
        public int firepitStage;
        MeshData firepitMesh;
        double lastTickTotalHours;
        private float blockMaxTemperature = 750;
        public float blockTemperature;
    
        public override InventoryBase Inventory { get; }
        public override string InventoryClassName { get; }
        public override bool OnPlayerRightClick(IPlayer byPlayer, BlockSelection blockSel)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// If CanIgnite is changed this needs to be overidden to make it fit with the new firepitStage in that methode. 
        /// </summary>
        public virtual bool IsBurning => firepitStage == 6 && fuelHours > 0;
        public virtual bool IsSmoldering => firepitStage == 7 && fuelHours > -3;
        
        public static AssetLocation[] firepitShapeBlockCodes = new AssetLocation[]
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
                RegisterGameTickListener(onFirepitBurnTick, 100);
                //_heatPerSecondBase = base.Block.Attributes["heatpersecond"].AsInt(0);
            }
            else
            {
                capi = api as ICoreClientAPI;
                RegisterGameTickListener(onFirepitBurnTick, 100);
                loadFirepitMesh();
                if (firepitStage == 6 && IsBurning)
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
        private void onFirepitBurnTick(float dt)
        {
            if (firepitStage == 6 && !IsBurning)
            {
                GetBehavior<BEBehaviorFirepitAmbient>()?.ToggleAmbientSounds(false);
                firepitStage++;
                MarkDirty(true);
            }

            if (IsBurning)
            {
                heatBlock(dt);
                //blockTemperature += 3 * 1;
            } else if (IsSmoldering)
            {
                coolBlock(dt);
            }

            double dh = Api.World.Calendar.TotalHours - lastTickTotalHours;
            if (dh > 0.1f)
            {
                if (IsBurning) fuelHours -= (float)dh;
                lastTickTotalHours = Api.World.Calendar.TotalHours;
            }
            // to do add rest of function/methode See BEBoiler onBurnTick for more
        }

        /// <summary>
        /// Calculation Function for Temperature of the block used for making the block hotter.
        /// </summary>
        /// <param name="dt"></param>
        public virtual void heatBlock(float dt)
        {
            if (blockTemperature <= blockMaxTemperature)
            {
                blockTemperature += dt * 8;
            }
        }

        public virtual void coolBlock(float dt)
        {
            if(blockTemperature >= 0) blockTemperature -= dt * 8;
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

            bool addGrass = hotbarSlot.Itemstack?.Collectible is ItemDryGrass && firepitStage == 0;
            bool addFireWood = hotbarSlot.Itemstack?.Collectible is ItemFirewood && firepitStage >= 1 && firepitStage <= 4;
            bool reignite = hotbarSlot.Itemstack?.Collectible is ItemFirewood && (firepitStage >= 5 && fuelHours <= 6f);

            if (addGrass || addFireWood || reignite)
            {
                if (!reignite) firepitStage++;
                else if (firepitStage == 7) firepitStage = 5;

                MarkDirty(true);
                hotbarSlot.TakeOut(1);
                (byPlayer as IClientPlayer)?.TriggerFpAnimation(EnumHandInteract.HeldItemInteract);
                Block block = Api.World.GetBlock(firepitShapeBlockCodes[firepitStage]);
                if (block?.Sounds != null) Api.World.PlaySoundAt(block.Sounds.Place, Pos, 0, byPlayer);
            }

            if (addGrass) return true;
            if (addFireWood || reignite)
            {
                fuelHours = Math.Max(2, fuelHours + 2);
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
            return firepitStage == 5;
        }
        
        /// <summary>
        /// Tries to ignite the block if it is CanIgnite returns true
        /// else it is skipped
        /// </summary>
        public virtual void TryIgnite()
        {
            if (!CanIgnite()) return;

            firepitStage++;
            GetBehavior<BEBehaviorFirepitAmbient>()?.ToggleAmbientSounds(true);

            MarkDirty(true);
            lastTickTotalHours = Api.World.Calendar.TotalHours;
        }
        
        /// <summary>
        /// Loads in the Mesh of the Firepit block,
        /// and assignes the mesh stage to the Mesh.
        /// </summary>
        private void loadFirepitMesh()
        {
            if (Api.Side == EnumAppSide.Server) return;
            if (firepitStage <= 0)
            {
                firepitMesh = null;
                return;
            }

            Block block = Api.World.GetBlock(firepitShapeBlockCodes[firepitStage]);
            firepitMesh = capi.TesselatorManager.GetDefaultBlockMesh(block);
        }
        
        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessorForResolve)
        {
            base.FromTreeAttributes(tree, worldAccessorForResolve);

            firepitStage = tree.GetInt("firepitConstructionStage");
            lastTickTotalHours = tree.GetDouble("lastTickTotalHours");
            fuelHours = tree.GetFloat("fuelHours");
            blockTemperature  = tree.GetFloat("blockTemperature");

            if (Api != null) loadFirepitMesh();
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);

            tree.SetInt("firepitConstructionStage", firepitStage);
            tree.SetDouble("lastTickTotalHours", lastTickTotalHours);
            tree.SetFloat("fuelHours", fuelHours);
            tree.SetFloat("blockTemperature", blockTemperature);
        }
        public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
        {
            mesher.AddMeshData(firepitMesh);

            return base.OnTesselation(mesher, tessThreadTesselator);
        }
    }
}

