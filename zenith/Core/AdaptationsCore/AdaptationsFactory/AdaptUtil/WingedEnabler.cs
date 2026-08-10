//using System;
//using System.Collections.Generic;
//using System.Text;
//using Vintagestory.API.Client;
//using Vintagestory.API.Common;
//using Vintagestory.GameContent;
//using zenith.Core.Helper;

//namespace zenith.Core.AdaptationsCore.AdaptationsFactory.AdaptUtil
//{
//    public class WingedEnabler : IDisposable

//    {
//        private readonly ICoreClientAPI capi;
//        private readonly EntityPlayer entityPlayer;
//        public WingedEnabler(ICoreClientAPI capi, EntityPlayer player)
//        {
//            this.capi = capi;
//            this.entityPlayer = player;

//            capi.Input.InWorldAction += OnInWorldAction;
//        }

//        private void OnInWorldAction(EnumEntityAction action, bool on, ref EnumHandling handled)
//        {
//            if (action == EnumEntityAction.Jump && on && !entityPlayer.OnGround && HasWings && entityPlayer.Controls.Gliding )
//            {
//                entityPlayer.Pos.Motion.Y += 0.5f;
//            }
//        }

//        public void ToggleWings()
//        {
//            HasWings = !HasWings;
//            if (HasWings)
//            {
//                capi.ShowChatMessage($"Wings Enabled");
//            }
//            else if (!HasWings)
//            {
//                capi.ShowChatMessage($"Wings Disabled");
//            }
//        }

        

//        public void Dispose()
//        {
//            capi.Input.InWorldAction -= OnInWorldAction;
//        }
//        public bool HasWings { get; set; }
//    }
//}
