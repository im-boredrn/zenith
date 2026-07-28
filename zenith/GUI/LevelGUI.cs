using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using zenith.Core.Assimilation;
using zenith.Core.Progression;

namespace zenith.GUI
{
    internal class LevelGUI : GuiDialog
    {

        public override string ToggleKeyCombinationCode => null;

        private readonly AssimilationCore assimilationCore;
        public LevelGUI(ICoreClientAPI capi, AssimilationCore assimilationCore) : base(capi)
        {
            this.assimilationCore = assimilationCore;

            assimilationCore.OnAssimChanged += () =>
            {
                RefreshLevels();
            };

            SetupDialog();
        }

        public void SetupDialog()
        {

            ElementBounds dialogBounds =
    ElementBounds.Fixed(0, 0, 300, 300)
    .WithAlignment(EnumDialogArea.CenterMiddle);

            var bounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);

            var zenith = capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("zenith"); // Method for live updating
            var speed = zenith?.GetFloat("Speed", 0f);
            var dmg = zenith?.GetFloat("Dmg", 0f);
            var jHeight = zenith?.GetFloat("JHM", 0f);
            


            SingleComposer = capi.Gui.CreateCompo("Adaptation Levels", bounds)
                .AddShadedDialogBG(ElementBounds.Fill, true)
                .AddDialogTitleBar($" Assimilation Levels", OnGuiClosed);

            int i = 0;
            int spacing = 20;
            foreach (var kvp in assimilationCore.Definitions.Where(kvp => !kvp.Value.IsUnknown))
            {
                var key = kvp.Key;
                var trait = kvp.Value;

                var assimLevel = zenith?.GetInt($"{key}LVL", 0);
                string text = $"{trait.EntityName} lvl: {assimLevel}";
                string textID = $"Creature{trait.EntityName}";

                SingleComposer.AddDynamicText(text,
              CairoFont.WhiteSmallishText(), ElementBounds.Fixed(20,50 + (i * spacing) , 300, 300), textID);

                i++;
            }
              

                SingleComposer.Compose();
        }

        public void RefreshLevels()
        {

            if (!IsOpened()) return;
            var zenith = capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("zenith"); // Method for live updating

            foreach (var kvp in assimilationCore.Definitions.Where(kvp => !kvp.Value.IsUnknown))
            {

                var key = kvp.Key;
                var trait = kvp.Value;

                var assimLevel = zenith?.GetInt($"{key}LVL", 0);

                string text = $"{trait.EntityName} lvl: {assimLevel}";
                string textID = $"Creature{trait.EntityName}";

                if (SingleComposer != null)
                {
                    SingleComposer.GetDynamicText(textID)
                        .SetNewText(text, false, true, false);
                }
            }
        }

        public override void OnGuiOpened()
        {

            base.OnGuiOpened(); // Recently added 

            RefreshLevels();
        }

        public override void OnGuiClosed()
        {

            assimilationCore.OnAssimChanged -= () =>
            {
                RefreshLevels();
            };


            base.OnGuiClosed();
            this.TryClose();
            this.Dispose();
        }


    }
}
