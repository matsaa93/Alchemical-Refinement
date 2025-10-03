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
            
            ElementBounds dialogBounds = ElementBounds.Fixed(320, 218 + titlebarheight);
            ElementBounds dialog = ElementBounds.Fill.WithFixedPadding(0);
            dialog.BothSizing = ElementSizing.FitToChildren;
            
            ElementBounds titlebarbounds = ElementBounds.Fixed(0, 0, 320,titlebarheight); 
            ElementBounds inputslotinset = ElementBounds.Fixed(5, 8 + titlebarheight, 164, 58);
            ElementBounds outputslotinset = ElementBounds.Fixed(204, 8 + titlebarheight, 111, 58);
            ElementBounds fuelslotinset = ElementBounds.Fixed(58, 155 + titlebarheight, 58, 58);
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
                outputslotinset,
                fuelslotinset,
                textareainset,
                //blocktemptextinset,
                blocktemptextbnds,
                //fuelhourtextinset,
                fuelhourtextbnds
            });
            double[] yellow = new double[3] { 1, 1, 0 };
            CairoFont leftyellow = CairoFont.WhiteDetailText().WithWeight(FontWeight.Normal).WithOrientation(EnumTextOrientation.Left).WithColor(yellow);
            ElementBounds windowBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle)
                .WithFixedAlignmentOffset(-GuiStyle.DialogToScreenPadding, 0);

            BlockPos blockPos = base.BlockEntityPosition;

            this.SingleComposer = capi.Gui.CreateCompo("arcalcinatordlg" + blockPos?.ToString(), windowBounds)
                .AddShadedDialogBG(dialog, true, 5)
                .AddDialogTitleBar(DialogTitle, OnTitleBarClosed)
                .BeginChildElements(dialog)
                .AddInset(inputslotinset, insetdepth, insetbrightness)
                .AddInset(outputslotinset, insetdepth, insetbrightness)
                .AddInset(fuelslotinset, insetdepth, insetbrightness)
                .AddInset(textareainset, insetdepth, insetbrightness)
                .AddDynamicText(GetTemperatureText(), leftyellow, blocktemptextbnds, "blockTemp")
                .AddDynamicText(GetFuelHours(), leftyellow, fuelhourtextbnds, "fuelHours")
                .EndChildElements()
                .Compose(true);
                
        }

        public void Update(float blocktemp, float fuelhours, float craftProgress)
        {
            if (!IsOpened()) return;
            _blockTemp = blocktemp;
            _fuelHours = fuelhours;
            _craftProgress = craftProgress;
            
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
    }
}

