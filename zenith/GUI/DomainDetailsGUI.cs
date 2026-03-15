using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using zenith.Core.Domains;
using static zenith.Core.ZenithBehavior;

namespace zenith.GUI
{
    public class DomainDetailsGUI : GuiDialog
    {
        DomainEnum domain;  // <- the domain this GUI is showing
        DomainManager domainManager;

        public override string ToggleKeyCombinationCode => null;

        public DomainDetailsGUI(ICoreClientAPI capi, DomainManager dm, DomainEnum domain) : base(capi)
        {
            domainManager = dm;
            this.domain = domain;

            SetupDialog();
            domainManager.domains[domain].OnTierUp += (s) => UpdateDomainStats();
            domainManager.domains[domain].DomainMaxed += (s) => UpdateDomainStats();
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
            var sponge = domainManager.domains[domain]; 
            int Tier = sponge.Tier;
            bool Status = sponge.IsMaxed;

            capi.Logger.Warning($"[FLOW] UpdateDomainStatsCalled! | Current Tier : {Tier} | Current Status: {Status}");

            if (SingleComposer != null)
            {
                SingleComposer.GetDynamicText("tiertext")
                    .SetNewText($"Tier: {Tier}\n Maxed: {Status}");

            }
        }
    }
}
