using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace zenith.Core.NetWork
{
    public class Keybinds
    {
        public ZenithNetwork ZenithNetwork { get; private set; }


        public Keybinds(ZenithNetwork zenithNetwork)
        {
            this.ZenithNetwork = zenithNetwork;
            ZenithNetwork = zenithNetwork ?? throw new ArgumentNullException(nameof(zenithNetwork));

        }

        public void WireKeybinds(ICoreClientAPI api )
        {
#pragma warning disable IDE0019

            var player = api.World.Player?.Entity as EntityPlayer;
            if (player != null)


            // Register hotkey once, at client start
            api.Input.RegisterHotKey("opendomain", "Open Organism GUI", GlKeys.G, HotkeyType.GUIOrOtherControls);
            api.Input.SetHotKeyHandler("opendomain", comb =>
            {
                var behavior = player.GetBehavior<ZenithBehavior>();
                if (behavior?.systems?.ZenithGui != null)
                {
                    behavior.systems.ZenithGui.Toggle();
                    return true;
                }
                return false;  
            });


            api.Input.RegisterHotKey("assimilate", "Consume", GlKeys.V, HotkeyType.GUIOrOtherControls); // Use Znetwork
            api.Input.SetHotKeyHandler("assimilate", comb =>
            {
                ZenithNetwork.Request(GlKeys.V);
                return true; // returning false causes double firing.
            });

            api.Input.RegisterHotKey("statselect", "Switch Stat", GlKeys.Keypad9, HotkeyType.GUIOrOtherControls);
            api.Input.SetHotKeyHandler("statselect", comb =>
            {
                ZenithNetwork.Request(GlKeys.Keypad9);
                return true;

            });

            api.Input.RegisterHotKey("increaseoutput", "Increase Stat", GlKeys.AltLeft, HotkeyType.GUIOrOtherControls);
            api.Input.SetHotKeyHandler("increaseoutput", comb =>
            {
                ZenithNetwork.Request(GlKeys.AltLeft);
                return true;
            });

            api.Input.RegisterHotKey("decreaseoutput", "Decrease Stat", GlKeys.AltRight, HotkeyType.GUIOrOtherControls);
            api.Input.SetHotKeyHandler("decreaseoutput", comb =>
            {
                ZenithNetwork.Request(GlKeys.AltLeft);
                return true;
            });
        }
    }
}
