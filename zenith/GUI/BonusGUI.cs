using Cairo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core;
using zenith.Core.Assimilation;
using zenith.Core.Domains;
using zenith.Core.NetWork;
using zenith.Core.Progression;
using static zenith.Core.Assimilation.StatOutput;
using static zenith.Core.Traits.Traits;
using static zenith.Core.ZenithBehavior;
using StatType = zenith.Core.Assimilation.StatOutput.StatType;
namespace zenith.GUI
{
    public class BonusGUI : GuiDialog
    {
        public static bool DebugMode => ZenithSettings.ZDebugMode;
        public override string ToggleKeyCombinationCode => null;
        private readonly StatOutput statOutput;
        private readonly ZenithNetwork zenithNetwork;
        public BonusGUI(ICoreClientAPI capi, StatOutput statOutput, ZenithNetwork zenithNetwork) : base(capi)
        {

            this.statOutput = statOutput;
            this.zenithNetwork = zenithNetwork; 
            SetupDialog();

           // Log($"[BONUS-DATA]BONUS GUI CREATED {this.GetHashCode()}");
        }

        public void SetupDialog()
        {

            ElementBounds dialogBounds =
    ElementBounds.Fixed(0, 0, 450, 500)
    .WithAlignment(EnumDialogArea.RightTop);

            var bounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);

            SingleComposer = capi.Gui.CreateCompo("Bonuses", dialogBounds)
                .AddShadedDialogBG(ElementBounds.Fill, true)
                .AddDialogTitleBar($"", OnGuiClosed);
         
            var zenith = capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("zenith"); // Method for live updating

            int i = 0;
            int spacing = 50;
      

            foreach (StatType stat in Enum.GetValues<StatType>())
            {

                var y = i * spacing;
                ElementBounds buttonBounds =
                                   ElementBounds.Fixed(10,y, 120, 30)
                                   .WithAlignment(EnumDialogArea.LeftTop);

                var textBounds = ElementBounds.Fixed(100, y + 5, 200, 30)
                    .WithAlignment(EnumDialogArea.CenterTop);

               

                SingleComposer.AddButton($"{stat}", () => OnSelectStat(stat), buttonBounds, EnumButtonStyle.Small)
                    
                    .AddDynamicText(BuildBonusText(stat), CairoFont.WhiteSmallishText(),textBounds, $"{stat} text" );
                i++;

            }

            SingleComposer.Compose();

        }

        public void UpdateBonusStats()
        {
         //   Log("[FLOW] BONUS GUI UPDATE CALLED");
          //  Log($"[BONUS-DATA]BONUS GUI CREATED {this.GetHashCode()}");

            if (!IsOpened()) return;

            
            

                foreach (StatType stat in Enum.GetValues<StatType>())
                {
            //    Log($"[DATA {stat} Updating");
                    SingleComposer.GetDynamicText($"{stat} text")?
                    .SetNewText(BuildBonusText(stat), false, true, false);
                }
                
            
        }

        private bool OnSelectStat(StatType statType)
        {
            zenithNetwork.RequestStat(statType);
            return true;
        }





        private string BuildBonusText(StatType stat)
        {

            var zenith = capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("zenith"); // Method for live updating
                                                                                                // Idk if any better methods exist
            var guiValue = zenith?.GetFloat(ZenithKeys.GUIKeys[stat], 0f);
            var guiOutput = zenith?.GetFloat(ZenithKeys.GOutputKeys[stat], 100f);


            return $"+{guiValue:F0}%| {guiOutput:F0}% Output";
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

      
    }
}