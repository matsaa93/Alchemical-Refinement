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

    public class BECalcinator : BlockEntityOpenableContainer, IFirePit
    {
        //public override string InventoryClassName => "calcinator";
        private ICoreClientAPI capi;
        private ICoreServerAPI sapi;
        MeshData firepitMesh;
        public int firepitStage;
        double lastTickTotalHours;
        public float fuelHours;
        float calcinationAccum;
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
        
        public virtual float SoundLevel
        {
            get { return 0.66f; }
        }

        public bool IsBurning => firepitStage == 6 && fuelHours > 0;
        public bool IsSmoldering => firepitStage == 6 && fuelHours > -3;

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

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            if (api.Side == EnumAppSide.Server)
            {
                sapi = api as ICoreServerAPI;
                RegisterGameTickListener(onBurnTick, 100);
                //_heatPerSecondBase = base.Block.Attributes["heatpersecond"].AsInt(0);
            }
            else
            {
                capi = api as ICoreClientAPI;
            }

            //RegisterGameTickListener(onBurnTick, 100);
            loadMesh();

            if (firepitStage == 6 && IsBurning)
            {
                GetBehavior<BEBehaviorFirepitAmbient>()?.ToggleAmbientSounds(true);
            }
        }

        private void onBurnTick(float dt)
        {
            if (firepitStage == 6 && !IsBurning)
            {
                GetBehavior<BEBehaviorFirepitAmbient>()?.ToggleAmbientSounds(false);
                firepitStage++;
                MarkDirty(true);
            }

            if (IsBurning)
            {
                heatItem(dt);
            }

            double dh = Api.World.Calendar.TotalHours - lastTickTotalHours;
            if (dh > 0.1f)
            {
                if (IsBurning) fuelHours -= (float)dh;
                lastTickTotalHours = Api.World.Calendar.TotalHours;
            }
            // to do add rest of function/methode See BEBoiler onBurnTick for more
        }

        public void heatItem(float dt)
        {
            if (_inventory[1].Empty || _inventory[2].Empty) return;

            if (InputStackTemp < 100)
            {
                InputStackTemp += dt * 2;
            }
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

        public bool OnInteract(IPlayer byPlayer, BlockSelection blockSel)
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

        public bool CanIgnite()
        {
            return firepitStage == 5;
        }

        public void TryIgnite()
        {
            if (!CanIgnite()) return;

            firepitStage++;
            GetBehavior<BEBehaviorFirepitAmbient>()?.ToggleAmbientSounds(true);

            MarkDirty(true);
            lastTickTotalHours = Api.World.Calendar.TotalHours;
        }

        private void loadMesh()
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

            if (Api != null) loadMesh();
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);

            tree.SetInt("firepitConstructionStage", firepitStage);
            tree.SetDouble("lastTickTotalHours", lastTickTotalHours);
            tree.SetFloat("fuelHours", fuelHours);
        }

        public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
        {
            mesher.AddMeshData(firepitMesh);

            return base.OnTesselation(mesher, tessThreadTesselator);
        }
        public override bool OnPlayerRightClick(IPlayer byPlayer, BlockSelection blockSel)
        {
            return true;
        }
    }
}
