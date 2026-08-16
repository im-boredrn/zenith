using System.Collections.Generic;
using Vintagestory.API.Client;
using zenith.Config;
using zenith.Core.AdaptationsCore;
using zenith.Core.Assimilation;
using zenith.Core.Domains;
using zenith.Core.Inventory;
using zenith.Core.NetWork;
using zenith.Core.Progression;
using static zenith.Core.ZenithBehavior;

namespace zenith.GUI
{
    public  class ZenithGui : GuiDialog
    {
        public static bool DebugMode => ZenithSettings.ZDebugMode;

        public override string ToggleKeyCombinationCode => null;

        private readonly DomainManager domainManager;
        private readonly AssimilationCore AssimilationCore;
        DomainDetailsGUI DomainDetailsGUI;
        public BonusGUI BonusGUI;
        LevelGUI LevelGUI;
        public AdaptationGUI AdaptationGUI;
        private readonly StatOutput StatOutut;
        private readonly Adaptations creatureAdaptations;
        private readonly Dictionary<DomainEnum, string> domainButtonIds = [];
        private readonly AssimilationInventory inventory;
        public ZenithGui(ICoreClientAPI capi,   DomainManager domainManager, AssimilationCore assimilationCore, StatOutput statOutput,
            Adaptations adaptations, AssimilationInventory inv) : base(capi)
        {
            this.domainManager = domainManager;
            this.AssimilationCore = assimilationCore;
            this.StatOutut = statOutput;
            this.creatureAdaptations = adaptations;
            this.inventory = inv;
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
            var StageName = zenith?.GetString("StageName", "Adapting Organism"); 
            var DomainPoints = zenith?.GetInt("DomainPoints") ?? 0;


            // Compute stage name based on the Stage int

            int buttonWidth = 120;
            int buttonHeight = 30;
            int padding = 10;


            // Start X so the row is centered
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

            ElementBounds buttonBounds3 = buttonBounds2.FlatCopy().FixedUnder(buttonBounds, -70);


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
                 .AddButton("Adaptations", () => OnShowAdaptations(), buttonBounds3, EnumButtonStyle.Small) // Eventual Horizontal Tabs for Levels and Bonuses
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
            var StageName = zenith?.GetString("StageName", "Adapting Organism"); 
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
            var zenithNetwork = capi.ModLoader.GetModSystem<ZenithCore>().ZenithNetwork;

            BonusGUI = new BonusGUI(capi,  StatOutut, zenithNetwork);
            BonusGUI.TryOpen();

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

        private bool OnShowAdaptations()
        {
            var zenithNetwork = capi.ModLoader.GetModSystem<ZenithCore>().ZenithNetwork;

            if (AdaptationGUI != null)
            {
                zenithNetwork.RequestCloseAssimInventory();
                capi.World.Player.InventoryManager.CloseInventoryAndSync(inventory);

                AdaptationGUI.TryClose();
                AdaptationGUI.Dispose();
            }
            AdaptationGUI = new AdaptationGUI(capi, creatureAdaptations, inventory,zenithNetwork);

            zenithNetwork.RequestOpenAssimInventory();
            capi.World.Player.InventoryManager.OpenInventory(inventory);

            AdaptationGUI.TryOpen();

            return true;
        }


        public override void OnGuiOpened()
        {
            Log("Dialog opened");
            base.OnGuiOpened();
            SingleComposer.Dispose();
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
