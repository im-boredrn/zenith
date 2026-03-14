using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using zenith.Core;

namespace zenith.GUI
{
    public class ZenithGui : GuiDialog
    {

        public override string ToggleKeyCombinationCode => null;

        ProgressionManager progressionManager;

        public ZenithGui(ICoreClientAPI capi, ProgressionManager progressionManager) : base(capi)
        {
            this.progressionManager = progressionManager;
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

            ElementBounds dialogBounds =
     ElementBounds.Fixed(0, 0, 600, 600)
     .WithAlignment(EnumDialogArea.CenterMiddle);



            ElementBounds textBounds =
       ElementBounds.Fixed(0, 30, 250, 30)
       .WithAlignment(EnumDialogArea.CenterTop);

            ElementBounds numberBounds =
                ElementBounds.Fixed(0, 0, 250, 40)
                .WithAlignment(EnumDialogArea.CenterMiddle);

            ElementBounds buttonBounds =
                ElementBounds.Fixed(0, 0, 120, 30)
                .WithAlignment(EnumDialogArea.RightBottom);

            ElementBounds buttonBounds2 = buttonBounds.FlatCopy().FixedUnder(buttonBounds, 5);

            // Composer builds the UI
            SingleComposer = capi.Gui
                .CreateCompo("ExampleDialog", dialogBounds) // Then Chain Elements
                .AddShadedDialogBG(ElementBounds.Fill, true) // Everything is on Top of this
                .AddDialogTitleBar("Domain Control Panel", OnGuiClosed) // makes it Draggable
                .AddDynamicText($"Stage : {Stage}\nDomainPoints: {DomainPoints}", CairoFont.WhiteSmallishText(), numberBounds, "statstext")
                .AddButton("Update Stats", OnClickUpdateStats, buttonBounds)
                .Compose(); // Finalize
        }


       public void UpdateStats() // TODO MAKE THIS CALLED ONSTAGEUP AND TIERUP
        {
            var Stage = progressionManager.Stage;
            var DomainPoints = progressionManager.DomainPoints;

            capi.Logger.Warning("UpdateStats Called!");
            if (SingleComposer != null)
            {
                SingleComposer.GetDynamicText("statstext")
                    .SetNewText($"Stage : {Stage}\nDomainPoints: {DomainPoints}");
            }

        }


        bool OnClickUpdateStats()
        {
            UpdateStats();
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
