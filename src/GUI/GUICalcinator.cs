using System;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace AlchemicalRefinement.GUI
{
    public class GUICalcinator: GuiDialogBlockEntity
    {
        private BECalcinator _beCalcinator;
        private float _craftProgress;
        private float _blockTemp = 0;
        private float _fuelHours = 0;
        private long lastRedrawMs;
        
        protected override double FloatyDialogPosition => 0.75;

        public GUICalcinator(string dialogTitle, InventoryBase inventory, BlockPos blockEntityPos, ICoreClientAPI capi): base(dialogTitle, inventory, blockEntityPos, capi)
        {
            if (IsDuplicate) return;
            _beCalcinator = capi.World.BlockAccessor.GetBlockEntity(blockEntityPos) as BECalcinator;
            capi.World.Player.InventoryManager.OpenInventory(inventory);
            if (_beCalcinator != null)
            {
                _blockTemp = _beCalcinator.BlockTemperature;
                _fuelHours = _beCalcinator.FuelHours;
            }

            SetupDialog();
        }

        private void SetupDialog()
        {
            int titlebarheight = 32;
            double slotpadding = 3;
            double itemslotwidth = 48;
            float insetbrightness = 0.85f;
            int insetdepth = 2;
            ItemSlot hoveredSlot = capi.World.Player.InventoryManager.CurrentHoveredSlot;
            if (hoveredSlot != null && hoveredSlot.Inventory == base.Inventory)
            {
                capi.Input.TriggerOnMouseLeaveSlot(hoveredSlot);
            }
            else hoveredSlot = null;

            
            ElementBounds dialogBounds = ElementBounds.Fixed(320, 218 + titlebarheight);
            ElementBounds dialog = ElementBounds.Fill.WithFixedPadding(0);
            dialog.BothSizing = ElementSizing.FitToChildren;
            
            ElementBounds titlebarbounds = ElementBounds.Fixed(0, 0, 320,titlebarheight);
            
            ElementBounds inputslotinset = ElementBounds.Fixed(5, 8 + titlebarheight, 164, 58);
            //ElementBounds inputslotbnd = ElementBounds.Fixed(10, 13 + titlebarheight, 48, 154);
            ElementBounds inputslotbnd1 = ElementBounds.Fixed(10, 13 + titlebarheight, 48, 48);
            ElementBounds inputslotbnd2 = ElementBounds.Fixed(63, 13 + titlebarheight, 48, 48);
            ElementBounds inputslotbnd3 = ElementBounds.Fixed(116, 13 + titlebarheight, 48, 48);
            
            ElementBounds outputslotinset = ElementBounds.Fixed(204, 8 + titlebarheight, 111, 58);
            //ElementBounds outputslotbnd = ElementBounds.Fixed(209, 13 + titlebarheight, 48, 101);
            ElementBounds outputslotbnd1 = ElementBounds.Fixed(209, 13 + titlebarheight, 48, 48);
            ElementBounds outputslotbnd2 = ElementBounds.Fixed(262, 13 + titlebarheight, 48, 48);
            
            ElementBounds fuelslotinset = ElementBounds.Fixed(58, 155 + titlebarheight, 58, 58);
            ElementBounds fuelslotbnd = ElementBounds.Fixed(63, 160 + titlebarheight, 48, 48);

            ElementBounds textareainset = ElementBounds.Fixed(131, 155 + titlebarheight, 158, 58);
            //ElementBounds blocktemptextinset = ElementBounds.Fixed(58, 90 + titlebarheight, 221, 96);
            ElementBounds blocktemptextbnds = ElementBounds.Fixed(136, 160 + titlebarheight, 148, 18);
            
            //ElementBounds fuelhourtextinset = ElementBounds.Fixed(58, 113 + titlebarheight, 221, 96);
            ElementBounds fuelhourtextbnds = ElementBounds.Fixed(136, 190 + titlebarheight, 148, 18);

            dialog.WithChildren(new ElementBounds[]
            {
                dialogBounds,
                titlebarbounds,
                inputslotinset,
                //inputslotbnd,
                inputslotbnd1,
                inputslotbnd2,
                inputslotbnd3,
                outputslotinset,
                //outputslotbnd,
                outputslotbnd1,
                outputslotbnd2,
                fuelslotinset,
                textareainset,
                //blocktemptextinset,
                blocktemptextbnds,
                //fuelhourtextinset,
                fuelhourtextbnds
            });
            double[] yellow = new double[3] { 1, 1, 0 };
            CairoFont leftyellow = CairoFont.WhiteDetailText().WithWeight(FontWeight.Normal).WithOrientation(EnumTextOrientation.Left).WithColor(yellow);
            ElementBounds window = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle)
                .WithFixedAlignmentOffset(-GuiStyle.DialogToScreenPadding, 0);
            
            if (capi.Settings.Bool["immersiveMouseMode"])
            {
                window.WithAlignment(EnumDialogArea.RightMiddle).WithFixedAlignmentOffset(-12, 0);
            }
            else
            {
                window.WithAlignment(EnumDialogArea.CenterMiddle).WithFixedAlignmentOffset(20, 0);
            }
            BlockPos blockPos = base.BlockEntityPosition;
            //int inputslotamount = 3;
            int[] inputSlotIds = new int[3] { 1, 2, 3 };
            //for (int i = 1; i < inputslotamount; i++) inputSlotIds[i]= i;
            //int outputslotamount = 2;
            int[] outputSlotIds = new int[2] { 4, 5 };
            //for (int i = 1; i < outputslotamount; i++) outputSlotIds[i] =3 + i;

            this.SingleComposer = capi.Gui.CreateCompo("arcalcinatordlg" + blockPos?.ToString(), window)
                .AddShadedDialogBG(dialog, true, 5)
                .AddDialogTitleBar(DialogTitle, OnTitleBarClosed)
                .BeginChildElements(dialog)
                .AddInset(inputslotinset, insetdepth, insetbrightness)
                //.AddItemSlotGrid(Inventory, SendInvPacket,3, inputSlotIds, inputslotbnd, "inputSlots")
                .AddItemSlotGrid(Inventory, SendInvPacket,1, new int[] {1}, inputslotbnd1, "inputSlots1")
                .AddItemSlotGrid(Inventory, SendInvPacket,1, new int[] {2}, inputslotbnd2, "inputSlots2")
                .AddItemSlotGrid(Inventory, SendInvPacket,1, new int[] {3}, inputslotbnd3, "inputSlots3")
                .AddInset(outputslotinset, insetdepth, insetbrightness)
                //.AddItemSlotGrid(Inventory, SendInvPacket,2, outputSlotIds, outputslotbnd, "outputSlots")
                .AddItemSlotGrid(Inventory, SendInvPacket,1, new int[] {4}, outputslotbnd1, "outputSlots1")
                .AddItemSlotGrid(Inventory, SendInvPacket,1, new int[] {5}, outputslotbnd2, "outputSlots2")
                .AddInset(fuelslotinset, insetdepth, insetbrightness)
                .AddItemSlotGrid(Inventory, SendInvPacket, 1, new int[] { 0 }, fuelslotbnd, "fuelSlots")
                .AddInset(textareainset, insetdepth, insetbrightness)
                .AddDynamicText(GetTemperatureText(), leftyellow, blocktemptextbnds, "blockTemp")
                .AddDynamicText(GetFuelHours(), leftyellow, fuelhourtextbnds, "fuelHours")
                .EndChildElements()
                .Compose(true);
                
        }

        public void Update(float blocktemp, float fuelhours, float craftProgress)
        {
            _blockTemp = blocktemp;
            _fuelHours = fuelhours;
            _craftProgress = craftProgress;
            if (!IsOpened()) return;
            if (base.SingleComposer != null)
            {
                SingleComposer.GetDynamicText("blockTemp").SetNewText(GetTemperatureText());
                SingleComposer.GetDynamicText("fuelHours").SetNewText(GetFuelHours());
            }
            
        }

        private string GetFuelHours()
        {
            return Lang.Get("Fuel for {0:#.#} hours.", _fuelHours);
        }
        private string GetTemperatureText()
        {
            return Lang.Get("Temperature: {0}°C", (int)_blockTemp);
        }
        private void OnTitleBarClosed()
        {
            this.TryClose();
        }

        private void SendInvPacket(object obj)
        {
            capi.Network.SendBlockEntityPacket(BlockEntityPosition.X, BlockEntityPosition.Y, BlockEntityPosition.Z, obj);
        }

        public override void OnGuiOpened()
        {
            base.OnGuiOpened();
            Inventory.SlotModified += OnSlotModified;
        }
        private void OnSlotModified(int slotid)
        {
            capi.Event.EnqueueMainThreadTask(new Action(SetupDialog), "setupcalcinatordlg");
        }

        public override void OnGuiClosed()
        {
            Inventory.SlotModified -= OnSlotModified;
            //SingleComposer.GetSlotGrid("inputSlots").OnGuiClosed(capi);
            SingleComposer.GetSlotGrid("inputSlots1").OnGuiClosed(capi);
            SingleComposer.GetSlotGrid("inputSlots2").OnGuiClosed(capi);
            SingleComposer.GetSlotGrid("inputSlots3").OnGuiClosed(capi);
            //SingleComposer.GetSlotGrid("outputSlots").OnGuiClosed(capi);
            SingleComposer.GetSlotGrid("outputSlots1").OnGuiClosed(capi);
            SingleComposer.GetSlotGrid("outputSlots2").OnGuiClosed(capi);
            SingleComposer.GetSlotGrid("fuelSlots").OnGuiClosed(capi);
            
            base.OnGuiClosed();
            
        }
    }
}

