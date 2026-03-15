using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
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
        public ZenithGui(ICoreClientAPI capi, ProgressionManager progressionManager, DomainManager domainManager) : base(capi)
        {
            this.progressionManager = progressionManager;
            this.domainManager = domainManager;
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
            var Stage = progressionManager.Stage;
            var DomainPoints = progressionManager.DomainPoints;

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
            int y = 550; // near bottom (600 dialog height - 50 px)
            int buttonsPerRow = 2;

            int dialogWidth = 600;
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

            // Composer builds the UI TODO Add DropDown | Fix Stage Cutoff Issue
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

                SingleComposer.AddButton(text, () => OnDomainButton(domain), bounds);
                i++;     
               
            }
                SingleComposer.Compose(); // Finalize
        }


       public void UpdateStats() 
        {
            var Stage = progressionManager.Stage;
            var DomainPoints = progressionManager.DomainPoints;
            string stageName = Stage switch
            {
                1 => "Adapting Organism",
                2 => "Hyper-Adaptive Organism",
                3 => "Paragon of Seraphs",
                _ => "Unknown"
            };

            capi.Logger.Warning("UpdateStats Called!");
            if (SingleComposer != null)
            {
                SingleComposer.GetDynamicText("statstext")
                    .SetNewText($"Stage : {stageName}\nDomainPoints: {DomainPoints}");
            }

            

        }

        bool OnDomainButton(DomainEnum domain)
        {
            var detailGui = new DomainDetailsGUI(capi, domainManager, domain);
            detailGui.TryOpen();
            detailGui.UpdateDomainStats();
            return true;
        }


        public override void OnGuiOpened()
        {
            capi.Logger.Notification("Dialog opened");
        }

        public override void OnGuiClosed()
        {
            capi.Logger.Notification("Dialog closed");
        }

    }
}
