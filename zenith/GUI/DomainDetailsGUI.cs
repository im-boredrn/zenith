using System;
using System.Collections.Generic;
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
        public bool DebugMode => ZenithSettings.ZDebugMode;

        public override string ToggleKeyCombinationCode => null;
        readonly Action changeHandler;
        public DomainDetailsGUI(ICoreClientAPI capi, DomainManager dm, DomainEnum domain) : base(capi)
        {
            domainManager = dm;
            this.domain = domain;
            var sponge = domainManager.domains[domain];


           // changeHandler = (s) => UpdateDomainStats();
            domainInfo.OnTierUp += (s) => UpdateDomainStats();

            domainInfo.OnDomainChanged += UpdateDomainStats;
           // sponge.OnDomainChanged += changeHandler;

            domainInfo = sponge;
            
            SetupDialog();

           
        }

        public void SetupDialog()
        {
            var sponge = domainInfo.GetDomain(); // Always the live object
            int Tier = domainInfo.GetTier();
            bool Status = domainInfo.IsDMaxed();

            var bounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);
            SingleComposer = capi.Gui.CreateCompo("DomainDetail", bounds)
                .AddShadedDialogBG(ElementBounds.Fill, true)
                .AddDialogTitleBar($"{domain} Details", OnGuiClosed)
                .AddDynamicText($"Tier: {Tier}\n Maxed: {Status} ", CairoFont.WhiteSmallText(), ElementBounds.Fixed(20, 50, 200, 40), "tiertext")
                .Compose();
        }

        public void UpdateDomainStats( )
        {
            if (!IsOpened())
            {
                Log("[DomainGUI] DomainGUI isnt opened Returning");
                    return;
            }
            var sponge = domainInfo.GetDomain();
            var zenithTree = capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("zenith");

            var domainTree = zenithTree?.GetTreeAttribute(domain.ToString());

            int Tier = domainTree?.GetInt("Tier") ?? -1;
            bool Status = domainTree?.GetBool("Maxed") ?? false;
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
           
            var sponge = domainManager.domains[domain];

            sponge.OnDomainChanged += changeHandler;
            UpdateDomainStats();
        }

        public override void OnGuiClosed()
        {
            var sponge = domainManager.domains[domain];

            sponge.OnDomainChanged -= changeHandler;

            base.OnGuiClosed(); // I just realized the syntax base most likely means its usual behavior before override im so fucking tired
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
