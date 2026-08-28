using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Datastructures;
using zenith.Config;
using zenith.Core;
using zenith.Core.Domains;
using zenith.Core.Helper;
using static zenith.Core.ZenithBehaviorServer;

namespace zenith.GUI
{
    public class DomainDetailsGUI : GuiDialog
    {
      private readonly  DomainEnum domain;  // <- the domain this GUI is showing
        public static bool DebugMode => ZenithSettings.ZDebugMode;
        private ZenithData ZenithData;

        public override string ToggleKeyCombinationCode => null;
        public DomainDetailsGUI(ICoreClientAPI capi, DomainEnum domain, ZenithData data) : base(capi)
        {
            this.domain = domain;
            ZenithData = data;
 
            SetupDialog();
        }

        public void SetupDialog()
        {

            ElementBounds buttonBounds =
                ElementBounds.Fixed(0, 0, 120, 30)
                .WithAlignment(EnumDialogArea.RightBottom);


            var domains = ZenithData.Tree.GetTreeAttribute("Domains");

            if (domains == null) return;

    
                var domainTree = domains.GetTreeAttribute(domain.ToString());

                if (domainTree == null) return;

                var tier = domainTree.GetInt("Tier", 0);
                var isMaxed = domainTree.GetBool("Maxed", false);

                var bounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);


                SingleComposer = capi.Gui.CreateCompo("DomainDetail", bounds)
                    .AddShadedDialogBG(ElementBounds.Fill, true)
                    .AddDialogTitleBar($"{domain} Details", OnGuiClosed)
                    .AddDynamicText($"Tier: {tier}\n Maxed: {isMaxed}", CairoFont.WhiteSmallText(), ElementBounds.Fixed(20, 50, 200, 40), "tiertext")
                   
                    .Compose();
      
        }

        public void UpdateDomainStats()
        {
            if (!IsOpened())
            {
               Logger.Log(capi.World.Player.Entity, "[DomainGUI] DomainGUI isnt opened Returning");
                    return;
            }

            var domains = ZenithData.Tree.GetTreeAttribute("Domains");

            if (domains == null) return;


            var domainTree = domains.GetTreeAttribute(domain.ToString());

            if (domainTree == null) return;
        

            var tier = domainTree.GetInt("Tier", 0);
            var isMaxed = domainTree.GetBool("Maxed", false);

        
            string newText = $"Tier: {tier}\n Maxed: {isMaxed}";

        //   Logger.Log(capi.World.Player.Entity, $"[CLIENT CHECK] Tier directly from entity: {tier}");
       //  Logger.Log(capi.World.Player.Entity, $"[FLOW] UpdateDomainStatsCalled! Current Tier : {tier} | Current Status: {isMaxed}");

            
                SingleComposer?.GetDynamicText("tiertext")
                    .SetNewText(newText,false,true,false);
            
       //  Logger.Log(capi.World.Player.Entity,$"[DomainGUI] UpdateDomainStatsFinished! Current Tier : {tier} | Current Status: {isMaxed}");

        }

     
        public override void OnGuiOpened()
        {

            base.OnGuiOpened(); 
            
            UpdateDomainStats();
        }

        public override void OnGuiClosed()
        {

            base.OnGuiClosed();
            this.TryClose();
            this.Dispose(); 
        }
    }
}