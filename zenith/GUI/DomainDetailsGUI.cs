using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Datastructures;
using zenith.Core.Domains;
using static zenith.Core.ZenithBehavior;

namespace zenith.GUI
{
    public class DomainDetailsGUI : GuiDialog
    {
        DomainEnum domain;  // <- the domain this GUI is showing
        DomainManager domainManager;

        public override string ToggleKeyCombinationCode => null;
        Action<DomainSponge> tierUpHandler;
        Action<DomainSponge> maxHandler;
        Action<DomainSponge> changeHandler;
        public DomainDetailsGUI(ICoreClientAPI capi, DomainManager dm, DomainEnum domain) : base(capi)
        {
            domainManager = dm;
            this.domain = domain;
            var sponge = domainManager.domains[domain];
            tierUpHandler = (s) => UpdateDomainStats();
            maxHandler = (s) => UpdateDomainStats();
            changeHandler = (s) => UpdateDomainStats();

            sponge.OnTierUp += tierUpHandler;
            sponge.DomainMaxed += maxHandler;
            sponge.OnDomainChanged += changeHandler;

            
            SetupDialog();

           
        }

        public void SetupDialog()
        {
            var sponge = domainManager.domains[domain]; // Always the live object
            int Tier = sponge.Tier;
            bool Status = sponge.IsMaxed;

            var bounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);
            SingleComposer = capi.Gui.CreateCompo("DomainDetail", bounds)
                .AddShadedDialogBG(ElementBounds.Fill, true)
                .AddDialogTitleBar($"{domain} Details", OnGuiClosed)
                .AddDynamicText($"Tier: {Tier}\n Maxed: {Status} ", CairoFont.WhiteSmallText(), ElementBounds.Fixed(20, 50, 200, 40), "tiertext")
                .Compose();
        }

        public void UpdateDomainStats()
        {
            if (!IsOpened())
            {
                capi.Logger.Warning("[FLOW] DomainGUI isnt opened Returning");
                    return;
            }
            var sponge = domainManager.domains[domain];
            int Tier = sponge.Tier;
            bool Status = sponge.IsMaxed;
            string newText = $"Tier: {Tier}\n Maxed: {Status}";

            capi.Logger.Warning($"[FLOW] UpdateDomainStatsCalled! Current Tier : {Tier} | Current Status: {Status}");

            if ( SingleComposer != null)
            {
                SingleComposer.GetDynamicText("tiertext")
                    .SetNewText(newText,false,true,false);
            }


        }


        public override void OnGuiOpened()
        {
           
            var sponge = domainManager.domains[domain];

            sponge.OnTierUp += tierUpHandler;
            sponge.DomainMaxed += maxHandler;
            sponge.OnDomainChanged += changeHandler;
            UpdateDomainStats();
        }

        public override void OnGuiClosed()
        {
            var sponge = domainManager.domains[domain];

            sponge.OnTierUp -= tierUpHandler;
            sponge.DomainMaxed -= maxHandler;
            sponge.OnDomainChanged -= changeHandler;

            base.OnGuiClosed(); // I just realized the syntax base most likely means its usual behavior before override im so fucking tired
            this.TryClose();
            this.Dispose(); 
        }
    }
}
