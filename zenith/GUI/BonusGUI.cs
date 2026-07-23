using Cairo;
using System;
using System.Collections.Generic;
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
            stageProvider.OnStageUp += () =>
            {

                UpdateBonusStats();
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
            var selectedStat = (StatType)zenith.GetInt("SelectedStat", 0);
            var speedOutput = statOutput.OutputPercentages[StatType.Speed];
            var damageOutput = statOutput.OutputPercentages[StatType.Strength];
            var jumpOutput = statOutput.OutputPercentages[StatType.Jump];

            SingleComposer = capi.Gui.CreateCompo("Bonuses", bounds)
                .AddShadedDialogBG(ElementBounds.Fill, true)
                .AddDialogTitleBar($" Bonuses", OnGuiClosed)
                .AddDynamicText($"+{speed:F0}% Speed | {speedOutput}% Output\n +{dmg:F0}% Damage |" +
                $" {damageOutput}% Output\n +{jHeight:F0}% Jump | {jumpOutput}% Output\n Selected Stat : {selectedStat} ",
                CairoFont.WhiteSmallishText(), ElementBounds.Fixed(20, 50, 300, 300), "Bonustext")




                .Compose();

        }


        public void UpdateBonusStats()
        {
            if (!IsOpened()) return;


            var zenith = capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("zenith"); // Method for live updating
            // Idk if any better methods exist

            var speed = zenith?.GetFloat("SPD", 0f);
            var dmg = zenith?.GetFloat("Dmg", 0f);
              var jHeight = zenith?.GetFloat("JHM" , 0f);
            var selectedStat = (StatType)zenith.GetInt("SelectedStat",0);

             var speedOutput = zenith?.GetFloat($"Speed Output", 100f);
            var damageOutput = zenith?.GetFloat($"Strength Output", 100f);
            var jumpOutput = zenith?.GetFloat($"Jump Output", 100f);

            Log($"Raw Jump Height = {jHeight}");


            string newText = $"+{speed:F0}% Speed | {speedOutput}% Output\n +{dmg:F0}% Damage |" +
                $" {damageOutput}% Output\n +{jHeight:F0}% Jump | {jumpOutput}% Output\n Selected Stat : {selectedStat} ";


            Log($"[CLIENT CHECK] Stats directly from entity");
            Log($"[FLOW] UpdateBonusStatsCalled! Current Speed : {speed} | Current Damage: {dmg} | ");


            if (SingleComposer != null)
            {
                SingleComposer.GetDynamicText("Bonustext")
                    .SetNewText(newText, false, true, false);
            }
            Log($"[DomainGUI] UpdateBonusStatsFinished! Current Speed : {speed} | Current Damage: {dmg} | Current Jump Height {jHeight} ");


        }


        public override void OnGuiOpened()
        {

            base.OnGuiOpened(); // Recently added 

            UpdateBonusStats();
        }

        public override void OnGuiClosed()
        {

            stageProvider.OnStageUp -= () =>
            {
                UpdateBonusStats();
            };


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
