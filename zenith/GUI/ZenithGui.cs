using System.Collections.Generic;
using Vintagestory.API.Client;
using zenith.Config;
using zenith.Core.Assimilation;
using zenith.Core.Domains;
using zenith.Core.Progression;
using static zenith.Core.ZenithBehavior;

namespace zenith.GUI
{
    public class ZenithGui : GuiDialog
    {
        public static bool DebugMode => ZenithSettings.ZDebugMode;

        public override string ToggleKeyCombinationCode => null;

        DomainManager domainManager;
        private readonly IStageProvider stageProvider;
        private AssimilationCore AssimilationCore;
        DomainDetailsGUI DomainDetailsGUI;
        public BonusGUI BonusGUI;
        LevelGUI LevelGUI;
        private readonly StatOutput StatOutut;
        private Dictionary<DomainEnum, string> domainButtonIds = new();
        public ZenithGui(ICoreClientAPI capi, ProgressionManager progressionManager, DomainManager domainManager, AssimilationCore assimilationCore, StatOutput statOutput) : base(capi)
        {
            this.stageProvider = progressionManager;
            this.domainManager = domainManager;
            this.AssimilationCore = assimilationCore;
            this.StatOutut = statOutput;
            progressionManager.OnProgressionChanged += UpdateStats;
            SetupDialog();

        }


        // Dialog Cycle
        // 1. Create Dialog
        // 2. Setup Layout
        // 3. Open() 
        // 4. User Interation
        // 5. Close()
        public void SetupDialog()
        {
            var zenith = capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("zenith");
            var Stage = zenith?.GetInt("Stage", 1);
            var StageName = zenith?.GetString("StageName", stageProvider.GetStageName()); 
            var DomainPoints = zenith?.GetInt("DomainPoints") ?? 0;

            string[] domainNames = domainManager.GetDomainNames();

            // Compute stage name based on the Stage int

            int buttonWidth = 120;
            int buttonHeight = 30;
            int padding = 10;

            int totalButtons = domainManager.Domains.Count;
            int totalWidth = totalButtons * (buttonWidth + padding) - padding;

            // Start X so the row is centered
            int startX = (600 - totalWidth) / 2; // 600 = dialog width
            int buttonsPerRow = 2;

            int dialogHeight = 600; 

            int startY = dialogHeight - 50; // 50 px from bottom for first row

            ElementBounds dialogBounds =
     ElementBounds.Fixed(0, 0, 600, 600)
     .WithAlignment(EnumDialogArea.CenterMiddle);



            ElementBounds textBounds =
       ElementBounds.Fixed(0, 30, 250, 30)
       .WithAlignment(EnumDialogArea.CenterTop);

            ElementBounds numberBounds =
                ElementBounds.Fixed(0, 0, 300, 80)
                .WithAlignment(EnumDialogArea.CenterMiddle);

            ElementBounds buttonBounds =
                ElementBounds.Fixed(0, 0, 120, 30)
                .WithAlignment(EnumDialogArea.RightBottom);

            ElementBounds buttonBounds2 = buttonBounds.FlatCopy().FixedUnder(buttonBounds, -70);

            ElementBounds dropdownBounds = ElementBounds.Fixed(20, 60, 200, 35)
                .WithAlignment(EnumDialogArea.LeftTop);

            ClearComposers();

            // Composer builds the UI 
            SingleComposer = capi.Gui
                .CreateCompo("OrganismTracker", dialogBounds) // Then Chain Elements
                .AddShadedDialogBG(ElementBounds.Fill, true) // Everything is on Top of this
                .AddDialogTitleBar("Organism State", OnGuiClosed) // makes it Draggable
                 .AddButton("Bonuses", () => OnShowBonuses(), buttonBounds, EnumButtonStyle.Small)
                 .AddButton("Levels", () => OnShowLevels(), buttonBounds2, EnumButtonStyle.Small)
                .AddDynamicText($"Stage : {StageName}\nDomainPoints : {DomainPoints}", CairoFont.WhiteSmallishText(), numberBounds, "statstext");

            int i = 0;
            foreach (var kvp in domainManager.Domains)
            {
               
                DomainEnum domain = kvp.Key;
                IDomainInfo sponge = kvp.Value;

                string text = $"{domain}";

                if (sponge.IsDMaxed())
                    text += "MAX";

                int row = i / buttonsPerRow;
                int col = i % buttonsPerRow;

                int colX = padding + col * (buttonWidth + padding); // horizontal position
                int rowY = startY - row * (buttonHeight + padding);  // vertical position

                ElementBounds bounds = ElementBounds.Fixed(colX, rowY, buttonWidth, buttonHeight);
                string buttonId = $"domainbtn_{domain}";

                SingleComposer.AddButton(text, () => OnDomainButton(domain), bounds, EnumButtonStyle.Small, buttonId);

                // Store ID for live updates
                domainButtonIds[domain] = buttonId;
                i++;     
               
            }
                SingleComposer.Compose(); // Finalize
        }


       public void UpdateStats() 
        {
            if (SingleComposer == null) return;
            Log("[GUI] UpdateStats called");

            var zenith = capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("zenith");
            var Stage = zenith?.GetInt("Stage", 1);
            var StageName = zenith?.GetString("StageName", stageProvider.GetStageName()); 
            var DomainPoints = zenith?.GetInt("DomainPoints") ?? 0;
            string newText = $"Stage : {StageName}\nDomainPoints: {DomainPoints}/{ZenithSettings.ZStageUpRequirement}";

            Log($"[GUI] Stage seen by GUI: {Stage}");
            Log($"[GUI] DomainPoints seen by GUI: {DomainPoints}");
            Log($"[GUI] Before: Current stageName {StageName}");
            SingleComposer.GetDynamicText("statstext")?
                          .SetNewText(newText,false,true,false);
            Log($"[GUI] After: Current stageName {StageName}");
            Log($"[GUI] Stage seen by GUI: {Stage}");
            Log($"[GUI] DomainPoints seen by GUI: {DomainPoints}");
        }

        bool OnDomainButton(DomainEnum domain)
        {


            if (DomainDetailsGUI != null)
            {
                DomainDetailsGUI.TryClose();
                DomainDetailsGUI.Dispose();
            }

            DomainDetailsGUI = new DomainDetailsGUI(capi, domainManager, domain);
            DomainDetailsGUI.TryOpen();
            Log("Domain button clicked: " + domain);

            return true;

          
        }

        private bool OnShowBonuses()
        {
            // Display the Domains Stat Boosts

            if (BonusGUI != null)
            {
                BonusGUI.TryClose();
                BonusGUI.Dispose(); // Previously this line was Tryopen - caused BlackBox Issue
            }

            BonusGUI = new BonusGUI(capi, stageProvider, AssimilationCore, StatOutut);
            BonusGUI.TryOpen();
            Log("Bonus GUI Button Clicked:");

            return true;
        }

        private bool OnShowLevels()
        {
            if (LevelGUI != null)
            {
                LevelGUI.TryClose();
                LevelGUI.Dispose();
            }

            LevelGUI = new LevelGUI(capi, AssimilationCore);
            LevelGUI.TryOpen();

            return true;
        }


        public override void OnGuiOpened()
        {
            Log("Dialog opened");

            SingleComposer.Dispose();
            Log("Disposing...");
            SetupDialog();   // rebuild UI from fresh data
            UpdateStats(); // Pull latest stage/domain values
            

        }

        public override void OnGuiClosed()
        {
            this.TryClose();
            DomainDetailsGUI? .TryClose(); 
           Log("Dialog closed");
        }
        private void Log(string message)
        {
            if (!DebugMode) return;
            capi.World.Logger.Warning(message);
        }

    }
}
