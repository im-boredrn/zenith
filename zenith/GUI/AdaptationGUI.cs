using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
using zenith.Core;
using zenith.Core.AdaptationsCore;
using zenith.Core.Definitions;
using zenith.Core.Helper;
using zenith.Core.Inventory;
using zenith.Core.NetWork;

namespace zenith.GUI
{
    public sealed class AdaptationGUI : GuiDialog
    {
        public override string ToggleKeyCombinationCode => null;

        private readonly AssimilationInventory inventory;
        private readonly ZenithNetwork zenithNetwork;
        private readonly ZenithData zenithData;
        public AdaptationGUI(ICoreClientAPI capi , AssimilationInventory assimInventory, ZenithNetwork zenithNetwork, ZenithData data) : base (capi)
        {
            this.inventory = assimInventory;
            this.zenithNetwork = zenithNetwork;
            this.zenithData = data;

        
            SetupDialog();

           // Events need to Be initialized in pocket GUI or they won't stick. 
            //#TODO Fix Other GUI events | Maybe idk atp. If the game compiles later delete this

        }

        private void SetupDialog()
        {

            ElementBounds bounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.RightMiddle);

            ElementBounds dialogBounds =
    ElementBounds.Fixed(0, 0, 200, 300)
    .WithAlignment(EnumDialogArea.RightMiddle);

            ElementBounds buttonBounds =
            ElementBounds.Fixed(0, 0, 30, 30)
            .WithAlignment(EnumDialogArea.CenterBottom);

            double pad = GuiElementItemSlotGridBase.unscaledSlotPadding;
            ElementBounds slotBounds = ElementStdBounds.SlotGrid(EnumDialogArea.LeftBottom, pad, 0 + pad, 1, 1).FixedGrow(2.0 * pad, 2.0 * pad);

            SingleComposer = capi.Gui.CreateCompo("Adaptations", dialogBounds)
                .AddShadedDialogBG(ElementBounds.Fill, true)
                .AddDialogTitleBar($"Adaptations", OnGuiClosed);
            var adaptations = zenithData.Tree.GetTreeAttribute("Adaptations");
             
            int i = 0;
            int spacing = 40;

            foreach (AdaptationsEnum.AdaptationEnum adaptation in Enum.GetValues<AdaptationsEnum.AdaptationEnum>())
            {

                var adaptationTree = adaptations?.GetTreeAttribute($"{adaptation.ToString()}State");

                if (adaptationTree == null) continue;

                bool unlocked =
                    adaptationTree?.GetBool($"{adaptation.ToString()}State IsUnlocked", false) ?? false;


                string text = unlocked
                   ? AdaptationDefinitions.GetName(adaptation)
                   : "[LOCKED]";

                string suffix = unlocked
                    ? "K"
                    : "L";



                SingleComposer.AddDynamicText($"{text}", CairoFont.WhiteSmallishText(),
                        ElementBounds.Fixed(20, 50 + (i * spacing), 200, 50), $"{AdaptationDefinitions.GetName(adaptation)}{suffix}")
                .AddHoverText(unlocked ? AdaptationDefinitions.GetDescription(adaptation) : AdaptationDefinitions.GetLockedDescription(adaptation),
                CairoFont.WhiteSmallishText(), 300, ElementBounds.Fixed(20, 50 + (i * spacing), 100, 30));

                // Possibly Add Icons -- Like a pixel art image of food or teeth for Wolf Adaptation and A colored eye for bear Sense.
                i++;
            }
          
            SingleComposer.AddItemSlotGrid(inventory, SendEntityPacket, 1, slotBounds);
            SingleComposer.AddButton("Eat", () => OnSubmitAssim(), buttonBounds, EnumButtonStyle.Small);
            SingleComposer.Compose();
        }

        
        public void RefreshAdaptation()
        {
            if (!IsOpened()) return;

            var adaptations = zenithData.Tree.GetTreeAttribute("Adaptations");


            foreach (AdaptationsEnum.AdaptationEnum adaptation in Enum.GetValues<AdaptationsEnum.AdaptationEnum>())
            {
                var adaptationTree = adaptations?.GetTreeAttribute(adaptation.ToString());

                bool unlocked =
                     adaptationTree?.GetBool($"{adaptation.ToString()}State IsUnlocked", false) ?? false;

                string suffix = unlocked ? "K" : "L";


                string text = unlocked
                    ? AdaptationDefinitions.GetName(adaptation)
                    : "[LOCKED]";

                if (SingleComposer.GetDynamicText($"{AdaptationDefinitions.GetName(adaptation)}{suffix}") is GuiElementDynamicText elementDynamicText)
                {
                    elementDynamicText.SetNewText(text, false, true, false);
                }
             
            }

        }

        private void SendEntityPacket(object p)
        {
          //  ObjectReader.DumpObject(capi.World.Player.Entity, p, 2);
            capi.Network.SendPacketClient( p);
        }
        
       

        public bool OnSubmitAssim()
        {
            zenithNetwork.RequestSubmitItem();
            RefreshAdaptation();
            return true;
        }


        public override void OnGuiOpened()
        {

            base.OnGuiOpened();

            RefreshAdaptation();
        }

        public override void OnGuiClosed()
        {
        

            base.OnGuiClosed();
           

          
            capi.World.Player.InventoryManager.CloseInventoryAndSync(inventory);

            this.TryClose();
            this.Dispose();
        }

        

    }
}
