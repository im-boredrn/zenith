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


            // Register hotkey once, at client start
        

            api.Input.RegisterHotKey("assimilate", "Consume", GlKeys.V, HotkeyType.GUIOrOtherControls); // Use Znetwork
            api.Input.SetHotKeyHandler("assimilate", comb =>
            {
                ZenithNetwork.Request(GlKeys.V, false, false);
                return true; // returning false causes double firing.
            });

            api.Input.RegisterHotKey("increaseoutput", "Increase Stat", GlKeys.B, HotkeyType.GUIOrOtherControls);
            api.Input.SetHotKeyHandler("increaseoutput", comb =>
            {
                bool shiftHeld = api.Input.KeyboardKeyState[(int)GlKeys.ShiftLeft] ||
              api.Input.KeyboardKeyState[(int)GlKeys.ShiftRight];

                bool altHeld = api.Input.KeyboardKeyState[(int)GlKeys.AltLeft] ||
                api.Input.KeyboardKeyState[(int)GlKeys.AltRight];

                ZenithNetwork.Request(GlKeys.B, shiftHeld, altHeld);
                return true;
            });

            api.Input.RegisterHotKey("decreaseoutput", "Decrease Stat", GlKeys.N, HotkeyType.GUIOrOtherControls);
            api.Input.SetHotKeyHandler("decreaseoutput", comb =>
            {

                bool shiftHeld = api.Input.KeyboardKeyState[(int)GlKeys.ShiftLeft] ||
               api.Input.KeyboardKeyState[(int)GlKeys.ShiftRight];

                bool altHeld = api.Input.KeyboardKeyState[(int)GlKeys.AltLeft] ||
                api.Input.KeyboardKeyState[(int)GlKeys.AltRight];


                ZenithNetwork.Request(GlKeys.N, shiftHeld, altHeld );


            
                return true;
            });
        }
    }
}
