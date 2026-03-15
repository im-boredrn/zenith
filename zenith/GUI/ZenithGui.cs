using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using zenith.Config;
using zenith.Core;
using zenith.Core.Domains;
using static HarmonyLib.Code;
using static zenith.Core.ZenithBehavior;

namespace zenith.GUI
{
    public class ZenithGui : GuiDialog
    {

        public override string ToggleKeyCombinationCode => null;

        ProgressionManager progressionManager;
        DomainManager domainManager;
        DomainDetailsGUI DomainDetailsGUI;
        private Dictionary<DomainEnum, string> domainButtonIds = new();
        public ZenithGui(ICoreClientAPI capi, ProgressionManager progressionManager, DomainManager domainManager) : base(capi)
        {
            this.progressionManager = progressionManager;
            this.domainManager = domainManager;
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
            var Stage = zenith?.GetInt("Stage", 1) ?? 1;
            var DomainPoints = zenith?.GetInt("DomainPoints") ?? 0;

            string[] domainNames = domainManager.GetDomainNames();

            // Compute stage name based on the Stage int
            string stageName = progressionManager.GetStageName(Stage);

            int buttonWidth = 120;
            int buttonHeight = 30;
            int padding = 10;

            int totalButtons = domainManager.domains.Count;
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

            ElementBounds buttonBounds2 = buttonBounds.FlatCopy().FixedUnder(buttonBounds, 5);

            ElementBounds dropdownBounds = ElementBounds.Fixed(20, 60, 200, 35)
                .WithAlignment(EnumDialogArea.LeftTop);

            ClearComposers();

            // Composer builds the UI 
            SingleComposer = capi.Gui
                .CreateCompo("OrganismTracker", dialogBounds) // Then Chain Elements
                .AddShadedDialogBG(ElementBounds.Fill, true) // Everything is on Top of this
                .AddDialogTitleBar("Organism State", OnGuiClosed) // makes it Draggable

                .AddDynamicText($"Stage : {stageName}\nDomainPoints: {DomainPoints}", CairoFont.WhiteSmallishText(), numberBounds, "statstext"); // This should Stay since its an overall

            int i = 0;
            foreach (var kvp in domainManager.domains)
            {
               
                DomainEnum domain = kvp.Key;
                DomainSponge sponge = kvp.Value;

                string text = $"{domain}";

                if (sponge.IsMaxed)
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

            capi.Logger.Warning("[GUI] UpdateStats called");
            var stageName = progressionManager.GetStageName(progressionManager.Stage);
            string newText = $"Stage : {stageName}\nDomainPoints: {progressionManager.DomainPoints}/{ZenithSettings.ZStageUpRequirement}";
            capi.Logger.Warning($"[GUI] Stage seen by GUI: {progressionManager.Stage}");
            capi.Logger.Warning($"[GUI] DomainPoints seen by GUI: {progressionManager.DomainPoints}");
            capi.Logger.Warning($"[GUI] Before: Current stageName {stageName}");
            SingleComposer.GetDynamicText("statstext")?
                          .SetNewText(newText,false,true,false);
            capi.Logger.Warning($"[GUI] After: Current stageName {stageName}");
            capi.Logger.Warning($"[GUI] Stage seen by GUI: {progressionManager.Stage}");
            capi.Logger.Warning($"[GUI] DomainPoints seen by GUI: {progressionManager.DomainPoints}");
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
            capi.Logger.Warning("Domain button clicked: " + domain);

            return true;

          
        }

    
        public override void OnGuiOpened()
        {
            capi.Logger.Notification("Dialog opened");

            SingleComposer.Dispose();
            capi.Logger.Notification("Disposing...");
            SetupDialog();   // rebuild UI from fresh data
            UpdateStats(); // Pull latest stage/domain values
            

        }

        public override void OnGuiClosed()
        {
            this.TryClose();
            progressionManager.OnProgressionChanged -= UpdateStats;
            DomainDetailsGUI? .TryClose(); 
            capi.Logger.Notification("Dialog closed");
        }
        private void Log(string message)
        {
            if (!DebugMode) return;
            entity.World.Logger.Warning(message);
        }

    }
}
