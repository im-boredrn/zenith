using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using zenith.Core.Adaptations;
using zenith.Core.AdaptationsCore;

namespace zenith.GUI
{
    public class AdaptationGUI : GuiDialog
    {
        public override string ToggleKeyCombinationCode => null;

        private readonly CreatureAdaptations creatureAdaptations;
        InventoryGeneric inv;
        public int packetIDOffset;

        public AdaptationGUI(ICoreClientAPI capi, CreatureAdaptations adaptations) : base (capi)
        {
            this.creatureAdaptations = adaptations;
            // Refresh on Adaptation Change

            creatureAdaptations.OnAdaptationChanged += () =>
            {
                RefreshAdaptation();
            };
            SetupDialog();
        }

        private void SetupDialog()
        {

            ElementBounds bounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.RightMiddle);


            double pad = GuiElementItemSlotGridBase.unscaledSlotPadding;
            ElementBounds slotBounds = ElementStdBounds.SlotGrid(EnumDialogArea.LeftBottom, pad, 40.0 + pad, 1, 1).FixedGrow(2.0 * pad, 2.0 * pad);

            SingleComposer = capi.Gui.CreateCompo("Adaptations", bounds)
                .AddShadedDialogBG(ElementBounds.Fill, true)
                .AddDialogTitleBar($"Adaptations", OnGuiClosed);
            inv = new InventoryGeneric(1, "assim-slot", capi, (id, self) => new ItemSlot(self));

            int i = 0;
            int spacing = 40;
            foreach (var adaptationFactory in creatureAdaptations.AdaptationManager.Values)
            {
                var adaptation = adaptationFactory.Invoke();

                var unlocked = creatureAdaptations.CreatureDefinitions[adaptation.SourceCreature].IsLocked;

                if (unlocked)
                {
                    SingleComposer.AddDynamicText($"{adaptation.AdaptationName}", CairoFont.WhiteSmallishText(),
                        ElementBounds.Fixed(20, 50 + (i * spacing), 200, 50), $"{adaptation.AdaptationName}K")
                .AddHoverText($"{adaptation.AdaptationDescription}",
                CairoFont.WhiteSmallishText(), 300, ElementBounds.Fixed(20, 50 + (i * spacing), 200, 30));
                }
                else
                {
                    SingleComposer.AddDynamicText($"[LOCKED]", CairoFont.WhiteSmallishText(),
                        ElementBounds.Fixed(20, 50 + (i * spacing), 200, 50), $"{adaptation.AdaptationName}L")
             .AddHoverText($"{adaptation.LockedDescription}",
             CairoFont.WhiteSmallishText(), 300, ElementBounds.Fixed(20, 50 + (i * spacing), 200, 30));
                }
            

               // Possibly Add Icons -- Like a pixel art image of food or teeth for Wolf Adaptation and A colored eye for bear Sense.
                i++;
            }
            SingleComposer.AddItemSlotGrid(inv, SendEntityPacket, 1, slotBounds); // Idk if thats the right packet.
            SingleComposer.Compose();
        }


        public void RefreshAdaptation()
        {
            if (!IsOpened()) return;

            foreach (var adaptationFactory in creatureAdaptations.AdaptationManager.Values)
            {
                var adaptation = adaptationFactory.Invoke();
                var unlocked = creatureAdaptations.CreatureDefinitions[adaptation.SourceCreature].IsLocked;
                string suffix = unlocked ? "K" : "L";

            
                
                    SingleComposer.GetDynamicText($"{adaptation.AdaptationName}{suffix}") // May be running IsUnlocked Twice 1
                        .SetNewText(BuildAdaptationText(adaptation), false, true , false);
              
            }


        }


        public string BuildAdaptationText(Adaptation adaptation)
        {
        
                var unlocked = creatureAdaptations.CreatureDefinitions[adaptation.SourceCreature].IsLocked;

                string text = unlocked ? $"{adaptation.AdaptationName}" : $"[Locked]";

                return text;
        }

        private void SendEntityPacket(object p)
        {

            var slot = inv[0];

           

            long entityid = capi.World.Player.Entity.EntityId;
            capi.Network.SendEntityPacketWithOffset(entityid, packetIDOffset, p);
            //Analyze Block Method() Send An Event
            if (slot.Empty)
            {
                return;
            }
            else
            {
                var stack = slot.Itemstack;
                ItemSent?.Invoke(stack);
            }
        }

        private void SendEvolvableAdaptation(Adaptation adaptation)
        {
            EvolveSelected?.Invoke(adaptation);
        }


        public override void OnGuiOpened()
        {

            base.OnGuiOpened();

            RefreshAdaptation();
        }

        public override void OnGuiClosed()
        {
        

            base.OnGuiClosed();
            creatureAdaptations.OnAdaptationChanged += () =>
            {
                RefreshAdaptation();
            };
            this.TryClose();
            this.Dispose();
        }
        public Action<Adaptation> EvolveSelected;
        public Action<ItemStack> ItemSent;
    }
}
