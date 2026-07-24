using Cairo;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.Assimilation;
using zenith.Core.Domains;
using zenith.Core.NetWork;
using zenith.Core.Progression;
using static zenith.Core.NetWork.StatOutput;
using static zenith.Core.ZenithBehavior;

namespace zenith.GUI
{
    public class BonusGUI : GuiDialog
    {
        public static bool DebugMode => ZenithSettings.ZDebugMode;
        private readonly IStageProvider stageProvider;
        private readonly AssimilationCore assimilationCore;
        public override string ToggleKeyCombinationCode => null;
        private readonly StatOutput statOutput;

        public BonusGUI(ICoreClientAPI capi, IStageProvider stageP, AssimilationCore assimilationCore, StatOutput statOutput) : base(capi)
        {

            this.stageProvider = stageP;
            this.assimilationCore = assimilationCore;
            this.statOutput = statOutput;
          
            SetupDialog();
        }

        public void SetupDialog()
        {

            ElementBounds dialogBounds =
    ElementBounds.Fixed(0, 0, 300, 300)
    .WithAlignment(EnumDialogArea.CenterMiddle);

            var bounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);

            SingleComposer = capi.Gui.CreateCompo("Bonuses", bounds)
                .AddShadedDialogBG(ElementBounds.Fill, true)
                .AddDialogTitleBar($" Bonuses", OnGuiClosed)
                .AddDynamicText(BuildBonusText(),
                CairoFont.WhiteSmallishText(), ElementBounds.Fixed(20, 50, 300, 300), "Bonustext")
                .Compose();

        }

        public void UpdateBonusStats()
        {
            if (!IsOpened()) return;

            if (SingleComposer != null)
            {
                SingleComposer.GetDynamicText("Bonustext")
                    .SetNewText(BuildBonusText(), false, true, false);
            }
        }





        private string BuildBonusText()
        {

            var zenith = capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("zenith"); // Method for live updating
            // Idk if any better methods exist

            var speed = zenith?.GetFloat("SPD", 0f);
            var dmg = zenith?.GetFloat("Dmg", 0f); // Clean Up Keys Man...
            var jHeight = zenith?.GetFloat("JHM", 0f);

            var health = zenith?.GetFloat("MHP", 0f);
            var aNLOOT = zenith?.GetFloat("ALD", 0f); // Clean Up Variables
            var harvest = zenith?.GetFloat("AHT", 0f);


            var selectedStat = (StatType)zenith.GetInt("SelectedStat", 0);

            var speedOutput = zenith?.GetFloat($"Speed Output", 100f);
            var damageOutput = zenith?.GetFloat($"Strength Output", 100f);
            var jumpOutput = zenith?.GetFloat($"Jump Output", 100f);

            var healthOutput = zenith?.GetFloat("Health Output", 100f);
            var aNLootOutput = zenith?.GetFloat("ANLoot Output", 100f);
            var harvestingOutput = zenith?.GetFloat("Harvesting Output", 100f);

            //  Log($"Raw Jump Height = {jHeight}");

            //  Log($"[CLIENT CHECK] Stats directly from entity");
            //   Log($"[FLOW] UpdateBonusStatsCalled! Current Speed : {speed} | Current Damage: {dmg} | ");

            //  Log($"[DomainGUI] UpdateBonusStatsFinished! Current Speed : {speed} | Current Damage: {dmg} | Current Jump Height {jHeight} ");


            return $"+{speed:F0}% Speed | {speedOutput}% Output\n +{dmg:F0}% Damage |" +
                $" {damageOutput}% Output\n +{jHeight:F0}% Jump | {jumpOutput}% Output\n" +
                $" +{health:F0}% Health | {healthOutput}% Output\n +{aNLOOT:F0}% ANLoot | {aNLootOutput}% Output\n" +
                $"+{harvest:F0}% Harvest | {harvestingOutput}% Output\n Selected Stat : {selectedStat} ";
        }

      
        public override void OnGuiOpened()
        {

            base.OnGuiOpened(); 

            UpdateBonusStats();
        }

        public override void OnGuiClosed()
        {
            base.OnGuiClosed();
            this.TryClose();
            this.Dispose();
        }

        private void Log(string message)
        {
            if (!DebugMode) return;
            capi.World.Logger.Warning(message);
        }
    }
}