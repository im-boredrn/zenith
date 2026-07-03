using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Datastructures;
using zenith.Config;
using zenith.Core.Domains;
using static zenith.Core.ZenithBehavior;

namespace zenith.GUI
{
    public class DomainDetailsGUI : GuiDialog
    {
        DomainEnum domain;  // <- the domain this GUI is showing
        readonly DomainManager  domainManager;
        private readonly IDomainInfo domainInfo;
        public static bool DebugMode => ZenithSettings.ZDebugMode;

        public override string ToggleKeyCombinationCode => null;
        public DomainDetailsGUI(ICoreClientAPI capi, DomainManager dm, DomainEnum domain) : base(capi)
        {
            domainManager = dm;
            this.domain = domain;

            domainInfo = domainManager.Domains[domain]; // Important for dictionary lookup

            domainInfo.OnTierUp += (d) => // I need to unsub or event leakage
            {

                UpdateDomainStats();
            };


           // changeHandler = (s) => UpdateDomainStats(); Delete once no bugs are found
       

            SetupDialog();

            // sponge.OnDomainChanged += changeHandler;


        }

        public void SetupDialog()
        {

            ElementBounds buttonBounds =
                ElementBounds.Fixed(0, 0, 120, 30)
                .WithAlignment(EnumDialogArea.RightBottom);

            bool Status = domainInfo.IsDMaxed();
                int Tier = domainInfo.GetTier();
           // float counter = domainInfo.GetCounter();
          //  float threshold = domainInfo.GetThreshold();
            var bounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);


                SingleComposer = capi.Gui.CreateCompo("DomainDetail", bounds)
                    .AddShadedDialogBG(ElementBounds.Fill, true)
                    .AddDialogTitleBar($"{domain} Details", OnGuiClosed)
                    .AddDynamicText($"Tier: {Tier}\n Maxed: {Status}", CairoFont.WhiteSmallText(), ElementBounds.Fixed(20, 50, 200, 40), "tiertext")
                   
                    .Compose();
      
        }

        public void UpdateDomainStats()
        {
            if (!IsOpened())
            {
                Log("[DomainGUI] DomainGUI isnt opened Returning");
                    return;
            }
            var sponge = domainInfo.GetDomain();
            var zenithTree = capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("zenith");

            var domainTree = zenithTree?.GetTreeAttribute(domain.ToString());

            int Tier = domainTree?.GetInt("Tier") ?? 0;
            bool Status = domainTree?.GetBool("Maxed") ?? false;
          //  float Counter = domainTree?.GetFloat("Counter") ?? 0;
          //  int Threshold = domainInfo.GetThreshold();
            string newText = $"Tier: {Tier}\n Maxed: {Status}";

            Log($"[CLIENT CHECK] Tier directly from entity: {Tier}");
           Log($"[FLOW] UpdateDomainStatsCalled! Current Tier : {Tier} | Current Status: {Status}");

            if ( SingleComposer != null)
            {
                SingleComposer.GetDynamicText("tiertext")
                    .SetNewText(newText,false,true,false);
            }
           Log($"[DomainGUI] UpdateDomainStatsFinished! Current Tier : {Tier} | Current Status: {Status}");


        }

     


        public override void OnGuiOpened()
        {

            base.OnGuiOpened(); 
            
            UpdateDomainStats();
        }

        public override void OnGuiClosed()
        {
            var sponge = domainManager.Domains[domain];


            domainInfo.OnTierUp -= (d) => 
            {
                UpdateDomainStats();
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
