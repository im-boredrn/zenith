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
    public class AdaptationGUI : GuiDialog
    {
        public override string ToggleKeyCombinationCode => null;

        private readonly Adaptations Adaptations;
        public int packetIDOffset;
        private readonly AssimilationInventory inventory;
        private readonly ZenithNetwork zenithNetwork;
        public AdaptationGUI(ICoreClientAPI capi, Adaptations adaptations, AssimilationInventory assimInventory, ZenithNetwork zenithNetwork) : base (capi)
        {
            this.Adaptations = adaptations;
            this.inventory = assimInventory;
            this.zenithNetwork = zenithNetwork;
            // Refresh on Adaptation Change

            Adaptations.OnAdaptationChanged += () =>
            {
                RefreshAdaptation();
            };
            SetupDialog();

            ItemSent  += (stack) => // Should move to server side | maybe through packet
            {
                Adaptations.EatItem(stack);
            }; // Events need to Be initialized in pocket GUI or they will be null. 
            //TODO: Fix Other GUI events

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
            var zenith = capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("zenith");
            
            int i = 0;
            int spacing = 40;
            foreach (var adaptationFactory in Adaptations.FullAdaptations.Values)
            {
                var def = adaptationFactory.Definitions;
                var state = adaptationFactory.State;
                bool unlocked =  state.IsUnlocked;

                string text = unlocked
                    ? def.AdaptationName
                    : "[LOCKED]";

                string suffix = unlocked 
                    ? "K" 
                    : "L";



                SingleComposer.AddDynamicText($"{text}", CairoFont.WhiteSmallishText(),
                        ElementBounds.Fixed(20, 50 + (i * spacing), 200, 50), $"{def.AdaptationName}{suffix}")
                .AddHoverText(unlocked ? def.AdaptationDescription : def.LockedDescription,
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

            foreach (var adaptationFactory in Adaptations.FullAdaptations.Values)
            {
                var def = adaptationFactory.Definitions;
                var state = adaptationFactory.State;
             
                bool unlocked = state.IsUnlocked;
                string suffix = unlocked ? "K" : "L";


                string text = unlocked
                    ? def.AdaptationName
                    : "[LOCKED]";

                if (SingleComposer.GetDynamicText($"{def.AdaptationName}{suffix}") is GuiElementDynamicText elementDynamicText)
                {
                    elementDynamicText.SetNewText(text, false, true, false);
                }
             
            
              
            }


        }


        //public string BuildAdaptationText(Adaptation adaptation)
        //{
        
        //        var unlocked = adaptation.IsUnlocked;

        //        string text = unlocked ? $"{adaptation.AdaptationName}" : $"[Locked]";

        //        return text;
        //}

        private void SendEntityPacket(object p)
        {
            long entityid = capi.World.Player.Entity.EntityId;
          //  ObjectReader.DumpObject(capi.World.Player.Entity, p, 2);
            capi.Network.SendPacketClient( p);
        }
        
        private void SendEvolvableAdaptation(AdaptationDefinitions adaptation)
        {
            EvolveSelected?.Invoke(adaptation);
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
            Adaptations.OnAdaptationChanged -= () =>
            {
                RefreshAdaptation();
            };

            ItemSent -= (stack) =>
            {
                Adaptations.EatItem(stack);
            };
            capi.World.Player.InventoryManager.CloseInventoryAndSync(inventory);

            this.TryClose();
            this.Dispose();
        }

        

        public Action<AdaptationDefinitions> EvolveSelected;
        public Action<ItemStack> ItemSent;
    }
}
