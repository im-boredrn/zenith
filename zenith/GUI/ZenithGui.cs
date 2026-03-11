//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vintagestory.API.Client;

//namespace zenith.GUI
//{
//    internal class ZenithGui : GuiDialog
//    {

//        public override string ToggleKeyCombinationCode => throw new NotImplementedException();
//        public GuiTrackers(ICoreClientAPI capi) : base(capi)
//        {
//            SetupDialog();
//        }


//        private void SetupDialog()
//        {
//            ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog
//                .WithAlignment(EnumDialogArea.RightTop)
//                .WithFixedPadding(10);

//            ElementBounds textBounds = ElementBounds.Fixed(0, 0, 200, 30);

//            SingleComposer = capi.Gui
//                .CreateCompo("trackershud", dialogBounds)
//                .AddStaticText("Kinetic Tier : 0 ", CairoFont.WhiteMediumText(), textBounds, "kineticTier")
//                .AddStaticText("Hemorrhage: 0", CairoFont.WhiteMediumText(), textBounds.BelowCopy(0, 25), "hemorrhage")
//                .Compose();
//        }

//        public void UpdateTrackers(int kineticTier, int hemorrhage)
//        {
//            SingleComposer.GetStaticText("kineticTier")
//                .SetValue($"Kinetic Tier: {kineticTier}");

//            SingleComposer.GetStaticText("hemorrhage")
//                .SetValue($"Hemorrhage: {hemorrhage}");
//        }

//        public override string Pro

//    }
//}
