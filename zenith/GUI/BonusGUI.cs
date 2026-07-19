//using Cairo;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Vintagestory.API.Client;
//using zenith.Config;
//using zenith.Core.Domains;
//using zenith.Core.Progression;
//using static zenith.Core.ZenithBehavior;

//namespace zenith.GUI
//{
//    public class BonusGUI : GuiDialog
//    {
//        DomainEnum domain;  // <- the domain this GUI is showing
//        readonly DomainManager domainManager;
//        private readonly IDomainInfo domainInfo;
//        public static bool DebugMode => ZenithSettings.ZDebugMode;
//        private readonly IStageProvider stageProvider;
//        public override string ToggleKeyCombinationCode => null;


//        public BonusGUI(ICoreClientAPI capi, IStageProvider stageP) : base(capi)
//        {

//            this.stageProvider = stageP;

//            stageProvider.OnStageUp += () =>
//            {

//                UpdateBonusStats();
//            };

//            SetupDialog();
//        }

//        public void SetupDialog()
//        {

//            ElementBounds dialogBounds =
//    ElementBounds.Fixed(0, 0, 300, 300)
//    .WithAlignment(EnumDialogArea.CenterMiddle);

//            var bounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);

//         //   var speed = stageProvider.GetSpeedMultiplier();
//        //   var dmg = stageProvider.GetDamageMultiplier();
//            var mSpeed = stageProvider.GetMiningSpeedMultiplier();
//            // var jHeight = stageProvider.GetJumpHeightMultiplier();

//            SingleComposer = capi.Gui.CreateCompo("Bonuses", bounds)
//                .AddShadedDialogBG(ElementBounds.Fill, true)
//                .AddDialogTitleBar($"Domain Bonuses", OnGuiClosed)
//                .AddDynamicText($"Speed : +{speed}X\n Damage : +{dmg}X\n MiningSpeed : +{mSpeed}X ",
//                CairoFont.WhiteSmallishText(), ElementBounds.Fixed(20, 50, 300, 300), "Bonustext")
                

               

//                .Compose();

//        }


//        private void UpdateBonusStats()
//        {
//            if (!IsOpened()) return;


//            var zenith = capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("zenith"); // Method for live updating
//            // Idk if any better methods exist

//            var speed = zenith?.GetFloat("Speed", 0f ); 
//            var dmg = zenith?.GetFloat("Dmg", 0f);
//            var mSpeed = zenith?.GetFloat("MSM", 0f);
//         //   var jHeight = zenith?.GetFloat("JHM" , 0f);




//            string newText = $"Speed : +{speed}X\n Damage : +{dmg}X\n MiningSpeed : +{mSpeed}X ";


//            Log($"[CLIENT CHECK] Stats directly from entity");
//            Log($"[FLOW] UpdateBonusStatsCalled! Current Speed : {speed} | Current Damage: {dmg} |" +
//                $" Current Mining Speed : {mSpeed} ");


//            if (SingleComposer != null)
//            {
//                SingleComposer.GetDynamicText("Bonustext")
//                    .SetNewText(newText, false, true, false);
//            }
//            Log($"[DomainGUI] UpdateBonusStatsFinished! Current Speed : {speed} | Current Damage: {dmg} |" +
//                $"Current Mining Speed : {mSpeed} ");


//        }


//        public override void OnGuiOpened()
//        {

//            base.OnGuiOpened(); // Recently added 

//            UpdateBonusStats();
//        }

//        public override void OnGuiClosed()
//        {

//            stageProvider.OnStageUp -= () =>
//            {
//                UpdateBonusStats();
//            };


//            base.OnGuiClosed();
//            this.TryClose();
//            this.Dispose();
//        }

//        private void Log(string message)
//        {
//            if (!DebugMode) return;
//            capi.World.Logger.Warning(message);
//        }

//    }

     

//    }
